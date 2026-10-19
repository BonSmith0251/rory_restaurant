using Rory.Popups;
using Rory.Screens.MainMenu;
using Rory.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.EditorTools
{
    /// <summary>Generates Scenes/MainMenu.unity from the layout in MainMenuScreen.initialize() / Credits.init().</summary>
    public static class MainMenuBuilder
    {
        static readonly Color MenuText = UIBuild.Hex(3875592); // dark red-brown of the menu entries
        static readonly Color Red = UIBuild.Hex(0xFF0000);

        [MenuItem("Rory/Build/Scenes/Main Menu")]
        public static void Build()
        {
            var stage = UIBuild.NewScene(out var popups);
            var s = stage.transform;
            Sprite T(string n) => UIBuild.Sprite("title", n);

            UIBuild.Image(s, "Background", UIBuild.Background("title_BG"));

            var board = T("board");
            float boardX = 50 + board.rect.width / 2, boardY = board.rect.height / 2;
            UIBuild.ImageCentered(s, "Board", board, boardX, boardY);
            var rory = UIBuild.ImageCentered(s, "Rory", T("rory"), boardX, boardY - 70, true);
            UIBuild.ImageCentered(s, "Restaurant", T("restaurant"), boardX, boardY);
            var deluxe = UIBuild.ImageCentered(s, "Deluxe", T("deluxe"), boardX, boardY + 60, true);
            var roryButton = PlainButton(rory);
            var deluxeButton = PlainButton(deluxe);

            // "asap" sign animation: 12 frames at 10 fps followed by an empty frame shown for 0.2 s
            var asap = UIBuild.Image(s, "AsapSign", T("asap_0001"), 700, 0);
            var anim = UIBuild.Animation(asap, UIBuild.Frames("title", "asap_"), 10);
            anim.blankAtEnd = 0.2f;

            UIBuild.Image(s, "PlayerNameBoard", T("player_name"), 400, 500);
            UIBuild.Image(s, "MenuBoard", T("title_board"), -50, 240);

            var story = UIBuild.TextButton(s, "StoryButton", "Story", UIBuild.Fonts.JennaSue, 50, MenuText, 120, 365, 100, 60, -9);
            var extras = UIBuild.TextButton(s, "ExtrasButton", "Extras", UIBuild.Fonts.JennaSue, 50, MenuText, 130, 435, 100, 60, -9);
            var credits = UIBuild.TextButton(s, "CreditsButton", "Credits", UIBuild.Fonts.JennaSue, 50, MenuText, 145, 495, 100, 60, -9);
            var exit = UIBuild.TextButton(s, "ExitButton", "Exit", UIBuild.Fonts.JennaSue, 50, MenuText, 105, 560, 200, 60, -9);

            var sound = UIBuild.Button(s, "SoundButton", T("sound_button"), 880, 5, topLeft: true);
            var soundMute = UIBuild.Image(s, "SoundMuteIcon", T("mute_icon"), 870, -7);
            var music = UIBuild.Button(s, "MusicButton", UIBuild.Sprite("window", "music_icon"), 950, 5, topLeft: true);
            var musicMute = UIBuild.Image(s, "MusicMuteIcon", T("mute_icon"), 940, -7);

            var changePlayer = UIBuild.Button(s, "ChangePlayerButton", T("change_player"), 425, 475, topLeft: true);
            var playerName = UIBuild.Text(s, "PlayerName", "Guest", UIBuild.Fonts.JennaSue, 45, UIBuild.Hex(255), 430, 540, 250, 50);
            playerName.rectTransform.localEulerAngles = new Vector3(0, 0, 5);

            var fullScreen = UIBuild.Button(s, "FullScreenButton", UIBuild.Sprite("window", "small_btn_green"), 900, 560,
                label: "Full Screen", font: UIBuild.Fonts.Bromine, fontSize: 25, labelColor: Color.black);

            // ---------------------------------------------------------------- popups
            var p = popups.transform;
            var select = BuildPlayerSelect(p);
            var extrasMenu = BuildExtras(p);
            var quit = BuildQuit(p);
            var creditsPopup = BuildCredits(p);

            var controller = new GameObject("MainMenuController").AddComponent<MainMenuController>();
            UIBuild.SetField(controller, "storyButton", story);
            UIBuild.SetField(controller, "extrasButton", extras);
            UIBuild.SetField(controller, "creditsButton", credits);
            UIBuild.SetField(controller, "exitButton", exit);
            UIBuild.SetField(controller, "changePlayerButton", changePlayer);
            UIBuild.SetField(controller, "playerNameText", playerName);
            UIBuild.SetField(controller, "soundButton", sound);
            UIBuild.SetField(controller, "soundMuteIcon", soundMute.gameObject);
            UIBuild.SetField(controller, "musicButton", music);
            UIBuild.SetField(controller, "musicMuteIcon", musicMute.gameObject);
            UIBuild.SetField(controller, "fullScreenButton", fullScreen);
            UIBuild.SetField(controller, "fullScreenLabel", fullScreen.GetComponentInChildren<TextMeshProUGUI>());
            UIBuild.SetField(controller, "playerSelect", select);
            UIBuild.SetField(controller, "extrasMenu", extrasMenu);
            UIBuild.SetField(controller, "quitConfirm", quit);
            UIBuild.SetField(controller, "credits", creditsPopup);
            UIBuild.SetField(controller, "roryLogo", roryButton);
            UIBuild.SetField(controller, "deluxeLogo", deluxeButton);

            UIBuild.SaveScene("MainMenu");
        }

        static Button PlainButton(Image img)
        {
            var b = img.gameObject.AddComponent<Button>();
            b.transition = Selectable.Transition.None;
            return b;
        }

        static T ScenePopup<T>(T popup) where T : Popup
        {
            popup.destroyOnClose = false;
            popup.gameObject.SetActive(false);
            return popup;
        }

        static PlayerSelectPopup BuildPlayerSelect(Transform layer)
        {
            var w = WindowBuilder.BuildWindow<PlayerSelectPopup>(layer, false, "PlayerSelect");
            w.title.text = "Select Player";
            w.info.gameObject.SetActive(false);
            WindowBuilder.Label(w.leftButton, "Delete");
            WindowBuilder.Label(w.rightButton, "Create");
            WindowBuilder.Label(w.uniButton, "Close");
            for (int i = 0; i < GameRules.SaveSlots; i++)
            {
                float y = i * 70 + 145 - 300; // stage y of the row, relative to the window centre
                var name = UIBuild.TextCentered(w.transform, $"Slot{i}", "Empty", UIBuild.Fonts.JennaSue, 30, Color.black, 0, y, 380, 70);
                name.raycastTarget = true;
                var b = name.gameObject.AddComponent<Button>();
                b.transition = Selectable.Transition.None;
                w.slotButtons[i] = b;
                w.slotNames[i] = name;
                UIBuild.Quad(w.transform, $"Underline{i}", -190, y + 15 - 2.5f, 380, 5, Color.black);
            }
            // keep the buttons above the rows
            w.leftButton.transform.SetAsLastSibling();
            w.rightButton.transform.SetAsLastSibling();
            w.uniButton.transform.SetAsLastSibling();

            w.createPlayer = ScenePopup(BuildCreatePlayer(layer, out var sameName));
            ScenePopup(sameName);
            w.deleteConfirm = ScenePopup(SmallWindow(layer, "DeletePlayerConfirm", "Delete Player?",
                "Are you sure want to delete this player? This process can't be undone.", "Cancel", "Delete"));
            return ScenePopup(w);
        }

        static WindowPopup SmallWindow(Transform layer, string name, string title, string info, string left, string right)
        {
            var w = WindowBuilder.BuildWindow<WindowPopup>(layer, true, name);
            w.title.text = title;
            w.info.text = info;
            WindowBuilder.Label(w.leftButton, left);
            WindowBuilder.Label(w.rightButton, right);
            w.uniButton.gameObject.SetActive(false);
            return w;
        }

        static CreatePlayerPopup BuildCreatePlayer(Transform layer, out WindowPopup sameName)
        {
            var w = WindowBuilder.BuildWindow<CreatePlayerPopup>(layer, true, "CreatePlayer");
            w.title.text = "Create Player";
            w.info.text = "Enter Your Name :";
            WindowBuilder.Label(w.leftButton, "Cancel");
            WindowBuilder.Label(w.rightButton, "Done");
            w.uniButton.gameObject.SetActive(false);

            // Name field: 369x40 with a border, at stage (337.5, 300) top-left = (-174.5, 0) from the centre.
            // Transparent field with a 1 px black border (the original native TextField with border = true).
            var field = UIBuild.Quad(w.transform, "NameInput", -174.5f, 0, 369, 40, new Color(1, 1, 1, 0), true);
            UIBuild.Quad(field.transform, "BorderTop", 0, 0, 369, 1, Color.black);
            UIBuild.Quad(field.transform, "BorderBottom", 0, 39, 369, 1, Color.black);
            UIBuild.Quad(field.transform, "BorderLeft", 0, 0, 1, 40, Color.black);
            UIBuild.Quad(field.transform, "BorderRight", 368, 0, 1, 40, Color.black);
            var area = UIBuild.Layer(field.transform, "Text Area");
            area.gameObject.AddComponent<RectMask2D>();
            var text = UIBuild.Text(area, "Text", "", UIBuild.Fonts.JennaSue, 30, Color.black, 0, 0, 0, 0);
            UIBuild.Stretch(text.rectTransform);
            var placeholder = UIBuild.Text(area, "Placeholder", "", UIBuild.Fonts.JennaSue, 30, new Color(0, 0, 0, 0.4f), 0, 0, 0, 0);
            UIBuild.Stretch(placeholder.rectTransform);
            var input = field.gameObject.AddComponent<TMP_InputField>();
            input.textViewport = area;
            input.textComponent = text;
            input.placeholder = placeholder;
            input.targetGraphic = field;
            input.characterLimit = 12;
            input.fontAsset = UIBuild.Fonts.JennaSue;
            input.pointSize = 30;
            input.lineType = TMP_InputField.LineType.SingleLine;
            w.nameInput = input;

            sameName = WindowBuilder.BuildWindow<WindowPopup>(layer, true, "SameNameWarning");
            sameName.title.text = "Try Again!";
            sameName.info.text = "Oops, the name you entered is already exist. Please try again!";
            sameName.leftButton.gameObject.SetActive(false);
            sameName.rightButton.gameObject.SetActive(false);
            WindowBuilder.Label(sameName.uniButton, "OK");
            w.sameNameWarning = sameName;
            return w;
        }

        static WindowPopup BuildExtras(Transform layer)
        {
            var w = WindowBuilder.BuildWindow<WindowPopup>(layer, false, "ExtrasMenu");
            w.title.text = "Select Game";
            WindowBuilder.Move(w.title, 13, -200);
            w.info.gameObject.SetActive(false);
            WindowBuilder.Label(w.leftButton, "Hidden Object");
            WindowBuilder.Label(w.rightButton, "Dessert Making");
            WindowBuilder.Label(w.uniButton, "Cancel");
            WindowBuilder.Move(w.leftButton, 0, -50);
            WindowBuilder.Move(w.rightButton, 0, 40);
            WindowBuilder.Move(w.uniButton, 0, 130);
            foreach (var b in new[] { w.leftButton, w.rightButton, w.uniButton }) b.transform.localScale = new Vector3(1.2f, 1.2f, 1);
            return ScenePopup(w);
        }

        static WindowPopup BuildQuit(Transform layer)
        {
            var w = WindowBuilder.BuildWindow<WindowPopup>(layer, true, "QuitConfirm");
            w.title.gameObject.SetActive(false);
            w.info.text = "Are you sure you want to quit game?";
            w.info.rectTransform.sizeDelta = new Vector2(400, 150);
            WindowBuilder.Move(w.info, 0, -55);
            WindowBuilder.Label(w.leftButton, "No");
            WindowBuilder.Label(w.rightButton, "Yes");
            WindowBuilder.Move(w.leftButton, -100, 15);
            WindowBuilder.Move(w.rightButton, 105, 15);
            w.leftButton.transform.localScale = w.rightButton.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            w.uniButton.gameObject.SetActive(false);
            return ScenePopup(w);
        }

        static Popup BuildCredits(Transform layer)
        {
            var root = UIBuild.Layer(layer, "Credits");
            var popup = root.gameObject.AddComponent<Popup>();
            popup.modal = false;
            var bg = root.gameObject.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.85f);
            var close = root.gameObject.AddComponent<Button>();
            close.transition = Selectable.Transition.None;

            var c = UIBuild.Container(root, "Content");
            TextMeshProUGUI Txt(string name, string text, float size, Color color, float x, float y, float w, float h, float px = 0, float py = 0,
                TextAlignmentOptions align = TextAlignmentOptions.Left)
            {
                var t = UIBuild.Text(c, name, text, UIBuild.Fonts.Pfennig, size, color, x, y, w, h, px, py, align);
                t.fontStyle = FontStyles.Bold;
                return t;
            }

            Txt("Title", "CREDITS", 60, Color.white, 512, 90, 400, 150, 200, 75, TextAlignmentOptions.Center);
            Txt("ProducersTitle", "PRODUCERS", 30, Red, 125, 120, 250, 100);
            Txt("Producers", "Andre Yu\nRobert YKG", 25, Color.white, 125, 177, 250, 100);
            UIBuild.Image(c, "OrigamingLogo", UIBuild.Sprite("title", "origaminglogo"), 400, 140);
            var copy = Txt("Copyright", "All rights reserved. Origaming Media (c) 2015", 20, Color.white, 400, 190, 600, 50);
            copy.fontStyle = FontStyles.Bold | FontStyles.Italic;
            Txt("GameConceptTitle", "GAME CONCEPT", 30, Red, 125, 260, 250, 100);
            Txt("GameConcept", "Andre Yu", 25, Color.white, 125, 320, 150, 50);
            Txt("LeadArtistTitle", "LEAD ARTIST", 30, Red, 400, 260, 250, 100);
            Txt("LeadArtist", "Robert YKG", 25, Color.white, 400, 320, 150, 50);
            Txt("ProgrammersTitle", "PROGRAMMERS", 30, Red, 125, 360, 250, 100);
            Txt("Programmers", "Wilson\nSherson Watson\nAndri", 25, Color.white, 125, 425, 300, 150, align: TextAlignmentOptions.TopLeft);
            Txt("GraphicArtistTitle", "GRAPHIC ARTIST", 30, Red, 400, 360, 350, 100);
            Txt("GraphicArtists", "Alfa Lui\nHandrison", 25, Color.white, 400, 360, 200, 200);
            Txt("Version", "Rory's Restaurant  v. 1.0.2", 20, Color.white, 122, 540, 600, 60);
            foreach (var g in c.GetComponentsInChildren<Graphic>()) g.raycastTarget = false;
            return ScenePopup(popup);
        }
    }
}
