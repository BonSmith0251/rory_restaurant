using System;
using System.Collections.Generic;
using System.Globalization;
using Rory.Data;
using Rory.Popups;
using Rory.Tweening;
using Rory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Tutorials
{
    /// <summary>
    /// Tutorial / dialogue bubble (Tutorial.as): Rory's text box with avatar, bouncing arrow and Next button, driven by
    /// <c>GameDatabase.dialogues</c> and by actions the screens report through <see cref="Trigger"/>.
    /// One per scene (built by <c>TutorialBuilder.AddTo(stage)</c>); reach it with <see cref="Current"/>.
    ///
    /// <para><b>API</b></para>
    /// <code>
    /// static TutorialManager Current                 // the scene's tutorial (null if the scene has none)
    /// ITutorialHost Host { get; set; }                // the screen registers itself (tokens, juice text, commands)
    /// int  CurrentDialogueId                          // TutorialIds.*, -1 = none (currentDialogueID)
    /// int  DialogueCounter                            // current line (dialogueCounter)
    /// bool IsShowing                                  // bubble visible (textBox.visible)  - timers pause on it
    /// bool IsButtonShowing                            // Next button visible (button.visible) - screens block input on it
    /// bool BlockCustomerIn                            // match-3: customers must not tween in (blockCustomerIn)
    /// IReadOnlyList&lt;Vector2Int&gt; HighlightedTiles     // LEVEL_26 highlighted (col,row) list (highlightedAr; x = col, y = row)
    /// bool IsHighlightedTile(int col, int row)        // LEVEL_26 dc 3 swap check: (col,row) among the first 3
    /// int  HighlightX, HighlightY                     // gameScreen.highlightX/Y (TILEX/TILEY)
    /// bool CheckTut(int id, int day, bool force = false)  // checkTut; day = original 0-based day (= passedDays)
    /// void Show(int id, int line = 0)                 // showTutorial
    /// void Next()                                     // nextTutorial
    /// void Hide()                                     // hideTutorial (keeps CurrentDialogueId)
    /// void Abort()                                    // hide + CurrentDialogueId = -1 (TWindowDisplay on screen exit)
    /// void Trigger(string action, IReadOnlyDictionary&lt;string, object&gt; info = null)   // TutorialEvent.TRIGGER_NEXT
    /// void Trigger(string action, string key, object value)                           // one-key info shortcut
    /// bool CanClickThisButton(int buttonId)           // canClickThisButton (TutorialButtons.*)
    /// string DisplayText(int id, int line, DialogueLine l)  // the only text -> bubble string step ("(player)", "(juice percentage)")
    /// string ButtonLabel(int id, int line, DialogueLine l)  // the only buttonText -> Next label step
    /// event Action&lt;int,int&gt; LineShown               // (id, line) after a line is displayed
    /// event Action StartLevelLineReached              // a "(start level)" line was reached (Host also gets StartLevel)
    /// event Action&lt;int&gt; DialogueFinished             // Next() ran past the last line of dialogue id
    /// event Action&lt;TutorialCommand&gt; CommandIssued   // every command, after Host.OnTutorialCommand (observers only)
    /// </code>
    /// Screens act on commands in <see cref="ITutorialHost.OnTutorialCommand"/>; the events are notifications for
    /// observers and must not start the level a second time.
    ///
    /// <para><b>checkTut calls of the original</b> (in this order; day = 0-based passedDays)</para>
    /// <code>
    /// Kitchen (after the kitchen window): OPENING_STORY 0, AFTER_LEVEL_1 1, AFTER_LEVEL_2 2, AFTER_LEVEL_7 7,
    ///          AFTER_LEVEL_8 8, AFTER_LEVEL_34 34 (all called); hide the achievement bubble/quad if OPENING_STORY shows.
    /// Match-3 (init, else-if chain: first match wins): LEVEL_1 0, LEVEL_5 4, LEVEL_7 6, LEVEL_8 7,
    ///          LEVEL_12 11 (then juice counter = target-3), LEVEL_13 12, LEVEL_14 13, LEVEL_16 15,
    ///          LEVEL_17 16 (fries counter = target-3), LEVEL_19 18, LEVEL_21 20, LEVEL_22 21,
    ///          LEVEL_26 25 (then HighlightPossibleMove itself), LEVEL_27 26 (freeze [0][0], torch at [1][1]),
    ///          LEVEL_31 30 (cookie at [1][1]). Then the beginning window shows only if CurrentDialogueId &lt; 0 or
    ///          the day has alwaysShow; with no windows startLevel() is called.
    /// Hidden object (init): LEVEL_4 3 force=true, LEVEL_18 17, LEVEL_32 31.
    /// Dessert (init, not in extras mode): LEVEL_11 10, LEVEL_25 24 force=true.
    /// </code>
    ///
    /// <para><b>Trigger actions</b> (TutorialActions.*; info keys in braces)</para>
    /// <code>
    /// Tutorial  clicked_ok            Next button (sent before the bubble advances/hides)
    /// Match-3   customer_in {position}         TCustomer.tweenIn finished (only if gameScreen.ready); position = slot 0..2
    ///           special_customer_in {position} right after customer_in when the customer is the critic (avatar 13)
    ///           highlight {col,row}   tweenHighlighted while dispatchHighlightPosition &gt; 0 (the first 3 tiles)
    ///           matched               swap finished with a match (after the LEVEL_26 dc 3 frozen-tile check)
    ///           start_torch           swap finished and one of the tiles is a torch (type 15)
    ///           dish_completed        TCustomer.setDishCompleteState
    ///           clicked_coin          customer's money collected (touchCoin, after the juice/fries priority check)
    ///           changed_priority      TCustomer.selectPriorCust when the focus actually changes
    ///           clicked_beverage      any touch on the juice while it is visible; offered_beverage: juice dropped on a customer
    ///           clicked_fries         any touch on the fries while visible;     offered_fries: fries dropped on a customer
    ///           give_cookies          cookie patience tween finished
    /// Kitchen   clicked_recipe_shop   RECIPE_ID clicked;   clicked_ingredients: SHOP_ID clicked
    ///           clicked_hardware      HARDWARE_ID clicked (only the quad handler sends it, the bubble-sprite one doesn't)
    ///           right_button          RIGHT_BUTTON clicked; then if CurrentDialogueId == AFTER_LEVEL_2 &amp;&amp; DialogueCounter == 2
    ///                                 the item does not move
    ///           buy_item              buyItem() (not for the "Info" case), before paying
    ///           Every kitchen button first checks !IsButtonShowing and CanClickThisButton(id); swipe uses id -1.
    /// HO        get_correct_ho_item   HOClass.getCorrectItem;   used_hint: revealItem when an item can be revealed
    /// Dessert   touch_ingredient {id} touchIngredient after !IsButtonShowing and CanClickThisButton(id)
    ///           check_times_up        start of checkTimesUp()
    /// </code>
    ///
    /// <para><b>State the screens read</b> (original checks)</para>
    /// <code>
    /// IsButtonShowing: match-3 update + tile touch, kitchen buttons/hover/swipe, HO timer, dessert ingredient touch,
    ///                  window close: startLevel only if the Next button isn't showing (match-3, dessert).
    /// IsShowing:       match-3 timer, dessert timer + patience pause, HO beginning window skipped.
    /// Match-3 touch:   blocked at LEVEL_1 dc 1/7, LEVEL_12 dc 4/5, LEVEL_17 dc 4/5; swaps blocked at LEVEL_26 dc &lt; 3,
    ///                  LEVEL_26 dc 3 unless IsHighlightedTile(lastCol,lastRow), LEVEL_27 dc 0, LEVEL_27 dc 2 unless the
    ///                  torch is swapped upwards, LEVEL_31 dc &lt;= 2. highlightPossible at LEVEL_1 only types 1 and 2.
    /// TCustomer:       LEVEL_31 dc 0 halves customer 0's patience; tweenIn after tweenOut only if !BlockCustomerIn.
    /// HO:              item clicks ignored at LEVEL_4 dc &lt; 4; hint ignored at LEVEL_5 dc &lt; 5.
    /// Dessert:         LEVEL_11 stops the timers, fixes the order to 0/1/3/4/6, customer char_2 after dc 10.
    /// </code>
    /// Shown tutorials are marked in <c>PlayerProfile.tutorialShowed</c> (not saved here; the screens save as before).
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class TutorialManager : MonoBehaviour
    {
        // Layout constants of Tutorial.init()
        public const float TextBoxScaleX = 1.2f;
        public const float ButtonOffsetY = 110;

        [SerializeField] RectTransform textBox;
        [SerializeField] RectTransform avatar;
        [SerializeField] RectTransform arrow;
        [SerializeField] Button nextButton;
        [SerializeField] TextMeshProUGUI nextLabel;
        [SerializeField] TextMeshProUGUI text;
        [Tooltip("Keep this object directly below the scene's Popups layer (windows and their black screen cover the bubble, like the original)")]
        [SerializeField] bool keepBelowPopups = true;

        public static TutorialManager Current { get; private set; }

        public ITutorialHost Host { get; set; }

        public int CurrentDialogueId { get; private set; } = TutorialIds.None;
        public int DialogueCounter { get; private set; }
        public bool IsShowing => textBox != null && textBox.gameObject.activeSelf;
        public bool IsButtonShowing => nextButton != null && nextButton.gameObject.activeSelf;
        public bool BlockCustomerIn { get; set; }
        public int HighlightX { get; private set; }
        public int HighlightY { get; private set; }
        public IReadOnlyList<Vector2Int> HighlightedTiles => highlighted;

        public event Action<int, int> LineShown;
        public event Action StartLevelLineReached;
        public event Action<int> DialogueFinished;
        public event Action<TutorialCommand> CommandIssued;

        // highlightedAr (LEVEL_26) and highlightedAr2 (every "highlight")
        List<Vector2Int> highlighted = new List<Vector2Int>();
        List<Vector2Int> highlighted2 = new List<Vector2Int>();

        // The original wrote these into the dialogue XML (global, kept for the whole run). The data asset must not
        // change at runtime, so they live here: (dialogue, line) -> value, for the lifetime of the application.
        static readonly Dictionary<(int, int), int> arrowDirectionOverrides = new Dictionary<(int, int), int>();
        static readonly Dictionary<(int, int), string> arrowYOverrides = new Dictionary<(int, int), string>();

        /// <summary>Forgets the runtime changes the rules made to dialogue lines (tests).</summary>
        public static void ResetRuntimeOverrides()
        {
            arrowDirectionOverrides.Clear();
            arrowYOverrides.Clear();
        }

        // bubble layout of the line being shown (textBox.x/y; processArrowPosition may move it)
        float boxX, boxY;

        // ------------------------------------------------------------------ lifecycle

        void Awake()
        {
            if (nextButton != null) nextButton.onClick.AddListener(OnNextButton);
            HideBubble();
            if (keepBelowPopups) PlaceBelowPopups();
        }

        void OnEnable() => Current = this;

        void OnDisable()
        {
            if (Current == this) Current = null;
        }

        void OnDestroy()
        {
            if (arrow != null) arrow.KillTweens();
        }

        /// <summary>bringTutorialToTop + the black screen above it: just below the Popups layer among our siblings.</summary>
        void PlaceBelowPopups()
        {
            var parent = transform.parent;
            if (parent == null) return;
            for (int i = 0; i < parent.childCount; i++)
            {
                var c = parent.GetChild(i);
                if (c == transform || c.GetComponent<PopupManager>() == null) continue;
                int mine = transform.GetSiblingIndex();
                transform.SetSiblingIndex(mine < i ? i - 1 : i);
                return;
            }
        }

        // ------------------------------------------------------------------ data

        static DialogueLine GetLine(int id, int line)
        {
            var db = GameDatabase.Instance != null ? GameDatabase.Instance.dialogues : null;
            if (db == null || id < 0 || id >= db.dialogues.Count) return null;
            var lines = db.dialogues[id].lines;
            return line >= 0 && line < lines.Count ? lines[line] : null;
        }

        string ArrowY(int id, int line, DialogueLine l) =>
            arrowYOverrides.TryGetValue((id, line), out var v) ? v : l.arrowY;

        int ArrowDirection(int id, int line, DialogueLine l) =>
            arrowDirectionOverrides.TryGetValue((id, line), out var v) ? v : l.arrowDirection;

        /// <summary>The dialogue line currently set (null if none). Read-only data: don't modify it.</summary>
        public DialogueLine CurrentLine => GetLine(CurrentDialogueId, DialogueCounter);

        // ------------------------------------------------------------------ checkTut / show / next / hide

        /// <summary>
        /// checkTut: shows tutorial <paramref name="id"/> if the player has passed exactly <paramref name="day"/> days,
        /// the screen is the kitchen or the played day is <paramref name="day"/>, and it wasn't shown yet (or
        /// <paramref name="force"/>). Marks it in tutorialShowed. <paramref name="day"/> is the 0-based day, like
        /// GameSession.currentPlayedLevel.
        /// </summary>
        public bool CheckTut(int id, int day, bool force = false)
        {
            var p = GameManager.Player;
            if (p == null) return false;
            bool kitchen = Host != null && Host.IsKitchen;
            int playedDay = GameManager.Instance != null ? GameManager.Instance.Session.currentPlayedLevel : -1;
            bool shown = id >= 0 && id < p.tutorialShowed.Count && p.tutorialShowed[id];
            if (p.passedDays == day && (kitchen || playedDay == day) && (!shown || force))
            {
                Show(id);
                if (id >= 0 && id < p.tutorialShowed.Count) p.tutorialShowed[id] = true;
                return true;
            }
            return false;
        }

        /// <summary>showTutorial: displays line <paramref name="line"/> of dialogue <paramref name="id"/>.</summary>
        public void Show(int id, int line = 0)
        {
            CurrentDialogueId = id;
            DialogueCounter = line;
            Hide();
            var l = GetLine(id, line);
            if (l == null)
            {
                Debug.LogWarning($"[Rory] Tutorial: dialogue {id} has no line {line}");
                return;
            }
            // logic keys on the source data, never on the displayed (possibly localized) text
            if (l.text == StartLevelText)
            {
                Issue(new TutorialCommand(TutorialCommandType.StartLevel, 3, fromStartLevelLine: true));
                StartLevelLineReached?.Invoke();
                return;
            }

            string arrowY = ArrowY(id, line, l);
            textBox.gameObject.SetActive(true);
            avatar.gameObject.SetActive(true);
            if (!string.IsNullOrEmpty(arrowY)) arrow.gameObject.SetActive(true);
            if (l.proceedType == "button") nextButton.gameObject.SetActive(true);
            text.gameObject.SetActive(true);

            boxX = l.posX;
            boxY = l.posY;
            // processArrowPosition may move the text box (HOITEMX, TILEY) and change the arrow direction (HOITEMX)
            float ax = ProcessArrowPosition(l.arrowX);
            float ay = ProcessArrowPosition(arrowY);
            textBox.SetStagePosition(boxX, boxY);
            arrow.SetStagePosition(ax, ay);
            nextButton.GetComponent<RectTransform>().SetStagePosition(boxX, boxY + ButtonOffsetY);
            var boxSize = textBox.rect.size;
            float pivotX = boxSize.x / 2, pivotY = boxSize.y / 2;
            avatar.SetStagePosition(boxX - pivotX * TextBoxScaleX + 5, boxY + pivotY - 5);
            text.rectTransform.SetStagePosition(boxX, boxY);

            int dir = ArrowDirection(id, line, l);
            var s = arrow.localScale;
            arrow.localScale = new Vector3(dir, s.y, s.z);
            TweenArrow(dir != 1);

            text.text = DisplayText(id, line, l);
            if (nextLabel != null) nextLabel.text = ButtonLabel(id, line, l);
            LineShown?.Invoke(id, line);
        }

        /// <summary>Source text of a line that starts the match-3 level instead of showing a bubble.</summary>
        public const string StartLevelText = "(start level)";

        /// <summary>
        /// The one place that turns a line's text into what the bubble shows: "(player)" becomes the player's name and,
        /// on match-3 (<see cref="ITutorialHost.JuicePercentageText"/> not null), "(juice percentage)" the juicer
        /// text; first occurrence only, like AS3 String.replace. Localization hooks in here (look the text up by
        /// <paramref name="id"/>/<paramref name="line"/> before the substitutions).
        /// </summary>
        public string DisplayText(int id, int line, DialogueLine l)
        {
            string body = l.text ?? "";
            body = ReplaceFirst(body, "(player)", GameManager.Player != null ? GameManager.Player.name : "");
            var juice = Host?.JuicePercentageText;
            if (juice != null) body = ReplaceFirst(body, "(juice percentage)", juice);
            return body;
        }

        /// <summary>The one place that turns a line's buttonText into the Next button label (localization hook).</summary>
        public string ButtonLabel(int id, int line, DialogueLine l) => l.buttonText ?? "";

        /// <summary>nextTutorial: next line, or ends the dialogue (CurrentDialogueId = -1) after the last one.</summary>
        public void Next()
        {
            DialogueCounter++;
            if (GetLine(CurrentDialogueId, DialogueCounter) != null)
            {
                Show(CurrentDialogueId, DialogueCounter);
                return;
            }
            int finished = CurrentDialogueId;
            CurrentDialogueId = TutorialIds.None;
            Hide();
            if (finished >= 0) DialogueFinished?.Invoke(finished);
        }

        /// <summary>hideTutorial: hides the bubble; the dialogue stays current (waiting for an action).</summary>
        public void Hide() => HideBubble();

        /// <summary>Hides the bubble and forgets the dialogue (TWindowDisplay.removeWindow when leaving a screen).</summary>
        public void Abort()
        {
            HideBubble();
            CurrentDialogueId = TutorialIds.None;
        }

        void HideBubble()
        {
            if (textBox != null) textBox.gameObject.SetActive(false);
            if (avatar != null) avatar.gameObject.SetActive(false);
            if (arrow != null)
            {
                arrow.gameObject.SetActive(false);
                arrow.KillTweens();
            }
            if (nextButton != null) nextButton.gameObject.SetActive(false);
            if (text != null) text.gameObject.SetActive(false);
        }

        /// <summary>touchButton: "clicked_ok", then the next line, or just hide on a blockNext line.</summary>
        void OnNextButton()
        {
            Trigger(TutorialActions.ClickedOk);
            var l = GetLine(CurrentDialogueId, DialogueCounter);
            if (l == null || !l.blockNext) Next();
            else Hide();
        }

        // ------------------------------------------------------------------ arrow

        /// <summary>processArrowPosition: a number, or a token resolved from the screen (with its text-box side effects).</summary>
        int ProcessArrowPosition(string value)
        {
            switch (value)
            {
                case "HOITEMX":
                {
                    if (Host == null || !Host.TryGetFirstListedItem(out var item, out _)) return 0;
                    int x = (int)(item.x - 40);
                    int y = (int)item.y;
                    boxY = y > 300 ? 250 : 500;
                    if (x > 512) boxX = 335;
                    else
                    {
                        boxX = 670;
                        arrowDirectionOverrides[(TutorialIds.LEVEL_4, 4)] = -1;
                        x += 100;
                    }
                    return x;
                }
                case "HOITEMY":
                {
                    if (Host == null || !Host.TryGetFirstListedItem(out var item, out var height)) return 0;
                    return (int)(item.y + height / 2);
                }
                case "TILEX":
                    return HighlightX;
                case "TILEY":
                    boxY = HighlightY > 300 ? 250 : 500;
                    return HighlightY;
                case "TORCHX":
                case "COOKIEX":
                    return (int)(Tile(1, 1).x + 40);
                case "TORCHY":
                case "COOKIEY":
                    return (int)Tile(1, 1).y;
                default:
                    return ToInt(value);
            }
        }

        /// <summary>tweenArrow: 0.6 s to scale (0.8, 0.95) and 40 px along the arrow, 0.5 s back, forever.</summary>
        void TweenArrow(bool reverse)
        {
            float s = reverse ? -1 : 1;
            float toX = arrow.anchoredPosition.x - 40 * s;
            Vector3 fromScale = default;
            float fromX = 0;
            Tweens.To(arrow, 0.6f, p =>
            {
                arrow.localScale = new Vector3(Mathf.LerpUnclamped(fromScale.x, 0.8f * s, p), Mathf.LerpUnclamped(fromScale.y, 0.95f, p), 1);
                arrow.anchoredPosition = new Vector2(Mathf.LerpUnclamped(fromX, toX, p), arrow.anchoredPosition.y);
            }, () =>
            {
                fromScale = arrow.localScale;
                fromX = arrow.anchoredPosition.x;
            }).OnComplete(() =>
            {
                float backX = arrow.anchoredPosition.x + 40 * s;
                Vector3 fromScale2 = default;
                float fromX2 = 0;
                Tweens.To(arrow, 0.5f, p =>
                {
                    arrow.localScale = new Vector3(Mathf.LerpUnclamped(fromScale2.x, s, p), Mathf.LerpUnclamped(fromScale2.y, 1, p), 1);
                    arrow.anchoredPosition = new Vector2(Mathf.LerpUnclamped(fromX2, backX, p), arrow.anchoredPosition.y);
                }, () =>
                {
                    fromScale2 = arrow.localScale;
                    fromX2 = arrow.anchoredPosition.x;
                }).OnComplete(() => TweenArrow(reverse));
            });
        }

        // ------------------------------------------------------------------ rules (onTriggerNext)

        public void Trigger(string action, string key, object value) =>
            Trigger(action, new Dictionary<string, object> { [key] = value });

        /// <summary>TutorialEvent.TRIGGER_NEXT: the screen reports <paramref name="action"/> (see the table above).</summary>
        public void Trigger(string action, IReadOnlyDictionary<string, object> info = null)
        {
            if (CurrentDialogueId == TutorialIds.None) return;

            if (action == TutorialActions.Highlight) HighlightForArrow(info);

            int dc = DialogueCounter;
            switch (CurrentDialogueId)
            {
                case TutorialIds.LEVEL_1:
                    if (action == "clicked_ok" && dc == 1) Issue(new TutorialCommand(TutorialCommandType.StartLevel, 1));
                    else if (action == "customer_in" && dc == 1) Next();
                    else if (action == "clicked_ok" && dc == 2) Issue(new TutorialCommand(TutorialCommandType.HighlightPossibleMove));
                    else if (action == "matched" && dc == 3) Next();
                    else if (action == "dish_completed" && dc == 4)
                    {
                        if (IsShowing) Hide();
                        Next();
                    }
                    else if (action == "clicked_coin" && dc == 5)
                    {
                        BlockCustomerIn = true;
                        Next();
                    }
                    else if (action == "clicked_ok" && dc == 6)
                    {
                        BlockCustomerIn = false;
                        Issue(new TutorialCommand(TutorialCommandType.ClearHasWindow));
                        Issue(new TutorialCommand(TutorialCommandType.SendCustomersOutSatisfied));
                    }
                    else if (action == "customer_in" && InfoNumber(info, TutorialActions.KeyPosition) == 2 && dc == 6) Next();
                    else if (action == "changed_priority" && dc == 7) Next();
                    break;

                case TutorialIds.AFTER_LEVEL_2:
                    if (action == "clicked_recipe_shop" && dc == 1) Next();
                    else if (action == "right_button" && dc == 3) Next();
                    else if (action == "buy_item" && dc == 4) Next();
                    break;

                case TutorialIds.LEVEL_4:
                    if (action == "get_correct_ho_item" && dc == 4) Next();
                    else if (action == "used_hint" && dc == 5) Hide();
                    else if (action == "get_correct_ho_item" && dc == 5) Next();
                    break;

                case TutorialIds.LEVEL_5:
                    if (action == "clicked_ok" && dc == 2) StartLevel();
                    break;

                case TutorialIds.LEVEL_7:
                    if (action == "clicked_ok" && dc == 2) StartLevel();
                    else if (action == "special_customer_in" && dc == 2)
                    {
                        // the original rewrote the XML: dialogue[LEVEL_7].text[3].@arrowY = 120 + position * 118
                        double y = 120 + InfoNumber(info, TutorialActions.KeyPosition) * 118;
                        arrowYOverrides[(TutorialIds.LEVEL_7, 3)] = y.ToString(CultureInfo.InvariantCulture);
                        Next();
                    }
                    break;

                case TutorialIds.LEVEL_8:
                    if (action == "clicked_ok" && dc == 1) StartLevel();
                    break;

                case TutorialIds.AFTER_LEVEL_8:
                    if (action == "clicked_ingredients" && dc == 2) Next();
                    else if (action == "buy_item" && dc == 8) Next();
                    break;

                case TutorialIds.LEVEL_11:
                    if (action == "clicked_ok" && dc == 4) Issue(new TutorialCommand(TutorialCommandType.StartDessertLevel));
                    else if (action == "check_times_up" && dc == 4) Next();
                    else if (action == "touch_ingredient" && dc == 7 && InfoNumber(info, TutorialActions.KeyId) == 0) Next();
                    else if (action == "touch_ingredient" && dc == 8 && InfoNumber(info, TutorialActions.KeyId) == 1) Next();
                    else if (action == "touch_ingredient" && dc == 9 && InfoNumber(info, TutorialActions.KeyId) == 3) Next();
                    else if (action == "touch_ingredient" && dc == 10 && InfoNumber(info, TutorialActions.KeyId) == 4) Next();
                    else if (action == "touch_ingredient" && dc == 11 && InfoNumber(info, TutorialActions.KeyId) == 6) Hide();
                    else if (action == "check_times_up" && dc == 11) Next();
                    break;

                case TutorialIds.LEVEL_12:
                    if (action == "customer_in" && dc == 0)
                    {
                        Issue(new TutorialCommand(TutorialCommandType.ClearHasWindow));
                        Next();
                    }
                    else if (action == "clicked_ok" && dc == 2) Issue(new TutorialCommand(TutorialCommandType.HighlightPossibleMove));
                    else if (action == "matched" && dc == 3) Next();
                    else if (action == "clicked_beverage" && dc == 4) Next();
                    else if (action == "offered_beverage" && dc == 5) Next();
                    break;

                case TutorialIds.LEVEL_13:
                    if (action == "clicked_ok" && dc == 2) StartLevel();
                    break;

                case TutorialIds.LEVEL_14:
                    if (action == "clicked_ok" && dc == 0) StartLevel();
                    break;

                case TutorialIds.LEVEL_16:
                    if (action == "clicked_ok" && dc == 2) StartLevel();
                    break;

                case TutorialIds.LEVEL_17:
                    if (action == "customer_in" && dc == 0)
                    {
                        Issue(new TutorialCommand(TutorialCommandType.ClearHasWindow));
                        Next();
                    }
                    else if (action == "clicked_ok" && dc == 2) Issue(new TutorialCommand(TutorialCommandType.HighlightPossibleMove));
                    else if (action == "matched" && dc == 3) Next();
                    else if (action == "clicked_fries" && dc == 4) Next();
                    else if (action == "offered_fries" && dc == 5) Next();
                    break;

                case TutorialIds.LEVEL_19:
                    if (action == "clicked_ok" && dc == 0) StartLevel();
                    break;

                case TutorialIds.LEVEL_21:
                    if (action == "clicked_ok" && dc == 0) StartLevel();
                    break;

                case TutorialIds.LEVEL_22:
                    if (action == "clicked_ok" && dc == 1) StartLevel();
                    break;

                case TutorialIds.LEVEL_25:
                    if (action == "clicked_ok" && dc == 0) Issue(new TutorialCommand(TutorialCommandType.StartDessertLevel));
                    break;

                case TutorialIds.LEVEL_26:
                    if (action == "customer_in" && dc == 0)
                    {
                        Issue(new TutorialCommand(TutorialCommandType.ClearHasWindow));
                        Next();
                    }
                    else if (action == "highlight" && dc == 0) FreezeHighlighted(info);
                    else if (action == "clicked_ok" && dc == 2) Issue(new TutorialCommand(TutorialCommandType.HighlightPossibleMove));
                    else if (action == "matched" && dc == 3) Next();
                    break;

                case TutorialIds.LEVEL_27:
                    if (action == "customer_in" && dc == 0) Next();
                    else if (action == "start_torch" && dc == 2)
                    {
                        Next();
                        Issue(new TutorialCommand(TutorialCommandType.ClearHasWindow));
                    }
                    break;

                case TutorialIds.LEVEL_28:
                    if (action == "clicked_ok" && dc == 0) StartLevel();
                    break;

                case TutorialIds.LEVEL_31:
                    if (action == "customer_in" && dc == 0)
                    {
                        Next();
                        Issue(new TutorialCommand(TutorialCommandType.ClearHasWindow));
                    }
                    else if (action == "give_cookies" && dc == 2) Next();
                    break;

                case TutorialIds.AFTER_LEVEL_34:
                    if (action == "clicked_hardware" && dc == 1) Next();
                    break;
            }
        }

        void StartLevel() => Issue(new TutorialCommand(TutorialCommandType.StartLevel, 3));

        void Issue(TutorialCommand command)
        {
            Host?.OnTutorialCommand(command);
            CommandIssued?.Invoke(command);
        }

        /// <summary>
        /// Every "highlight": collects highlightedAr2 and, at 3 tiles, picks the tile to swap (the one out of line)
        /// and sets HighlightX = tile[col][0].x + 40, HighlightY = tile[0][row].y (TILEX/TILEY).
        /// </summary>
        void HighlightForArrow(IReadOnlyDictionary<string, object> info)
        {
            int col = InfoInt(info, TutorialActions.KeyCol);
            int row = InfoInt(info, TutorialActions.KeyRow);
            if (highlighted2.Count >= 3) highlighted2 = new List<Vector2Int>();
            highlighted2.Add(new Vector2Int(col, row));
            if (highlighted2.Count < 3) return;
            var h = highlighted2;
            int c0 = h[0].x, c1 = h[1].x, c2 = h[2].x, r0 = h[0].y, r1 = h[1].y, r2 = h[2].y;
            if (r0 == r1 && r1 == r2)
            {
                if (Mathf.Abs(c0 - c1) == 1) { col = c2; row = r2; }
                else if (Mathf.Abs(c0 - c2) == 1) { col = c1; row = r1; }
                else { col = c0; row = r0; }
            }
            else if (c0 == c1 && c1 == c2)
            {
                if (Mathf.Abs(r0 - r1) == 1) { col = c2; row = r2; }
                else if (Mathf.Abs(r0 - r2) == 1) { col = c1; row = r1; }
                else { col = c0; row = r0; }
            }
            else if (!(r0 == r1 || r0 == r2 || c0 == c1 || c0 == c2)) { col = c0; row = r0; }
            else if (!(r1 == r0 || r1 == r2 || c1 == c0 || c1 == c2)) { col = c1; row = r1; }
            HighlightX = (int)(Tile(col, 0).x + 40);
            HighlightY = (int)Tile(0, row).y;
        }

        /// <summary>LEVEL_26 line 0 "highlight": collects highlightedAr and, at 3 tiles, freezes the one to defrost.</summary>
        void FreezeHighlighted(IReadOnlyDictionary<string, object> info)
        {
            int col = InfoInt(info, TutorialActions.KeyCol);
            int row = InfoInt(info, TutorialActions.KeyRow);
            if (highlighted.Count >= 3) highlighted = new List<Vector2Int>();
            highlighted.Add(new Vector2Int(col, row));
            if (highlighted.Count < 3) return;
            var h = highlighted;
            int c0 = h[0].x, c1 = h[1].x, c2 = h[2].x, r0 = h[0].y, r1 = h[1].y, r2 = h[2].y;
            if (r0 == r1 && r1 == r2)
            {
                if (Mathf.Abs(c0 - c1) == 1) { col = c0; row = r0; }
                else { col = c2; row = r2; }
            }
            else if (c0 == c1 && c1 == c2)
            {
                if (Mathf.Abs(r0 - r1) == 1) { col = c0; row = r0; }
                else { col = c2; row = r2; }
            }
            else if (!(r2 == r0 || r2 == r1 || c2 == c0 || c2 == c1)) { col = c0; row = r0; }
            Issue(new TutorialCommand(TutorialCommandType.FreezeTile, col: col, row: row));
        }

        /// <summary>LEVEL_26 swap check: is (col, row) one of the first three highlighted tiles?</summary>
        public bool IsHighlightedTile(int col, int row)
        {
            for (int i = 0; i < 3 && i < highlighted.Count; i++)
                if (highlighted[i].x == col && highlighted[i].y == row) return true;
            return false;
        }

        // ------------------------------------------------------------------ canClickThisButton

        /// <summary>canClickThisButton: false while the current line waits for a different button (TutorialButtons.*).</summary>
        public bool CanClickThisButton(int buttonId)
        {
            int dc = DialogueCounter;
            switch (CurrentDialogueId)
            {
                case TutorialIds.AFTER_LEVEL_2:
                    if (dc == 1 && buttonId != TutorialButtons.RECIPE_ID) return false;
                    if (dc == 3 && buttonId != TutorialButtons.RIGHT_BUTTON) return false;
                    if (dc == 4 && buttonId != TutorialButtons.BUY_BUTTON) return false;
                    break;
                case TutorialIds.AFTER_LEVEL_8:
                    if (dc == 2 && buttonId != TutorialButtons.SHOP_ID) return false;
                    if (dc == 8 && buttonId != TutorialButtons.BUY_BUTTON) return false;
                    break;
                case TutorialIds.LEVEL_11:
                    if (dc == 7 && buttonId != TutorialButtons.DESSERT_CUP) return false;
                    if (dc == 8 && buttonId != TutorialButtons.DESSERT_YOGURT) return false;
                    if (dc == 9 && buttonId != TutorialButtons.DESSERT_SYRUP) return false;
                    if (dc == 10 && buttonId != TutorialButtons.DESSERT_TOPPING) return false;
                    if (dc == 11 && buttonId != TutorialButtons.DESSERT_BEVERAGE) return false;
                    break;
                case TutorialIds.AFTER_LEVEL_34:
                    if (dc == 1 && buttonId != TutorialButtons.HARDWARE_ID) return false;
                    break;
            }
            return true;
        }

        // ------------------------------------------------------------------ helpers

        Vector2 Tile(int col, int row) =>
            Host != null && Host.TryGetTilePosition(col, row, out var p) ? p : Vector2.zero;

        /// <summary>AS3 Number(info[key]) (NaN -> compares false; missing -> NaN).</summary>
        static double InfoNumber(IReadOnlyDictionary<string, object> info, string key)
        {
            if (info == null || !info.TryGetValue(key, out var v) || v == null) return double.NaN;
            try
            {
                return Convert.ToDouble(v, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return double.NaN;
            }
        }

        /// <summary>AS3 int(info[key]): truncated number, 0 when missing or not a number.</summary>
        static int InfoInt(IReadOnlyDictionary<string, object> info, string key)
        {
            double d = InfoNumber(info, key);
            return double.IsNaN(d) || double.IsInfinity(d) ? 0 : (int)d;
        }

        /// <summary>AS3 int(string): truncated number, 0 for empty or non-numeric text.</summary>
        static int ToInt(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) && !double.IsNaN(d) && !double.IsInfinity(d)
                ? (int)d
                : 0;
        }

        /// <summary>AS3 String.replace with a string pattern: first occurrence only.</summary>
        static string ReplaceFirst(string s, string pattern, string value)
        {
            int i = s.IndexOf(pattern, StringComparison.Ordinal);
            return i < 0 ? s : s.Substring(0, i) + value + s.Substring(i + pattern.Length);
        }
    }
}
