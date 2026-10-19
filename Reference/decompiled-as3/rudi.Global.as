package rudi {
class Global {
    static var _instance:Global;
    var _game:Game;
    var _gameS:Sprite;
    var _gameEngine:gameEngine;
    var _stage:Stage;
    var _soundAsset:SoundAsset;
    var _context3D:Context3D;
    var _Starling:Starling;
    var _maxCrypto:maxCrypto;
    var _onFade:Boolean;
    var _isHandlingLostContext:Boolean;
    var startHandling:Boolean;
    var resizingStage:Boolean;
    var _mainMenuScreen:MainMenuScreen;
    var _dumLoadingScreen:dumLoading;
    var gameScreen:GameScreen;
    var _creditsScreen:Credits;
    var _kitchen:Kitchen;
    var _levelSelectScreen:LevelSelectScreen;
    var dish:Dish;
    var _minigameHO:MiniGameHO;
    var _minigameDessert:MiniGameMakeDessert;
    var tutorial:Tutorial;
    var _opening:OpeningScreen;
    var _ending:EndingClass;
    var _achievementScreen:AchievementScreen;
    var graphicsPath:String;
    var soundsPath:String;
    var currentSceneID:int;
    const MAINMENU_ID:int = 1;
    const GAMESCREEN_ID:int = 2;
    const KITCHEN_ID:int = 3;
    const HIDDEN_OBJECT_ID:int = 4;
    const MAKE_DESSERT_ID:int = 5;
    const LEVEL_SELECT_ID:int = 6;
    const ACHIEVEMENTS_ID:int = 7;
    const OPENING_ID:int = 8;
    const ENDING_ID:int = 9;
    var loadGame:Boolean;
    var newGame:Boolean;
    var firstInit:Boolean = false;
    var assetHD:Boolean;
    var embedAsset:Boolean;
    var assetEncrypt:Boolean;
    var scale:Number;
    var initFlox:Boolean;
    var assetManager:AssetManager;
    var unPurgedAsset:AssetManager;
    var ps:PDParticleSystem;
    var psFound:PDParticleSystem;
    var psCongrats:PDParticleSystem;
    var _muteSound:Boolean = false;
    var _muteMusic:Boolean = false;
    const GAMESTATE_MAIN_MENU:String = "1";
    const GAMESTATE_PLAYING:String = "2";
    const GAMESTATE_GAME_END:String = "3";
    const GAMESTATE_LEVEL_SELECT:String = "4";
    const GAMESTATE_LEVEL_END:String = "5";

    function Global(arg1:PrivateClass):* {
        super();
    }

    static function create(arg1:String):Object {
        var _local2 = maxCrypto.getInstance().StripAll(arg1);
        var _local3 = maxCrypto.getInstance().ExtractExt(arg1).toLowerCase();
        var $t0 = maxCrypto.getInstance().StripExt(arg1);
        var _local4 = $t0;
        var $t1 = $t0.split(_local2).join("");
        _local4 = $t1;
        _local4 = $t1.split("/").join("");
        _local2 = _local2.split("-").join("_").toLowerCase();
        _local2 = _local4 + _local2;
        _local2 = _local2.toLowerCase();
        var $t2 = _local3 == "mp3";
        $s0 = $t2;
        if ($t2) goto L131;
        $s0 = _local3 == "origamings";
      L131:
        if (!$s0) goto L144;
        _local2 = _local2 + "Sound";
        goto L191;
      L144:
        var $t3 = _local3 == "xml";
        $s0 = $t3;
        if ($t3) goto L160;
        $s0 = _local3 == "origamingx";
      L160:
        if ($s0) goto L171;
        $s0 = _local3 == "pex";
      L171:
        if (!$s0) goto L185;
        _local2 = _local2 + "xmlAsset";
        goto L191;
      L185:
        _local2 = _local2 + "Asset";
      L191:
        return null;
    }

    static function getInstance():Global {
        if (Global._instance != null) goto L29;
        Global._instance = new this(new PrivateClass());
      L29:
        return Global._instance;
    }

    function init(arg1:Boolean = true, arg2:Boolean = false, arg3:Boolean = false):void {
        if (!this.firstInit) goto L11;
        return;
      L11:
        this.firstInit = true;
        this.assetEncrypt = arg3;
        this.assetHD = arg1;
        this.embedAsset = arg2;
        this.soundsPath = "data/sounds/";
        if (!this.assetHD) goto L63;
        this.graphicsPath = "data/graphic/hd/";
        this.scale = 1;
        goto L76;
      L63:
        this.graphicsPath = "data/graphic/sd/";
        this.scale = 0.8;
      L76:
        this.assetManager = new AssetManager(this.scale);
        this.unPurgedAsset = new AssetManager(this.scale);
        this._gameEngine = gameEngine.getInstance();
        this._soundAsset = new SoundAsset();
        this._maxCrypto = maxCrypto.getInstance();
        this._soundAsset.firstInit();
        this._mainMenuScreen = new MainMenuScreen();
        this._mainMenuScreen._global = this;
        this.gameScreen = new GameScreen();
        this.gameScreen._global = this;
        this._creditsScreen = new Credits();
        this._creditsScreen._global = this;
        this._kitchen = new Kitchen();
        this._kitchen._global = this;
        this._levelSelectScreen = new LevelSelectScreen();
        this._levelSelectScreen._global = this;
        this._minigameHO = new MiniGameHO();
        this._minigameHO._global = this;
        this._minigameDessert = new MiniGameMakeDessert();
        this._minigameDessert._global = this;
        this._opening = new OpeningScreen();
        this._opening._global = this;
        this._ending = new EndingClass();
        this._ending._global = this;
        this._achievementScreen = new AchievementScreen();
        this._achievementScreen._global = this;
        this._dumLoadingScreen = new dumLoading();
        this._dumLoadingScreen._global = this;
        this.dish = new Dish();
        this.dish.init();
        this._dumLoadingScreen.PreStart();
        this._onFade = false;
    }

    function getCurrentSceneNameByID(arg1:int):String {
        goto L36;
      L6:
        return "Main Menu";
      L11:
        return "Match 3";
      L16:
        return "Kitchen";
      L21:
        return "Hidden Objects";
      L26:
        return "Dessert Making";
      L31:
        goto L140;
      L36:
        var _local2 = arg1;
        if (1 !== _local2) goto L51;
        $s0 = 0;
        goto L115;
      L51:
        if (2 !== _local2) goto L64;
        $s0 = 1;
        goto L115;
      L64:
        if (3 !== _local2) goto L77;
        $s0 = 2;
        goto L115;
      L77:
        if (4 !== _local2) goto L90;
        $s0 = 3;
        goto L115;
      L90:
        if (5 !== _local2) goto L103;
        $s0 = 4;
        goto L115;
      L103:
        goto L113;
      L113:
        $s0 = 5;
      L115:
        switch ($s0) { case 0: goto L6; case 1: goto L11; case 2: goto L16; case 3: goto L21; case 4: goto L26; case 5: goto L31; default: goto L31; }
      L140:
        return "Unknown";
    }

    function ReportGameState(arg1:String = ""):void {
    }

    function addCommas(arg1:Number):String {
        var _local5 = NaN;
        var _local6 = null;
        var _local7 = NaN;
        var _local2 = "";
        if (!(arg1 < 0)) goto L36;
        _local2 = "-";
        arg1 = Number(Math.abs(arg1));
      L36:
        var _local3 = String(arg1);
        var $t0 = _local3.split(new RegExp("\\."));
        var _local4 = $t0;
        _local3 = $t0[0];
        if (!(_local3.length > 3)) goto L252;
        _local5 = Number(_local3.length % 3);
        _local6 = _local3.substr(0, _local5);
        _local7 = Number(_local5);
        goto L168;
      L112:
        var $t1 = _local5 == 0;
        $s0 = _local6;
        $s1 = $t1;
        if (!$t1) goto L131;
        $s1 = _local7 == 0;
      L131:
        if (!$s1) goto L142;
        $s1 = "";
        goto L146;
      L142:
        $s1 = ",";
      L146:
        _local6 = $s0 + ($s1 + _local3.substr(_local7, 3));
        _local7 = Number(_local7 + 3);
      L168:
        if (_local7 < _local3.length) goto L112;
        if (!(_local4.length > 1)) goto L247;
        if (_local4[1].length != 1) goto L226;
        return _local2 + _local6 + "." + _local4[1] + "0";
      L226:
        return _local2 + _local6 + "." + _local4[1];
      L247:
        return _local2 + _local6;
      L252:
        if (!(_local4.length > 1)) goto L319;
        if (_local4[1].length != 1) goto L299;
        return _local2 + _local3 + "." + _local4[1] + "0";
      L299:
        return _local2 + _local3 + "." + _local4[1];
      L319:
        return _local2 + _local3;
    }

    function getAssets(arg1:String):Texture {
        var _local2 = create(arg1);
        if (!(_local2 is Bitmap)) goto L41;
        return Texture.fromBitmap(_local2 as Bitmap, true, false, this.scale);
      L41:
        if (!(_local2 is ByteArray)) goto L67;
        return Texture.fromAtfData(_local2 as ByteArray, this.scale);
      L67:
        return Texture.fromAtfData(_local2 as ByteArray, this.scale);
    }

    function getBitmap(arg1:String):Bitmap {
        var _local2 = create(arg1);
        return _local2 as Bitmap;
    }

    function getXml(arg1:String):XML {
        return XML(create(arg1));
    }

    function getSounds(arg1:String):Sound {
        return create(arg1) as Sound;
    }

    function SetMute(arg1:Boolean, arg2:Boolean = true):void {
        if (!arg1) goto L31;
        this._soundAsset.SetMusicVolume(0);
        this._soundAsset.SetSFXVolume(0);
        goto L51;
      L31:
        this._soundAsset.SetMusicVolume(1);
        this._soundAsset.SetSFXVolume(1);
      L51:
        if (!arg2) goto L66;
        this._muteMusic = arg1;
        this._muteSound = arg1;
      L66:
    }

    function get MuteMusic():Boolean {
        return this._muteMusic;
    }

    function set MuteMusic(arg1:Boolean):void {
        if (!arg1) goto L21;
        this._soundAsset.SetMusicVolume(0);
        goto L31;
      L21:
        this._soundAsset.SetMusicVolume(1);
      L31:
        this._muteMusic = arg1;
    }

    function get MuteSound():Boolean {
        return this._muteSound;
    }

    function set MuteSound(arg1:Boolean):void {
        if (!arg1) goto L21;
        this._soundAsset.SetSFXVolume(0);
        goto L31;
      L21:
        this._soundAsset.SetSFXVolume(1);
      L31:
        this._muteSound = arg1;
    }
}
}
