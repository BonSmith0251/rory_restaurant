using System.Collections.Generic;
using Rory.Achievements;
using AchievementList = Rory.Achievements.Achievements;
using Rory.Data;
using Rory.Tutorials;
using Rory.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Rory.UI;

namespace Rory.Screens.Match3
{
    /// <summary>
    /// One of the three customer rows (TCustomer): avatar, patience heart and bar, the order (up to five ingredients
    /// with counters and check marks), juice / fries on the tray, the priority marker, the price and "READY!".
    /// Walks in from the left (x 0) and out to x = -width over 1 s. Patience counts down one per original frame
    /// (1800 frames per patience point of the customer data); the price drops with it below 80%.
    /// Prefab: Prefabs/Match3/Customer.prefab, three instances in the scene (Match3Builder).
    /// </summary>
    public class Match3Customer : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public const int Satisfied = 1;
        public const int Leave = 2;
        public const int Critic = 13;
        public const int MaxIngredients = 5;
        /// <summary>Width of the click quad; the row's width is this plus the part of the avatar left of x = 0.</summary>
        public const float ClickWidth = 435, ClickHeight = 110;
        /// <summary>The avatar's bottom-right pivot sits at (70, 100).</summary>
        public const float AvatarX = 70, AvatarY = 100;
        public const float PatienceWidth = 89;

        [Header("Parts (TCustomer children)")]
        [SerializeField] Image avatar;
        [SerializeField] Image heartBG;
        [SerializeField] RectTransform heart;
        [SerializeField] GameObject patienceSprite;
        [SerializeField] Image patienceBar;
        [SerializeField] Image patienceGlass;
        [SerializeField] Image[] ingredientIcons = new Image[MaxIngredients];
        [SerializeField] TextMeshProUGUI[] ingredientTexts = new TextMeshProUGUI[MaxIngredients];
        [SerializeField] Image[] checkMarks = new Image[MaxIngredients];
        [SerializeField] Image juice;
        [SerializeField] Image fries;
        [SerializeField] Image queueNumberTransparent;
        [SerializeField] Image queueNumber;
        [SerializeField] TextMeshProUGUI moneyDisplay;
        [SerializeField] TextMeshProUGUI readyText;

        // ---- state (TCustomer fields)
        public int idx;
        public int customerAvaID;
        public List<int> ingredientType = new List<int>();
        public List<int> ingredientTarget = new List<int>();
        public List<bool> ingredientFulfilled = new List<bool>();
        public bool onTween;
        public int queueIdx;
        public int currentDish;
        public int totalPatience;
        public int patience;
        public int money;
        public int totalMoney;
        public int tipsBonus;
        public bool completed = true;
        public bool hasJuice;
        public bool hasFries;
        public bool tweeningPatience;
        public bool asVIP;
        /// <summary>patienceSprite.clipRect.x: -89 (empty) .. 0 (full); read by giveTheCookies.</summary>
        public float patienceClipX;

        Match3Controller game;
        RectTransform rt;

        public RectTransform Rect => rt != null ? rt : rt = (RectTransform)transform;
        public float X => Rect.anchoredPosition.x;
        public float Y => -Rect.anchoredPosition.y;
        /// <summary>Starling width of the row: click quad plus the avatar sticking out on the left.</summary>
        public float Width => ClickWidth + Mathf.Max(0, AvatarWidth - AvatarX);
        float AvatarWidth => avatar.sprite != null ? avatar.sprite.LogicalSize().x : 0;

        public Image Juice => juice;
        public Image Fries => fries;
        public TextMeshProUGUI MoneyDisplay => moneyDisplay;
        public TextMeshProUGUI ReadyText => readyText;
        public bool PriorityVisible => queueNumber.gameObject.activeSelf;
        public IReadOnlyList<TextMeshProUGUI> IngredientTexts => ingredientTexts;
        public IReadOnlyList<Image> CheckMarks => checkMarks;
        public IReadOnlyList<Image> IngredientIcons => ingredientIcons;

        /// <summary>Stage position of ingredient icon <paramref name="k"/> (ing.parent.localToGlobal(ing.x, ing.y)).</summary>
        public Vector2 IngredientStagePosition(int k) => new Vector2(X + k * 51 + 95, Y + 40);

        /// <summary>The click quad in stage coordinates (juice / fries drop target).</summary>
        public Rect ClickBounds => new Rect(X, Y, ClickWidth, ClickHeight);

