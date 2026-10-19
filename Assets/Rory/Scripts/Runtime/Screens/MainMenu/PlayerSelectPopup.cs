using System;
using Rory.Localization;
using Rory.Popups;
using Rory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Screens.MainMenu
{
    /// <summary>
    /// "Select Player" window (TWindowState.CHANGE_PLAYER): five save slots with Delete / Create / Select.
    /// Uses the window's left button as Delete, right as Create and the uni button as Select/Close.
    /// </summary>
    public class PlayerSelectPopup : WindowPopup
    {
        static readonly Color Normal = Color.black;
        static readonly Color Chosen = new Color(1, 0, 0);

        [Header("Player select")]
        public Button[] slotButtons = new Button[GameRules.SaveSlots];
        public TextMeshProUGUI[] slotNames = new TextMeshProUGUI[GameRules.SaveSlots];
        public CreatePlayerPopup createPlayer;
        public WindowPopup deleteConfirm;

        int chosen = -1;

        /// <summary>Raised when a player was selected (or the current one deleted).</summary>
        public event Action PlayerChanged;

        void Awake()
        {
            for (int i = 0; i < slotButtons.Length; i++)
            {
                int slot = i;
                slotButtons[i].onClick.AddListener(() => Choose(slot));
            }
            leftButton.onClick.AddListener(() => Manager.ShowExisting(deleteConfirm));
            rightButton.onClick.AddListener(OpenCreate);
            uniButton.onClick.AddListener(() =>
            {
                if (chosen >= 0 && Saves.IsUsed(chosen))
                {
                    Saves.Select(chosen);
                    PlayerChanged?.Invoke();
                }
                Close();
            });
            deleteConfirm.leftButton.onClick.AddListener(deleteConfirm.Close);
            deleteConfirm.rightButton.onClick.AddListener(() =>
            {
                deleteConfirm.Close();
                Saves.Delete(chosen);
                PlayerChanged?.Invoke();
                Refresh();
            });
            createPlayer.Created += slot =>
            {
                PlayerChanged?.Invoke();
                Refresh();
            };
        }

        static Save.SaveSystem Saves => GameManager.Instance.Saves;

        protected override void OnOpened() => Refresh();

        /// <summary>Opens the name entry on top of the list (used when there is no player yet).</summary>
        public void OpenCreate() => Manager.ShowExisting(createPlayer);

        void Refresh()
        {
            int used = 0;
            for (int i = 0; i < GameRules.SaveSlots; i++)
            {
                var p = Saves.GetSlot(i);
                slotNames[i].text = p != null ? p.name : Loc.T("Empty");
                slotNames[i].color = Normal;
                if (p != null) used++;
            }
            rightButton.GetComponent<RoryButton>().Interactable = used < GameRules.SaveSlots;
            chosen = Saves.Current != null ? Saves.Current.slot : -1;
            UpdateChoice();
        }

        void Choose(int slot)
        {
            chosen = slot;
            UpdateChoice();
        }

        void UpdateChoice()
        {
            for (int i = 0; i < slotNames.Length; i++) slotNames[i].color = Normal;
            bool used = chosen >= 0 && Saves.IsUsed(chosen);
            if (used) slotNames[chosen].color = Chosen;
            leftButton.GetComponent<RoryButton>().Interactable = used;
            uniButton.GetComponentInChildren<TextMeshProUGUI>().text = used ? "Select" : "Close";
        }
    }
}
