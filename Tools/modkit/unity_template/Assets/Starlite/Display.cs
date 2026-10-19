// Starling-style display list. Logic (transforms, bounds, hit testing, events) is pure C# and
// matches Starling 1.x; every node mirrors itself onto a UGUI RectTransform for rendering:
//
//   node.go (RectTransform)    position / rotation / scale / alpha / visibility
//     node.inner (RectTransform) offset by -pivot; holds the graphic and all children
//
// Unity UI is y-up, Starling is y-down: anchoredPosition = (x, -y), rotation = -degrees.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UObject = UnityEngine.Object;

namespace Starlite
{
    public static class BlendMode
    {
        public const string AUTO = "auto", NONE = "none", NORMAL = "normal", ADD = "add", MULTIPLY = "multiply",
            SCREEN = "screen", ERASE = "erase", BELOW = "below";
    }

    public abstract class DisplayObject : EventDispatcher
    {
        float mX, mY, mPivotX, mPivotY, mScaleX = 1, mScaleY = 1, mRotation, mSkewX, mSkewY, mAlpha = 1;
        bool mVisible = true;
        string mBlendMode = BlendMode.AUTO;
        public bool touchable = true;
        public string name;
        public virtual bool useHandCursor { get; set; }
        public object filter; // filters are not emulated (glow etc.)
        internal DisplayObjectContainer mParent;

        internal readonly GameObject go;
        internal readonly RectTransform rt;
        internal readonly RectTransform inner;
        CanvasGroup mCanvasGroup;

        protected DisplayObject()
        {
            go = new GameObject(GetType().Name, typeof(RectTransform));
            rt = (RectTransform)go.transform;
            Setup(rt);
            var innerGo = new GameObject("inner", typeof(RectTransform));
            inner = (RectTransform)innerGo.transform;
            inner.SetParent(rt, false);
            Setup(inner);
            rt.SetParent(Starling.Limbo, false);
        }

        internal static void Setup(RectTransform r)
        {
            r.anchorMin = r.anchorMax = new Vector2(0, 1);
            r.pivot = new Vector2(0, 1);
            r.sizeDelta = Vector2.zero;
            r.anchoredPosition = Vector2.zero;
        }

        // ----------------------------------------------------------------- properties
        public virtual float x { get => mX; set { if (mX != value) { mX = value; SyncTransform(); } } }
        public virtual float y { get => mY; set { if (mY != value) { mY = value; SyncTransform(); } } }
        public float pivotX { get => mPivotX; set { if (mPivotX != value) { mPivotX = value; SyncInner(); } } }
        public float pivotY { get => mPivotY; set { if (mPivotY != value) { mPivotY = value; SyncInner(); } } }
        public float scaleX { get => mScaleX; set { if (mScaleX != value) { mScaleX = value; SyncTransform(); } } }
        public float scaleY { get => mScaleY; set { if (mScaleY != value) { mScaleY = value; SyncTransform(); } } }
        public float scale { get => mScaleX; set { mScaleX = mScaleY = value; SyncTransform(); } }
        public float skewX { get => mSkewX; set { mSkewX = value; SyncTransform(); } }
        public float skewY { get => mSkewY; set { mSkewY = value; SyncTransform(); } }

        /// <summary>Rotation in radians, normalized to [-PI, PI] like Starling.</summary>
        public float rotation
        {
            get => mRotation;
            set
            {
                while (value < -Mathf.PI) value += Mathf.PI * 2;
                while (value > Mathf.PI) value -= Mathf.PI * 2;
                mRotation = value;
                SyncTransform();
            }
        }

        public float alpha
        {
            get => mAlpha;
            set
            {
                mAlpha = Mathf.Clamp01(value);
                if (isDisposed) return;
                if (mCanvasGroup == null && mAlpha < 1) mCanvasGroup = go.AddComponent<CanvasGroup>();
                if (mCanvasGroup != null) { mCanvasGroup.alpha = mAlpha; mCanvasGroup.blocksRaycasts = false; }
            }
        }

        public bool visible
        {
            get => mVisible;
            set { mVisible = value; if (!isDisposed && go != null) go.SetActive(value); }
        }

        public string blendMode { get => mBlendMode; set { mBlendMode = value; OnBlendModeChanged(); } }
        protected virtual void OnBlendModeChanged() { }

        public DisplayObjectContainer parent => mParent;

        public DisplayObject root
        {
            get
            {
                DisplayObject cur = this;
                while (cur.mParent != null)
                {
                    if (cur.mParent is Stage) return cur;
                    cur = cur.mParent;
                }
                return null;
            }
        }

