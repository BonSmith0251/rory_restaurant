// Replaces the AIR document class "Main": reads settings, initialises Global and starts Starling(Game).
// Runs automatically after the first scene loads, so any scene (even an empty one) starts the game.
using Starlite;

public static class GameBootstrap
{
    static bool sStarted;

    [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (sStarted) return;
        // The native rework's scenes (Assets/Rory/Scenes) run without the legacy Starling game; starting it there
        // would draw the old game over them.
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().path.StartsWith("Assets/Rory/")) return;
        sStarted = true;
        UnityEngine.Application.targetFrameRate = 60;
        if (UnityEngine.Camera.main == null)
        {
            var cam = new UnityEngine.GameObject("Main Camera").AddComponent<UnityEngine.Camera>();
            cam.clearFlags = UnityEngine.CameraClearFlags.SolidColor;
            cam.backgroundColor = UnityEngine.Color.black;
            cam.orthographic = true;
            UnityEngine.Object.DontDestroyOnLoad(cam.gameObject);
        }
        Main.init();
    }
}

/// <summary>Port of Main.init (the stage-scaling and Starling-viewport parts are handled by Starlite).</summary>
public static class Main
{
    public static Global _global;

    public static void init()
    {
        Settings.initSetting();
        bool hd = true, embed = false, encrypt = false;
        if (Settings.textureSize == "hd") hd = true;
        else if (Settings.textureSize == "sd") hd = false;
        else if (Settings.textureSize == "all") hd = Starling.Stage.stageWidth > 1024;
        if (Settings.embedAssets == "true") embed = true;
        if (Settings.encryptAssets == "true") encrypt = true;
        Starling.multitouchEnabled = true;
        Starling.handleLostContext = true;
        _global = Global.getInstance();
        _global._stage = Starling.Stage;
        _global.init(hd, embed, encrypt);
        _global._Starling = new Starling(typeof(Game), _global._stage, new Rectangle(0, 0, 1024, 600));
        Starling.current.start();
    }
}
