using System.Collections;
using NUnit.Framework;
using Rory.Popups;
using Rory.Save;
using Rory.Screens.Kitchen;
using Rory.Tutorials;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using AchievementList = global::Rory.Achievements.Achievements;
using Shop = Rory.Screens.Kitchen.KitchenController.KitchenShop;
using BuyLabel = Rory.Screens.Kitchen.KitchenController.BuyLabel;

namespace Rory.Tests
{
    /// <summary>
    /// Kitchen room and shops. Expected texts, prices and positions come from the original (legacy port in the
    /// harness: day N sets passedDays = lastDayKitchen = N, new player with 0 coins).
    /// </summary>
    public class KitchenTests
    {
        static GameSession Session => GameManager.Instance.Session;
        static PlayerProfile Player => GameManager.Player;
        static string saveFolder;
        KitchenController kitchen;

        /// <summary>Fresh saves with player "Tester" on day <paramref name="day"/> (like the harness "day N"), then the kitchen.</summary>
        IEnumerator LoadKitchen(int day, int money = 0, System.Action<PlayerProfile> setup = null)
        {
            while (GameManager.Instance.Flow.IsTransitioning) yield return null;
            saveFolder = TestUtil.TempSaveFolder();
            GameManager.Instance.UseSaveFolder(saveFolder);
            var p = GameManager.Instance.Saves.CreatePlayer(0, "Tester");
            p.passedDays = day;
            p.lastDayKitchen = day;
            p.money = money;
            setup?.Invoke(p);
            Session.currentPlayedLevel = day;
            yield return TestUtil.LoadScene(GameScene.Kitchen, false);
            kitchen = Object.FindFirstObjectByType<KitchenController>();
            Assert.IsNotNull(kitchen, "No KitchenController");
        }

        static bool Active(string name) => TestUtil.Find(name) != null;
        static string Text(string name) => TestUtil.Find<TextMeshProUGUI>(name, true).text;
        static string BuyText => TestUtil.Find("BuyButton", true).GetComponentInChildren<TextMeshProUGUI>(true).text;
        static Vector2 StagePos(string name) => UI.StageLayout.GetStagePosition(TestUtil.Find<RectTransform>(name, true));
        static PlayerProfile Saved()
        {
            var s = new SaveSystem(saveFolder);
            s.LoadAll();
            return s.GetSlot(0);
        }

        IEnumerator Settle() => TestUtil.Seconds(0.6f);

        // ------------------------------------------------------------------ room

        [UnityTest]
        public IEnumerator RoomOnDay5ShowsRecipeBookOnly()
        {
            yield return LoadKitchen(5);
            Assert.IsTrue(Active("PlayBubble") && Active("PlayQuad"));
            Assert.IsTrue(Active("RecipeBubble") && Active("RecipeQuad"));
            Assert.IsTrue(Active("AchievementBubble") && Active("AchievementQuad"));
            Assert.IsFalse(Active("ShopBubble") || Active("ShopQuad"), "Ingredient shop opens on day 8");
            Assert.IsFalse(Active("HardwareBubble") || Active("HardwareQuad"), "Hardware store opens on day 34");
            Assert.IsFalse(Active("ShopBackground"));
            Assert.AreEqual(new Vector2(60 + 207 / 2f, 535 + 29), StagePos("BackButton"));
            Assert.AreEqual(GameManager.Instance.Audio.CurrentMusic, "feeling-positive");
            // no free dishes yet
            Assert.AreEqual(0, Player.chineseDishLevel[0]);
            Assert.AreEqual(0, Player.hardwareLevel[GameRules.Juicer]);
            Assert.AreEqual(5, Saved().lastDayKitchen);
            yield return TestUtil.Seconds(0.3f);
            TestUtil.Capture("Kitchen_Room_Day5");

            // the bubbles bounce: up 25 px in 0.2 s after 0.5 s, then back with a bounce in 0.4 s
            float y0 = 388;
            float minY = y0;
            for (float t = 0; t < 1.2f; t += Time.deltaTime)
            {
                minY = Mathf.Min(minY, StagePos("PlayBubble").y);
                yield return null;
            }
            Assert.AreEqual(y0 - 25, minY, 1.5f, "Play bubble rises 25 px");
        }

