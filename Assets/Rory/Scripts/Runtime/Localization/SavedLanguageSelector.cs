using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Rory.Localization
{
    /// <summary>
    /// Startup language = the player's last choice, saved by <see cref="Loc.SetLanguage"/> (PlayerPrefs
    /// <see cref="Loc.PlayerPrefsKey"/>). Replaces the package's PlayerPrefLocaleSelector, which records the language
    /// from an asynchronous step that can finish after a newer choice and overwrite it.
    /// </summary>
    [Serializable]
    public class SavedLanguageSelector : IStartupLocaleSelector
    {
        public Locale GetStartupLocale(ILocalesProvider availableLocales)
        {
            var code = PlayerPrefs.GetString(Loc.PlayerPrefsKey, "");
            return string.IsNullOrEmpty(code) ? null : availableLocales.GetLocale(code);
        }
    }
}
