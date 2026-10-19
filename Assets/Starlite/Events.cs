using System;
using System.Collections.Generic;

namespace Starlite
{
    public class Event
    {
        public const string ADDED = "added", ADDED_TO_STAGE = "addedToStage", REMOVED = "removed",
            REMOVED_FROM_STAGE = "removedFromStage", ENTER_FRAME = "enterFrame", COMPLETE = "complete",
            TRIGGERED = "triggered", CHANGE = "change", RESIZE = "resize", ACTIVATE = "activate",
            DEACTIVATE = "deactivate", SOUND_COMPLETE = "soundComplete", REMOVE_FROM_JUGGLER = "removeFromJuggler",
            IO_ERROR = "ioError", SELECT = "select", CANCEL = "cancel", CLOSE = "close", OPEN = "open",
            SCROLL = "scroll", UPDATE = "update", READY = "ready", CONTEXT3D_CREATE = "context3DCreate",
            ROOT_CREATED = "rootCreated", FLATTEN = "flatten", EXITING = "exiting";

        public string type { get; internal set; }
        public bool bubbles { get; internal set; }
        public object data { get; internal set; }
        public EventDispatcher target { get; internal set; }
        public EventDispatcher currentTarget { get; internal set; }
        internal bool stopsPropagation, stopsImmediatePropagation, defaultPrevented;

        public Event(string type, bool bubbles = false, object data = null) { this.type = type; this.bubbles = bubbles; this.data = data; }
        public void stopPropagation() => stopsPropagation = true;
        public void stopImmediatePropagation() { stopsPropagation = stopsImmediatePropagation = true; }
        public void preventDefault() => defaultPrevented = true;
        public bool isDefaultPrevented() => defaultPrevented;
        public override string ToString() => $"[{GetType().Name} type=\"{type}\" bubbles={bubbles}]";
    }

    public class EnterFrameEvent : Event
    {
        public float passedTime { get; internal set; }
        public EnterFrameEvent(string type, float passedTime, bool bubbles = false) : base(type, bubbles) { this.passedTime = passedTime; }
    }

    public class ResizeEvent : Event
    {
        public new const string RESIZE = "resize";
        public int width, height;
        public ResizeEvent(string type, int width, int height, bool bubbles = false) : base(type, bubbles) { this.width = width; this.height = height; }
    }

    public class KeyboardEvent : Event
    {
        public const string KEY_DOWN = "keyDown", KEY_UP = "keyUp";
        public uint keyCode, charCode;
        public bool ctrlKey, shiftKey, altKey;
        public KeyboardEvent(string type, uint charCode = 0, uint keyCode = 0) : base(type, false) { this.charCode = charCode; this.keyCode = keyCode; }
    }

    /// <summary>flash.ui.Keyboard key codes.</summary>
    public static class Keyboard
    {
        public const uint BACKSPACE = 8, TAB = 9, ENTER = 13, SHIFT = 16, CONTROL = 17, ESCAPE = 27, SPACE = 32,
            PAGE_UP = 33, PAGE_DOWN = 34, END = 35, HOME = 36, LEFT = 37, UP = 38, RIGHT = 39, DOWN = 40, DELETE = 46,
            A = 65, D = 68, P = 80, S = 83, W = 87, F = 70, M = 77,
            BACK = 0x01000016, MENU = 0x01000012, SEARCH = 0x0100001F;
    }

    public static class TouchPhase
    {
        public const string HOVER = "hover", BEGAN = "began", MOVED = "moved", STATIONARY = "stationary", ENDED = "ended";
    }

    public class Touch
    {
        public int id { get; internal set; }
        public float globalX { get; internal set; }
        public float globalY { get; internal set; }
        public float previousGlobalX { get; internal set; }
        public float previousGlobalY { get; internal set; }
        public string phase { get; internal set; }
        public DisplayObject target { get; internal set; }
        public int tapCount { get; internal set; }
        public double timestamp { get; internal set; }

        public Touch(int id) { this.id = id; }

        public Point getLocation(DisplayObject space, Point result = null)
        {
            var p = space.globalToLocal(new Point(globalX, globalY));
            if (result != null) { result.setTo(p.x, p.y); return result; }
            return p;
        }

        public Point getPreviousLocation(DisplayObject space, Point result = null)
        {
            var p = space.globalToLocal(new Point(previousGlobalX, previousGlobalY));
            if (result != null) { result.setTo(p.x, p.y); return result; }
            return p;
        }