        [UnityTest]
        public IEnumerator RoomOnDay40UnlocksRegionsAndHardware()
        {
            yield return LoadKitchen(40);
            Assert.IsTrue(Active("ShopBubble") && Active("ShopQuad"));
            Assert.IsTrue(Active("HardwareBubble") && Active("HardwareQuad"));
            var p = Player;
            Assert.AreEqual(new[] { 1, 1, 0, 0, 0, 0 }, p.chineseDishLevel.ToArray());
            Assert.AreEqual(new[] { 1, 1, 0, 0, 0, 0 }, p.mideastDishLevel.ToArray());
            Assert.AreEqual(2, p.achievementCounter[AchievementList.AsianRecipe]);
            Assert.AreEqual(2, p.achievementCounter[AchievementList.MiddleEastRecipe]);
            Assert.AreEqual(new[] { 1, 1, 0 }, p.hardwareLevel.ToArray());
            var saved = Saved();
            Assert.AreEqual(1, saved.chineseDishLevel[1]);
            Assert.AreEqual(1, saved.hardwareLevel[GameRules.Fryer]);
            yield return TestUtil.Seconds(0.3f);
            TestUtil.Capture("Kitchen_Room_Day40");
        }

        [UnityTest]
        public IEnumerator HoverLightsTheGlow()
        {
            yield return LoadKitchen(40);
            var quad = TestUtil.Find("ShopQuad");
            var glow = TestUtil.Find<Image>("ShopGlow");
            Assert.AreEqual(0, glow.color.a);
            UnityEngine.EventSystems.ExecuteEvents.Execute(quad, new UnityEngine.EventSystems.PointerEventData(
                UnityEngine.EventSystems.EventSystem.current), UnityEngine.EventSystems.ExecuteEvents.pointerEnterHandler);
            Assert.AreEqual(1, glow.color.a);
            TestUtil.Capture("Kitchen_Room_ShopGlow");
            UnityEngine.EventSystems.ExecuteEvents.Execute(quad, new UnityEngine.EventSystems.PointerEventData(
                UnityEngine.EventSystems.EventSystem.current), UnityEngine.EventSystems.ExecuteEvents.pointerExitHandler);
            Assert.AreEqual(0, glow.color.a);
        }

        // ------------------------------------------------------------------ navigation

        [UnityTest]
        public IEnumerator PlayGoesToLevelSelect()
        {
            yield return LoadKitchen(5);
            TestUtil.Click("PlayQuad");
            yield return TestUtil.ExpectSceneRequest(GameScene.LevelSelect);
        }

        [UnityTest]
        public IEnumerator PlayBubbleGoesToLevelSelect()
        {
            yield return LoadKitchen(5);
            TestUtil.Click("PlayBubble");
            yield return TestUtil.ExpectSceneRequest(GameScene.LevelSelect);
        }

        [UnityTest]
        public IEnumerator AchievementQuadGoesToAchievementsAndBack()
        {
            yield return LoadKitchen(5);
            Session.achievementsBackToMainMenu = true;
            TestUtil.Click("AchievementQuad");
            Assert.IsFalse(Session.achievementsBackToMainMenu, "Back from the achievements returns to the kitchen");
            yield return TestUtil.ExpectSceneRequest(GameScene.Achievements);
        }

        [UnityTest]
        public IEnumerator BackInRoomGoesToMainMenu()
        {
            yield return LoadKitchen(5);
            TestUtil.Click("BackButton");
            yield return TestUtil.ExpectSceneRequest(GameScene.MainMenu);
        }

        // ------------------------------------------------------------------ recipe shop

