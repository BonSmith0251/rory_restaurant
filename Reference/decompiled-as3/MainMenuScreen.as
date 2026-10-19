package {
class MainMenuScreen extends Screen {
    static const CongratsConfig:Class;
    static const CongratsParticle:Class;
    static const KomikandyGoldXML:Class;
    static const KomikandyGold:Class;
    static const KomikandyWhiteXML:Class;
    static const KomikandyWhite:Class;
    static const KomikandyLargeXML:Class;
    static const KomikandyLarge:Class;
    const STORY_IDX:int = 0;
    const EXTRAS_IDX:int = 1;
    const CREDITS_IDX:int = 2;
    const MOREGAMES_IDX:int = 3;
    const QUITGAME_IDX:int = 4;
    const YESEXIT_IDX:int = 5;
    const NOEXIT_IDX:int = 6;
    const ACHIEVEMENT_IDX:int = 7;
    const FULLVERSION_IDX:int = 8;
    const SOUND_BUTTON_IDX:int = 9;
    const MUSIC_BUTTON_IDX:int = 10;
    const FULLSCREEN_IDX:int = 11;
    const DELUXE_ID:int = 12;
    const RORY_ID:int = 13;
    var stringButton:Vector.<ExtendTextField>;
    var _udaLoad:Boolean;
    var belumPlay:Boolean;
    var _bolehKlik:Boolean;
    var _global:Global;
    var titleTexture:Texture;
    var titleXML:XML;
    var titleAtlas:TextureAtlas;
    var _background:cSprite;
    var initVector:Boolean;
    var onLoadProgress:Boolean;
    var mainMenuBoard:cSprite;
    var playerName:cSprite;
    var changePlayerButton:TNonStateButton;
    var soundButton:TNonStateButton;
    var musicButton:TNonStateButton;
    var mute_icon:cSprite;
    var mute_icon_music:cSprite;
    var playerNameText:ExtendTextField;
    var storyText:TStringButton;
    var extraText:TStringButton;
    var creditsText:TStringButton;
    var moreGamesText:ExtendTextField;
    var fullVersionText:ExtendTextField;
    var board:cSprite;
    var restaurant:cSprite;
    var rory:cSprite;
    var deluxe:cSprite;
    var asap:MovieClip;
    var loadBitmapFont:Boolean;
    var cheatButtonAr:Vector.<cSprite>;
    var deluxeClicked:int;
    var roryClicked:int;
    var moreGameBtn:TNonStateButton;
    var fullVersionBtn:TNonStateButton;
    var fullScreenBtn:TNonStateButton;
    var exitTxt:ExtendTextField;
    var confirmExitTxt:ExtendTextField;
    var allButtonAr:Vector.<TNonStateButton>;
    var yesBtn:TNonStateButton;
    var yesTxt:ExtendTextField;
    var noBtn:TNonStateButton;
    var noTxt:ExtendTextField;
    var moreGameExitBtn:TNonStateButton;
    var moreGameExitTxt:ExtendTextField;

    // static initializer
    static {
        CongratsConfig = MainMenuScreen_CongratsConfig;
        CongratsParticle = MainMenuScreen_CongratsParticle;
        KomikandyGoldXML = MainMenuScreen_KomikandyGoldXML;
        KomikandyGold = MainMenuScreen_KomikandyGold;
        KomikandyWhiteXML = MainMenuScreen_KomikandyWhiteXML;
        KomikandyWhite = MainMenuScreen_KomikandyWhite;
        KomikandyLargeXML = MainMenuScreen_KomikandyLargeXML;
        KomikandyLarge = MainMenuScreen_KomikandyLarge;
    }

    function MainMenuScreen():* {
        super();
    }

    override function Start():void {
        texture = null;
        xml = null;
        psConfig = null;
        psTexture = null;
        super.Start();
        assetPath.start(this._global.graphicsPath, this._global.soundsPath);
        this._global.currentSceneID = this._global.MAINMENU_ID;
        TGlobalClass.extrasMode = false;
        if (this.loadBitmapFont) goto L280;
        texture = Texture.fromBitmap(new KomikandyGold());
        xml = new XML(new KomikandyGoldXML());
        TextField.registerBitmapFont(new BitmapFont(texture, xml));
        texture = Texture.fromBitmap(new KomikandyWhite());
        xml = new XML(new KomikandyWhiteXML());
        TextField.registerBitmapFont(new BitmapFont(texture, xml));
        texture = Texture.fromBitmap(new KomikandyLarge());
        xml = new XML(new KomikandyLargeXML());
        TextField.registerBitmapFont(new BitmapFont(texture, xml));
        this.loadBitmapFont = true;
      L280:
        if (this.initVector) goto L335;
        this.stringButton = new Vector.<ExtendTextField>();
        this.allButtonAr = new Vector.<TNonStateButton>();
        this.cheatButtonAr = new Vector.<cSprite>();
        this.initVector = true;
      L335:
        if (this._global.psCongrats) goto L529;
        System.pauseForGCIfCollectionImminent(0);
        System.gc();
        psConfig = XML(new CongratsConfig());
        psTexture = Texture.fromBitmap(new CongratsParticle());
        psTexture.root.onRestore = function():void {
            psTexture = Texture.fromBitmap(new CongratsParticle());
            _global.psCongrats.texture = psTexture;
        };
        this._global.psCongrats = new PDParticleSystem(psConfig, psTexture);
        Starling.juggler.add(this._global.psCongrats);
        this._global.psCongrats.emitterX = 512;
        this._global.psCongrats.emitterY = 0;
        this._global.psCongrats.blendFactorSource = Context3DBlendFactor.ONE;
        this._global.psCongrats.blendFactorDestination = Context3DBlendFactor.ONE_MINUS_SOURCE_ALPHA;
        this._global._gameS.addChild(this._global.psCongrats);
      L529:
        this.onLoadProgress = true;
        this._global.assetManager.enqueue(this._global.graphicsPath + "title.ean", this._global.graphicsPath + "title.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "title_BG.eag");
        if (this._udaLoad) goto L611;
        this._global.assetManager.enqueue("data/sound_file.xml");
      L611:
        this._global.assetManager.loadQueue(function(f1_arg1:Number):void {
            var _local2 = null;
            var _local3 = 0;
            var _local4 = null;
            var _local5 = null;
            var _local6 = null;
            var _local7 = null;
            if (f1_arg1 != 1) goto L249;
            _local2 = _global.assetManager.getXml("sound_file");
            if (_udaLoad) goto L136;
            _local3 = 0;
            goto L123;
          L62:
            _local4 = _local2.soundName[_local3].toString();
            assetPath.soundAr.push(_local4);
            _local4 = _local4.slice(0, _local4.length - 4);
            assetPath.soundNameAr.push(_local4);
            _local3++;
          L123:
            if (_local3 < _local2.soundName.length()) goto L62;
          L136:
            if (_udaLoad) goto L242;
            _udaLoad = true;
            var $t0 = _global.soundsPath + assetPath.soundAr[assetPath.soundArCnt];
            _local5 = $t0;
            if ($t0 == "") goto L231;
            _local6 = new URLRequest(_local5);
            var $t1 = new Sound();
            _local7 = $t1;
            $t1.addEventListener(Event.COMPLETE, onSoundLoad);
            _local7.load(_local6);
            goto L238;
          L231:
            initialize();
          L238:
            goto L249;
          L242:
            initialize();
          L249:
        });
    }

    function onSoundLoad(arg1:Event):void {
        var _local2 = null;
        var _local3 = null;
        var _local4 = null;
        var _local5 = assetPath;
        var _local6 = assetPath.soundArCnt + 1;
        _local5.soundArCnt = _local6;
        if (assetPath.soundArCnt == assetPath.soundAr.length) goto L173;
        this._global._soundAsset.addSound(assetPath.getSoundsName(), arg1.target as Sound);
        _local2 = this._global.soundsPath + "" + assetPath.soundAr[assetPath.soundArCnt];
        if (_local2 == "") goto L164;
        _local3 = new URLRequest(_local2);
        var $t0 = new Sound();
        _local4 = $t0;
        $t0.addEventListener(Event.COMPLETE, this.onSoundLoad);
        _local4.load(_local3);
        goto L169;
      L164:
        this.initialize();
      L169:
        goto L204;
      L173:
        this._global._soundAsset.addSound(assetPath.getSoundsName(), arg1.target as Sound);
        this.initialize();
      L204:
    }

    override function Update():void {
        if (ready) goto L10;
        return;
      L10:
    }

    override function Kill():void {
        var _local1 = 0;
        var _local2 = 0;
        assetPath.reset();
        if (!this.onLoadProgress) goto L24;
        return;
      L24:
        _local2 = int(this.stringButton.length);
        _local1 = 0;
        goto L87;
      L40:
        this.stringButton[_local1].removeEventListener(TouchEvent.TOUCH, this.touchButton);
        this.stringButton[_local1].removeFromParent(true);
        this.stringButton[_local1] = null;
        _local1++;
      L87:
        if (_local1 < _local2) goto L40;
        this.stringButton.length = 0;
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
        if (!TGlobalClass.FreeVersion) goto L431;
        this.fullVersionBtn.removeFromParent(true);
        this.fullVersionBtn = null;
        this.fullVersionText.removeFromParent(true);
        this.fullVersionText = null;
      L431:
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
        _local1 = 0;
        goto L574;
      L550:
        this.allButtonAr[_local1].removeEventListener(Event.TRIGGERED, this.touchExitBtn);
        _local1++;
      L574:
        if (_local1 < this.allButtonAr.length) goto L550;
        this.allButtonAr.length = 0;
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

    function initialize():void {
        var _local2 = 0;
        if (GALib.initGALib) goto L29;
        GALib.Init();
        GALib.initGALib = true;
      L29:
        this._global.ReportGameState(this._global.GAMESTATE_MAIN_MENU);
        this.onLoadProgress = false;
        System.pauseForGCIfCollectionImminent(0);
        System.gc();
        if (TGlobalClass.musicPlayingIdx == 1) goto L99;
        this._global._soundAsset.playMusic("feeling-positive");
        TGlobalClass.musicPlayingIdx = 1;
      L99:
        var _local1 = Number(Math.PI / 180);
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
        this.cheatButtonAr.push(this.rory);
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
        this.cheatButtonAr.push(this.deluxe);
        this.asap = new MovieClip(this.titleAtlas.getTextures("asap_"), 10);
        this.asap.x = 700;
        this.asap.y = 0;
        this.asap.touchable = false;
        Starling.juggler.add(this.asap);
        this._global._gameS.addChild(this.asap);
        this.asap.addFrame(Texture.empty(1, 1), null, 0.2);
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
        this.stringButton.push(this.storyText);
        this.extraText = new TStringButton(100, 60, "Extras", "jenna_sue", 50, 3875592);
        this.extraText.alignPivot();
        this.extraText.x = 130 + this.extraText.pivotX;
        this.extraText.y = 435 + this.extraText.pivotY;
        this.extraText.rotation = -9 * _local1;
        this.extraText.idx = this.EXTRAS_IDX;
        this._global._gameS.addChild(this.extraText);
        this.stringButton.push(this.extraText);
        this.creditsText = new TStringButton(100, 60, "Credits", "jenna_sue", 50, 3875592);
        this.creditsText.alignPivot();
        this.creditsText.x = 145 + this.creditsText.pivotX;
        this.creditsText.y = 495 + this.creditsText.pivotY;
        this.creditsText.rotation = -9 * _local1;
        this.creditsText.idx = this.CREDITS_IDX;
        this._global._gameS.addChild(this.creditsText);
        this.stringButton.push(this.creditsText);
        $s0 = this;
        $s1 = TStringButton;
        $s2 = 200;
        $s3 = 60;
        if (!TGlobalClass.EnableQuitButton) goto L1266;
        $s4 = "Exit";
        goto L1269;
      L1266:
        $s4 = "Achievement";
      L1269:
        $s0.exitTxt = new $s1.TStringButton($s2, $s3, $s4, "jenna_sue", 50, 3875592);
        this.exitTxt.alignPivot();
        this.exitTxt.x = 105 + this.exitTxt.pivotX;
        this.exitTxt.y = 560 + this.exitTxt.pivotY;
        this.exitTxt.rotation = -9 * _local1;
        $s0 = this.exitTxt;
        if (!TGlobalClass.EnableQuitButton) goto L1360;
        $s1 = int(this.QUITGAME_IDX);
        goto L1365;
      L1360:
        $s1 = int(this.ACHIEVEMENT_IDX);
      L1365:
        $s0.idx = $s1;
        this._global._gameS.addChild(this.exitTxt);
        this.stringButton.push(this.exitTxt);
        this.soundButton = new TNonStateButton(this.titleAtlas.getTexture("sound_button"));
        this.soundButton.x = 880;
        this.soundButton.y = 5;
        this.soundButton.idx = this.SOUND_BUTTON_IDX;
        this._global._gameS.addChild(this.soundButton);
        this.allButtonAr.push(this.soundButton);
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
        this.allButtonAr.push(this.musicButton);
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
        this.playerNameText.skewY = -0.01;
        this._global._gameS.addChild(this.playerNameText);
        this.moreGameBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_green"));
        this.moreGameBtn.alignPivot();
        this.moreGameBtn.x = 900;
        this.moreGameBtn.y = 560;
        this.moreGameBtn.idx = this.MOREGAMES_IDX;
        this.moreGameBtn.visible = TGlobalClass.EnableMoreGames;
        this._global._gameS.addChild(this.moreGameBtn);
        this.allButtonAr.push(this.moreGameBtn);
        $s0 = this;
        $s1 = TNonStateButton;
        $s2 = TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_green");
        if (!TSavegame.fullScreen) goto L2040;
        $s3 = "Windowed";
        goto L2043;
      L2040:
        $s3 = "Full Screen";
      L2043:
        $s0.fullScreenBtn = new $s1.TNonStateButton($s2, $s3, "bromine", 25, 0);
        this.fullScreenBtn.alignPivot();
        this.fullScreenBtn.x = 900;
        this.fullScreenBtn.y = 560;
        this.fullScreenBtn.idx = this.FULLSCREEN_IDX;
        this._global._gameS.addChild(this.fullScreenBtn);
        this.allButtonAr.push(this.fullScreenBtn);
        if (!TGlobalClass.FreeVersion) goto L2335;
        this.fullVersionBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_green"));
        this.fullVersionBtn.alignPivot();
        this.fullVersionBtn.x = 900;
        this.fullVersionBtn.y = 490;
        this.fullVersionBtn.idx = this.FULLVERSION_IDX;
        this._global._gameS.addChild(this.fullVersionBtn);
        this.allButtonAr.push(this.fullVersionBtn);
        this.fullVersionText = new ExtendTextField(this.fullVersionBtn.width, this.fullVersionBtn.height, "Full Version", "bromine", 25, 0);
        this.fullVersionText.alignPivot();
        this.fullVersionText.x = this.fullVersionBtn.x;
        this.fullVersionText.y = this.fullVersionBtn.y + 5;
        this.fullVersionText.touchable = false;
        this._global._gameS.addChild(this.fullVersionText);
        this.stringButton.push(this.fullVersionText);
      L2335:
        this.moreGamesText = new ExtendTextField(this.moreGameBtn.width, this.moreGameBtn.height, "More Games", "bromine", 27, 0);
        this.moreGamesText.alignPivot();
        this.moreGamesText.x = this.moreGameBtn.x;
        this.moreGamesText.y = this.moreGameBtn.y + 5;
        this.moreGamesText.touchable = false;
        this.moreGamesText.visible = TGlobalClass.EnableMoreGames;
        this._global._gameS.addChild(this.moreGamesText);
        this.stringButton.push(this.moreGamesText);
        this.confirmExitTxt = new ExtendTextField(400, 150, "Are you sure you want to quit game?", "jenna_sue", 40, 0);
        this.confirmExitTxt.alignPivot();
        this.confirmExitTxt.touchable = false;
        this.confirmExitTxt.visible = false;
        this.yesBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_green"));
        this.yesBtn.alignPivot();
        this.yesBtn.idx = this.YESEXIT_IDX;
        this.yesBtn.visible = false;
        this.yesTxt = new ExtendTextField(this.yesBtn.width, this.yesBtn.height, "Yes", "bromine", 20, 0);
        this.yesTxt.alignPivot();
        this.yesTxt.touchable = false;
        this.yesTxt.visible = false;
        this.noBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_green"));
        this.noBtn.alignPivot();
        this.noBtn.idx = this.NOEXIT_IDX;
        this.noBtn.visible = false;
        this.noTxt = new ExtendTextField(this.noBtn.width, this.noBtn.height, "No", "bromine", 20, 0);
        this.noTxt.alignPivot();
        this.noTxt.visible = false;
        this.noTxt.touchable = false;
        this.moreGameExitBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_green"));
        this.moreGameExitBtn.visible = false;
        this.moreGameExitBtn.alignPivot();
        this.moreGameExitBtn.idx = this.MOREGAMES_IDX;
        this.moreGameExitTxt = new ExtendTextField(this.moreGameExitBtn.width, this.moreGameExitBtn.height, "More Games", "bromine", 22, 0);
        this.moreGameExitTxt.alignPivot();
        this.moreGameExitTxt.visible = false;
        this.moreGameExitTxt.touchable = false;
        this._global._creditsScreen.init();
        if (TSavegame.lastPlayer == 0) goto L2886;
        this.playerNameText.text = TSavegame.currentPlayer.name;
      L2886:
        this.changePlayerButton.addEventListener(TouchEvent.TOUCH, this.changePlayer);
        TGlobalClass._windowDisplay.initializeWindow(this._global.MAINMENU_ID);
        this._global._gameS.addChild(this.confirmExitTxt);
        this._global._gameS.addChild(this.yesBtn);
        this._global._gameS.addChild(this.yesTxt);
        this._global._gameS.addChild(this.noBtn);
        this._global._gameS.addChild(this.noTxt);
        this._global._gameS.addChild(this.moreGameExitBtn);
        this._global._gameS.addChild(this.moreGameExitTxt);
        this.allButtonAr.push(this.yesBtn);
        this.allButtonAr.push(this.noBtn);
        this.allButtonAr.push(this.moreGameExitBtn);
        AdsClass.initAds(512, 240);
        _local2 = 0;
        goto L3107;
      L3082:
        this.stringButton[_local2].addEventListener(TouchEvent.TOUCH, this.touchButton);
        _local2++;
      L3107:
        if (_local2 < this.stringButton.length) goto L3082;
        _local2 = 0;
        goto L3150;
      L3126:
        this.allButtonAr[_local2].addEventListener(Event.TRIGGERED, this.touchExitBtn);
        _local2++;
      L3150:
        if (_local2 < this.allButtonAr.length) goto L3126;
        _local2 = 0;
        goto L3194;
      L3169:
        this.cheatButtonAr[_local2].addEventListener(TouchEvent.TOUCH, this.touchCheatButton);
        _local2++;
      L3194:
        if (_local2 < this.cheatButtonAr.length) goto L3169;
        if (this._global._stage.displayState != StageDisplayState.FULL_SCREEN_INTERACTIVE) goto L3246;
        this.fullScreenBtn._text.text = "Windowed";
        TSavegame.fullScreen = true;
      L3246:
        this._bolehKlik = true;
        ready = true;
    }

    function touchCheatButton(arg1:TouchEvent):void {
        var _local2 = 0;
        if (!arg1.getTouch(this._global._game, TouchPhase.BEGAN)) goto L190;
        _local2 = int((arg1.currentTarget as cSprite).idx);
        if (_local2 != this.DELUXE_ID) goto L83;
        var _local3 = this;
        var _local4 = this.deluxeClicked + 1;
        _local3.deluxeClicked = _local4;
        if (!(this.deluxeClicked > 4)) goto L79;
        return;
      L79:
        goto L122;
      L83:
        if (_local2 != this.RORY_ID) goto L122;
        _local3 = this;
        _local4 = this.roryClicked + 1;
        _local3.roryClicked = _local4;
        if (!(this.roryClicked > 3)) goto L122;
        return;
      L122:
        var $t0 = this.deluxeClicked == 4;
        $s0 = $t0;
        if (!$t0) goto L142;
        $s0 = this.roryClicked == 3;
      L142:
        if (!$s0) goto L161;
        $s0 = TSavegame.currentPlayer.name == "OrIgAmInglOl";
      L161:
        if (!$s0) goto L190;
        this._global.gameScreen.cheatActive = true;
        this._global._soundAsset.playSFX("expert_reached");
      L190:
    }

    function touchExitBtn(arg1:Event):void {
        var _local2 = int((arg1.currentTarget as TNonStateButton).idx);
        if (_local2 != this.YESEXIT_IDX) goto L38;
        NativeApplication.nativeApplication.exit();
        goto L430;
      L38:
        if (_local2 != this.NOEXIT_IDX) goto L174;
        if (!TGlobalClass.EnableCrossPromo) goto L74;
        TGlobalClass._windowDisplay.widgetBG.visible = false;
        goto L87;
      L74:
        TGlobalClass._windowDisplay.smallWidgetBG.visible = false;
      L87:
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
        goto L430;
      L174:
        if (_local2 != this.MOREGAMES_IDX) goto L187;
        goto L430;
      L187:
        if (_local2 != this.FULLVERSION_IDX) goto L216;
        TGlobalClass._windowDisplay.showWindow(TWindowState.UNLOCK_FULL_VERSION);
        goto L430;
      L216:
        if (_local2 != this.SOUND_BUTTON_IDX) goto L259;
        this._global.MuteSound = !this._global.MuteSound;
        this.mute_icon.visible = !this.mute_icon.visible;
        goto L430;
      L259:
        if (_local2 != this.MUSIC_BUTTON_IDX) goto L302;
        this._global.MuteMusic = !this._global.MuteMusic;
        this.mute_icon_music.visible = !this.mute_icon_music.visible;
        goto L430;
      L302:
        if (_local2 != this.FULLSCREEN_IDX) goto L430;
        TSavegame.fullScreen = !TSavegame.fullScreen;
        if (!TSavegame.fullScreen) goto L351;
        this.fullScreenBtn._text.text = "Windowed";
        goto L364;
      L351:
        this.fullScreenBtn._text.text = "Full Screen";
      L364:
        $s0 = this._global._stage;
        if (!TSavegame.fullScreen) goto L392;
        $s1 = StageDisplayState.FULL_SCREEN_INTERACTIVE;
        goto L399;
      L392:
        $s1 = StageDisplayState.NORMAL;
      L399:
        $s0.displayState = $s1;
        TSavegame._saveGame.data.fullScreen = TSavegame.fullScreen;
        TSavegame._saveGame.flush();
      L430:
    }

    function touchButton(arg1:TouchEvent):void {
        var _local2 = 0;
        var _local3 = 0;
        var _local4 = 0;
        if (!arg1.getTouch(this._global._game, TouchPhase.BEGAN)) goto L1324;
        _local2 = int((arg1.currentTarget as ExtendTextField).idx);
        if (this._bolehKlik) goto L56;
        return;
      L56:
        if (_local2 != this.STORY_IDX) goto L248;
        if (TSavegame.lastPlayer == 0) goto L150;
        if (!TSavegame.currentPlayer.openingViewed) goto L112;
        this._global._game.fadeToScreen(this._global._kitchen);
        goto L130;
      L112:
        this._global._game.fadeToScreen(this._global._opening);
      L130:
        if (this._global._onFade) goto L146;
        this._bolehKlik = false;
      L146:
        goto L244;
      L150:
        _local3 = 0;
        goto L178;
      L157:
        if (!TSavegame.usedSlot[_local3]) goto L176;
        goto L185;
      L176:
        _local3++;
      L178:
        if (_local3 < 5) goto L157;
      L185:
        if (!(_local3 < 5)) goto L212;
        TGlobalClass._windowDisplay.showWindow(TWindowState.CHANGE_PLAYER);
        goto L244;
      L212:
        TGlobalClass._windowDisplay.showWindow(TWindowState.CHANGE_PLAYER);
        TGlobalClass._windowDisplay.showWindow(TWindowState.CREATE_PLAYER);
      L244:
        goto L1324;
      L248:
        if (_local2 != this.EXTRAS_IDX) goto L277;
        TGlobalClass._windowDisplay.showWindow(TWindowState.EXTRAS_MENU);
        goto L1324;
      L277:
        if (_local2 != this.CREDITS_IDX) goto L317;
        GALib.TrackEvent(GALib.EVENT_CHECK_CREDITS);
        this._global._creditsScreen.creditsContainer.visible = true;
        goto L1324;
      L317:
        if (_local2 != this.MOREGAMES_IDX) goto L330;
        goto L1324;
      L330:
        if (_local2 != this.QUITGAME_IDX) goto L1286;
        if (!TGlobalClass.EnableCrossPromo) goto L818;
        AdsClass.checkConnection();
        TGlobalClass._windowDisplay.widgetBG.visible = true;
        TGlobalClass._windowDisplay._blackScreen.visible = true;
        this._global._gameS.setChildIndex(TGlobalClass._windowDisplay._blackScreen, this._global._gameS.getChildIndex(TGlobalClass._windowDisplay.widgetBG) - 1);
        this.confirmExitTxt.visible = true;
        this.yesBtn.visible = true;
        this.yesTxt.visible = true;
        this.noBtn.visible = true;
        this.noTxt.visible = true;
        this.moreGameExitBtn.visible = TGlobalClass.EnableMoreGames;
        this.moreGameExitTxt.visible = TGlobalClass.EnableMoreGames;
        this.confirmExitTxt.x = TGlobalClass._windowDisplay.widgetBG.x;
        this.confirmExitTxt.y = TGlobalClass._windowDisplay.widgetBG.y + 125;
        if (!TGlobalClass.EnableMoreGames) goto L547;
        $s0 = 170;
        goto L549;
      L547:
        $s0 = 120;
      L549:
        _local4 = int($s0);
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
        if (!TGlobalClass.EnableMoreGames) goto L814;
        var _local5 = 0.8;
        this.yesBtn.scaleY = 0.8;
        this.yesBtn.scaleX = _local5;
        _local5 = 0.8;
        this.noBtn.scaleY = 0.8;
        this.noBtn.scaleX = _local5;
        _local5 = 0.8;
        this.moreGameExitBtn.scaleY = 0.8;
        this.moreGameExitBtn.scaleX = _local5;
      L814:
        goto L1282;
      L818:
        TGlobalClass._windowDisplay.smallWidgetBG.visible = true;
        TGlobalClass._windowDisplay._blackScreen.visible = true;
        this._global._gameS.setChildIndex(TGlobalClass._windowDisplay._blackScreen, this._global._gameS.getChildIndex(TGlobalClass._windowDisplay.widgetBG) - 1);
        this.confirmExitTxt.visible = true;
        this.yesBtn.visible = true;
        this.yesTxt.visible = true;
        this.noBtn.visible = true;
        this.noTxt.visible = true;
        this.moreGameExitBtn.visible = TGlobalClass.EnableMoreGames;
        this.moreGameExitTxt.visible = TGlobalClass.EnableMoreGames;
        this.confirmExitTxt.x = TGlobalClass._windowDisplay.widgetBG.x;
        this.confirmExitTxt.y = TGlobalClass._windowDisplay.widgetBG.y - 55;
        if (!TGlobalClass.EnableMoreGames) goto L1008;
        $s0 = 120;
        goto L1010;
      L1008:
        $s0 = 120;
      L1010:
        _local4 = int($s0);
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
        _local5 = 0.8;
        this.yesBtn.scaleY = 0.8;
        this.yesBtn.scaleX = _local5;
        _local5 = 0.8;
        this.noBtn.scaleY = 0.8;
        this.noBtn.scaleX = _local5;
        if (!TGlobalClass.EnableMoreGames) goto L1282;
        _local5 = 0.8;
        this.moreGameExitBtn.scaleY = 0.8;
        this.moreGameExitBtn.scaleX = _local5;
      L1282:
        goto L1324;
      L1286:
        if (_local2 != this.ACHIEVEMENT_IDX) goto L1324;
        this._global._achievementScreen.backToMainMenu = true;
        this._global._game.fadeToScreen(this._global._achievementScreen);
      L1324:
    }

    function changePlayer(arg1:TouchEvent):void {
        if (!arg1.getTouch(this._global._game, TouchPhase.BEGAN)) goto L40;
        TGlobalClass._windowDisplay.showWindow(TWindowState.CHANGE_PLAYER);
      L40:
    }
}
}
