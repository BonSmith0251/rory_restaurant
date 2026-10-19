using System.Collections;
using NUnit.Framework;
using Rory.Popups;
using Rory.Screens.MainMenu;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

namespace Rory.Tests
{
    public class MainMenuTests
    {
        [UnityTest]
        public IEnumerator SplashGoesToMainMenu()
        {
            yield return TestUtil.LoadScene(GameScene.Splash);
            TestUtil.Capture("Splash");
            // two splashes of 0.5 + 1.5 + 0.5 s, then the fade
            float timeout = 10;
            while (GameManager.Instance.Flow.Current != GameScene.MainMenu && timeout > 0)
            {
                timeout -= Time.unscaledDeltaTime;
                yield return null;
            }
            Assert.AreEqual(GameScene.MainMenu, GameManager.Instance.Flow.Current);
        }

        [UnityTest]
        public IEnumerator MainMenuShowsAndCreatesPlayer()
        {
            yield return TestUtil.LoadScene(GameScene.MainMenu);
            yield return TestUtil.Seconds(0.3f);
            Assert.IsNotNull(Object.FindFirstObjectByType<MainMenuController>());
            Assert.AreEqual("Guest", TestUtil.Find<TextMeshProUGUI>("PlayerName").text);
            TestUtil.Capture("MainMenu");

            // Story without a player opens the player list and the name entry on top
            TestUtil.Click("StoryButton");
            yield return null;
            var create = Object.FindFirstObjectByType<CreatePlayerPopup>();
            Assert.IsNotNull(create, "Create player window not open");
            Assert.IsNotNull(Object.FindFirstObjectByType<PlayerSelectPopup>());
            create.nameInput.text = "Tester";
            yield return null;
            TestUtil.Capture("MainMenu_CreatePlayer");
            TestUtil.Click("RightButton"); // Done
            yield return null;

            Assert.IsNotNull(GameManager.Player);
            Assert.AreEqual("Tester", GameManager.Player.name);
            var select = Object.FindFirstObjectByType<PlayerSelectPopup>();
            Assert.IsNotNull(select, "Player list should be shown again");
            Assert.AreEqual("Tester", select.slotNames[0].text);
            TestUtil.Capture("MainMenu_PlayerSelect");
            TestUtil.ClickLabel("Select");
            yield return null;
            Assert.AreEqual("Tester", TestUtil.Find<TextMeshProUGUI>("PlayerName").text);
            Assert.IsFalse(PopupManager.Current.HasOpenPopup);
        }

        [UnityTest]
        public IEnumerator MainMenuPopups()
        {
            yield return TestUtil.LoadScene(GameScene.MainMenu);
            yield return null;
            TestUtil.Click("ExtrasButton");
            yield return null;
            TestUtil.Capture("MainMenu_Extras");
            TestUtil.ClickLabel("Cancel");
            yield return null;
            Assert.IsFalse(PopupManager.Current.HasOpenPopup);

            TestUtil.Click("ExitButton");
            yield return null;
            TestUtil.Capture("MainMenu_Quit");
            TestUtil.ClickLabel("No");
            yield return null;

            TestUtil.Click("CreditsButton");
            yield return null;
            TestUtil.Capture("MainMenu_Credits");
            TestUtil.Click("Credits");
            yield return null;
            Assert.IsFalse(PopupManager.Current.HasOpenPopup);

            bool muted = GameManager.Instance.Audio.MuteSound;
            TestUtil.Click("SoundButton");
            Assert.AreNotEqual(muted, GameManager.Instance.Audio.MuteSound);
            Assert.AreEqual(!muted, TestUtil.Find("SoundMuteIcon", true).activeSelf);
            TestUtil.Click("SoundButton");
        }
    }
}