        [UnityTest]
        public IEnumerator RecipeShopWithoutMoney()
        {
            yield return LoadKitchen(5);
            TestUtil.Click("RecipeQuad");
            Assert.AreEqual(Shop.Recipe, kitchen.CurrentShop);
            Assert.IsTrue(Active("ShopBackground"));
            Assert.AreEqual("EMMA'S RECIPES", Text("ShopName"));
            Assert.AreEqual("Veggie Croissant", Text("ItemName"));
            Assert.AreEqual("Current price : $15 ($3 increase)", Text("UpgradeInfo"));
            Assert.AreEqual(" Cost : 2", Text("UpgradeCost"));
            Assert.AreEqual("Can't Afford", BuyText);
            Assert.AreEqual(BuyLabel.CantAfford, kitchen.BuyState);
            Assert.IsFalse(kitchen.IsBuyTouchable);
            Assert.AreEqual("0", Text("MoneyPreview"));
            Assert.IsFalse(Active("WesternButton") || Active("ChineseButton") || Active("MidEastButton"), "Regions open on day 7");
            Assert.IsFalse(Active("LeftButton"));
            Assert.IsTrue(Active("RightButton"));
            Assert.AreEqual(new Vector2(800 + 207 / 2f, 535 + 29), StagePos("BackButton"));
            // ingredients: bread x6, lettuce x7
            Assert.IsTrue(Active("IngredientPreview0") && Active("IngredientPreview1") && !Active("IngredientPreview2"));
            Assert.AreEqual("croissant_small", TestUtil.Find<Image>("IngredientPreview0").sprite.name);
            Assert.AreEqual("x6", Text("IngredientCount0"));
            Assert.AreEqual("x7", Text("IngredientCount1"));
            // three empty stars at 452, 512, 572
            Assert.IsFalse(Active("Star0"));
            for (int i = 1; i <= 3; i++)
            {
                Assert.AreEqual(new Vector2(392 + i * 60, 410), StagePos("Star" + i));
                Assert.AreEqual("star_empty", TestUtil.Find<Image>("Star" + i).sprite.name);
            }
            Assert.IsFalse(Active("Star4"));
            TestUtil.Capture("Kitchen_Recipe_Western0");

            TestUtil.Click("RightButton");
            Assert.IsTrue(kitchen.IsItemTweening);
            Assert.AreEqual(1, kitchen.CurrentFocus);
            Assert.AreEqual("Deluxe Chicken Croissant", Text("ItemName"));
            Assert.AreEqual("Dish price : $21", Text("UpgradeInfo"));
            Assert.AreEqual(" Cost : 5", Text("UpgradeCost"));
            Assert.IsFalse(Active("Star1"), "No stars for a recipe not bought yet");
            // a second click while the plates move is ignored
            TestUtil.Click("RightButton");
            Assert.AreEqual(1, kitchen.CurrentFocus);
            yield return Settle();
            Assert.IsFalse(kitchen.IsItemTweening);
            Assert.AreEqual(512 - 550, kitchen.Slots[0].X, 0.01f);
            Assert.AreEqual(512, kitchen.Slots[1].X, 0.01f);
            Assert.AreEqual(1, kitchen.Slots[1].Scale, 0.001f);
            Assert.AreEqual(0.7f, kitchen.Slots[0].Alpha, 0.001f);
            TestUtil.Capture("Kitchen_Recipe_Western1");

            // to the last recipe: needs 30 expert badges
            for (int i = 0; i < 4; i++)
            {
                TestUtil.Click("RightButton");
                yield return Settle();
            }
            Assert.AreEqual(5, kitchen.CurrentFocus);
            Assert.AreEqual("Premium Salmon Steak", Text("ItemName"));
            Assert.AreEqual("Info", BuyText);
            Assert.IsTrue(kitchen.IsBuyTouchable);
            Assert.IsFalse(Active("RightButton"));
            Assert.AreEqual(" Cost : 30", Text("UpgradeCost"));
            // plates loop: slot 0 came back on the right with item 4, slot 1 with item 5
            Assert.AreEqual(4, kitchen.Slots[0].ItemIndex);
            Assert.AreEqual(5, kitchen.Slots[1].ItemIndex);
            Assert.AreEqual(512, kitchen.Slots[1].X, 0.01f);
            TestUtil.Capture("Kitchen_Recipe_Western5");

            int money = Player.money;
            TestUtil.Click("BuyButton");
            yield return null;
            var info = PopupManager.Current.Open[PopupManager.Current.Open.Count - 1] as WindowPopup;
            Assert.IsNotNull(info, "Locked! window");
            Assert.AreEqual("Locked!", info.title.text);
            Assert.AreEqual("To unlock this recipe, you need to collect 30 expert badges.\nCurrent Expert Badges: 0", info.info.text);
            Assert.AreEqual(money, Player.money);
            Assert.AreEqual(0, Player.westernDishLevel[5]);
            TestUtil.Capture("Kitchen_Recipe_Locked");
            TestUtil.ClickLabel("OK");
            yield return null;
            Assert.IsFalse(PopupManager.Current.HasOpenPopup);

            // back closes the shop; the back button stays at x = 20 (original quirk)
            TestUtil.Click("BackButton");
            Assert.AreEqual(Shop.Recipe, kitchen.CurrentShop);
            Assert.IsFalse(Active("ShopBackground"));
            Assert.IsFalse(Active("Star1") || Active("ItemName") || Active("BuyButton"));
            Assert.AreEqual(new Vector2(20 + 207 / 2f, 535 + 29), StagePos("BackButton"));
            yield return TestUtil.Seconds(0.3f);
            TestUtil.Capture("Kitchen_Room_AfterBack");
        }

