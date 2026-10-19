using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Rory
{
    /// <summary>
    /// Screen transitions: fade to black, load the next scene, wait until its <see cref="ScreenController"/> is
    /// ready, fade back in (the original Game.fadeToScreen: 10 frames out, 10 frames in at 30 fps).
    /// </summary>
    public class SceneFlow : MonoBehaviour
    {
        [SerializeField] float fadeDuration = 0.33f;
        [SerializeField] Color fadeColor = Color.black;

        CanvasGroup fade;
        public bool IsTransitioning { get; private set; }
        public GameScene? Current { get; private set; }
        /// <summary>Target of the latest <see cref="Go"/> call (also when that scene could not be loaded).</summary>
        public GameScene? LastRequested { get; private set; }

        public event Action<GameScene> SceneStarted;

        void Awake()
        {
            var go = new GameObject("Fade", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup), typeof(GraphicRaycaster));
            go.transform.SetParent(transform, false);
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            fade = go.GetComponent<CanvasGroup>();
            fade.alpha = 0;
            fade.blocksRaycasts = false;
            var img = new GameObject("Black", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            img.transform.SetParent(go.transform, false);
            img.color = fadeColor;
            var rt = img.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            if (Enum.TryParse(SceneManager.GetActiveScene().name, out GameScene s)) Current = s;
        }

        /// <summary>Fades out, loads <paramref name="scene"/> and fades in. Ignored while a transition runs.</summary>
        public void Go(GameScene scene)
        {
            if (IsTransitioning) return;
            LastRequested = scene;
            StartCoroutine(Transition(scene));
        }

        IEnumerator Transition(GameScene scene)
        {
            IsTransitioning = true;
            fade.blocksRaycasts = true;
            yield return Fade(0, 1);

            if (!Application.CanStreamedLevelBeLoaded(scene.ToString()))
            {
                // Not converted / not in Build Settings yet: stay on the current screen.
                Debug.LogError($"[Rory] Scene '{scene}' is not in the build");
                yield return Fade(1, 0);
                fade.blocksRaycasts = false;
                IsTransitioning = false;
                yield break;
            }
            var op = SceneManager.LoadSceneAsync(scene.ToString());
            while (!op.isDone) yield return null;
            Current = scene;

            var controller = FindFirstObjectByType<ScreenController>();
            float timeout = 10;
            while (controller != null && !controller.IsReady && timeout > 0)
            {
                timeout -= Time.unscaledDeltaTime;
                yield return null;
            }
            SceneStarted?.Invoke(scene);

            yield return Fade(1, 0);
            fade.blocksRaycasts = false;
            IsTransitioning = false;
            if (controller != null) controller.OnScreenVisible();
        }

        IEnumerator Fade(float from, float to)
        {
            for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                fade.alpha = Mathf.Lerp(from, to, t / fadeDuration);
                yield return null;
            }
            fade.alpha = to;
        }
    }
}
