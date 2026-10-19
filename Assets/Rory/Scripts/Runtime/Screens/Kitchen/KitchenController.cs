using System.Collections.Generic;
using Rory.Data;
using Rory.Localization;
using Rory.Popups.Game;
using Rory.Save;
using Rory.Tutorials;
using Rory.Tweening;
using Rory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AchievementList = global::Rory.Achievements.Achievements;
using AchievementNotifier = global::Rory.Achievements.AchievementNotifier;

namespace Rory.Screens.Kitchen
{
    /// <summary>
    /// The kitchen between days (Kitchen.as): the room with its bouncing bubbles (play, recipe book, ingredient shop,
    /// hardware store, achievements) and the three shops drawn over it on the shop table: Emma's recipes (three
    /// regions), Milly's ingredient market and the Hypermart. Unlocks the region dishes and the hardware by day,
    /// runs the kitchen tutorials (AFTER_LEVEL_2, AFTER_LEVEL_8, AFTER_LEVEL_34, ...) and the recipe / ingredient
    /// achievements.
    /// <para>
    /// Every button reaches <see cref="OnButton"/> with its Kitchen.as id (<see cref="TutorialButtons"/>), through
    /// the checks of touchQuad (quads and TNonStateButtons) or touchButton (bubble sprites); the two differ slightly
    /// and the differences are kept (see <see cref="OnBubble"/>).
    /// </para>
    /// </summary>
    public class KitchenController : ScreenController, ITutorialHost
    {
        public enum KitchenShop { None = 0, Ingredient = 1, Recipe = 2, Hardware = 3 }
        public enum RecipeRegion { Western = 1, Chinese = 2, MidEast = 3 }

        /// <summary>Label (and state) of the buy button.</summary>
        public enum BuyLabel { Buy, Upgrade, CantAfford, Maxed, SoldOut, Info }

        public const float SlotX = 512, SlotSpacing = 550, SlotY = 280;
        public const float StarY = 410;
        public const float BackButtonRoomX = 60, BackButtonShopX = 800, BackButtonClosedX = 20;
        const float SwipeDistance = 70;
        const int SlotCount = 5, LoopedSlots = 4, StarCount = 10;

        [Header("Room: bubbles (bounce), invisible quads and hover glows")]
        [SerializeField] Button playBubble;
        [SerializeField] Button shopBubble;
        [SerializeField] Button hardwareBubble;
        [SerializeField] Button achievementBubble;
        [SerializeField] Button recipeBubble;
        [SerializeField] Button playQuad;
        [SerializeField] Button shopQuad;
        [SerializeField] Button hardwareQuad;
        [SerializeField] Button achievementQuad;
        [SerializeField] Button recipeQuad;
        [SerializeField] Image playGlow;
        [SerializeField] Image shopGlow;
        [SerializeField] Image achievementGlow;
        [SerializeField] Image recipeGlow;
        [SerializeField] Image hardwareGlow;

        [Header("Shop")]
        [SerializeField] KitchenSwipeArea shopBackground;
        [SerializeField] GameObject shopCoin;
        [SerializeField] TextMeshProUGUI shopName;
        [SerializeField] Button westernButton;
        [SerializeField] Button chineseButton;
        [SerializeField] Button midEastButton;
        [SerializeField] GameObject moneyPlaceBG;
        [SerializeField] TextMeshProUGUI moneyPreview;
        [SerializeField] GameObject coinPreview;
        [SerializeField] TextMeshProUGUI moneyPreviewMinus;
        [SerializeField] KitchenShopSlot[] slots = new KitchenShopSlot[SlotCount];
        [SerializeField] Image[] ingredientPreviews = new Image[SlotCount];
        [SerializeField] TextMeshProUGUI[] ingredientCounts = new TextMeshProUGUI[SlotCount];
        [SerializeField] Image[] stars = new Image[StarCount];
        [SerializeField] TextMeshProUGUI itemName;
        [SerializeField] TextMeshProUGUI upgradeInfo;
        [SerializeField] TextMeshProUGUI upgradeCost;
        [SerializeField] Button rightButton;
        [SerializeField] Button leftButton;
        [SerializeField] Button buyButton;
        [SerializeField] TextMeshProUGUI buyLabel;
        [SerializeField] Button backButton;

        [Header("Sprites (kitchen atlas)")]
        [SerializeField] Sprite buttonBlue;
        [SerializeField] Sprite buttonBlueDark;
        [SerializeField] Sprite buttonGreen;
        [SerializeField] Sprite buttonRed;
        [SerializeField] Sprite starFull;
        [SerializeField] Sprite starEmpty;

        PlayerProfile profile;
        bool canClick;          // _bolehKlik
        float startTouchX;
        bool touchedBG;
        int startPointLooping;
        float moneyMinusY;      // moneyPlaceBG.y
        BuyLabel buyState;
        int buyExpertNeeded;    // buyButton.idx for the "Info" case

        public KitchenShop CurrentShop { get; private set; }
        public int CurrentFocus { get; private set; }
        public RecipeRegion CurrentRegion { get; private set; } = RecipeRegion.Western;
        public bool IsItemTweening { get; private set; }
        public BuyLabel BuyState => buyState;
        public bool IsBuyTouchable => buyButton.interactable;
        public IReadOnlyList<KitchenShopSlot> Slots => slots;

