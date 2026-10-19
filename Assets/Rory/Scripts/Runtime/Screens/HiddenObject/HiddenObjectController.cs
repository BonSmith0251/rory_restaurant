using System.Collections.Generic;
using Rory.Achievements;
using Rory.Data;
using Rory.Localization;
using Rory.Popups;
using Rory.Popups.Game;
using Rory.Tutorials;
using Rory.Tweening;
using Rory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AchievementList = Rory.Achievements.Achievements;

namespace Rory.Screens.HiddenObject
{
    /// <summary>
    /// The hidden-object minigame (MiniGameHO + HOClass): a room (HO1_A .. HO2_C) full of objects, a list of three
    /// objects to find at the top, a timer, a hint jug that refills in 15 s, pause / sound / music buttons.
    /// Story HO days (every 14th day from day 3) pick the room from the day (day / 14) and need 75% of the listed
    /// objects (goal) or all of them (expert); extras mode plays 120 s for score.
    ///
    /// <para>Original quirks kept: objects under the HUD bar can be clicked through it (only the hint jug and the
    /// buttons block); clicking a listed object that is not shown in the list yet is a miss; the flight to the
    /// list uses QuadOut (BezierPath never stored the Linear ease it was given); the found-item particles never
    /// play (TGlobalClass.EnableFoundItemParticle was never set); extras mode always restarts at room 0 (Start()
    /// reset currentLevel, so "Next Level" replays HO1_A with the score carried over and "completed all levels" is
    /// unreachable); on a replayed story day daysEarning (objects found) is compared with the score; three Esc
    /// presses set the objects left to 0 (developer cheat).</para>
    /// </summary>
    public class HiddenObjectController : ScreenController, ITutorialHost
    {
        // ------------------------------------------------------------------ constants (MiniGameHO / HOClass)

        /// <summary>totalItemPerLevel: objects listed per room index.</summary>
        public static readonly IReadOnlyList<int> TotalItemPerLevel = new[] { 21, 24, 27, 30, 33, 33, 33 };
        /// <summary>levelSet: room per index (the 7th story HO day replays HO1_A).</summary>
        public static readonly IReadOnlyList<string> LevelSet = new[] { "HO1_A", "HO1_B", "HO1_C", "HO2_A", "HO2_B", "HO2_C", "HO1_A" };

        public const int DaysPerRoom = 14;
        /// <summary>HINT_RECOVER_TIME = 450 frames at 30 fps.</summary>
        public const float HintRecoverTime = 450 / 30f;
        public const float FrameTime = 1 / 30f;
        /// <summary>Extras mode: 120 * 30 frames.</summary>
        public const float ExtrasTime = 120;
        public const float HintX = 940, HintY = 60;
        public const int MaxDisplayedItems = 3;
        /// <summary>allowedMissClick (no penalty is attached to miss clicks in the original).</summary>
        const int AllowedMissClick = 0;
        /// <summary>TGlobalClass.EnableHintParticle (set to true by initialize()).</summary>
        const bool EnableHintParticle = true;
        /// <summary>TGlobalClass.EnableFoundItemParticle: declared but never set in the original, so psFound never plays.</summary>
        const bool EnableFoundItemParticle = false;
        /// <summary>Esc presses after which the objects left are set to 0 (escCounter &gt; 2).</summary>
        const int EscCheatPresses = 3;

        // list layout (setListedItems / setDisplayedItems)
        const float LabelWidth = 190, LabelHeight = 130;
        const float ListX = 220, ListY = 0;

        [Header("Room")]
        [SerializeField] HiddenObjectZoom zoom;
        [SerializeField] Image background;
        [SerializeField] HiddenObjectItem itemPrefab;

        [Header("HUD")]
        [SerializeField] TextMeshProUGUI moneyText;
        [SerializeField] Image hintFill;
        [SerializeField] Button hintButton;
        [SerializeField] Image timeFill;
        [SerializeField] Button pauseButton;
        [SerializeField] Button soundButton;
        [SerializeField] Button musicButton;
        [SerializeField] GameObject muteIcon;
        [SerializeField] GameObject muteMusicIcon;

        [Header("Object list")]
        [SerializeField] RectTransform labelLayer;
        [SerializeField] TextMeshProUGUI labelPrefab;