        [UnityTest]
        public IEnumerator BuyAndUpgradeRecipes()
        {
            yield return LoadKitchen(40, 1000);
            TestUtil.Click("RecipeQuad");
            Assert.IsTrue(Active("WesternButton") && Active("ChineseButton") && Active("MidEastButton"));
            Assert.AreEqual("small_btn_blue_dark", TestUtil.Find<Image>("WesternButton").sprite.name);
            Assert.AreEqual("1,000", Text("MoneyPreview"));

            // upgrade the veggie croissant: level 1 -> 2 costs 2
            Assert.AreEqual("Upgrade", BuyText);
            Assert.AreEqual("small_btn_blue", TestUtil.Find<Image>("BuyButton").sprite.name);
            TestUtil.Click("BuyButton");
            Assert.AreEqual(2, Player.westernDishLevel[0]);
            Assert.AreEqual(998, Player.money);
            Assert.AreEqual("998", Text("MoneyPreview"));
            Assert.AreEqual("-2", Text("MoneyPreviewMinus"));
            Assert.IsTrue(Active("MoneyPreviewMinus"));
            Assert.AreEqual("Current price : $18 ($3 increase)", Text("UpgradeInfo"));
            Assert.AreEqual(" Cost : 4", Text("UpgradeCost"));
            Assert.AreEqual("star", TestUtil.Find<Image>("Star1").sprite.name);
            Assert.AreEqual("star_empty", TestUtil.Find<Image>("Star2").sprite.name);
            Assert.AreEqual(998, Saved().money);
            yield return TestUtil.Seconds(0.25f);
            Assert.Greater(StagePos("MoneyPreviewMinus").y, 70, "the -cost text slides down");
            TestUtil.Capture("Kitchen_Recipe_Bought");
            yield return TestUtil.Seconds(1);
            Assert.IsFalse(Active("MoneyPreviewMinus"));
            Assert.AreEqual(70, StagePos("MoneyPreviewMinus").y, 0.01f);

            // max it: 4 then 10, then "Maxed"
            TestUtil.Click("BuyButton");
            TestUtil.Click("BuyButton");
            Assert.AreEqual(4, Player.westernDishLevel[0]);
            Assert.AreEqual(998 - 4 - 10, Player.money);
            Assert.AreEqual("Maxed", BuyText);
            Assert.IsFalse(kitchen.IsBuyTouchable);
            Assert.AreEqual("Current price : $24", Text("UpgradeInfo"));
            Assert.IsFalse(Active("UpgradeCost") || Active("ShopCoin"));

            // buy the deluxe chicken croissant (5): western recipe book 1 -> 2
            TestUtil.Click("RightButton");
            yield return Settle();
            Assert.AreEqual("Buy", BuyText);
            Assert.AreEqual("small_btn_green", TestUtil.Find<Image>("BuyButton").sprite.name);
            TestUtil.Click("BuyButton");
            Assert.AreEqual(1, Player.westernDishLevel[1]);
            Assert.AreEqual(984 - 5, Player.money);
            Assert.AreEqual(2, Player.achievementCounter[AchievementList.WesternRecipe]);
            Assert.AreEqual("Current price : $21 ($5 increase)", Text("UpgradeInfo"));

            // Asian region: third recipe bought -> 3 of 6 (progress banner)
            TestUtil.Click("ChineseButton");
            Assert.AreEqual(KitchenController.RecipeRegion.Chinese, kitchen.CurrentRegion);
            Assert.AreEqual(0, kitchen.CurrentFocus);
            Assert.AreEqual("small_btn_blue_dark", TestUtil.Find<Image>("ChineseButton").sprite.name);
            Assert.AreEqual("small_btn_blue", TestUtil.Find<Image>("WesternButton").sprite.name);
            Assert.AreEqual("Asian Fried Rice", Text("ItemName"));
            Assert.AreEqual("x3", Text("IngredientCount0"));
            Assert.AreEqual("rice_small", TestUtil.Find<Image>("IngredientPreview0").sprite.name);
            TestUtil.Capture("Kitchen_Recipe_Asian0");
            TestUtil.Click("RightButton");
            yield return Settle();
            TestUtil.Click("RightButton");
            yield return Settle();
            Assert.AreEqual("Kalasan Fried Chicken", Text("ItemName"));
            Assert.AreEqual("Dish price : $23", Text("UpgradeInfo"));
            int before = Player.money;
            TestUtil.Click("BuyButton");
            Assert.AreEqual(before - 10, Player.money);
            Assert.AreEqual(3, Player.achievementCounter[AchievementList.AsianRecipe]);
            Assert.IsFalse(Player.unlocked[AchievementList.AsianRecipe]);

            // Middle East, with an ingredient bonus: pita bread level 3 adds $2 to the doner kebab
            Player.ingredientLevel[10] = 3; // type 11 - 1 (the recipe price reads ingredientLevel[type - 1])
            TestUtil.Click("MidEastButton");
            Assert.AreEqual("Doner Kebab", Text("ItemName"));
            Assert.AreEqual("Current price : $16 ($3 increase)", Text("UpgradeInfo"));
            for (int i = 0; i < 3; i++)
            {
                TestUtil.Click("RightButton");
                yield return Settle();
            }
            Assert.AreEqual("Tuscan Humus", Text("ItemName"));
            Assert.AreEqual("Dish price : $38 ($2 ingredient bonus)", Text("UpgradeInfo"));
            TestUtil.Capture("Kitchen_Recipe_MidEast2");
        }

