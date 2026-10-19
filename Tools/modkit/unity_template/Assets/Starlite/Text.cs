using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UObject = UnityEngine.Object;

namespace Starlite
{
    public static class HAlign { public const string LEFT = "left", CENTER = "center", RIGHT = "right"; }
    public static class VAlign { public const string TOP = "top", CENTER = "center", BOTTOM = "bottom"; }
    public static class TextFieldAutoSize
    {
        public const string NONE = "none", HORIZONTAL = "horizontal", VERTICAL = "vertical", BOTH_DIRECTIONS = "bothDirections";
    }

    /// <summary>Maps the game's font names to TTF files under Resources/RR/fonts.</summary>
    public static class Fonts
    {
        static readonly Dictionary<string, string> Files = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["bromine"] = "Bromine", ["jenna_sue"] = "JennaSue", ["jennasue"] = "JennaSue", ["pfennig"] = "Pfennig",
            ["komikandy"] = "KOMIKND_",
        };
        static readonly Dictionary<string, Font> Cache = new Dictionary<string, Font>(StringComparer.OrdinalIgnoreCase);
        static Font sFallback;

        public static Font Get(string name)
        {
            Font f = null;
            if (name != null && Cache.TryGetValue(name, out f) && f != null) return f;
            if (name != null && Files.TryGetValue(name, out var file)) f = Resources.Load<Font>(AssetManager.ResourceRoot + "fonts/" + file);
            if (f == null && name != null) f = Resources.Load<Font>(AssetManager.ResourceRoot + "fonts/" + name);
            if (f == null) f = Fallback;
            if (name != null) Cache[name] = f;
            return f;
        }

        public static Font Fallback
        {
            get
            {
                if (sFallback == null)
                {
                    // built-in font was renamed in 2022.2 (Arial.ttf -> LegacyRuntime.ttf); asking for the wrong one logs an error
                    bool legacyName = string.CompareOrdinal(Application.unityVersion, "2022.2") >= 0;
                    try { sFallback = Resources.GetBuiltinResource<Font>(legacyName ? "LegacyRuntime.ttf" : "Arial.ttf"); } catch { }
                }
                return sFallback;
            }
        }