        [Header("Effects")]
        [Tooltip("_global.ps: the hint fire that flies to the object and circles it")]
        [SerializeField] UIParticleEmitter hintParticles;
        [Tooltip("_global.psFound (never plays, see the class notes)")]
        [SerializeField] UIParticleEmitter foundParticles;
        [Tooltip("Found objects move here (the original re-parented them to the top of the game layer)")]
        [SerializeField] RectTransform foundLayer;

        /// <summary>An entry of the object list (HOText / itemTextUIAr).</summary>
        public class ListedLabel
        {
            public TextMeshProUGUI text;
            /// <summary>The English label ("Alarm Clock"): the translation key; the text shows its translation.</summary>
            public string english;
            /// <summary>idx: the object's index.</summary>
            public int idx;
            public bool itemIsFound;
            /// <summary>aktif: has been given a place in the list (stays true after being found).</summary>
            public bool aktif;
            public int placeNumber;
            public Vector2 position;
            public RectTransform Rect => text.rectTransform;

            public void SetPosition(float x, float y)
            {
                position = new Vector2(x, y);
                Rect.SetStagePosition(x, y);
            }
        }

        // ------------------------------------------------------------------ state

        bool ready, paused, gameover, hasWindow;
        int currentLevel;
        int goalTarget, expertTarget;
        float totalTime, timer;
        bool hasHint;
        float hintTimer;
        int escCounter;

        readonly List<HiddenObjectItem> items = new List<HiddenObjectItem>();
        readonly List<int> itemAsNormal = new List<int>();
        readonly List<int> listedItemAr = new List<int>();
        readonly List<int> itemFoundAr = new List<int>();
        readonly List<ListedLabel> labels = new List<ListedLabel>();
        readonly bool[] occupiedPlace = new bool[MaxDisplayedItems];
        int itemFoundCount, totalItems, maxItems;
        int missClickCount, soundCombo;
        int targetIDBegan = -1;
        int revealedIdx;
        HiddenObjectItem revealedItem;

        // hint fire circling the revealed object (tweenCircle)
        bool circling;
        float currentAngle = 0.25f, centerX = 150, centerY = 150, radiusX = 80, radiusY = 80;
        const float AngleStep = 0.02f;

        // ------------------------------------------------------------------ public state (tests, tools)

        public override bool IsReady => ready;
        /// <summary>Index into <see cref="LevelSet"/> (currentLevel).</summary>
        public int CurrentLevel => currentLevel;
        /// <summary>Room being played, e.g. "HO2_A".</summary>
        public string SceneId => LevelSet[currentLevel];
        public IReadOnlyList<HiddenObjectItem> Items => items;
        /// <summary>listedItemAr: object indices to find, in list order.</summary>
        public IReadOnlyList<int> ListedItems => listedItemAr;
        public IReadOnlyList<ListedLabel> Labels => labels;
        public int ItemFoundCount => itemFoundCount;
        /// <summary>totalItems: listed objects not found (removed from the list) yet.</summary>
        public int TotalItems => totalItems;
        public int GoalTarget => goalTarget;
        public int ExpertTarget => expertTarget;
        public int BonusMoney => Session.hoBonusMoney;
        public bool HasHint => hasHint;
        public bool IsGameOver => gameover;
        public bool IsPaused => paused;
        public float TotalTime => totalTime;
        /// <summary>Seconds left (timer / 30). Settable for tests and tools.</summary>
        public float TimeLeft { get => timer; set => timer = value; }
        public float HintTimer => hintTimer;
        public int RevealedIndex => revealedIdx;
        public bool HintCircling => circling;
        public HiddenObjectZoom Zoom => zoom;
        public TextMeshProUGUI MoneyText => moneyText;
        public UIParticleEmitter HintParticles => hintParticles;
        public Button HintButton => hintButton;

        static TutorialManager Tut => TutorialManager.Current;
        static bool BlackScreenVisible => PopupManager.Current != null && PopupManager.Current.HasOpenPopup;

        // ------------------------------------------------------------------ Start + initialize

