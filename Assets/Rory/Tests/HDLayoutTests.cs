using System.Collections;
using System.Linq;
using NUnit.Framework;
using Rory.Data;
using Rory.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Rory.Tests
{
    /// <summary>The 16:9 / 1920x1080 setup: viewport, scaling, backgrounds filling the view, logical sprite sizes.</summary>
    public class HDLayoutTests
    {
        static RectTransform Viewport => (RectTransform)UIStage.Current.transform.parent;

        [UnityTest]
        public IEnumerator StageSitsInA16By9Viewport()
        {
            yield return TestUtil.LoadScene(GameScene.MainMenu);
            var stage = UIStage.Current;
            Assert.AreEqual(new Vector2(1024, 600), stage.Rect.rect.size, "design area unchanged");
            Assert.AreEqual(UIStage.ViewWidth, Viewport.rect.width, 0.01f);
            Assert.AreEqual(600, Viewport.rect.height, 0.01f);
            Assert.IsNotNull(Viewport.GetComponent<RectMask2D>(), "the view clips");
            Assert.IsNull(stage.GetComponent<RectMask2D>(), "the stage itself doesn't clip the side margins");
            var scaler = stage.GetComponentInParent<CanvasScaler>();
            Assert.AreEqual(new Vector2(UIStage.ViewWidth, 600), scaler.referenceResolution);
            Assert.AreEqual(1920 / UIStage.ViewWidth, 1080 / 600f, 0.0001f, "1920x1080 = exactly 1.8 px per unit");
        }

        [UnityTest]
        public IEnumerator WideBackgroundFillsTheView()
        {
            yield return TestUtil.LoadScene(GameScene.MainMenu);
            // title_BG is a 16:9 HD image: it spans the whole view, centred on the stage
            var bg = TestUtil.Find<Image>("Background");
            var size = bg.sprite.LogicalSize();
            Assert.AreEqual(600, size.y, 0.5f);
            Assert.GreaterOrEqual(size.x, UIStage.ViewWidth - 1);
            Assert.AreEqual((1024 - size.x) / 2, bg.rectTransform.anchoredPosition.x, 0.5f);
            Assert.Greater(bg.sprite.pixelsPerUnit, 100, "an HD image has more pixels per unit");
            TestUtil.Capture("HD_MainMenu");
        }

        [UnityTest]
        public IEnumerator CustomersShareOneFrame()
        {
            yield return TestUtil.LoadScene(GameScene.MainMenu);
            // the games pin a customer by a corner of its sprite: one size per game puts every customer at the same place
            var sprites = GameDatabase.Instance.sprites;
            for (int n = 1; n <= 13; n++)
                Assert.AreEqual(new Vector2(88, 100), sprites.Get("game", "char_" + n).LogicalSize(), "game/char_" + n);
            for (int n = 1; n <= 12; n++)
                Assert.AreEqual(new Vector2(138, 166), sprites.Get("make_dessert", "char_" + n).LogicalSize(), "make_dessert/char_" + n);
        }

        [UnityTest]
        public IEnumerator OriginalBackgroundGetsEdgeStrips()
        {
            yield return TestUtil.LoadScene(GameScene.LevelSelect);
            var bg = Object.FindObjectsByType<Image>(FindObjectsSortMode.None)
                .First(i => i.sprite != null && i.sprite.name == "level_select_BG");
            Assert.AreEqual(new Vector2(1024, 600), bg.sprite.LogicalSize());
            var strips = bg.GetComponentsInChildren<ViewEdgeFill>();
            Assert.AreEqual(2, strips.Length, "mirrored strips left and right");
            foreach (var s in strips)
                Assert.AreEqual(UIStage.SideMargin, ((RectTransform)s.transform).rect.width, 0.01f);
            TestUtil.Capture("HD_LevelSelect");
        }
    }
}