        public Stage stage
        {
            get
            {
                DisplayObject cur = this;
                while (cur.mParent != null) cur = cur.mParent;
                return cur as Stage;
            }
        }

        public DisplayObject Base
        {
            get
            {
                DisplayObject cur = this;
                while (cur.mParent != null) cur = cur.mParent;
                return cur;
            }
        }

        public virtual float width
        {
            get => getBounds(mParent).width;
            set
            {
                scaleX = 1;
                float w = width;
                if (w != 0) scaleX = value / w;
            }
        }

        public virtual float height
        {
            get => getBounds(mParent).height;
            set
            {
                scaleY = 1;
                float h = height;
                if (h != 0) scaleY = value / h;
            }
        }

        public Rectangle bounds => getBounds(mParent);

        public void alignPivot(string hAlign = HAlign.CENTER, string vAlign = VAlign.CENTER)
        {
            var b = getBounds(this);
            pivotX = hAlign == HAlign.LEFT ? b.x : hAlign == HAlign.CENTER ? b.x + b.width / 2 : b.x + b.width;
            pivotY = vAlign == VAlign.TOP ? b.y : vAlign == VAlign.CENTER ? b.y + b.height / 2 : b.y + b.height;
        }

        // ----------------------------------------------------------------- transforms
        public Matrix transformationMatrix
        {
            get
            {
                var m = new Matrix();
                if (mSkewX == 0 && mSkewY == 0)
                {
                    float cos = Mathf.Cos(mRotation), sin = Mathf.Sin(mRotation);
                    m.a = mScaleX * cos; m.b = mScaleX * sin; m.c = -mScaleY * sin; m.d = mScaleY * cos;
                }
                else
                {
                    m.a = mScaleX * Mathf.Cos(mRotation + mSkewY); m.b = mScaleX * Mathf.Sin(mRotation + mSkewY);
                    m.c = -mScaleY * Mathf.Sin(mRotation + mSkewX); m.d = mScaleY * Mathf.Cos(mRotation + mSkewX);
                }
                m.tx = mX - mPivotX * m.a - mPivotY * m.c;
                m.ty = mY - mPivotX * m.b - mPivotY * m.d;
                return m;
            }
        }

        Matrix ToBase()
        {
            var m = new Matrix();
            for (DisplayObject cur = this; cur != null && cur.mParent != null; cur = cur.mParent)
                m.concat(cur.transformationMatrix);
            return m;
        }

        public Matrix getTransformationMatrix(DisplayObject targetSpace, Matrix result = null)
        {
            Matrix m;
            if (targetSpace == this) m = new Matrix();
            else if (targetSpace == mParent || (targetSpace == null && mParent == null))
                m = mParent == null && targetSpace == null ? new Matrix() : transformationMatrix;
            else
            {
                m = ToBase();
                if (targetSpace != null)
                {
                    var t = targetSpace.ToBase();
                    t.invert();
                    m.concat(t);
                }
            }
            if (result != null) { result.copyFrom(m); return result; }
            return m;
        }

        public Point localToGlobal(Point p, Point result = null)
        {
            var r = ToBase().transformPoint(p);
            if (result != null) { result.setTo(r.x, r.y); return result; }
            return r;
        }

        public Point globalToLocal(Point p, Point result = null)
        {
            var m = ToBase();
            m.invert();
            var r = m.transformPoint(p);
            if (result != null) { result.setTo(r.x, r.y); return result; }
            return r;
        }

        /// <summary>Local, untransformed bounds of this object's own graphics (null for containers).</summary>
        internal virtual Rectangle LocalBounds => null;

        public virtual Rectangle getBounds(DisplayObject targetSpace, Rectangle result = null)
        {
            var lb = LocalBounds ?? new Rectangle();
            var r = TransformRect(lb, getTransformationMatrix(targetSpace));
            if (result != null) { result.setTo(r.x, r.y, r.width, r.height); return result; }
            return r;
        }