        void Start()
        {
            var p = Player;
            // Start(): the room
            if (!Session.extrasMode)
            {
                currentLevel = Mathf.Clamp(Mathf.FloorToInt((float)Session.currentPlayedLevel / DaysPerRoom), 0, LevelSet.Count - 1);
                goalTarget = (int)(TotalItemPerLevel[currentLevel] * 0.75f);
                expertTarget = TotalItemPerLevel[currentLevel];
            }
            else
            {
                currentLevel = 0;
            }
            Session.hoCurrentLevel = currentLevel;

            // initialize()
            Sound.PlayMusic("sunny-bliss");
            if (!Session.extrasMode)
            {
                var day = Session.CurrentDay;
                totalTime = day != null ? day.time : 120;
                Session.hoBonusMoney = 0;
            }
            else
            {
                totalTime = ExtrasTime;
                // the main menu's HO button reset bonusMoney; "Next Level" keeps it
                if (!Session.hoExtrasNextLevel) Session.hoBonusMoney = 0;
            }
            Session.hoExtrasNextLevel = false;
            timer = totalTime;

            string sceneId = SceneId;
            background.sprite = Db.sprites.Get("Backgrounds", sceneId + "_BG");
            ReadItems(Db.GetHiddenObjectLayout(sceneId));
            InitHOItems(TotalItemPerLevel[currentLevel]);

            RefreshMoneyText(itemFoundCount);
            hasHint = true;
            hintFill.fillAmount = 1;
            timeFill.fillAmount = 1;
            hintButton.onClick.AddListener(OnHintButton);
            pauseButton.onClick.AddListener(OnPauseButton);
            soundButton.onClick.AddListener(ToggleSound);
            musicButton.onClick.AddListener(ToggleMusic);
            RefreshMuteIcons();

            InitUIObjects();
            SetupParticles();

            Session.currentNotifWindow = 0;
            GameWindows.LastDayInfo = "";
            if (Tut != null) Tut.Host = this;
            var events = Db.levelEvents != null ? Db.levelEvents.days : null;
            int lvl = Session.currentPlayedLevel;
            if (!Session.extrasMode && events != null && lvl >= 0 && lvl < events.Count && events[lvl].windows.Count > 0 &&
                (Tut == null || !Tut.IsShowing))
            {
                GameWindows.ShowBeginningWindows(lvl, new BeginningWindowOptions { startIndex = 0, onAllClosed = StartLevel });
            }
            if (Tut != null)
            {
                Tut.CheckTut(TutorialIds.LEVEL_4, 3, true);
                Tut.CheckTut(TutorialIds.LEVEL_18, 17);
                Tut.CheckTut(TutorialIds.LEVEL_32, 31);
            }
            hasHint = true;
            gameover = false;
            paused = false;
            escCounter = 0;
            ready = true;
            if (p == null) Debug.LogWarning("[Rory] Hidden object screen started without a player");
        }

        /// <summary>startLevel(): the last beginning window closed.</summary>
        void StartLevel() => hasWindow = false;

        /// <summary>readItemData: names, positions and types from the layout ("name,nm", "name,bg", "name,bg2", ...).</summary>
        void ReadItems(HiddenObjectLayout layout)
        {
            if (layout == null)
            {
                Debug.LogError($"[Rory] No hidden object layout for {SceneId}");
                return;
            }
            string atlas = SceneId;
            for (int i = 0; i < layout.items.Count; i++)
            {
                var entry = layout.items[i];
                string fileName = entry.name;
                int comma = fileName.IndexOf(',');
                string tags = comma >= 0 ? fileName.Substring(comma + 1) : fileName;
                int type;
                string itemName;
                if (tags.Contains("nm,mul"))
                {
                    // multiples are never listed (only "nm" objects are), so the original's multiple-object list
                    // handling never ran; they behave like any unlisted object
                    type = HiddenObjectItem.TypeMultiple;
                    int us = fileName.IndexOf('_');
                    itemName = us >= 0 ? fileName.Substring(0, us) : fileName;
                }
                else
                {
                    itemName = comma >= 0 ? fileName.Substring(0, comma) : fileName;
                    if (tags.Contains("nm,coin")) type = HiddenObjectItem.TypeCoin;
                    else if (tags.Contains("nm,logo")) type = HiddenObjectItem.TypeLogo;
                    else if (tags.Contains("nm"))
                    {
                        type = HiddenObjectItem.TypeObject;
                        itemAsNormal.Add(i);
                    }
                    else if (tags.Contains("bg2")) type = HiddenObjectItem.TypeBackground2;
                    else type = HiddenObjectItem.TypeBackground; // "bg"
                }
                var sprite = Db.sprites.Get(atlas, fileName);
                if (sprite == null) Debug.LogWarning($"[Rory] Hidden object sprite {atlas}/{fileName} missing");
                var item = Instantiate(itemPrefab, zoom.transform, false);
                item.Setup(this, i, fileName, itemName, type, sprite, entry.locX, entry.locY);
                items.Add(item);
            }
        }