        [UnityTest]
        public IEnumerator RegionPagesWithoutMoney()
        {
            yield return LoadKitchen(40);
            TestUtil.Click("RecipeQuad");
            TestUtil.Click("ChineseButton");
            Assert.AreEqual(" Cost : 2", Text("UpgradeCost"));
            Assert.AreEqual("Current price : $13 ($3 increase)", Text("UpgradeInfo"));
            TestUtil.Capture("Kitchen_Recipe_Asian0_NoMoney");
            TestUtil.Click("MidEastButton");
            Assert.AreEqual("Doner Kebab", Text("ItemName"));
            Assert.AreEqual("Current price : $14 ($3 increase)", Text("UpgradeInfo"));
            TestUtil.Capture("Kitchen_Recipe_MidEast0");
        }

        [UnityTest]
        public IEnumerator UnlockingTheLastRecipeCompletesTheBook()
        {
            yield return LoadKitchen(40, 500, p =>
            {
                for (int i = 0; i < 5; i++) p.westernDishLevel[i] = 1;
                p.achievementCounter[AchievementList.WesternRecipe] = 5;
                p.achievementCounter[AchievementList.InternationalChef] = 30;
            });
            TestUtil.Click("RecipeQuad");
            for (int i = 0; i < 5; i++)
            {
                TestUtil.Click("RightButton");
                yield return Settle();
            }
            Assert.AreEqual("Buy", BuyText, "30 expert badges unlock the last recipe");
            TestUtil.Click("BuyButton");
            Assert.AreEqual(500 - 30, Player.money);
            Assert.IsTrue(Player.unlocked[AchievementList.WesternRecipe]);
            Assert.IsTrue(Saved().unlocked[AchievementList.WesternRecipe]);
            yield return TestUtil.Seconds(1.2f);
            Assert.AreEqual("Western Recipes Book", TestUtil.Find<TextMeshProUGUI>("Title", true).text);
            TestUtil.Capture("Kitchen_Recipe_BookUnlocked");
        }

        [UnityTest]
        public IEnumerator SwipeBrowsesThePlates()
        {
            yield return LoadKitchen(5);
            TestUtil.Click("RecipeQuad");
            var bg = TestUtil.Find("ShopBackground");
            // less than 70 px: nothing
            TestUtil.Press(bg, new Vector2(600, 300), new Vector2(540, 300));
            Assert.AreEqual(0, kitchen.CurrentFocus);
            // to the left: next item
            TestUtil.Press(bg, new Vector2(600, 300), new Vector2(560, 300), new Vector2(520, 300), new Vector2(400, 300));
            Assert.AreEqual(1, kitchen.CurrentFocus, "one step per touch");
            yield return Settle();
            // to the right: previous item
            TestUtil.Press(bg, new Vector2(400, 300), new Vector2(500, 300));
            Assert.AreEqual(0, kitchen.CurrentFocus);
            yield return Settle();
            // no item before the first one
            TestUtil.Press(bg, new Vector2(400, 300), new Vector2(500, 300));
            Assert.AreEqual(0, kitchen.CurrentFocus);
            Assert.IsFalse(kitchen.IsItemTweening);
        }

        // ------------------------------------------------------------------ ingredient shop