        public void Init(Match3Controller controller, int index)
        {
            game = controller;
            idx = index;
            completed = true;
            Rect.anchoredPosition = new Vector2(-Width, Rect.anchoredPosition.y);
            readyText.transform.localScale = Vector3.zero;
            readyText.gameObject.SetActive(false);
            queueNumber.gameObject.SetActive(false);
            queueNumberTransparent.gameObject.SetActive(false);
            juice.gameObject.SetActive(false);
            fries.gameObject.SetActive(false);
            for (int i = 0; i < MaxIngredients; i++) checkMarks[i].gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            Rect.KillTweens();
            heart.KillTweens();
            readyText.transform.KillTweens();
            queueNumber.transform.KillTweens();
            this.KillTweens();
        }

        // ------------------------------------------------------------------ touch

        public void OnPointerDown(PointerEventData e) => SelectPriorCust();
        public void OnPointerEnter(PointerEventData e) => queueNumberTransparent.gameObject.SetActive(true);
        public void OnPointerExit(PointerEventData e) => queueNumberTransparent.gameObject.SetActive(false);

        /// <summary>selectPriorCust: this customer's order gets the board's ingredients first.</summary>
        public void SelectPriorCust()
        {
            var session = game.Session;
            if (session.customerFocus == queueIdx) return;
            game.Trigger(TutorialActions.ChangedPriority);
            foreach (var c in game.Customers)
                if (c.queueIdx != queueIdx && c.PriorityVisible) c.HidePriority();
            session.customerFocus = queueIdx;
            ShowPriority();
        }

        // ------------------------------------------------------------------ patience

        /// <summary>dePatience (one original frame).</summary>
        public void DePatience()
        {
            double half = 0.5 * totalMoney;
            double ratio = (double)patience / totalPatience;
            if (patience > 0) patience--;
            else game.TweenDishAlpha(idx, Leave);
            patienceClipX = (float)(ratio * PatienceWidth - PatienceWidth);
            if (patience < 0.8 * totalPatience && !tweeningPatience)
            {
                money = (int)(half + half * ratio);
                moneyDisplay.text = "$" + money;
            }
            RefreshPatienceBar();
        }

        public void RefreshPatienceBar() => patienceBar.fillAmount = Mathf.Clamp01((patienceClipX + PatienceWidth) / PatienceWidth);

        void TweenCustHeart()
        {
            heart.TweenScale(1.2f, 0.5f).SetDelay(patience / 6000f).SetEase(Ease.Linear).OnComplete(() =>
                heart.TweenScale(1, 0.5f).SetEase(Ease.Linear).OnComplete(TweenCustHeart));
        }

        /// <summary>Cookie: patience tweens up by <paramref name="amount"/> in 0.3 s ("give_cookies" when done).</summary>
        public void TweenPatienceTo(int target)
        {
            tweeningPatience = true;
            this.KillTweens();
            float from = patience;
            Tweens.To(this, 0.3f, p => patience = (int)Mathf.LerpUnclamped(from, target, p), () => from = patience).OnComplete(() =>
            {
                tweeningPatience = false;
                game.Trigger(TutorialActions.GiveCookies);
            });
        }

        // ------------------------------------------------------------------ new customer

