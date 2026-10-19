using Rory.Localization;
using Rory.Screens.Kitchen;
using Rory.Tutorials;
using Rory.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.EditorTools
{
    /// <summary>
    /// Generates Scenes/Kitchen.unity from Kitchen.initialize(): the room (background, bubbles with their invisible
    /// quads, hover glows), then the shop layer drawn over it (shop table, names, region buttons, money, five plates,
    /// ingredient previews, stars, texts, arrows, buy and back buttons), in the original addChild order.
    /// The Free Coins button (ads) is not ported.
    /// </summary>
    public static class KitchenBuilder
    {
        static readonly Color White = Color.white;
        static readonly Color Black = Color.black;
        static readonly Color Red = UIBuild.Hex(16711680);
        static readonly Color Invisible = new Color(1, 1, 1, 0);

        [MenuItem("Rory/Build/Scenes/Kitchen")]
        [RoryBuilder(100)]
        public static void Build()
        {
            var stage = UIBuild.NewScene(out _);
            var s = stage.transform;
            Sprite K(string n) => UIBuild.Sprite("kitchen", n);

            UIBuild.Image(s, "Background", UIBuild.Background("kitchen_BG"));

            // ---------------------------------------------------------------- room
            var playSprite = K("play_bubble");
            var playBubble = Bubble(s, "PlayBubble", playSprite, 468, 388, TutorialButtons.PLAY_ID);
            var playQuad = HotQuad(s, "PlayQuad", 468 - 20, 388 + 50, playSprite.rect.width + 20, playSprite.rect.height + 10, TutorialButtons.PLAY_ID);

            var shopSprite = K("shop_bubble");
            var shopBubble = Bubble(s, "ShopBubble", shopSprite, 616, 340, TutorialButtons.SHOP_ID, false);
            var shopQuad = HotQuad(s, "ShopQuad", 616 + 20, 340 + 50, shopSprite.rect.width - 20, shopSprite.rect.height + 10,
                TutorialButtons.SHOP_ID, false);

            var hardwareBubble = Bubble(s, "HardwareBubble", K("convenience_bubble"), 20, 270, TutorialButtons.HARDWARE_ID, false);
            var hardwareQuad = HotQuad(s, "HardwareQuad", 120, 315, 100, 100, TutorialButtons.HARDWARE_ID, false);

            var achievementBubble = Bubble(s, "AchievementBubble", K("achievement_bubble"), 669, 152, TutorialButtons.ACHIEVEMENT_ID);
            var achievementQuad = HotQuad(s, "AchievementQuad", 669 + 100, 152 + 55, 70, 70, TutorialButtons.ACHIEVEMENT_ID);

            var recipeBubble = Bubble(s, "RecipeBubble", K("recipe_bubble"), 224, 143, TutorialButtons.RECIPE_ID, false);
            var recipeQuad = HotQuad(s, "RecipeQuad", 224 + 50, 143 + 55, 70, 70, TutorialButtons.RECIPE_ID, false);

            var playGlow = Glow(s, "PlayGlow", K("bookGlow"), 523.5f, 477.5f);
            var shopGlow = Glow(s, "ShopGlow", K("shopGlow"), 698.5f, 436);
            var achievementGlow = Glow(s, "AchievementGlow", K("achievementGlow"), 810.5f, 246.5f);
            var recipeGlow = Glow(s, "RecipeGlow", K("recipeGlow"), 304.5f, 241.5f);
            var hardwareGlow = Glow(s, "HardwareGlow", K("hardwareGlow"), 164, 371.5f);

            // ---------------------------------------------------------------- shop
            var shopBg = UIBuild.Image(s, "ShopBackground", UIBuild.Background("shop_table_BG"), 0, 0, raycast: true);
            var swipe = shopBg.gameObject.AddComponent<KitchenSwipeArea>();
            Hide(shopBg);

            var coin = UIBuild.Sprite("window", "coin");
            var shopCoin = UIBuild.Image(s, "ShopCoin", coin, 565, 470);
            shopCoin.rectTransform.localScale = new Vector3(0.5f, 0.5f, 1);
            Hide(shopCoin);

            var shopName = UIBuild.Text(s, "ShopName", "MILLY'S INGREDIENT MARKET", UIBuild.Fonts.Bromine, 35, White,
                512, 20, 600, 100, 300, 50, autoSize: true, localize: false);
            Hide(shopName);

            var blue = K("small_btn_blue");
            var western = RegionButton(s, "WesternButton", blue, 700, 70, "Western");
            var chinese = RegionButton(s, "ChineseButton", blue, 860, 70, "Asian");
            var midEast = RegionButton(s, "MidEastButton", blue, 860, 120, "Middle East");

            var coinBg = K("coin_BG");
            float bgW = coinBg.rect.width, bgH = coinBg.rect.height;
            var moneyBg = UIBuild.Image(s, "MoneyPlaceBG", coinBg, 30, 70);
            Hide(moneyBg);
            var money = UIBuild.Text(s, "MoneyPreview", "0", UIBuild.Fonts.Bromine, 25, Black, 30 + 5, 70, bgW - 20, bgH,
                align: TextAlignmentOptions.Right, localize: false);
            Hide(money);
            var coinPreview = UIBuild.Image(s, "CoinPreview", coin, 30 + 25, 70 + 5);
            coinPreview.rectTransform.localScale = new Vector3(0.7f, 0.7f, 1);
            Hide(coinPreview);
            var moneyMinus = UIBuild.Text(s, "MoneyPreviewMinus", "0", UIBuild.Fonts.Bromine, 25, Red, 30 + 5, 70, bgW - 20, bgH,
                align: TextAlignmentOptions.Right, localize: false);
            Hide(moneyMinus);

            var plate = K("plate");
            float pw = plate.rect.width, ph = plate.rect.height;
            var slots = new Object[5];
            var previews = new Object[5];
            var counts = new Object[5];
            for (int i = 0; i < 5; i++)
            {
                var container = UIBuild.Container(s, $"ShopSlot{i}", KitchenController.SlotX + i * KitchenController.SlotSpacing,
                    KitchenController.SlotY);
                var group = container.gameObject.AddComponent<CanvasGroup>();
                group.blocksRaycasts = false;
                group.interactable = false;
                var plateImg = Hide(UIBuild.ImageCentered(container, "Plate", plate, 0, 0));
                var item = Hide(UIBuild.ImageCentered(container, "Item", K("fine_imperial_cuisine"), 0, 0));
                var slot = container.gameObject.AddComponent<KitchenShopSlot>();
                UIBuild.SetField(slot, "container", container);
                UIBuild.SetField(slot, "group", group);
                UIBuild.SetField(slot, "plate", plateImg);
                UIBuild.SetField(slot, "item", item);
                UIBuild.SetField(slot, "pivot", new Vector2(pw / 2, ph / 2));
                slots[i] = slot;

                previews[i] = Hide(UIBuild.Image(s, $"IngredientPreview{i}", K("chicken_small"), 30 + i * 70, 520));
                counts[i] = Hide(UIBuild.Text(s, $"IngredientCount{i}", "x6", UIBuild.Fonts.KomikandyWhite, 25, White,
                    30 + i * 70, 540, 50, 50, localize: false));
            }

            var stars = new Object[10];
            for (int i = 0; i < 10; i++) stars[i] = Hide(UIBuild.ImageCentered(s, $"Star{i}", K("star_empty"), -150, 0));

            var itemName = Hide(UIBuild.Text(s, "ItemName", "", UIBuild.Fonts.Pfennig, 35, White, 512, 120, 400, 150, 200, 75,
                autoSize: true, localize: false));
            var upgradeInfo = Hide(UIBuild.Text(s, "UpgradeInfo", "Current price : $45 ($10 increase)", UIBuild.Fonts.Bromine, 25, White,
                512, 450, 600, 70, 300, 35, autoSize: true, localize: false));
            var upgradeCost = Hide(UIBuild.Text(s, "UpgradeCost", "Cost : 250", UIBuild.Fonts.Bromine, 25, White,
                512, 490, 400, 70, 200, 35, autoSize: true, localize: false));

            // arrows: pivot set to the centre first, so (x, y) is the centre; they grow 20% on hover
            var arrow = K("arrow");
            var right = UIBuild.Button(s, "RightButton", arrow, 712, 300, hoverScale: 1.2f);
            right.transform.localScale = new Vector3(-1, 1, 1);
            Hide(right);
            var left = UIBuild.Button(s, "LeftButton", arrow, 312, 300, hoverScale: 1.2f);
            Hide(left);

            // buy: label box 15 px narrower, at x 7
            var green = K("small_btn_green");
            var buy = UIBuild.Button(s, "BuyButton", green, 580, 535, topLeft: true,
                label: "Buy", font: UIBuild.Fonts.Bromine, fontSize: 25, labelColor: Black);
            var buyLabel = buy.GetComponentInChildren<TextMeshProUGUI>();
            Object.DestroyImmediate(buyLabel.GetComponent<Localize>());
            float bw = green.rect.width;
            buyLabel.rectTransform.sizeDelta = new Vector2(bw - 15, green.rect.height);
            buyLabel.rectTransform.anchoredPosition = new Vector2(7 + (bw - 15) / 2 - bw / 2, 0);
            Hide(buy);

            var back = UIBuild.Button(s, "BackButton", K("small_btn_red"), KitchenController.BackButtonRoomX, 535, topLeft: true,
                label: "Back", font: UIBuild.Fonts.Bromine, fontSize: 25, labelColor: Black);

            TutorialBuilder.AddTo(stage);
            AchievementBuilder.AddNotifier(stage);

            var c = new GameObject("KitchenController").AddComponent<KitchenController>();
            UIBuild.SetField(c, "playBubble", playBubble);
            UIBuild.SetField(c, "shopBubble", shopBubble);
            UIBuild.SetField(c, "hardwareBubble", hardwareBubble);
            UIBuild.SetField(c, "achievementBubble", achievementBubble);
            UIBuild.SetField(c, "recipeBubble", recipeBubble);
            UIBuild.SetField(c, "playQuad", playQuad);
            UIBuild.SetField(c, "shopQuad", shopQuad);
            UIBuild.SetField(c, "hardwareQuad", hardwareQuad);
            UIBuild.SetField(c, "achievementQuad", achievementQuad);
            UIBuild.SetField(c, "recipeQuad", recipeQuad);
            UIBuild.SetField(c, "playGlow", playGlow);
            UIBuild.SetField(c, "shopGlow", shopGlow);
            UIBuild.SetField(c, "achievementGlow", achievementGlow);
            UIBuild.SetField(c, "recipeGlow", recipeGlow);
            UIBuild.SetField(c, "hardwareGlow", hardwareGlow);
            UIBuild.SetField(c, "shopBackground", swipe);
            UIBuild.SetField(c, "shopCoin", shopCoin.gameObject);
            UIBuild.SetField(c, "shopName", shopName);
            UIBuild.SetField(c, "westernButton", western);
            UIBuild.SetField(c, "chineseButton", chinese);
            UIBuild.SetField(c, "midEastButton", midEast);
            UIBuild.SetField(c, "moneyPlaceBG", moneyBg.gameObject);
            UIBuild.SetField(c, "moneyPreview", money);
            UIBuild.SetField(c, "coinPreview", coinPreview.gameObject);
            UIBuild.SetField(c, "moneyPreviewMinus", moneyMinus);
            UIBuild.SetArray(c, "slots", slots);
            UIBuild.SetArray(c, "ingredientPreviews", previews);
            UIBuild.SetArray(c, "ingredientCounts", counts);
            UIBuild.SetArray(c, "stars", stars);
            UIBuild.SetField(c, "itemName", itemName);
            UIBuild.SetField(c, "upgradeInfo", upgradeInfo);
            UIBuild.SetField(c, "upgradeCost", upgradeCost);
            UIBuild.SetField(c, "rightButton", right);
            UIBuild.SetField(c, "leftButton", left);
            UIBuild.SetField(c, "buyButton", buy);
            UIBuild.SetField(c, "buyLabel", buyLabel);
            UIBuild.SetField(c, "backButton", back);
            UIBuild.SetField(c, "buttonBlue", blue);
            UIBuild.SetField(c, "buttonBlueDark", K("small_btn_blue_dark"));
            UIBuild.SetField(c, "buttonGreen", green);
            UIBuild.SetField(c, "buttonRed", K("small_btn_red"));
            UIBuild.SetField(c, "starFull", K("star"));
            UIBuild.SetField(c, "starEmpty", K("star_empty"));

            UIBuild.SaveScene("Kitchen");
        }

        static T Hide<T>(T c) where T : Component
        {
            c.gameObject.SetActive(false);
            return c;
        }

        /// <summary>A bouncing room bubble (cSprite with a touch listener; no click sound, no hover scale).</summary>
        static Button Bubble(Transform s, string name, Sprite sprite, float x, float y, int id, bool visible = true)
        {
            var img = UIBuild.Image(s, name, sprite, x, y, raycast: true);
            var b = img.gameObject.AddComponent<Button>();
            b.transition = Selectable.Transition.None;
            b.targetGraphic = img;
            img.gameObject.AddComponent<KitchenHotspot>().buttonId = id;
            img.gameObject.SetActive(visible);
            return b;
        }

        /// <summary>The invisible touch area (cQuad, alpha 0) in front of a bubble.</summary>
        static Button HotQuad(Transform s, string name, float x, float y, float w, float h, int id, bool visible = true)
        {
            var img = UIBuild.Quad(s, name, x, y, w, h, Invisible, true);
            var b = img.gameObject.AddComponent<Button>();
            b.transition = Selectable.Transition.None;
            b.targetGraphic = img;
            img.gameObject.AddComponent<KitchenHotspot>().buttonId = id;
            img.gameObject.SetActive(visible);
            return b;
        }

        static Image Glow(Transform s, string name, Sprite sprite, float x, float y)
        {
            var img = UIBuild.ImageCentered(s, name, sprite, x, y);
            img.color = Invisible;
            return img;
        }

        /// <summary>A 70% region button: TNonStateButton(small_btn_blue, label, "bromine", 25, 0) scaled 0.7 at (x, y).</summary>
        static Button RegionButton(Transform s, string name, Sprite sprite, float x, float y, string label)
        {
            var b = UIBuild.Button(s, name, sprite, x, y, topLeft: true, label: label, font: UIBuild.Fonts.Bromine, fontSize: 25,
                labelColor: Black);
            b.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            b.gameObject.SetActive(false);
            return b;
        }
    }
}
