using System.Collections.Generic;
using Rory.Achievements;
using AchievementList = Rory.Achievements.Achievements;
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

namespace Rory.Screens.Match3
{
    /// <summary>
    /// The match-3 restaurant (GameScreen, story days of type M3 and M3S): three customers on the left order dishes,
    /// the 8x8 board on the right provides the ingredients, served customers leave coins to collect.
    /// <para>Structure: <see cref="Match3Board"/> (board model) drawn by <see cref="BoardView"/>; three
    /// <see cref="Match3Customer"/> rows; <see cref="HardwareStation"/> juicer / fryer; <see cref="ScoreCounter"/>;
    /// <see cref="DishPlanner"/> (today's dishes); shared windows through <see cref="GameWindows"/>.</para>
    /// <para>Timing: the original ran its Update at 30 fps with per-frame counters (timer, patience, gravity, score
    /// roll-up, idle hint). <see cref="Tick"/> is exactly that frame, run by a fixed 30 Hz accumulator, so every
    /// counter keeps its original duration at any frame rate. TweenLite tweens are time based (board tweens:
    /// <see cref="BoardTweens"/>, the rest: Rory.Tweening) and keep running while paused, like the original.</para>
    /// </summary>
    public class Match3Controller : ScreenController, IMatch3BoardHost, ITutorialHost
    {
        public const float FrameTime = 1f / 30f;
        const int CustomerCount = 3;

        [Header("HUD")]
        [SerializeField] Button pauseButton;
        [SerializeField] Button soundButton;
        [SerializeField] GameObject muteIcon;
        [SerializeField] Button musicButton;
        [SerializeField] GameObject muteIconMusic;
        [SerializeField] Image timerFill;
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] TextMeshProUGUI targetInfoText;
        [SerializeField] TextMeshProUGUI targetStateText;

        [Header("Customers (slot 0..2)")]
        [SerializeField] Match3Customer[] customers = new Match3Customer[CustomerCount];
        [SerializeField] Image[] dishes = new Image[CustomerCount];
        [SerializeField] CoinButton[] coins = new CoinButton[CustomerCount];
        [SerializeField] TextMeshProUGUI[] earnedMoneyTexts = new TextMeshProUGUI[CustomerCount];
        [SerializeField] TextMeshProUGUI[] chainBonusTexts = new TextMeshProUGUI[CustomerCount];

        [Header("Hardware")]
        [SerializeField] HardwareStation juicer;
        [SerializeField] HardwareStation fryer;

        [Header("Board")]
        [SerializeField] BoardView boardView;
        [SerializeField] BoardInput boardInput;
        [SerializeField] FlyingTiles flyingTiles;

        [Header("Texts and overlays")]
        [SerializeField] TextMeshProUGUI largeText;
        [SerializeField] Image blackScreen;
        [SerializeField] TextMeshProUGUI noPossibleMoveText;
        [Tooltip("Dragged juice / fries are moved just below this marker (on top of the screen content)")]
        [SerializeField] RectTransform dragTop;

        // ------------------------------------------------------------------ state (GameScreen fields)
        public bool hasWindow;
        public bool paused;
        public bool timesUp;
        public int timer;
        public int totalTime;
        public int goalTarget;
        public int expertTarget;
        public int servedCustomerCount;
        public int missedCustomerCount;
        public int VIPServeBonus;
        public int chainCombo;
        /// <summary>goalExpertText.idx: 1 = playing for the goal, 2 = for the expert target, 3 = expert reached.</summary>
        public int goalState = 1;
        public bool calledShowWindow;
        public bool firstCust;
        public bool choosenJuice;
        public bool choosenFries;
        public int cookiesEffect;
        public int cookiesTarget;
        int escCounter;
        float tickTime;

        public bool Ready { get; private set; }
        public bool TimesUp => timesUp;
        public Match3Rng Rng { get; private set; }
        public Match3Board Board { get; private set; }
        public DishPlanner Planner { get; private set; }
        public ScoreCounter Score { get; private set; }
        public new GameSession Session => ScreenController.Session;
        public new PlayerProfile Player { get; private set; }
        public IReadOnlyList<Match3Customer> Customers => customers;
        public IReadOnlyList<CoinButton> Coins => coins;
        public IReadOnlyList<Image> Dishes => dishes;
        public HardwareStation Juicer => juicer;
        public HardwareStation Fryer => fryer;
        public BoardView BoardView => boardView;
        public FlyingTiles FlyingTiles => flyingTiles;
        public TextMeshProUGUI LargeText => largeText;
        public TextMeshProUGUI TargetInfoText => targetInfoText;
        public TextMeshProUGUI TargetStateText => targetStateText;
        public TextMeshProUGUI ScoreText => scoreText;
        public Image TimerFill => timerFill;
        /// <summary>gameScreen.customerOnQueue: avatar ids of the customers in the restaurant.</summary>
        public readonly List<int> CustomerOnQueue = new List<int>();
        /// <summary>The day being played (Session.currentPlayedLevel, 0-based).</summary>
        public int Day { get; private set; }

        static TutorialManager Tut => TutorialManager.Current;

        public static Match3Controller Current { get; private set; }

        void OnEnable() => Current = this;

        void OnDisable()
        {
            if (Current == this) Current = null;
        }

        // ================================================================== initialize()

