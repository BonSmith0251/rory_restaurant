using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UObject = UnityEngine.Object;

namespace Starlite
{
    public class Stage : DisplayObjectContainer
    {
        public int stageWidth = 1024, stageHeight = 600;
        public uint color;
        public readonly NativeWindow nativeWindow = new NativeWindow();
        public string scaleMode, align;

        internal Stage(int w, int h) { stageWidth = w; stageHeight = h; name = "Stage"; }

        public override DisplayObject hitTest(Point localPoint, bool forTouch = false)
        {
            if (forTouch && (!visible || !touchable)) return null;
            if (localPoint.x < 0 || localPoint.x > stageWidth || localPoint.y < 0 || localPoint.y > stageHeight) return null;
            return base.hitTest(localPoint, forTouch) ?? this;
        }

        /// <summary>flash StageDisplayState: "normal", "fullScreen", "fullScreenInteractive".</summary>
        public string displayState
        {
            get => UnityEngine.Screen.fullScreen ? StageDisplayState.FULL_SCREEN_INTERACTIVE : StageDisplayState.NORMAL;
            set => UnityEngine.Screen.fullScreen = value != StageDisplayState.NORMAL;
        }

        public float frameRate { get => Application.targetFrameRate; set => Application.targetFrameRate = (int)value; }
    }

    public static class StageDisplayState
    {
        public const string NORMAL = "normal", FULL_SCREEN = "fullScreen", FULL_SCREEN_INTERACTIVE = "fullScreenInteractive";
    }

    public class NativeWindow
    {
        public void minimize() { }
        public void activate() { }
        public void close() => Application.Quit();
    }

    public interface IAnimatable
    {
        void advanceTime(float time);
    }

    public class Starling
    {
        public static Starling current { get; private set; }
        public static bool multitouchEnabled, handleLostContext;
        public static Juggler juggler => current?.mJuggler ?? sBootJuggler;
        static readonly Juggler sBootJuggler = new Juggler();

        internal static Transform Limbo => Host.limbo;
        internal static UnityEngine.Texture WhiteTexture => Texture2D.whiteTexture;
        static Material sAdditive;
        internal static Material AdditiveMaterial
        {
            get
            {
                if (sAdditive == null)
                {
                    var sh = Shader.Find("Starlite/UIAdditive");
                    sAdditive = sh != null ? new Material(sh) : null;
                }
                return sAdditive;
            }
        }

        internal static StarliteHost Host
        {
            get
            {
                if (StarliteHost.Instance == null) StarliteHost.Create(1024, 600);
                return StarliteHost.Instance;
            }
        }

        readonly Juggler mJuggler = sBootJuggler;
        public readonly Stage stage;
        public DisplayObject root { get; private set; }
        public bool isStarted { get; private set; }
        public Rectangle viewPort = new Rectangle(0, 0, 1024, 600);
        public bool showStats;
        public float contentScaleFactor => 1;

        public Starling(Type rootClass, object nativeStage = null, Rectangle viewPort = null, object stage3D = null,
            string renderMode = "auto", string profile = "auto")
        {
            current = this;
            stage = Host.stage;
            if (viewPort != null) this.viewPort = viewPort;
            root = (DisplayObject)Activator.CreateInstance(rootClass);
            stage.addChild(root);
            stage.dispatchEventWith(Event.ROOT_CREATED);
        }

        public void start() { isStarted = true; Host.paused = false; }
        public void stop(bool suspendRendering = false) { isStarted = false; Host.paused = true; }

        /// <summary>The stage without needing a Starling instance (available as soon as the host exists).</summary>
        public static Stage Stage => Host.stage;

        internal static void RegisterEnterFrame(DisplayObject o) => Host.enterFrame.Add(o);
        internal static void UnregisterEnterFrame(DisplayObject o) { if (StarliteHost.Instance != null) StarliteHost.Instance.enterFrame.Remove(o); }

        static float sLastUnload;
        internal static void RequestUnloadUnused()
        {
            if (Time.realtimeSinceStartup - sLastUnload < 5) return;
            sLastUnload = Time.realtimeSinceStartup;
            Resources.UnloadUnusedAssets();
        }

