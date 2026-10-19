// FakeUnity runtime core: frame loop, simulated input, Resources from disk, text measuring and a software
// renderer for UI canvases (RawImage / Text / CanvasGroup / RectMask2D) to PNG screenshots.
#pragma warning disable CS0618
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;

namespace UnityEngine
{
    public static class FakeUnity
    {
        public static float DeltaTime = 1f / 60f;
        public static float Now;
        public static int Frame;
        public static int ScreenWidth = 1024, ScreenHeight = 600;
        public static Vector3 Mouse;
        public static bool MouseHeld, MouseDown, MouseUp;
        public static readonly HashSet<KeyCode> KeysHeld = new HashSet<KeyCode>();
        public static readonly HashSet<KeyCode> KeysDown = new HashSet<KeyCode>();
        public static string InputString = "";
        public static string DataPath = Path.Combine(Path.GetTempPath(), "fakeunity_data");
        public static string ResourcesDir = "";
        public static bool QuitRequested;
        public static bool Verbose = true;
        public static readonly List<string> Logs = new List<string>();
        public static readonly List<string> SoundsPlayed = new List<string>();
        static readonly List<MonoBehaviour> Behaviours = new List<MonoBehaviour>();
        static readonly Dictionary<Type, MethodInfo> UpdateMethods = new Dictionary<Type, MethodInfo>();
        static readonly PrivateFontCollection Fonts = new PrivateFontCollection();

        public static void Log(object m)
        {
            string s = "[" + Frame + "] " + m;
            Logs.Add(s);
            if (Verbose) Console.WriteLine(s);
        }

        internal static void OnComponentAdded(Component c)
        {
            if (c is MonoBehaviour mb) Behaviours.Add(mb);
        }

        /// <summary>Runs one frame: Update() on every active MonoBehaviour, then clears one-frame input.</summary>
        public static void Step()
        {
            foreach (var mb in Behaviours.ToArray())
            {
                if (mb.destroyed || !mb.enabled || mb.gameObject == null || !mb.gameObject.activeInHierarchy) continue;
                var t = mb.GetType();
                if (!UpdateMethods.TryGetValue(t, out var m))
                    UpdateMethods[t] = m = t.GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                try { m?.Invoke(mb, null); }
                catch (TargetInvocationException e) { Log("EXCEPTION in " + t.Name + ".Update: " + e.InnerException); throw e.InnerException; }
            }
            Behaviours.RemoveAll(b => b.destroyed);
            MouseDown = MouseUp = false;
            KeysDown.Clear();
            InputString = "";
            Now += DeltaTime;
            Frame++;
        }

