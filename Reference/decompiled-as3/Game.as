package {
class Game extends Sprite {
    static const jenna_sue:Class;
    static const komikandy:Class;
    static const bromine:Class;
    static const pfennig:Class;
    var timeRatio:Number;
    var _onFadeOutAc:Boolean;
    var _global:Global;
    var _container:Sprite;
    var _loading:Quad;
    var _loading2:TextField;
    var pauseGame:Boolean;
    var tMusicVolume:Number;
    var tSfxVolume:Number;
    var loader:Loader;

    // static initializer
    static {
        jenna_sue = Game_jenna_sue;
        komikandy = Game_komikandy;
        bromine = Game_bromine;
        pfennig = Game_pfennig;
    }

    function Game():* {
        this.loader = new Loader();
        super();
        this.addEventListener(Event.ADDED_TO_STAGE, this.onAddedToStage);
        this._global = Global.getInstance();
        this._container = new Sprite();
        this._global._game = this;
        this._global._gameS = this._container;
        this._global._game.addChild(this._global._gameS);
        TSavegame.init();
        TSavegame.loadAll();
        this._global.MuteSound = TSavegame.muteSound;
        this._global.MuteMusic = TSavegame.muteMusic;
        if (!(TSavegame.lastPlayer > 0)) goto L159;
        TSavegame.loadPlayer(TSavegame.lastPlayer - 1);
        goto L172;
      L159:
        TSavegame._saveGame.data.fullScreen = true;
      L172:
        this._global._stage.displayState = StageDisplayState.FULL_SCREEN_INTERACTIVE;
        TGlobalClass._windowDisplay = new TWindowDisplay();
        Font.registerFont(jenna_sue);
        Font.registerFont(komikandy);
        Font.registerFont(bromine);
        Font.registerFont(pfennig);
        this._loading = new Quad(1024, 600, 0);
        this._loading.touchable = false;
        this._loading.alpha = 0;
        this._loading2 = new TextField(1000, 100, "Now Loading...", "bromine", 50, 16777215);
        this._loading2.touchable = false;
        this._loading2.y = 510;
        this._loading2.x = 20;
        this._loading2.hAlign = "left";
        this._loading2.alpha = 0;
        this._global._game.addChild(this._loading);
        this._global._game.addChild(this._loading2);
        this.pauseGame = false;
    }

    function reCreate():void {
        this._loading2.text = "";
        this._loading2.text = "Now Loading...";
    }

    function fadeToScreen(arg1:Screen, arg2:Number = 1, arg3:Number = 1):void {
        if (!this._global._isHandlingLostContext) goto L21;
        this._global._onFade = false;
      L21:
        if (!this._global._onFade) goto L33;
        return;
      L33:
        this._global._onFade = true;
        this._global._gameEngine._nextScreen = arg1;
        this.tMusicVolume = arg2;
        this.tSfxVolume = arg3;
        this.timeRatio = 1;
        this._onFadeOutAc = true;
    }

    function enter_frame(arg1:Event):void {
        if (!this.pauseGame) goto L11;
        return;
      L11:
        this._global._gameEngine._currentScreen.Update();
        if (this._global._onFade) goto L37;
        return;
      L37:
        if (!this._onFadeOutAc) goto L176;
        this.timeRatio = this.timeRatio - 0.1;
        if (!(this.timeRatio < 0)) goto L172;
        this._global._gameEngine._currentScreen.Kill();
        System.gc();
        // try { (catch z:* -> L119)
        new LocalConnection().connect("foo");
        new LocalConnection().connect("foo");
        // } end try (handler L119)
        goto L133;
      L119: // catch (z:*)
        var _local2 = $catchscope;
        $catchscope.slot1 = $exception;
      L133:
        this._global._gameEngine._currentScreen = this._global._gameEngine._nextScreen;
        this._global._gameEngine._currentScreen.Start();
        this._onFadeOutAc = false;
      L172:
        goto L222;
      L176:
        if (!this._global._gameEngine._currentScreen.ready) goto L222;
        this.timeRatio = this.timeRatio + 0.1;
        if (!(this.timeRatio > 1)) goto L222;
        this._global._onFade = false;
      L222:
        if (this._global.MuteMusic) goto L279;
        this._global._soundAsset.SetSFXVolume(this.timeRatio / 1 * this.tSfxVolume);
        this._global._soundAsset.SetMusicVolume(this.timeRatio / 1 * this.tMusicVolume);
      L279:
        var $t0 = 1 - this.timeRatio;
        _local2 = $t0;
        this._loading2.alpha = $t0;
        this._loading.alpha = _local2;
    }

    function onAddedToStage(arg1:Event):void {
        this.removeEventListener(Event.ADDED_TO_STAGE, this.onAddedToStage);
        if (Settings.handleDeactivate != "true") goto L53;
        this._global._stage.addEventListener(Event.DEACTIVATE, this.onDeactivateHandler);
        goto L73;
      L53:
        this._global._stage.removeEventListener(Event.DEACTIVATE, this.onDeactivateHandler);
      L73:
        this._global._stage.addEventListener(KeyboardEvent.KEY_DOWN, this.onExit);
        this._global._gameEngine._currentScreen.Start();
        this._global._stage.addEventListener(Event.ENTER_FRAME, this.enter_frame);
    }

    function onAppClose(arg1:Event):void {
    }

    function onDeactivateHandler(arg1:Event):void {
        TSavegame.saveSetting();
        TGlobalClass.deactivate = true;
        this._global._stage.addEventListener(Event.ACTIVATE, this.onActivateHandler);
        this._global._soundAsset.PauseMusic();
        this._global._soundAsset.PauseSFX();
        var $t0 = this._global.currentSceneID == this._global.GAMESCREEN_ID;
        $s0 = $t0;
        if (!$t0) goto L90;
        $s0 = Boolean(this._global.gameScreen.ready);
      L90:
        if (!$s0) goto L105;
        this._global.gameScreen.pauseGame();
      L105:
        this._global._gameEngine._currentScreen.Pause();
        Starling.current.stop();
        this.pauseGame = true;
        if (!TSavegame.fullScreen) goto L165;
        TGlobalClass.minimized = true;
        this._global._stage.nativeWindow.minimize();
      L165:
    }

    function onActivateHandler(arg1:Event):void {
        TGlobalClass.deactivate = false;
        this._global._soundAsset.ResumeMusic();
        this._global._stage.removeEventListener(Event.ACTIVATE, this.onActivateHandler);
        var $t0 = this._global.currentSceneID == this._global.GAMESCREEN_ID;
        $s0 = $t0;
        if (!$t0) goto L72;
        $s0 = Boolean(this._global.gameScreen.ready);
      L72:
        if (!$s0) goto L87;
        this._global.gameScreen.resumeGame();
      L87:
        this._global._gameEngine._currentScreen.Resume();
        Starling.current.start();
        this.pauseGame = false;
        var $t1 = Boolean(TSavegame.fullScreen);
        $s0 = $t1;
        if (!$t1) goto L136;
        $s0 = Boolean(TGlobalClass.minimized);
      L136:
        if (!$s0) goto L177;
        TGlobalClass.minimized = false;
        this._global._stage.nativeWindow.activate();
        this._global._stage.displayState = StageDisplayState.FULL_SCREEN_INTERACTIVE;
      L177:
    }

    function onExit(arg1:KeyboardEvent):void {
        if (arg1.keyCode != Keyboard.ESCAPE) goto L21;
        arg1.preventDefault();
      L21:
        if (!this._global._onFade) goto L33;
        return;
      L33:
        if (arg1.keyCode != Keyboard.BACK) goto L123;
        var $t0 = this._global.currentSceneID == this._global.MAINMENU_ID;
        $s0 = $t0;
        if (!$t0) goto L79;
        $s0 = Boolean(this._global._mainMenuScreen._bolehKlik);
      L79:
        if (!$s0) goto L98;
        $s0 = !TGlobalClass._windowDisplay._blackScreen.visible;
      L98:
        if (!$s0) goto L118;
        TGlobalClass._windowDisplay.showWindow(TWindowState.EXIT_WINDOW);
      L118:
        arg1.preventDefault();
      L123:
        this._global._gameEngine._currentScreen.OnKeyDown(arg1.keyCode);
    }
}
}
