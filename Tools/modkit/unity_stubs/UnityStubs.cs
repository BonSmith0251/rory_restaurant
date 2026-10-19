// "FakeUnity": a functional stand-in for the subset of UnityEngine used by Starlite and the game port.
// Used for (1) compile checks without Unity and (2) headless play tests: transforms, UI components, Resources
// loading from a project folder, simulated input/time, and a software renderer for screenshots.
// Signatures mirror Unity 2022.3. NOT part of the Unity project.
#pragma warning disable CS0626, CS0067, CS0649, CS0414, CS0660, CS0661
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnityEngine
{
    public class MissingReferenceException : Exception { public MissingReferenceException(string m) : base(m) { } }

    public class Object
    {
        internal bool destroyed;
        internal void Chk() { if (destroyed) throw new MissingReferenceException("The object of type '" + GetType().FullName + "' has been destroyed but you are still trying to access it."); }
        public string name { get; set; }
        public static void Destroy(Object obj, float t = 0)
        {
            if (ReferenceEquals(obj, null)) return;
            if (obj is GameObject g) g.DestroyRecursive();
            else if (obj is Component c) { c.destroyed = true; c.gameObject?.RemoveComponent(c); }
            else obj.destroyed = true;
        }
        public static void DestroyImmediate(Object obj) => Destroy(obj);
        public static void DontDestroyOnLoad(Object target) { }
        public static T Instantiate<T>(T original) where T : Object => original;
        public static T FindObjectOfType<T>() where T : Object => null;
        public static implicit operator bool(Object o) => !ReferenceEquals(o, null) && !o.destroyed;
        public static bool operator ==(Object a, Object b)
        {
            bool an = ReferenceEquals(a, null) || a.destroyed, bn = ReferenceEquals(b, null) || b.destroyed;
            if (an || bn) return an && bn;
            return ReferenceEquals(a, b);
        }
        public static bool operator !=(Object a, Object b) => !(a == b);
        public override bool Equals(object o) => ReferenceEquals(this, o);
        public override int GetHashCode() => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);
    }

    public class Component : Object
    {
        internal GameObject mGo;
        public GameObject gameObject => mGo;
        public Transform transform => mGo?.transform;
        public string tag { get; set; }
        public T GetComponent<T>() => mGo != null ? mGo.GetComponent<T>() : default;
        public T AddComponent<T>() where T : Component => mGo?.AddComponent<T>();
    }

    public class Behaviour : Component
    {
        bool mEnabled = true;
        public bool enabled { get => mEnabled; set => mEnabled = value; }
        public bool isActiveAndEnabled => mEnabled && mGo != null && mGo.activeInHierarchy;
    }

    public class MonoBehaviour : Behaviour
    {
        public Coroutine StartCoroutine(IEnumerator routine) => null;
        public void StopAllCoroutines() { }
        public void Invoke(string methodName, float time) { }
    }

    public sealed class Coroutine { }

    public sealed class GameObject : Object
    {
        internal static readonly List<GameObject> All = new List<GameObject>();
        internal readonly List<Component> components = new List<Component>();
        Transform mTransform;
        bool mActive = true;

        public GameObject() : this("GameObject") { }
        public GameObject(string name) : this(name, new Type[0]) { }
        public GameObject(string name, params Type[] comps)
        {
            this.name = name;
            bool rect = comps.Contains(typeof(RectTransform));
            mTransform = rect ? new RectTransform() : new Transform();
            mTransform.mGo = this;
            components.Add(mTransform);
            foreach (var t in comps) if (t != typeof(RectTransform) && t != typeof(Transform)) AddComponent(t);
            All.Add(this);
        }

        public Transform transform => mTransform;
        public bool activeSelf => mActive;
        public bool activeInHierarchy => mActive && (mTransform.parent == null || mTransform.parent.gameObject.activeInHierarchy);
        public string tag { get; set; }
        public int layer { get; set; }
        public void SetActive(bool value) { Chk(); mActive = value; }

        public T AddComponent<T>() where T : Component => (T)AddComponent(typeof(T));

        public Component AddComponent(Type t)
        {
            bool needsRect = t == typeof(Canvas) || t == typeof(CanvasGroup) || typeof(UnityEngine.UI.Graphic).IsAssignableFrom(t) || t == typeof(UnityEngine.UI.RectMask2D);
            if (needsRect && !(mTransform is RectTransform)) AddComponent(typeof(RectTransform)); // Unity converts the Transform
            if (t == typeof(RectTransform) && !(mTransform is RectTransform))
            {
                var rt = new RectTransform { mGo = this };
                rt.CopyFrom(mTransform);
                components[0] = rt;
                mTransform = rt;
                return rt;
            }
            var c = (Component)Activator.CreateInstance(t, true);
            c.mGo = this;
            components.Add(c);
            FakeUnity.OnComponentAdded(c);
            return c;
        }

        internal void RemoveComponent(Component c) => components.Remove(c);

        public T GetComponent<T>()
        {
            foreach (var c in components) if (c is T t && !c.destroyed) return t;
            return default;
        }

        internal void DestroyRecursive()
        {
            if (destroyed) return;
            destroyed = true;
            foreach (var c in components) c.destroyed = true;
            foreach (var ch in mTransform.children.ToArray()) ch.gameObject.DestroyRecursive();
            mTransform.SetParent(null);
            All.Remove(this);
        }

        public static GameObject Find(string name) => All.FirstOrDefault(g => g.name == name);
    }

    public class Transform : Component, IEnumerable
    {
        internal Transform mParent;
        internal readonly List<Transform> children = new List<Transform>();
        Vector3 _f_localPosition; public Vector3 localPosition { get => _f_localPosition; set { Chk(); _f_localPosition = value; } }
        Vector3 _f_localScale = Vector3.one; public Vector3 localScale { get => _f_localScale; set { Chk(); _f_localScale = value; } }
        Vector3 _f_localEulerAngles; public Vector3 localEulerAngles { get => _f_localEulerAngles; set { Chk(); _f_localEulerAngles = value; } }
        public Quaternion localRotation { get; set; }
        public Vector3 position { get; set; }
        public Transform parent { get => mParent; set => SetParent(value); }
        public int childCount => children.Count;

        internal virtual void CopyFrom(Transform t)
        {
            localPosition = t.localPosition; localScale = t.localScale; localEulerAngles = t.localEulerAngles;
            mParent = t.mParent;
            if (mParent != null) { int i = mParent.children.IndexOf(t); if (i >= 0) mParent.children[i] = this; }
            children.AddRange(t.children);
            foreach (var c in children) c.mParent = this;
        }

        public void SetParent(Transform p) => SetParent(p, true);
        public void SetParent(Transform p, bool worldPositionStays)
        {
            mParent?.children.Remove(this);
            mParent = p;
            p?.children.Add(this);
        }

        public void SetSiblingIndex(int index)
        {
            if (mParent == null) return;
            mParent.children.Remove(this);
            index = Math.Max(0, Math.Min(index, mParent.children.Count));
            mParent.children.Insert(index, this);
        }

        public int GetSiblingIndex() => mParent?.children.IndexOf(this) ?? 0;
        public void SetAsLastSibling() => SetSiblingIndex(int.MaxValue);
        public void SetAsFirstSibling() => SetSiblingIndex(0);
        public Transform GetChild(int index) => children[index];
        public Vector3 InverseTransformPoint(Vector3 p) => p;
        public Vector3 TransformPoint(Vector3 p) => p;
        public IEnumerator GetEnumerator() => children.ToList().GetEnumerator();
    }

    public sealed class RectTransform : Transform
    {
        Vector2 _f_anchorMin = new Vector2(0.5f, 0.5f); public Vector2 anchorMin { get => _f_anchorMin; set { Chk(); _f_anchorMin = value; } }
        Vector2 _f_anchorMax = new Vector2(0.5f, 0.5f); public Vector2 anchorMax { get => _f_anchorMax; set { Chk(); _f_anchorMax = value; } }
        Vector2 _f_pivot = new Vector2(0.5f, 0.5f); public Vector2 pivot { get => _f_pivot; set { Chk(); _f_pivot = value; } }
        Vector2 _f_sizeDelta = new Vector2(100, 100); public Vector2 sizeDelta { get => _f_sizeDelta; set { Chk(); _f_sizeDelta = value; } }
        Vector2 _f_anchoredPosition; public Vector2 anchoredPosition { get => _f_anchoredPosition; set { Chk(); _f_anchoredPosition = value; } }
        public Vector2 offsetMin { get => Vector2.zero; set { } }
        public Vector2 offsetMax { get => Vector2.zero; set { } }

        /// <summary>Rect size: sizeDelta plus the anchor span inside the parent rect.</summary>
        public Vector2 Size
        {
            get
            {
                if (mParent == null && mGo != null && mGo.GetComponent<Canvas>() != null) return new Vector2(Screen.width, Screen.height);
                var ps = mParent is RectTransform pr ? pr.Size : new Vector2(Screen.width, Screen.height);
                return new Vector2(sizeDelta.x + (anchorMax.x - anchorMin.x) * ps.x, sizeDelta.y + (anchorMax.y - anchorMin.y) * ps.y);
            }
        }

        public Rect rect { get { var s = Size; return new Rect(-pivot.x * s.x, -pivot.y * s.y, s.x, s.y); } }
        public void SetSizeWithCurrentAnchors(int axis, float size) { }

        /// <summary>Local-to-screen matrix (screen space, origin bottom-left, like ScreenSpaceOverlay).</summary>
        internal M2 World
        {
            get
            {
                // canvas root: fills the screen, pivot at its centre
                if (mParent == null) return M2.Translate(Screen.width * 0.5f, Screen.height * 0.5f);
                M2 parentW;
                Rect pRect;
                if (mParent is RectTransform pr) { parentW = pr.World; pRect = pr.rect; }
                else { parentW = M2.Translate(Screen.width * 0.5f, Screen.height * 0.5f); pRect = new Rect(-Screen.width / 2f, -Screen.height / 2f, Screen.width, Screen.height); }
                var am = new Vector2((anchorMin.x + anchorMax.x) / 2, (anchorMin.y + anchorMax.y) / 2);
                var anchorRef = new Vector2(pRect.x + am.x * pRect.width, pRect.y + am.y * pRect.height);
                var l = M2.Translate(anchorRef.x + anchoredPosition.x, anchorRef.y + anchoredPosition.y)
                    * M2.Rotate(localEulerAngles.z * Mathf.Deg2Rad) * M2.Scale(localScale.x, localScale.y);
                return parentW * l;
            }
        }

        internal override void CopyFrom(Transform t) { base.CopyFrom(t); }
    }

    /// <summary>2D affine matrix helper for the fake renderer (column vectors).</summary>
    public struct M2
    {
        public float a, b, c, d, tx, ty; // x' = a x + c y + tx ; y' = b x + d y + ty
        public static M2 Identity => new M2 { a = 1, d = 1 };
        public static M2 Translate(float x, float y) => new M2 { a = 1, d = 1, tx = x, ty = y };
        public static M2 Scale(float x, float y) => new M2 { a = x, d = y };
        public static M2 Rotate(float r) { float cs = (float)Math.Cos(r), sn = (float)Math.Sin(r); return new M2 { a = cs, b = sn, c = -sn, d = cs }; }
        public static M2 operator *(M2 m, M2 n) => new M2
        {
            a = m.a * n.a + m.c * n.b, b = m.b * n.a + m.d * n.b,
            c = m.a * n.c + m.c * n.d, d = m.b * n.c + m.d * n.d,
            tx = m.a * n.tx + m.c * n.ty + m.tx, ty = m.b * n.tx + m.d * n.ty + m.ty
        };
        public Vector2 Apply(Vector2 p) => new Vector2(a * p.x + c * p.y + tx, b * p.x + d * p.y + ty);
        public M2 Inverse()
        {
            float det = a * d - b * c;
            if (Math.Abs(det) < 1e-12f) return Identity;
            var r = new M2 { a = d / det, b = -b / det, c = -c / det, d = a / det };
            r.tx = -(r.a * tx + r.c * ty); r.ty = -(r.b * tx + r.d * ty);
            return r;
        }
    }

    public struct Quaternion
    {
        public float x, y, z, w;
        public static Quaternion identity => default;
        public static Quaternion Euler(float x, float y, float z) => default;
    }

    public struct Vector2
    {
        public float x, y;
        public Vector2(float x, float y) { this.x = x; this.y = y; }
        public static Vector2 zero => default;
        public static Vector2 one => new Vector2(1, 1);
        public float magnitude => (float)Math.Sqrt(x * x + y * y);
        public static float Distance(Vector2 a, Vector2 b) => (a - b).magnitude;
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
        public static Vector2 operator *(Vector2 a, float d) => new Vector2(a.x * d, a.y * d);
        public static bool operator ==(Vector2 a, Vector2 b) => a.x == b.x && a.y == b.y;
        public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);
        public static implicit operator Vector2(Vector3 v) => new Vector2(v.x, v.y);
        public static implicit operator Vector3(Vector2 v) => new Vector3(v.x, v.y, 0);
        public override bool Equals(object o) => o is Vector2 v && v == this;
        public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode();
        public override string ToString() => $"({x}, {y})";
    }

    public struct Vector3
    {
        public float x, y, z;
        public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        public Vector3(float x, float y) { this.x = x; this.y = y; z = 0; }
        public static Vector3 zero => default;
        public static Vector3 one => new Vector3(1, 1, 1);
    }

    public struct Rect
    {
        public float x, y, width, height;
        public Rect(float x, float y, float width, float height) { this.x = x; this.y = y; this.width = width; this.height = height; }
        public float xMin => x; public float yMin => y; public float xMax => x + width; public float yMax => y + height;
    }

    public struct Color
    {
        public float r, g, b, a;
        public Color(float r, float g, float b, float a = 1) { this.r = r; this.g = g; this.b = b; this.a = a; }
        public static Color black => new Color(0, 0, 0, 1);
        public static Color white => new Color(1, 1, 1, 1);
        public static Color clear => new Color(0, 0, 0, 0);
    }

    public struct Color32
    {
        public byte r, g, b, a;
        public Color32(byte r, byte g, byte b, byte a) { this.r = r; this.g = g; this.b = b; this.a = a; }
    }

    public static class Mathf
    {
        public const float PI = 3.14159274f, Deg2Rad = PI / 180f, Rad2Deg = 180f / PI, Infinity = float.PositiveInfinity, Epsilon = float.Epsilon;
        public static float Sin(float f) => (float)Math.Sin(f);
        public static float Cos(float f) => (float)Math.Cos(f);
        public static float Tan(float f) => (float)Math.Tan(f);
        public static float Asin(float f) => (float)Math.Asin(f);
        public static float Acos(float f) => (float)Math.Acos(f);
        public static float Atan(float f) => (float)Math.Atan(f);
        public static float Atan2(float y, float x) => (float)Math.Atan2(y, x);
        public static float Sqrt(float f) => (float)Math.Sqrt(f);
        public static float Abs(float f) => Math.Abs(f);
        public static int Abs(int v) => Math.Abs(v);
        public static float Min(float a, float b) => a < b ? a : b;
        public static int Min(int a, int b) => a < b ? a : b;
        public static float Min(params float[] v) => v.Min();
        public static float Max(float a, float b) => a > b ? a : b;
        public static int Max(int a, int b) => a > b ? a : b;
        public static float Max(params float[] v) => v.Max();
        public static float Pow(float f, float p) => (float)Math.Pow(f, p);
        public static float Exp(float p) => (float)Math.Exp(p);
        public static float Log(float f) => (float)Math.Log(f);
        public static float Ceil(float f) => (float)Math.Ceiling(f);
        public static float Floor(float f) => (float)Math.Floor(f);
        public static float Round(float f) => (float)Math.Round(f, MidpointRounding.ToEven);
        public static int CeilToInt(float f) => (int)Math.Ceiling(f);
        public static int FloorToInt(float f) => (int)Math.Floor(f);
        public static int RoundToInt(float f) => (int)Math.Round(f, MidpointRounding.ToEven);
        public static float Sign(float f) => f >= 0 ? 1 : -1;
        public static float Clamp(float v, float min, float max) => v < min ? min : v > max ? max : v;
        public static int Clamp(int v, int min, int max) => v < min ? min : v > max ? max : v;
        public static float Clamp01(float v) => Clamp(v, 0, 1);
        public static float Lerp(float a, float b, float t) => a + (b - a) * Clamp01(t);
        public static bool Approximately(float a, float b) => Math.Abs(a - b) < 1e-6f;
        public static float Repeat(float t, float l) => t - (float)Math.Floor(t / l) * l;
    }

    public static class Random
    {
        static readonly System.Random R = new System.Random(1);
        public static float value => (float)R.NextDouble();
        public static float Range(float min, float max) => min + (float)R.NextDouble() * (max - min);
        public static int Range(int min, int max) => R.Next(min, max);
        public static void InitState(int seed) { }
    }

    public static class Time
    {
        public static float deltaTime => FakeUnity.DeltaTime;
        public static float unscaledDeltaTime => FakeUnity.DeltaTime;
        public static float time => FakeUnity.Now;
        public static float unscaledTime => FakeUnity.Now;
        public static float realtimeSinceStartup => FakeUnity.Now;
        public static float timeScale { get; set; } = 1;
        public static int frameCount => FakeUnity.Frame;
    }

    public static class Screen
    {
        public static int width => FakeUnity.ScreenWidth;
        public static int height => FakeUnity.ScreenHeight;
        public static bool fullScreen { get; set; }
        public static FullScreenMode fullScreenMode { get; set; }
        public static void SetResolution(int w, int h, bool full) { }
        public static void SetResolution(int w, int h, FullScreenMode mode) { }
    }

    public enum FullScreenMode { ExclusiveFullScreen, FullScreenWindow, MaximizedWindow, Windowed }

    public static class Input
    {
        public static Vector3 mousePosition => FakeUnity.Mouse;
        public static bool anyKey => FakeUnity.KeysHeld.Count > 0;
        public static bool anyKeyDown => FakeUnity.KeysDown.Count > 0 || FakeUnity.InputString.Length > 0;
        public static string inputString => FakeUnity.InputString;
        public static bool GetMouseButton(int b) => b == 0 && FakeUnity.MouseHeld;
        public static bool GetMouseButtonDown(int b) => b == 0 && FakeUnity.MouseDown;
        public static bool GetMouseButtonUp(int b) => b == 0 && FakeUnity.MouseUp;
        public static bool GetKey(KeyCode k) => FakeUnity.KeysHeld.Contains(k);
        public static bool GetKeyDown(KeyCode k) => FakeUnity.KeysDown.Contains(k);
        public static bool GetKeyUp(KeyCode k) => false;
        public static int touchCount => 0;
    }

    public enum KeyCode
    {
        None = 0, Backspace = 8, Tab = 9, Return = 13, Escape = 27, Space = 32, Delete = 127, KeypadEnter = 271,
        UpArrow = 273, DownArrow = 274, RightArrow = 275, LeftArrow = 276, LeftShift = 304, RightShift = 303,
        LeftControl = 306, RightControl = 305, F = 102, M = 109, P = 112, S = 115, A = 97, D = 100, W = 119
    }

    public static class Application
    {
        public static string persistentDataPath => FakeUnity.DataPath;
        public static string dataPath => FakeUnity.DataPath;
        public static string streamingAssetsPath => FakeUnity.DataPath;
        public static int targetFrameRate { get; set; }
        public static bool runInBackground { get; set; }
        public static bool isEditor => false;
        public static bool isBatchMode => true;
        public static string unityVersion => "2022.3.0f1";
        public static RuntimePlatform platform => RuntimePlatform.WindowsPlayer;
        public static void Quit() { FakeUnity.QuitRequested = true; }
        public static void OpenURL(string url) { FakeUnity.Log("OpenURL " + url); }
    }

    public enum RuntimePlatform { WindowsPlayer, WindowsEditor, OSXPlayer, Android, IPhonePlayer, WebGLPlayer }

    public static class Debug
    {
        public static void Log(object m) => FakeUnity.Log(m);
        public static void LogWarning(object m) => FakeUnity.Log("WARN " + m);
        public static void LogError(object m) => FakeUnity.Log("ERROR " + m);
        public static void LogException(Exception e) => FakeUnity.Log("EXCEPTION " + e);
    }

    public static class PlayerPrefs
    {
        public static void SetString(string k, string v) { }
        public static string GetString(string k, string d = "") => d;
        public static void SetInt(string k, int v) { }
        public static int GetInt(string k, int d = 0) => d;
        public static void Save() { }
        public static bool HasKey(string k) => false;
    }

    public class AsyncOperation { public bool isDone => true; }
    public sealed class ResourceRequest : AsyncOperation { public Object asset => null; }

    public static class Resources
    {
        static readonly Dictionary<string, Object> Cache = new Dictionary<string, Object>();

        public static T Load<T>(string path) where T : Object
        {
            string key = typeof(T).Name + ":" + path;
            if (Cache.TryGetValue(key, out var o)) return (T)o;
            var obj = FakeUnity.LoadResource(path, typeof(T));
            Cache[key] = obj;
            return (T)obj;
        }

        public static Object Load(string path) => Load<TextAsset>(path);
        public static T GetBuiltinResource<T>(string path) where T : Object => typeof(T) == typeof(Font) ? (T)(Object)new Font("Arial") : null;
        public static AsyncOperation UnloadUnusedAssets() => new AsyncOperation();
        public static void UnloadAsset(Object o) { }
    }

    public class Texture : Object
    {
        internal int w, h;
        public virtual int width { get => w; set => w = value; }
        public virtual int height { get => h; set => h = value; }
        public FilterMode filterMode { get; set; }
        public TextureWrapMode wrapMode { get; set; }
    }

    public sealed class Texture2D : Texture
    {
        internal System.Drawing.Bitmap bmp;
        internal string path;
        Color32[] mPixels;
        static Texture2D sWhite;

        public Texture2D(int width, int height) { w = width; h = height; }
        public Texture2D(int width, int height, TextureFormat format, bool mipChain) { w = width; h = height; }

        public static Texture2D whiteTexture
        {
            get
            {
                if (sWhite == null)
                {
                    sWhite = new Texture2D(4, 4) { name = "white" };
                    sWhite.bmp = new System.Drawing.Bitmap(4, 4);
                    using (var g = System.Drawing.Graphics.FromImage(sWhite.bmp)) g.Clear(System.Drawing.Color.White);
                }
                return sWhite;
            }
        }

        public static Texture2D blackTexture => null;
        public void SetPixels32(Color32[] colors) { mPixels = colors; }
        public void SetPixel(int x, int y, Color c) { }
        public Color GetPixel(int x, int y) => default;

        public Color32[] GetPixels32()
        {
            if (mPixels != null) return mPixels;
            if (bmp == null) return new Color32[w * h];
            var px = new Color32[w * h];
            var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var raw = new byte[data.Stride * h];
            System.Runtime.InteropServices.Marshal.Copy(data.Scan0, raw, 0, raw.Length);
            bmp.UnlockBits(data);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int s = y * data.Stride + x * 4;
                    // Unity stores rows bottom-up
                    px[(h - 1 - y) * w + x] = new Color32(raw[s + 2], raw[s + 1], raw[s], raw[s + 3]);
                }
            mPixels = px;
            return px;
        }

        public void Apply() { }
        public bool isReadable => true;
    }

    public enum TextureFormat { Alpha8 = 1, RGB24 = 3, RGBA32 = 4, ARGB32 = 5 }
    public enum FilterMode { Point, Bilinear, Trilinear }
    public enum TextureWrapMode { Repeat, Clamp, Mirror, MirrorOnce }

    public class TextAsset : Object
    {
        internal string mText = "";
        internal byte[] mBytes = new byte[0];
        public string text => mText;
        public byte[] bytes => mBytes;
    }

    public sealed class AudioClip : Object
    {
        internal float mLength = 1;
        public float length => mLength;
        public int samples => (int)(mLength * 44100);
        public int frequency => 44100;
    }

    public sealed class AudioSource : Behaviour
    {
        float mStartedAt = -1, mStartPos;
        public AudioClip clip { get; set; }
        public float volume { get; set; } = 1;
        public float panStereo { get; set; }
        public float pitch { get; set; } = 1;
        public bool loop { get; set; }
        public bool playOnAwake { get; set; }
        public bool mute { get; set; }
        float mTime;
        public float time
        {
            get => mStartedAt < 0 ? mTime : Math.Min(clip?.length ?? 0, mStartPos + (FakeUnity.Now - mStartedAt));
            set { mTime = value; if (mStartedAt >= 0) { mStartPos = value; mStartedAt = FakeUnity.Now; } }
        }
        public bool isPlaying => mStartedAt >= 0 && clip != null && (loop || mStartPos + (FakeUnity.Now - mStartedAt) < clip.length);
        public void Play() { mStartPos = mTime; mStartedAt = FakeUnity.Now; FakeUnity.SoundsPlayed.Add(clip?.name); }
        public void Stop() { mStartedAt = -1; mTime = 0; }
        public void Pause() { mTime = time; mStartedAt = -1; }
        public void UnPause() => Play();
        public void PlayOneShot(AudioClip c, float v = 1) { }
    }

    public sealed class AudioListener : Behaviour
    {
        public static float volume { get; set; } = 1;
        public static bool pause { get; set; }
    }

    public sealed class Font : Object
    {
        internal System.Drawing.FontFamily family;
        public Font() { name = "Arial"; }
        public Font(string name) { this.name = name; }
        public int fontSize => 12;
        public bool dynamic => true;
    }

    public enum FontStyle { Normal, Bold, Italic, BoldAndItalic }
    public enum TextAnchor { UpperLeft, UpperCenter, UpperRight, MiddleLeft, MiddleCenter, MiddleRight, LowerLeft, LowerCenter, LowerRight }
    public enum HorizontalWrapMode { Wrap, Overflow }
    public enum VerticalWrapMode { Truncate, Overflow }

    public class Material : Object
    {
        public Material(Shader shader) { this.shader = shader; }
        public Material(Material source) { shader = source.shader; }
        public Color color { get; set; }
        public Shader shader { get; set; }
    }

    public sealed class Shader : Object
    {
        public static Shader Find(string name) => new Shader { name = name };
    }

    public sealed class Camera : Behaviour
    {
        static Camera sMain;
        public Camera() { sMain = this; }
        public static Camera main => sMain;
        public CameraClearFlags clearFlags { get; set; }
        public Color backgroundColor { get; set; }
        public bool orthographic { get; set; }
        public float orthographicSize { get; set; }
        public int depth { get; set; }
    }

    public enum CameraClearFlags { Skybox = 1, SolidColor = 2, Depth = 3, Nothing = 4 }
    public enum RenderMode { ScreenSpaceOverlay, ScreenSpaceCamera, WorldSpace }

    public sealed class Canvas : Behaviour
    {
        public RenderMode renderMode { get; set; }
        public bool pixelPerfect { get; set; }
        public float scaleFactor { get; set; } = 1;
        public int sortingOrder { get; set; }
        public Camera worldCamera { get; set; }
    }

    public sealed class CanvasGroup : Behaviour
    {
        float _f_alpha = 1; public float alpha { get => _f_alpha; set { Chk(); _f_alpha = value; } }
        public bool blocksRaycasts { get; set; }
        public bool interactable { get; set; }
        public bool ignoreParentGroups { get; set; }
    }

    public static class RectTransformUtility
    {
        public static bool ScreenPointToLocalPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam, out Vector2 localPoint)
        {
            localPoint = rect.World.Inverse().Apply(screenPoint);
            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RuntimeInitializeOnLoadMethodAttribute : Attribute
    {
        public RuntimeInitializeOnLoadMethodAttribute() { }
        public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType t) { }
    }

    public enum RuntimeInitializeLoadType { AfterSceneLoad, BeforeSceneLoad, AfterAssembliesLoaded, BeforeSplashScreen, SubsystemRegistration }

    [AttributeUsage(AttributeTargets.Field)] public class SerializeField : Attribute { }
    [AttributeUsage(AttributeTargets.Class)] public class DisallowMultipleComponent : Attribute { }
    public class HideInInspector : Attribute { }
    public enum AudioClipLoadType { DecompressOnLoad, CompressedInMemory, Streaming }
    public enum AudioCompressionFormat { PCM, Vorbis, ADPCM, MP3 }
}