        /// <summary>Seconds since startup in ms like flash.utils.getTimer().</summary>
        public static int getTimer() => (int)(Time.realtimeSinceStartup * 1000);
    }

    /// <summary>MonoBehaviour driving the frame loop, letterboxing and input.</summary>
    public class StarliteHost : MonoBehaviour
    {
        internal static StarliteHost Instance;
        internal Stage stage;
        internal Transform limbo;
        internal RectTransform stageRt;
        internal readonly HashSet<DisplayObject> enterFrame = new HashSet<DisplayObject>();
        internal bool paused;
        Canvas mCanvas;
        float mWidth, mHeight;

        // touch state
        readonly Touch mTouch = new Touch(0);
        DisplayObject mHoverTarget;
        bool mDown;
        Vector2 mLastMouse = new Vector2(float.NaN, float.NaN);
        float mLastTapTime = -10;
        Vector2 mLastTapPos;

        internal static StarliteHost Create(int w, int h)
        {
            var root = new GameObject("Starlite");
            DontDestroyOnLoad(root);
            var host = root.AddComponent<StarliteHost>();
            host.mWidth = w;
            host.mHeight = h;
            host.mCanvas = root.AddComponent<Canvas>();
            host.mCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            host.mCanvas.pixelPerfect = false;

            var bg = new GameObject("Letterbox", typeof(RectTransform));
            var bgRt = (RectTransform)bg.transform;
            bgRt.SetParent(root.transform, false);
            bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one; bgRt.offsetMin = bgRt.offsetMax = Vector2.zero;
            var bgImg = bg.AddComponent<RawImage>();
            bgImg.color = UnityEngine.Color.black;
            bgImg.raycastTarget = false;

            var limboGo = new GameObject("Limbo", typeof(RectTransform));
            limboGo.transform.SetParent(root.transform, false);
            limboGo.SetActive(false);
            host.limbo = limboGo.transform;
            Instance = host; // must be set before any DisplayObject (the stage) is constructed

            host.stage = new Stage(w, h);
            host.stageRt = host.stage.rt;
            host.stageRt.SetParent(root.transform, false);
            host.stageRt.anchorMin = host.stageRt.anchorMax = new Vector2(0.5f, 0.5f);
            host.stageRt.pivot = new Vector2(0, 1);
            // like the Flash stage, nothing outside 1024x600 is drawn (off-stage objects stay hidden in the letterbox)
            host.stageRt.sizeDelta = new Vector2(w, h);
            host.stage.go.AddComponent<RectMask2D>();
            host.LayoutStage();
            return host;
        }

        void LayoutStage()
        {
            float s = Mathf.Min(UnityEngine.Screen.width / mWidth, UnityEngine.Screen.height / mHeight);
            stageRt.localScale = new Vector3(s, s, 1);
            stageRt.anchoredPosition = new Vector2(-mWidth * s / 2, mHeight * s / 2);
        }

        int mLastW, mLastH;

        void Update()
        {
            if (UnityEngine.Screen.width != mLastW || UnityEngine.Screen.height != mLastH)
            {
                mLastW = UnityEngine.Screen.width; mLastH = UnityEngine.Screen.height;
                LayoutStage();
                stage.dispatchEvent(new ResizeEvent(Event.RESIZE, mLastW, mLastH));
            }
            float dt = Mathf.Min(Time.unscaledDeltaTime, 0.1f);
            ProcessKeyboard();
            if (paused) return;
            ProcessTouches();
            Starling.juggler.advanceTime(dt);
            TweenEngine.Advance(dt);
            foreach (var sp in new List<Sprite>(Sprite.Clipped)) sp.SyncClip();
            var e = new EnterFrameEvent(Event.ENTER_FRAME, dt);
            stage.InvokeEvent(e);
            foreach (var o in new List<DisplayObject>(enterFrame))
            {
                if (o.isDisposed) { enterFrame.Remove(o); continue; }
                if (!o.hasEventListener(Event.ENTER_FRAME)) { enterFrame.Remove(o); continue; }
                if (o != stage && o.stage != null) o.InvokeEvent(e);
            }
        }

        void OnApplicationFocus(bool focus) => stage?.dispatchEventWith(focus ? Event.ACTIVATE : Event.DEACTIVATE);

