using System.Collections.Generic;
using System.Linq;
using Rory.Screens.Match3;
using Rory.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.EditorTools
{
    /// <summary>
    /// Generates the match-3 prefabs (Prefabs/Match3: tile, ice, board effects, customer row) and Scenes/Match3.unity,
    /// laid out like GameScreen.initialize() / TCustomer() in the original addChild order.
    /// </summary>
    public static class Match3Builder
    {
        const string Folder = RoryPaths.Prefabs + "/Match3";
        public const string TilePrefab = Folder + "/Tile.prefab";
        public const string FrozenPrefab = Folder + "/FrozenTile.prefab";
        public const string FirePrefab = Folder + "/EffectFire.prefab";
        public const string SmokePrefab = Folder + "/EffectSmoke.prefab";
        public const string StarPrefab = Folder + "/EffectStar.prefab";
        public const string CustomerPrefab = Folder + "/Customer.prefab";

        /// <summary>BlurFilter.createGlow(15988227): the yellow hover glow of the tiles.</summary>
        static readonly Color TileGlow = UIBuild.Hex(15988227);
        /// <summary>BlurFilter.createGlow(65322): the green hover glow of the coins.</summary>
        static readonly Color CoinGlow = UIBuild.Hex(65322);

        static Sprite G(string name) => UIBuild.Sprite("game", name);
        static TMP_FontAsset Font(string name) => UIBuild.Fonts.ByOriginalName(name);

        // ================================================================== prefabs

        [MenuItem("Rory/Build/Match3 Prefabs")]
        [RoryBuilder(50)]
        public static void BuildPrefabs()
        {
            BuildTile();
            UIBuild.SavePrefab<Image>(UIBuild.ImageCentered(null, "FrozenTile", G("ice"), 0, 0).gameObject, FrozenPrefab);
            BuildEffect("EffectFire", "api_", FirePrefab);
            BuildEffect("EffectSmoke", "asap_", SmokePrefab);
            BuildEffect("EffectStar", "star_", StarPrefab);
            BuildCustomer();
            AssetDatabase.SaveAssets();
        }

        static void BuildTile()
        {
            var img = UIBuild.ImageCentered(null, "Tile", G("keju"), 0, 0);
            var glow = img.gameObject.AddComponent<Outline>();
            glow.effectColor = TileGlow;
            glow.effectDistance = new Vector2(3, -3);
            glow.useGraphicAlpha = true;
            glow.enabled = false;
            var view = img.gameObject.AddComponent<TileView>();
            UIBuild.SetField(view, "image", img);
            UIBuild.SetField(view, "glow", glow);
            UIBuild.SavePrefab<TileView>(img.gameObject, TilePrefab);
        }

        static void BuildEffect(string name, string prefix, string path)
        {
            var frames = UIBuild.Frames("game", prefix);
            var img = UIBuild.ImageCentered(null, name, frames[0], 0, 0);
            var anim = UIBuild.Animation(img, frames, BoardView.EffectFps, loop: false);
            anim.playOnEnable = false;
            UIBuild.SavePrefab<SpriteAnimator>(img.gameObject, path);
        }

        /// <summary>TCustomer(): positions are local to the row (the row's x is 0 when the customer is in).</summary>
        static void BuildCustomer()
        {
            var root = UIBuild.Container(null, "Customer");
            var c = root.gameObject.AddComponent<Match3Customer>();
            var ava = G("char_1");
            var avatar = UIBuild.Image(root, "Avatar", ava, Match3Customer.AvatarX, Match3Customer.AvatarY, ava.rect.width, ava.rect.height);
            var heartBG = UIBuild.Image(root, "HeartBG", G("cust_bar_BG"), 95, 68);
            var heart = UIBuild.ImageCentered(root, "Heart", G("heart_cust"), 95 + 19, 68 + 17);
            var patience = UIBuild.Container(root, "Patience", 95 + 36, 68 + 11);
            var bar = UIBuild.Image(patience, "Bar", G("cust_fill"), 0, 0);
            bar.type = Image.Type.Filled;
            bar.fillMethod = Image.FillMethod.Horizontal;
            bar.fillOrigin = (int)Image.OriginHorizontal.Left;
            var glass = UIBuild.Image(root, "Glass", G("cust_glass"), 95 + 36, 68 + 11);
            var icons = new Object[Match3Customer.MaxIngredients];
            var texts = new Object[Match3Customer.MaxIngredients];
            var checks = new Object[Match3Customer.MaxIngredients];
            for (int i = 0; i < Match3Customer.MaxIngredients; i++)
            {
                float x = i * 51 + 95, y = 40;
                var icon = UIBuild.ImageCentered(root, $"Ingredient{i}", G("keju"), x, y);
                icon.rectTransform.localScale = new Vector3(0.8f, 0.8f, 1);
                icons[i] = icon;
                texts[i] = UIBuild.Text(root, $"Count{i}", "x99", Font("KomikandyWhite"), 20, Color.white, x - 10, y - 5, 50, 40,
                    localize: false);
                var check = UIBuild.Image(root, $"Check{i}", G("check"), x - 10, y - 5);
                check.gameObject.SetActive(false);
                checks[i] = check;
            }
            var juice = UIBuild.Image(root, "Juice", G("juice"), 330, 47);
            juice.gameObject.SetActive(false);
            var fries = UIBuild.Image(root, "Fries", G("fries"), 380, 42);
            fries.gameObject.SetActive(false);
            var queueT = UIBuild.ImageCentered(root, "PriorityHover", G("priority"), 70, 80);
            queueT.color = new Color(1, 1, 1, 0.5f);
            queueT.gameObject.SetActive(false);
            var queue = UIBuild.ImageCentered(root, "Priority", G("priority"), 70, 80);
            queue.gameObject.SetActive(false);
            var money = UIBuild.Text(root, "Money", "$", Font("KomikandyGold"), 25, Color.white, 323, 65, 100, 50, localize: false);
            var ready = UIBuild.Text(root, "Ready", "READY!", Font("KomikandyGold"), 32, Color.white, 265, 60, 150, 100, 75, 50,
                autoSize: true);
            ready.rectTransform.localScale = Vector3.zero;
            ready.gameObject.SetActive(false);
            var click = UIBuild.Quad(root, "ClickQuad", 0, 0, Match3Customer.ClickWidth, Match3Customer.ClickHeight, new Color(1, 1, 1, 0), true);

            UIBuild.SetField(c, "avatar", avatar);
            UIBuild.SetField(c, "heartBG", heartBG);
            UIBuild.SetField(c, "heart", heart.rectTransform);
            UIBuild.SetField(c, "patienceSprite", patience.gameObject);
            UIBuild.SetField(c, "patienceBar", bar);
            UIBuild.SetField(c, "patienceGlass", glass);
            UIBuild.SetArray(c, "ingredientIcons", icons);
            UIBuild.SetArray(c, "ingredientTexts", texts);
            UIBuild.SetArray(c, "checkMarks", checks);
            UIBuild.SetField(c, "juice", juice);
            UIBuild.SetField(c, "fries", fries);
            UIBuild.SetField(c, "queueNumberTransparent", queueT);
            UIBuild.SetField(c, "queueNumber", queue);
            UIBuild.SetField(c, "moneyDisplay", money);
            UIBuild.SetField(c, "readyText", ready);
            _ = click;
            UIBuild.SavePrefab<Match3Customer>(root.gameObject, CustomerPrefab);
        }

        // ================================================================== scene

        [MenuItem("Rory/Build/Scenes/Match3")]
        [RoryBuilder(100)]
        public static void Build()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(CustomerPrefab) == null) BuildPrefabs();
            var stage = UIBuild.NewScene(out _);
            var s = stage.transform;

            var bg = UIBuild.Image(s, "Background", UIBuild.Background("match3_BG"));
            UIBuild.Image(s, "TargetBoard", G("score_board"), 140, 0);

            // TNonStateButtons placed by x/y without alignPivot (top-left quirk), scaled 0.7 around their centre
            var pause = UIBuild.Button(s, "PauseButton", G("pause_button"), 0, 0, topLeft: true);
            pause.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            var sound = UIBuild.Button(s, "SoundButton", G("sound_button"), 45, 0, topLeft: true);
            sound.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            float soundPivot = G("sound_button").rect.width / 2;
            // mute_icon.x = soundButton.x + 18 - soundButton.pivotX (x reads back the centre)
            var mute = UIBuild.Image(s, "MuteIcon", G("mute_icon"), 45 + 18, 0 + 14);
            var musicSprite = UIBuild.Sprite("window", "music_icon");
            var music = UIBuild.Button(s, "MusicButton", musicSprite, 90, 0, topLeft: true);
            music.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            // mute_icon_music.x = musicButton.x - pivotX(0) - 10, y = musicButton.y - 14 (centre of the button)
            var muteMusic = UIBuild.Image(s, "MuteIconMusic", G("mute_icon"), 90 + musicSprite.rect.width / 2 - 10,
                0 + musicSprite.rect.height / 2 - 14);
            _ = soundPivot;

            UIBuild.Image(s, "TimerBG", G("timer_BG"), 550, 7);
            var timerSprite = UIBuild.Container(s, "TimerSprite", 623, 11.2f);
            var timerFill = UIBuild.Image(timerSprite, "TimerFill", G("timer_fill"), 0, 0);
            timerFill.type = Image.Type.Filled;
            timerFill.fillMethod = Image.FillMethod.Horizontal;
            timerFill.fillOrigin = (int)Image.OriginHorizontal.Left;
            UIBuild.Image(s, "TimerGlass", G("timer_glass"), 600, 0);

            var customerPrefab = AssetDatabase.LoadAssetAtPath<Match3Customer>(CustomerPrefab);
            var dishes = new Object[3];
            var customers = new Object[3];
            var coins = new Object[3];
            var earned = new Object[3];
            var chains = new Object[3];
            var coinSprite = G("coins");
            for (int i = 0; i < 3; i++)
            {
                var dishSprite = G("veggie_croissant");
                var dish = UIBuild.Image(s, $"Dish{i}", dishSprite, 375, i * 117 + 140, dishSprite.rect.width / 2, dishSprite.rect.height);
                dish.gameObject.SetActive(false);
                dishes[i] = dish;

                var cust = (Match3Customer)PrefabUtility.InstantiatePrefab(customerPrefab, s);
                cust.name = $"Customer{i}";
                float custWidth = Match3Customer.ClickWidth + Mathf.Max(0, G("char_1").rect.width - Match3Customer.AvatarX);
                ((RectTransform)cust.transform).SetStagePosition(-custWidth, i * 118 + 63);
                customers[i] = cust;

                float cx = 120 + coinSprite.rect.width / 2, cy = i * 117 + 90 + coinSprite.rect.height / 2;
                var coin = UIBuild.ImageCentered(s, $"Coins{i}", coinSprite, cx, cy);
                var coinGlow = coin.gameObject.AddComponent<Outline>();
                coinGlow.effectColor = CoinGlow;
                coinGlow.effectDistance = new Vector2(3, -3);
                coinGlow.enabled = false;
                // paddingX = 80, paddingY = 20: the touch area is the image grown on every side
                var hit = UIBuild.Quad(coin.transform, "HitArea", -CoinButton.PaddingX, -CoinButton.PaddingY,
                    coinSprite.rect.width + 2 * CoinButton.PaddingX, coinSprite.rect.height + 2 * CoinButton.PaddingY, new Color(1, 1, 1, 0), true);
                _ = hit;
                var coinButton = coin.gameObject.AddComponent<CoinButton>();
                UIBuild.SetField(coinButton, "glow", coinGlow);
                coin.rectTransform.localScale = Vector3.zero;
                coin.gameObject.SetActive(false);
                coins[i] = coinButton;

                var e = UIBuild.Text(s, $"EarnedMoney{i}", "", Font("KomikandyGold"), 15, Color.white, cx, cy, 250, 70, 75, 35,
                    autoSize: true, localize: false);
                e.gameObject.SetActive(false);
                earned[i] = e;
                var ch = UIBuild.Text(s, $"ChainBonus{i}", "", Font("KomikandyGold"), 15, Color.white, cx, cy, 150, 50, 75, 25,
                    autoSize: true, localize: false);
                ch.gameObject.SetActive(false);
                chains[i] = ch;
            }

            // ScoreManager.initScore(255, 0): TextField(150, 50, "$ 0", "jenna_sue", 35) at x + 30
            var score = UIBuild.Text(s, "Score", "$ 0", Font("jenna_sue"), 35, Color.black, 255 + 30, 0, 150, 50, localize: false);
            var info = UIBuild.Text(s, "TargetInfo", "0/5", Font("jenna_sue"), 35, Color.white, 138, 0, 150, 50,
                align: TextAlignmentOptions.Right, localize: false);
            var state = UIBuild.Text(s, "TargetState", "Goal:", Font("jenna_sue"), 30, Color.white, 145, 0, 150, 50,
                align: TextAlignmentOptions.Left, localize: false);

            var juicer = Hardware(s, "Juicer", true, G("juicer"), 0, 400, G("juicer_layer"), 0, 395, G("juice"), 80, 500, 30, 510);
            var fryer = Hardware(s, "Fryer", false, G("fryer"), 160, 440, G("fryer_layer"), 160, 440, G("fries"), 193, 490, 200, 510);

            // board: tiles, touch quad, ice, effects, flying pieces (original addChild order)
            var board = UIBuild.Container(s, "Board");
            var boardView = board.gameObject.AddComponent<BoardView>();
            var tileLayer = UIBuild.Container(board, "Tiles");
            var touch = UIBuild.Quad(board, "TouchQuad", Match3Board.TouchX, Match3Board.TouchY, Match3Board.TouchSize, Match3Board.TouchSize,
                new Color(1, 1, 1, 0), true);
            var input = touch.gameObject.AddComponent<BoardInput>();
            var frozenLayer = UIBuild.Container(board, "FrozenTiles");
            var effectLayer = UIBuild.Container(board, "Effects");
            var flying = UIBuild.Container(board, "FlyingTiles");
            var flyingTiles = flying.gameObject.AddComponent<FlyingTiles>();
            var tilePrefab = AssetDatabase.LoadAssetAtPath<TileView>(TilePrefab);
            UIBuild.SetField(boardView, "tileLayer", tileLayer);
            UIBuild.SetField(boardView, "frozenLayer", frozenLayer);
            UIBuild.SetField(boardView, "effectLayer", effectLayer);
            UIBuild.SetField(boardView, "tilePrefab", tilePrefab);
            UIBuild.SetField(boardView, "frozenPrefab", AssetDatabase.LoadAssetAtPath<Image>(FrozenPrefab));
            UIBuild.SetField(boardView, "firePrefab", AssetDatabase.LoadAssetAtPath<SpriteAnimator>(FirePrefab));
            UIBuild.SetField(boardView, "smokePrefab", AssetDatabase.LoadAssetAtPath<SpriteAnimator>(SmokePrefab));
            UIBuild.SetField(boardView, "starPrefab", AssetDatabase.LoadAssetAtPath<SpriteAnimator>(StarPrefab));
            UIBuild.SetField(flyingTiles, "prefab", tilePrefab);

            // goalExpertText ("Goal!!!") is never shown by the original and is left out; its idx is Match3Controller.goalState.
            var large = UIBuild.Text(s, "LargeText", "Awesome!", Font("KomikandyLarge"), 50, UIBuild.Hex(16776960), 730, 300, 500, 400,
                250, 200, autoSize: true, localize: false);
            large.gameObject.SetActive(false);
            var black = UIBuild.Quad(s, "BlackScreen", 0, 0, 1024, 600, new Color(0, 0, 0, 0.5f), true);
            black.gameObject.SetActive(false);
            // outOfMoveText / levelCompleteText are never shown by the original and are left out.
            var noMoves = UIBuild.Text(s, "NoPossibleMoves", "NO POSSIBLE MOVES!", Font("komikandy"), 50, Color.white, -500, 300, 500, 200,
                250, 100, autoSize: true);
            noMoves.gameObject.SetActive(false);
            var dragTop = UIBuild.Container(s, "DragTop");

            TutorialBuilder.AddTo(stage);
            AchievementBuilder.AddNotifier(stage);

            var c = new GameObject("Match3Controller").AddComponent<Match3Controller>();
            UIBuild.SetField(c, "pauseButton", pause);
            UIBuild.SetField(c, "soundButton", sound);
            UIBuild.SetField(c, "muteIcon", mute.gameObject);
            UIBuild.SetField(c, "musicButton", music);
            UIBuild.SetField(c, "muteIconMusic", muteMusic.gameObject);
            UIBuild.SetField(c, "timerFill", timerFill);
            UIBuild.SetField(c, "scoreText", score);
            UIBuild.SetField(c, "targetInfoText", info);
            UIBuild.SetField(c, "targetStateText", state);
            UIBuild.SetArray(c, "customers", customers);
            UIBuild.SetArray(c, "dishes", dishes);
            UIBuild.SetArray(c, "coins", coins);
            UIBuild.SetArray(c, "earnedMoneyTexts", earned);
            UIBuild.SetArray(c, "chainBonusTexts", chains);
            UIBuild.SetField(c, "juicer", juicer);
            UIBuild.SetField(c, "fryer", fryer);
            UIBuild.SetField(c, "boardView", boardView);
            UIBuild.SetField(c, "boardInput", input);
            UIBuild.SetField(c, "flyingTiles", flyingTiles);
            UIBuild.SetField(c, "largeText", large);
            UIBuild.SetField(c, "blackScreen", black);
            UIBuild.SetField(c, "noPossibleMoveText", noMoves);
            UIBuild.SetField(c, "dragTop", dragTop);
            _ = bg;

            UIBuild.SaveScene("Match3");
        }

        /// <summary>Juicer / fryer: machine button (x/y = top-left), its layer, the product and its dragged copy (centre
        /// pivot at productX/Y + half size) and the percentage text.</summary>
        static HardwareStation Hardware(Transform s, string name, bool juicer, Sprite machineSprite, float mx, float my,
            Sprite layerSprite, float lx, float ly, Sprite productSprite, float px, float py, float tx, float ty)
        {
            var station = UIBuild.Container(s, name).gameObject.AddComponent<HardwareStation>();
            var machine = UIBuild.Button(s, name + "Machine", machineSprite, mx, my, topLeft: true);
            var layer = UIBuild.Image(s, name + "Layer", layerSprite, lx, ly);
            float hx = px + productSprite.rect.width / 2, hy = py + productSprite.rect.height / 2;
            var product = UIBuild.ImageCentered(s, name + "Product", productSprite, hx, hy);
            product.gameObject.SetActive(false);
            var copy = UIBuild.ImageCentered(s, name + "Drag", productSprite, hx, hy, raycast: true);
            copy.gameObject.SetActive(false);
            var text = UIBuild.Text(s, name + "Percentage", "0%", Font("KomikandyWhite"), 25, Color.white, tx, ty, 150, 100,
                autoSize: true, localize: false);
            foreach (var go in new[] { machine.gameObject, copy.gameObject })
                go.AddComponent<HardwareTouch>().station = station;
            UIBuild.SetField(station, "isJuicer", juicer);
            UIBuild.SetField(station, "machine", machine);
            UIBuild.SetField(station, "layer", layer);
            UIBuild.SetField(station, "product", product);
            UIBuild.SetField(station, "dragCopy", copy);
            UIBuild.SetField(station, "percentage", text);
            UIBuild.SetField(station, "home", new Vector2(hx, hy));
            return station;
        }
    }
}