namespace UnityEngine.UI
{
    public abstract class Graphic : Behaviour
    {
        Color _f_color = Color.white; public Color color { get => _f_color; set { Chk(); _f_color = value; } }
        bool _f_raycastTarget; public bool raycastTarget { get => _f_raycastTarget; set { Chk(); _f_raycastTarget = value; } }
        public virtual Material material { get; set; }
        public RectTransform rectTransform => transform as RectTransform;
        public void SetAllDirty() { }
    }

    public abstract class MaskableGraphic : Graphic { public bool maskable { get; set; } = true; }

    public class RawImage : MaskableGraphic
    {
        Texture _f_texture; public Texture texture { get => _f_texture; set { Chk(); _f_texture = value; } }
        Rect _f_uvRect = new Rect(0, 0, 1, 1); public Rect uvRect { get => _f_uvRect; set { Chk(); _f_uvRect = value; } }
    }

    public class Image : MaskableGraphic { }

    public class Text : MaskableGraphic
    {
        Font _f_font; public Font font { get => _f_font; set { Chk(); _f_font = value; } }
        string _f_text = ""; public string text { get => _f_text; set { Chk(); _f_text = value; } }
        int _f_fontSize = 14; public int fontSize { get => _f_fontSize; set { Chk(); _f_fontSize = value; } }
        FontStyle _f_fontStyle; public FontStyle fontStyle { get => _f_fontStyle; set { Chk(); _f_fontStyle = value; } }
        TextAnchor _f_alignment; public TextAnchor alignment { get => _f_alignment; set { Chk(); _f_alignment = value; } }
        public HorizontalWrapMode horizontalOverflow { get; set; }
        public VerticalWrapMode verticalOverflow { get; set; }
        public bool supportRichText { get; set; }
        public bool resizeTextForBestFit { get; set; }
        public int resizeTextMinSize { get; set; }
        public int resizeTextMaxSize { get; set; }
        public float lineSpacing { get; set; } = 1;
        public float preferredWidth => FakeUnity.MeasureText(this).x;
        public float preferredHeight => FakeUnity.MeasureText(this).y;
    }

