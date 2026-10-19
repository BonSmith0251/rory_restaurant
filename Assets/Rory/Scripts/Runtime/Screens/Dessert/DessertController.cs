using System.Collections.Generic;
using Rory.Achievements;
using Rory.Data;
using Rory.Localization;
using Rory.Popups;
using Rory.Popups.Game;
using Rory.Save;
using Rory.Tutorials;
using Rory.Tweening;
using Rory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AchievementList = Rory.Achievements.Achievements;

namespace Rory.Screens.Dessert
{
    /// <summary>
    /// Frozen-yogurt minigame (MiniGameMakeDessert): story days of type DM and the Dessert Making extras game.
    /// <para>
    /// A customer slides in with an order on the preview paper (a set from <c>DessertDatabase.sets</c> for the current
    /// level, filled with random cups / yogurts / syrups / toppings / beverages); the player taps the ingredient stations
    /// in the order's sequence to build the dessert on the pan. A wrong ingredient throws the dessert away (start again);
    /// the last right one serves it for the sum of the items' worth. A customer whose patience runs out leaves (extras:
    /// one life less). Story days end when the time is up (results, coins, achievements, save); the extras game runs
    /// through five levels of increasing goals (three lives).
    /// </para>
    /// <para>
    /// Per-frame logic of the original (30 fps) runs on time: the day timer and the customer's patience are counted in
    /// original frames (<see cref="FramesPerSecond"/> per second). <see cref="Level"/> is the original "level"
    /// (story: (currentPlayedLevel + 1) / 14 + 1, extras 1..5); <see cref="GameSession.currentPlayedLevel"/> is 0-based.
    /// </para>
    /// </summary>
    public class DessertController : ScreenController, ITutorialHost
    {
        // Ingredient types (MakeDessert.xml "type")
        public const int CUP = 1;
        public const int YOGURT = 2;
        public const int SYRUP = 3;
        public const int TOPPING = 4;
        public const int BEVERAGE = 5;

        /// <summary>The original's frame rate: the timer and patience count frames.</summary>
        public const float FramesPerSecond = 30;
        public const int PlateCount = 15;
        public const int TileCount = 14;
        public const int ExtrasLives = 3;
        public const int ExtrasLastLevel = 5;

        // Tween targets (stage x of the pivot) of tweenIn / tweenOut
        const float OutX = -250;
        const float CustomerInX = -20;
        const float BarInX = 0;
        const float HeartInX = 25;
        const float FillInX = 47;
        const float PanInX = 450;

        const string Atlas = "make_dessert";

        [Header("HUD")]
        [SerializeField] Button pauseButton;
        [SerializeField] Button soundButton;
        [SerializeField] GameObject soundMuteIcon;
        [SerializeField] Button musicButton;
        [SerializeField] GameObject musicMuteIcon;
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] TextMeshProUGUI goalValueText;
        [SerializeField] TextMeshProUGUI goalLabelText;
        [Tooltip("timer_fill, Image Filled horizontally (the clipRect of timeFillSprite)")]
        [SerializeField] Image timerFill;
        [SerializeField] TextMeshProUGUI livesText;

        [Header("Customer")]
        [SerializeField] Image customer;
        [Tooltip("customer.pivotY: the height of char_1, kept in pixels when the texture changes (readjustSize)")]
        [SerializeField] float customerPivotY = 166;
        [SerializeField] RectTransform customerBarBG;
        [SerializeField] RectTransform heart;
        [Tooltip("cust_fill, Image Filled horizontally (the clipRect of custFillSprite)")]
        [SerializeField] Image customerFill;
        [SerializeField] RectTransform customerGlass;

        [Header("Dessert")]
        [SerializeField] RectTransform previewPaper;
        [SerializeField] RectTransform pan;
        [Tooltip("onPlateObjAr: one image per ingredient id")]
        [SerializeField] Image[] plateItems = new Image[PlateCount];
        [SerializeField] IngredientTile[] tiles = new IngredientTile[TileCount];
        [Tooltip("previewAr: the order on the preview paper")]
        [SerializeField] Image[] previews = new Image[PlateCount];
        [Tooltip("Layer above the tutorial bubble and below the windows: placed ingredients (except beverages) go to its top " +
                 "(setChildIndex(obj, index of the black screen - 1))")]
        [SerializeField] RectTransform plateTop;