        [UnityTest]
        public IEnumerator IngredientShopWithoutMoney()
        {
            yield return LoadKitchen(40);
            TestUtil.Click("ShopQuad");
            Assert.AreEqual(Shop.Ingredient, kitchen.CurrentShop);
            Assert.AreEqual("MILLY'S INGREDIENT MARKET", Text("ShopName"));
            Assert.AreEqual("Croissant", Text("ItemName"));
            Assert.AreEqual("Upgrading this will increase 1 dish by $1", Text("UpgradeInfo"));
            Assert.AreEqual("Cost : 1", Text("UpgradeCost"));
            Assert.AreEqual("Can't Afford", BuyText);
            Assert.IsFalse(Active("WesternButton"));
            // ten empty stars from x = 242
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(new Vector2(242 + i * 60, 410), StagePos("Star" + i));
                Assert.AreEqual("star_empty", TestUtil.Find<Image>("Star" + i).sprite.name);
            }
            Assert.IsFalse(Active("IngredientPreview0"));
            TestUtil.Capture("Kitchen_Ingredient0");
            TestUtil.Click("RightButton");
            Assert.AreEqual("Vegetables", Text("ItemName"));
            // lettuce: veggie croissant, doner kebab, gyro style (the Asian dishes owned don't use it)
            Assert.AreEqual("Upgrading this will increase 3 dishes by $1", Text("UpgradeInfo"));
            yield return Settle();
            TestUtil.Capture("Kitchen_Ingredient1");
        }

        [UnityTest]
        public IEnumerator BuyIngredientsAndSecretIngredient()
        {
            yield return LoadKitchen(40, 1000, p =>
            {
                p.ingredientLevel[0] = 4;
                p.achievementCounter[AchievementList.SecretIngredient] = 4;
            });
            TestUtil.Click("ShopQuad");
            Assert.AreEqual("Upgrade", BuyText);
            Assert.AreEqual("Cost : 4", Text("UpgradeCost"));
            Assert.AreEqual("star", TestUtil.Find<Image>("Star2").sprite.name);
            Assert.AreEqual("star_empty", TestUtil.Find<Image>("Star3").sprite.name);
            TestUtil.Click("BuyButton");
            Assert.AreEqual(5, Player.ingredientLevel[0]);
            Assert.AreEqual(996, Player.money);
            Assert.AreEqual(5, Player.achievementCounter[AchievementList.SecretIngredient]);
            Assert.AreEqual("star", TestUtil.Find<Image>("Star3").sprite.name);
            Assert.AreEqual("Cost : 5", Text("UpgradeCost"));
            yield return TestUtil.Seconds(1.2f);
            Assert.AreEqual("You've upgraded an ingredient to 5/10 level!", TestUtil.Find<TextMeshProUGUI>("Info", true).text);

            // up to the max level (11): 5 + 6 + ... + 10
            for (int level = 5; level < 11; level++) TestUtil.Click("BuyButton");
            Assert.AreEqual(11, Player.ingredientLevel[0]);
            Assert.AreEqual(996 - (5 + 6 + 7 + 8 + 9 + 10), Player.money);
            Assert.IsTrue(Player.unlocked[AchievementList.SecretIngredient]);
            Assert.AreEqual("Maxed", BuyText);
            Assert.AreEqual("You've maxed this ingredient quality.", Text("UpgradeInfo"));
            Assert.IsFalse(Active("UpgradeCost") || Active("ShopCoin"));
            Assert.IsFalse(kitchen.IsBuyTouchable);
            TestUtil.Capture("Kitchen_Ingredient_Maxed");

            // bacon (shop index 5): no owned recipe uses it
            for (int i = 0; i < 5; i++)
            {
                TestUtil.Click("RightButton");
                yield return Settle();
            }
            Assert.AreEqual("Bacon", Text("ItemName"));
            Assert.AreEqual("Sold Out!", BuyText);
            Assert.AreEqual("No recipe with this ingredient yet.", Text("UpgradeInfo"));
            Assert.IsFalse(kitchen.IsBuyTouchable);
            // plates loop through the 12 ingredients
            Assert.AreEqual(4, kitchen.Slots[0].ItemIndex);
            Assert.AreEqual(5, kitchen.Slots[1].ItemIndex);
            Assert.AreEqual(512, kitchen.Slots[1].X, 0.01f);
            Assert.AreEqual("bacon", kitchen.Slots[1].Item.sprite.name);
            TestUtil.Capture("Kitchen_Ingredient_SoldOut");
            // pita bread (index 9): doner kebab (bought on day 21)
            for (int i = 0; i < 4; i++)
            {
                TestUtil.Click("RightButton");
                yield return Settle();
            }
            Assert.AreEqual("Pita Bread", Text("ItemName"));
            Assert.AreEqual("Upgrading this will increase 1 dish by $1", Text("UpgradeInfo"));
            TestUtil.Click("BuyButton");
            Assert.AreEqual(2, Player.ingredientLevel[9]);
        }