        internal static Rectangle TransformRect(Rectangle lb, Matrix m)
        {
            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            void P(float px, float py)
            {
                m.transform(px, py, out var ox, out var oy);
                if (ox < minX) minX = ox;
                if (oy < minY) minY = oy;
                if (ox > maxX) maxX = ox;
                if (oy > maxY) maxY = oy;
            }
            P(lb.x, lb.y); P(lb.right, lb.y); P(lb.x, lb.bottom); P(lb.right, lb.bottom);
            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        public virtual DisplayObject hitTest(Point localPoint, bool forTouch = false)
        {
            if (forTouch && (!mVisible || !touchable)) return null;
            var lb = LocalBounds;
            return lb != null && lb.containsPoint(localPoint) ? this : null;
        }

        // ----------------------------------------------------------------- lifecycle
        public void removeFromParent(bool dispose = false)
        {
            if (mParent != null) mParent.removeChild(this, dispose);
            else if (dispose) this.dispose();
        }

        public bool isDisposed { get; private set; }

        public virtual void dispose()
        {
            if (isDisposed) return;
            isDisposed = true;
            removeEventListeners();
            Starling.UnregisterEnterFrame(this);
            if (go != null) UObject.Destroy(go);
        }

        internal override void OnListenerAdded(string type)
        {
            if (type == Event.ENTER_FRAME) Starling.RegisterEnterFrame(this);
        }

        // ----------------------------------------------------------------- view sync
        void SyncTransform()
        {
            if (isDisposed || rt == null) return;
            rt.anchoredPosition = new Vector2(mX, -mY);
            rt.localScale = new Vector3(mScaleX, mScaleY, 1);
            rt.localEulerAngles = new Vector3(0, 0, -mRotation * Mathf.Rad2Deg);
        }

        protected void SyncInner()
        {
            if (isDisposed) return;
            var o = InnerOffset;
            inner.anchoredPosition = new Vector2(o.x, -o.y);
        }

        /// <summary>Starling-space offset of the inner rect (default: -pivot).</summary>
        protected virtual Vector2 InnerOffset => new Vector2(-mPivotX, -mPivotY);

        public override string ToString() => $"[{GetType().Name} {name}]";
    }

    public class DisplayObjectContainer : DisplayObject
    {
        internal readonly List<DisplayObject> mChildren = new List<DisplayObject>();

        /// <summary>Transform children are parented to (changes when a clipRect mask is active).</summary>
        internal virtual RectTransform ChildRoot => inner;

        public int numChildren => mChildren.Count;

        public DisplayObject addChild(DisplayObject child) => addChildAt(child, mChildren.Count);

        public DisplayObject addChildAt(DisplayObject child, int index)
        {
            if (child == null) throw new ArgumentNullException(nameof(child));
            if (index < 0 || index > mChildren.Count) throw new ArgumentOutOfRangeException(nameof(index));
            if (child.mParent == this)
            {
                setChildIndex(child, Math.Min(index, mChildren.Count - 1));
                return child;
            }
            child.removeFromParent();
            mChildren.Insert(index, child);
            child.mParent = this;
            if (child.rt != null)
            {
                child.rt.SetParent(ChildRoot, false);
                ResyncSiblings(index);
            }
            child.dispatchEvent(new Event(Event.ADDED, true));
            if (stage != null) BroadcastRecursive(child, Event.ADDED_TO_STAGE);
            return child;
        }

        void ResyncSiblings(int from)
        {
            for (int i = from; i < mChildren.Count; i++)
                if (mChildren[i].rt != null) mChildren[i].rt.SetSiblingIndex(i);
        }

        internal static void BroadcastRecursive(DisplayObject obj, string type)
        {
            var e = new Event(type);
            var list = new List<DisplayObject>();
            Collect(obj, list);
            foreach (var o in list) o.dispatchEvent(e);
        }

        static void Collect(DisplayObject o, List<DisplayObject> list)
        {
            list.Add(o);
            if (o is DisplayObjectContainer c)
                foreach (var ch in c.mChildren.ToArray()) Collect(ch, list);
        }

        public DisplayObject removeChild(DisplayObject child, bool dispose = false)
        {
            int i = getChildIndex(child);
            if (i != -1) removeChildAt(i, dispose);
            return child;
        }

        public DisplayObject removeChildAt(int index, bool dispose = false)
        {
            if (index < 0 || index >= mChildren.Count) throw new ArgumentOutOfRangeException(nameof(index));
            var child = mChildren[index];
            child.dispatchEvent(new Event(Event.REMOVED, true));
            if (stage != null) BroadcastRecursive(child, Event.REMOVED_FROM_STAGE);
            index = mChildren.IndexOf(child); // listeners may have changed the list
            if (index >= 0) mChildren.RemoveAt(index);
            child.mParent = null;
            if (child.rt != null && !child.isDisposed) child.rt.SetParent(Starling.Limbo, false);
            if (dispose) child.dispose();
            return child;
        }

