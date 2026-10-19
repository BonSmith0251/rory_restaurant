using System;
using UnityEngine;

namespace Rory.Popups
{
    /// <summary>
    /// Base class of every window (the original TWindowDisplay states). Prefabs are opened with
    /// <see cref="PopupManager.Show{T}"/> and destroyed when closed; popups that live (inactive) in a scene are
    /// opened with <see cref="PopupManager.ShowExisting{T}"/> and only hidden.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class Popup : MonoBehaviour
    {
        [Tooltip("Dim the screen behind the popup and block input to it (the original 70% black screen)")]
        public bool modal = true;
        [Tooltip("Destroy when closed (popups instantiated from prefabs). Off for popups that live in the scene.")]
        public bool destroyOnClose = true;

        public event Action<Popup> Closed;

        public bool IsOpen { get; private set; }
        public PopupManager Manager { get; internal set; }

        internal void NotifyOpened()
        {
            IsOpen = true;
            OnOpened();
        }

        protected virtual void OnOpened() { }

        public void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            OnClosing();
            Closed?.Invoke(this);
            if (Manager != null) Manager.Remove(this);
            if (destroyOnClose) Destroy(gameObject);
            else gameObject.SetActive(false);
        }

        protected virtual void OnClosing() { }
    }
}
