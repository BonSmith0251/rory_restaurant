// Ported from MainMenuScreen.as (decompiled). Title screen.
using System;
using System.Collections.Generic;
using Starlite;
using Mathf = UnityEngine.Mathf;

public class MainMenuScreen : Screen
{
    // [Embed] classes -> embedded asset symbol names (loaded via AS3.embeddedTexture / AS3.embeddedXml)
    public const string CongratsConfig = "MainMenuScreen_CongratsConfig";
    public const string CongratsParticle = "MainMenuScreen_CongratsParticle";
    public const string KomikandyGoldXML = "MainMenuScreen_KomikandyGoldXML";
    public const string KomikandyGold = "MainMenuScreen_KomikandyGold";
    public const string KomikandyWhiteXML = "MainMenuScreen_KomikandyWhiteXML";
    public const string KomikandyWhite = "MainMenuScreen_KomikandyWhite";
    public const string KomikandyLargeXML = "MainMenuScreen_KomikandyLargeXML";
    public const string KomikandyLarge = "MainMenuScreen_KomikandyLarge";

    public readonly int STORY_IDX = 0;
    public readonly int EXTRAS_IDX = 1;
    public readonly int CREDITS_IDX = 2;
    public readonly int MOREGAMES_IDX = 3;
    public readonly int QUITGAME_IDX = 4;
    public readonly int YESEXIT_IDX = 5;
    public readonly int NOEXIT_IDX = 6;
    public readonly int ACHIEVEMENT_IDX = 7;
    public readonly int FULLVERSION_IDX = 8;
    public readonly int SOUND_BUTTON_IDX = 9;
    public readonly int MUSIC_BUTTON_IDX = 10;
    public readonly int FULLSCREEN_IDX = 11;
    public readonly int DELUXE_ID = 12;
    public readonly int RORY_ID = 13;

    public List<ExtendTextField> stringButton;
    public bool _udaLoad;
    public bool belumPlay;
    public bool _bolehKlik;
    public Global _global;
    public Texture titleTexture;
    public XML titleXML;
    public TextureAtlas titleAtlas;
    public cSprite _background;
    public bool initVector;
    public bool onLoadProgress;
    public cSprite mainMenuBoard;
    public cSprite playerName;
    public TNonStateButton changePlayerButton;
    public TNonStateButton soundButton;
    public TNonStateButton musicButton;
    public cSprite mute_icon;
    public cSprite mute_icon_music;
    public ExtendTextField playerNameText;
    public TStringButton storyText;
    public TStringButton extraText;
    public TStringButton creditsText;
    public ExtendTextField moreGamesText;
    public ExtendTextField fullVersionText;
    public cSprite board;
    public cSprite restaurant;
    public cSprite rory;
    public cSprite deluxe;
    public MovieClip asap;
    public bool loadBitmapFont;
    public List<cSprite> cheatButtonAr;
    public int deluxeClicked;
    public int roryClicked;
    public TNonStateButton moreGameBtn;
    public TNonStateButton fullVersionBtn;
    public TNonStateButton fullScreenBtn;
    public ExtendTextField exitTxt;
    public ExtendTextField confirmExitTxt;
    public List<TNonStateButton> allButtonAr;
    public TNonStateButton yesBtn;
    public ExtendTextField yesTxt;
    public TNonStateButton noBtn;
    public ExtendTextField noTxt;
    public TNonStateButton moreGameExitBtn;
    public ExtendTextField moreGameExitTxt;

    public MainMenuScreen()
    {
    }