        public void removeChildren(int beginIndex = 0, int endIndex = -1, bool dispose = false)
        {
            if (endIndex < 0 || endIndex >= mChildren.Count) endIndex = mChildren.Count - 1;
            for (int i = beginIndex; i <= endIndex; i++) removeChildAt(beginIndex, dispose);
        }

        public DisplayObject getChildAt(int index)
        {
            if (index < 0) index += mChildren.Count;
            return index >= 0 && index < mChildren.Count ? mChildren[index] : null;
        }

        public DisplayObject getChildByName(string n)
        {
            foreach (var c in mChildren) if (c.name == n) return c;
            return null;
        }

        public int getChildIndex(DisplayObject child) => mChildren.IndexOf(child);

        public void setChildIndex(DisplayObject child, int index)
        {
            int old = getChildIndex(child);
            if (old == index) return;
            if (old == -1) throw new ArgumentException("Not a child of this container");
            mChildren.RemoveAt(old);
            index = Math.Max(0, Math.Min(index, mChildren.Count));
            mChildren.Insert(index, child);
            ResyncSiblings(Math.Min(old, index));
        }

        public void swapChildren(DisplayObject a, DisplayObject b) => swapChildrenAt(getChildIndex(a), getChildIndex(b));

        public void swapChildrenAt(int i1, int i2)
        {
            var a = mChildren[i1];
            var b = mChildren[i2];
            mChildren[i1] = b;
            mChildren[i2] = a;
            ResyncSiblings(Math.Min(i1, i2));
        }

        public void sortChildren(Comparison<DisplayObject> compare)
        {
            mChildren.Sort(compare);
            ResyncSiblings(0);
        }

        public bool contains(DisplayObject child)
        {
            for (var c = child; c != null; c = c.mParent) if (c == this) return true;
            return false;
        }

        public override Rectangle getBounds(DisplayObject targetSpace, Rectangle result = null)
        {
            Rectangle r;
            if (mChildren.Count == 0)
            {
                var m = getTransformationMatrix(targetSpace);
                r = new Rectangle(m.tx, m.ty, 0, 0);
            }
            else
            {
                float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
                foreach (var c in mChildren)
                {
                    var b = c.getBounds(targetSpace);
                    minX = Math.Min(minX, b.x); minY = Math.Min(minY, b.y);
                    maxX = Math.Max(maxX, b.right); maxY = Math.Max(maxY, b.bottom);
                }
                r = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            }
            if (result != null) { result.setTo(r.x, r.y, r.width, r.height); return result; }
            return r;
        }

        public override DisplayObject hitTest(Point localPoint, bool forTouch = false)
        {
            if (forTouch && (!visible || !touchable)) return null;
            for (int i = mChildren.Count - 1; i >= 0; i--)
            {
                var child = mChildren[i];
                var m = child.transformationMatrix;
                m.invert();
                var p = m.transformPoint(localPoint);
                var hit = child.hitTest(p, forTouch);
                if (hit != null) return hit;
            }
            return null;
        }

        public override void dispose()
        {
            foreach (var c in mChildren.ToArray()) c.dispose();
            mChildren.Clear();
            base.dispose();
        }

        public void broadcastEvent(Event e)
        {
            var list = new List<DisplayObject>();
            Collect(this, list);
            foreach (var o in list) o.dispatchEvent(e);
        }

        public void broadcastEventWith(string type, object data = null) => broadcastEvent(new Event(type, false, data));
    }

    public class Sprite : DisplayObjectContainer
    {
        Rectangle mClipRect;
        RectTransform mMask, mMaskContent;

        internal static readonly HashSet<Sprite> Clipped = new HashSet<Sprite>();
        float mClipX = float.NaN, mClipY, mClipW, mClipH;

        /// <summary>Like Starling, the rectangle is kept by reference: changing its fields later moves the mask.</summary>
        public Rectangle clipRect
        {
            get => mClipRect;
            set
            {
                mClipRect = value;
                if (value != null) Clipped.Add(this); else Clipped.Remove(this);
                if (mClipRect != null && mMask == null)
                {
                    var m = new GameObject("clip", typeof(RectTransform), typeof(RectMask2D));
                    mMask = (RectTransform)m.transform;
                    mMask.SetParent(inner, false);
                    Setup(mMask);
                    var c = new GameObject("content", typeof(RectTransform));
                    mMaskContent = (RectTransform)c.transform;
                    mMaskContent.SetParent(mMask, false);
                    Setup(mMaskContent);
                    foreach (var ch in mChildren) if (ch.rt != null) ch.rt.SetParent(mMaskContent, false);
                }
                else if (mClipRect == null && mMask != null)
                {
                    foreach (var ch in mChildren) if (ch.rt != null) ch.rt.SetParent(inner, false);
                    UObject.Destroy(mMask.gameObject);
                    mMask = mMaskContent = null;
                }
                mClipX = float.NaN;
                SyncClip();
                for (int i = 0; i < mChildren.Count; i++) if (mChildren[i].rt != null) mChildren[i].rt.SetSiblingIndex(i);
            }
        }