        /// <summary>newCust(queue): picks a new face and dish for queue number <paramref name="queue"/>.</summary>
        public void NewCust(int queue)
        {
            heart.KillTweens();
            heart.localScale = Vector3.one;
            hasFries = false;
            hasJuice = false;
            var session = game.Session;
            var rng = game.Rng;
            var pool = session.tempCustUse;
            var onQueue = game.CustomerOnQueue;
            int custID;
            while (true)
            {
                if (pool.Count == onQueue.Count) return;
                int pick = rng.Range(0, pool.Count - 1);
                custID = pool[pick];
                if (onQueue.Contains(custID)) continue;
                if (pool.Count > 3) pool.RemoveAt(pick);
                break;
            }
            asVIP = false;
            if (session.isPlayingSpecialLevel && queue == 6)
            {
                custID = Critic;
                asVIP = true;
            }
            var planner = game.Planner;
            planner.CheckDishPriceAndAvailability(custID == Critic ? 2 : 4);
            var dishes = planner.RegionDishes;
            int dishID = 0;
            if (dishes != null && dishes.Count > 0)
            {
                if (session.isPlayingSpecialLevel && custID == Critic)
                    dishID = dishes.Count > 1 ? dishes[rng.Range(0, 1)] : dishes[0];
                else
                    dishID = dishes[rng.Range(0, dishes.Count - 1)];
            }
            currentDish = dishID;
            for (int k = 0; k < MaxIngredients; k++)
            {
                ingredientIcons[k].gameObject.SetActive(false);
                ingredientTexts[k].gameObject.SetActive(false);
                checkMarks[k].gameObject.SetActive(false);
            }
            onQueue.Add(custID);
            avatar.sprite = GameSprite("char_" + custID);
            customerAvaID = custID;
            queueIdx = queue;
            var data = GameDatabase.Instance.customers.customers[custID - 1];
            totalPatience = (int)(1800 * (double)data.patience);
            patience = totalPatience;
            var tut = TutorialManager.Current;
            if (tut != null && tut.CurrentDialogueId == TutorialIds.LEVEL_31 && tut.DialogueCounter == 0 && idx == 0)
                patience = (int)(patience / 2f);
            completed = false;
            avatar.SetNativeSize();
            if (pool.Count <= 0) game.RefillCustomerPool();
            ingredientType.Clear();
            ingredientTarget.Clear();
            ingredientFulfilled.Clear();
            ingredientType = planner.IngredientTypes(dishID);
            ingredientTarget = planner.Targets(dishID);
            if (session.isPlayingSpecialLevel && custID == Critic)
                for (int k = 0; k < ingredientTarget.Count; k++) ingredientTarget[k] *= 2;
            money = planner.DishPrice(dishID);
            if (custID == Critic) money = (int)(money * 1.5);
            // (sic) the tip is a share of the previous customer's totalMoney
            tipsBonus = (int)(rng.Range(1, (int)((double)data.tipsBonus - 1)) * 0.1 * totalMoney);
            var player = game.Player;
            for (int k = 0; k < ingredientType.Count && k < MaxIngredients; k++)
            {
                ingredientFulfilled.Add(false);
                int type = ingredientType[k];
                money += (player.ingredientLevel[type - 1] - 1) * planner.IngredientPriceIncrease(type - 1);
                SetIngredientIcon(k, GameSprite(Match3Board.TextureName(type)));
                ingredientTexts[k].text = "x" + ingredientTarget[k];
                ingredientIcons[k].gameObject.SetActive(true);
                ingredientTexts[k].gameObject.SetActive(true);
            }
            totalMoney = money + tipsBonus;
            moneyDisplay.text = "$" + totalMoney;
            game.SetNextPriorCust();
            TweenCustHeart();
        }

        static Sprite GameSprite(string name) => GameDatabase.Instance.sprites.Get("game", name);

        /// <summary>texture + readjustSize + pivot = width / 2 of the 0.8-scaled icon: (sic) the pivot ends up at 40%.</summary>
        void SetIngredientIcon(int k, Sprite s)
        {
            var img = ingredientIcons[k];
            img.sprite = s;
            img.SetNativeSize();
            img.rectTransform.pivot = new Vector2(0.4f, 0.6f);
        }

        // ------------------------------------------------------------------ priority / states

        public void ShowPriority()
        {
            var q = queueNumber.transform;
            q.KillTweens();
            queueNumber.gameObject.SetActive(true);
            q.localScale = Vector3.zero;
            q.TweenScale(1, 0.5f).SetEase(Ease.BackOut);
        }

        public void HidePriority()
        {
            queueNumber.transform.KillTweens();
            queueNumber.gameObject.SetActive(false);
        }

        /// <summary>setDishCompleteState: the order is done ("dish_completed").</summary>
        public void SetDishCompleteState()
        {
            game.Trigger(TutorialActions.DishCompleted);
            completed = true;
            for (int i = 0; i < MaxIngredients; i++)
            {
                checkMarks[i].gameObject.SetActive(false);
                ingredientIcons[i].gameObject.SetActive(false);
                ingredientTexts[i].gameObject.SetActive(false);
            }
        }

        /// <summary>The ingredient counters that are done show their check mark.</summary>
        public void ShowFulfilled(int k)
        {
            ingredientFulfilled[k] = true;
            checkMarks[k].gameObject.SetActive(true);
            ingredientTexts[k].gameObject.SetActive(false);
        }

        /// <summary>The order's counter <paramref name="k"/> pulses (0.2 s to 1.2x and back).</summary>
        public void PulseCounter(int k)
        {
            var t = ingredientTexts[k].transform;
            t.KillTweens();
            t.TweenScale(1.2f, 0.2f).SetEase(Ease.Linear).OnComplete(() => t.TweenScale(1, 0.2f).SetEase(Ease.Linear));
        }

