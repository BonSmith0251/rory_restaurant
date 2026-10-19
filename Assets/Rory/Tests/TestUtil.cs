using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Rory.Tests
{
    /// <summary>Helpers for the PlayMode tests: scene loading, finding UI by name, clicking and screen captures.</summary>
    public static class TestUtil
    {
        /// <summary>Captures go to &lt;project&gt;/Logs/Captures/&lt;name&gt;.png (1024x600).</summary>
        public static string CaptureFolder => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Logs", "Captures"));

        public static string TempSaveFolder()
        {
            var dir = Path.Combine(Path.GetTempPath(), "RoryTests", System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            return dir;
        }

        /// <summary>Loads a scene directly (no fade) with fresh saves in a temporary folder.</summary>
        public static IEnumerator LoadScene(GameScene scene, bool freshSaves = true)
        {
            if (freshSaves) GameManager.Instance.UseSaveFolder(TempSaveFolder());
            SceneManager.LoadScene(scene.ToString());
            yield return null;
            yield return null;
        }

        public static IEnumerator Frames(int n)
        {
            for (int i = 0; i < n; i++) yield return null;
        }

        public static IEnumerator Seconds(float s)
        {
            float end = Time.realtimeSinceStartup + s;
            while (Time.realtimeSinceStartup < end) yield return null;
        }

        /// <summary>Active GameObject by name (searches loaded scenes, including inactive objects if asked).</summary>
        public static GameObject Find(string name, bool includeInactive = false) =>
            Object.FindObjectsByType<Transform>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .Where(t => t.name == name).Select(t => t.gameObject).FirstOrDefault();

        public static T Find<T>(string name, bool includeInactive = false) where T : Component
        {
            var go = Find(name, includeInactive);
            return go != null ? go.GetComponent<T>() : null;
        }

        /// <summary>
        /// Clicks a button by GameObject name (fails if missing, inactive or not interactable). While popups are
        /// open the top-most popup is searched first, like a real click would hit it.
        /// </summary>
        public static void Click(string name)
        {
            Button b = null;
            var popups = Popups.PopupManager.Current;
            if (popups != null && popups.HasOpenPopup)
                b = popups.Open[popups.Open.Count - 1].GetComponentsInChildren<Button>().FirstOrDefault(x => x.name == name);
            if (b == null) b = Find<Button>(name);
            NUnit.Framework.Assert.IsNotNull(b, $"No active button '{name}'");
            NUnit.Framework.Assert.IsTrue(b.IsInteractable(), $"Button '{name}' is not interactable");
            b.onClick.Invoke();
        }

        /// <summary>Clicks the first active button whose label (or own text) is <paramref name="label"/>.</summary>
        public static void ClickLabel(string label)
        {
            var popups = Popups.PopupManager.Current;
            var scope = popups != null && popups.HasOpenPopup
                ? popups.Open[popups.Open.Count - 1].GetComponentsInChildren<Button>()
                : Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
            var b = scope.FirstOrDefault(x => x.GetComponentsInChildren<TMP_Text>().Any(t => t.text == label));
            NUnit.Framework.Assert.IsNotNull(b, $"No active button labelled '{label}'");
            b.onClick.Invoke();
        }

        /// <summary>Renders every canvas into a 1024x600 PNG.</summary>
        public static string Capture(string name)
        {
            Directory.CreateDirectory(CaptureFolder);
            const int w = 1024, h = 600;
            var rt = new RenderTexture(w, h, 24);
            var camGo = new GameObject("CaptureCamera");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.orthographic = true;
            cam.targetTexture = rt;
            cam.cullingMask = ~0;

            var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None).Where(c => c.isRootCanvas).ToArray();
            var modes = canvases.Select(c => c.renderMode).ToArray();
            var cams = canvases.Select(c => c.worldCamera).ToArray();
            foreach (var c in canvases)
            {
                c.renderMode = RenderMode.ScreenSpaceCamera;
                c.worldCamera = cam;
                c.planeDistance = 1;
            }
            Canvas.ForceUpdateCanvases();
            cam.Render();

            var prev = RenderTexture.active;
            RenderTexture.active = rt;
            var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            tex.Apply();
            RenderTexture.active = prev;

            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].renderMode = modes[i];
                canvases[i].worldCamera = cams[i];
            }
            var path = Path.Combine(CaptureFolder, name + ".png");
            File.WriteAllBytes(path, tex.EncodeToPNG());
            cam.targetTexture = null;
            Object.Destroy(camGo);
            Object.Destroy(rt);
            Object.Destroy(tex);
            return path;
        }

        // ------------------------------------------------------------------ pointer events and scene requests (story screens)

        /// <summary>Screen position of a point in original stage coordinates (x right, y down).</summary>
        public static Vector2 StageToScreen(float x, float y)
        {
            var stage = UI.UIStage.Current;
            NUnit.Framework.Assert.IsNotNull(stage, "No UIStage");
            var r = stage.Rect.rect;
            var world = stage.Rect.TransformPoint(new Vector3(r.xMin + x, r.yMax - y, 0));
            var canvas = stage.GetComponentInParent<Canvas>();
            var cam = canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
            return RectTransformUtility.WorldToScreenPoint(cam, world);
        }

        static UnityEngine.EventSystems.PointerEventData PointerData(float x, float y) =>
            new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current)
            {
                position = StageToScreen(x, y),
                button = UnityEngine.EventSystems.PointerEventData.InputButton.Left,
            };

        /// <summary>
        /// Press on <paramref name="target"/> at stage (x, y), optionally move to each of <paramref name="moves"/>
        /// (stage points), then release: the pointer-down / drag / up handlers get the events like a real touch.
        /// </summary>
        public static void Press(GameObject target, Vector2 at, params Vector2[] moves)
        {
            NUnit.Framework.Assert.IsNotNull(target);
            var down = PointerData(at.x, at.y);
            UnityEngine.EventSystems.ExecuteEvents.Execute(target, down, UnityEngine.EventSystems.ExecuteEvents.pointerDownHandler);
            UnityEngine.EventSystems.ExecuteEvents.Execute(target, down, UnityEngine.EventSystems.ExecuteEvents.initializePotentialDrag);
            var last = down;
            foreach (var m in moves)
            {
                last = PointerData(m.x, m.y);
                UnityEngine.EventSystems.ExecuteEvents.Execute(target, last, UnityEngine.EventSystems.ExecuteEvents.dragHandler);
            }
            UnityEngine.EventSystems.ExecuteEvents.Execute(target, last, UnityEngine.EventSystems.ExecuteEvents.pointerUpHandler);
        }

        /// <summary>
        /// Asserts that a scene transition to <paramref name="scene"/> is running. When the scene exists it waits for
        /// it to load. When it is not built yet (screens converted on other branches) SceneFlow fails to load it:
        /// the "couldn't be loaded" error for exactly that scene is expected, and the managers are recreated so later
        /// tests get a working SceneFlow.
        /// </summary>
        public static IEnumerator ExpectSceneRequest(GameScene scene, float timeout = 5)
        {
            var gm = GameManager.Instance;
            NUnit.Framework.Assert.IsTrue(gm.Flow.IsTransitioning, $"No transition to {scene} started");
            if (Application.CanStreamedLevelBeLoaded(scene.ToString()))
            {
                float end = Time.realtimeSinceStartup + timeout;
                while (gm.Flow.Current != scene && Time.realtimeSinceStartup < end) yield return null;
                NUnit.Framework.Assert.AreEqual(scene, gm.Flow.Current);
                yield break;
            }
            // Scene not on this branch yet: SceneFlow logs it and stays on the current screen.
            UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "[Rory] Scene '" + scene + "' is not in the build");
            float until = Time.realtimeSinceStartup + timeout;
            while (gm.Flow.IsTransitioning && Time.realtimeSinceStartup < until) yield return null;
            NUnit.Framework.Assert.AreEqual(scene, gm.Flow.LastRequested);
        }
    }
}