        internal void SyncClip()
        {
            if (isDisposed || mMask == null || mClipRect == null) return;
            if (mClipRect.x == mClipX && mClipRect.y == mClipY && mClipRect.width == mClipW && mClipRect.height == mClipH) return;
            mClipX = mClipRect.x; mClipY = mClipRect.y; mClipW = mClipRect.width; mClipH = mClipRect.height;
            mMask.anchoredPosition = new Vector2(mClipX, -mClipY);
            mMask.sizeDelta = new Vector2(mClipW, mClipH);
            mMaskContent.anchoredPosition = new Vector2(-mClipX, mClipY);
        }

        public override void dispose()
        {
            Clipped.Remove(this);
            base.dispose();
        }

        internal override RectTransform ChildRoot => mMaskContent != null ? mMaskContent : inner;

        public override DisplayObject hitTest(Point localPoint, bool forTouch = false)
        {
            if (mClipRect != null && !mClipRect.containsPoint(localPoint)) return null;
            return base.hitTest(localPoint, forTouch);
        }

        public override Rectangle getBounds(DisplayObject targetSpace, Rectangle result = null)
        {
            var b = base.getBounds(targetSpace, result);
            if (mClipRect != null)
            {
                var clip = TransformRect(mClipRect, getTransformationMatrix(targetSpace));
                var i = b.intersection(clip);
                b.setTo(i.x, i.y, i.width, i.height);
            }
            return b;
        }

        public void flatten() { }
        public void unflatten() { }
        public bool isFlattened => false;
    }

    /// <summary>Colored rectangle.</summary>
    public class Quad : DisplayObject
    {
        protected float mWidth, mHeight;
        protected uint mColor;
        protected readonly RawImage graphic;
        protected RectTransform graphicRt;

        public Quad(float width, float height, uint color = 0xffffff, bool premultipliedAlpha = true)
        {
            mWidth = width;
            mHeight = height;
            mColor = color;
            graphic = CreateGraphic();
            graphic.texture = Starling.WhiteTexture;
            SyncGraphic();
        }

        protected virtual RawImage CreateGraphic()
        {
            var img = inner.gameObject.AddComponent<RawImage>();
            img.raycastTarget = false;
            graphicRt = inner;
            return img;
        }

        internal override Rectangle LocalBounds => new Rectangle(0, 0, mWidth, mHeight);

        public uint color
        {
            get => mColor;
            set { mColor = value; if (!isDisposed) graphic.color = Color.ToUnity(value); }
        }

        public void setVertexColor(int vertexID, uint color) => this.color = color;
        public uint getVertexColor(int vertexID) => mColor;
        public void setVertexAlpha(int vertexID, float alpha) { }

        protected virtual void SyncGraphic()
        {
            if (isDisposed) return;
            graphicRt.sizeDelta = new Vector2(mWidth, mHeight);
            graphic.color = Color.ToUnity(mColor);
            SyncInner();
        }

        protected override void OnBlendModeChanged()
        {
            if (!isDisposed) graphic.material = blendMode == BlendMode.ADD ? Starling.AdditiveMaterial : null;
        }
    }

    public class Image : Quad
    {
        Texture mTexture;
        RectTransform mGfx;
        public string smoothing = "bilinear";

        public Image(Texture texture) : base(texture?.width ?? 1, texture?.height ?? 1)
        {
            if (texture == null) throw new ArgumentNullException(nameof(texture), "Image texture cannot be null");
            mTexture = texture;
            ApplyTexture();
            color = 0xffffff;
        }

        protected override RawImage CreateGraphic()
        {
            var g = new GameObject("gfx", typeof(RectTransform));
            mGfx = (RectTransform)g.transform;
            mGfx.SetParent(inner, false);
            Setup(mGfx);
            graphicRt = mGfx;
            var img = g.AddComponent<RawImage>();
            img.raycastTarget = false;
            return img;
        }

