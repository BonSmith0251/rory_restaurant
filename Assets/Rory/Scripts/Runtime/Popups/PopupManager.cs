using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Popups
{
    /// <summary>
    /// The popup layer of a screen (last child of the UIStage). Keeps a stack of open popups and puts the
    /// dimming backdrop right under the top-most modal one.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class PopupManager : MonoBehaviour
    {
        [SerializeField] Image backdrop;
        [SerializeField] Color backdropColor = new Color(0, 0, 0, 0.7f);

        readonly List<Popup> stack = new List<Popup>();

        public static PopupManager Current { get; private set; }

        public bool HasOpenPopup => stack.Count > 0;
        public IReadOnlyList<Popup> Open => stack;

        void Awake()
        {
            if (backdrop == null) CreateBackdrop();
            backdrop.gameObject.SetActive(false);
        }

        void OnEnable() => Current = this;

        void OnDisable()
        {
            if (Current == this) Current = null;
        }

        void CreateBackdrop()
        {
            var go = new GameObject("Backdrop", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(transform, false);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            backdrop = go.GetComponent<Image>();
            backdrop.color = backdropColor;
        }

        /// <summary>Instantiates <paramref name="prefab"/> on top of the other popups.</summary>
        public T Show<T>(T prefab) where T : Popup
        {
            var popup = Instantiate(prefab, transform, false);
            popup.name = prefab.name;
            Add(popup);
            return popup;
        }

        /// <summary>Shows a popup that already exists in the scene (inactive child of this layer).</summary>
        public T ShowExisting<T>(T popup) where T : Popup
        {
            if (popup.transform.parent != transform) popup.transform.SetParent(transform, false);
            Add(popup);
            return popup;
        }

        void Add(Popup popup)
        {
            if (stack.Contains(popup)) stack.Remove(popup);
            popup.Manager = this;
            popup.gameObject.SetActive(true);
            popup.transform.SetAsLastSibling();
            stack.Add(popup);
            UpdateBackdrop();
            popup.NotifyOpened();
        }

        internal void Remove(Popup popup)
        {
            stack.Remove(popup);
            UpdateBackdrop();
        }

        public void CloseAll()
        {
            for (int i = stack.Count - 1; i >= 0; i--) stack[i].Close();
        }

        public T Find<T>() where T : Popup
        {
            for (int i = stack.Count - 1; i >= 0; i--)
                if (stack[i] is T t) return t;
            return null;
        }

        void UpdateBackdrop()
        {
            Popup top = null;
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                if (stack[i] != null && stack[i].modal)
                {
                    top = stack[i];
                    break;
                }
            }
            backdrop.gameObject.SetActive(top != null);
            if (top == null) return;
            // Put the backdrop directly below the top modal popup.
            int t = top.transform.GetSiblingIndex(), b = backdrop.transform.GetSiblingIndex();
            backdrop.transform.SetSiblingIndex(b < t ? t - 1 : t);
        }
    }
}