        /// <summary>initHOItems: picks <paramref name="count"/> random "nm" objects to find.</summary>
        void InitHOItems(int count)
        {
            listedItemAr.Clear();
            maxItems = count;
            totalItems = count;
            for (int n = 0; n < count && itemAsNormal.Count > 0; n++)
            {
                int pick = GameRules.RandomRange(0, itemAsNormal.Count - 1);
                int id = itemAsNormal[pick];
                itemAsNormal.RemoveAt(pick);
                listedItemAr.Add(id);
                items[id].Listed = true;
            }
        }

        /// <summary>initUIObjects: one label per listed object, then setListedItems.</summary>
        void InitUIObjects()
        {
            foreach (int id in listedItemAr)
            {
                string english = FormatLabel(items[id].ItemName);
                var t = Instantiate(labelPrefab, labelLayer, false);
                t.name = "Label_" + id;
                t.text = Loc.T(english);
                var label = new ListedLabel { text = t, english = english, idx = id };
                label.SetPosition(ListX, -300); // HOText.x = listedItemsPlace.x, y = -300 (centre pivot)
                if (itemFoundAr.Contains(id)) label.itemIsFound = true;
                labels.Add(label);
            }
            SetListedItems();
        }

        /// <summary>
        /// The list text of an object name: "_QUOTE_" becomes "'", the first letter and every letter after a space are
        /// capitalised, then " Of " / " And " are lowered again (first occurrence each, like AS3 String.replace).
        /// </summary>
        public static string FormatLabel(string itemName)
        {
            string s = ReplaceFirst(itemName ?? "", "_QUOTE_", "'");
            if (s.Length == 0) return s;
            var chars = s.ToCharArray();
            chars[0] = char.ToUpperInvariant(chars[0]);
            for (int c = 0; c < chars.Length - 1; c++)
                if (chars[c] == ' ') chars[c + 1] = char.ToUpperInvariant(chars[c + 1]);
            s = new string(chars);
            s = ReplaceFirst(s, " Of ", " of ");
            s = ReplaceFirst(s, " And ", " and ");
            return s;
        }

        static string ReplaceFirst(string s, string find, string with)
        {
            int i = s.IndexOf(find, System.StringComparison.Ordinal);
            return i < 0 ? s : s.Substring(0, i) + with + s.Substring(i + find.Length);
        }

        static float PlaceX(int place) => place * 200 + 235 + LabelWidth / 2;
        static float PlaceY => ListY + 50;

        /// <summary>setListedItems: the first three objects not found take the three places.</summary>
        void SetListedItems()
        {
            int place = 0;
            for (int i = 0; i < labels.Count; i++)
            {
                var l = labels[i];
                if (l.itemIsFound) continue;
                if (place > MaxDisplayedItems - 1) break;
                Place(l, place);
                place++;
            }
        }

        void Place(ListedLabel l, int place)
        {
            l.SetPosition(PlaceX(place), PlaceY);
            l.aktif = true;
            l.placeNumber = place;
            occupiedPlace[place] = true;
        }

        /// <summary>setDisplayedItems: every free place gets the first object neither found nor shown yet.</summary>
        void SetDisplayedItems()
        {
            for (int place = 0; place < occupiedPlace.Length; place++)
            {
                foreach (var l in labels)
                {
                    if (!l.itemIsFound && !l.aktif && !occupiedPlace[place])
                    {
                        Place(l, place);
                        break;
                    }
                }
            }
        }

        void SetupParticles()
        {
            if (hintParticles != null)
            {
                hintParticles.Stop(true);
                hintParticles.emitterOffset = new Vector2(20, 40);
            }
            if (foundParticles != null && foundParticles.config != null)
            {
                // psFound: emitAngleVariance = 3 (radians), speed = 70, startColorVariance white. A runtime copy:
                // the config asset must not change.
                var c = Instantiate(foundParticles.config);
                c.angleVariance = 3 * Mathf.Rad2Deg;
                c.speed = 70;
                c.startColorVariance = new Color(1, 1, 1, c.startColorVariance.a);
                foundParticles.config = c;
                foundParticles.Stop(true);
            }
        }

        // ------------------------------------------------------------------ ITutorialHost

        public bool IsKitchen => false;
        public string JuicePercentageText => null;