        void Start()
        {
            Player = GameManager.Player ?? new PlayerProfile();
            Rng = new Match3Rng(Match3Rng.SeedOverride);
            Match3Rng.SeedOverride = null;
            if (Tut != null) Tut.Host = this;

            Day = Session.currentPlayedLevel;
            var day = Db.levels.days[Mathf.Clamp(Day, 0, Db.levels.days.Count - 1)];
            timesUp = false;
            if ((Day + 1) % GameRules.Match3SpecialEvery == 0) Session.isPlayingSpecialLevel = true;
            if (!Session.isPlayingSpecialLevel)
            {
                if (!Sound.IsPlayingMusic("fun-time", "sunny-advert")) Sound.PlayMusic(Rng.Range(1, 2) == 1 ? "fun-time" : "sunny-advert");
            }
            else
            {
                Sound.PlayMusic("upbeat-feeling_loop");
            }
            Session.customerFocus = 0;
            Planner = new DishPlanner(Db.dishes, Player, Session);
            Planner.CheckDishPriceAndAvailability(4);
            var availableTypes = Planner.AvailableTypes();
            if (Player.passedDays >= GameRules.UnlockCookies && Player.hardwareLevel[GameRules.Cookies] == 0)
                Player.hardwareLevel[GameRules.Cookies] = 1;
            RefillCustomerPool();
            CustomerOnQueue.Clear();
            Session.totalCust = 12;
            Session.currentCust = 0;
            Session.vipServedRating = 0;
            Session.juiceServed = 0;
            Session.friesServed = 0;
            Session.cookiesUsed = 0;
            Score = new ScoreCounter(scoreText);
            servedCustomerCount = 0;
            missedCustomerCount = 0;
            VIPServeBonus = 0;

            Board = new Match3Board(this, Rng)
            {
                AvailableTypes = availableTypes,
                PlayedDay = Day,
                maxFrozenTile = Match3Board.MaxFrozenFor(Day),
                currentFrozenTile = 0,
                torchTarget = GameRules.TileNeededForTorch,
            };
            cookiesTarget = Planner.HardwareTarget(GameRules.Cookies);
            juicer.target = Planner.HardwareTarget(GameRules.Juicer);
            fryer.target = Planner.HardwareTarget(GameRules.Fryer);
            cookiesEffect = Planner.HardwareEffect(GameRules.Cookies);
            juicer.removed = 0;
            fryer.removed = 0;

            timer = (int)(day.time * 30.0);
            totalTime = timer;
            goalTarget = day.goal;
            expertTarget = day.expert;
            Board.gameover = false;
            Board.levelComplete = false;

            for (int i = 0; i < CustomerCount; i++)
            {
                dishes[i].gameObject.SetActive(false);
                customers[i].Init(this, i);
                int slot = i;
                coins[i].transform.localScale = Vector3.zero;
                coins[i].gameObject.SetActive(false);
                coins[i].Pressed += () => TouchCoins(slot);
                earnedMoneyTexts[i].gameObject.SetActive(false);
                chainBonusTexts[i].gameObject.SetActive(false);
            }
            targetInfoText.text = "0/" + goalTarget;
            targetStateText.text = Loc.T("Goal:");
            if (Session.isPlayingSpecialLevel)
            {
                targetStateText.text = "";
                targetInfoText.text = Loc.T("0 / 1 food critic");
            }
            scoreText.text = "$ 0";

            foreach (var hw in new[] { juicer, fryer })
            {
                hw.Touched += OnHardwareTouched;
                hw.Percentage.text = "0%";
                hw.Product.gameObject.SetActive(false);
                hw.DragCopy.gameObject.SetActive(false);
                hw.DragCopy.rectTransform.SetStagePosition(hw.Home.x, hw.Home.y);
            }
            if (Player.hardwareLevel[GameRules.Juicer] == 0) juicer.HideAll();
            if (Player.hardwareLevel[GameRules.Fryer] == 0) fryer.HideAll();

            boardView.Bind(Board);
            boardInput.Touched += (phase, pos) => Board.Touch(phase, pos.x, pos.y);
            Board.Generate();
            boardView.Sync();

            goalState = 1;
            if (Player.goalAchieved[Day])
            {
                targetStateText.text = Loc.T("Expert:");
                targetInfoText.text = "$" + expertTarget;
                goalState = 2;
            }
            largeText.gameObject.SetActive(false);
            blackScreen.gameObject.SetActive(false);
            noPossibleMoveText.gameObject.SetActive(false);

            pauseButton.onClick.AddListener(OnPauseButton);
            soundButton.onClick.AddListener(() =>
            {
                Sound.MuteSound = !Sound.MuteSound;
                muteIcon.SetActive(!muteIcon.activeSelf);
            });
            musicButton.onClick.AddListener(() =>
            {
                Sound.MuteMusic = !Sound.MuteMusic;
                muteIconMusic.SetActive(!muteIconMusic.activeSelf);
            });
            RefreshMuteIcons();

            var events = Day >= 0 && Day < Db.levelEvents.days.Count ? Db.levelEvents.days[Day] : null;
            int windowCount = events != null ? events.windows.Count : 0;
            Session.currentNotifWindow = Player.passedDays == Day ? 0 : windowCount - 1;

            CheckTutorials();

            GameWindows.LastDayInfo = Loc.F("Goal   : {0}\nExpert : {1}", goalTarget, expertTarget);
            if (windowCount > 0 && (Tut == null || Tut.CurrentDialogueId < 0 || events.alwaysShow))
            {
                GameWindows.ShowBeginningWindows(Day, new BeginningWindowOptions
                {
                    startIndex = Session.currentNotifWindow,
                    goalTarget = goalTarget,
                    expertTarget = expertTarget,
                    todaysDishes = Planner.RegionDishes,
                    onAllClosed = () =>
                    {
                        paused = false;
                        if (Tut == null || !Tut.IsButtonShowing) StartLevel();
                    },
                });
            }
            else if (windowCount == 0)
            {
                StartLevel();
            }
            escCounter = 0;
            firstCust = true;
            calledShowWindow = false;
            hasWindow = true;
            paused = false;
            Ready = true;
        }