        [Header("Extras level banner")]
        [Tooltip("The shared 70% black screen shown while the level banner flies by")]
        [SerializeField] GameObject blackScreen;
        [SerializeField] TextMeshProUGUI bigInfoText;

        // ------------------------------------------------------------------ data (copied from the database)

        float[] posX, posY;
        string[] itemName;
        int[] itemPrice, itemType;
        readonly List<List<int>> availableSet = new List<List<int>>();

        // ------------------------------------------------------------------ state

        PlayerProfile profile;
        DessertTimeline timeline;
        readonly List<int> orderList = new List<int>();
        List<int> choosenSet;
        int currentLayer;
        int lives;
        int level;
        int earning;
        int price;
        int goalTarget, expertTarget, froyoMadeCount;
        /// <summary>goalOrExpertText.idx: 1 = goal shown, 2 = expert shown, 3 = expert reached.</summary>
        int goalState = 1;
        float time, totalTime;
        bool timesUp;
        float patience, totalPatience;
        bool orderInPlace;
        bool hasWindow, paused, ready;
        bool resultShown;

        // ------------------------------------------------------------------ read-only state (tests, inspector)

        public int Level => level;
        public int Lives => lives;
        public int Earning => earning;
        public int Price => price;
        public int FroyoMade => froyoMadeCount;
        public int GoalTarget => goalTarget;
        public int ExpertTarget => expertTarget;
        public IReadOnlyList<int> OrderList => orderList;
        public int CurrentLayer => currentLayer;
        public bool OrderInPlace => orderInPlace;
        public bool TimesUp => timesUp;
        /// <summary>Day time left in original frames (30 per second).</summary>
        public float TimeLeft => time;
        public float PatienceLeft => patience;
        public bool HasWindow => hasWindow;
        public bool Paused => paused;
        public DessertTimeline Timeline => timeline;
        public int ItemCount => itemName != null ? itemName.Length : 0;
        public int ItemType(int id) => itemType[id];
        public int ItemWorth(int id) => itemPrice[id];

        static TutorialManager Tut => TutorialManager.Current;
        static AchievementNotifier Notifier => AchievementNotifier.Current;
        bool Extras => Session.extrasMode;

        // ------------------------------------------------------------------ lifecycle

        void Start()
        {
            profile = Player ?? new PlayerProfile();
            level = (int)((float)(Session.currentPlayedLevel + 1) / 14 + 1);
            if (Extras)
            {
                level = 1;
                lives = ExtrasLives;
            }
            if (Tut != null) Tut.Host = this;

            pauseButton.onClick.AddListener(OnPauseButton);
            soundButton.onClick.AddListener(() =>
            {
                Sound.MuteSound = !Sound.MuteSound;
                RefreshMuteIcons();
            });
            musicButton.onClick.AddListener(() =>
            {
                Sound.MuteMusic = !Sound.MuteMusic;
                RefreshMuteIcons();
            });

            ReadItems();
            SetLevelSet();
            Initialize();
        }

        /// <summary>readXML: the ingredient table (positions on the pan, sprite names, worth, type).</summary>
        void ReadItems()
        {
            var items = Db.desserts.items;
            int n = items.Count;
            posX = new float[n];
            posY = new float[n];
            itemName = new string[n];
            itemPrice = new int[n];
            itemType = new int[n];
            for (int i = 0; i < n; i++)
            {
                posX[i] = items[i].posX;
                posY[i] = items[i].posY;
                itemName[i] = items[i].name;
                itemPrice[i] = items[i].worth;
                itemType[i] = items[i].type;
            }
        }

        /// <summary>setLevelSet: the order sets of every level up to the current one (copies: setOrder appends to them).</summary>
        void SetLevelSet()
        {
            availableSet.Clear();
            foreach (var s in Db.desserts.sets)
                if (s.level <= level) availableSet.Add(new List<int>(s.types));
        }

