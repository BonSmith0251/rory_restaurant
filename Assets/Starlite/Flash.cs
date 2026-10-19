// Remaining flash.* / extension APIs the game touches.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Starlite
{
    public class URLRequest
    {
        public string url;
        public URLRequest(string url = null) { this.url = url; }
    }

    public static class URLLoaderDataFormat { public const string TEXT = "text", BINARY = "binary", VARIABLES = "variables"; }

    public class IOErrorEvent : Event
    {
        public new const string IO_ERROR = "ioError";
        public string text;
        public IOErrorEvent(string type, string text = "") : base(type) { this.text = text; }
    }

    /// <summary>Loads game files from Resources (see AssetManager.ToResourcePath); completes next frame like flash.</summary>
    public class URLLoader : EventDispatcher
    {
        public string dataFormat = URLLoaderDataFormat.TEXT;
        public object data;

        public void load(URLRequest req)
        {
            var ta = Resources.Load<TextAsset>(AssetManager.ToResourcePath(req.url));
            Starling.juggler.delayCall((Action)(() =>
            {
                if (ta == null)
                {
                    Debug.LogWarning("[URLLoader] not found: " + req.url);
                    dispatchEvent(new IOErrorEvent(IOErrorEvent.IO_ERROR, req.url));
                    return;
                }
                data = dataFormat == URLLoaderDataFormat.BINARY ? (object)ta.bytes : ta.text;
                dispatchEvent(new Event(Event.COMPLETE));
            }), 0);
        }

        public void close() { }
    }

    public static class Net
    {
        public static void navigateToURL(URLRequest req, string window = "_blank") { if (req?.url != null) Application.OpenURL(req.url); }
    }

    /// <summary>CPU-side pixels of a texture (the source Texture2D must be imported Read/Write).</summary>
    public class BitmapData
    {
        public readonly Texture2D native;
        Color32[] mPixels;
        public BitmapData(Texture2D tex) { native = tex; }
        public int width => native != null ? native.width : 0;
        public int height => native != null ? native.height : 0;

        Color32[] Pixels
        {
            get
            {
                if (mPixels == null && native != null)
                {
                    try { mPixels = native.GetPixels32(); }
                    catch (Exception e) { Debug.LogWarning("[BitmapData] texture not readable: " + native.name + " " + e.Message); mPixels = new Color32[0]; }
                }
                return mPixels;
            }
        }

        /// <summary>Alpha 0..255 at (x, y) with y measured from the top like flash.</summary>
        public int getAlpha(int x, int y)
        {
            var px = Pixels;
            if (px == null || px.Length == 0 || x < 0 || y < 0 || x >= width || y >= height) return 0;
            return px[(height - 1 - y) * width + x].a;
        }

        public uint getPixel32(int x, int y)
        {
            var px = Pixels;
            if (px == null || px.Length == 0 || x < 0 || y < 0 || x >= width || y >= height) return 0;
            var c = px[(height - 1 - y) * width + x];
            return ((uint)c.a << 24) | ((uint)c.r << 16) | ((uint)c.g << 8) | c.b;
        }

        public void dispose() { mPixels = null; }
    }

    /// <summary>krecha PixelHitArea: an alpha map of a whole atlas texture, sampled at a resolution factor.</summary>
    public class PixelHitArea
    {
        readonly BitmapData mData;
        public readonly float resolution;
        public PixelHitArea(Bitmap bitmap, float sampleResolution = 1) { mData = bitmap.bitmapData; resolution = sampleResolution; }
        public PixelHitArea(BitmapData data, float sampleResolution = 1) { mData = data; resolution = sampleResolution; }
        public int getAlphaPixel(float x, float y)
        {
            if (mData == null) return 255;
            // sample at reduced resolution like the original (snap to the coarser grid)
            float step = resolution > 0 ? 1f / resolution : 1;
            int sx = (int)(Mathf.Floor(x / step) * step), sy = (int)(Mathf.Floor(y / step) * step);
            return mData.getAlpha(sx, sy);
        }
        public void dispose() { }
    }

    /// <summary>krecha PixelImageTouch: an Image whose touch area ignores transparent pixels.</summary>
    public class PixelImageTouch : Image
    {
        public PixelHitArea hitArea;
        public uint threshold;

        public PixelImageTouch(Texture texture, PixelHitArea hitArea = null, uint threshold = 255) : base(texture)
        {
            this.hitArea = hitArea;
            this.threshold = threshold;
        }

        public override DisplayObject hitTest(Point localPoint, bool forTouch = false)
        {
            var hit = base.hitTest(localPoint, forTouch);
            if (hit == null || hitArea == null) return hit;
            var region = texture.region;
            var frame = texture.frame;
            float px = localPoint.x * (texture.width / Math.Max(1, getBounds(this).width)) + (frame != null ? frame.x : 0);
            float py = localPoint.y * (texture.height / Math.Max(1, getBounds(this).height)) + (frame != null ? frame.y : 0);
            if (px < 0 || py < 0 || px >= region.width || py >= region.height) return null;
            return hitArea.getAlphaPixel(region.x + px, region.y + py) >= Math.Min(threshold, 255u) ? this : null;
        }
    }

    public class BlurFilter
    {
        public static BlurFilter createGlow(uint color = 0xffff00, float alpha = 1, float blur = 1, float resolution = 0.5f) => new BlurFilter();
        public static BlurFilter createDropShadow(float distance = 4, float angle = 0.785f, uint color = 0, float alpha = 0.5f, float blur = 1, float resolution = 0.5f) => new BlurFilter();
        public void dispose() { }
    }

    public static class MouseCursor { public const string AUTO = "auto", ARROW = "arrow", BUTTON = "button", HAND = "hand", IBEAM = "ibeam"; }
    public static class Mouse { public static string cursor = MouseCursor.AUTO; public static void hide() { } public static void show() { } }

    public class NativeApplication
    {
        public static readonly NativeApplication nativeApplication = new NativeApplication();
        public void exit(int code = 0)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