    public class RectMask2D : Behaviour { }
}

namespace UnityEditor
{
    public class AssetPostprocessor
    {
        public string assetPath => "";
        public AssetImporter assetImporter => null;
    }

    public class AssetImporter : UnityEngine.Object { }

    public sealed class TextureImporter : AssetImporter
    {
        public TextureImporterType textureType { get; set; }
        public TextureImporterNPOTScale npotScale { get; set; }
        public bool mipmapEnabled { get; set; }
        public bool alphaIsTransparency { get; set; }
        public UnityEngine.TextureWrapMode wrapMode { get; set; }
        public UnityEngine.FilterMode filterMode { get; set; }
        public TextureImporterCompression textureCompression { get; set; }
        public int maxTextureSize { get; set; }
        public bool sRGBTexture { get; set; }
        public bool isReadable { get; set; }
    }

    public enum TextureImporterType { Default = 0, NormalMap = 1, GUI = 2, Sprite = 8 }
    public enum TextureImporterNPOTScale { None, ToNearest, ToLarger, ToSmaller }
    public enum TextureImporterCompression { Uncompressed, Compressed, CompressedHQ, CompressedLQ }

    public sealed class AudioImporter : AssetImporter
    {
        public AudioImporterSampleSettings defaultSampleSettings { get; set; }
        public bool loadInBackground { get; set; }
    }