        /// <summary>The checkTut chain of initialize(): only the first tutorial that applies is shown.</summary>
        void CheckTutorials()
        {
            var t = Tut;
            if (t == null) return;
            if (t.CheckTut(TutorialIds.LEVEL_1, 0)) { }
            else if (t.CheckTut(TutorialIds.LEVEL_5, 4)) { }
            else if (t.CheckTut(TutorialIds.LEVEL_7, 6)) { }
            else if (t.CheckTut(TutorialIds.LEVEL_8, 7)) { }
            else if (t.CheckTut(TutorialIds.LEVEL_12, 11))
            {
                juicer.removed = juicer.target - 3;
                juicer.ShowPercentage();
            }
            else if (t.CheckTut(TutorialIds.LEVEL_13, 12)) { }
            else if (t.CheckTut(TutorialIds.LEVEL_14, 13)) { }
            else if (t.CheckTut(TutorialIds.LEVEL_16, 15)) { }
            else if (t.CheckTut(TutorialIds.LEVEL_17, 16))
            {
                fryer.removed = fryer.target - 3;
                fryer.ShowPercentage();
            }
            else if (t.CheckTut(TutorialIds.LEVEL_19, 18)) { }
            else if (t.CheckTut(TutorialIds.LEVEL_21, 20)) { }
            else if (t.CheckTut(TutorialIds.LEVEL_22, 21)) { }
            else if (t.CheckTut(TutorialIds.LEVEL_26, 25))
            {
                Board.dispatchHighlightPosition = 3;
                Board.HighlightPossible();
                Board.highlightPossibleTile = true;
                Board.ResetHintScale();
            }
            else if (t.CheckTut(TutorialIds.LEVEL_27, 26))
            {
                Board.FreezeTile(0, 0);
                Board.SetType(1, 1, TileTypes.Torch);
            }
            else if (t.CheckTut(TutorialIds.LEVEL_31, 30))
            {
                Board.SetType(1, 1, TileTypes.Cookie);
            }
        }

        void OnDestroy()
        {
            if (Tut != null && ReferenceEquals(Tut.Host, this)) Tut.Host = null;
            foreach (var t in earnedMoneyTexts) if (t != null) t.rectTransform.KillTweens();
            foreach (var t in chainBonusTexts) if (t != null) t.rectTransform.KillTweens();
            foreach (var c in coins) if (c != null) c.transform.KillTweens();
            foreach (var d in dishes) if (d != null) d.KillTweens();
            if (largeText != null) largeText.KillTweens();
            if (noPossibleMoveText != null) noPossibleMoveText.rectTransform.KillTweens();
            this.KillTweens();
            Board?.Tweens.KillAll();
        }

        // ================================================================== Update

        void Update()
        {
            Board?.StepTweens(Time.deltaTime);
            if (!Ready) return;
            CheckCheatKeys();
            tickTime += Time.deltaTime;
            int frames = 0;
            while (tickTime >= FrameTime)
            {
                tickTime -= FrameTime;
                Tick();
                // after a long hitch the original simply ran slower: drop what is more than a few frames behind
                if (++frames >= 8)
                {
                    tickTime = 0;
                    break;
                }
            }
        }

        /// <summary>One frame of the original's 30 fps Update.</summary>
        public void Tick()
        {
            if (!Ready || hasWindow) return;
            Score.Tick();
            if (paused || Board.gameover) return;
            if (Tut != null && Tut.IsButtonShowing) return;
            Board.Tick();
            for (int c = 0; c < CustomerCount; c++)
            {
                var cust = customers[c];
                if (!cust.onTween && cust.X > -10)
                {
                    cust.DePatience();
                    if (cust.patienceClipX > 0)
                    {
                        cust.patienceClipX = 0;
                        cust.RefreshPatienceBar();
                    }
                }
            }
            if (!timesUp && (Tut == null || !Tut.IsShowing))
            {
                timer--;
                if (timer <= 0) timesUp = true;
                timerFill.fillAmount = Mathf.Max(0, (float)timer / totalTime);
            }
            Board.TickIdle();
        }

        /// <summary>KeyDown: the developer cheat (player "OrIgAmInglOl" with the title-screen cheat active).</summary>
        void CheckCheatKeys()
        {
            if (!Session.cheatActive || Player.name != "OrIgAmInglOl" || !Input.anyKeyDown) return;
            Player.money = 50000; // "faaaa"
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            escCounter++;
            if (escCounter > 2)
            {
                servedCustomerCount = goalTarget;
                Score.SetTotal(expertTarget);
                Player.goalAchieved[Day] = true;
                ShowWindow();
            }
        }

        void OnApplicationFocus(bool focus)
        {
            // Game.onDeactivateHandler -> gameScreen.pauseGame(): the pause window opens when the game loses focus
            if (focus || !Ready || Application.isBatchMode) return;
            if (PopupManager.Current != null && PopupManager.Current.HasOpenPopup) return;
            if (calledShowWindow) return;
            OpenPause();
        }

        // ================================================================== level flow

        /// <summary>startLevel(n): the board starts and the first <paramref name="count"/> customers walk in.</summary>
        public void StartLevel(int count = 3)
        {
            hasWindow = false;
            for (int i = 0; i < count && i < CustomerCount; i++) customers[i].TweenOut(Match3Customer.Satisfied);
        }

        void OnPauseButton()
        {
            if (calledShowWindow) return;
            OpenPause();
        }

        void OpenPause()
        {
            paused = true;
            GameWindows.ShowPause(new PauseOptions
            {
                onResume = () => paused = false,
                onMuteChanged = RefreshMuteIcons,
                resetGoalOnExit = true,
            });
        }

