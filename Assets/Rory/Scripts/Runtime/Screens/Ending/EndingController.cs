using Rory.Screens.Opening;
using Rory.Tweening;
using Rory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Screens.Ending
{
    /// <summary>
    /// Ending story (EndingClass): nine slides advanced by tapping anywhere, then back to the kitchen.
    /// Entering the screen marks the ending as viewed and saves.
    /// </summary>
    public class EndingController : ScreenController
    {
        [SerializeField] Button touchArea;
        [SerializeField] Image secondSlide;
        [SerializeField] Image smile;
        [SerializeField] Image wonder;
        [SerializeField] Image bubble;
        [SerializeField] Image restaurant;
        [SerializeField] Image toFrench;
        [SerializeField] Image higherEdu;
        [SerializeField] Image finalSlide;
        [SerializeField] TextMeshProUGUI textField;
        [SerializeField] StoryBubble speech;

        public const int LastStory = 9;

        public int CurrentStory { get; private set; }
        public bool CanProceed { get; private set; }

        void Start()
        {
            if (Player != null)
            {
                Player.endingViewed = true;
                Game.Saves.Save();
            }
            touchArea.onClick.AddListener(NextStory);
            textField.text = "And there she goes, Rory’s little restaurant gets more crowded day by day.";
            CurrentStory = 0;
            CanProceed = true;
        }

        public void NextStory()
        {
            if (!CanProceed) return;
            CurrentStory++;
            CanProceed = false;
            speech.Hide();
            switch (CurrentStory)
            {
                case 1:
                    FadeIn(secondSlide, 1, Ease.Linear, () =>
                    {
                        textField.text = "One night, Rory is counting her profit.";
                        Proceed();
                    });
                    break;
                case 2:
                    speech.MoveToX(X(smile));
                    Tweens.DelayedCall(1, () => speech.Show("30.."), this);
                    Tweens.DelayedCall(2, () => speech.Show("30..40.."), this);
                    Tweens.DelayedCall(3, () =>
                    {
                        speech.Show("30..40..50..");
                        Proceed();
                    }, this);
                    break;
                case 3:
                    smile.gameObject.SetActive(true);
                    smile.rectTransform.SetStagePosition(1024 + W(smile), Y(smile));
                    smile.rectTransform.TweenAnchoredX(1024 - W(smile), 1).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        speech.MoveToX(X(smile) + 100);
                        speech.Show("50 thousand dollars and more! I have earned more than enough to pay off my Student Loan!");
                        textField.text = "Thanks to you, now she has enough money to pay her debts!";
                        Proceed();
                    });
                    break;
                case 4:
                    textField.text = "But what goal will she pursue next? What does Rory dream of now?";
                    wonder.rectTransform.SetStagePosition(-W(wonder), Y(wonder));
                    wonder.gameObject.SetActive(true);
                    SetAlpha(bubble, 0);
                    bubble.gameObject.SetActive(true);
                    wonder.rectTransform.TweenAnchoredX(0, 1).SetEase(Ease.Linear).OnComplete(() =>
                        bubble.TweenAlpha(1, 0.5f).SetEase(Ease.Linear).OnComplete(Proceed));
                    break;
                case 5:
                    // The AS3 passed the Linear class (not an ease function): TweenLite's default Quad.easeOut applies.
                    FadeIn(restaurant, 1, Ease.QuadOut, () =>
                    {
                        textField.text = "Own a Grand restaurant.";
                        Proceed();
                    });
                    break;
                case 6:
                    restaurant.TweenAlpha(0, 0.7f).SetEase(Ease.Linear);
                    FadeIn(toFrench, 1, Ease.Linear, () =>
                    {
                        textField.text = "Maybe start a living in French.";
                        restaurant.gameObject.SetActive(false);
                        Proceed();
                    });
                    break;
                case 7:
                    toFrench.TweenAlpha(0, 0.7f).SetEase(Ease.Linear);
                    FadeIn(higherEdu, 1, Ease.Linear, () =>
                    {
                        textField.text = "Or pursue higher culinary education.";
                        toFrench.gameObject.SetActive(false);
                        Proceed();
                    });
                    break;
                case 8:
                    finalSlide.rectTransform.SetStagePosition(X(finalSlide), 600);
                    finalSlide.gameObject.SetActive(true);
                    // Default ease again (see slide 5).
                    finalSlide.rectTransform.TweenAnchoredY(0, 1.5f).SetEase(Ease.QuadOut).OnComplete(() =>
                    {
                        textField.text = "Anyway now she already owns a very successful restaurant, let's help her grow it even bigger!";
                        Proceed();
                    });
                    break;
                case LastStory:
                    Session.coinsGot = 0;
                    GoTo(GameScene.Kitchen);
                    break;
            }
        }

        void FadeIn(Image img, float duration, EaseFunction ease, System.Action done)
        {
            SetAlpha(img, 0);
            img.gameObject.SetActive(true);
            img.TweenAlpha(1, duration).SetEase(ease).OnComplete(done);
        }

        void Proceed() => CanProceed = true;

        static float W(Image img) => img.rectTransform.rect.width;
        static float X(Image img) => img.rectTransform.GetStagePosition().x;
        static float Y(Image img) => img.rectTransform.GetStagePosition().y;

        static void SetAlpha(Graphic g, float a)
        {
            var c = g.color;
            c.a = a;
            g.color = c;
        }
    }
}