    public struct AudioImporterSampleSettings
    {
        public UnityEngine.AudioClipLoadType loadType;
        public UnityEngine.AudioCompressionFormat compressionFormat;
        public float quality;
    }

    [AttributeUsage(AttributeTargets.Class)] public class InitializeOnLoadAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Method)] public class MenuItem : Attribute { public MenuItem(string itemName) { } }

    public static class EditorApplication
    {
        public delegate void CallbackFunction();
        public static CallbackFunction delayCall;
        public static bool isPlayingOrWillChangePlaymode => false;
        public static bool isPlaying { get; set; }
        public static void Exit(int code) { }
    }

    public static class AssetDatabase { public static void Refresh() { } }

    public class EditorBuildSettingsScene
    {
        public EditorBuildSettingsScene(string path, bool enabled) { this.path = path; }
        public string path;
    }

    public static class EditorBuildSettings { public static EditorBuildSettingsScene[] scenes { get; set; } }

    public static class PlayerSettings
    {
        public static string companyName { get; set; }
        public static string productName { get; set; }
        public static int defaultScreenWidth { get; set; }
        public static int defaultScreenHeight { get; set; }
        public static UnityEngine.FullScreenMode fullScreenMode { get; set; }
        public static bool resizableWindow { get; set; }
        public static bool runInBackground { get; set; }
    }
}

