using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Starlite
{
    /// <summary>A (sub-)region of a Unity texture, with optional trim frame (Starling SubTexture semantics).</summary>
    public class Texture
    {
        internal readonly Texture2D Native;
        readonly Rectangle mRegion;   // pixels, top-left origin, inside Native
        readonly Rectangle mFrame;    // trim frame (x/y usually <= 0) or null
        public readonly float scale = 1;

        internal Texture(Texture2D native, Rectangle region = null, Rectangle frame = null)
        {
            Native = native;
            mRegion = region ?? new Rectangle(0, 0, native != null ? native.width : 1, native != null ? native.height : 1);
            mFrame = frame;
        }

        public float width => mFrame != null ? mFrame.width : mRegion.width;
        public float height => mFrame != null ? mFrame.height : mRegion.height;
        public float nativeWidth => mRegion.width;
        public float nativeHeight => mRegion.height;
        public Rectangle frame => mFrame?.clone();
        public Rectangle region => mRegion.clone();
        public Texture root => new Texture(Native);
        internal float RegionWidth => mRegion.width;
        internal float RegionHeight => mRegion.height;

        internal UnityEngine.Rect UvRect
        {
            get
            {
                if (Native == null) return new UnityEngine.Rect(0, 0, 1, 1);
                float tw = Native.width, th = Native.height;
                return new UnityEngine.Rect(mRegion.x / tw, 1 - (mRegion.y + mRegion.height) / th, mRegion.width / tw, mRegion.height / th);
            }
        }

        public static Texture fromTexture(Texture texture, Rectangle region = null, Rectangle frame = null)
        {
            var r = region == null ? texture.mRegion.clone()
                : new Rectangle(texture.mRegion.x + region.x, texture.mRegion.y + region.y, region.width, region.height);
            return new Texture(texture.Native, r, frame);
        }

        public static Texture fromUnity(Texture2D t) => new Texture(t);

        /// <summary>Used by ported code that built textures from flash Bitmaps.</summary>
        public static Texture fromBitmap(Bitmap bmp, bool generateMipMaps = true, bool optimizeForRenderToTexture = false, float scale = 1) => bmp.texture;

        public static Texture empty(float width, float height, bool premultipliedAlpha = true, bool mipMapping = true, bool optimizeForRenderToTexture = false, float scale = -1)
        {
            var t = new Texture2D(Math.Max(1, (int)width), Math.Max(1, (int)height), TextureFormat.RGBA32, false);
            var px = new Color32[t.width * t.height];
            t.SetPixels32(px);
            t.Apply();
            return new Texture(t);
        }

        public void dispose() { }
    }

    /// <summary>Stand-in for flash.display.Bitmap holding a texture.</summary>
    public class Bitmap
    {
        public readonly Texture texture;
        public readonly BitmapData bitmapData;
        public Bitmap(Texture t) { texture = t; bitmapData = new BitmapData(t.Native); }
        public Bitmap(BitmapData data) { bitmapData = data; texture = new Texture(data.native); }
        public float width => texture.width;
        public float height => texture.height;
    }

    public class TextureAtlas
    {
        readonly Texture mAtlasTexture;
        readonly Dictionary<string, (Rectangle region, Rectangle frame)> mRegions = new Dictionary<string, (Rectangle, Rectangle)>();
        readonly Dictionary<string, Texture> mCache = new Dictionary<string, Texture>();
        readonly List<string> mNames = new List<string>();

        public TextureAtlas(Texture texture, XML atlasXml = null)
        {
            mAtlasTexture = texture;
            if (atlasXml != null) ParseAtlasXml(atlasXml);
        }

        public Texture texture => mAtlasTexture;

        void ParseAtlasXml(XML atlasXml)
        {
            foreach (var st in atlasXml["SubTexture"])
            {
                string n = st.attribute("name");
                float F(string a) => AS3.toNumber(st.attribute(a));
                var region = new Rectangle(F("x"), F("y"), F("width"), F("height"));
                Rectangle frame = null;
                if (st.attribute("frameWidth").length() > 0 && F("frameWidth") > 0 && F("frameHeight") > 0)
                    frame = new Rectangle(F("frameX"), F("frameY"), F("frameWidth"), F("frameHeight"));
                addRegion(n, region, frame);
            }
        }

        public void addRegion(string name, Rectangle region, Rectangle frame = null)
        {
            if (!mRegions.ContainsKey(name)) mNames.Add(name);
            mRegions[name] = (region, frame);
            mCache.Remove(name);
        }

        public void removeRegion(string name) { mRegions.Remove(name); mNames.Remove(name); mCache.Remove(name); }

        public Texture getTexture(string name)
        {
            if (mCache.TryGetValue(name, out var t)) return t;
            if (!mRegions.TryGetValue(name, out var r)) return null;
            t = Texture.fromTexture(mAtlasTexture, r.region, r.frame);
            mCache[name] = t;
            return t;
        }

        public List<string> getNames(string prefix = "", List<string> result = null)
        {
            result = result ?? new List<string>();
            foreach (var n in mNames) if (n.StartsWith(prefix, StringComparison.Ordinal)) result.Add(n);
            result.Sort(StringComparer.Ordinal);
            return result;
        }

        public List<Texture> getTextures(string prefix = "", List<Texture> result = null)
        {
            result = result ?? new List<Texture>();
            foreach (var n in getNames(prefix)) result.Add(getTexture(n));
            return result;
        }

        public Rectangle getRegion(string name) => mRegions.TryGetValue(name, out var r) ? r.region.clone() : null;
        public Rectangle getFrame(string name) => mRegions.TryGetValue(name, out var r) ? r.frame?.clone() : null;
        public void dispose() { }
    }

    /// <summary>
    /// Starling AssetManager over Unity Resources. Game URLs such as "data/graphic/hd/kitchen.ean" map to
    /// Resources/RR/graphic/hd/kitchen (the converter decrypts .ean to PNG and copies everything there).
    /// Asset names are file names without extension, exactly like Starling.
    /// </summary>
    public class AssetManager
    {
        public const string ResourceRoot = "RR/";
        readonly List<string> mQueue = new List<string>();
        readonly Dictionary<string, Texture> mTextures = new Dictionary<string, Texture>();
        readonly Dictionary<string, TextureAtlas> mAtlases = new Dictionary<string, TextureAtlas>();
        readonly Dictionary<string, XML> mXmls = new Dictionary<string, XML>();
        readonly Dictionary<string, Sound> mSounds = new Dictionary<string, Sound>();
        readonly Dictionary<string, object> mObjects = new Dictionary<string, object>();
        public float scaleFactor;
        public bool verbose;

        public AssetManager(float scaleFactor = 1, bool useMipmaps = false) { this.scaleFactor = scaleFactor; }

        public static string ToResourcePath(string url)
        {
            url = url.Replace('\\', '/');
            if (url.StartsWith("data/")) url = url.Substring(5);
            int dot = url.LastIndexOf('.');
            if (dot > url.LastIndexOf('/')) url = url.Substring(0, dot);
            return ResourceRoot + url;
        }

        public static string getName(string url)
        {
            url = url.Replace('\\', '/');
            string n = url.Substring(url.LastIndexOf('/') + 1);
            int dot = n.LastIndexOf('.');
            return dot > 0 ? n.Substring(0, dot) : n;
        }

        public void enqueue(params object[] rawAssets)
        {
            foreach (var a in rawAssets)
            {
                if (a is string s) mQueue.Add(s);
                else if (a is IEnumerable<string> list) mQueue.AddRange(list);
                else if (a is object[] arr) enqueue(arr);
            }
        }

        public int numQueuedAssets => mQueue.Count;

        /// <summary>Loads everything synchronously, then reports progress 1.0 on the next frame (Starling is async).</summary>
        public void loadQueue(Action<float> onProgress)
        {
            var queue = mQueue.ToArray();
            mQueue.Clear();
            var atlasXmls = new List<(string name, XML xml)>();
            foreach (var url in queue)
            {
                string ext = Path.GetExtension(url).ToLowerInvariant();
                string name = getName(url);
                string res = ToResourcePath(url);
                switch (ext)
                {
                    case ".ean": case ".png": case ".jpg": case ".jpeg": case ".eag": case ".gif": case ".atf":
                        var tex = Resources.Load<Texture2D>(res);
                        if (tex == null) Debug.LogWarning("[AssetManager] texture not found: " + res);
                        else addTexture(name, new Texture(tex));
                        break;
                    case ".xml": case ".fnt": case ".pex":
                        var ta = Resources.Load<TextAsset>(res);
                        if (ta == null) { Debug.LogWarning("[AssetManager] xml not found: " + res); break; }
                        var xml = new XML(ta.text);
                        if (xml.name() == "TextureAtlas") atlasXmls.Add((name, xml));
                        else addXml(name, xml);
                        break;
                    case ".mp3": case ".wav": case ".ogg":
                        var clip = Resources.Load<AudioClip>(res);
                        if (clip == null) Debug.LogWarning("[AssetManager] sound not found: " + res);
                        else addSound(name, new Sound(clip));
                        break;
                    default:
                        var other = Resources.Load<TextAsset>(res);
                        if (other != null) addObject(name, other.bytes);
                        break;
                }
            }
            foreach (var (name, xml) in atlasXmls)
            {
                string img = getName(xml.attribute("imagePath"));
                var t = getTexture(img);
                if (t == null) { Debug.LogWarning("[AssetManager] atlas texture missing: " + img); continue; }
                addTextureAtlas(name, new TextureAtlas(t, xml));
                mTextures.Remove(img); // Starling disposes the plain texture once the atlas owns it
            }
            Starling.juggler.delayCall((Action)(() => onProgress?.Invoke(1f)), 0);
        }

        public void addTexture(string name, Texture t) => mTextures[name] = t;
        public void addTextureAtlas(string name, TextureAtlas a) => mAtlases[name] = a;
        public void addXml(string name, XML x) => mXmls[name] = x;
        public void addSound(string name, Sound s) => mSounds[name] = s;
        public void addObject(string name, object o) => mObjects[name] = o;

        public Texture getTexture(string name)
        {
            if (mTextures.TryGetValue(name, out var t)) return t;
            foreach (var a in mAtlases.Values)
            {
                t = a.getTexture(name);
                if (t != null) return t;
            }
            return null;
        }

        public List<Texture> getTextures(string prefix = "", List<Texture> result = null)
        {
            result = result ?? new List<Texture>();
            foreach (var n in getTextureNames(prefix)) result.Add(getTexture(n));
            return result;
        }

        public List<string> getTextureNames(string prefix = "", List<string> result = null)
        {
            result = result ?? new List<string>();
            foreach (var n in mTextures.Keys) if (n.StartsWith(prefix, StringComparison.Ordinal)) result.Add(n);
            foreach (var a in mAtlases.Values) a.getNames(prefix, result);
            result.Sort(StringComparer.Ordinal);
            return result;
        }

        /// <summary>Pixels of a loaded texture or atlas texture (game-specific AssetManager extension).</summary>
        public BitmapData getBitmapData(string name)
        {
            if (mAtlases.TryGetValue(name, out var atlas)) return new BitmapData(atlas.texture.Native);
            var t = getTexture(name);
            return t != null ? new BitmapData(t.Native) : null;
        }

        public TextureAtlas getTextureAtlas(string name) => mAtlases.TryGetValue(name, out var a) ? a : null;
        public XML getXml(string name) => mXmls.TryGetValue(name, out var x) ? x : null;
        public Sound getSound(string name) => mSounds.TryGetValue(name, out var s) ? s : null;
        public object getObject(string name) => mObjects.TryGetValue(name, out var o) ? o : null;

        public void removeTexture(string name, bool dispose = true) => mTextures.Remove(name);
        public void removeTextureAtlas(string name, bool dispose = true) => mAtlases.Remove(name);
        public void removeXml(string name, bool dispose = true) => mXmls.Remove(name);
        public void removeSound(string name) => mSounds.Remove(name);

        public void purge()
        {
            mTextures.Clear(); mAtlases.Clear(); mXmls.Clear(); mSounds.Clear(); mObjects.Clear();
            Starling.RequestUnloadUnused();
        }
    }
}