        public Texture texture
        {
            get => mTexture;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value != mTexture) { mTexture = value; ApplyTexture(); }
            }
        }

        void ApplyTexture()
        {
            // disposed objects keep their Starling state (Starling tolerates use after dispose) but no longer touch Unity
            if (isDisposed) return;
            graphic.texture = mTexture.Native;
            graphic.uvRect = mTexture.UvRect;
            SyncGraphic();
        }

        public void readjustSize()
        {
            mWidth = mTexture.width;
            mHeight = mTexture.height;
            SyncGraphic();
        }

        protected override void SyncGraphic()
        {
            if (isDisposed) return;
            if (mTexture == null) { base.SyncGraphic(); return; }
            // the texture's trimmed region is drawn at its frame offset, scaled to the quad size
            float sx = mWidth / mTexture.width, sy = mHeight / mTexture.height;
            var f = mTexture.frame;
            float ox = f != null ? -f.x : 0, oy = f != null ? -f.y : 0;
            mGfx.anchoredPosition = new Vector2(ox * sx, -oy * sy);
            mGfx.sizeDelta = new Vector2(mTexture.RegionWidth * sx, mTexture.RegionHeight * sy);
            graphic.color = Color.ToUnity(mColor);
            SyncInner();
        }
    }

    public class MovieClip : Image, IAnimatable
    {
        readonly List<Texture> mTextures = new List<Texture>();
        readonly List<float> mDurations = new List<float>();
        float mDefaultFrameDuration, mCurrentTime;
        int mCurrentFrame;
        bool mLoop = true, mPlaying = true;

        public MovieClip(List<Texture> textures, float fps = 12) : base(textures != null && textures.Count > 0 ? textures[0] : throw new ArgumentException("Empty texture list"))
        {
            mDefaultFrameDuration = 1f / fps;
            foreach (var t in textures) { mTextures.Add(t); mDurations.Add(mDefaultFrameDuration); }
        }

        public void addFrame(Texture t, float duration = -1) { mTextures.Add(t); mDurations.Add(duration < 0 ? mDefaultFrameDuration : duration); }
        public int numFrames => mTextures.Count;
        public float totalTime { get { float s = 0; foreach (var d in mDurations) s += d; return s; } }
        public bool loop { get => mLoop; set => mLoop = value; }
        public bool isPlaying => mPlaying && (mLoop || mCurrentTime < totalTime);
        public bool isComplete => !mLoop && mCurrentTime >= totalTime;

        public float fps
        {
            get => 1f / mDefaultFrameDuration;
            set
            {
                float nd = 1f / value;
                for (int i = 0; i < mDurations.Count; i++) mDurations[i] *= nd / mDefaultFrameDuration;
                mDefaultFrameDuration = nd;
            }
        }

        public int currentFrame
        {
            get => mCurrentFrame;
            set
            {
                mCurrentFrame = Math.Max(0, Math.Min(value, mTextures.Count - 1));
                mCurrentTime = 0;
                for (int i = 0; i < mCurrentFrame; i++) mCurrentTime += mDurations[i];
                texture = mTextures[mCurrentFrame];
            }
        }

        public void play() => mPlaying = true;
        public void pause() => mPlaying = false;
        public void stop() { mPlaying = false; currentFrame = 0; }

        public void advanceTime(float passedTime)
        {
            if (isDisposed) { Starling.juggler.remove(this); return; }
            if (!mPlaying || passedTime <= 0 || mTextures.Count == 0) return;
            float total = totalTime;
            int prev = mCurrentFrame;
            bool completed = false;
            if (mLoop && mCurrentTime >= total) { mCurrentTime = 0; mCurrentFrame = 0; }
            if (mCurrentTime < total)
            {
                mCurrentTime += passedTime;
                float acc = 0;
                int f = 0;
                while (f < mTextures.Count && acc + mDurations[f] <= mCurrentTime) { acc += mDurations[f]; f++; }
                if (f >= mTextures.Count)
                {
                    completed = true;
                    if (mLoop) { mCurrentTime -= total; f = 0; acc = 0; while (f < mTextures.Count - 1 && acc + mDurations[f] <= mCurrentTime) { acc += mDurations[f]; f++; } }
                    else { mCurrentTime = total; f = mTextures.Count - 1; }
                }
                mCurrentFrame = f;
            }
            if (mCurrentFrame != prev) texture = mTextures[mCurrentFrame];
            if (completed) dispatchEventWith(Event.COMPLETE);
        }
    }
}