        /// <summary>Invokes OnApplicationFocus(bool) on behaviours (simulates focus changes).</summary>
        public static void Focus(bool f)
        {
            foreach (var mb in Behaviours.ToArray())
                mb.GetType().GetMethod("OnApplicationFocus", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.Invoke(mb, new object[] { f });
        }

        // ------------------------------------------------------------------ resources
        static readonly string[] ImageExt = { ".png", ".jpg", ".jpeg" };
        static readonly string[] TextExt = { ".xml", ".txt", ".json", ".bytes", ".fnt", ".csv" };
        static readonly string[] AudioExt = { ".mp3", ".wav", ".ogg" };
        static readonly string[] FontExt = { ".ttf", ".otf" };

        static string Find(string path, string[] exts)
        {
            foreach (var e in exts)
            {
                var p = Path.Combine(ResourcesDir, path.Replace('/', Path.DirectorySeparatorChar)) + e;
                if (File.Exists(p)) return p;
            }
            return null;
        }

        internal static Object LoadResource(string path, Type t)
        {
            if (t == typeof(Texture2D) || t == typeof(Texture))
            {
                var f = Find(path, ImageExt);
                if (f == null) return null;
                using (var src = new System.Drawing.Bitmap(f))
                {
                    var bmp = new System.Drawing.Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (var g = Graphics.FromImage(bmp)) g.DrawImage(src, 0, 0, src.Width, src.Height);
                    return new Texture2D(bmp.Width, bmp.Height) { bmp = bmp, path = f, name = Path.GetFileNameWithoutExtension(f) };
                }
            }
            if (t == typeof(TextAsset))
            {
                var f = Find(path, TextExt);
                if (f == null) return null;
                var bytes = File.ReadAllBytes(f);
                var text = System.Text.Encoding.UTF8.GetString(bytes);
                if (text.Length > 0 && text[0] == '﻿') text = text.Substring(1);
                return new TextAsset { mText = text, mBytes = bytes, name = Path.GetFileNameWithoutExtension(f) };
            }
            if (t == typeof(AudioClip))
            {
                var f = Find(path, AudioExt);
                if (f == null) return null;
                return new AudioClip { mLength = Mp3Length(File.ReadAllBytes(f)), name = Path.GetFileNameWithoutExtension(f) };
            }
            if (t == typeof(Font))
            {
                var f = Find(path, FontExt);
                if (f == null) return null;
                Fonts.AddFontFile(f);
                var fam = Fonts.Families.Last();
                return new Font(fam.Name) { family = fam };
            }
            return null;
        }

        static float Mp3Length(byte[] d)
        {
            int[] br = { 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 };
            int[] br2 = { 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160 };
            int p = 0;
            if (d.Length > 10 && d[0] == 'I' && d[1] == 'D' && d[2] == '3') p = 10 + (d[6] << 21 | d[7] << 14 | d[8] << 7 | d[9]);
            for (; p + 4 < d.Length; p++)
            {
                if (d[p] != 0xFF || (d[p + 1] & 0xE0) != 0xE0) continue;
                int ver = (d[p + 1] >> 3) & 3, bri = d[p + 2] >> 4;
                if (bri == 0 || bri == 15 || ver == 1) continue;
                int kbps = ver == 3 ? br[bri] : br2[bri];
                return Math.Max(0.05f, (d.Length - p) * 8f / (kbps * 1000f));
            }
            return 1;
        }

        // ------------------------------------------------------------------ text
        internal static System.Drawing.Font MakeFont(Text t, int size)
        {
            var style = t.fontStyle == UnityEngine.FontStyle.Bold ? System.Drawing.FontStyle.Bold :
                t.fontStyle == UnityEngine.FontStyle.Italic ? System.Drawing.FontStyle.Italic :
                t.fontStyle == UnityEngine.FontStyle.BoldAndItalic ? System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic : System.Drawing.FontStyle.Regular;
            var fam = t.font?.family ?? System.Drawing.FontFamily.GenericSansSerif;
            if (!fam.IsStyleAvailable(style)) style = System.Drawing.FontStyle.Regular;
            return new System.Drawing.Font(fam, Math.Max(1, size), style, GraphicsUnit.Pixel);
        }

        static readonly Graphics Measure = Graphics.FromImage(new System.Drawing.Bitmap(1, 1));

        internal static Vector2 MeasureText(Text t)
        {
            var rt = t.rectTransform;
            float w = rt != null ? rt.Size.x : 1000;
            using (var f = MakeFont(t, t.fontSize))
            {
                var s = t.horizontalOverflow == HorizontalWrapMode.Wrap && w > 0
                    ? Measure.MeasureString(t.text ?? "", f, (int)Math.Max(1, w))
                    : Measure.MeasureString(t.text ?? "", f);
                return new Vector2(s.Width, s.Height);
            }
        }

        // ------------------------------------------------------------------ rendering
        public static System.Drawing.Bitmap Render()
        {
            var img = new System.Drawing.Bitmap(ScreenWidth, ScreenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(img))
            {
                g.Clear(System.Drawing.Color.Black);
                g.InterpolationMode = InterpolationMode.Bilinear;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                foreach (var go in GameObject.All.ToArray())
                    if (go.transform.parent == null && go.GetComponent<Canvas>() != null) Draw(g, go.transform, 1, null);
            }
            return img;
        }

        public static void Screenshot(string file)
        {
            using (var b = Render()) b.Save(file, ImageFormat.Png);
        }

        static PointF ToImage(Vector2 p) => new PointF(p.x, ScreenHeight - p.y);

        static void Draw(Graphics g, Transform t, float alpha, RectangleF? clip)
        {
            var go = t.gameObject;
            if (go == null || go.destroyed || !go.activeSelf) return;
            var cg = go.GetComponent<CanvasGroup>();
            if (cg != null && cg.enabled) alpha *= cg.alpha;
            if (alpha <= 0.001f) return;
            if (t is RectTransform rt)
            {
                var world = rt.World;
                var r = rt.rect;
                var ri = go.GetComponent<RawImage>();
                if (ri != null && ri.enabled && ri.texture != null) DrawRaw(g, ri, world, r, alpha, clip);
                var tx = go.GetComponent<Text>();
                if (tx != null && tx.enabled && !string.IsNullOrEmpty(tx.text)) DrawText(g, tx, world, r, alpha, clip);
                var mask = go.GetComponent<RectMask2D>();
                if (mask != null && mask.enabled)
                {
                    var c = Aabb(world, r);
                    clip = clip.HasValue ? RectangleF.Intersect(clip.Value, c) : c;
                }
            }
            foreach (Transform ch in t.children.ToArray()) Draw(g, ch, alpha, clip);
        }

        static RectangleF Aabb(M2 w, Rect r)
        {
            var pts = new[] { ToImage(w.Apply(new Vector2(r.x, r.y))), ToImage(w.Apply(new Vector2(r.xMax, r.y))),
                              ToImage(w.Apply(new Vector2(r.x, r.yMax))), ToImage(w.Apply(new Vector2(r.xMax, r.yMax))) };
            float x0 = pts.Min(p => p.X), y0 = pts.Min(p => p.Y), x1 = pts.Max(p => p.X), y1 = pts.Max(p => p.Y);
            return new RectangleF(x0, y0, x1 - x0, y1 - y0);
        }

        static void DrawRaw(Graphics g, RawImage ri, M2 w, Rect r, float alpha, RectangleF? clip)
        {
            if (!(ri.texture is Texture2D tex) || tex.bmp == null) return;
            if (r.width == 0 || r.height == 0) return;
            var uv = ri.uvRect;
            float sx = uv.x * tex.width, sy = (1 - (uv.y + uv.height)) * tex.height, sw = uv.width * tex.width, sh = uv.height * tex.height;
            var dest = new[] { ToImage(w.Apply(new Vector2(r.x, r.yMax))), ToImage(w.Apply(new Vector2(r.xMax, r.yMax))), ToImage(w.Apply(new Vector2(r.x, r.y))) };
            var c = ri.color;
            var cm = new ColorMatrix(new[]
            {
                new float[] { c.r, 0, 0, 0, 0 }, new float[] { 0, c.g, 0, 0, 0 }, new float[] { 0, 0, c.b, 0, 0 },
                new float[] { 0, 0, 0, c.a * alpha, 0 }, new float[] { 0, 0, 0, 0, 1 }
            });
            using (var ia = new ImageAttributes())
            {
                ia.SetColorMatrix(cm);
                ia.SetWrapMode(WrapMode.TileFlipXY);
                var state = g.Save();
                if (clip.HasValue) g.SetClip(clip.Value);
                try { g.DrawImage(tex.bmp, dest, new RectangleF(sx, sy, sw, sh), GraphicsUnit.Pixel, ia); }
                catch (Exception) { }
                g.Restore(state);
            }
        }

        static void DrawText(Graphics g, Text t, M2 w, Rect r, float alpha, RectangleF? clip)
        {
            int size = Math.Max(1, t.fontSize);
            var fmt = new StringFormat();
            int a = (int)t.alignment;
            fmt.Alignment = (a % 3) == 0 ? StringAlignment.Near : (a % 3) == 1 ? StringAlignment.Center : StringAlignment.Far;
            fmt.LineAlignment = (a / 3) == 0 ? StringAlignment.Near : (a / 3) == 1 ? StringAlignment.Center : StringAlignment.Far;
            // Unity wraps only at word boundaries; GDI+ would also break single long words mid-word
            if (t.horizontalOverflow == HorizontalWrapMode.Overflow || t.text.IndexOfAny(new[] { ' ', (char)10 }) < 0) fmt.FormatFlags |= StringFormatFlags.NoWrap;
            fmt.FormatFlags |= StringFormatFlags.NoClip;
            var box = new RectangleF(0, 0, r.width, r.height);
            var f = MakeFont(t, size);
            if (t.resizeTextForBestFit)
            {
                while (size > 2)
                {
                    var m = g.MeasureString(t.text, f, (int)Math.Max(1, box.Width), fmt);
                    if (m.Height <= box.Height + 1 && m.Width <= box.Width + 1) break;
                    size--;
                    f.Dispose();
                    f = MakeFont(t, size);
                }
            }
            var c = t.color;
            using (var br = new SolidBrush(System.Drawing.Color.FromArgb((int)(Mathf.Clamp01(c.a * alpha) * 255), (int)(c.r * 255), (int)(c.g * 255), (int)(c.b * 255))))
            {
                var state = g.Save();
                if (clip.HasValue) g.SetClip(clip.Value);
                // local text space (origin top-left of the rect, y down) -> image space
                g.Transform = new Matrix(w.a, -w.b, -w.c, w.d,
                    w.a * r.x + w.c * r.yMax + w.tx, ScreenHeight - (w.b * r.x + w.d * r.yMax + w.ty));
                g.DrawString(t.text, f, br, box, fmt);
                g.Restore(state);
            }
            f.Dispose();
        }
    }
}