        /// <summary>The order is complete: "READY!" pops up and keeps pulsing.</summary>
        public void ShowReady()
        {
            readyText.gameObject.SetActive(true);
            readyText.transform.TweenScale(1, 0.4f).SetEase(Ease.BackOut);
            TweenReadyText();
        }

        void TweenReadyText()
        {
            var t = readyText.transform;
            t.TweenScale(1.1f, 0.5f).SetDelay(0.4f).OnComplete(() => t.TweenScale(1, 0.5f).OnComplete(TweenReadyText));
        }

        void SetParts(bool visible)
        {
            heart.gameObject.SetActive(visible);
            heartBG.gameObject.SetActive(visible);
            patienceGlass.gameObject.SetActive(visible);
            patienceSprite.SetActive(visible);
            patienceBar.gameObject.SetActive(visible);
            moneyDisplay.gameObject.SetActive(visible);
        }

        void HideOrder()
        {
            for (int i = 0; i < MaxIngredients; i++)
            {
                ingredientIcons[i].gameObject.SetActive(false);
                ingredientTexts[i].gameObject.SetActive(false);
                checkMarks[i].gameObject.SetActive(false);
            }
        }

        void HideReady()
        {
            readyText.transform.KillTweens();
            readyText.gameObject.SetActive(false);
        }

        /// <summary>tweenOut: walks out (1 s) and, unless time is up, the next customer of the queue walks in.</summary>
        public void TweenOut(int state, bool nextCustomer = true)
        {
            onTween = true;
            completed = true;
            HideOrder();
            SetParts(false);
            queueNumber.gameObject.SetActive(false);
            HideReady();
            bool nextCust = false;
            var session = game.Session;
            if (!game.TimesUp)
            {
                session.currentCust++;
                if (nextCustomer) nextCust = true;
            }
            else
            {
                game.CheckCustomerLeft();
            }
            int queue = session.currentCust;
            Rect.KillTweens();
            Rect.TweenAnchoredX(-Width, 1).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (state == Satisfied)
                {
                    var p = game.Player;
                    if (p != null && !p.unlocked[AchievementList.ServiceExcellence] && (double)patience / totalPatience >= 0.8)
                    {
                        var r = AchievementList.Add(p, AchievementList.ServiceExcellence, 1, 200);
                        AchievementNotifier.Current?.Show(AchievementList.ServiceExcellence, r);
                    }
                }
                else if (state == Leave)
                {
                    game.missedCustomerCount++;
                }
                if (nextCust)
                {
                    NewCust(queue);
                    HideOrder();
                    SetParts(false);
                    HideReady();
                    if (TutorialManager.Current == null || !TutorialManager.Current.BlockCustomerIn) TweenIn();
                }
            });
        }

        /// <summary>tweenIn: walks in (1 s); then the order shows ("customer_in", "special_customer_in" for the critic).</summary>
        public void TweenIn()
        {
            if (game.CheckCustomerLeft()) return;
            game.PlaySfx("bell_ding-ding");
            // AS3: ingredientTarget[0] of an empty vector is undefined (not 0)
            if (ingredientTarget.Count > 0 && ingredientTarget[0] == 0)
            {
                // (sic) the targets of the first western dish, whatever the region
                var west = game.Session.dishRegionWestern;
                if (west != null && west.Count > 0)
                {
                    ingredientTarget = game.Planner.Targets(west[0]);
                    for (int i = 0; i < ingredientType.Count && i < MaxIngredients && i < ingredientTarget.Count; i++)
                        ingredientTexts[i].text = "x" + ingredientTarget[i];
                }
            }
            onTween = true;
            int position = idx;
            Rect.KillTweens();
            Rect.TweenAnchoredX(0, 1).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (!game.Ready) return;
                game.Trigger(TutorialActions.CustomerIn, TutorialActions.KeyPosition, position);
                onTween = false;
                for (int j = 0; j < ingredientType.Count && j < MaxIngredients; j++)
                {
                    ingredientIcons[j].gameObject.SetActive(true);
                    ingredientTexts[j].gameObject.SetActive(true);
                }
                SetParts(true);
                if (customerAvaID == Critic) game.Trigger(TutorialActions.SpecialCustomerIn, TutorialActions.KeyPosition, position);
            });
        }
    }
}