        public bool TryGetTilePosition(int col, int row, out Vector2 position)
        {
            position = default;
            return false;
        }

        public bool TryGetFirstListedItem(out Vector2 position, out float height)
        {
            if (listedItemAr.Count == 0)
            {
                position = default;
                height = 0;
                return false;
            }
            var item = items[listedItemAr[0]];
            position = item.Location;
            height = item.Size.y * item.Rect.localScale.y;
            return true;
        }

        public void OnTutorialCommand(TutorialCommand command) { }

        // ------------------------------------------------------------------ frame update

        void Update()
        {
            if (!ready) return;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                escCounter++;
                if (escCounter >= EscCheatPresses) totalItems = 0;
            }
            if (circling && !paused) TweenCircle(Time.deltaTime);

            if (hasWindow || paused || gameover) return;
            if (timer > 0)
            {
                if (!BlackScreenVisible && (Tut == null || !Tut.IsButtonShowing))
                {
                    timer -= Time.deltaTime;
                    timeFill.fillAmount = Mathf.Clamp01(timer / totalTime);
                }
            }
            else
            {
                gameover = true;
                OnLevelComplete();
            }
            if (!hasHint)
            {
                hintTimer += Time.deltaTime;
                hintFill.fillAmount = Mathf.Clamp01(hintTimer / HintRecoverTime);
                if (hintTimer >= HintRecoverTime)
                {
                    hasHint = true;
                    hintButton.gameObject.SetActive(true);
                }
            }
        }

        // ------------------------------------------------------------------ buttons

        void OnPauseButton()
        {
            paused = true;
            GameWindows.ShowPause(new PauseOptions { onResume = () => paused = false, onMuteChanged = RefreshMuteIcons });
        }

        void ToggleSound()
        {
            Sound.MuteSound = !Sound.MuteSound;
            RefreshMuteIcons();
        }

        void ToggleMusic()
        {
            Sound.MuteMusic = !Sound.MuteMusic;
            RefreshMuteIcons();
        }

        void RefreshMuteIcons()
        {
            muteIcon.SetActive(Sound.MuteSound);
            muteMusicIcon.SetActive(Sound.MuteMusic);
        }

        void OnHintButton()
        {
            if (Tut != null && Tut.CurrentDialogueId == TutorialIds.LEVEL_5 && Tut.DialogueCounter < 5) return;
            RevealItem();
        }

        // ------------------------------------------------------------------ touches

        internal void OnItemPointerDown(HiddenObjectItem item) => targetIDBegan = item.Index;

        /// <summary>touchHOObjects (TouchPhase.ENDED).</summary>
        internal void OnItemPointerUp(HiddenObjectItem item)
        {
            if (!ready) return;
            if (Tut != null && Tut.CurrentDialogueId == TutorialIds.LEVEL_4 && Tut.DialogueCounter < 4) return;
            if (itemFoundCount >= maxItems || zoom.TouchDragged) return;
            int target = item.Index;
            if (targetIDBegan != target) return;
            if (item.Listed && !item.Found)
            {
                for (int i = 0; i < labels.Count; i++)
                {
                    var l = labels[i];
                    if (target != l.idx || l.itemIsFound) continue;
                    if (l.aktif)
                    {
                        GetCorrectItem(target, i);
                        Sound.PlaySfx(ClickSound(soundCombo));
                    }
                    else
                    {
                        OnMissClick();
                        missClickCount++;
                        Sound.PlaySfx("SFX_wrong_click");
                    }
                    break;
                }
            }
            else if (!item.Listed)
            {
                Sound.PlaySfx("SFX_wrong_click");
                OnMissClick();
                missClickCount++;
            }
        }

        /// <summary>touchBackground (TouchPhase.ENDED).</summary>
        internal void OnBackgroundPointerUp()
        {
            if (ready) OnMissClick();
        }

        static string ClickSound(int combo) => "SFX_gameplay_click_0" + (combo > 7 ? 8 : combo + 1);

        /// <summary>onMissClick: wrong-click sound, the click-sound combo restarts.</summary>
        void OnMissClick()
        {
            Sound.PlaySfx("SFX_wrong_click");
            soundCombo = 0;
            missClickCount++;
            if (missClickCount > AllowedMissClick) missClickCount = 0;
        }

        // ------------------------------------------------------------------ found objects

