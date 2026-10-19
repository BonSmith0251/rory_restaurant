using System;
using Rory.Localization;
using Rory.Popups;
using Rory.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Screens.MainMenu
{
    /// <summary>Language picker (not in the original): one button per language, each labelled in its own language.</summary>
    public class LanguagePopup : WindowPopup
    {
        [Header("Languages")]
        public Button[] languageButtons = new Button[0];
        public string[] codes = new string[0];

        /// <summary>Raised with the chosen locale code when it differs from the current one.</summary>
        public event Action<string> LanguageChosen;

        void Awake()
        {
            for (int i = 0; i < languageButtons.Length; i++)
            {
                string code = codes[i];
                languageButtons[i].onClick.AddListener(() =>
                {
                    Close();
                    if (code != Loc.Current) LanguageChosen?.Invoke(code);
                });
            }
            uniButton.onClick.AddListener(Close);
        }

        protected override void OnOpened()
        {
            // Mark the current language.
            for (int i = 0; i < languageButtons.Length; i++)
            {
                var scale = Vector3.one * (codes[i] == Loc.Current ? 1.05f : 0.9f);
                if (languageButtons[i].TryGetComponent(out RoryButton rb)) rb.SetBaseScale(scale);
                else languageButtons[i].transform.localScale = scale;
            }
        }
    }
}