        void Initialize()
        {
            Sound.PlayMusic("sunny-bliss");
            timeline = new DessertTimeline();
            earning = 0;
            froyoMadeCount = 0;
            timesUp = false;
            int day = Session.currentPlayedLevel;
            if (!Extras)
            {
                var d = Session.CurrentDay;
                time = (int)(d.time * FramesPerSecond);
                totalTime = time;
                goalTarget = d.goal;
                expertTarget = d.expert;
            }
            else
            {
                var d = Db.levels.dessertLevels[level - 1];
                time = (int)(d.time * FramesPerSecond);
                totalTime = time;
                goalTarget = d.goal;
            }

            RefreshMuteIcons();
            scoreText.text = "$ 0";
            goalValueText.text = "$ " + goalTarget;
            goalLabelText.text = Loc.T("Goal:");
            goalState = 1;
            if (!Extras && profile.goalAchieved[day])
            {
                goalValueText.text = "$ " + expertTarget;
                goalLabelText.text = Loc.T("EXPERT:");
                goalState = 2;
            }
            timerFill.fillAmount = 1;
            customerFill.fillAmount = 1;

            livesText.gameObject.SetActive(Extras);
            if (Extras) livesText.text = Loc.F("LIVES x{0}", lives);
            bigInfoText.gameObject.SetActive(Extras);
            blackScreen.SetActive(false);

            Session.currentNotifWindow = 0;
            bool windowShown = false;
            if (!Extras)
            {
                windowShown = GameWindows.ShowBeginningWindows(day, new BeginningWindowOptions
                {
                    startIndex = Session.currentNotifWindow,
                    goalTarget = goalTarget,
                    expertTarget = expertTarget,
                    onAllClosed = OnBeginningWindowsClosed,
                });
            }
            else
            {
                ShowCurrentLevel();
            }
            orderInPlace = false;
            if (!Extras && Tut != null)
            {
                Tut.CheckTut(TutorialIds.LEVEL_11, 10);
                Tut.CheckTut(TutorialIds.LEVEL_25, 24, true);
            }
            hasWindow = true;
            paused = false;
            ready = true;
            // Every DM day has a beginning window; without one, start like its Continue button would.
            if (!Extras && !windowShown) OnBeginningWindowsClosed();
        }

        /// <summary>NEXT_WINDOW after the last day window: startLevel() unless the tutorial's Next button is showing.</summary>
        void OnBeginningWindowsClosed()
        {
            if (Tut == null || !Tut.IsButtonShowing) StartLevel();
        }

        void Update()
        {
            // TweenLite runs on its own clock, whatever the screen's flags
            timeline?.Advance(Time.deltaTime);

            if (!ready || hasWindow || paused) return;
            var tut = Tut;
            if (tut != null && tut.CurrentDialogueId == TutorialIds.LEVEL_11) return;
            bool bubble = tut != null && tut.IsShowing;
            float frames = Time.deltaTime * FramesPerSecond;
            if (!timesUp && !bubble)
            {
                time -= frames;
                timerFill.fillAmount = Mathf.Clamp01(time / totalTime);
                if (time <= 0) timesUp = true;
            }
            if (orderInPlace && !bubble)
            {
                patience -= frames;
                customerFill.fillAmount = Mathf.Clamp01(patience / totalPatience);
                if (patience <= 0) CustomerLeaves();
            }
        }

        /// <summary>The customer's patience ran out: extras lose a life; the customer leaves without his order.</summary>
        void CustomerLeaves()
        {
            if (Extras)
            {
                lives--;
                livesText.text = Loc.F("LIVES x{0}", lives);
                var t = livesText.transform;
                t.localScale = new Vector3(0, 0, 1);
                if (lives <= 0) timesUp = true;
                t.KillTweens();
                t.TweenScale(1, 1).SetEase(Ease.BackOut).OnComplete(() =>
                {
                    if (lives <= 0) ShowResult(() => GameWindows.ShowDessertExtrasFailed(Result()));
                });
            }
            Sound.PlaySfx("bell_dring");
            orderInPlace = false;
            TweenOut();
            HidePreviews();
        }