        static TutorialManager Tutorial => TutorialManager.Current;
        static bool TutorialButtonShowing => Tutorial != null && Tutorial.IsButtonShowing;
        static bool CanClickThisButton(int id) => Tutorial == null || Tutorial.CanClickThisButton(id);

        static void Trigger(string action)
        {
            if (Tutorial != null) Tutorial.Trigger(action);
        }

        static DishDatabase Dishes => GameDatabase.Instance.dishes;

        // ================================================================== initialize

        void Start()
        {
            profile = Player ?? new PlayerProfile();
            Sound.PlayMusic("feeling-positive");
            if (Tutorial != null) Tutorial.Host = this;

            Wire(playBubble, TutorialButtons.PLAY_ID, false);
            Wire(shopBubble, TutorialButtons.SHOP_ID, false);
            Wire(hardwareBubble, TutorialButtons.HARDWARE_ID, false);
            Wire(achievementBubble, TutorialButtons.ACHIEVEMENT_ID, false);
            Wire(recipeBubble, TutorialButtons.RECIPE_ID, false);
            Wire(playQuad, TutorialButtons.PLAY_ID, true);
            Wire(shopQuad, TutorialButtons.SHOP_ID, true);
            Wire(hardwareQuad, TutorialButtons.HARDWARE_ID, true);
            Wire(achievementQuad, TutorialButtons.ACHIEVEMENT_ID, true);
            Wire(recipeQuad, TutorialButtons.RECIPE_ID, true);
            Wire(westernButton, TutorialButtons.WESTERN_BUTTON, true);
            Wire(chineseButton, TutorialButtons.CHINESE_BUTTON, true);
            Wire(midEastButton, TutorialButtons.MID_EAST_BUTTON, true);
            Wire(rightButton, TutorialButtons.RIGHT_BUTTON, true);
            Wire(leftButton, TutorialButtons.LEFT_BUTTON, true);
            Wire(buyButton, TutorialButtons.BUY_BUTTON, true);
            Wire(backButton, TutorialButtons.BACK_BUTTON, true);
            shopBackground.Began += OnSwipeBegan;
            shopBackground.Moved += OnSwipeMoved;
            moneyMinusY = moneyPreviewMinus.rectTransform.GetStagePosition().y;

            int passed = profile.passedDays;
            if (passed >= GameRules.UnlockRecipeShop) SetRoomButton(recipeBubble, recipeQuad, true);
            if (passed >= GameRules.UnlockIngredientShop) SetRoomButton(shopBubble, shopQuad, true);
            if (passed >= GameRules.UnlockHardwareShop) SetRoomButton(hardwareBubble, hardwareQuad, true);

            moneyPreview.text = GameRules.AddCommas(profile.money);

            // buttonAr order: play, shop, hardware, achievement, recipe
            foreach (var b in new[] { playBubble, shopBubble, hardwareBubble, achievementBubble, recipeBubble })
                TweenBubble((RectTransform)b.transform);

            UnlockByDay();

            Session.currentNotifWindow = 0;
            var kitchenWindows = KitchenWindows(passed);
            if (passed < 60 && kitchenWindows != null && kitchenWindows.Count > 0 && profile.lastDayKitchen == passed - 1)
                GameWindows.ShowKitchenWindows(kitchenWindows, Session.currentNotifWindow, null);

            if (Tutorial != null)
            {
                Tutorial.CheckTut(TutorialIds.OPENING_STORY, 0);
                Tutorial.CheckTut(TutorialIds.AFTER_LEVEL_1, 1);
                Tutorial.CheckTut(TutorialIds.AFTER_LEVEL_2, 2);
                Tutorial.CheckTut(TutorialIds.AFTER_LEVEL_7, 7);
                Tutorial.CheckTut(TutorialIds.AFTER_LEVEL_8, 8);
                Tutorial.CheckTut(TutorialIds.AFTER_LEVEL_34, 34);
                if (Tutorial.CurrentDialogueId == TutorialIds.OPENING_STORY)
                    SetRoomButton(achievementBubble, achievementQuad, false);
            }

            profile.lastDayKitchen = passed;
            Game.Saves.Save();
            CurrentRegion = RecipeRegion.Western;
            canClick = true;
        }

        /// <summary>
        /// LevelEvent.xml &lt;kitchen&gt; windows of a day. The original showed them on entering the kitchen, but every
        /// &lt;kitchen&gt; entry of the shipped data is commented out and the data model has no list for them.
        /// </summary>
        static IList<EventWindowData> KitchenWindows(int day) => null;

