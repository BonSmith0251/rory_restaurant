using Rory.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Screens.MainMenu
{
    /// <summary>Title screen (MainMenuScreen): story / extras / credits / exit, player select, sound and display toggles.</summary>
    public class MainMenuController : ScreenController
    {
        const string CheatName = "OrIgAmInglOl";

        [Header("Menu")]
        [SerializeField] Button storyButton;
        [SerializeField] Button extrasButton;
        [SerializeField] Button creditsButton;
        [SerializeField] Button exitButton;
        [SerializeField] Button changePlayerButton;
        [SerializeField] TextMeshProUGUI playerNameText;

        [Header("Toggles")]
        [SerializeField] Button soundButton;
        [SerializeField] GameObject soundMuteIcon;
        [SerializeField] Button musicButton;
        [SerializeField] GameObject musicMuteIcon;
        [SerializeField] Button fullScreenButton;
        [SerializeField] TextMeshProUGUI fullScreenLabel;

        [Header("Popups (inactive children of the Popups layer)")]
        [SerializeField] PlayerSelectPopup playerSelect;
        [SerializeField] WindowPopup extrasMenu;
        [SerializeField] WindowPopup quitConfirm;
        [SerializeField] Popup credits;

        [Header("Cheat (tap Deluxe 4x and Rory 3x as player OrIgAmInglOl)")]
        [SerializeField] Button roryLogo;
        [SerializeField] Button deluxeLogo;

        int roryClicks, deluxeClicks;

        void Start()
        {
            Session.extrasMode = false;
            Sound.PlayMusic("feeling-positive");

            storyButton.onClick.AddListener(OnStory);
            extrasButton.onClick.AddListener(() => PopupManager.Current.ShowExisting(extrasMenu));
            creditsButton.onClick.AddListener(() => PopupManager.Current.ShowExisting(credits));
            exitButton.onClick.AddListener(() => PopupManager.Current.ShowExisting(quitConfirm));
            changePlayerButton.onClick.AddListener(() => PopupManager.Current.ShowExisting(playerSelect));

            soundButton.onClick.AddListener(() => { Sound.MuteSound = !Sound.MuteSound; RefreshToggles(); });
            musicButton.onClick.AddListener(() => { Sound.MuteMusic = !Sound.MuteMusic; RefreshToggles(); });
            fullScreenButton.onClick.AddListener(ToggleFullScreen);

            // Extras: Hidden Object (left), Dessert Making (right), Cancel (uni)
            extrasMenu.leftButton.onClick.AddListener(() => StartExtra(GameScene.HiddenObject));
            extrasMenu.rightButton.onClick.AddListener(() => StartExtra(GameScene.Dessert));
            extrasMenu.uniButton.onClick.AddListener(extrasMenu.Close);

            // Quit: No (left), Yes (right)
            quitConfirm.leftButton.onClick.AddListener(quitConfirm.Close);
            quitConfirm.rightButton.onClick.AddListener(Quit);

            credits.GetComponent<Button>().onClick.AddListener(credits.Close);

            playerSelect.PlayerChanged += RefreshPlayer;
            roryLogo.onClick.AddListener(() => Cheat(ref roryClicks, 3));
            deluxeLogo.onClick.AddListener(() => Cheat(ref deluxeClicks, 4));

            RefreshPlayer();
            RefreshToggles();
        }

        void OnStory()
        {
            if (Player != null)
            {
                GoTo(Player.openingViewed ? GameScene.Kitchen : GameScene.Opening);
                return;
            }
            var popups = PopupManager.Current;
            popups.ShowExisting(playerSelect);
            bool anyUsed = false;
            for (int i = 0; i < GameRules.SaveSlots; i++) anyUsed |= Game.Saves.IsUsed(i);
            if (!anyUsed) playerSelect.OpenCreate();
        }

        void StartExtra(GameScene scene)
        {
            Session.extrasMode = true;
            GoTo(scene);
        }

        void RefreshPlayer() => playerNameText.text = Player != null ? Player.name : "Guest";

        void RefreshToggles()
        {
            soundMuteIcon.SetActive(Sound.MuteSound);
            musicMuteIcon.SetActive(Sound.MuteMusic);
            fullScreenLabel.text = Screen.fullScreen ? "Windowed" : "Full Screen";
        }

        void ToggleFullScreen()
        {
            bool full = !Screen.fullScreen;
            Screen.fullScreen = full;
            Game.Saves.Settings.fullScreen = full;
            Game.Saves.SaveSettings();
            fullScreenLabel.text = full ? "Windowed" : "Full Screen";
        }

        void Cheat(ref int counter, int needed)
        {
            if (counter >= needed) return;
            counter++;
            if (deluxeClicks == 4 && roryClicks == 3 && Player != null && Player.name == CheatName)
            {
                Session.cheatActive = true;
                Sound.PlaySfx("expert_reached");
            }
        }

        static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