        void OnApplicationFocus(bool focus)
        {
            // Event.DEACTIVATE: the original paused the screen when the window lost focus. Not in batch mode (no window).
            if (!focus && ready && !Application.isBatchMode) PauseOnDeactivate();
        }

        /// <summary>Pause(): on deactivation, unless a window (or the black screen) is up.</summary>
        void PauseOnDeactivate()
        {
            if (BlackScreenVisible) return;
            paused = true;
            timeline.Pause();
            ShowPauseWindow();
        }

        /// <summary>Resume(): back from the pause window; the timeline only stops on deactivation.</summary>
        void Resume()
        {
            if (BlackScreenVisible) return;
            paused = false;
            timeline.Resume();
        }

        bool BlackScreenVisible => blackScreen.activeSelf || (PopupManager.Current != null && PopupManager.Current.HasOpenPopup);

        // ------------------------------------------------------------------ buttons

        void OnPauseButton()
        {
            paused = true;
            ShowPauseWindow();
        }

        void ShowPauseWindow() => GameWindows.ShowPause(new PauseOptions
        {
            onResume = () =>
            {
                paused = false;
                Resume();
            },
            onMuteChanged = RefreshMuteIcons,
        });

        void RefreshMuteIcons()
        {
            soundMuteIcon.SetActive(Sound.MuteSound);
            musicMuteIcon.SetActive(Sound.MuteMusic);
        }

        // ------------------------------------------------------------------ level flow

        /// <summary>startLevel: the first customer comes in; every time the timeline ends, checkTimesUp.</summary>
        public void StartLevel()
        {
            hasWindow = false;
            TweenIn();
            timeline.OnComplete = CheckTimesUp;
        }

        /// <summary>showCurrentLevel (extras): "LEVEL: N" flies over the black screen; the first time it starts the level.</summary>
        void ShowCurrentLevel()
        {
            bigInfoText.text = Loc.F("LEVEL: {0}", level);
            blackScreen.SetActive(true);
            var rt = bigInfoText.rectTransform;
            rt.KillTweens();
            rt.anchoredPosition = new Vector2(-400, rt.anchoredPosition.y);
            rt.TweenAnchoredX(512, 1).SetEase(Ease.ExpoOut).OnComplete(() =>
                rt.TweenAnchoredX(1424, 1).SetEase(Ease.ExpoIn).OnComplete(() =>
                {
                    blackScreen.SetActive(false);
                    if (hasWindow)
                    {
                        StartLevel();
                        hasWindow = false;
                    }
                }));
        }