        Point MouseToStage(Vector2 screen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(stageRt, screen, null, out var local);
            return new Point(local.x, -local.y);
        }

        void Dispatch(DisplayObject target, string phase, Point p)
        {
            mTouch.previousGlobalX = mTouch.globalX;
            mTouch.previousGlobalY = mTouch.globalY;
            mTouch.globalX = p.x;
            mTouch.globalY = p.y;
            mTouch.phase = phase;
            mTouch.target = target;
            mTouch.timestamp = Time.realtimeSinceStartup;
            if (target == null) return;
            var ev = new TouchEvent(TouchEvent.TOUCH, new List<Touch> { mTouch.clone() }, Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.LeftControl));
            target.dispatchEvent(ev);
        }

        void ProcessTouches()
        {
            Vector2 mouse = Input.mousePosition;
            var p = MouseToStage(mouse);
            bool moved = mouse != mLastMouse;
            mLastMouse = mouse;

            if (Input.GetMouseButtonDown(0))
            {
                mHoverTarget = null;
                var target = stage.hitTest(p, true);
                float now = Time.realtimeSinceStartup;
                mTouch.tapCount = (now - mLastTapTime < 0.3f && Vector2.Distance(mLastTapPos, mouse) < 20) ? mTouch.tapCount + 1 : 1;
                mLastTapTime = now;
                mLastTapPos = mouse;
                mTouch.globalX = p.x; mTouch.globalY = p.y;
                mDown = true;
                Dispatch(target, TouchPhase.BEGAN, p);
                return;
            }
            if (mDown)
            {
                var target = mTouch.target;
                if (target != null && target.isDisposed) target = null;
                if (Input.GetMouseButtonUp(0) || !Input.GetMouseButton(0))
                {
                    mDown = false;
                    Dispatch(target, TouchPhase.ENDED, p);
                    mHoverTarget = null;
                }
                else if (moved)
                {
                    Dispatch(target, TouchPhase.MOVED, p);
                }
                return;
            }
            if (moved)
            {
                var target = stage.hitTest(p, true);
                if (target != mHoverTarget && mHoverTarget != null && !mHoverTarget.isDisposed)
                {
                    // tell the previous target the pointer left it (getTouch(old) returns null for it)
                    mTouch.globalX = p.x; mTouch.globalY = p.y; mTouch.phase = TouchPhase.HOVER; mTouch.target = target;
                    mHoverTarget.dispatchEvent(new TouchEvent(TouchEvent.TOUCH, new List<Touch> { mTouch.clone() }));
                }
                mHoverTarget = target;
                Dispatch(target, TouchPhase.HOVER, p);
            }
        }

        static readonly (KeyCode key, uint code)[] KeyMap =
        {
            (KeyCode.Escape, Keyboard.ESCAPE), (KeyCode.Return, Keyboard.ENTER), (KeyCode.KeypadEnter, Keyboard.ENTER),
            (KeyCode.Space, Keyboard.SPACE), (KeyCode.Backspace, Keyboard.BACKSPACE), (KeyCode.Tab, Keyboard.TAB),
            (KeyCode.LeftArrow, Keyboard.LEFT), (KeyCode.RightArrow, Keyboard.RIGHT), (KeyCode.UpArrow, Keyboard.UP),
            (KeyCode.DownArrow, Keyboard.DOWN), (KeyCode.Delete, Keyboard.DELETE),
        };

        void ProcessKeyboard()
        {
            if (!Input.anyKeyDown && !Input.anyKey) return;
            foreach (var (key, code) in KeyMap)
            {
                if (Input.GetKeyDown(key)) stage.InvokeEvent(new KeyboardEvent(KeyboardEvent.KEY_DOWN, 0, code));
                if (Input.GetKeyUp(key)) stage.InvokeEvent(new KeyboardEvent(KeyboardEvent.KEY_UP, 0, code));
            }
            foreach (char ch in Input.inputString)
            {
                if (ch == '\b' || ch == '\n' || ch == '\r') continue;
                uint code = char.ToUpperInvariant(ch);
                stage.InvokeEvent(new KeyboardEvent(KeyboardEvent.KEY_DOWN, ch, code));
            }
        }
    }
}
