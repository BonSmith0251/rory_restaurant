// Ported from rudi.screen.Screen and rudi.gameEngine.
public class Screen
{
    public string name = "";
    public bool autoFadeIn = false;
    public float autoFadeInTime = 50;
    public bool autoFadeInSound = false;
    public bool autoFadeInMusic = false;
    public bool ready = false;

    public virtual void OnTouchHit(int arg1, int arg2, int arg3) { }
    public virtual void OnTouchClick(int arg1, int arg2, int arg3) { }
    public virtual void OnTouchFling(int arg1, int arg2, float arg3, float arg4, float arg5, int arg6) { }
    public virtual void OnTouchReleased(int arg1, int arg2, int arg3) { }
    public virtual void OnTouchDragged(int arg1, int arg2, int arg3, int arg4, int arg5) { }
    public virtual void OnTouchLongPress(int arg1, int arg2, int arg3) { }
    public virtual void OnAnyKeyHit() { }
    public virtual void OnKeyHit(int arg1) { }
    public virtual void OnAnyKeyDown() { }
    public virtual void OnKeyDown(int arg1) { }
    public virtual void OnAnyKeyReleased() { }
    public virtual void OnKeyReleased(int arg1) { }
    public virtual void OnMouseHit(int arg1, int arg2, int arg3) { }
    public virtual void OnMouseDown(int arg1, int arg2, int arg3) { }
    public virtual void OnMouseReleased(int arg1, int arg2, int arg3) { }
    public virtual void Kill() { }
    public virtual void Start() { gameEngine.getInstance()._currentScreen = this; }
    public virtual void PreStart() { gameEngine.getInstance()._currentScreen = this; }
    public virtual void PostFadeOut() { Kill(); }
    public virtual void PostFadeIn() { }
    public virtual void Update() { }
    public virtual void Suspend() { }
    public virtual void Resume() { }
    public virtual void Pause() { }
}

public class gameEngine
{
    static gameEngine _instance;
    public Screen _currentScreen;
    public Screen _nextScreen;

    gameEngine() { }

    public static gameEngine getInstance() => _instance ?? (_instance = new gameEngine());
}