        /// <summary>setOrder: the next customer's order, shown on the preview paper.</summary>
        void SetOrder()
        {
            int toppingCount = 0;
            int beverageCount = 0;
            int slot = 0;
            bool hasSyrup = false;
            List<int> options;
            int pick, roll;
            var toppings = new List<int> { 4, 5, 11, 12 };
            var beverages = new List<int> { 6, 13 };
            orderList.Clear();
            currentLayer = 0;
            price = 0;
            choosenSet = availableSet[GameRules.RandomRange(0, availableSet.Count - 1)];
            totalPatience = 2;
            customerFill.fillAmount = 1;
            // From level 4 every set gets a beverage (two from level 5). The set itself is changed, as in the original.
            if (level >= 4 && choosenSet.IndexOf(BEVERAGE) == -1)
            {
                roll = GameRules.RandomRange(0, 100);
                if (roll <= 100) choosenSet.Add(BEVERAGE);
                if (level >= 5)
                {
                    roll = GameRules.RandomRange(0, 100);
                    if (roll <= 100) choosenSet.Add(BEVERAGE);
                }
            }
            bool tutorial = InLevel11Tutorial;
            if (tutorial) choosenSet = new List<int> { CUP, YOGURT, SYRUP, TOPPING, BEVERAGE };
            for (int i = 0; i < choosenSet.Count; i++)
            {
                int kind = choosenSet[i];
                if (kind == CUP)
                {
                    options = new List<int> { 0, 7 };
                    if (level < 5) orderList.Add(0);
                    else orderList.Add(options[GameRules.RandomRange(0, options.Count - 1)]);
                }
                else if (kind == YOGURT)
                {
                    options = new List<int> { 1, 2, 8 };
                    if (tutorial) options = new List<int> { 1 };
                    orderList.Add(options[GameRules.RandomRange(0, options.Count - 1)]);
                }
                else if (kind == SYRUP)
                {
                    options = new List<int> { 3, 9, 10 };
                    if (tutorial) options = new List<int> { 3 };
                    orderList.Add(options[GameRules.RandomRange(0, options.Count - 1)]);
                    hasSyrup = true;
                }
                else if (kind == TOPPING)
                {
                    pick = GameRules.RandomRange(0, toppings.Count - 1);
                    orderList.Add(tutorial ? 4 : toppings[pick]);
                    toppings.RemoveAt(pick);
                    toppingCount++;
                }
                else if (kind == BEVERAGE)
                {
                    pick = GameRules.RandomRange(0, beverages.Count - 1);
                    orderList.Add(tutorial ? 6 : beverages[pick]);
                    beverages.RemoveAt(pick);
                    beverageCount++;
                }
                totalPatience++;
            }
            totalPatience *= 60;
            patience = totalPatience;

            var paperPos = previewPaper.GetStagePosition();
            var paperSize = previewPaper.rect.size;
            int baseX = (int)(paperPos.x + paperSize.x / 2 + 10);
            int baseY = (int)(paperPos.y + paperSize.y / 2 + 10);
            bool cupFirst = itemType[orderList[0]] == CUP;
            if (cupFirst)
            {
                if (beverageCount > 0 && toppingCount > 0) baseX -= 80;
                else if (beverageCount > 0 || toppingCount > 0) baseX -= 40;
            }
            slot = 0;
            for (int i = 0; i < orderList.Count; i++)
            {
                var p = previews[i];
                int order = orderList[i];
                price += itemPrice[order];
                SetSprite(p, ItemSprite(itemName[order] + "_preview"));
                var rt = p.rectTransform;
                rt.KillTweens();
                rt.localScale = Vector3.one;
                p.gameObject.SetActive(true);
                rt.pivot = new Vector2(0.5f, 0.5f);
                float x = baseX;
                float y = paperPos.y + previews[0].rectTransform.rect.height + (order == 7 ? -10 : 10);
                if (!hasSyrup) y += 40;
                if (cupFirst)
                {
                    int kind = itemType[order];
                    if (kind == YOGURT)
                    {
                        y += 55 + (order == 7 ? 10 : 0);
                    }
                    else if (kind == SYRUP)
                    {
                        y += 120 + (order == 7 ? 10 : 0);
                    }
                    else if (kind == TOPPING)
                    {
                        x = baseX + 80;
                        y = baseY + slot * 50 - (float)((toppingCount - 1) * 50) / 2;
                        slot++;
                        if (slot >= toppingCount) slot = 0;
                    }
                    else if (kind == BEVERAGE)
                    {
                        x = toppingCount == 0 ? baseX + 80 : baseX + 160;
                        y = slot * 95 + baseY - (float)((beverageCount - 1) * 95) / 2;
                        slot++;
                    }
                }
                else
                {
                    y = slot * 95 + baseY - (float)((beverageCount - 1) * 95) / 2;
                    slot++;
                }
                rt.SetStagePosition(x, y);
            }
            for (int i = 0; i < orderList.Count; i++)
            {
                int index = i;
                previews[i].rectTransform.localScale = new Vector3(0, 0, 1);
                Tweens.DelayedCall(i * 0.1f, () => PopOrder(index), this);
            }
            orderInPlace = true;
        }

        void PopOrder(int index)
        {
            Sound.PlaySfx("other_ingredient_matched_v3");
            previews[index].rectTransform.TweenScale(1, 0.2f).SetEase(Ease.BackOut);
        }

        void HidePreviews()
        {
            foreach (var p in previews)
                if (p.gameObject.activeSelf) p.gameObject.SetActive(false);
        }

        bool InLevel11Tutorial => Tut != null && Tut.CurrentDialogueId == TutorialIds.LEVEL_11;