        /// <summary>getCorrectItem: score, Hawk Eye, the object leaves the room for the top layer and flies to its label.</summary>
        void GetCorrectItem(int target, int uiIdx)
        {
            if (Tut != null) Tut.Trigger(TutorialActions.GetCorrectHoItem);
            UpdateMoney();
            var p = Player;
            if (!Session.extrasMode && p != null && !p.unlocked[AchievementList.HawkEye])
            {
                // HAWKEYE: unlocked above the target (not at it), progress every 50
                int id = AchievementList.HawkEye;
                p.achievementCounter[id]++;
                if (p.achievementCounter[id] > AchievementList.Target[id])
                {
                    p.unlocked[id] = true;
                    AchievementNotifier.Current?.Show(id, AchievementList.Result.Unlocked);
                }
                else if (p.achievementCounter[id] % 50 == 0)
                {
                    AchievementNotifier.Current?.Show(id, AchievementList.Result.Progress);
                }
            }
            var item = items[target];
            var pos = zoom.RoomToStage(item.Rect.GetStagePosition());
            item.Rect.SetParent(foundLayer, false);
            item.Rect.SetAsLastSibling();
            item.Rect.SetStagePosition(pos.x, pos.y);
            item.Rect.localScale = new Vector3(zoom.Scale, zoom.Scale, 1);
            item.Found = true;
            itemFoundAr.Add(target);
            missClickCount = 0;
            labels[uiIdx].itemIsFound = true;
            itemFoundCount++;
            soundCombo++;
            if (!hasHint) hintTimer += FrameTime;
            TweenFoundItem(target, uiIdx, 0);
        }

        /// <summary>tweenFoundItem: pop (+0.4 scale, elastic, 0.4 s), then a Bezier flight to the label while
        /// spinning once and shrinking to the label's scale; 0.2 s after landing the object and label are removed.</summary>
        void TweenFoundItem(int target, int uiIdx, int tweenCount)
        {
            var item = items[target];
            var rt = item.Rect;
            item.OnTween = true;
            rt.KillTweens();
            if (tweenCount < 1)
            {
                float s0 = rt.localScale.x, s1 = s0 + 0.4f;
                Tweens.To(rt, 0.4f, p => rt.localScale = new Vector3(Mathf.LerpUnclamped(s0, s1, p), Mathf.LerpUnclamped(s0, s1, p), 1))
                    .SetEase(Ease.ElasticOut).OnComplete(() => TweenFoundItem(target, uiIdx, 1));
                if (target == revealedIdx)
                {
                    if (hintParticles != null) hintParticles.Stop();
                    circling = false;
                    currentAngle = 0.25f;
                }
                if (EnableFoundItemParticle && foundParticles != null)
                {
                    foundParticles.emitterOffset = rt.GetStagePosition();
                    foundParticles.Play(float.PositiveInfinity);
                    Tweens.DelayedCall(0.5f, () => foundParticles.Stop(), this);
                }
                return;
            }

            float randomFirstX = GameRules.RandomRange(1, 2) == 1 ? -100 : 100;
            float randomSecondX = GameRules.RandomRange(30, 80);
            if (!(randomFirstX > 0)) randomSecondX = -randomSecondX;
            var label = labels[uiIdx];
            var from = rt.GetStagePosition();
            var to = label.position;
            float selisih = from.y - to.y;
            var path = new BezierPath(from,
                new Vector2(from.x + randomFirstX, from.y + selisih / 3),
                new Vector2(from.x + randomSecondX, from.y - selisih / 3 * 4),
                to);
            float jarak = Vector2.Distance(from, to);
            float multiplier = jarak > 500 ? 0.001f : (jarak < 200 ? 0.0025f : 0.0015f);
            float duration = 0.3f + jarak * multiplier;
            // BezierPath.start(): the tween's ease stays null, i.e. TweenLite's default (QuadOut)
            Tweens.To(this, duration, p =>
            {
                if (rt != null && path.TryGetFollowerPosition(p, out var at)) rt.SetStagePosition(at.x, at.y);
            }).SetEase(Ease.QuadOut).OnComplete(() =>
                Tweens.DelayedCall(0.2f, () => RemoveItem(target, uiIdx), this));
            float sFrom = rt.localScale.x, sTo = label.Rect.localScale.x;
            Tweens.To(rt, duration, p =>
            {
                float s = Mathf.LerpUnclamped(sFrom, sTo, p);
                rt.localScale = new Vector3(s, s, 1);
                rt.localEulerAngles = new Vector3(0, 0, -360 * p); // rotation 0 -> 2 PI (clockwise)
            }).SetEase(Ease.Linear);
        }

