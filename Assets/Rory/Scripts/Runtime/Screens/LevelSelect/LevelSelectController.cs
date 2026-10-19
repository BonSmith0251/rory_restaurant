using Rory.Data;
using Rory.Save;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Screens.LevelSelect
{
    /// <summary>
    /// Story day selection (LevelSelectScreen): a week planner showing seven days per page (91 days, 13 pages), with
    /// goal / expert badges, the notes of the selected day, and Play / Back / page arrows.
    /// Day indices in this class are 0-based like the original and <see cref="GameSession.currentPlayedLevel"/>.
    /// </summary>
    public class LevelSelectController : ScreenController
    {
        public const int DaysPerPage = 7;
        static readonly Color Black = Color.black;
        static readonly Color Selected = new Color(1, 0, 0); // 16711680

        [Header("Week (Monday .. Sunday)")]
        [SerializeField] TextMeshProUGUI[] dayTexts = new TextMeshProUGUI[DaysPerPage];
        [SerializeField] Button[] dayButtons = new Button[DaysPerPage];
        [SerializeField] Image[] badges = new Image[DaysPerPage];

        [Header("Notes")]
        [SerializeField] Image badgeBig;
        [SerializeField] TextMeshProUGUI noteDescription;
        [SerializeField] GameObject checkMark1;
        [SerializeField] GameObject checkMark2;

        [Header("Buttons")]
        [SerializeField] Button playButton;
        [SerializeField] Button backButton;
        [SerializeField] Button rightButton;
        [SerializeField] Button leftButton;

        [Header("Badge sprites (window atlas)")]
        [SerializeField] Sprite goalSprite;
        [SerializeField] Sprite expertSprite;

        // Texts of the notes and day labels (one template per text, for translation)
        const string DayLabel = "Day\n{0}";
        const string NoteMatch3 = "{0} (Match 3)\nGoal: serve {1} customer(s)\nExpert: earn $ {2}\nYour Best Score: {3}";
        const string NoteFoodCritic = "{0} (Match 3)\nGoal: serve {1} food critic\nExpert: earn $ {2}\nYour Best Score: {3}";
        const string NoteHiddenObject = "Cleaning up (Hidden Objects)\nGoal: find {0} items\nExpert: {1} items\nYour Best Score: {2}";
        const string NoteDessert = "Froyo Business (Minigame)\nGoal: earn ${0}\nExpert: earn ${1}\nYour Best Score: {2}";

        readonly int[] dayIndex = new int[DaysPerPage];

        public int CurrentPage { get; private set; }
        /// <summary>Selected day, 0-based (selectedDays).</summary>
        public int SelectedDay { get; private set; }

        static int TotalDays => GameRules.TotalDays;
        PlayerProfile profile;

        void Start()
        {
            profile = Player ?? new PlayerProfile();
            Session.isPlayingSpecialLevel = false;

            for (int i = 0; i < DaysPerPage; i++)
            {
                int slot = i;
                dayButtons[i].onClick.AddListener(() => SelectDay(dayIndex[slot]));
            }
            playButton.onClick.AddListener(Play);
            backButton.onClick.AddListener(() => GoTo(GameScene.Kitchen));
            rightButton.onClick.AddListener(() => ChangePage(1));
            leftButton.onClick.AddListener(() => ChangePage(-1));

            // TGlobalClass.currentPlayedLevel = passedDays (0-based there)
            Session.currentPlayedLevel = profile.passedDays;
            if (profile.passedDays != TotalDays && profile.goalAchieved[profile.passedDays])
                profile.goalAchieved[profile.passedDays] = false;
            CurrentPage = Mathf.FloorToInt((float)profile.passedDays / DaysPerPage);
            if (CurrentPage >= (float)TotalDays / DaysPerPage) CurrentPage = (int)((float)TotalDays / DaysPerPage - 1);
            SetAvailableDays();
        }

        static DayData Day(int index) => GameDatabase.Instance.levels.days[index];

        bool IsExpert(int index) => profile.daysEarning[index] >= Day(index).expert;

        void SetAvailableDays()
        {
            for (int i = 0; i < DaysPerPage; i++)
            {
                int index = CurrentPage * DaysPerPage + i;
                if (index > profile.passedDays)
                {
                    dayTexts[i].text = "";
                    SetTouchable(i, false);
                    badges[i].gameObject.SetActive(false);
                }
                else
                {
                    dayTexts[i].text = string.Format(DayLabel, index + 1);
                    dayIndex[i] = index;
                    SetTouchable(i, true);
                    SelectedDay = index;
                    if (profile.goalAchieved[index])
                    {
                        badges[i].sprite = IsExpert(index) ? expertSprite : goalSprite;
                        badges[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        badges[i].gameObject.SetActive(false);
                    }
                }
            }
            float passedPages = Mathf.Floor((float)profile.passedDays / DaysPerPage);
            rightButton.gameObject.SetActive(CurrentPage < passedPages);
            leftButton.gameObject.SetActive(CurrentPage > 0);
            if (profile.passedDays == TotalDays && CurrentPage + 1 >= Mathf.Floor((float)profile.passedDays / DaysPerPage))
                rightButton.gameObject.SetActive(false);

            if (SelectedDay > TotalDays - 1) SelectedDay = TotalDays - 1;
            if (SelectedDay < (int)((float)profile.passedDays / DaysPerPage) * DaysPerPage) SelectedDay -= 6;
            if (profile.passedDays == TotalDays && CurrentPage == (float)TotalDays / DaysPerPage - 1) SelectedDay = TotalDays - 1;
            dayTexts[SelectedDay % DaysPerPage].color = Selected;
            ShowDescription();
        }

        void SetTouchable(int slot, bool on)
        {
            dayTexts[slot].raycastTarget = on;
            dayButtons[slot].interactable = on;
        }

        /// <summary>selectDay: a tap on one of the days of the page.</summary>
        public void SelectDay(int index)
        {
            dayTexts[SelectedDay % DaysPerPage].color = Black;
            SelectedDay = index;
            dayTexts[SelectedDay % DaysPerPage].color = Selected;
            ShowDescription();
            checkMark2.SetActive(false);
            checkMark1.SetActive(false);
            if (profile.goalAchieved[index])
            {
                if (IsExpert(index)) checkMark2.SetActive(true);
                checkMark1.SetActive(true);
            }
        }

        void ShowDescription()
        {
            checkMark2.SetActive(false);
            checkMark1.SetActive(false);
            badgeBig.gameObject.SetActive(false);
            if (profile.goalAchieved[SelectedDay])
            {
                if (IsExpert(SelectedDay)) checkMark2.SetActive(true);
                checkMark1.SetActive(true);
                badgeBig.sprite = IsExpert(SelectedDay) ? expertSprite : goalSprite;
                badgeBig.gameObject.SetActive(true);
            }
            var day = Day(SelectedDay);
            int best = profile.daysEarning[SelectedDay];
            string text;
            switch (day.gameType)
            {
                case GameType.HiddenObject:
                    text = string.Format(NoteHiddenObject, day.goal, day.expert, best);
                    break;
                case GameType.DessertMaking:
                    text = string.Format(NoteDessert, day.goal, day.expert, best);
                    break;
                default:
                    string region;
                    switch (day.region)
                    {
                        case GameRules.Western: region = "Western Cuisine"; break;
                        case GameRules.Chinese: region = "Asian Cuisine"; break;
                        default: region = "Middle East Cuisine"; break;
                    }
                    // every 7th day is the food critic day
                    text = string.Format((SelectedDay + 1) % 7 == 0 ? NoteFoodCritic : NoteMatch3, region, day.goal, day.expert, best);
                    break;
            }
            noteDescription.text = text;
        }

        void ChangePage(int delta)
        {
            dayTexts[SelectedDay % DaysPerPage].color = Black;
            CurrentPage += delta;
            SetAvailableDays();
        }

        /// <summary>Play: starts the selected day in the scene of its game type.</summary>
        void Play()
        {
            var day = Day(SelectedDay);
            Session.SelectDay(SelectedDay);
            // The original copies the day's region even when it is empty (minigame days).
            Session.currentRegionDish = day.region ?? "";
            switch (day.gameType)
            {
                case GameType.Match3:
                    GoTo(GameScene.Match3);
                    break;
                case GameType.HiddenObject:
                    GoTo(GameScene.HiddenObject);
                    break;
                case GameType.Match3Special:
                    Session.isPlayingSpecialLevel = true;
                    GoTo(GameScene.Match3);
                    break;
                case GameType.DessertMaking:
                    GoTo(GameScene.Dessert);
                    break;
            }
        }
    }
}
