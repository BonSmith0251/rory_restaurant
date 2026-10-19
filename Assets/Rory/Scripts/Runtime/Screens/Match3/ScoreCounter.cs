using TMPro;

namespace Rory.Screens.Match3
{
    /// <summary>
    /// ScoreManager: the day's earnings (totalScore) and the "$ 1,234" counter that rolls up to it, one step per frame:
    /// shown = int(shown + 5 + 5 * (total - shown) / 100), capped at the total.
    /// </summary>
    public class ScoreCounter
    {
        readonly TMP_Text display;

        /// <summary>ScoreManager.totalScore.</summary>
        public int Total { get; private set; }
        /// <summary>ScoreManager.score (the displayed value).</summary>
        public int Shown { get; private set; }
        public bool Updating { get; private set; }

        public ScoreCounter(TMP_Text display)
        {
            this.display = display;
            Reset();
        }

        public void Reset()
        {
            Total = 0;
            Shown = 0;
            if (display != null) display.text = "$ 0";
        }

        public void Add(int amount)
        {
            Total += amount;
            Updating = true;
        }

        /// <summary>Cheat: totalScore = expert target.</summary>
        public void SetTotal(int value) => Total = value;

        /// <summary>updateDisplayedScore, once per original frame.</summary>
        public void Tick()
        {
            if (Shown < Total)
            {
                Shown = (int)(Shown + 5 + 5.0 * (Total - Shown) / 100);
                Show();
            }
            else
            {
                Updating = false;
            }
            if (Shown > Total)
            {
                Shown = Total;
                Show();
            }
        }

        void Show()
        {
            if (display != null) display.text = "$ " + GameRules.AddCommas(Shown);
        }
    }
}