        // ------------------------------------------------------------------ building the dessert

        /// <summary>touchIngredient: ingredient station <paramref name="id"/> was pressed.</summary>
        public void TouchIngredient(int id)
        {
            if (!orderInPlace) return;
            var tut = Tut;
            if (tut != null && tut.IsButtonShowing) return;
            if (tut != null && !tut.CanClickThisButton(id)) return;
            tut?.Trigger(TutorialActions.TouchIngredient, TutorialActions.KeyId, id);

            if (id != orderList[currentLayer])
            {
                // wrong ingredient: the dessert goes to the bin
                currentLayer = 0;
                foreach (var item in plateItems) item.gameObject.SetActive(false);
                Sound.PlaySfx("put_beverage_fries_on_tray_v2");
                return;
            }
            Sound.PlaySfx("other_ingredient_matched_v3");
            PlaceIngredient(id);
            currentLayer++;
            if (currentLayer < orderList.Count) return;
            Serve();
        }

        /// <summary>The ingredient drops onto the dessert (squashed 0.9 x 0.5, 40 px higher, Back.easeOut in 0.3 s).</summary>
        void PlaceIngredient(int id)
        {
            var obj = plateItems[id];
            SetSprite(obj, ItemSprite(itemName[id]));
            obj.gameObject.SetActive(true);
            var rt = obj.rectTransform;
            rt.pivot = new Vector2(0.5f, 0.5f);
            // on the pineapple cup everything but the beverages sits 35 px higher
            int lift = currentLayer != 0 && orderList[0] == 7 && orderList[currentLayer] != 6 && orderList[currentLayer] != 13 ? 35 : 0;
            float toY = posY[id] - lift;
            rt.SetStagePosition(posX[id], posY[id] - 40 - lift);
            rt.localScale = new Vector3(0.9f, 0.5f, 1);
            rt.KillTweens();
            float fromY = 0;
            Vector3 fromScale = Vector3.one;
            Tweens.To(rt, 0.3f, p =>
            {
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, Mathf.LerpUnclamped(fromY, -toY, p));
                rt.localScale = Vector3.LerpUnclamped(fromScale, Vector3.one, p);
            }, () =>
            {
                fromY = rt.anchoredPosition.y;
                fromScale = rt.localScale;
            }).SetEase(Ease.BackOut);
            if (id != 6 && id != 13)
            {
                // on top of everything but the windows (beverages keep their place behind the dessert)
                rt.SetParent(plateTop, false);
                rt.SetAsLastSibling();
            }
        }

        /// <summary>The last ingredient was right: paid, goal / expert / next extras level, the customer leaves.</summary>
        void Serve()
        {
            Sound.PlaySfx("money_collected_v2");
            froyoMadeCount++;
            earning += price;
            scoreText.text = "$ " + earning;
            if (earning >= goalTarget)
            {
                if (!Extras)
                {
                    if (goalState == 1)
                    {
                        profile.goalAchieved[Session.currentPlayedLevel] = true;
                        goalValueText.text = "$ " + expertTarget;
                        goalLabelText.text = Loc.T("EXPERT:");
                        GameWindows.ShowGoalIconSmall(GoalIconPlace.Dessert, earning >= expertTarget);
                        Sound.PlaySfx("goal_reached");
                        goalState++;
                    }
                    if (goalState == 2 && earning >= expertTarget)
                    {
                        GameWindows.ShowGoalIconSmall(GoalIconPlace.Dessert, true);
                        Sound.PlaySfx("expert_reached");
                        goalState = 3;
                    }
                }
                else
                {
                    level++;
                    if (level <= ExtrasLastLevel)
                    {
                        ShowCurrentLevel();
                        var d = Db.levels.dessertLevels[level - 1];
                        time = (int)(d.time * FramesPerSecond);
                        totalTime = time;
                        goalTarget = d.goal;
                        goalValueText.text = "$ " + goalTarget;
                        earning = 0;
                        scoreText.text = "$ 0";
                        SetLevelSet();
                    }
                    else
                    {
                        timesUp = true;
                        ShowResult(() => GameWindows.ShowDessertExtrasComplete(Result()));
                    }
                }
            }
            orderInPlace = false;
            Tweens.DelayedCall(0.4f, TweenOut, this);
            HidePreviews();

            // FROYO_MASTER (story only): unlocked past the target (">", as in the original), progress every 50
            if (!Extras && !profile.unlocked[AchievementList.FroyoMaster])
            {
                int id = AchievementList.FroyoMaster;
                profile.achievementCounter[id]++;
                if (profile.achievementCounter[id] > AchievementList.Target[id])
                {
                    profile.unlocked[id] = true;
                    Notifier?.ShowUnlocked(id);
                }
                else if (profile.achievementCounter[id] % 50 == 0)
                {
                    Notifier?.ShowProgress(id);
                }
            }
        }

        // ------------------------------------------------------------------ customer in / out

        /// <summary>tweenOut: customer, bar, pan and dessert slide out; the next customer follows unless time is (nearly) up.</summary>
        void TweenOut()
        {
            timeline.Restart();
            timeline.To(pan, pan.anchoredPosition.x - 1000, 0.8f, Ease.ExpoIn, 0);
            timeline.To(customerBarBG, OutX, 0.8f, Ease.ExpoIn, 0);
            timeline.To(heart, OutX, 0.8f, Ease.ExpoIn, 0);
            timeline.To(customerFill.rectTransform, OutX, 0.8f, Ease.ExpoIn, 0);
            timeline.To(customerGlass, OutX, 0.8f, Ease.ExpoIn, 0);
            foreach (var item in plateItems)
            {
                if (!item.gameObject.activeSelf) continue;
                var rt = item.rectTransform;
                timeline.To(rt, rt.anchoredPosition.x - 1000, 0.8f, Ease.ExpoIn, 0);
            }
            timeline.To(customer.rectTransform, OutX, 1, Ease.ExpoIn, 0, NextCustomer);
            if (!timesUp && time > 30) TweenIn();
        }

        /// <summary>The customer is off screen: a random one (char_2 at the end of the LEVEL_11 tutorial) takes his place.</summary>
        void NextCustomer()
        {
            int charIdx = GameRules.RandomRange(1, 12);
            var tut = Tut;
            if (tut != null && tut.CurrentDialogueId == TutorialIds.LEVEL_11 && tut.DialogueCounter > 10) charIdx = 2;
            SetCustomerSprite(ItemSprite("char_" + charIdx));
            customerFill.fillAmount = 1;
        }

        void TweenIn()
        {
            timeline.To(pan, PanInX, 0.8f, Ease.ExpoOut, 1);
            timeline.To(customerBarBG, BarInX, 1, Ease.ExpoOut, 1);
            timeline.To(heart, HeartInX, 1, Ease.ExpoOut, 1);
            timeline.To(customerFill.rectTransform, FillInX, 1, Ease.ExpoOut, 1);
            timeline.To(customerGlass, FillInX, 1, Ease.ExpoOut, 1);
            timeline.To(customer.rectTransform, CustomerInX, 1, Ease.ExpoOut, 1);
        }

        /// <summary>checkTimesUp (end of the timeline): the next order, or the day's results.</summary>
        void CheckTimesUp()
        {
            Tut?.Trigger(TutorialActions.CheckTimesUp);
            if (!timesUp)
            {
                SetOrder();
            }
            else if (!Extras)
            {
                StoryResults();
            }
            else if (level < ExtrasLastLevel + 1)
            {
                ShowResult(() => GameWindows.ShowDessertExtrasFailed(Result()));
            }
            timeline.Clear();
        }

        /// <summary>Story day over: rewards, day earnings, achievements, save, then the summary window.</summary>
        void StoryResults()
        {
            var p = profile;
            int lvl = Session.currentPlayedLevel;
            int earned = 0;
            int delayedNotifs = 0;
            if (p.daysEarning[lvl] == 0 && lvl == p.passedDays)
            {
                // first play of the next story day
                if (earning >= goalTarget)
                {
                    var q = AchievementList.Add(p, AchievementList.QualifiedChef, 1, 15);
                    Notifier?.Show(AchievementList.QualifiedChef, q);
                    if (q != AchievementList.Result.None) delayedNotifs++;
                    if (earning >= expertTarget)
                    {
                        BumpInternationalChef(delayedNotifs);
                        p.money += 6;
                        Session.coinsGot = 6;
                    }
                    else
                    {
                        p.money += 3;
                        Session.coinsGot = 3;
                    }
                    p.daysEarning[lvl] = earning;
                    p.passedDays++;
                    earned = earning;
                    Game.Saves.Save();
                    ShowResult(() => GameWindows.ShowDessertLevelComplete(Result()));
                }
                else
                {
                    ShowResult(() => GameWindows.ShowDessertLevelFailed(Result()));
                }
            }
            else if (earning >= goalTarget)
            {
                // replay: only a better score counts (expert reached for the first time: 3 more coins)
                if (earning > p.daysEarning[lvl])
                {
                    if (earning >= expertTarget && p.daysEarning[lvl] < expertTarget)
                    {
                        BumpInternationalChef(delayedNotifs);
                        p.money += 3;
                        Session.coinsGot = 3;
                    }
                    Session.daysEarningPrev = p.daysEarning[lvl];
                    earned = earning - p.daysEarning[lvl];
                    p.daysEarning[lvl] = earning;
                    Game.Saves.Save();
                }
                ShowResult(() => GameWindows.ShowDessertLevelComplete(Result()));
            }
            else
            {
                ShowResult(() => GameWindows.ShowDessertLevelFailed(Result()));
            }
            var bb = AchievementList.Add(p, AchievementList.BigBusiness, earned, 20000);
            Notifier?.Show(AchievementList.BigBusiness, bb);
            Game.Saves.Save();
        }

        /// <summary>INTERNATIONAL_CHEF +1 (progress every 15). The original delayed the banner by 6 s per earlier banner;
        /// the notifier queues banners 6 s apart.</summary>
        void BumpInternationalChef(int delayedNotifs)
        {
            var r = AchievementList.Add(profile, AchievementList.InternationalChef, 1, 15);
            Notifier?.Show(AchievementList.InternationalChef, r);
        }

        DessertResult Result() => new DessertResult
        {
            froyoServed = froyoMadeCount,
            earning = earning,
            expertTarget = expertTarget,
            coinsGot = Session.coinsGot,
        };

        /// <summary>Opens a summary once: the shared window of the original could not be shown twice (extras: the last
        /// life and the end of the timeline both asked for DESSERT_LEVEL_FAILED_EXTRAS).</summary>
        void ShowResult(System.Action open)
        {
            if (resultShown) return;
            resultShown = true;
            open();
        }

        // ------------------------------------------------------------------ sprites

        static Sprite ItemSprite(string name)
        {
            var s = GameDatabase.Instance.sprites.Get(Atlas, name);
            if (s == null) Debug.LogWarning($"[Rory] Dessert: no sprite {Atlas}/{name}");
            return s;
        }

        /// <summary>texture = ...; readjustSize(): native size of the new sprite.</summary>
        static void SetSprite(Image img, Sprite s)
        {
            img.sprite = s;
            if (s != null) img.rectTransform.sizeDelta = s.rect.size;
        }

        void SetCustomerSprite(Sprite s)
        {
            SetSprite(customer, s);
            customer.rectTransform.SetPivotPixels(0, customerPivotY);
        }

        // ------------------------------------------------------------------ ITutorialHost

        bool ITutorialHost.IsKitchen => false;
        string ITutorialHost.JuicePercentageText => null;

        bool ITutorialHost.TryGetTilePosition(int col, int row, out Vector2 position)
        {
            position = default;
            return false;
        }

        bool ITutorialHost.TryGetFirstListedItem(out Vector2 position, out float height)
        {
            position = default;
            height = 0;
            return false;
        }

        void ITutorialHost.OnTutorialCommand(TutorialCommand command)
        {
            // LEVEL_11 line 4 / LEVEL_25 line 0 "clicked_ok": _minigameDessert.startLevel()
            if (command.Type == TutorialCommandType.StartDessertLevel) StartLevel();
        }
    }
}