        /// <summary>removeItem: the label and object leave the screen, the free place is refilled; all found = level end.</summary>
        void RemoveItem(int target, int uiIdx)
        {
            var label = labels[uiIdx];
            label.SetPosition(-100, label.position.y);
            label.itemIsFound = true;
            occupiedPlace[label.placeNumber] = false;
            var rt = items[target].Rect;
            rt.SetStagePosition(-200, rt.GetStagePosition().y);
            items[target].OnTween = false;
            totalItems--;
            SetDisplayedItems();
            if (totalItems <= 0) OnLevelComplete();
        }

        /// <summary>updateMoney (addScore): +5 * (room + 1) + 5 per object; the goal badge when the goal is reached.</summary>
        void UpdateMoney()
        {
            Session.hoBonusMoney += 5 * (currentLevel + 1) + 5;
            if (Session.extrasMode)
            {
                RefreshMoneyText(itemFoundCount + 1);
                return;
            }
            var p = Player;
            int lvl = Session.currentPlayedLevel;
            RefreshMoneyText(itemFoundCount + 1);
            if (p != null && !p.goalAchieved[lvl] && itemFoundCount + 1 == goalTarget)
            {
                GameWindows.ShowGoalIconSmall(GoalIconPlace.HiddenObject, itemFoundCount + 1 >= expertTarget);
                Sound.PlaySfx("goal_reached");
                p.goalAchieved[lvl] = true;
                RefreshMoneyText(itemFoundCount + 1);
            }
        }

        /// <summary>moneyText: "Goal: n/goal" until the goal is reached, then "Exp.: n/expert"; extras "$ score".</summary>
        void RefreshMoneyText(int found)
        {
            if (Session.extrasMode)
            {
                moneyText.text = "$ " + Session.hoBonusMoney;
                return;
            }
            var p = Player;
            int lvl = Session.currentPlayedLevel;
            bool goal = p != null && lvl >= 0 && lvl < p.goalAchieved.Count && p.goalAchieved[lvl];
            moneyText.text = goal ? Loc.F("Exp.: {0}/{1}", found, expertTarget) : Loc.F("Goal: {0}/{1}", found, goalTarget);
        }

        // ------------------------------------------------------------------ hints

        /// <summary>revealItem: a random shown object gets the hint fire (flies from the jug, then circles it twice).</summary>
        public void RevealItem()
        {
            if (!hasHint || !ready) return;
            var available = new List<int>();
            foreach (var l in labels)
                if (l.aktif && !l.itemIsFound) available.Add(l.idx);
            if (available.Count == 0) return;
            Sound.PlaySfx("bell_dring");
            if (Tut != null) Tut.Trigger(TutorialActions.UsedHint);
            if (zoom.Scale > 1) zoom.ScaleBack();
            revealedIdx = available[GameRules.RandomRange(0, available.Count - 1)];
            revealedItem = items[revealedIdx];
            UseHint();
            if (EnableHintParticle && hintParticles != null)
            {
                hintParticles.Play(float.PositiveInfinity);
                // the object's position in the room (= on the stage once the zoom is back at x1)
                var c = revealedItem.Rect.GetStagePosition();
                float x = c.x, y = c.y;
                hintParticles.emitterOffset = new Vector2(HintX, HintY);
                radiusX = revealedItem.Size.x;
                radiusY = revealedItem.Size.y;
                var path = new BezierPath(new Vector2(HintX, HintY),
                    new Vector2(HintX, HintY - GameRules.RandomRange(-100, 300)),
                    new Vector2(x - GameRules.RandomRange(200, 400), y + radiusY),
                    new Vector2(x, y + radiusY));
                float jarak = Mathf.Sqrt(Mathf.Pow(x - HintX, 2) + Mathf.Pow(y - HintY, 2));
                circling = false;
                var ps = hintParticles;
                // BezierPath.startPS(): ease left null -> TweenLite's default (QuadOut)
                Tweens.To(ps, 0.003f * jarak, p =>
                {
                    if (path.TryGetFollowerPosition(p, out var at)) ps.emitterOffset = at;
                }).SetEase(Ease.QuadOut).OnComplete(() => circling = true);
                centerX = x;
                centerY = y;
            }
        }

