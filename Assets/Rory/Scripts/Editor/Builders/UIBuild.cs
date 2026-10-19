using System.Collections.Generic;
using System.Linq;
using Rory.Popups;
using Rory.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Rory.EditorTools
{
    /// <summary>
    /// Helpers for the scene and prefab builders. Every position is given in the original game's coordinates
    /// (x right, y down, pivot in pixels from the top-left of the element), so builder code reads like the
    /// AS3 it replaces:  <c>UIBuild.Image(stage, "board", sprite, x: 50, y: 0)</c>.
    /// Elements are anchored at the parent's top-left; containers have zero size (see StageLayout).
    /// </summary>
    public static class UIBuild
    {
        // ------------------------------------------------------------------ fonts & colours

        public static class Fonts
        {
            public static TMP_FontAsset Bromine => Load("Bromine SDF");
            public static TMP_FontAsset JennaSue => Load("JennaSue SDF");
            public static TMP_FontAsset Pfennig => Load("Pfennig SDF");
            public static TMP_FontAsset Komikandy => Load("KOMIKND_ SDF");
            public static TMP_FontAsset KomikandyGold => Load("KomikandyGold");
            public static TMP_FontAsset KomikandyWhite => Load("KomikandyWhite");
            public static TMP_FontAsset KomikandyLarge => Load("KomikandyLarge");

            /// <summary>Font by the name used in the original code ("bromine", "jenna_sue", "KomikandyGold", ...).</summary>
            public static TMP_FontAsset ByOriginalName(string name)
            {
                switch (name.ToLowerInvariant())
                {
                    case "bromine": return Bromine;
                    case "jenna_sue": case "jennasue": return JennaSue;
                    case "pfennig": return Pfennig;
                    case "komikandy": return Komikandy;
                    case "komikandygold": return KomikandyGold;
                    case "komikandywhite": return KomikandyWhite;
                    case "komikandylarge": return KomikandyLarge;
                    default:
                        Debug.LogWarning($"[Rory] Unknown font '{name}', using Bromine");
                        return Bromine;
                }
            }

            static TMP_FontAsset Load(string file) => RoryAssets.Load<TMP_FontAsset>($"{RoryPaths.Fonts}/{file}.asset");
        }

        /// <summary>0xRRGGBB as used by the original code (e.g. 3875592).</summary>
        public static Color Hex(uint rgb, float alpha = 1) =>
            new Color(((rgb >> 16) & 0xFF) / 255f, ((rgb >> 8) & 0xFF) / 255f, (rgb & 0xFF) / 255f, alpha);

        // ------------------------------------------------------------------ scenes

        /// <summary>
        /// New empty scene with a black camera, an EventSystem, a Canvas scaled to 1024x600 and the
        /// <see cref="UIStage"/> (clipped). Returns the stage; content goes under it, popups under
        /// <paramref name="popups"/>, and the "Overlay" layer (see <see cref="Overlay"/>) stays on top of both.
        /// </summary>
        public static UIStage NewScene(out PopupManager popups)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cam = new GameObject("Main Camera", typeof(Camera)).GetComponent<Camera>();
            cam.tag = "MainCamera";
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.orthographic = true;
            cam.cullingMask = 0;

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            var canvasGo = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas));
            var canvas = canvasGo.GetComponent<Canvas>();
            UIStage.SetupCanvas(canvas);

            var stageGo = new GameObject("Stage", typeof(RectTransform), typeof(RectMask2D), typeof(UIStage));
            var stage = (RectTransform)stageGo.transform;
            stage.SetParent(canvasGo.transform, false);
            stage.anchorMin = stage.anchorMax = stage.pivot = new Vector2(0.5f, 0.5f);
            stage.sizeDelta = new Vector2(UIStage.Width, UIStage.Height);

            var popupGo = new GameObject("Popups", typeof(RectTransform), typeof(PopupManager));
            var prt = (RectTransform)popupGo.transform;
            prt.SetParent(stage, false);
            Stretch(prt);
            popups = popupGo.GetComponent<PopupManager>();
            var backdrop = new GameObject("Backdrop", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            backdrop.transform.SetParent(prt, false);
            Stretch(backdrop.rectTransform);
            backdrop.color = new Color(0, 0, 0, 0.7f);
            backdrop.gameObject.SetActive(false);
            SetField(popups, "backdrop", backdrop);

            // Above the popups: tutorial bubble, achievement banner (drawn over windows in the original).
            Layer(stage, OverlayName);

            SceneManager.SetActiveScene(scene);
            return stageGo.GetComponent<UIStage>();
        }

        /// <summary>Saves the active scene to Scenes/&lt;name&gt;.unity (Build Settings are set by BuildAll).</summary>
        public static void SaveScene(string name)
        {
            RoryAssets.EnsureFolder(RoryPaths.Scenes);
            var path = $"{RoryPaths.Scenes}/{name}.unity";
            var scene = SceneManager.GetActiveScene();
            // Keep the popup layer, then the overlay, on top of everything the builder added to the stage.
            var popups = Object.FindFirstObjectByType<PopupManager>();
            if (popups != null)
            {
                popups.transform.SetAsLastSibling();
                var overlay = popups.transform.parent.Find(OverlayName);
                if (overlay != null) overlay.SetAsLastSibling();
            }
            EditorSceneManager.SaveScene(scene, path);
            BuildAll.UpdateBuildSettings();
        }

        public const string OverlayName = "Overlay";

        /// <summary>The scene's top layer (above popups) for the tutorial bubble and the achievement banner.</summary>
        public static RectTransform Overlay(UIStage stage) => (RectTransform)stage.transform.Find(OverlayName);

        // ------------------------------------------------------------------ elements

        public static RectTransform Rect(Transform parent, string name, params System.Type[] components)
        {
            var go = new GameObject(name, new[] { typeof(RectTransform) }.Concat(components).ToArray());
            go.layer = LayerMask.NameToLayer("UI");
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = StageLayout.TopLeft;
            rt.pivot = StageLayout.TopLeft;
            rt.sizeDelta = Vector2.zero;
            return rt;
        }

        /// <summary>Empty group (a Starling Sprite) at (x, y).</summary>
        public static RectTransform Container(Transform parent, string name, float x = 0, float y = 0)
        {
            var rt = Rect(parent, name);
            rt.anchoredPosition = new Vector2(x, -y);
            return rt;
        }

        /// <summary>Full-size layer filling its parent (e.g. a full-screen layer under the stage).</summary>
        public static RectTransform Layer(Transform parent, string name)
        {
            var rt = Rect(parent, name);
            Stretch(rt);
            return rt;
        }

        public static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }

        /// <summary>Sprite at native size; (pivotX, pivotY) in pixels from its top-left (Starling pivot).</summary>
        public static Image Image(Transform parent, string name, Sprite sprite, float x = 0, float y = 0,
            float pivotX = 0, float pivotY = 0, bool raycast = false)
        {
            var rt = Rect(parent, name, typeof(CanvasRenderer), typeof(Image));
            var img = rt.GetComponent<Image>();
            img.sprite = sprite;
            img.raycastTarget = raycast;
            var size = sprite != null ? sprite.rect.size : Vector2.zero;
            StageLayout.Place(rt, x, y, size.x, size.y, pivotX, pivotY);
            return img;
        }

        /// <summary>Sprite with its pivot at its centre placed at (x, y) (alignPivot() in the original).</summary>
        public static Image ImageCentered(Transform parent, string name, Sprite sprite, float x, float y, bool raycast = false)
        {
            var size = sprite != null ? sprite.rect.size : Vector2.zero;
            return Image(parent, name, sprite, x, y, size.x / 2, size.y / 2, raycast);
        }

        /// <summary>Solid rectangle (Starling Quad).</summary>
        public static Image Quad(Transform parent, string name, float x, float y, float width, float height, Color color, bool raycast = false)
        {
            var rt = Rect(parent, name, typeof(CanvasRenderer), typeof(Image));
            var img = rt.GetComponent<Image>();
            img.color = color;
            img.raycastTarget = raycast;
            StageLayout.Place(rt, x, y, width, height);
            return img;
        }

        /// <summary>
        /// Text box of the original TextField size. Alignment defaults to centred like TextField.
        /// <paramref name="autoSize"/> shrinks the font to fit (TextField.autoScale).
        /// </summary>
        public static TextMeshProUGUI Text(Transform parent, string name, string text, TMP_FontAsset font, float size, Color color,
            float x, float y, float width, float height, float pivotX = 0, float pivotY = 0,
            TextAlignmentOptions align = TextAlignmentOptions.Center, bool autoSize = false, bool raycast = false)
        {
            var rt = Rect(parent, name, typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            var t = rt.GetComponent<TextMeshProUGUI>();
            t.text = text;
            t.font = font;
            t.fontSize = size;
            t.color = color;
            t.alignment = align;
            t.raycastTarget = raycast;
            t.textWrappingMode = TextWrappingModes.Normal;
            t.overflowMode = TextOverflowModes.Overflow;
            t.margin = Vector4.zero;
            if (autoSize)
            {
                t.enableAutoSizing = true;
                t.fontSizeMax = size;
                t.fontSizeMin = Mathf.Max(4, size * 0.3f);
            }
            StageLayout.Place(rt, x, y, width, height, pivotX, pivotY);
            return t;
        }

        /// <summary>Text with its pivot centred at (x, y) (TextField + alignPivot()).</summary>
        public static TextMeshProUGUI TextCentered(Transform parent, string name, string text, TMP_FontAsset font, float size, Color color,
            float x, float y, float width, float height, bool autoSize = false)
            => Text(parent, name, text, font, size, color, x, y, width, height, width / 2, height / 2, TextAlignmentOptions.Center, autoSize);

        /// <summary>
        /// Image button (TNonStateButton). The pivot is centred like the original after alignPivot(), so (x, y) is
        /// the centre unless <paramref name="topLeft"/> is set (buttons positioned without alignPivot()).
        /// An optional label is added as a child TextMeshPro.
        /// </summary>
        public static Button Button(Transform parent, string name, Sprite sprite, float x, float y, bool topLeft = false,
            string label = null, TMP_FontAsset font = null, float fontSize = 20, Color? labelColor = null, float hoverScale = 1.05f)
        {
            var size = sprite.rect.size;
            var img = topLeft
                ? Image(parent, name, sprite, x + size.x / 2, y + size.y / 2, size.x / 2, size.y / 2, true)
                : ImageCentered(parent, name, sprite, x, y, true);
            var button = img.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = img;
            var rb = img.gameObject.AddComponent<RoryButton>();
            rb.hoverScale = hoverScale;
            if (label != null)
            {
                var t = Text(img.transform, "Label", label, font ?? Fonts.Bromine, fontSize, labelColor ?? Color.black,
                    0, 0, size.x, size.y, autoSize: true);
                t.rectTransform.anchorMin = t.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                t.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                t.rectTransform.anchoredPosition = Vector2.zero;
            }
            return button;
        }

        /// <summary>Text-only button (TStringButton: text that grows 10% on hover). (x, y) is the top-left of its box;
        /// <paramref name="rotationDegrees"/> is the original (clockwise) rotation.</summary>
        public static Button TextButton(Transform parent, string name, string text, TMP_FontAsset font, float size, Color color,
            float x, float y, float width, float height, float rotationDegrees = 0)
        {
            var t = TextCentered(parent, name, text, font, size, color, x + width / 2, y + height / 2, width, height);
            t.raycastTarget = true;
            if (rotationDegrees != 0) t.rectTransform.localEulerAngles = new Vector3(0, 0, -rotationDegrees);
            var button = t.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = t;
            var rb = t.gameObject.AddComponent<RoryButton>();
            rb.hoverScale = 1.1f;
            rb.pressScale = 1;
            return button;
        }

        public static UIParticleEmitter Particles(Transform parent, string name, Data.ParticleConfig config, float x, float y)
        {
            var rt = Rect(parent, name, typeof(CanvasRenderer), typeof(UIParticleEmitter));
            rt.anchoredPosition = new Vector2(x, -y);
            var e = rt.GetComponent<UIParticleEmitter>();
            e.config = config;
            e.raycastTarget = false;
            if (config != null && config.additive)
                e.additiveMaterial = RoryAssets.Load<Material>($"{RoryPaths.Root}/Shaders/UIAdditive.mat");
            return e;
        }

        public static SpriteAnimator Animation(Image image, List<Sprite> frames, float fps, bool loop = true)
        {
            var a = image.gameObject.AddComponent<SpriteAnimator>();
            a.frames = frames;
            a.fps = fps;
            a.loop = loop;
            if (frames.Count > 0) image.sprite = frames[0];
            return a;
        }

        // ------------------------------------------------------------------ sprites

        public static Sprite Sprite(string group, string name) => RoryAssets.Sprite(group, name);
        public static Sprite Background(string name) => RoryAssets.Background(name);

        /// <summary>Frames "prefix1", "prefix2", ... (or "prefix_1"...) from a sprite folder, in numeric order.</summary>
        public static List<Sprite> Frames(string group, string prefix)
        {
            var list = new List<(int, Sprite)>();
            foreach (var guid in AssetDatabase.FindAssets("t:Sprite", new[] { $"{RoryPaths.Sprites}/{group}" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var file = System.IO.Path.GetFileNameWithoutExtension(path);
                if (!file.StartsWith(prefix)) continue;
                if (int.TryParse(file.Substring(prefix.Length), out var n)) list.Add((n, AssetDatabase.LoadAssetAtPath<Sprite>(path)));
            }
            return list.OrderBy(p => p.Item1).Select(p => p.Item2).ToList();
        }

        // ------------------------------------------------------------------ prefabs & wiring

        public static T SavePrefab<T>(GameObject go, string path) where T : Component
        {
            RoryAssets.EnsureFolder(System.IO.Path.GetDirectoryName(path).Replace('\\', '/'));
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab.GetComponent<T>();
        }

        /// <summary>Sets a serialized (possibly private) field, for wiring controllers to scene objects.</summary>
        public static void SetField(Object target, string field, object value)
        {
            var so = new SerializedObject(target);
            var p = so.FindProperty(field);
            if (p == null)
            {
                Debug.LogError($"[Rory] {target.GetType().Name} has no serialized field '{field}'");
                return;
            }
            switch (value)
            {
                case Object o: p.objectReferenceValue = o; break;
                case int i: p.intValue = i; break;
                case float f: p.floatValue = f; break;
                case bool b: p.boolValue = b; break;
                case string s: p.stringValue = s; break;
                case Color c: p.colorValue = c; break;
                case Vector2 v: p.vector2Value = v; break;
                default: Debug.LogError($"[Rory] SetField: unsupported value type {value?.GetType()}"); break;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>Fills a serialized array/list field with objects.</summary>
        public static void SetArray(Object target, string field, IList<Object> values)
        {
            var so = new SerializedObject(target);
            var p = so.FindProperty(field);
            p.arraySize = values.Count;
            for (int i = 0; i < values.Count; i++) p.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
