// Ported from rudi.Global: singleton holding the engine, screens and asset managers.
using System;
using System.Collections.Generic;
using Starlite;

public class Global
{
    static Global _instance;
    public Game _game;
    public Sprite _gameS;
    public gameEngine _gameEngine;
    public Stage _stage;
    public SoundAsset _soundAsset;
    public object _context3D;
    public Starling _Starling;
    public maxCrypto _maxCrypto;
    public bool _onFade;
    public bool _isHandlingLostContext;
    public bool startHandling;
    public bool resizingStage;
    public MainMenuScreen _mainMenuScreen;
    public dumLoading _dumLoadingScreen;
    public GameScreen gameScreen;
    public Credits _creditsScreen;
    public Kitchen _kitchen;
    public LevelSelectScreen _levelSelectScreen;
    public Dish dish;
    public MiniGameHO _minigameHO;
    public MiniGameMakeDessert _minigameDessert;
    public Tutorial tutorial;
    public OpeningScreen _opening;
    public EndingClass _ending;
    public AchievementScreen _achievementScreen;
    public string graphicsPath;
    public string soundsPath;
    public int currentSceneID;
    // AS3 instance constants: kept as instance fields so "_global.MAINMENU_ID" still compiles
    public readonly int MAINMENU_ID = 1;
    public readonly int GAMESCREEN_ID = 2;
    public readonly int KITCHEN_ID = 3;
    public readonly int HIDDEN_OBJECT_ID = 4;
    public readonly int MAKE_DESSERT_ID = 5;
    public readonly int LEVEL_SELECT_ID = 6;
    public readonly int ACHIEVEMENTS_ID = 7;
    public readonly int OPENING_ID = 8;
    public readonly int ENDING_ID = 9;
    public bool loadGame;
    public bool newGame;
    public bool firstInit = false;
    public bool assetHD;
    public bool embedAsset;
    public bool assetEncrypt;
    public float scale;
    public bool initFlox;
    public AssetManager assetManager;
    public AssetManager unPurgedAsset;
    public PDParticleSystem ps;
    public PDParticleSystem psFound;
    public PDParticleSystem psCongrats;
    public bool _muteSound = false;
    public bool _muteMusic = false;
    public readonly string GAMESTATE_MAIN_MENU = "1";
    public readonly string GAMESTATE_PLAYING = "2";
    public readonly string GAMESTATE_GAME_END = "3";
    public readonly string GAMESTATE_LEVEL_SELECT = "4";
    public readonly string GAMESTATE_LEVEL_END = "5";

    Global() { }

    public static Global getInstance() => _instance ?? (_instance = new Global());

    public void init(bool arg1 = true, bool arg2 = false, bool arg3 = false)
    {
        if (firstInit) return;
        firstInit = true;
        assetEncrypt = arg3;
        assetHD = arg1;
        embedAsset = arg2;
        soundsPath = "data/sounds/";
        if (assetHD)
        {
            graphicsPath = "data/graphic/hd/";
            scale = 1;
        }
        else
        {
            graphicsPath = "data/graphic/sd/";
            scale = 0.8f;
        }
        assetManager = new AssetManager(scale);
        unPurgedAsset = new AssetManager(scale);
        _gameEngine = gameEngine.getInstance();
        _soundAsset = new SoundAsset();
        _maxCrypto = maxCrypto.getInstance();
        _soundAsset.firstInit();
        _mainMenuScreen = new MainMenuScreen();
        _mainMenuScreen._global = this;
        gameScreen = new GameScreen();
        gameScreen._global = this;
        _creditsScreen = new Credits();
        _creditsScreen._global = this;
        _kitchen = new Kitchen();
        _kitchen._global = this;
        _levelSelectScreen = new LevelSelectScreen();
        _levelSelectScreen._global = this;
        _minigameHO = new MiniGameHO();
        _minigameHO._global = this;
        _minigameDessert = new MiniGameMakeDessert();
        _minigameDessert._global = this;
        _opening = new OpeningScreen();
        _opening._global = this;
        _ending = new EndingClass();
        _ending._global = this;
        _achievementScreen = new AchievementScreen();
        _achievementScreen._global = this;
        _dumLoadingScreen = new dumLoading();
        _dumLoadingScreen._global = this;
        dish = new Dish();
        dish.init();
        _dumLoadingScreen.PreStart();
        _onFade = false;
    }

    public string getCurrentSceneNameByID(int arg1)
    {
        switch (arg1)
        {
            case 1: return "Main Menu";
            case 2: return "Match 3";
            case 3: return "Kitchen";
            case 4: return "Hidden Objects";
            case 5: return "Dessert Making";
            default: return "Unknown";
        }
    }

    public void ReportGameState(string arg1 = "") { }

    public string addCommas(float arg1) => TGlobalClass.addCommas(arg1);

    // Embedded-asset lookups were unused (EmbedAssets=false); kept for API compatibility.
    public Texture getAssets(string arg1) => AS3.embeddedTexture(arg1);
    public Bitmap getBitmap(string arg1) => new Bitmap(AS3.embeddedTexture(arg1));
    public XML getXml(string arg1) => AS3.embeddedXml(arg1);
    public Sound getSounds(string arg1) => null;

    public void SetMute(bool arg1, bool arg2 = true)
    {
        if (arg1)
        {
            _soundAsset.SetMusicVolume(0);
            _soundAsset.SetSFXVolume(0);
        }
        else
        {
            _soundAsset.SetMusicVolume(1);
            _soundAsset.SetSFXVolume(1);
        }
        if (arg2)
        {
            _muteMusic = arg1;
            _muteSound = arg1;
        }
    }

    public bool MuteMusic
    {
        get => _muteMusic;
        set
        {
            _soundAsset.SetMusicVolume(value ? 0 : 1);
            _muteMusic = value;
        }
    }

    public bool MuteSound
    {
        get => _muteSound;
        set
        {
            _soundAsset.SetSFXVolume(value ? 0 : 1);
            _muteSound = value;
        }
    }
}