        /// <summary>Region dishes and hardware given for free once their day is reached (saved at once).</summary>
        void UnlockByDay()
        {
            var p = profile;
            if (p.passedDays >= GameRules.UnlockChineseDish && p.chineseDishLevel[0] == 0)
            {
                p.chineseDishLevel[0] = 1;
                p.chineseDishLevel[1] = 1;
                p.achievementCounter[AchievementList.AsianRecipe] += 2;
                Game.Saves.Save();
            }
            if (p.passedDays >= GameRules.UnlockMidEastDish && p.mideastDishLevel[0] == 0)
            {
                p.mideastDishLevel[0] = 1;
                p.mideastDishLevel[1] = 1;
                p.achievementCounter[AchievementList.MiddleEastRecipe] += 2;
                Game.Saves.Save();
            }
            if (p.passedDays >= GameRules.UnlockBeverage && p.hardwareLevel[GameRules.Juicer] == 0)
            {
                p.hardwareLevel[GameRules.Juicer] = 1;
                Game.Saves.Save();
            }
            if (p.passedDays >= GameRules.UnlockFries && p.hardwareLevel[GameRules.Fryer] == 0)
            {
                p.hardwareLevel[GameRules.Fryer] = 1;
                Game.Saves.Save();
            }
        }

        static void SetRoomButton(Button bubble, Button quad, bool visible)
        {
            bubble.gameObject.SetActive(visible);
            quad.gameObject.SetActive(visible);
        }

        void Wire(Button b, int id, bool quad)
        {
            if (quad) b.onClick.AddListener(() => OnQuad(id));
            else b.onClick.AddListener(() => OnBubble(id));
            var hot = b.GetComponent<KitchenHotspot>();
            if (hot != null) hot.HoverChanged += OnHover;
        }

        /// <summary>tweenBubbleText: up 25 px (0.2 s linear, after 0.5 s), down again with a bounce (0.4 s), forever.</summary>
        static void TweenBubble(RectTransform bubble)
        {
            float up = bubble.GetStagePosition().y - 25;
            bubble.TweenAnchoredY(-up, 0.2f).SetDelay(0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                float down = bubble.GetStagePosition().y + 25;
                bubble.TweenAnchoredY(-down, 0.4f).SetEase(Ease.BounceOut).OnComplete(() => TweenBubble(bubble));
            });
        }

        // ================================================================== input

        void OnHover(int id, bool hovered)
        {
            if (TutorialButtonShowing) return;
            if (!hovered)
            {
                foreach (var g in new[] { playGlow, shopGlow, recipeGlow, achievementGlow, hardwareGlow }) SetAlpha(g, 0);
                return;
            }
            switch (id)
            {
                case TutorialButtons.PLAY_ID: SetAlpha(playGlow, 1); break;
                case TutorialButtons.SHOP_ID: SetAlpha(shopGlow, 1); break;
                case TutorialButtons.RECIPE_ID: SetAlpha(recipeGlow, 1); break;
                case TutorialButtons.ACHIEVEMENT_ID: SetAlpha(achievementGlow, 1); break;
                case TutorialButtons.HARDWARE_ID: SetAlpha(hardwareGlow, 1); break;
            }
        }

        static void SetAlpha(Graphic g, float a)
        {
            if (g == null) return;
            var c = g.color;
            c.a = a;
            g.color = c;
        }

        void Leave(GameScene scene)
        {
            bool fading = Game.Flow.IsTransitioning;
            GoTo(scene);
            if (!fading) canClick = false;
        }

        /// <summary>touchQuad: the invisible quads of the room and every TNonStateButton of the shops.</summary>
        void OnQuad(int id)
        {
            if (TutorialButtonShowing) return;
            if (!canClick) return;
            if (!CanClickThisButton(id)) return;
            OnButton(id, true);
        }

        /// <summary>
        /// touchButton: the bubble sprites. Same as the quads except that the achievement bubble does not set
        /// Session.achievementsBackToMainMenu (only the quad did) and the hardware bubble does not report
        /// "clicked_hardware" to the tutorial, like the original.
        /// </summary>
        void OnBubble(int id)
        {
            if (TutorialButtonShowing) return;
            if (!canClick) return;
            if (!CanClickThisButton(id)) return;
            OnButton(id, false);
        }

        void OnButton(int id, bool quad)
        {
            switch (id)
            {
                case TutorialButtons.PLAY_ID:
                    Leave(GameScene.LevelSelect);
                    break;
                case TutorialButtons.SHOP_ID:
                    ResetItem();
                    Trigger(TutorialActions.ClickedIngredients);
                    ShowShop(KitchenShop.Ingredient);
                    break;
                case TutorialButtons.HARDWARE_ID:
                    ResetItem();
                    if (quad) Trigger(TutorialActions.ClickedHardware);
                    ShowShop(KitchenShop.Hardware);
                    break;
                case TutorialButtons.ACHIEVEMENT_ID:
                    if (quad) Session.achievementsBackToMainMenu = false;
                    Leave(GameScene.Achievements);
                    break;
                case TutorialButtons.RECIPE_ID:
                    ResetItem();
                    Trigger(TutorialActions.ClickedRecipeShop);
                    ShowShop(KitchenShop.Recipe);
                    break;
                case TutorialButtons.BACK_BUTTON:
                    if (IsItemTweening) return;
                    if (shopBackground.gameObject.activeSelf) CloseShop();
                    else Leave(GameScene.MainMenu);
                    break;
                case TutorialButtons.WESTERN_BUTTON:
                    SelectRegion(RecipeRegion.Western);
                    break;
                case TutorialButtons.CHINESE_BUTTON:
                    SelectRegion(RecipeRegion.Chinese);
                    break;
                case TutorialButtons.MID_EAST_BUTTON:
                    SelectRegion(RecipeRegion.MidEast);
                    break;
                case TutorialButtons.BUY_BUTTON:
                    BuyItem();
                    break;
                case TutorialButtons.LEFT_BUTTON:
                    if (IsItemTweening) return;
                    TweenItem(false);
                    break;
                case TutorialButtons.RIGHT_BUTTON:
                    Trigger(TutorialActions.RightButton);
                    if (Tutorial != null && Tutorial.CurrentDialogueId == TutorialIds.AFTER_LEVEL_2 && Tutorial.DialogueCounter == 2) return;
                    if (IsItemTweening) return;
                    TweenItem(true);
                    break;
            }
        }