        // ------------------------------------------------------------------ hardware shop

        [UnityTest]
        public IEnumerator HardwareShop()
        {
            yield return LoadKitchen(40, 100);
            TestUtil.Click("HardwareQuad");
            Assert.AreEqual(Shop.Hardware, kitchen.CurrentShop);
            Assert.AreEqual("HYPERMART", Text("ShopName"));
            Assert.AreEqual("Juicer", Text("ItemName"));
            Assert.AreEqual("Current price : $9 ($2 increase)", Text("UpgradeInfo"));
            Assert.AreEqual("Cost : 10", Text("UpgradeCost"));
            Assert.AreEqual("Upgrade", BuyText);
            for (int i = 1; i <= 5; i++) Assert.AreEqual(new Vector2(332 + i * 60, 410), StagePos("Star" + i));
            Assert.IsFalse(Active("Plate"), "no plates in the hardware store");
            TestUtil.Click("BuyButton");
            Assert.AreEqual(2, Player.hardwareLevel[GameRules.Juicer]);
            Assert.AreEqual(90, Player.money);
            Assert.AreEqual("Current price : $11 ($2 increase)", Text("UpgradeInfo"));
            Assert.AreEqual("Cost : 20", Text("UpgradeCost"));
            Assert.AreEqual("star", TestUtil.Find<Image>("Star1").sprite.name);
            Assert.AreEqual(2, Saved().hardwareLevel[GameRules.Juicer]);

            // cookies (level 0) are not for sale
            TestUtil.Click("RightButton");
            yield return Settle();
            TestUtil.Click("RightButton");
            yield return Settle();
            Assert.AreEqual("Cookies", Text("ItemName"));
            Assert.AreEqual("NOT AVAILABLE", Text("UpgradeCost"));
            Assert.IsFalse(Active("BuyButton") || Active("UpgradeInfo"));
            Assert.IsFalse(Active("RightButton"));
            TestUtil.Capture("Kitchen_Hardware_Cookies");
        }

        [UnityTest]
        public IEnumerator HardwarePagesWithoutMoney()
        {
            yield return LoadKitchen(40);
            TestUtil.Click("HardwareQuad");
            Assert.AreEqual("Can't Afford", BuyText);
            TestUtil.Capture("Kitchen_Hardware0");
            TestUtil.Click("RightButton");
            Assert.AreEqual("Fryer", Text("ItemName"));
            Assert.AreEqual("Current price : $12 ($2 increase)", Text("UpgradeInfo"));
            yield return Settle();
            TestUtil.Capture("Kitchen_Hardware1");
            TestUtil.Click("RightButton");
            Assert.AreEqual("Current Effect : 0%", Text("UpgradeInfo"));
            yield return Settle();
            TestUtil.Capture("Kitchen_Hardware2");
        }

        // ------------------------------------------------------------------ tutorial

        [UnityTest]
        public IEnumerator AfterLevel2TutorialBuysARecipe()
        {
            yield return LoadKitchen(2, 20);
            var tut = TutorialManager.Current;
            Assert.IsNotNull(tut);
            Assert.AreEqual(TutorialIds.AFTER_LEVEL_2, tut.CurrentDialogueId);
            Assert.AreEqual(0, tut.DialogueCounter);
            Assert.IsTrue(tut.IsButtonShowing);
            Assert.IsTrue(Player.tutorialShowed[TutorialIds.AFTER_LEVEL_2]);
            yield return TestUtil.Seconds(0.3f);
            TestUtil.Capture("Kitchen_Tutorial2_0");

            // the Next button blocks every kitchen button
            TestUtil.Click("RecipeQuad");
            Assert.AreEqual(Shop.None, kitchen.CurrentShop);
            TestUtil.Click("NextButton");
            Assert.AreEqual(1, tut.DialogueCounter);
            Assert.IsFalse(tut.IsButtonShowing);
            // only the recipe book may be opened now
            TestUtil.Click("PlayQuad");
            Assert.IsFalse(GameManager.Instance.Flow.IsTransitioning);
            TestUtil.Capture("Kitchen_Tutorial2_1");
            TestUtil.Click("RecipeBubble");
            Assert.AreEqual(Shop.Recipe, kitchen.CurrentShop);
            Assert.AreEqual(2, tut.DialogueCounter);
            Assert.IsTrue(tut.IsButtonShowing);
            TestUtil.Capture("Kitchen_Tutorial2_2");
            TestUtil.Click("NextButton");
            Assert.AreEqual(3, tut.DialogueCounter);
            // the buy button is blocked until the next recipe is shown
            TestUtil.Click("BuyButton");
            Assert.AreEqual(20, Player.money);
            TestUtil.Click("RightButton");
            Assert.AreEqual(4, tut.DialogueCounter);
            Assert.AreEqual(1, kitchen.CurrentFocus);
            yield return Settle();
            TestUtil.Capture("Kitchen_Tutorial2_4");
            TestUtil.Click("BuyButton");
            Assert.AreEqual(5, tut.DialogueCounter);
            Assert.AreEqual(1, Player.westernDishLevel[1]);
            Assert.AreEqual(15, Player.money);
            Assert.IsTrue(tut.IsButtonShowing);
            TestUtil.Capture("Kitchen_Tutorial2_5");
        }