        public Point getMovement(DisplayObject space, Point result = null)
        {
            var a = getLocation(space);
            var b = getPreviousLocation(space);
            var p = new Point(a.x - b.x, a.y - b.y);
            if (result != null) { result.setTo(p.x, p.y); return result; }
            return p;
        }

        public bool isTouching(DisplayObject t) => t != null && target != null && (t == target || (t is DisplayObjectContainer c && c.contains(target)));
        internal Touch clone() => (Touch)MemberwiseClone();
    }

    public class TouchEvent : Event
    {
        public const string TOUCH = "touch";
        readonly List<Touch> mTouches;
        public bool shiftKey, ctrlKey;
        public double timestamp;

        public TouchEvent(string type, List<Touch> touches, bool shiftKey = false, bool ctrlKey = false, bool bubbles = true)
            : base(type, bubbles, touches) { mTouches = touches ?? new List<Touch>(); this.shiftKey = shiftKey; this.ctrlKey = ctrlKey; }

        public List<Touch> touches => mTouches;

        public List<Touch> getTouches(DisplayObject target, string phase = null, List<Touch> result = null)
        {
            result = result ?? new List<Touch>();
            foreach (var t in mTouches)
            {
                bool correctTarget = t.isTouching(target);
                bool correctPhase = phase == null || phase == t.phase;
                if (correctTarget && correctPhase) result.Add(t);
            }
            return result;
        }

        public Touch getTouch(DisplayObject target, string phase = null, int id = -1)
        {
            foreach (var t in mTouches)
            {
                if (t.isTouching(target) && (phase == null || phase == t.phase) && (id < 0 || t.id == id)) return t;
            }
            return null;
        }

        public bool interactsWith(DisplayObject target)
        {
            foreach (var t in mTouches) if (t.isTouching(target)) return true;
            return false;
        }
    }

    public class EventDispatcher
    {
        Dictionary<string, List<Delegate>> mListeners;

        public void addEventListener(string type, Action<Event> listener) => AddListener(type, listener);
        public void addEventListener(string type, Action listener) => AddListener(type, listener);
        public void removeEventListener(string type, Action<Event> listener) => RemoveListener(type, listener);
        public void removeEventListener(string type, Action listener) => RemoveListener(type, listener);

        void AddListener(string type, Delegate listener)
        {
            if (listener == null) return;
            if (mListeners == null) mListeners = new Dictionary<string, List<Delegate>>();
            if (!mListeners.TryGetValue(type, out var list)) mListeners[type] = list = new List<Delegate>();
            if (!list.Contains(listener)) list.Add(listener);
            OnListenerAdded(type);
        }

        void RemoveListener(string type, Delegate listener)
        {
            if (mListeners != null && mListeners.TryGetValue(type, out var list))
            {
                list.Remove(listener);
                if (list.Count == 0) mListeners.Remove(type);
            }
        }

        public void removeEventListeners(string type = null)
        {
            if (type == null) mListeners = null;
            else mListeners?.Remove(type);
        }

        public bool hasEventListener(string type) => mListeners != null && mListeners.TryGetValue(type, out var l) && l.Count > 0;

        internal virtual void OnListenerAdded(string type) { }

        public virtual void dispatchEvent(Event e)
        {
            bool bubbles = e.bubbles;
            if (!bubbles && !hasEventListener(e.type)) return;
            var previousTarget = e.target;
            e.target = this;
            if (bubbles && this is DisplayObject d) BubbleEvent(e, d);
            else InvokeEvent(e);
            if (previousTarget != null) e.target = previousTarget;
        }

        public void dispatchEventWith(string type, bool bubbles = false, object data = null) => dispatchEvent(new Event(type, bubbles, data));

        internal bool InvokeEvent(Event e)
        {
            if (mListeners == null || !mListeners.TryGetValue(e.type, out var list) || list.Count == 0) return false;
            e.currentTarget = this;
            var copy = list.ToArray(); // listeners may be removed while dispatching
            foreach (var l in copy)
            {
                if (l is Action<Event> a1) a1(e);
                else if (l is Action a0) a0();
                if (e.stopsImmediatePropagation) return true;
            }
            return e.stopsPropagation;
        }

        static void BubbleEvent(Event e, DisplayObject start)
        {
            var chain = new List<EventDispatcher>();
            for (DisplayObject el = start; el != null; el = el.parent) chain.Add(el);
            foreach (var el in chain)
            {
                if (el.InvokeEvent(e)) break;
            }
        }
    }
}
