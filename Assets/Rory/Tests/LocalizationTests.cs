using System.Collections;
using NUnit.Framework;
using Rory.Data;
using Rory.Localization;
using Rory.Screens.MainMenu;
using Rory.Tutorials;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

namespace Rory.Tests
{
    public class LocalizationTests
    {
        [TearDown]
        public void BackToEnglish() => Loc.SetLanguage(Loc.English);

        static IEnumerator MenuIn(string code)
        {
            Loc.SetLanguage(code);
            yield return null;
            yield return TestUtil.LoadScene(GameScene.MainMenu);
            yield return TestUtil.Frames(3);
        }

        static string Text(string name) => TestUtil.Find<TextMeshProUGUI>(name).text;

        [UnityTest]
        public IEnumerator EnglishIsTheSource()
        {
            yield return MenuIn(Loc.English);
            Assert.AreEqual(Loc.English, Loc.Current);
            Assert.AreEqual("Story", Text("StoryButton"));
            Assert.AreEqual("Select", Loc.T("Select"), "missing keys fall back to English");
        }

        [UnityTest]
        public IEnumerator MainMenuInEachLanguage([Values("zh-Hans", "ja", "ko")] string code)
        {
            yield return MenuIn(code);
            Assert.AreEqual(code, Loc.Current);
            string story = Text("StoryButton");
            Assert.AreNotEqual("Story", story, "menu entry translated");
            Assert.AreEqual(Loc.T("Story"), story);
            Assert.AreEqual(Loc.T("Guest"), Text("PlayerName"));
            // the language's fallback font is first in TMP's fallback list
            Assert.AreSame(LanguageFonts.FontFor(code), TMP_Settings.fallbackFontAssets[0]);
            TestUtil.Capture("Loc_MainMenu_" + code);

            TestUtil.Click("ChangePlayerButton");
            yield return null;
            Assert.AreEqual(Loc.T("Select Player"), Object.FindFirstObjectByType<PlayerSelectPopup>().title.text);
            Assert.AreEqual(Loc.T("Empty"), Object.FindFirstObjectByType<PlayerSelectPopup>().slotNames[0].text);
            TestUtil.Capture("Loc_PlayerSelect_" + code);
        }

        [UnityTest]
        public IEnumerator LanguagePickerSwitchesAndReloads()
        {
            yield return MenuIn(Loc.English);
            TestUtil.Click("LanguageButton");
            yield return null;
            TestUtil.Capture("Loc_LanguagePicker");
            TestUtil.Click("Language_ja");
            float end = Time.realtimeSinceStartup + 5;
            while ((GameManager.Instance.Flow.IsTransitioning || Loc.Current != "ja") && Time.realtimeSinceStartup < end) yield return null;
            yield return TestUtil.Frames(3);
            Assert.AreEqual("ja", Loc.Current);
            Assert.AreEqual(Loc.T("Story"), Text("StoryButton"));
        }

        [UnityTest]
        public IEnumerator TutorialBubbleIsTranslated()
        {
            yield return MenuIn("ko");
            GameManager.Instance.Saves.CreatePlayer(0, "Tester");
            var stage = Object.FindFirstObjectByType<UI.UIStage>();
            var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<TutorialManager>("Assets/Rory/Prefabs/Tutorial/Tutorial.prefab");
            Assume.That(prefab, Is.Not.Null, "tutorial prefab");
            var tut = Object.Instantiate(prefab, stage.transform, false);
            yield return null;
            tut.Show(TutorialIds.AFTER_LEVEL_1, 0); // dialogue 2: "Great job, (player)! ..."
            yield return null;
            var line = GameDatabase.Instance.dialogues.dialogues[2].lines[0];
            string shown = tut.DisplayText(2, 0, line);
            StringAssert.Contains("Tester", shown, "(player) substituted after translation");
            Assert.AreNotEqual(line.text.Replace("(player)", "Tester"), shown, "dialogue translated");
            TestUtil.Capture("Loc_Tutorial_ko");
            Object.Destroy(tut.gameObject);
        }
    }
}