        /// <summary>useHint: the jug empties and refills in 15 s; the HINT! button hides meanwhile.</summary>
        void UseHint()
        {
            hasHint = false;
            hintTimer = 0;
            hintFill.fillAmount = 0;
            hintButton.gameObject.SetActive(false);
        }

        /// <summary>tweenCircle: 0.02 turn per frame around the object (radii = its size), two turns, then the fire stops.</summary>
        void TweenCircle(float dt)
        {
            currentAngle -= AngleStep * 30 * dt;
            float twoPi = Mathf.PI * 2;
            hintParticles.emitterOffset = new Vector2(centerX + Mathf.Cos(currentAngle * twoPi) * radiusX,
                centerY + Mathf.Sin(currentAngle * twoPi) * radiusY);
            if (currentAngle < -1.75f)
            {
                currentAngle = 0.25f;
                hintParticles.Stop();
                circling = false;
            }
        }

        // ------------------------------------------------------------------ level end

        /// <summary>onLevelComplete: rewards, achievements and save (story), then the summary window.</summary>
        void OnLevelComplete()
        {
            var r = new HiddenObjectResult { itemsFound = itemFoundCount, earning = Session.hoBonusMoney, itemsLeft = totalItems };
            if (Session.extrasMode)
            {
                Sound.PlaySfx("goal_reached");
                if (currentLevel >= LevelSet.Count - 1) GameWindows.ShowHOExtrasCompleteAll(r);
                else if (gameover) GameWindows.ShowHOExtrasFailed(r);
                else GameWindows.ShowHOExtrasComplete(r, NextExtrasLevel);
                return;
            }

            var p = Player;
            int lvl = Session.currentPlayedLevel;
            int earned = 0;
            if (p != null && lvl >= 0 && lvl < p.daysEarning.Count)
            {
                int bonus = Session.hoBonusMoney;
                if (p.daysEarning[lvl] == 0 && lvl == p.passedDays)
                {
                    // first play of the day
                    if (itemFoundCount >= goalTarget)
                    {
                        Notify(AchievementList.QualifiedChef, AchievementList.Add(p, AchievementList.QualifiedChef, 1, 15));
                        p.daysEarning[lvl] = itemFoundCount;
                        p.passedDays++;
                        if (totalItems == 0)
                        {
                            p.money += 6;
                            Session.coinsGot = 6;
                            Notify(AchievementList.InternationalChef, AchievementList.Add(p, AchievementList.InternationalChef, 1, 15));
                        }
                        else
                        {
                            p.money += 3;
                            Session.coinsGot = 3;
                        }
                    }
                    earned = bonus;
                    Game.Saves.Save();
                }
                else
                {
                    // replay: (sic) daysEarning holds the objects found, compared here with the score
                    if (itemFoundCount >= goalTarget && bonus > p.daysEarning[lvl])
                    {
                        Session.daysEarningPrev = p.daysEarning[lvl];
                        if (totalItems == 0 && itemFoundCount > p.daysEarning[lvl])
                        {
                            p.money += 3;
                            Session.coinsGot = 3;
                            Notify(AchievementList.InternationalChef, AchievementList.Add(p, AchievementList.InternationalChef, 1, 15));
                        }
                        earned = bonus - p.daysEarning[lvl];
                        p.daysEarning[lvl] = itemFoundCount;
                    }
                    Game.Saves.Save();
                }
                Notify(AchievementList.BigBusiness, AchievementList.Add(p, AchievementList.BigBusiness, earned, 20000));
                Game.Saves.Save();
            }
            r.coinsGot = Session.coinsGot;
            if (itemFoundCount >= goalTarget) GameWindows.ShowHOLevelComplete(r);
            else GameWindows.ShowHOLevelFailed(r);
        }

        static void Notify(int id, AchievementList.Result result) => AchievementNotifier.Current?.Show(id, result);

        /// <summary>NEXT_LEVEL: currentLevel + 1 and the HO screen again (which, in extras mode, starts at room 0 again).</summary>
        void NextExtrasLevel()
        {
            Session.hoCurrentLevel = currentLevel + 1;
            Session.hoExtrasNextLevel = true;
            GoTo(GameScene.HiddenObject);
        }

        void OnDestroy()
        {
            if (Tut != null && ReferenceEquals(Tut.Host, this)) Tut.Host = null;
        }
    }
}