        void RefreshMuteIcons()
        {
            muteIcon.SetActive(Sound.MuteSound);
            muteIconMusic.SetActive(Sound.MuteMusic);
        }

        public void RefillCustomerPool()
        {
            Session.tempCustUse.Clear();
            var all = GameWindows.CustomerInUse;
            for (int i = 0; i < Player.lastCustIdx + 1 && i < all.Count; i++) Session.tempCustUse.Add(all[i]);
        }

        // ================================================================== customers

        /// <summary>checkCustomerLeft: when time is up and all three customers are gone, the day ends.</summary>
        public bool CheckCustomerLeft()
        {
            int left = 0;
            for (int c = 0; c < CustomerCount; c++)
                if (customers[c].completed && !coins[c].gameObject.activeSelf) left++;
            if (timesUp && left >= 3)
            {
                ShowWindow();
                return true;
            }
            return false;
        }

        /// <summary>setNextPriorCust: the focus moves to the lowest queue number still waiting.</summary>
        public void SetNextPriorCust()
        {
            for (int c = 0; c < CustomerCount; c++)
                if (customers[c].queueIdx == Session.customerFocus && !customers[c].completed) return;
            var waiting = new List<int>();
            for (int c = 0; c < CustomerCount; c++)
                if (!coins[c].gameObject.activeSelf) waiting.Add(customers[c].queueIdx);
            if (firstCust) firstCust = false;
            if (waiting.Count <= 0)
            {
                for (int c = 0; c < CustomerCount; c++) customers[c].HidePriority();
                return;
            }
            waiting.Sort();
            Session.customerFocus = waiting[0];
            for (int c = 0; c < CustomerCount; c++)
            {
                if (customers[c].queueIdx == Session.customerFocus)
                {
                    if (!customers[c].PriorityVisible) customers[c].ShowPriority();
                }
                else
                {
                    customers[c].HidePriority();
                }
            }
        }