        [UnityTest]
        public IEnumerator OpeningStoryHidesTheAchievementBubble()
        {
            yield return LoadKitchen(0);
            Assert.AreEqual(TutorialIds.OPENING_STORY, TutorialManager.Current.CurrentDialogueId);
            Assert.IsFalse(Active("AchievementBubble") || Active("AchievementQuad"));
            Assert.IsFalse(Active("RecipeBubble"));
            TestUtil.Capture("Kitchen_OpeningStory");
        }

        [UnityTest]
        public IEnumerator AfterLevel34TutorialOpensTheHardwareStore()
        {
            yield return LoadKitchen(34, 0);
            var tut = TutorialManager.Current;
            Assert.AreEqual(TutorialIds.AFTER_LEVEL_34, tut.CurrentDialogueId);
            TestUtil.Click("NextButton");
            Assert.AreEqual(1, tut.DialogueCounter);
            TestUtil.Click("ShopQuad");
            Assert.AreEqual(Shop.None, kitchen.CurrentShop, "only the hardware store may be opened");
            // the bubble advances the tutorial like the quad (the original only did it for the quad and soft-locked)
            TestUtil.Click("HardwareBubble");
            Assert.AreEqual(Shop.Hardware, kitchen.CurrentShop);
            Assert.AreEqual(2, tut.DialogueCounter);
            yield return null;
        }

        [UnityTest]
        public IEnumerator AfterLevel34TutorialViaTheQuad()
        {
            yield return LoadKitchen(34, 0);
            var tut = TutorialManager.Current;
            TestUtil.Click("NextButton");
            TestUtil.Click("HardwareQuad");
            Assert.AreEqual(Shop.Hardware, kitchen.CurrentShop);
            Assert.AreEqual(2, tut.DialogueCounter);
            Assert.AreEqual("Juicer", Text("ItemName"));
            yield return TestUtil.Seconds(0.3f);
            TestUtil.Capture("Kitchen_Tutorial34_2");
        }

        // ------------------------------------------------------------------ data

        [Test]
        public void ItemNamesMatchTheTranslationKeys()
        {
            foreach (var n in KitchenItems.Ingredients)
                Assert.AreEqual(KitchenItems.IngredientEnglish(n), KitchenItems.Translate(KitchenItems.IngredientEnglish(n)));
            Assert.AreEqual("Vegetables", KitchenItems.IngredientEnglish("selada"));
            Assert.AreEqual("Pita Bread", KitchenItems.IngredientEnglish("pita_bread"));
            Assert.AreEqual("French BBQ Combo", KitchenItems.DishEnglish("french_BBQ_combo"));
            Assert.AreEqual("Deluxe BBQ Combo", KitchenItems.DishEnglish("deluxe_BBQ_combo"));
            foreach (var list in new[] { KitchenItems.WesternDishes, KitchenItems.ChineseDishes, KitchenItems.MidEastDishes })
                foreach (var n in list)
                    Assert.AreEqual(KitchenItems.DishEnglish(n), KitchenItems.Translate(KitchenItems.DishEnglish(n)));
            foreach (var n in KitchenItems.Hardware)
                Assert.AreEqual(KitchenItems.HardwareEnglish(n), KitchenItems.Translate(KitchenItems.HardwareEnglish(n)));
        }
    }
}
