using System.Collections.Generic;
using Rory.Screens.Dessert;
using Rory.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.EditorTools
{
    /// <summary>
    /// Generates Scenes/Dessert.unity from MiniGameMakeDessert.initialize() (the frozen-yogurt minigame).
    /// Draw order of the original: HUD, customer, pan, the dessert on it, the touch areas, the order preview, the lives
    /// text; then the tutorial bubble, the layer that placed ingredients move to (above the bubble), the extras level
    /// banner over its black screen, the windows and the overlay (achievement banner).
    /// </summary>
    public static class DessertBuilder
    {
        const float HudScale = 0.7f;

        [MenuItem("Rory/Build/Scenes/Dessert")]
        [RoryBuilder(100)]
        public static void Build()
        {
            var stage = UIBuild.NewScene(out _);
            var s = stage.transform;
            Sprite D(string n) => UIBuild.Sprite("make_dessert", n);
            var white = Color.white;

            UIBuild.Image(s, "Background", UIBuild.Background("Make_Dessert_BG"));

            // HUD buttons: TNonStateButton scaled 0.7, then x/y set without alignPivot (the pivot goes to the centre of
            // the unscaled texture: centre at x + width / 2)
            var pause = HudButton(s, "PauseButton", D("pause_button"), 5, 5);
            var sound = HudButton(s, "SoundButton", D("sound_button"), 48, 5);
            var soundMute = MuteIcon(s, "SoundMuteIcon", D("mute_icon"), sound);
            var music = HudButton(s, "MusicButton", UIBuild.Sprite("window", "music_icon"), 91, 5);
            var musicMute = MuteIcon(s, "MusicMuteIcon", D("mute_icon"), music);

            UIBuild.Image(s, "ScoreBoard", D("score_board"), 730, 0);
            var jenna = UIBuild.Fonts.JennaSue;
            var scoreText = UIBuild.Text(s, "ScoreText", "$ 0", jenna, 35, Color.black, 870, 0, 150, 50, localize: false);
            var goalValue = UIBuild.Text(s, "GoalValueText", "$ 0", jenna, 35, white, 725, 0, 150, 50,
                align: TextAlignmentOptions.Right, localize: false);
            // "Goal:" / "EXPERT:" are set (translated) by the controller
            var goalLabel = UIBuild.Text(s, "GoalLabelText", "Goal:", jenna, 25, white, 740, 0, 150, 50,
                align: TextAlignmentOptions.Left, localize: false);

            UIBuild.Image(s, "TimerBG", D("timer_BG"), 200, 0);
            var timerFill = Filled(UIBuild.Image(s, "TimerFill", D("timer_fill"), 200 + 73, 4));
            UIBuild.Image(s, "TimerGlass", D("timer_glass"), 200 + 49, -7);

            var paperSprite = D("preview_list");
            var paper = UIBuild.Image(s, "PreviewPaper", paperSprite, 1024 - paperSprite.rect.width, 50);

            var char1 = D("char_1");
            float customerPivotY = char1.rect.height;
            var customer = UIBuild.Image(s, "Customer", char1, -250, 255, 0, customerPivotY);
            const float barY = 240;
            var barBG = UIBuild.Image(s, "CustomerBarBG", D("cust_bar_BG"), -250, barY);
            var heart = UIBuild.ImageCentered(s, "Heart", D("heart_cust"), -250, barY + 22);
            var custFill = Filled(UIBuild.Image(s, "CustomerFill", D("cust_fill"), -250, barY + 15));
            var custGlass = UIBuild.Image(s, "CustomerGlass", D("cust_glass"), -250, barY + 15);
            var pan = UIBuild.ImageCentered(s, "Pan", D("pan"), -1000, 300);

            // onPlateObjAr: 15 cup_paper images parked off screen (x -200), one per ingredient id
            var plates = new List<Object>();
            for (int i = 0; i < DessertController.PlateCount; i++)
                plates.Add(UIBuild.ImageCentered(s, "PlateItem" + i, D("cup_paper"), -200, 0));

            // tileAr: 14 invisible 100x100 quads (centre pivot) at 132 + col * 126, 426 + row * 115, 7 per row
            var tileQuads = new List<Image>();
            for (int i = 0, col = 0, row = 0; i < DessertController.TileCount; i++)
            {
                float x = 132 + col * 126, y = 426 + row * 115;
                var q = UIBuild.Quad(s, "Tile" + i, x - 50, y - 50, 100, 100, new Color(1, 1, 1, 0), true);
                tileQuads.Add(q);
                if (++col == 7)
                {
                    col = 0;
                    row++;
                }
            }

            // previewAr: 15 images parked off screen (x -300)
            var previews = new List<Object>();
            for (int i = 0; i < DessertController.PlateCount; i++)
                previews.Add(UIBuild.ImageCentered(s, "Preview" + i, D("cup_paper"), -300, 0));

            // extras only: ExtendTextField(250, 70, "LIVES x3", "KomikandyGold", 20, white), pivot (125, 35) at (425, 65)
            var lives = UIBuild.Text(s, "LivesText", "LIVES x3", UIBuild.Fonts.KomikandyGold, 20, white, 425, 65, 250, 70, 125, 35,
                localize: false);
            lives.gameObject.SetActive(false);

            // Tutorial bubble (initializeWindow: bringTutorialToTop, then the black screen and the windows above it).
            // It stays at this place (placed ingredients and the banner are drawn above it).
            var tutorial = TutorialBuilder.AddTo(stage);
            UIBuild.SetField(tutorial, "keepBelowPopups", false);

            // setChildIndex(obj, getChildIndex(_blackScreen) - 1): placed ingredients go just below the black screen
            var plateTop = UIBuild.Container(s, "PlateTop");

            // extras: "LEVEL: N" (bromine 70, 600x300, pivot (300, 150), from x -400 at y 300) over the black screen
            var banner = UIBuild.Container(s, "LevelBanner");
            var black = UIBuild.Quad(banner, "BlackScreen", 0, 0, 1024, 600, new Color(0, 0, 0, 0.7f), true);
            black.gameObject.SetActive(false);
            var bigInfo = UIBuild.Text(banner, "BigInfoText", "LEVEL: 1", UIBuild.Fonts.Bromine, 70, white, -400, 300, 600, 300, 300, 150,
                localize: false);
            bigInfo.gameObject.SetActive(false);

            AchievementBuilder.AddNotifier(stage);

            var c = new GameObject("DessertController").AddComponent<DessertController>();
            var tiles = new List<Object>();
            for (int i = 0; i < tileQuads.Count; i++)
            {
                var t = tileQuads[i].gameObject.AddComponent<IngredientTile>();
                t.id = i;
                UIBuild.SetField(t, "controller", c);
                tiles.Add(t);
            }
            UIBuild.SetField(c, "pauseButton", pause);
            UIBuild.SetField(c, "soundButton", sound);
            UIBuild.SetField(c, "soundMuteIcon", soundMute.gameObject);
            UIBuild.SetField(c, "musicButton", music);
            UIBuild.SetField(c, "musicMuteIcon", musicMute.gameObject);
            UIBuild.SetField(c, "scoreText", scoreText);
            UIBuild.SetField(c, "goalValueText", goalValue);
            UIBuild.SetField(c, "goalLabelText", goalLabel);
            UIBuild.SetField(c, "timerFill", timerFill);
            UIBuild.SetField(c, "livesText", lives);
            UIBuild.SetField(c, "customer", customer);
            UIBuild.SetField(c, "customerPivotY", customerPivotY);
            UIBuild.SetField(c, "customerBarBG", barBG.rectTransform);
            UIBuild.SetField(c, "heart", heart.rectTransform);
            UIBuild.SetField(c, "customerFill", custFill);
            UIBuild.SetField(c, "customerGlass", custGlass.rectTransform);
            UIBuild.SetField(c, "previewPaper", paper.rectTransform);
            UIBuild.SetField(c, "pan", pan.rectTransform);
            UIBuild.SetArray(c, "plateItems", plates);
            UIBuild.SetArray(c, "tiles", tiles);
            UIBuild.SetArray(c, "previews", previews);
            UIBuild.SetField(c, "plateTop", plateTop);
            UIBuild.SetField(c, "blackScreen", black.gameObject);
            UIBuild.SetField(c, "bigInfoText", bigInfo);

            UIBuild.SaveScene("Dessert");
        }

        /// <summary>TNonStateButton(texture) with scale 0.7 positioned at (x, y) without alignPivot.</summary>
        static Button HudButton(Transform parent, string name, Sprite sprite, float x, float y)
        {
            var size = sprite.rect.size;
            var b = UIBuild.Button(parent, name, sprite, x + size.x / 2, y + size.y / 2);
            b.transform.localScale = new Vector3(HudScale, HudScale, 1);
            return b;
        }

        /// <summary>mute_icon centred at button.x + button.width / 2 (button.x is its pivot, width is scaled).</summary>
        static Image MuteIcon(Transform parent, string name, Sprite sprite, Button button)
        {
            var rt = (RectTransform)button.transform;
            var pos = rt.GetStagePosition();
            var scaled = rt.rect.size * HudScale;
            // mute.x = btn.x + btn.width / 2 - mute.pivotX, and mute's pivot is its centre
            var img = UIBuild.ImageCentered(parent, name, sprite, pos.x + scaled.x / 2 - sprite.rect.width / 2,
                pos.y + scaled.y / 2 - sprite.rect.height / 2);
            img.gameObject.SetActive(false);
            return img;
        }

        /// <summary>The clipRect of timeFillSprite / custFillSprite: a horizontal fill from the left.</summary>
        static Image Filled(Image img)
        {
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Horizontal;
            img.fillOrigin = (int)Image.OriginHorizontal.Left;
            img.fillAmount = 1;
            return img;
        }
    }
}
