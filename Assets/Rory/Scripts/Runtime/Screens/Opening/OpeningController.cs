using Rory.Tweening;
using Rory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Screens.Opening
{
    /// <summary>
    /// Intro story (OpeningScreen): seven slides advanced by tapping anywhere. The last tap marks the opening as
    /// viewed, saves and goes to the kitchen.
    /// </summary>
    public class OpeningController : ScreenController
    {
        [SerializeField] Button touchArea;
        [SerializeField] Image mask;
        [SerializeField] Image smile;
        [SerializeField] Image cry;
        [SerializeField] Image think;
        [SerializeField] Image studentLoan;
        [SerializeField] Image brosur;
        [SerializeField] Image lastSlide;
        [SerializeField] Image lightBulb;
        [SerializeField] TextMeshProUGUI textField;
        [SerializeField] StoryBubble bubble;

        public const int LastStory = 7;

        /// <summary>Slide shown (0..6); 7 = finished.</summary>
        public int CurrentStory { get; private set; }
        /// <summary>False while a slide animates (taps are ignored).</summary>
        public bool CanProceed { get; private set; }

        void Start()
        {
            touchArea.onClick.AddListener(NextStory);
            textField.text = "One early morning, our sweet Rory is checking up on her mails..";
            CurrentStory = 0;
            CanProceed = true;
        }

        public void NextStory()
        {
            if (!CanProceed) return;
            CurrentStory++;
            CanProceed = false;
            bubble.Hide();
            switch (CurrentStory)
            {
                case 1:
                    textField.text = "She just received her Chef Certificate after graduating from Culinary School.";
                    smile.rectTransform.SetStagePosition(1200, Y(smile));
                    smile.gameObject.SetActive(true);
                    smile.rectTransform.TweenAnchoredX(1024 - W(smile), 0.6f).SetEase(Ease.Linear).OnComplete(Proceed);
                    break;
                case 2:
                    textField.text = "But wait, what is that small note attached below?";
                    SetAlpha(mask, 0);
                    mask.gameObject.SetActive(true);
                    mask.TweenAlpha(1, 0.7f).SetEase(Ease.Linear).OnComplete(Proceed);
                    break;
                case 3:
                    textField.text = "Ow, Rory has a huge debt from her Student Loan!";
                    studentLoan.rectTransform.SetStagePosition(-W(studentLoan), Y(studentLoan));
                    SetAlpha(cry, 0);
                    studentLoan.gameObject.SetActive(true);
                    cry.gameObject.SetActive(true);
                    studentLoan.rectTransform.TweenAnchoredX(0, 0.7f).SetEase(Ease.Linear);
                    cry.TweenAlpha(1, 1.4f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        smile.gameObject.SetActive(false);
                        Proceed();
                    });
                    break;
                case 4:
                    textField.text = "And what’s this? A new restaurant just started out its business.. near Rory’s place!";
                    SetAlpha(brosur, 0);
                    brosur.gameObject.SetActive(true);
                    brosur.TweenAlpha(1, 1).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        studentLoan.gameObject.SetActive(false);
                        Proceed();
                    });
                    break;
                case 5:
                    textField.text = "Seems like Rory has got an idea to solve her financial problem.";
                    SetAlpha(think, 0);
                    think.gameObject.SetActive(true);
                    bubble.Show("HMM...");
                    think.TweenAlpha(1, 0.7f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        cry.gameObject.SetActive(false);
                        Proceed();
                    });
                    break;
                case 6:
                    lastSlide.rectTransform.SetStagePosition(1024, Y(lastSlide));
                    lastSlide.gameObject.SetActive(true);
                    lastSlide.rectTransform.TweenAnchoredX(0, 0.5f).SetEase(Ease.Linear);
                    Tweens.DelayedCall(1.2f, () =>
                    {
                        bubble.MoveTo(450, 300);
                        bubble.Show("I’ll open a mini restaurant to pay off my debts!");
                        textField.text = "Genius idea! Rory now has a sound plan to pay back her debts!";
                        CanProceed = true;
                        lightBulb.gameObject.SetActive(true);
                    }, this);
                    break;
                case LastStory:
                    bubble.Stop();
                    if (Player != null)
                    {
                        Player.openingViewed = true;
                        Game.Saves.Save();
                    }
                    GoTo(GameScene.Kitchen);
                    break;
            }
        }

        void Proceed() => CanProceed = true;

        static float W(Image img) => img.rectTransform.rect.width;
        static float Y(Image img) => img.rectTransform.GetStagePosition().y;

        static void SetAlpha(Graphic g, float a)
        {
            var c = g.color;
            c.a = a;
            g.color = c;
        }
    }
}