        public static void registerFont(object font) { }
    }

    public class BitmapChar
    {
        public int charID, xOffset, yOffset, xAdvance;
        public Texture texture;
        readonly Dictionary<int, int> mKernings = new Dictionary<int, int>();
        public BitmapChar(int id, Texture tex, int xOffset, int yOffset, int xAdvance)
        {
            charID = id; texture = tex; this.xOffset = xOffset; this.yOffset = yOffset; this.xAdvance = xAdvance;
        }
        public void addKerning(int charID, int amount) => mKernings[charID] = amount;
        public int getKerning(int charID) => mKernings.TryGetValue(charID, out var k) ? k : 0;
        public float width => texture.width;
        public float height => texture.height;
    }

    public class BitmapFont
    {
        public const float NATIVE_SIZE = -1;
        public string name;
        public float size, lineHeight, baseline;
        readonly Dictionary<int, BitmapChar> mChars = new Dictionary<int, BitmapChar>();
        readonly Texture mTexture;

        public BitmapFont(Texture texture, XML fontXml)
        {
            mTexture = texture;
            var info = fontXml["info"][0];
            var common = fontXml["common"][0];
            name = info.attribute("face");
            size = AS3.toNumber(info.attribute("size"));
            lineHeight = AS3.toNumber(common.attribute("lineHeight"));
            baseline = AS3.toNumber(common.attribute("base"));
            if (size <= 0) size = 30;
            foreach (var c in fontXml["chars"]["char"])
            {
                int id = c.attribute("id").toInt();
                var region = new Rectangle(c.attribute("x").toFloat(), c.attribute("y").toFloat(), c.attribute("width").toFloat(), c.attribute("height").toFloat());
                var tex = Texture.fromTexture(texture, region);
                mChars[id] = new BitmapChar(id, tex, c.attribute("xoffset").toInt(), c.attribute("yoffset").toInt(), c.attribute("xadvance").toInt());
            }
            foreach (var k in fontXml["kernings"]["kerning"])
            {
                int first = k.attribute("first").toInt(), second = k.attribute("second").toInt();
                if (mChars.TryGetValue(second, out var bc)) bc.addKerning(first, k.attribute("amount").toInt());
            }
        }

        public BitmapChar getChar(int id) => mChars.TryGetValue(id, out var c) ? c : null;
        public Texture texture => mTexture;

        internal struct Placed { public BitmapChar c; public float x, y; }

        /// <summary>Port of Starling's BitmapFont.arrangeChars (word wrap, auto scale, alignment).</summary>
        internal List<Placed> Arrange(float width, float height, string text, ref float fontSize, string hAlign, string vAlign, bool autoScale, bool kerning, out float scale)
        {
            var result = new List<Placed>();
            scale = 1;
            if (string.IsNullOrEmpty(text)) return result;
            if (fontSize < 0) fontSize *= -size;
            bool finished = false;
            List<List<Placed>> lines = null;
            float containerWidth = 0, containerHeight = 0;
            while (!finished)
            {
                lines = new List<List<Placed>>();
                scale = fontSize / size;
                containerWidth = width / scale;
                containerHeight = height / scale;
                if (lineHeight <= containerHeight)
                {
                    int lastWhiteSpace = -1, lastCharID = -1;
                    float currentX = 0, currentY = 0;
                    var currentLine = new List<Placed>();
                    for (int i = 0; i < text.Length; i++)
                    {
                        bool lineFull = false;
                        int charID = text[i];
                        var ch = getChar(charID);
                        if (charID == 10 || charID == 13) lineFull = true;
                        else if (ch == null) Debug.LogWarning($"[BitmapFont] missing char {charID} in {name}");
                        else
                        {
                            if (charID == 32 || charID == 9) lastWhiteSpace = i;
                            if (kerning) currentX += ch.getKerning(lastCharID);
                            currentLine.Add(new Placed { c = ch, x = currentX + ch.xOffset, y = currentY + ch.yOffset });
                            currentX += ch.xAdvance;
                            lastCharID = charID;
                            if (currentLine[currentLine.Count - 1].x + ch.width > containerWidth)
                            {
                                if (autoScale && lastWhiteSpace == -1) break;
                                int numCharsToRemove = lastWhiteSpace == -1 ? 1 : i - lastWhiteSpace;
                                int removeIndex = currentLine.Count - numCharsToRemove;
                                currentLine.RemoveRange(Math.Max(0, removeIndex), Math.Min(numCharsToRemove, currentLine.Count));
                                if (currentLine.Count == 0 && lastWhiteSpace == -1) break; // single char too wide
                                i -= numCharsToRemove;
                                lineFull = true;
                            }
                        }
                        if (i == text.Length - 1)
                        {
                            lines.Add(currentLine);
                            finished = true;
                        }
                        else if (lineFull)
                        {
                            lines.Add(currentLine);
                            if (lastWhiteSpace == i) currentLine.RemoveAt(currentLine.Count - 1);
                            if (currentY + 2 * lineHeight <= containerHeight)
                            {
                                currentLine = new List<Placed>();
                                currentX = 0;
                                currentY += lineHeight;
                                lastWhiteSpace = -1;
                                lastCharID = -1;
                            }
                            else break;
                        }
                    }
                }
                if (autoScale && !finished && fontSize > 3) fontSize -= 1;
                else finished = true;
            }
            int numLines = lines.Count;
            float bottom = numLines > 0 ? numLines * lineHeight : 0;
            float yOffset = 0;
            if (vAlign == VAlign.BOTTOM) yOffset = containerHeight - bottom;
            else if (vAlign == VAlign.CENTER) yOffset = (containerHeight - bottom) / 2;
            foreach (var line in lines)
            {
                if (line.Count == 0) continue;
                float xOffset = 0;
                var last = line[line.Count - 1];
                float right = last.x - last.c.xOffset + last.c.xAdvance;
                if (hAlign == HAlign.RIGHT) xOffset = containerWidth - right;
                else if (hAlign == HAlign.CENTER) xOffset = (containerWidth - right) / 2;
                foreach (var p in line) result.Add(new Placed { c = p.c, x = scale * (p.x + xOffset), y = scale * (p.y + yOffset) });
            }
            return result;
        }
    }

    public class TextField : DisplayObject
    {
        static readonly Dictionary<string, BitmapFont> sBitmapFonts = new Dictionary<string, BitmapFont>(StringComparer.OrdinalIgnoreCase);

        public static void registerBitmapFont(BitmapFont font, string name = null) => sBitmapFonts[name ?? font.name] = font;
        public static void unregisterBitmapFont(string name, bool dispose = true) => sBitmapFonts.Remove(name);
        public static BitmapFont getBitmapFont(string name) => name != null && sBitmapFonts.TryGetValue(name, out var f) ? f : null;

        float mW, mH;
        string mText, mFontName, mHAlign = HAlign.CENTER, mVAlign = VAlign.CENTER, mAutoSize = TextFieldAutoSize.NONE;
        float mFontSize;
        uint mColor;
        bool mBold, mItalic, mUnderline, mAutoScale, mKerning = true, mBorder;
        readonly UnityEngine.UI.Text mUText;
        RectTransform mGlyphRoot;
        readonly List<GameObject> mGlyphs = new List<GameObject>();
        Rectangle mTextBounds;
        public bool batchable;
        public object nativeFilters;

        public TextField(float width, float height, string text, string fontName = "Verdana", float fontSize = 12, uint color = 0, bool bold = false)
        {
            mW = width; mH = height; mText = text ?? ""; mFontName = fontName; mFontSize = fontSize; mColor = color; mBold = bold;
            mUText = inner.gameObject.AddComponent<UnityEngine.UI.Text>();
            mUText.raycastTarget = false;
            mUText.horizontalOverflow = HorizontalWrapMode.Wrap;
            mUText.verticalOverflow = VerticalWrapMode.Overflow;
            mUText.supportRichText = false;
            Redraw();
        }

        internal override Rectangle LocalBounds => new Rectangle(0, 0, mW, mH);

        public override float width { get => base.width; set { mW = value / Math.Max(0.0001f, scaleX); Redraw(); } }
        public override float height { get => base.height; set { mH = value / Math.Max(0.0001f, scaleY); Redraw(); } }

        public string text { get => mText; set { mText = value ?? ""; Redraw(); } }
        public string fontName { get => mFontName; set { mFontName = value; Redraw(); } }
        public float fontSize { get => mFontSize; set { mFontSize = value; Redraw(); } }
        public uint color { get => mColor; set { mColor = value; Redraw(); } }
        public string hAlign { get => mHAlign; set { mHAlign = value; Redraw(); } }
        public string vAlign { get => mVAlign; set { mVAlign = value; Redraw(); } }
        public bool bold { get => mBold; set { mBold = value; Redraw(); } }
        public bool italic { get => mItalic; set { mItalic = value; Redraw(); } }
        public bool underline { get => mUnderline; set { mUnderline = value; } }
        public bool kerning { get => mKerning; set { mKerning = value; Redraw(); } }
        public bool autoScale { get => mAutoScale; set { mAutoScale = value; Redraw(); } }
        public bool border { get => mBorder; set { mBorder = value; } }
        public string autoSize { get => mAutoSize; set { mAutoSize = value; Redraw(); } }
        public Rectangle textBounds => (mTextBounds ?? new Rectangle(0, 0, mW, mH)).clone();
        public void redraw() => Redraw();

        void ClearGlyphs()
        {
            foreach (var g in mGlyphs) if (g != null) UObject.Destroy(g);
            mGlyphs.Clear();
        }

        protected virtual void Redraw()
        {
            if (mUText == null || isDisposed) return;
            var bmf = getBitmapFont(mFontName);
            ClearGlyphs();
            inner.sizeDelta = new Vector2(mW, mH);
            if (bmf != null)
            {
                mUText.enabled = false;
                if (mGlyphRoot == null)
                {
                    var go2 = new GameObject("glyphs", typeof(RectTransform));
                    mGlyphRoot = (RectTransform)go2.transform;
                    mGlyphRoot.SetParent(inner, false);
                    Setup(mGlyphRoot);
                }
                float fs = mFontSize;
                float w = mW, h = mH;
                if (mAutoSize != TextFieldAutoSize.NONE) { w = mAutoSize == TextFieldAutoSize.VERTICAL ? mW : 100000; h = mAutoSize == TextFieldAutoSize.HORIZONTAL ? mH : 100000; }
                var placed = bmf.Arrange(w, h, mText, ref fs, mHAlign, mVAlign, mAutoScale, mKerning, out float sc);
                float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
                foreach (var p in placed)
                {
                    var g = new GameObject("g", typeof(RectTransform));
                    var r = (RectTransform)g.transform;
                    r.SetParent(mGlyphRoot, false);
                    Setup(r);
                    r.anchoredPosition = new Vector2(p.x, -p.y);
                    r.sizeDelta = new Vector2(p.c.texture.width * sc, p.c.texture.height * sc);
                    var ri = g.AddComponent<RawImage>();
                    ri.raycastTarget = false;
                    ri.texture = p.c.texture.Native;
                    ri.uvRect = p.c.texture.UvRect;
                    ri.color = Color.ToUnity(mColor);
                    mGlyphs.Add(g);
                    minX = Math.Min(minX, p.x); minY = Math.Min(minY, p.y);
                    maxX = Math.Max(maxX, p.x + p.c.texture.width * sc); maxY = Math.Max(maxY, p.y + p.c.texture.height * sc);
                }
                mTextBounds = placed.Count > 0 ? new Rectangle(minX, minY, maxX - minX, maxY - minY) : new Rectangle();
                if (mAutoSize != TextFieldAutoSize.NONE && placed.Count > 0)
                {
                    if (mAutoSize != TextFieldAutoSize.VERTICAL) mW = maxX + 2;
                    if (mAutoSize != TextFieldAutoSize.HORIZONTAL) mH = maxY + 2;
                    inner.sizeDelta = new Vector2(mW, mH);
                }
            }
            else
            {
                mUText.enabled = true;
                mUText.font = Fonts.Get(mFontName);
                mUText.text = mText;
                mUText.fontSize = Mathf.Max(1, Mathf.RoundToInt(mFontSize));
                mUText.fontStyle = mBold && mItalic ? FontStyle.BoldAndItalic : mBold ? FontStyle.Bold : mItalic ? FontStyle.Italic : FontStyle.Normal;
                mUText.color = Color.ToUnity(mColor);
                mUText.alignment = Anchor(mHAlign, mVAlign);
                mUText.resizeTextForBestFit = mAutoScale;
                mUText.resizeTextMaxSize = mUText.fontSize;
                mUText.resizeTextMinSize = 1;
                mUText.lineSpacing = 1;
                if (mAutoSize != TextFieldAutoSize.NONE)
                {
                    if (mAutoSize != TextFieldAutoSize.VERTICAL) mW = Mathf.Ceil(mUText.preferredWidth) + 4;
                    if (mAutoSize != TextFieldAutoSize.HORIZONTAL) mH = Mathf.Ceil(mUText.preferredHeight) + 4;
                    inner.sizeDelta = new Vector2(mW, mH);
                }
                float pw = Mathf.Min(mW, mUText.preferredWidth), ph = Mathf.Min(mH, mUText.preferredHeight);
                float tx = mHAlign == HAlign.LEFT ? 0 : mHAlign == HAlign.RIGHT ? mW - pw : (mW - pw) / 2;
                float ty = mVAlign == VAlign.TOP ? 0 : mVAlign == VAlign.BOTTOM ? mH - ph : (mH - ph) / 2;
                mTextBounds = new Rectangle(tx, ty, pw, ph);
            }
            SyncInner();
        }

        static TextAnchor Anchor(string h, string v)
        {
            int col = h == HAlign.LEFT ? 0 : h == HAlign.RIGHT ? 2 : 1;
            int row = v == VAlign.TOP ? 0 : v == VAlign.BOTTOM ? 2 : 1;
            return (TextAnchor)(row * 3 + col);
        }
    }

    /// <summary>flash.text.TextFormat stand-in.</summary>
    public class TextFormat
    {
        public string font, align = "left";
        public float size = 12;
        public uint color;
        public bool bold;
        public TextFormat(string font = null, float size = 12, uint color = 0, bool bold = false) { this.font = font; this.size = size; this.color = color; this.bold = bold; }
    }

    public static class TextFieldType { public const string INPUT = "input", DYNAMIC = "dynamic"; }

    /// <summary>
    /// Replacement for an editable flash.text.TextField overlay (player name entry). Lives on the
    /// Starling stage, captures keyboard input while visible and focused, and dispatches KEY_DOWN / KEY_UP.
    /// </summary>
    public class NativeTextField : TextField
    {
        public int maxChars;
        public string restrict;
        public string type = TextFieldType.DYNAMIC;
        public bool needsSoftKeyboard, embedFonts, selectable = true;
        TextFormat mFormat;
        static NativeTextField sFocus;

        public NativeTextField() : base(100, 20, "", "Verdana", 12, 0)
        {
            hAlign = HAlign.LEFT;
            vAlign = VAlign.CENTER;
            addEventListener(TouchEvent.TOUCH, OnTouch);
            addEventListener(Event.ENTER_FRAME, OnFrame);
        }

        public TextFormat defaultTextFormat
        {
            get => mFormat;
            set
            {
                mFormat = value;
                if (value == null) return;
                fontName = value.font; fontSize = value.size; color = value.color; bold = value.bold;
                hAlign = value.align == "center" ? HAlign.CENTER : value.align == "right" ? HAlign.RIGHT : HAlign.LEFT;
            }
        }

        public int length => text.Length;

        void OnTouch(Event e)
        {
            var t = ((TouchEvent)e).getTouch(this, TouchPhase.BEGAN);
            if (t != null) sFocus = this;
        }

        bool Accepts(char c)
        {
            if (string.IsNullOrEmpty(restrict)) return !char.IsControl(c);
            // supports simple ranges such as "a-zA-Z " (the only form the game uses)
            for (int i = 0; i < restrict.Length; i++)
            {
                if (i + 2 < restrict.Length && restrict[i + 1] == '-')
                {
                    if (c >= restrict[i] && c <= restrict[i + 2]) return true;
                    i += 2;
                }
                else if (restrict[i] == c) return true;
            }
            return false;
        }

        void OnFrame(Event e)
        {
            if (type != TextFieldType.INPUT || !visible || stage == null) { if (sFocus == this) sFocus = null; return; }
            if (sFocus == null) sFocus = this;
            if (sFocus != this) return;
            string before = text;
            foreach (char c in Input.inputString)
            {
                if (c == '\b') { if (before.Length > 0) before = before.Substring(0, before.Length - 1); }
                else if (c == '\n' || c == '\r') { }
                else if ((maxChars <= 0 || before.Length < maxChars) && Accepts(c)) before += c;
            }
            if (before != text) { text = before; dispatchEventWith(Event.CHANGE); }
            if (Input.inputString.Length > 0)
            {
                dispatchEvent(new KeyboardEvent(KeyboardEvent.KEY_DOWN, Input.inputString[0], char.ToUpperInvariant(Input.inputString[0])));
                dispatchEvent(new KeyboardEvent(KeyboardEvent.KEY_UP, Input.inputString[0], char.ToUpperInvariant(Input.inputString[0])));
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                dispatchEvent(new KeyboardEvent(KeyboardEvent.KEY_DOWN, 13, Keyboard.ENTER));
                dispatchEvent(new KeyboardEvent(KeyboardEvent.KEY_UP, 13, Keyboard.ENTER));
            }
        }
    }
}
