// Ported from Game (Starling root): screen switching with fade, loading overlay, focus pause/resume, Esc handling.
using System;
using Starlite;

public class Game : Sprite
{
    public float timeRatio;
    public bool _onFadeOutAc;
    public Global _global;
    public Sprite _container;
    public Quad _loading;
    public TextField _loading2;
    public bool pauseGame;
    public float tMusicVolume;
    public float tSfxVolume;

    public Game()
    {
        addEventListener(Event.ADDED_TO_STAGE, onAddedToStage);
        _global = Global.getInstance();
        _container = new Sprite();
        _global._game = this;
        _global._gameS = _container;
        _global._game.addChild(_global._gameS);
        TSavegame.init();
        TSavegame.loadAll();
        _global.MuteSound = TSavegame.muteSound;
        _global.MuteMusic = TSavegame.muteMusic;
        if (TSavegame.lastPlayer > 0) TSavegame.loadPlayer(TSavegame.lastPlayer - 1);
        else TSavegame._saveGame.data["fullScreen"] = true;
        _global._stage.displayState = StageDisplayState.FULL_SCREEN_INTERACTIVE;
        TGlobalClass._windowDisplay = new TWindowDisplay();
        _loading = new Quad(1024, 600, 0);
        _loading.touchable = false;
        _loading.alpha = 0;
        _loading2 = new TextField(1000, 100, "Now Loading...", "bromine", 50, 16777215);
        _loading2.touchable = false;
        _loading2.y = 510;
        _loading2.x = 20;
        _loading2.hAlign = "left";
        _loading2.alpha = 0;
        _global._game.addChild(_loading);
        _global._game.addChild(_loading2);
        pauseGame = false;
    }

    public void reCreate()
    {
        _loading2.text = "";
        _loading2.text = "Now Loading...";
    }

    public void fadeToScreen(Screen arg1, float arg2 = 1, float arg3 = 1)
    {
        if (_global._isHandlingLostContext) _global._onFade = false;
        if (_global._onFade) return;
        _global._onFade = true;
        _global._gameEngine._nextScreen = arg1;
        tMusicVolume = arg2;
        tSfxVolume = arg3;
        timeRatio = 1;
        _onFadeOutAc = true;
    }

    public void enter_frame(Event arg1)
    {
        if (pauseGame) return;
        _global._gameEngine._currentScreen.Update();
        if (!_global._onFade) return;
        if (_onFadeOutAc)
        {
            timeRatio = timeRatio - 0.1f;
            if (timeRatio < 0)
            {
                _global._gameEngine._currentScreen.Kill();
                _global._gameEngine._currentScreen = _global._gameEngine._nextScreen;
                _global._gameEngine._currentScreen.Start();
                _onFadeOutAc = false;
            }
        }
        else if (_global._gameEngine._currentScreen.ready)
        {
            timeRatio = timeRatio + 0.1f;
            if (timeRatio > 1) _global._onFade = false;
        }
        if (!_global.MuteMusic)
        {
            _global._soundAsset.SetSFXVolume(timeRatio / 1 * tSfxVolume);
            _global._soundAsset.SetMusicVolume(timeRatio / 1 * tMusicVolume);
        }
        float a = 1 - timeRatio;
        _loading2.alpha = a;
        _loading.alpha = a;
    }

    public void onAddedToStage(Event arg1)
    {
        removeEventListener(Event.ADDED_TO_STAGE, onAddedToStage);
        if (Settings.handleDeactivate == "true") _global._stage.addEventListener(Event.DEACTIVATE, onDeactivateHandler);
        else _global._stage.removeEventListener(Event.DEACTIVATE, onDeactivateHandler);
        _global._stage.addEventListener(KeyboardEvent.KEY_DOWN, onExit);
        _global._gameEngine._currentScreen.Start();
        _global._stage.addEventListener(Event.ENTER_FRAME, enter_frame);
    }

    public void onAppClose(Event arg1) { }

    public void onDeactivateHandler(Event arg1)
    {
        TSavegame.saveSetting();
        TGlobalClass.deactivate = true;
        _global._stage.addEventListener(Event.ACTIVATE, onActivateHandler);
        _global._soundAsset.PauseMusic();
        _global._soundAsset.PauseSFX();
        if (_global.currentSceneID == _global.GAMESCREEN_ID && _global.gameScreen.ready) _global.gameScreen.pauseGame();
        _global._gameEngine._currentScreen.Pause();
        Starling.current.stop();
        pauseGame = true;
        // the AIR build minimized its fullscreen window here; a Unity player just pauses
    }

    public void onActivateHandler(Event arg1)
    {
        TGlobalClass.deactivate = false;
        _global._soundAsset.ResumeMusic();
        _global._stage.removeEventListener(Event.ACTIVATE, onActivateHandler);
        if (_global.currentSceneID == _global.GAMESCREEN_ID && _global.gameScreen.ready) _global.gameScreen.resumeGame();
        _global._gameEngine._currentScreen.Resume();
        Starling.current.start();
        pauseGame = false;
        if (TSavegame.fullScreen && TGlobalClass.minimized)
        {
            TGlobalClass.minimized = false;
            _global._stage.displayState = StageDisplayState.FULL_SCREEN_INTERACTIVE;
        }
    }

    public void onExit(Event arg1)
    {
        var e = (KeyboardEvent)arg1;
        if (e.keyCode == Keyboard.ESCAPE) e.preventDefault();
        if (_global._onFade) return;
        if (e.keyCode == Keyboard.BACK)
        {
            if (_global.currentSceneID == _global.MAINMENU_ID && _global._mainMenuScreen._bolehKlik &&
                !TGlobalClass._windowDisplay._blackScreen.visible)
                TGlobalClass._windowDisplay.showWindow(TWindowState.EXIT_WINDOW);
            e.preventDefault();
        }
        _global._gameEngine._currentScreen.OnKeyDown((int)e.keyCode);
    }
}