    public override void Start()
    {
        Texture texture = null;
        XML xml = null;
        XML psConfig = null;
        Texture psTexture = null;
        base.Start();
        assetPath.start(this._global.graphicsPath, this._global.soundsPath);
        this._global.currentSceneID = this._global.MAINMENU_ID;
        TGlobalClass.extrasMode = false;
        if (!this.loadBitmapFont)
        {
            texture = AS3.embeddedTexture(KomikandyGold);
            xml = AS3.embeddedXml(KomikandyGoldXML);
            TextField.registerBitmapFont(new BitmapFont(texture, xml));
            texture = AS3.embeddedTexture(KomikandyWhite);
            xml = AS3.embeddedXml(KomikandyWhiteXML);
            TextField.registerBitmapFont(new BitmapFont(texture, xml));
            texture = AS3.embeddedTexture(KomikandyLarge);
            xml = AS3.embeddedXml(KomikandyLargeXML);
            TextField.registerBitmapFont(new BitmapFont(texture, xml));
            this.loadBitmapFont = true;
        }
        if (!this.initVector)
        {
            this.stringButton = new List<ExtendTextField>();
            this.allButtonAr = new List<TNonStateButton>();
            this.cheatButtonAr = new List<cSprite>();
            this.initVector = true;
        }
        if (this._global.psCongrats == null)
        {
            psConfig = AS3.embeddedXml(CongratsConfig);
            psTexture = AS3.embeddedTexture(CongratsParticle);
            // psTexture.root.onRestore (lost-context handler) dropped: no context loss in Unity.
            this._global.psCongrats = new PDParticleSystem(psConfig, psTexture);
            Starling.juggler.add(this._global.psCongrats);
            this._global.psCongrats.emitterX = 512;
            this._global.psCongrats.emitterY = 0;
            this._global.psCongrats.blendFactorSource = Context3DBlendFactor.ONE;
            this._global.psCongrats.blendFactorDestination = Context3DBlendFactor.ONE_MINUS_SOURCE_ALPHA;
            this._global._gameS.addChild(this._global.psCongrats);
        }
        this.onLoadProgress = true;
        this._global.assetManager.enqueue(this._global.graphicsPath + "title.ean", this._global.graphicsPath + "title.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "title_BG.eag");
        if (!this._udaLoad)
        {
            this._global.assetManager.enqueue("data/sound_file.xml");
        }
        this._global.assetManager.loadQueue((float f1_arg1) =>
        {
            if (f1_arg1 != 1) return;
            XML _local2 = this._global.assetManager.getXml("sound_file");
            if (!this._udaLoad)
            {
                for (int _local3 = 0; _local3 < _local2["soundName"].length(); _local3++)
                {
                    string _local4 = _local2["soundName"][_local3].toString();
                    assetPath.soundAr.push(_local4);
                    _local4 = _local4.slice(0, _local4.Length - 4);
                    assetPath.soundNameAr.push(_local4);
                }
            }
            if (!this._udaLoad)
            {
                this._udaLoad = true;
                string _local5 = this._global.soundsPath + AS3.str(assetPath.soundAr[assetPath.soundArCnt]);
                if (_local5 != "")
                {
                    var _local6 = new URLRequest(_local5);
                    var _local7 = new Sound();
                    _local7.addEventListener(Event.COMPLETE, this.onSoundLoad);
                    _local7.load(_local6);
                }
                else
                {
                    this.initialize();
                }
            }
            else
            {
                this.initialize();
            }
        });
    }

    public void onSoundLoad(Event arg1)
    {
        assetPath.soundArCnt = assetPath.soundArCnt + 1;
        if (assetPath.soundArCnt != assetPath.soundAr.Count)
        {
            this._global._soundAsset.addSound(assetPath.getSoundsName(), arg1.target as Sound);
            string _local2 = this._global.soundsPath + "" + AS3.str(assetPath.soundAr[assetPath.soundArCnt]);
            if (_local2 != "")
            {
                var _local3 = new URLRequest(_local2);
                var _local4 = new Sound();
                _local4.addEventListener(Event.COMPLETE, this.onSoundLoad);
                _local4.load(_local3);
            }
            else
            {
                this.initialize();
            }
        }
        else
        {
            this._global._soundAsset.addSound(assetPath.getSoundsName(), arg1.target as Sound);
            this.initialize();
        }
    }

    public override void Update()
    {
        if (!ready) return;
    }

    public override void Kill()
    {
        int _local1 = 0;
        int _local2 = 0;
        assetPath.reset();
        if (this.onLoadProgress) return;
        _local2 = this.stringButton.Count;
        for (_local1 = 0; _local1 < _local2; _local1++)
        {
            this.stringButton[_local1].removeEventListener(TouchEvent.TOUCH, this.touchButton);
            this.stringButton[_local1].removeFromParent(true);
            this.stringButton[_local1] = null;
        }
        this.stringButton.Clear();
        AdsClass.kill();
        this.playerName.removeFromParent(true);
        this.playerName = null;
        this.playerNameText.removeFromParent(true);
        this.playerNameText = null;
        this.mainMenuBoard.removeFromParent(true);
        this.mainMenuBoard = null;
        this.storyText.removeFromParent(true);
        this.storyText = null;
        this.extraText.removeFromParent(true);
        this.extraText = null;
        this.creditsText.removeFromParent(true);
        this.creditsText = null;
        this.moreGamesText.removeFromParent(true);
        this.moreGamesText = null;
        this.changePlayerButton.removeEventListener(TouchEvent.TOUCH, this.changePlayer);
        this.changePlayerButton.removeFromParent(true);
        this.changePlayerButton = null;
        this.board.removeFromParent(true);
        this.board = null;
        this.restaurant.removeFromParent(true);
        this.restaurant = null;
        this.rory.removeFromParent(true);
        this.rory = null;
        this.deluxe.removeFromParent(true);
        this.deluxe = null;
        this.asap.removeFromParent(true);
        this.asap = null;
        this.soundButton.removeFromParent(true);
        this.soundButton = null;
        this.musicButton.removeFromParent(true);
        this.musicButton = null;
        this.mute_icon.removeFromParent(true);
        this.mute_icon = null;
        this.mute_icon_music.removeFromParent(true);
        this.mute_icon_music = null;
        this.moreGameBtn.removeFromParent(true);
        this.moreGameBtn = null;
        this.fullScreenBtn.removeFromParent(true);
        this.fullScreenBtn = null;
        if (TGlobalClass.FreeVersion)
        {
            this.fullVersionBtn.removeFromParent(true);
            this.fullVersionBtn = null;
            this.fullVersionText.removeFromParent(true);
            this.fullVersionText = null;
        }
        this.exitTxt.removeFromParent(true);
        this.exitTxt = null;
        this.confirmExitTxt.removeFromParent(true);
        this.confirmExitTxt = null;
        this.yesBtn.removeFromParent(true);
        this.yesBtn = null;
        this.yesTxt.removeFromParent(true);
        this.yesTxt = null;
        this.noBtn.removeFromParent(true);
        this.noBtn = null;
        this.noTxt.removeFromParent(true);
        this.noTxt = null;
        this.moreGameExitBtn.removeFromParent(true);
        this.moreGameExitBtn = null;
        this.moreGameExitTxt.removeFromParent(true);
        this.moreGameExitTxt = null;
        for (_local1 = 0; _local1 < this.allButtonAr.Count; _local1++)
        {
            this.allButtonAr[_local1].removeEventListener(Event.TRIGGERED, this.touchExitBtn);
        }
        this.allButtonAr.Clear();
        this._background.removeFromParent(true);
        this._background = null;
        this._global._creditsScreen.Kill();
        this.titleAtlas.dispose();
        this.titleAtlas = null;
        this._global.assetManager.purge();
        TGlobalClass._windowDisplay.hideAllWindows(this._global.currentSceneID);
        TGlobalClass._windowDisplay.kill(this._global.currentSceneID);
        ready = false;
    }

    public void initialize()
    {
        int _local2 = 0;
        if (!GALib.initGALib)
        {
            GALib.Init();
            GALib.initGALib = true;
        }
        this._global.ReportGameState(this._global.GAMESTATE_MAIN_MENU);
        this.onLoadProgress = false;
        if (TGlobalClass.musicPlayingIdx != 1)
        {
            this._global._soundAsset.playMusic("feeling-positive");
            TGlobalClass.musicPlayingIdx = 1;
        }
        float _local1 = Mathf.PI / 180;
        this.titleAtlas = this._global.assetManager.getTextureAtlas("title");
        this._background = new cSprite(this._global.assetManager.getTexture("title_BG"));
        this._background.blendMode = BlendMode.NONE;
        this._background.touchable = false;
        this._global._gameS.addChild(this._background);
        this.board = new cSprite(this.titleAtlas.getTexture("board"));
        this.board.pivotX = this.board.width / 2;
        this.board.pivotY = this.board.height / 2;
        this.board.touchable = false;
        this.board.x = 50 + this.board.pivotX;
        this.board.y = this.board.pivotY;
        this._global._gameS.addChild(this.board);
        this.rory = new cSprite(this.titleAtlas.getTexture("rory"));
        this.rory.pivotX = this.rory.width / 2;
        this.rory.pivotY = this.rory.height / 2;
        this.rory.x = this.board.x;
        this.rory.y = this.board.y - 70;
        this.rory.idx = this.RORY_ID;
        this._global._gameS.addChild(this.rory);
        this.cheatButtonAr.Add(this.rory);
        this.restaurant = new cSprite(this.titleAtlas.getTexture("restaurant"));
        this.restaurant.pivotX = this.restaurant.width / 2;
        this.restaurant.pivotY = this.restaurant.height / 2;
        this.restaurant.touchable = false;
        this.restaurant.x = this.board.x;
        this.restaurant.y = this.board.y;
        this._global._gameS.addChild(this.restaurant);
        this.deluxe = new cSprite(this.titleAtlas.getTexture("deluxe"));
        this.deluxe.pivotX = this.deluxe.width / 2;
        this.deluxe.pivotY = this.deluxe.height / 2;
        this.deluxe.x = this.board.x;
        this.deluxe.y = this.board.y + 60;
        this.deluxe.idx = this.DELUXE_ID;
        this._global._gameS.addChild(this.deluxe);
        this.cheatButtonAr.Add(this.deluxe);
        this.asap = new MovieClip(this.titleAtlas.getTextures("asap_"), 10);
        this.asap.x = 700;
        this.asap.y = 0;
        this.asap.touchable = false;
        Starling.juggler.add(this.asap);
        this._global._gameS.addChild(this.asap);
        this.asap.addFrame(Texture.empty(1, 1), 0.2f); // AS3: addFrame(texture, null /*sound*/, 0.2)
        this.playerName = new cSprite(this.titleAtlas.getTexture("player_name"));
        this.playerName.x = 400;
        this.playerName.y = 500;
        this.playerName.touchable = false;
        this._global._gameS.addChild(this.playerName);
        this.mainMenuBoard = new cSprite(this.titleAtlas.getTexture("title_board"));
        this.mainMenuBoard.x = -50;
        this.mainMenuBoard.y = 240;
        this.mainMenuBoard.touchable = false;
        this._global._gameS.addChild(this.mainMenuBoard);
        this.storyText = new TStringButton(100, 60, "Story", "jenna_sue", 50, 3875592);
        this.storyText.alignPivot();
        this.storyText.x = 120 + this.storyText.pivotX;
        this.storyText.y = 365 + this.storyText.pivotY;
        this.storyText.rotation = -9 * _local1;
        this.storyText.idx = this.STORY_IDX;
        this._global._gameS.addChild(this.storyText);
        this.stringButton.Add(this.storyText);
        this.extraText = new TStringButton(100, 60, "Extras", "jenna_sue", 50, 3875592);
        this.extraText.alignPivot();
        this.extraText.x = 130 + this.extraText.pivotX;
        this.extraText.y = 435 + this.extraText.pivotY;
        this.extraText.rotation = -9 * _local1;
        this.extraText.idx = this.EXTRAS_IDX;
        this._global._gameS.addChild(this.extraText);
        this.stringButton.Add(this.extraText);
        this.creditsText = new TStringButton(100, 60, "Credits", "jenna_sue", 50, 3875592);
        this.creditsText.alignPivot();
        this.creditsText.x = 145 + this.creditsText.pivotX;
        this.creditsText.y = 495 + this.creditsText.pivotY;
        this.creditsText.rotation = -9 * _local1;
        this.creditsText.idx = this.CREDITS_IDX;
        this._global._gameS.addChild(this.creditsText);
        this.stringButton.Add(this.creditsText);
        this.exitTxt = new TStringButton(200, 60, TGlobalClass.EnableQuitButton ? "Exit" : "Achievement", "jenna_sue", 50, 3875592);
        this.exitTxt.alignPivot();
        this.exitTxt.x = 105 + this.exitTxt.pivotX;
        this.exitTxt.y = 560 + this.exitTxt.pivotY;
        this.exitTxt.rotation = -9 * _local1;
        this.exitTxt.idx = TGlobalClass.EnableQuitButton ? this.QUITGAME_IDX : this.ACHIEVEMENT_IDX;
        this._global._gameS.addChild(this.exitTxt);
        this.stringButton.Add(this.exitTxt);
        this.soundButton = new TNonStateButton(this.titleAtlas.getTexture("sound_button"));
        this.soundButton.x = 880;
        this.soundButton.y = 5;
        this.soundButton.idx = this.SOUND_BUTTON_IDX;
        this._global._gameS.addChild(this.soundButton);
        this.allButtonAr.Add(this.soundButton);
        this.mute_icon = new cSprite(this.titleAtlas.getTexture("mute_icon"));
        this.mute_icon.x = this.soundButton.x - 10;
        this.mute_icon.y = this.soundButton.y - 12;
        this.mute_icon.touchable = false;
        this._global._gameS.addChild(this.mute_icon);
        this.mute_icon.visible = this._global.MuteSound;
        this.musicButton = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("music_icon"));
        this.musicButton.x = 950;
        this.musicButton.y = 5;
        this.musicButton.idx = this.MUSIC_BUTTON_IDX;
        this._global._gameS.addChild(this.musicButton);
        this.allButtonAr.Add(this.musicButton);
        this.mute_icon_music = new cSprite(this.titleAtlas.getTexture("mute_icon"));
        this.mute_icon_music.x = this.musicButton.x - 10;
        this.mute_icon_music.y = this.musicButton.y - 12;
        this.mute_icon_music.touchable = false;
        this._global._gameS.addChild(this.mute_icon_music);
        this.mute_icon_music.visible = this._global.MuteMusic;
        this.changePlayerButton = new TNonStateButton(this.titleAtlas.getTexture("change_player"));
        this.changePlayerButton.x = 425;
        this.changePlayerButton.y = 475;
        this._global._gameS.addChild(this.changePlayerButton);
        this.playerNameText = new ExtendTextField(250, 50, "Guest", "jenna_sue", 45, 255);
        this.playerNameText.x = this.playerName.x + 30;
        this.playerNameText.y = this.playerName.y + 40;
        this.playerNameText.rotation = -5 * _local1;
        this.playerNameText.skewY = -0.01f;
        this._global._gameS.addChild(this.playerNameText);
        this.moreGameBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_green"));
        this.moreGameBtn.alignPivot();
        this.moreGameBtn.x = 900;
        this.moreGameBtn.y = 560;
        this.moreGameBtn.idx = this.MOREGAMES_IDX;
        this.moreGameBtn.visible = TGlobalClass.EnableMoreGames;
        this._global._gameS.addChild(this.moreGameBtn);
        this.allButtonAr.Add(this.moreGameBtn);
        this.fullScreenBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_green"),
            TSavegame.fullScreen ? "Windowed" : "Full Screen", "bromine", 25, 0);
        this.fullScreenBtn.alignPivot();
        this.fullScreenBtn.x = 900;
        this.fullScreenBtn.y = 560;
        this.fullScreenBtn.idx = this.FULLSCREEN_IDX;
        this._global._gameS.addChild(this.fullScreenBtn);
        this.allButtonAr.Add(this.fullScreenBtn);
        if (TGlobalClass.FreeVersion)
        {
            this.fullVersionBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_green"));
            this.fullVersionBtn.alignPivot();
            this.fullVersionBtn.x = 900;
            this.fullVersionBtn.y = 490;
            this.fullVersionBtn.idx = this.FULLVERSION_IDX;
            this._global._gameS.addChild(this.fullVersionBtn);
            this.allButtonAr.Add(this.fullVersionBtn);
            this.fullVersionText = new ExtendTextField((int)this.fullVersionBtn.width, (int)this.fullVersionBtn.height, "Full Version", "bromine", 25, 0);
            this.fullVersionText.alignPivot();
            this.fullVersionText.x = this.fullVersionBtn.x;
            this.fullVersionText.y = this.fullVersionBtn.y + 5;
            this.fullVersionText.touchable = false;
            this._global._gameS.addChild(this.fullVersionText);
            this.stringButton.Add(this.fullVersionText);
        }
        this.moreGamesText = new ExtendTextField((int)this.moreGameBtn.width, (int)this.moreGameBtn.height, "More Games", "bromine", 27, 0);
        this.moreGamesText.alignPivot();
        this.moreGamesText.x = this.moreGameBtn.x;
        this.moreGamesText.y = this.moreGameBtn.y + 5;
        this.moreGamesText.touchable = false;
        this.moreGamesText.visible = TGlobalClass.EnableMoreGames;
        this._global._gameS.addChild(this.moreGamesText);
        this.stringButton.Add(this.moreGamesText);
        this.confirmExitTxt = new ExtendTextField(400, 150, "Are you sure you want to quit game?", "jenna_sue", 40, 0);
        this.confirmExitTxt.alignPivot();
        this.confirmExitTxt.touchable = false;
        this.confirmExitTxt.visible = false;
        this.yesBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_green"));
        this.yesBtn.alignPivot();
        this.yesBtn.idx = this.YESEXIT_IDX;
        this.yesBtn.visible = false;
        this.yesTxt = new ExtendTextField((int)this.yesBtn.width, (int)this.yesBtn.height, "Yes", "bromine", 20, 0);
        this.yesTxt.alignPivot();
        this.yesTxt.touchable = false;
        this.yesTxt.visible = false;
        this.noBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_green"));
        this.noBtn.alignPivot();
        this.noBtn.idx = this.NOEXIT_IDX;
        this.noBtn.visible = false;
        this.noTxt = new ExtendTextField((int)this.noBtn.width, (int)this.noBtn.height, "No", "bromine", 20, 0);
        this.noTxt.alignPivot();
        this.noTxt.visible = false;
        this.noTxt.touchable = false;
        this.moreGameExitBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_green"));
        this.moreGameExitBtn.visible = false;
        this.moreGameExitBtn.alignPivot();
        this.moreGameExitBtn.idx = this.MOREGAMES_IDX;
        this.moreGameExitTxt = new ExtendTextField((int)this.moreGameExitBtn.width, (int)this.moreGameExitBtn.height, "More Games", "bromine", 22, 0);
        this.moreGameExitTxt.alignPivot();
        this.moreGameExitTxt.visible = false;
        this.moreGameExitTxt.touchable = false;
        this._global._creditsScreen.init();
        if (TSavegame.lastPlayer != 0)
        {
            this.playerNameText.text = TSavegame.currentPlayer.name;
        }
        this.changePlayerButton.addEventListener(TouchEvent.TOUCH, this.changePlayer);
        TGlobalClass._windowDisplay.initializeWindow(this._global.MAINMENU_ID);
        this._global._gameS.addChild(this.confirmExitTxt);
        this._global._gameS.addChild(this.yesBtn);
        this._global._gameS.addChild(this.yesTxt);
        this._global._gameS.addChild(this.noBtn);
        this._global._gameS.addChild(this.noTxt);
        this._global._gameS.addChild(this.moreGameExitBtn);
        this._global._gameS.addChild(this.moreGameExitTxt);
        this.allButtonAr.Add(this.yesBtn);
        this.allButtonAr.Add(this.noBtn);
        this.allButtonAr.Add(this.moreGameExitBtn);
        AdsClass.initAds(512, 240);
        for (_local2 = 0; _local2 < this.stringButton.Count; _local2++)
        {
            this.stringButton[_local2].addEventListener(TouchEvent.TOUCH, this.touchButton);
        }
        for (_local2 = 0; _local2 < this.allButtonAr.Count; _local2++)
        {
            this.allButtonAr[_local2].addEventListener(Event.TRIGGERED, this.touchExitBtn);
        }
        for (_local2 = 0; _local2 < this.cheatButtonAr.Count; _local2++)
        {
            this.cheatButtonAr[_local2].addEventListener(TouchEvent.TOUCH, this.touchCheatButton);
        }
        if (this._global._stage.displayState == StageDisplayState.FULL_SCREEN_INTERACTIVE)
        {
            this.fullScreenBtn._text.text = "Windowed";
            TSavegame.fullScreen = true;
        }
        this._bolehKlik = true;
        ready = true;
    }

    public void touchCheatButton(Event arg1)
    {
        var e = (TouchEvent)arg1;
        int _local2 = 0;
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) == null) return;
        _local2 = (e.currentTarget as cSprite).idx;
        if (_local2 == this.DELUXE_ID)
        {
            this.deluxeClicked = this.deluxeClicked + 1;
            if (this.deluxeClicked > 4) return;
        }
        else if (_local2 == this.RORY_ID)
        {
            this.roryClicked = this.roryClicked + 1;
            if (this.roryClicked > 3) return;
        }
        if (this.deluxeClicked == 4 && this.roryClicked == 3 && TSavegame.currentPlayer.name == "OrIgAmInglOl")
        {
            this._global.gameScreen.cheatActive = true;
            this._global._soundAsset.playSFX("expert_reached");
        }
    }

    public void touchExitBtn(Event arg1)
    {
        int _local2 = (arg1.currentTarget as TNonStateButton).idx;
        if (_local2 == this.YESEXIT_IDX)
        {
            NativeApplication.nativeApplication.exit();
        }
        else if (_local2 == this.NOEXIT_IDX)
        {
            if (TGlobalClass.EnableCrossPromo) TGlobalClass._windowDisplay.widgetBG.visible = false;
            else TGlobalClass._windowDisplay.smallWidgetBG.visible = false;
            TGlobalClass._windowDisplay._blackScreen.visible = false;
            this.confirmExitTxt.visible = false;
            this.yesBtn.visible = false;
            this.yesTxt.visible = false;
            this.noBtn.visible = false;
            this.noTxt.visible = false;
            this.moreGameExitBtn.visible = false;
            this.moreGameExitTxt.visible = false;
            AdsClass.forceCancle();
            AdsClass.RemoveAds();
        }
        else if (_local2 == this.MOREGAMES_IDX)
        {
            // nothing (more-games link removed in this build)
        }
        else if (_local2 == this.FULLVERSION_IDX)
        {
            TGlobalClass._windowDisplay.showWindow(TWindowState.UNLOCK_FULL_VERSION);
        }
        else if (_local2 == this.SOUND_BUTTON_IDX)
        {
            this._global.MuteSound = !this._global.MuteSound;
            this.mute_icon.visible = !this.mute_icon.visible;
        }
        else if (_local2 == this.MUSIC_BUTTON_IDX)
        {
            this._global.MuteMusic = !this._global.MuteMusic;
            this.mute_icon_music.visible = !this.mute_icon_music.visible;
        }
        else if (_local2 == this.FULLSCREEN_IDX)
        {
            TSavegame.fullScreen = !TSavegame.fullScreen;
            if (TSavegame.fullScreen) this.fullScreenBtn._text.text = "Windowed";
            else this.fullScreenBtn._text.text = "Full Screen";
            this._global._stage.displayState = TSavegame.fullScreen ? StageDisplayState.FULL_SCREEN_INTERACTIVE : StageDisplayState.NORMAL;
            TSavegame._saveGame.data["fullScreen"] = TSavegame.fullScreen;
            TSavegame._saveGame.flush();
        }
    }

    public void touchButton(Event arg1)
    {
        var e = (TouchEvent)arg1;
        int _local2 = 0;
        int _local3 = 0;
        int _local4 = 0;
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) == null) return;
        _local2 = (e.currentTarget as ExtendTextField).idx;
        if (!this._bolehKlik) return;
        if (_local2 == this.STORY_IDX)
        {
            if (TSavegame.lastPlayer != 0)
            {
                if (TSavegame.currentPlayer.openingViewed) this._global._game.fadeToScreen(this._global._kitchen);
                else this._global._game.fadeToScreen(this._global._opening);
                if (!this._global._onFade) this._bolehKlik = false;
            }
            else
            {
                for (_local3 = 0; _local3 < 5; _local3++)
                {
                    if (TSavegame.usedSlot[_local3]) break;
                }
                if (_local3 < 5)
                {
                    TGlobalClass._windowDisplay.showWindow(TWindowState.CHANGE_PLAYER);
                }
                else
                {
                    TGlobalClass._windowDisplay.showWindow(TWindowState.CHANGE_PLAYER);
                    TGlobalClass._windowDisplay.showWindow(TWindowState.CREATE_PLAYER);
                }
            }
        }
        else if (_local2 == this.EXTRAS_IDX)
        {
            TGlobalClass._windowDisplay.showWindow(TWindowState.EXTRAS_MENU);
        }
        else if (_local2 == this.CREDITS_IDX)
        {
            GALib.TrackEvent(GALib.EVENT_CHECK_CREDITS);
            this._global._creditsScreen.creditsContainer.visible = true;
        }
        else if (_local2 == this.MOREGAMES_IDX)
        {
            // nothing
        }
        else if (_local2 == this.QUITGAME_IDX)
        {
            var wd = TGlobalClass._windowDisplay;
            if (TGlobalClass.EnableCrossPromo)
            {
                AdsClass.checkConnection();
                wd.widgetBG.visible = true;
                wd._blackScreen.visible = true;
                this._global._gameS.setChildIndex(wd._blackScreen, this._global._gameS.getChildIndex(wd.widgetBG) - 1);
                this.confirmExitTxt.visible = true;
                this.yesBtn.visible = true;
                this.yesTxt.visible = true;
                this.noBtn.visible = true;
                this.noTxt.visible = true;
                this.moreGameExitBtn.visible = TGlobalClass.EnableMoreGames;
                this.moreGameExitTxt.visible = TGlobalClass.EnableMoreGames;
                this.confirmExitTxt.x = wd.widgetBG.x;
                this.confirmExitTxt.y = wd.widgetBG.y + 125;
                _local4 = TGlobalClass.EnableMoreGames ? 170 : 120;
                this.yesBtn.x = this.confirmExitTxt.x + _local4;
                this.yesBtn.y = this.confirmExitTxt.y + 70;
                this.yesTxt.x = this.yesBtn.x;
                this.yesTxt.y = this.yesBtn.y;
                this.noBtn.x = this.confirmExitTxt.x - _local4;
                this.noBtn.y = this.confirmExitTxt.y + 70;
                this.noTxt.x = this.noBtn.x;
                this.noTxt.y = this.noBtn.y;
                this.moreGameExitBtn.x = this.confirmExitTxt.x;
                this.moreGameExitBtn.y = this.confirmExitTxt.y + 70;
                this.moreGameExitTxt.x = this.moreGameExitBtn.x;
                this.moreGameExitTxt.y = this.moreGameExitBtn.y;
                if (TGlobalClass.EnableMoreGames)
                {
                    this.yesBtn.scaleY = 0.8f;
                    this.yesBtn.scaleX = 0.8f;
                    this.noBtn.scaleY = 0.8f;
                    this.noBtn.scaleX = 0.8f;
                    this.moreGameExitBtn.scaleY = 0.8f;
                    this.moreGameExitBtn.scaleX = 0.8f;
                }
            }
            else
            {
                wd.smallWidgetBG.visible = true;
                wd._blackScreen.visible = true;
                this._global._gameS.setChildIndex(wd._blackScreen, this._global._gameS.getChildIndex(wd.widgetBG) - 1);
                this.confirmExitTxt.visible = true;
                this.yesBtn.visible = true;
                this.yesTxt.visible = true;
                this.noBtn.visible = true;
                this.noTxt.visible = true;
                this.moreGameExitBtn.visible = TGlobalClass.EnableMoreGames;
                this.moreGameExitTxt.visible = TGlobalClass.EnableMoreGames;
                this.confirmExitTxt.x = wd.widgetBG.x;
                this.confirmExitTxt.y = wd.widgetBG.y - 55;
                _local4 = TGlobalClass.EnableMoreGames ? 120 : 120;
                this.yesBtn.x = this.confirmExitTxt.x + _local4 - 15;
                this.yesBtn.y = this.confirmExitTxt.y + 70;
                this.yesTxt.x = this.yesBtn.x;
                this.yesTxt.y = this.yesBtn.y;
                this.noBtn.x = this.confirmExitTxt.x - _local4 + 20;
                this.noBtn.y = this.confirmExitTxt.y + 70;
                this.noTxt.x = this.noBtn.x;
                this.noTxt.y = this.noBtn.y;
                this.moreGameExitBtn.x = this.confirmExitTxt.x;
                this.moreGameExitBtn.y = this.confirmExitTxt.y + 150;
                this.moreGameExitTxt.x = this.moreGameExitBtn.x;
                this.moreGameExitTxt.y = this.moreGameExitBtn.y;
                this.yesBtn.scaleY = 0.8f;
                this.yesBtn.scaleX = 0.8f;
                this.noBtn.scaleY = 0.8f;
                this.noBtn.scaleX = 0.8f;
                if (TGlobalClass.EnableMoreGames)
                {
                    this.moreGameExitBtn.scaleY = 0.8f;
                    this.moreGameExitBtn.scaleX = 0.8f;
                }
            }
        }
        else if (_local2 == this.ACHIEVEMENT_IDX)
        {
            this._global._achievementScreen.backToMainMenu = true;
            this._global._game.fadeToScreen(this._global._achievementScreen);
        }
    }

    public void changePlayer(Event arg1)
    {
        var e = (TouchEvent)arg1;
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) != null)
        {
            TGlobalClass._windowDisplay.showWindow(TWindowState.CHANGE_PLAYER);
        }
    }
}
