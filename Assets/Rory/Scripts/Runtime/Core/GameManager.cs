using Rory.Audio;
using Rory.Data;
using Rory.Save;
using UnityEngine;

namespace Rory
{
    /// <summary>
    /// Persistent root of the game: saves, session state, audio and scene flow.
    /// Created automatically before the first scene loads, so every scene can be played directly in the editor.
    /// A prefab at Resources/Managers is used when present (to tweak the fade or audio in the Inspector).
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public SaveSystem Saves { get; private set; }
        public GameSession Session { get; private set; }
        public GameDatabase Database => GameDatabase.Instance;
        public AudioManager Audio { get; private set; }
        public SceneFlow Flow { get; private set; }

        /// <summary>The selected player (null on the title screen before a player is chosen).</summary>
        public static PlayerProfile Player => Instance != null ? Instance.Saves.Current : null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap()
        {
            if (Instance != null) return;
            // The legacy Starling port (Assets/Scenes/Main.unity) runs without these managers.
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Main") return;
            var prefab = Resources.Load<GameManager>("Managers");
            var gm = prefab != null ? Instantiate(prefab) : new GameObject("Managers").AddComponent<GameManager>();
            gm.name = "Managers";
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;

            Saves = new SaveSystem();
            Saves.LoadAll();
            Session = new GameSession();

            // The one AudioListener lives here, so scene cameras don't need one.
            if (GetComponentInChildren<AudioListener>() == null) gameObject.AddComponent<AudioListener>();
            Audio = GetComponentInChildren<AudioManager>();
            if (Audio == null) Audio = gameObject.AddComponent<AudioManager>();
            Flow = GetComponentInChildren<SceneFlow>();
            if (Flow == null) Flow = gameObject.AddComponent<SceneFlow>();
            Audio.MuteMusic = Saves.Settings.muteMusic;
            Audio.MuteSound = Saves.Settings.muteSound;
            Audio.MuteChanged += () =>
            {
                Saves.Settings.muteMusic = Audio.MuteMusic;
                Saves.Settings.muteSound = Audio.MuteSound;
                Saves.SaveSettings();
            };
        }

        /// <summary>Switches to another save folder and reloads it (tests use a temporary folder).</summary>
        public void UseSaveFolder(string folder)
        {
            Saves = new SaveSystem(folder);
            Saves.LoadAll();
            Session = new GameSession();
        }

        void OnApplicationPause(bool paused)
        {
            if (paused) Saves?.Save();
        }

        void OnApplicationQuit() => Saves?.Save();
    }
}