namespace UnityEditor.SceneManagement
{
    public enum NewSceneSetup { EmptyScene, DefaultGameObjects }
    public enum NewSceneMode { Single, Additive }

    public static class EditorSceneManager
    {
        public static UnityEngine.SceneManagement.Scene NewScene(NewSceneSetup setup, NewSceneMode mode) => default;
        public static bool SaveScene(UnityEngine.SceneManagement.Scene scene, string path) => true;
        public static UnityEngine.SceneManagement.Scene OpenScene(string path) => default;
        public static UnityEngine.SceneManagement.Scene GetActiveScene() => default;
    }
}

namespace UnityEngine.SceneManagement
{
    public struct Scene { public string path => ""; public string name => ""; }
}

namespace UnityEditor
{
    public enum BuildTarget { StandaloneWindows = 5, StandaloneWindows64 = 19 }
    [Flags] public enum BuildOptions { None = 0, Development = 1 }
    public struct BuildPlayerOptions
    {
        public string[] scenes { get; set; }
        public string locationPathName { get; set; }
        public BuildTarget target { get; set; }
        public BuildOptions options { get; set; }
    }
    public static class BuildPipeline
    {
        public static UnityEditor.Build.Reporting.BuildReport BuildPlayer(BuildPlayerOptions o) => new UnityEditor.Build.Reporting.BuildReport();
    }
}

namespace UnityEditor.Build.Reporting
{
    public enum BuildResult { Unknown, Succeeded, Failed, Cancelled }
    public struct BuildSummary { public BuildResult result => BuildResult.Succeeded; }
    public class BuildReport : UnityEngine.Object { public BuildSummary summary => default; }
}
