using System;
using Rory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Popups.Game
{
    /// <summary>
    /// "Paused" window (TWindowState.PAUSE_GAME). Big window with SOUND / MUSIC / FULL SCREEN toggles (green "ON",
    /// red "OFF"), the day's info text and two buttons: left = Kitchen (story, asks for confirmation) or Main Menu
    /// (extras), right = Resume. Opened with <see cref="GameWindows.ShowPause"/>.
    /// </summary>
    public class PausePopup : WindowPopup
    {
        [Header("Toggles")]
        public Button soundButton;
        public Button musicButton;
        public Button fullScreenButton;
        public TextMeshProUGUI soundLabel;
        public TextMeshProUGUI musicLabel;
        public TextMeshProUGUI fullScreenLabel;

        PauseOptions options;

        internal void Setup(PauseOptions o)
        {
            options = o;
            var session = GameManager.Instance.Session;
            bool extras = session.extrasMode;
            SetTitle("Paused");
            // The info text (normally "Goal : serve N customer(s)\nExpert : earn $M") is hidden in extras mode.
            SetInfo(extras ? null : o.info ?? GameWindows.LastDayInfo);
            GameWindows.Bind(leftButton, extras ? "Main Menu" : "Kitchen", OnLeft);
            GameWindows.Bind(rightButton, "Resume", OnResume);
            GameWindows.Bind(uniButton, null, null);
            soundButton.onClick.AddListener(ToggleSound);
            musicButton.onClick.AddListener(ToggleMusic);
            fullScreenButton.onClick.AddListener(ToggleFullScreen);
            Refresh();
        }

        static Audio.AudioManager Audio => GameManager.Instance.Audio;
        static Save.SaveSystem Saves => GameManager.Instance.Saves;

        void OnLeft()
        {
            // extras: MAINMENU_BUTTON (straight to the menu); story: KITCHEN_WITH_CONFIRMATION
            if (GameManager.Instance.Session.extrasMode) GameManager.Instance.Flow.Go(GameScene.MainMenu);
            else GameWindows.ShowConfirmKitchen(options.resetGoalOnExit);
        }

        void OnResume()
        {
            Close();
            options.onResume?.Invoke();
        }

        void ToggleSound()
        {
            Audio.MuteSound = !Audio.MuteSound;
            Refresh();
            options.onMuteChanged?.Invoke();
        }

        void ToggleMusic()
        {
            Audio.MuteMusic = !Audio.MuteMusic;
            Refresh();
            options.onMuteChanged?.Invoke();
        }

        void ToggleFullScreen()
        {
            bool full = !Saves.Settings.fullScreen;
            Saves.Settings.fullScreen = full;
            DisplayMode.SetFullScreen(full);
            Saves.SaveSettings();
            Refresh();
        }

        /// <summary>Updates the toggle labels and colours from the current settings.</summary>
        public void Refresh()
        {
            SetToggle(soundButton, soundLabel, !Audio.MuteSound);
            SetToggle(musicButton, musicLabel, !Audio.MuteMusic);
            SetToggle(fullScreenButton, fullScreenLabel, Saves.Settings.fullScreen);
        }

        static void SetToggle(Button b, TextMeshProUGUI label, bool on)
        {
            var lib = GameWindowsLibrary.Instance;
            label.text = on ? "ON" : "OFF";
            ((Image)b.targetGraphic).sprite = on ? lib.buttonOn : lib.buttonOff;
        }
    }

    /// <summary>Parameters of <see cref="GameWindows.ShowPause"/>.</summary>
    public struct PauseOptions
    {
        /// <summary>Resume clicked (the window is already closed). The original set the screen's paused = false
        /// (the dessert screen also called Resume()).</summary>
        public Action onResume;
        /// <summary>Sound or music toggled: refresh the screen's own mute icons (the original set mute_icon /
        /// mute_icon_music.visible of the match-3, HO and dessert screens).</summary>
        public Action onMuteChanged;
        /// <summary>Match-3 only (the original tested currentSceneID == GAMESCREEN_ID): when leaving through
        /// Kitchen / Main Menu "Yes" during the first play of a day, goalAchieved of that day is reset.</summary>
        public bool resetGoalOnExit;
        /// <summary>Text under the toggles in story mode. Null = <see cref="GameWindows.LastDayInfo"/> (the original
        /// showed whatever the shared info field last held: the last day window's text).</summary>
        public string info;
    }
}
