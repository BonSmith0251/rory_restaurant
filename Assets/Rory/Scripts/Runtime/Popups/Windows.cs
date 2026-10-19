using System;
using Rory.Data;

namespace Rory.Popups
{
    /// <summary>Shortcuts for the common windows, shown in the current screen's <see cref="PopupManager"/>.</summary>
    public static class Windows
    {
        static WindowPopup Open(bool small)
        {
            var db = GameDatabase.Instance;
            var prefab = small ? db.smallWindow : db.bigWindow;
            return PopupManager.Current.Show(prefab);
        }

        public static WindowPopup Big() => Open(false);
        public static WindowPopup Small() => Open(true);

        /// <summary>Small yes/no window (e.g. "Are you sure you want to quit game?").</summary>
        public static WindowPopup Confirm(string text, string yesLabel, Action onYes, string noLabel = "No", Action onNo = null)
        {
            var w = Small();
            w.SetTitle(null).SetInfo(text);
            w.SetButtons(new WindowPopup.ButtonSpec(noLabel, onNo), new WindowPopup.ButtonSpec(yesLabel, onYes));
            return w;
        }

        /// <summary>Big window with a title, a text and one button.</summary>
        public static WindowPopup Message(string title, string text, string button = "OK", Action onClick = null)
        {
            var w = Big();
            w.SetTitle(title).SetInfo(text).SetPicture(null);
            w.SetButton(new WindowPopup.ButtonSpec(button, onClick));
            return w;
        }
    }
}
