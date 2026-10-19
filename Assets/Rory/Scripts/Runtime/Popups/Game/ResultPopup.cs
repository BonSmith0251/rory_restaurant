using System;
using Rory.Tweening;
using Rory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Rory.Localization;

namespace Rory.Popups.Game
{
    /// <summary>
    /// Level summary window (TWindowState STORY_*, HO_*, DESSERT_*): title, info line, a left-aligned list of
    /// labels with right-aligned values, an optional tip, one button, the goal / expert badge (showGoalIcon) and
    /// the congratulation particles (psCongrats). Filled by <see cref="GameWindows"/>.
    /// </summary>
    public class ResultPopup : WindowPopup
    {
        [Header("Summary")]
        public TextMeshProUGUI summaryText;
        public TextMeshProUGUI summaryStats;
        public TextMeshProUGUI summaryTips;

        [Header("Goal badge")]
        public Image goalIcon;
        public Image expertBlink;

        [Header("Effects")]
        public UIParticleEmitter congrats;

        /// <summary>True when the badge shows "expert" (only meaningful after <see cref="ShowGoalIcon"/>).</summary>
        public bool Expert { get; private set; }

        /// <summary>The summary labels as written by the original (with tab characters).</summary>
        public string Labels { get; private set; }

        /// <summary>
        /// Tab stop spacing in pixels. The original labels align their colons with tabs ("Customers Served\t\t :"),
        /// rendered by flash.text.TextField at its default tab stops; TextMeshPro takes the tab width from the shared
        /// font asset, so tabs are turned into an explicit &lt;pos&gt; column: each line's tabs advance to the next
        /// multiple of this spacing, and the text after the tabs of every line starts at the furthest such stop, so the
        /// colons form one column as the tabs intended (font metrics differ from Flash, exact stops would not line up).
        /// </summary>
        public const float TabStop = 34;

        internal void Setup(string titleText, string infoText, float infoOffsetY, string labels, string stats, string tips,
            string buttonLabel, Action onButton)
        {
            SetTitle(titleText);
            SetInfo(infoText);
            Offset(info, 0, infoOffsetY);
            Labels = labels;
            summaryText.text = ExpandTabs(summaryText, Loc.T(labels)); // translate before the tabs become <pos> tags
            summaryStats.text = stats;
            summaryTips.text = tips ?? "";
            summaryTips.gameObject.SetActive(tips != null);
            // KITCHEN / MAINMENU / NEXT_LEVEL / ENDING all leave the screen: the window stays up during the fade.
            GameWindows.Bind(leftButton, null, null);
            GameWindows.Bind(rightButton, null, null);
            GameWindows.Bind(uniButton, buttonLabel, onButton);
            goalIcon.gameObject.SetActive(false);
            expertBlink.gameObject.SetActive(false);
        }

        /// <summary>
        /// showGoalIcon(): the goal (or expert) badge at stage (780, 500) zooms from 2x / 50% alpha to 1x in
        /// 0.8 s (Back.easeIn); the expert badge then gets a blinking glow. Plays goal_reached / expert_reached.
        /// </summary>
        public void ShowGoalIcon(bool expert)
        {
            Expert = expert;
            GameWindows.HideGoalIconSmall();
            var lib = GameWindowsLibrary.Instance;
            GameManager.Instance.Audio.PlaySfx(expert ? "expert_reached" : "goal_reached");
            goalIcon.gameObject.SetActive(true);
            goalIcon.sprite = expert ? lib.expert : lib.goal;
            goalIcon.SetNativeSize();
            var t = goalIcon.transform;
            goalIcon.KillTweens();
            t.KillTweens();
            t.localScale = new Vector3(2, 2, 1);
            SetAlpha(goalIcon, 0.5f);
            t.TweenScale(1, 0.8f).SetEase(Ease.BackIn);
            goalIcon.TweenAlpha(1, 0.8f).SetEase(Ease.BackIn).OnComplete(() =>
            {
                if (!expert || expertBlink == null) return;
                expertBlink.gameObject.SetActive(true);
                TweenExpertBlink();
            });
        }

        /// <summary>tweenExpertBlink(): alpha 1 -> 0 -> 1, 0.4 s each, forever.</summary>
        void TweenExpertBlink()
        {
            if (expertBlink == null) return;
            expertBlink.TweenAlpha(0, 0.4f).OnComplete(() =>
            {
                if (expertBlink != null) expertBlink.TweenAlpha(1, 0.4f).OnComplete(TweenExpertBlink);
            });
        }

        /// <summary>psCongrats.start(), paused (emission stopped) after 6 seconds.</summary>
        public void PlayCongrats()
        {
            if (congrats == null) return;
            congrats.Play(6);
        }

        /// <summary>Replaces each tab by a &lt;pos&gt; tag at the next multiple of <see cref="TabStop"/>.</summary>
        public static string ExpandTabs(TMP_Text text, string s)
        {
            if (string.IsNullOrEmpty(s) || s.IndexOf('\t') < 0) return s;
            var lines = s.Split('\n');
            // the stop each line's tabs reach (labels only have tabs between the name and the colon)
            float column = 0;
            foreach (var line in lines)
            {
                var parts = line.Split('\t');
                if (parts.Length < 2) continue;
                float x = Width(text, parts[0]);
                for (int i = 1; i < parts.Length; i++)
                {
                    x = (Mathf.Floor(x / TabStop) + 1) * TabStop;
                    if (i < parts.Length - 1) x += Width(text, parts[i]);
                }
                column = Mathf.Max(column, x);
            }
            var pos = "<pos=" + column.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + ">";
            var sb = new System.Text.StringBuilder();
            for (int l = 0; l < lines.Length; l++)
            {
                if (l > 0) sb.Append('\n');
                int tab = lines[l].IndexOf('\t');
                if (tab < 0)
                {
                    sb.Append(lines[l]);
                    continue;
                }
                sb.Append(lines[l], 0, tab).Append(pos).Append(lines[l].Substring(tab).Replace("\t", ""));
            }
            return sb.ToString();
        }

        static float Width(TMP_Text text, string s) => string.IsNullOrEmpty(s) ? 0 : text.GetPreferredValues(s, 10000, 10000).x;

        static void SetAlpha(Graphic g, float a)
        {
            var c = g.color;
            c.a = a;
            g.color = c;
        }

        protected override void OnClosing()
        {
            goalIcon.KillTweens();
            goalIcon.transform.KillTweens();
            expertBlink.KillTweens();
        }
    }
}