        void SelectRegion(RecipeRegion region)
        {
            if (IsItemTweening) return;
            CurrentRegion = region;
            ResetItem();
            ShowShop(KitchenShop.Recipe);
        }

        /// <summary>BACK_BUTTON with a shop open: hides the shop; the back button stays at x = 20 (sic).</summary>
        void CloseShop()
        {
            westernButton.gameObject.SetActive(false);
            chineseButton.gameObject.SetActive(false);
            midEastButton.gameObject.SetActive(false);
            shopBackground.gameObject.SetActive(false);
            shopName.gameObject.SetActive(false);
            itemName.gameObject.SetActive(false);
            upgradeCost.gameObject.SetActive(false);
            upgradeInfo.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(false);
            moneyPlaceBG.SetActive(false);
            coinPreview.SetActive(false);
            moneyPreview.gameObject.SetActive(false);
            leftButton.gameObject.SetActive(false);
            rightButton.gameObject.SetActive(false);
            shopCoin.SetActive(false);
            SetBackButtonX(BackButtonClosedX);
            for (int i = 0; i < SlotCount; i++)
            {
                slots[i].PlateVisible = false;
                slots[i].ItemVisible = false;
                ingredientCounts[i].gameObject.SetActive(false);
                ingredientPreviews[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < StarCount; i++)
            {
                stars[i].gameObject.SetActive(false);
                stars[i].rectTransform.SetStagePosition(-150, stars[i].rectTransform.GetStagePosition().y);
            }
        }

        /// <summary>TNonStateButton x setter: x is the left edge (the pivot was moved to the centre on first placement).</summary>
        void SetBackButtonX(float x)
        {
            var rt = (RectTransform)backButton.transform;
            rt.SetStagePosition(x + rt.rect.width / 2, rt.GetStagePosition().y);
        }

        // ================================================================== swipe

        void OnSwipeBegan(float x)
        {
            if (TutorialButtonShowing) return;
            if (!CanClickThisButton(TutorialButtons.NONE)) return;
            startTouchX = x;
            if (!IsItemTweening) touchedBG = true;
        }

        void OnSwipeMoved(float x)
        {
            if (TutorialButtonShowing) return;
            if (!CanClickThisButton(TutorialButtons.NONE)) return;
            if (!touchedBG) return;
            if (x < startTouchX - SwipeDistance) TweenItem(true);
            else if (x > startTouchX + SwipeDistance) TweenItem(false);
        }

        // ================================================================== carousel

        int ListCount(KitchenShop shop) => shop switch
        {
            KitchenShop.Ingredient => KitchenItems.Ingredients.Count,
            // the original compares with the western list for every region (all have 6 dishes)
            KitchenShop.Recipe => KitchenItems.WesternDishes.Count,
            KitchenShop.Hardware => KitchenItems.Hardware.Count,
            _ => 0,
        };

        /// <summary>tweenItem: moves the plates one step (reverse = next item, to the left), recycling plates that
        /// left the screen two steps away, then refreshes the shop.</summary>
        void TweenItem(bool reverse)
        {
            if (reverse)
            {
                if (CurrentShop != KitchenShop.None && CurrentFocus == ListCount(CurrentShop) - 1) return;
            }
            else if (CurrentFocus == 0)
            {
                return;
            }
            touchedBG = false;
            IsItemTweening = true;
            CurrentFocus += reverse ? 1 : -1;

            for (int i = 0; i < LoopedSlots; i++)
            {
                var s = slots[i];
                if (reverse)
                {
                    if (s.X <= SlotX - 2 * SlotSpacing)
                    {
                        int count = CurrentShop == KitchenShop.Ingredient ? KitchenItems.Ingredients.Count
                            : CurrentShop == KitchenShop.Recipe ? KitchenItems.ChineseDishes.Count : -1;
                        if (count >= 0 && startPointLooping + 4 < count)
                        {
                            startPointLooping++;
                            s.X = SlotX + 2 * SlotSpacing;
                            s.ItemIndex = startPointLooping + 3;
                        }
                    }
                }
                else if (s.X >= SlotX + 2 * SlotSpacing && startPointLooping > 0)
                {
                    s.X = SlotX - 2 * SlotSpacing;
                    startPointLooping--;
                    s.ItemIndex = startPointLooping;
                }
            }
            for (int i = 0; i < LoopedSlots; i++)
            {
                var s = slots[i];
                float v = i == CurrentFocus % 4 ? 1 : 0.7f;
                float targetX = s.X + SlotSpacing * (reverse ? -1 : 1);
                var c = s.Container;
                c.TweenAnchoredX(targetX, 0.4f).SetEase(Ease.ExpoOut).OnComplete(() => IsItemTweening = false);
                c.TweenScale(v, 0.4f).SetEase(Ease.ExpoOut);
                s.Group.TweenAlpha(v, 0.4f).SetEase(Ease.ExpoOut);
            }
            ShowShop(CurrentShop);
        }

        void ResetItem()
        {
            startPointLooping = 0;
            CurrentFocus = 0;
            for (int i = 0; i < LoopedSlots; i++)
            {
                var s = slots[i];
                s.ItemIndex = i;
                s.X = SlotX + i * SlotSpacing;
                float v = i == CurrentFocus % 4 ? 1 : 0.7f;
                s.Scale = v;
                s.Alpha = v;
            }
        }

        // ================================================================== shops

        static Sprite KitchenSprite(string name) => GameDatabase.Instance.sprites.Get("kitchen", name);

        /// <summary>AS3 Vector access out of range gives 0 (the cost lists end with the max level).</summary>
        static int At(List<int> list, int i) => list != null && i >= 0 && i < list.Count ? list[i] : 0;

        static void SetVisible(Component c, bool on) => c.gameObject.SetActive(on);

        void SetBuyLabel(BuyLabel label)
        {
            buyState = label;
            buyLabel.text = label switch
            {
                BuyLabel.Buy => Loc.T("Buy"),
                BuyLabel.Upgrade => Loc.T("Upgrade"),
                BuyLabel.CantAfford => Loc.T("Can't Afford"),
                BuyLabel.Maxed => Loc.T("Maxed"),
                BuyLabel.SoldOut => Loc.T("Sold Out!"),
                _ => Loc.T("Info"),
            };
        }

        /// <summary>buyButton._image.texture and touchable (an untouchable button keeps its look and lets touches through).</summary>
        void SetBuyButton(Sprite sprite, bool touchable)
        {
            buyButton.image.sprite = sprite;
            buyButton.interactable = touchable;
            buyButton.image.raycastTarget = touchable;
        }

        void SetStar(int index, bool full, float x)
        {
            var s = stars[index];
            s.sprite = full ? starFull : starEmpty;
            s.gameObject.SetActive(true);
            s.rectTransform.SetStagePosition(x, StarY);
        }

        static string Cost(int cost) => Loc.F("Cost : {0}", GameRules.AddCommas(cost));

        /// <summary>showShop: opens or refreshes <paramref name="shop"/> on the current item.</summary>
        void ShowShop(KitchenShop shop)
        {
            CurrentShop = shop;
            SetVisible(shopBackground, true);
            SetVisible(shopName, true);
            SetVisible(itemName, true);
            SetVisible(upgradeCost, true);
            SetVisible(upgradeInfo, true);
            SetVisible(buyButton, true);
            moneyPlaceBG.SetActive(true);
            coinPreview.SetActive(true);
            SetVisible(moneyPreview, true);
            SetVisible(leftButton, true);
            SetVisible(rightButton, true);
            shopCoin.SetActive(true);
            SetBackButtonX(BackButtonShopX);

            switch (shop)
            {
                case KitchenShop.Ingredient: ShowIngredientShop(); break;
                case KitchenShop.Recipe: ShowRecipeShop(); break;
                case KitchenShop.Hardware: ShowHardwareShop(); break;
            }
            moneyPreview.text = GameRules.AddCommas(profile.money);
        }

        void ShowIngredientShop()
        {
            var p = profile;
            int focus = CurrentFocus;
            shopName.text = Loc.T("MILLY'S INGREDIENT MARKET");
            for (int i = 0; i < SlotCount; i++)
            {
                slots[i].ItemVisible = true;
                slots[i].PlateVisible = true;
                slots[i].SetItemOnPlate(KitchenSprite(KitchenItems.Ingredients[slots[i].ItemIndex]));
            }
            itemName.text = KitchenItems.Translate(KitchenItems.IngredientEnglish(KitchenItems.Ingredients[focus]));
            if (focus >= KitchenItems.Ingredients.Count - 1) SetVisible(rightButton, false);
            else if (focus <= 0) SetVisible(leftButton, false);

            int level = p.ingredientLevel[focus];
            int cost = At(Dishes.ingredients[focus].upgradeCost, level);
            if (level > 0)
            {
                for (int i = 1; i < GameRules.MaxIngredientLevel; i++)
                    SetStar(i - 1, level > 0 && i < level, 512 - 4 * 60 + i * 60 - 90);
            }

            // dishes owned (and of an open region) that use this ingredient
            int dishesUsing = 0;
            int total = GameRules.DishesPerRegion * 3;
            for (int d = 0; d < total; d++)
            {
                if (d < GameRules.DishesPerRegion && p.westernDishLevel[d] == 0) continue;
                if (d >= GameRules.DishesPerRegion && d < 2 * GameRules.DishesPerRegion
                    && (p.chineseDishLevel[d - GameRules.DishesPerRegion] == 0 || p.passedDays < 10)) continue;
                if (d >= 2 * GameRules.DishesPerRegion
                    && (p.mideastDishLevel[d - 2 * GameRules.DishesPerRegion] == 0 || p.passedDays < 20)) continue;
                foreach (int type in Dishes.dishes[d].ingredients)
                    if (focus == type - (focus > 8 ? 2 : 1)) dishesUsing++;
            }

            if (dishesUsing > 0)
            {
                upgradeInfo.text = dishesUsing > 1
                    ? Loc.F("Upgrading this will increase {0} dishes by $1", dishesUsing)
                    : Loc.F("Upgrading this will increase {0} dish by $1", dishesUsing);
                if (p.money < cost && level < GameRules.MaxIngredientLevel)
                {
                    SetBuyLabel(BuyLabel.CantAfford);
                    upgradeCost.text = Cost(cost);
                    SetBuyButton(buttonRed, false);
                }
                else if (level == 0)
                {
                    SetBuyLabel(BuyLabel.Buy);
                    upgradeCost.text = Cost(cost);
                    SetBuyButton(buttonGreen, true);
                }
                else if (level > 0 && level < GameRules.MaxIngredientLevel)
                {
                    SetBuyLabel(BuyLabel.Upgrade);
                    upgradeCost.text = Cost(cost);
                    SetBuyButton(buttonBlue, true);
                }
                else
                {
                    upgradeInfo.text = Loc.T("You've maxed this ingredient quality.");
                    SetBuyLabel(BuyLabel.Maxed);
                    SetVisible(upgradeCost, false);
                    SetBuyButton(buttonGreen, false);
                    shopCoin.SetActive(false);
                }
            }
            else
            {
                SetBuyLabel(BuyLabel.SoldOut);
                upgradeCost.text = Cost(cost);
                SetBuyButton(buttonRed, false);
                upgradeInfo.text = Loc.T("No recipe with this ingredient yet.");
            }
        }

        void ShowRecipeShop()
        {
            var p = profile;
            SetVisible(westernButton, p.passedDays >= GameRules.UnlockChineseDish);
            SetVisible(chineseButton, p.passedDays >= GameRules.UnlockChineseDish);
            SetVisible(midEastButton, p.passedDays >= GameRules.UnlockMidEastDish);
            shopName.text = Loc.T("EMMA'S RECIPES");
            westernButton.image.sprite = CurrentRegion == RecipeRegion.Western ? buttonBlueDark : buttonBlue;
            chineseButton.image.sprite = CurrentRegion == RecipeRegion.Chinese ? buttonBlueDark : buttonBlue;
            midEastButton.image.sprite = CurrentRegion == RecipeRegion.MidEast ? buttonBlueDark : buttonBlue;

            IReadOnlyList<string> names;
            IReadOnlyList<float> posX, posY;
            List<int> levels;
            int dishOffset;
            switch (CurrentRegion)
            {
                case RecipeRegion.Chinese:
                    names = KitchenItems.ChineseDishes; posX = KitchenItems.PosXChinese; posY = KitchenItems.PosYChinese;
                    levels = p.chineseDishLevel; dishOffset = GameRules.DishesPerRegion;
                    break;
                case RecipeRegion.MidEast:
                    names = KitchenItems.MidEastDishes; posX = KitchenItems.PosXMidEast; posY = KitchenItems.PosYMidEast;
                    levels = p.mideastDishLevel; dishOffset = 2 * GameRules.DishesPerRegion;
                    break;
                default:
                    names = KitchenItems.WesternDishes; posX = KitchenItems.PosXWestern; posY = KitchenItems.PosYWestern;
                    levels = p.westernDishLevel; dishOffset = 0;
                    break;
            }
            var dish = ShowRecipeRegion(names, levels, dishOffset);

            for (int i = 0; i < SlotCount; i++)
            {
                var s = slots[i];
                s.ItemVisible = true;
                s.PlateVisible = true;
                ingredientCounts[i].gameObject.SetActive(false);
                ingredientPreviews[i].gameObject.SetActive(false);
                s.SetItem(KitchenSprite(names[s.ItemIndex]), posX[s.ItemIndex], posY[s.ItemIndex]);
            }
            for (int i = 0; i < dish.ingredients.Count && i < SlotCount; i++)
            {
                int type = dish.ingredients[i];
                int shopIndex = type < 10 ? type - 1 : type - 2;
                var img = ingredientPreviews[i];
                img.sprite = KitchenSprite(KitchenItems.Ingredients[shopIndex] + "_small");
                img.SetNativeSize();
                img.gameObject.SetActive(true);
                ingredientCounts[i].gameObject.SetActive(true);
                ingredientCounts[i].text = "x" + dish.defaultTarget[i];
            }
        }

        /// <summary>The recipe-region block of showShop (identical for the three regions). Returns the focused dish.</summary>
        DishData ShowRecipeRegion(IReadOnlyList<string> names, List<int> levels, int dishOffset)
        {
            var p = profile;
            int focus = CurrentFocus;
            if (focus >= names.Count - 1) SetVisible(rightButton, false);
            else if (focus <= 0) SetVisible(leftButton, false);
            itemName.text = KitchenItems.Translate(KitchenItems.DishEnglish(names[focus]));

            var dish = Dishes.dishes[focus + dishOffset];
            int price = 0;
            foreach (int type in dish.ingredients)
                price += (p.ingredientLevel[type - 1] - 1) * Dishes.ingredients[type - 1].priceIncrease;
            int level = levels[focus];
            if (level > 0)
            {
                price += dish.defaultPrice + (level - 1) * dish.priceIncrease;
                upgradeInfo.text = Loc.F("Current price : ${0} (${1} increase)", price, dish.priceIncrease);
            }
            else
            {
                int bonus = price;
                price += dish.defaultPrice;
                upgradeInfo.text = bonus > 0
                    ? Loc.F("Dish price : ${0} (${1} ingredient bonus)", price, bonus)
                    : Loc.F("Dish price : ${0}", price);
            }
            // the original upgrade-cost table of the region: the region's focus-th dish
            int cost = At(dish.upgradeCost, level);
            if (level > 0)
            {
                for (int i = 1; i < GameRules.MaxDishLevel; i++)
                    SetStar(i, level > 1 && i <= level - 1, 512 - 2 * 60 + i * 60);
            }
            else
            {
                for (int i = 1; i < GameRules.MaxDishLevel; i++) stars[i].gameObject.SetActive(false);
            }

            // " Cost : N": the original prefixed an empty string and a space here
            string costText = " " + Cost(cost);
            int expertNeeded = dish.expertNeeded;
            if (focus == 5 && p.achievementCounter[AchievementList.InternationalChef] < expertNeeded)
            {
                SetBuyLabel(BuyLabel.Info);
                buyExpertNeeded = expertNeeded;
                upgradeCost.text = costText;
                SetBuyButton(buttonRed, true);
            }
            else if (p.money < cost && level < GameRules.MaxDishLevel)
            {
                SetBuyLabel(BuyLabel.CantAfford);
                upgradeCost.text = costText;
                SetBuyButton(buttonRed, false);
            }
            else if (level == 0)
            {
                SetBuyLabel(BuyLabel.Buy);
                upgradeCost.text = costText;
                SetBuyButton(buttonGreen, true);
            }
            else if (level > 0 && level < GameRules.MaxDishLevel)
            {
                SetBuyLabel(BuyLabel.Upgrade);
                upgradeCost.text = costText;
                SetBuyButton(buttonBlue, true);
            }
            else
            {
                upgradeInfo.text = Loc.F("Current price : ${0}", price);
                SetBuyLabel(BuyLabel.Maxed);
                SetVisible(upgradeCost, false);
                SetBuyButton(buttonGreen, false);
                shopCoin.SetActive(false);
            }
            return dish;
        }

        void ShowHardwareShop()
        {
            var p = profile;
            int focus = CurrentFocus;
            shopName.text = Loc.T("HYPERMART");
            if (focus >= KitchenItems.Hardware.Count - 1) SetVisible(rightButton, false);
            else if (focus <= 0) SetVisible(leftButton, false);
            itemName.text = KitchenItems.Translate(KitchenItems.HardwareEnglish(KitchenItems.Hardware[focus]));

            int level = p.hardwareLevel[focus];
            for (int i = 1; i <= 5; i++)
                SetStar(i, level > 1 && i <= level - 1, 512 - 3 * 60 + i * 60);

            var hw = Dishes.hardware[focus];
            // cookies (index 2): the values are customer patience in percent
            int value = hw.defaultPrice + (level - 1) * hw.priceIncrease;
            if (level > 0 && level < GameRules.MaxHardwareLevel)
            {
                upgradeInfo.text = focus != 2
                    ? Loc.F("Current price : ${0} (${1} increase)", value, hw.priceIncrease)
                    : Loc.F("Current Effect : {0}% ({1}% increase)", value, hw.priceIncrease);
            }
            else
            {
                upgradeInfo.text = focus != 2
                    ? Loc.F("Current Price : ${0}", value)
                    : Loc.F("Current Effect : {0}%", value);
            }

            int cost = At(hw.upgradeCost, level);
            if (p.money < cost && level < GameRules.MaxHardwareLevel)
            {
                SetBuyLabel(BuyLabel.CantAfford);
                SetVisible(upgradeInfo, true);
                upgradeCost.text = Cost(cost);
                SetBuyButton(buttonRed, false);
            }
            else if (level == 0)
            {
                // the texture and touchable state of the previous item stay (the button is hidden)
                SetBuyLabel(BuyLabel.Buy);
                SetVisible(buyButton, false);
                SetVisible(upgradeInfo, false);
                upgradeCost.text = Loc.T("NOT AVAILABLE");
            }
            else if (level > 0 && level < GameRules.MaxHardwareLevel)
            {
                SetBuyLabel(BuyLabel.Upgrade);
                SetVisible(upgradeInfo, true);
                upgradeCost.text = Cost(cost);
                SetBuyButton(buttonBlue, true);
            }
            else
            {
                SetBuyLabel(BuyLabel.Maxed);
                SetVisible(upgradeCost, false);
                SetBuyButton(buttonGreen, false);
                shopCoin.SetActive(false);
            }

            for (int i = 0; i < 3; i++)
            {
                slots[i].ItemVisible = true;
                slots[i].SetItemOnPlate(KitchenSprite(KitchenItems.Hardware[slots[i].ItemIndex]));
            }
        }

        // ================================================================== buying

        void BuyItem()
        {
            var p = profile;
            if (buyState == BuyLabel.Info)
            {
                GameWindows.ShowExpertNeededInfo(buyExpertNeeded);
                return;
            }
            Trigger(TutorialActions.BuyItem);
            Sound.PlaySfx("money_collected");
            int cost = 0;
            switch (CurrentShop)
            {
                case KitchenShop.Ingredient:
                {
                    cost = At(Dishes.ingredients[CurrentFocus].upgradeCost, p.ingredientLevel[CurrentFocus]);
                    p.ingredientLevel[CurrentFocus]++;
                    p.money -= cost;
                    SecretIngredient(p.ingredientLevel[CurrentFocus]);
                    ShowShop(KitchenShop.Ingredient);
                    break;
                }
                case KitchenShop.Recipe:
                {
                    List<int> levels;
                    int achievement, offset;
                    switch (CurrentRegion)
                    {
                        case RecipeRegion.Chinese:
                            levels = p.chineseDishLevel; achievement = AchievementList.AsianRecipe; offset = GameRules.DishesPerRegion;
                            break;
                        case RecipeRegion.MidEast:
                            levels = p.mideastDishLevel; achievement = AchievementList.MiddleEastRecipe; offset = 2 * GameRules.DishesPerRegion;
                            break;
                        default:
                            levels = p.westernDishLevel; achievement = AchievementList.WesternRecipe; offset = 0;
                            break;
                    }
                    cost = At(Dishes.dishes[CurrentFocus + offset].upgradeCost, levels[CurrentFocus]);
                    LevelUpDish(levels, achievement);
                    p.money -= cost;
                    ShowShop(KitchenShop.Recipe);
                    break;
                }
                case KitchenShop.Hardware:
                {
                    cost = At(Dishes.hardware[CurrentFocus].upgradeCost, p.hardwareLevel[CurrentFocus]);
                    p.hardwareLevel[CurrentFocus]++;
                    p.money -= cost;
                    ShowShop(KitchenShop.Hardware);
                    break;
                }
            }
            moneyPreview.text = GameRules.AddCommas(p.money);
            moneyPreviewMinus.text = "-" + cost;
            moneyPreviewMinus.gameObject.SetActive(true);
            var minus = moneyPreviewMinus.rectTransform;
            minus.KillTweens();
            minus.SetStagePosition(minus.GetStagePosition().x, moneyMinusY);
            minus.TweenAnchoredY(-(moneyMinusY + 50), 0.5f).OnComplete(() => Tweens.DelayedCall(0.5f, HideMoneyMinusText, minus));
            Game.Saves.Save();
        }

        /// <summary>Secret Ingredient: the counter is the highest ingredient level. As in the original, the progress
        /// banner shows on every purchase while the counter is exactly half the target.</summary>
        void SecretIngredient(int newLevel)
        {
            var p = profile;
            int id = AchievementList.SecretIngredient;
            if (p.unlocked[id]) return;
            if (newLevel > p.achievementCounter[id]) p.achievementCounter[id] = newLevel;
            if (p.achievementCounter[id] == AchievementList.Target[id] / 2f)
            {
                if (AchievementNotifier.Current != null) AchievementNotifier.Current.ShowProgress(id);
            }
            else if (p.achievementCounter[id] >= AchievementList.Target[id])
            {
                p.unlocked[id] = true;
                if (AchievementNotifier.Current != null) AchievementNotifier.Current.ShowUnlocked(id);
            }
        }

        /// <summary>Recipe level + 1; the first purchase of a recipe counts for the region's recipe book
        /// (progress banner at half the target, unlocked at the target).</summary>
        void LevelUpDish(List<int> levels, int achievement)
        {
            levels[CurrentFocus]++;
            if (levels[CurrentFocus] == 1)
            {
                var result = AchievementList.Add(profile, achievement, 1, AchievementList.Target[achievement] / 2);
                if (AchievementNotifier.Current != null) AchievementNotifier.Current.Show(achievement, result);
            }
        }

        void HideMoneyMinusText()
        {
            moneyPreviewMinus.gameObject.SetActive(false);
            var rt = moneyPreviewMinus.rectTransform;
            rt.SetStagePosition(rt.GetStagePosition().x, rt.GetStagePosition().y - 50);
        }

        // ================================================================== ITutorialHost

        public bool IsKitchen => true;
        public string JuicePercentageText => null;

        public bool TryGetTilePosition(int col, int row, out Vector2 position)
        {
            position = default;
            return false;
        }

        public bool TryGetFirstListedItem(out Vector2 position, out float height)
        {
            position = default;
            height = 0;
            return false;
        }

        public void OnTutorialCommand(TutorialCommand command) { }
    }
}
