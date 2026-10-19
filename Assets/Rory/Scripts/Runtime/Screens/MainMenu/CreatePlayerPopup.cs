using System;
using System.Linq;
using Rory.Popups;
using Rory.UI;
using TMPro;
using UnityEngine;

namespace Rory.Screens.MainMenu
{
    /// <summary>"Create Player" window (TWindowState.CREATE_PLAYER): name entry, max 12 letters and spaces.</summary>
    public class CreatePlayerPopup : WindowPopup
    {
        [Header("Create player")]
        public TMP_InputField nameInput;
        public WindowPopup sameNameWarning;

        /// <summary>Raised with the slot of the new player.</summary>
        public event Action<int> Created;

        void Awake()
        {
            nameInput.characterLimit = 12;
            nameInput.onValidateInput = (text, index, c) =>
                char.IsLetter(c) && c < 128 || (c == ' ' && text.Length > 0) ? c : '\0';
            nameInput.onValueChanged.AddListener(_ => UpdateDone());
            nameInput.onSubmit.AddListener(_ => Submit());
            leftButton.onClick.AddListener(Close);
            rightButton.onClick.AddListener(Submit);
            sameNameWarning.uniButton.onClick.AddListener(() =>
            {
                sameNameWarning.Close();
                Manager.ShowExisting(this);
            });
        }

        protected override void OnOpened()
        {
            nameInput.text = "";
            UpdateDone();
            nameInput.ActivateInputField();
        }

        void UpdateDone() =>
            rightButton.GetComponent<RoryButton>().Interactable = !string.IsNullOrWhiteSpace(nameInput.text);

        void Submit()
        {
            var name = nameInput.text.Trim();
            if (name.Length == 0) return;
            var saves = GameManager.Instance.Saves;
            Close();
            bool taken = Enumerable.Range(0, GameRules.SaveSlots).Any(i => saves.IsUsed(i) && saves.GetSlot(i).name == name);
            if (taken)
            {
                Manager.ShowExisting(sameNameWarning);
                return;
            }
            for (int i = 0; i < GameRules.SaveSlots; i++)
            {
                if (saves.IsUsed(i)) continue;
                saves.CreatePlayer(i, name);
                Created?.Invoke(i);
                return;
            }
        }
    }
}