        /// <summary>tweenDishAlpha: the served (or angry) customer leaves; the tray fades out.</summary>
        public void TweenDishAlpha(int custID, int custState)
        {
            var cust = customers[custID];
            cust.onTween = true;
            coins[custID].gameObject.SetActive(false);
            if (cust.asVIP)
            {
                if (custState == Match3Customer.Satisfied)
                {
                    int patienceLeft = (int)((double)cust.patience / cust.totalPatience * 100);
                    Session.vipServedRating = patienceLeft > 75 ? 1.5f : 1.25f;
                }
                else if (custState == Match3Customer.Leave)
                {
                    Session.vipServedRating = 0;
                }
            }
            cust.Fries.KillTweens();
            cust.Juice.KillTweens();
            cust.Fries.TweenAlpha(0, 0.3f);
            cust.Juice.TweenAlpha(0, 0.3f);
            var dish = dishes[custID];
            dish.KillTweens();
            dish.TweenAlpha(0, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                dish.gameObject.SetActive(false);
                SetAlpha(dish, 1);
                cust.Fries.gameObject.SetActive(false);
                cust.Juice.gameObject.SetActive(false);
                SetAlpha(cust.Fries, 1);
                SetAlpha(cust.Juice, 1);
            });
            int onQueueRemove = CustomerOnQueue.IndexOf(cust.customerAvaID);
            // AS3 splice(-1, 1) removes the last element
            if (onQueueRemove < 0) onQueueRemove = CustomerOnQueue.Count - 1;
            if (onQueueRemove >= 0) CustomerOnQueue.RemoveAt(onQueueRemove);
            cust.TweenOut(custState, Tut == null || !Tut.BlockCustomerIn);
        }

        static void SetAlpha(Graphic g, float a)
        {
            var c = g.color;
            c.a = a;
            g.color = c;
        }

        /// <summary>touchCoins: collects the money of a served customer (or, with juice / fries in hand, selects it).</summary>
        public void TouchCoins(int targetID)
        {
            var cust = customers[targetID];
            if ((choosenJuice && !cust.hasJuice) || (choosenFries && !cust.hasFries))
            {
                cust.SelectPriorCust();
                return;
            }
            PlaySfx("money_collected" + (Rng.Range(1, 2) == 1 ? "" : "_v2"));
            Trigger(TutorialActions.ClickedCoin);
            // (the original also played "serve", a sound that is not in the game data)
            var earned = earnedMoneyTexts[targetID];
            earned.gameObject.SetActive(true);
            int price = cust.money + cust.tipsBonus;
            earned.text = "$" + price;
            int juiceBonus = 0, friesBonus = 0;
            if (cust.hasJuice)
            {
                Session.juiceServed++;
                juiceBonus = Planner.HardwareEffect(GameRules.Juicer);
                earned.text = Loc.F("${0}\n+ ${1} Juice Bonus!", price, juiceBonus);
            }
            if (cust.hasFries)
            {
                Session.friesServed++;
                friesBonus = Planner.HardwareEffect(GameRules.Fryer);
                earned.text = Loc.F("${0}\n+ ${1} Fries Bonus!", price, friesBonus);
            }
            if (cust.hasJuice && cust.hasFries)
                earned.text = Loc.F("${0}\n+ ${1} Juice + Fries Bonus!", price, friesBonus + juiceBonus);
            int chainBonus = (int)(chainCombo * 0.2 * cust.money);
            chainCombo++;
            var coin = coins[targetID].transform;
            coin.KillTweens();
            coin.TweenScale(0, 0.4f).SetEase(Ease.BackIn);
            Score.Add(price + juiceBonus + friesBonus + chainBonus);
            if (Player.goalAchieved[Day] && Score.Total >= expertTarget && goalState == 2)
            {
                goalState = 3;
                ShowGoalIcon();
                PlaySfx("expert_reached");
            }
            var coinPos = ((RectTransform)coin).GetStagePosition();
            if (chainBonus > 0)
            {
                var chain = chainBonusTexts[targetID];
                chain.text = Loc.F("Chain Bonus ${0}", chainBonus);
                chain.gameObject.SetActive(true);
                FloatUp(chain, 0.8f, coinPos);
            }
            FloatUp(earned, 1, coinPos);
            servedCustomerCount++;
            if (!Session.isPlayingSpecialLevel)
            {
                if (goalState == 1) targetInfoText.text = servedCustomerCount + "/" + goalTarget;
                if (servedCustomerCount == goalTarget && goalState == 1) GoalReached();
            }
            else if (cust.asVIP && goalState == 1)
            {
                GoalReached();
            }
            TweenDishAlpha(targetID, Match3Customer.Satisfied);
        }

        /// <summary>The money text rises 65 px, then jumps back to the coin and hides.</summary>
        static void FloatUp(TextMeshProUGUI text, float duration, Vector2 home)
        {
            var rt = text.rectTransform;
            rt.KillTweens();
            var p = rt.GetStagePosition();
            rt.TweenAnchoredPosition(StageLayout.ToAnchored(p.x, p.y - 65), duration).OnComplete(() =>
            {
                rt.SetStagePosition(home.x, home.y);
                text.gameObject.SetActive(false);
            });
        }

        void GoalReached()
        {
            ShowGoalIcon();
            goalState = 2;
            targetInfoText.text = "$" + expertTarget;
            targetStateText.text = Loc.T("Expert:");
            PlaySfx("goal_reached");
            Player.goalAchieved[Day] = true;
        }

        /// <summary>showGoalIconSmall: the badge shows "expert" once the earnings reach the expert target.</summary>
        void ShowGoalIcon() => GameWindows.ShowGoalIconSmall(GoalIconPlace.Match3, Score.Total >= expertTarget);

        // ================================================================== orders

        void CheckCustOrderPrior(int col, int row)
        {
            int focus = Session.customerFocus;
            var tile = Board.Tiles[col][row];
            for (int i = 0; i < CustomerCount; i++)
            {
                var c = customers[i];
                if (c.onTween || c.completed || c.queueIdx != focus) continue;
                for (int j = 0; j < c.ingredientType.Count; j++)
                    if (tile.type == c.ingredientType[j] && !tile.used && c.ingredientTarget[j] > 0) TakeIngredient(col, row, i, j);
            }
        }

        void CheckCustOrder(int col, int row)
        {
            var tile = Board.Tiles[col][row];
            if (tile.used) return;
            var queue = new List<int>();
            for (int i = 0; i < CustomerCount; i++) queue.Add(customers[i].queueIdx);
            queue.Sort();
            for (int k = 0; k < CustomerCount; k++)
            {
                for (int i = 0; i < CustomerCount; i++)
                {
                    var c = customers[i];
                    if (c.queueIdx != queue[k]) continue;
                    if (c.onTween || c.completed) continue;
                    for (int j = 0; j < c.ingredientType.Count; j++)
                        if (tile.type == c.ingredientType[j] && !tile.used && c.ingredientTarget[j] > 0) TakeIngredient(col, row, i, j);
                }
            }
        }

        /// <summary>The tile flies to the customer's order; the counter goes down and pulses.</summary>
        void TakeIngredient(int col, int row, int i, int j)
        {
            StartTweenFromBoard(col, row, i, j);
            Board.Tiles[col][row].used = true;
            var c = customers[i];
            int left = c.ingredientTarget[j] - 1;
            c.ingredientTarget[j] = left;
            if (j < Match3Customer.MaxIngredients)
            {
                c.IngredientTexts[j].text = "x" + left;
                c.PulseCounter(j);
            }
        }

        void StartTweenFromBoard(int col, int row, int custIdx, int ingIdx)
        {
            int f = flyingTiles.FirstFree();
            if (f < 0) return;
            var tile = Board.Tiles[col][row];
            var to = customers[custIdx].IngredientStagePosition(ingIdx);
            flyingTiles.Fly(f, TileView.SpriteFor(tile.type), tile.x, tile.y, to.x, to.y, 0.5f + col * 0.05f, () => OnIngredientArrived(custIdx));
        }

        void OnIngredientArrived(int custIdx)
        {
            var cust = customers[custIdx];
            int countFulfilled = 0;
            if (!cust.onTween && !cust.completed)
            {
                for (int k = 0; k < cust.ingredientType.Count; k++)
                {
                    if (cust.ingredientTarget[k] > 0) continue;
                    countFulfilled++;
                    if (k < Match3Customer.MaxIngredients) cust.ShowFulfilled(k);
                }
            }
            if (countFulfilled != cust.ingredientType.Count || cust.onTween || cust.completed) return;
            PlaySfx("money_drop_v" + Rng.Range(1, 2));
            cust.ShowReady();
            var coin = coins[custIdx];
            coin.transform.KillTweens();
            coin.transform.TweenScale(1, 0.4f).SetEase(Ease.BackOut);
            coin.gameObject.SetActive(true);
            var dish = dishes[custIdx];
            dish.gameObject.SetActive(true);
            dish.sprite = GameDatabase.Instance.sprites.Get("game", DishPlanner.DishSprite(cust.currentDish));
            dish.SetNativeSize();
            dish.rectTransform.pivot = new Vector2(0.5f, 0); // alignPivot("center", "bottom")
            cust.SetDishCompleteState();
            CheckCustomerLeft();
            SetNextPriorCust();
        }

        /// <summary>getWantedIngredient: a random ingredient the focused customer still needs (0 = none).</summary>
        public int GetWantedIngredient()
        {
            var wanted = new List<int>();
            int focus = Session.customerFocus;
            for (int c = 0; c < CustomerCount; c++)
            {
                var cust = customers[c];
                if (cust.onTween || cust.completed || cust.queueIdx != focus) continue;
                for (int k = 0; k < cust.ingredientTarget.Count; k++)
                    if (cust.ingredientTarget[k] > 0) wanted.Add(cust.ingredientType[k]);
                break;
            }
            return wanted.Count > 0 ? wanted[Rng.Range(0, wanted.Count - 1)] : 0;
        }

        // ================================================================== cookies

        /// <summary>giveTheCookies: the cookie flies to the least patient customer and refills its patience.</summary>
        public void GiveCookies(BoardTile tile)
        {
            PlaySfx("cookie" + (Rng.Range(1, 2) == 1 ? "" : "_v2"));
            bool found = false;
            int lowest = 0, lowestIdx = 0;
            for (int i = 0; i < CustomerCount; i++)
            {
                if (customers[i].patienceClipX < lowest && !customers[i].onTween)
                {
                    lowest = (int)customers[i].patienceClipX;
                    lowestIdx = i;
                    found = true;
                }
            }
            if (tile == null || !found) return;
            int f = flyingTiles.FirstFree();
            if (f < 0) return;
            var target = customers[lowestIdx];
            float toX = target.X + 15, toY = target.Y + 25;
            tile.removed = true;
            Board.AffectAbove(tile);
            Board.AddNewPieces();
            Board.cookiesCountOnBoard--;
            int effect = cookiesEffect;
            int idx = lowestIdx;
            flyingTiles.Fly(f, TileView.SpriteFor(tile.type), tile.x, tile.y, toX, toY, 0.5f + tile.col * 0.05f, () =>
            {
                var c = customers[idx];
                double toadd = effect / 100.0 * c.totalPatience;
                if (c.patience + toadd > c.totalPatience) toadd = c.totalPatience - c.patience;
                c.TweenPatienceTo((int)(c.patience + toadd));
                double halfPrice = 0.5 * c.totalMoney;
                double patienceDivided = (c.patience + toadd) / c.totalPatience;
                c.money = c.patience >= 0.8 * c.totalPatience ? (int)(halfPrice * 2) : (int)(halfPrice + halfPrice * patienceDivided);
                c.MoneyDisplay.text = "$" + c.money;
            });
        }

        public void OnCookieUsed() => Session.cookiesUsed++;

        // ================================================================== juicer / fryer

        /// <summary>moveDrink / moveFries.</summary>
        void OnHardwareTouched(HardwareStation hw, HardwareStation.Phase phase, Vector2 pos)
        {
            var copy = hw.DragCopy;
            if (hw.Product.gameObject.activeSelf)
            {
                Trigger(hw.IsJuicer ? TutorialActions.ClickedBeverage : TutorialActions.ClickedFries);
                copy.gameObject.SetActive(true);
            }
            if (phase == HardwareStation.Phase.Moved)
            {
                copy.rectTransform.SetSiblingIndex(dragTop.GetSiblingIndex());
                hw.Product.gameObject.SetActive(false);
                choosenJuice = hw.IsJuicer;
                choosenFries = !hw.IsJuicer;
                copy.rectTransform.SetStagePosition(pos.x, pos.y);
            }
            if (phase != HardwareStation.Phase.Ended) return;
            int i;
            for (i = 0; i < CustomerCount; i++)
            {
                var c = customers[i];
                bool has = hw.IsJuicer ? c.hasJuice : c.hasFries;
                bool chosen = hw.IsJuicer ? choosenJuice : choosenFries;
                if (has || !chosen) continue;
                if (!StageBounds(copy).Overlaps(c.ClickBounds)) continue;
                PlaySfx("put_beverage_fries_on_tray" + (Rng.Range(1, 2) == 1 ? "" : "_v2"));
                Trigger(hw.IsJuicer ? TutorialActions.OfferedBeverage : TutorialActions.OfferedFries);
                if (hw.IsJuicer)
                {
                    choosenJuice = false;
                    c.hasJuice = true;
                    c.Juice.gameObject.SetActive(true);
                }
                else
                {
                    choosenFries = false;
                    c.hasFries = true;
                    c.Fries.gameObject.SetActive(true);
                }
                copy.gameObject.SetActive(false);
                copy.rectTransform.SetStagePosition(hw.Home.x, hw.Home.y);
                Remake(hw);
                break;
            }
            if (i > 2)
            {
                copy.rectTransform.SetStagePosition(hw.Home.x, hw.Home.y);
                // setChildIndex(tmp, getChildIndex(product) - 1): back below the machine's layer
                // (both setChildIndex and SetSiblingIndex give the final index)
                copy.transform.SetSiblingIndex(hw.Product.transform.GetSiblingIndex() - 1);
            }
        }

        /// <summary>Stage bounds of a centre-pivot image.</summary>
        static Rect StageBounds(Image img)
        {
            var p = img.rectTransform.GetStagePosition();
            var size = img.rectTransform.sizeDelta;
            return new Rect(p.x - size.x / 2, p.y - size.y / 2, size.x, size.y);
        }

        /// <summary>remakeJuice / remakeFries: the machine starts filling again.</summary>
        void Remake(HardwareStation hw)
        {
            hw.Product.gameObject.SetActive(false);
            hw.Layer.gameObject.SetActive(true);
            hw.Percentage.text = "0%";
        }

        void HardwareReady(HardwareStation hw)
        {
            hw.removed = 0;
            hw.Percentage.text = Loc.T("Drag to Customer");
            hw.Layer.gameObject.SetActive(false);
            hw.Product.gameObject.SetActive(true);
        }

        // ================================================================== day end

        /// <summary>showWindow: 2.5 s after the last customer left, the day's rewards and the summary window.</summary>
        public void ShowWindow()
        {
            if (calledShowWindow) return;
            calledShowWindow = true;
            if (Session.isPlayingSpecialLevel)
                VIPServeBonus = Session.vipServedRating > 0 ? (int)((double)Session.vipServedRating * Score.Total - Score.Total) : 0;
            // (sic) the original appended the bonus to the encrypted score string ("cba" + 52), which the decoder
            // ignores: the VIP bonus is shown in the summary but never added to the earnings.
            Tweens.DelayedCall(2.5f, ApplyDayResult, this);
        }

        void ApplyDayResult()
        {
            int earned = 0;
            calledShowWindow = false;
            var p = Player;
            int lvl = Day;
            int total = Score.Total;
            var notifier = AchievementNotifier.Current;
            if (p.daysEarning[lvl] == 0 && lvl == p.passedDays)
            {
                if (servedCustomerCount >= goalTarget)
                {
                    p.daysEarning[lvl] = total;
                    p.passedDays++;
                    earned = total;
                    notifier?.Show(AchievementList.QualifiedChef,
                        AchievementList.Add(p, AchievementList.QualifiedChef, 1, 15));
                    if (total >= expertTarget)
                    {
                        notifier?.Show(AchievementList.InternationalChef,
                            AchievementList.Add(p, AchievementList.InternationalChef, 1, 15));
                        p.money += 6;
                        Session.coinsGot = 6;
                    }
                    else
                    {
                        p.money += 3;
                        Session.coinsGot = 3;
                    }
                    Save();
                    var r = Result();
                    if (p.passedDays == GameRules.TotalDays) GameWindows.ShowStoryCompleteAll(r);
                    else GameWindows.ShowStoryLevelComplete(r);
                    Session.isPlayingSpecialLevel = false;
                }
                else
                {
                    Board.gameover = true;
                    GameWindows.ShowStoryLevelFailed(Result());
                    Session.isPlayingSpecialLevel = false;
                }
            }
            else if (servedCustomerCount >= goalTarget)
            {
                Session.daysEarningPrev = p.daysEarning[lvl];
                if (p.daysEarning[lvl] < total)
                {
                    if (total >= expertTarget && p.daysEarning[lvl] < expertTarget)
                    {
                        notifier?.Show(AchievementList.InternationalChef,
                            AchievementList.Add(p, AchievementList.InternationalChef, 1, 15));
                        p.money += 3;
                        Session.coinsGot = 3;
                    }
                    earned = total - p.daysEarning[lvl];
                    p.daysEarning[lvl] = total;
                    Save();
                }
                GameWindows.ShowStoryLevelComplete(Result());
                Session.isPlayingSpecialLevel = false;
            }
            else
            {
                Board.gameover = true;
                GameWindows.ShowStoryLevelFailed(Result());
                Session.isPlayingSpecialLevel = false;
            }
            notifier?.Show(AchievementList.BigBusiness,
                AchievementList.Add(p, AchievementList.BigBusiness, earned, 20000));
            Save();
        }

        StoryDayResult Result() => new StoryDayResult
        {
            servedCustomers = servedCustomerCount,
            juiceServed = Session.juiceServed,
            friesServed = Session.friesServed,
            earning = Score.Total,
            expertTarget = expertTarget,
            specialLevel = Session.isPlayingSpecialLevel,
            vipBonus = VIPServeBonus,
            coinsGot = Session.coinsGot,
        };

        static void Save()
        {
            if (GameManager.Instance != null && GameManager.Player != null) GameManager.Instance.Saves.Save();
        }

        // ================================================================== texts

        /// <summary>showGoodMovesText: "Good!" .. "WOW!" (+ match bonus) over the board; cookies appear from day 30.</summary>
        void ShowGoodMovesText(int length)
        {
            string word = length == 4 ? "Good!" : length == 5 ? "Nice!" : length == 6 ? "Great!" : length == 7 ? "Awesome!" : "WOW!";
            string text = Loc.T(word);
            if (Player.passedDays >= GameRules.UnlockMatchBonus && length > 4) text = Loc.F("{0}\n+ ${1}", text, length - 4);
            largeText.text = text;
            if (Player.passedDays >= GameRules.UnlockCookies && length > 4) Board.PopCookiesOnBoard();
            largeText.KillTweens();
            largeText.gameObject.SetActive(true);
            largeText.alpha = 0.5f;
            var rt = largeText.rectTransform;
            rt.localScale = new Vector3(0.8f, 0.8f, 1);
            var pos = rt.GetStagePosition();
            rt.SetStagePosition(pos.x, 250);
            float fromAlpha = 0, fromScale = 0, fromY = 0;
            Tweens.To(largeText, 1, k =>
            {
                largeText.alpha = Mathf.LerpUnclamped(fromAlpha, 1, k);
                float s = Mathf.LerpUnclamped(fromScale, 1, k);
                rt.localScale = new Vector3(s, s, 1);
                rt.SetStagePosition(pos.x, Mathf.LerpUnclamped(fromY, 300, k));
            }, () =>
            {
                fromAlpha = largeText.alpha;
                fromScale = rt.localScale.x;
                fromY = rt.GetStagePosition().y;
            }).SetEase(Ease.ExpoOut);
            Tweens.DelayedCall(2, () => largeText.gameObject.SetActive(false), largeText);
        }

        public void ShowNoPossibleMoves()
        {
            blackScreen.gameObject.SetActive(true);
            noPossibleMoveText.gameObject.SetActive(true);
            var rt = noPossibleMoveText.rectTransform;
            float y = rt.GetStagePosition().y;
            rt.KillTweens();
            rt.TweenAnchoredX(512, 1.5f).SetEase(Ease.ExpoOut).OnComplete(() =>
            {
                Board.OnNoMovesTextArrived();
                rt.TweenAnchoredX(1524, 1).SetEase(Ease.ExpoIn).OnComplete(() => rt.SetStagePosition(-500, y));
            });
        }

        public void SetBlackScreen(bool visible) => blackScreen.gameObject.SetActive(visible);

        // ================================================================== IMatch3BoardHost

        public void PlaySfx(string name)
        {
            if (GameManager.Instance != null) Sound.PlaySfx(name);
        }

        public void PlayEffect(BoardEffectKind kind, float x, float y) => boardView.PlayEffect(kind, x, y);

        public bool TouchBlockedByTutorial
        {
            get
            {
                var t = Tut;
                if (t == null) return false;
                if (t.IsButtonShowing) return true;
                int id = t.CurrentDialogueId, dc = t.DialogueCounter;
                return (id == TutorialIds.LEVEL_1 && (dc == 1 || dc == 7))
                    || (id == TutorialIds.LEVEL_12 && (dc == 4 || dc == 5))
                    || (id == TutorialIds.LEVEL_17 && (dc == 4 || dc == 5));
            }
        }

        public bool DragBlockedByTutorial(int lastCol, int lastRow, int col, int row, int lastTileType)
        {
            var t = Tut;
            if (t == null) return false;
            int id = t.CurrentDialogueId, dc = t.DialogueCounter;
            if (id == TutorialIds.LEVEL_26)
            {
                if (dc < 3) return true;
                if (dc == 3 && !t.IsHighlightedTile(lastCol, lastRow)) return true;
            }
            else if (id == TutorialIds.LEVEL_27)
            {
                if (dc == 0) return true;
                if (dc == 2 && (lastTileType != TileTypes.Torch || row >= lastRow)) return true;
            }
            else if (id == TutorialIds.LEVEL_31 && dc <= 2)
            {
                return true;
            }
            return false;
        }

        public bool MatchMustFreeFrozenTile => Tut != null && Tut.CurrentDialogueId == TutorialIds.LEVEL_26 && Tut.DialogueCounter == 3;

        public bool HintOnlyBreadAndLettuce => Tut != null && Tut.CurrentDialogueId == TutorialIds.LEVEL_1;

        public void OnHighlightDispatched(int col, int row) =>
            Trigger(TutorialActions.Highlight, new Dictionary<string, object> { [TutorialActions.KeyRow] = row, [TutorialActions.KeyCol] = col });

        public void OnMatched() => Trigger(TutorialActions.Matched);

        public void OnStartTorch() => Trigger(TutorialActions.StartTorch);

        public void OnTouchBegan()
        {
            choosenFries = false;
            choosenJuice = false;
            // (the original turned a "Select Customer" machine label into "READY!!" here; nothing sets that label)
            chainCombo = 0;
        }

        public void OnTileCollected(int col, int row)
        {
            if (fryer.LayerVisible) fryer.removed++;
            if (juicer.LayerVisible) juicer.removed++;
            if (fryer.LayerVisible) fryer.ShowPercentage();
            if (juicer.LayerVisible) juicer.ShowPercentage();
            CheckCustOrderPrior(col, row);
            CheckCustOrder(col, row);
        }

        public void AfterTilesCollected()
        {
            if (fryer.removed >= fryer.target)
            {
                PlaySfx("fries_machine_ready");
                HardwareReady(fryer);
            }
            if (juicer.removed >= juicer.target)
            {
                PlaySfx("beverage_machine_ready");
                HardwareReady(juicer);
            }
        }

        public void CheckCustomerOrders(int col, int row)
        {
            CheckCustOrderPrior(col, row);
            CheckCustOrder(col, row);
        }

        public void OnGoodMove(int length)
        {
            ShowGoodMovesText(length);
            if (Player.passedDays >= GameRules.UnlockMatchBonus && length >= 5)
            {
                Score.Add(length - 4);
                if (Score.Total >= expertTarget && goalState == 2)
                {
                    goalState = 3;
                    ShowGoalIcon();
                    PlaySfx("expert_reached");
                }
            }
        }

        // ================================================================== tutorial

        public void Trigger(string action) => Tut?.Trigger(action);

        public void Trigger(string action, string key, object value) => Tut?.Trigger(action, key, value);

        void Trigger(string action, IReadOnlyDictionary<string, object> info) => Tut?.Trigger(action, info);

        public bool IsKitchen => false;

        public string JuicePercentageText => juicer != null ? juicer.Percentage.text : "";

        public bool TryGetTilePosition(int col, int row, out Vector2 position)
        {
            if (Board == null || col < 0 || col > 7 || row < 0 || row > 7)
            {
                position = default;
                return false;
            }
            var t = Board.Tiles[col][row];
            position = new Vector2(t.x, t.y);
            return true;
        }

        public bool TryGetFirstListedItem(out Vector2 position, out float height)
        {
            position = default;
            height = 0;
            return false;
        }

        public void OnTutorialCommand(TutorialCommand command)
        {
            switch (command.Type)
            {
                case TutorialCommandType.StartLevel:
                    StartLevel(command.Customers);
                    break;
                case TutorialCommandType.HighlightPossibleMove:
                    Board.dispatchHighlightPosition = 3;
                    Board.HighlightPossible();
                    Board.highlightPossibleTile = true;
                    break;
                case TutorialCommandType.ClearHasWindow:
                    hasWindow = false;
                    break;
                case TutorialCommandType.SendCustomersOutSatisfied:
                    for (int i = 0; i < CustomerCount; i++) customers[i].TweenOut(Match3Customer.Satisfied);
                    break;
                case TutorialCommandType.FreezeTile:
                    Board.FreezeTile(command.Col, command.Row);
                    break;
            }
        }
    }
}
