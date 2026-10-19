using TMPro;
using UnityEngine;

namespace Rory.Localization
{
    /// <summary>
    /// Shows a TextMeshPro text in the current language. The English text in the component is the key; when
    /// code later assigns another English text it is translated too (checked once per frame, before rendering),
    /// and everything is re-translated when the language changes. Added to every text by the scene builders
    /// (UIBuild.Text); leave it off for text that must not be translated (player names, numbers).
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    [DisallowMultipleComponent]
    public class Localize : MonoBehaviour
    {
        [Tooltip("English source text (filled from the text when empty)")]
        [SerializeField, TextArea] string source;

        TMP_Text label;
        string applied;

        public string Source => source;

        void Awake()
        {
            label = GetComponent<TMP_Text>();
            if (string.IsNullOrEmpty(source)) source = label.text;
        }

        void OnEnable()
        {
            Loc.Changed += Apply;
            Apply();
        }

        void OnDisable() => Loc.Changed -= Apply;

        void LateUpdate()
        {
            var t = label.text;
            if (ReferenceEquals(t, applied) || t == applied) return;
            source = t; // code assigned a new (English) text
            Apply();
        }

        /// <summary>Sets the English text and shows its translation.</summary>
        public void SetSource(string english)
        {
            source = english;
            applied = null;
            Apply();
        }

        void Apply()
        {
            if (label == null) label = GetComponent<TMP_Text>();
            // Code may have changed the text while this was disabled: that text is the new source.
            if (applied != null && label.text != applied) source = label.text;
            applied = Loc.T(source);
            label.text = applied;
        }
    }
}
