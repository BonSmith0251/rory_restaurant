// Ported from MiniGameMakeDessert.as (decompiled). Frozen-yogurt / dessert making minigame screen.
using System;
using System.Collections.Generic;
using Starlite;

public class MiniGameMakeDessert : Screen
{
    public Global _global;
    public bool hasWindow;
    public bool paused;
    public TextureAtlas dessertMakingAtlas;
    public int goalTarget;
    public int expertTarget;
    public int froyoMadeCount;
    public const int CUP = 1;
    public const int YOGURT = 2;
    public const int SYRUP = 3;
    public const int TOPPING = 4;
    public const int BEVERAGE = 5;
    public const int PAUSE_BUTTON = 1;
    public const int SOUND_BUTTON = 2;
    public const int MUSIC_BUTTON = 3;
    public bool initVector;
    public cSprite _background;
    public cSprite onPlateObj;
    public Quad tile;
    public cSprite preview;
    public List<cSprite> onPlateObjAr;
    public List<Quad> tileAr;
    public List<cSprite> previewAr;
    public cSprite pan;
    public cSprite previewPaper;
    public cSprite customer;
    public cSprite custBG;
    public cSprite custGlass;
    public cSprite custFill;
    public Sprite custFillSprite;
    public cSprite heart;
    public TNonStateButton pauseBtn;
    public TNonStateButton soundBtn;
    public TNonStateButton musicBtn;
    public cSprite mute;
    public cSprite muteMusic;
    public List<TNonStateButton> allButtonAr;
    public cSprite timeBG;
    public cSprite timeFill;
    public cSprite timeGlass;
    public Sprite timeFillSprite;
    public float TIMER_WIDTH = float.NaN;
    public float CUST_FILL_WIDTH = float.NaN;
    public cSprite scorePreview;
    public ExtendTextField scorePreviewText;
    public ExtendTextField goalOrExpertPreviewText;
    public ExtendTextField goalOrExpertText;
    public ExtendTextField livesText;
    public ExtendTextField bigInfoText;
    public bool orderInPlace;
    public List<int> orderList;
    public List<int> choosenSet;
    public int currentLayer;
    public int lives;
    public int level;
    public string earning;
    public int price;
    public int totalTime;
    public int time;
    public bool timesUp;
    public int patience;
    public int totalPatience;
    public TimelineLite timeline;
    public List<float> posX;
    public List<float> posY;
    public List<string> itemName;
    public List<int> type;
    public List<List<int>> availableSet;
    public List<int> setsLevel;
    public List<int> itemPrice;

    public MiniGameMakeDessert()
    {
    }

    public override void Start()
    {
        base.Start();
        this._global.currentSceneID = this._global.MAKE_DESSERT_ID;
        this.level = (int)((float)(TGlobalClass.currentPlayedLevel + 1) / 14 + 1);
        if (TGlobalClass.extrasMode)
        {
            this.level = 1;
            this.lives = 3;
        }
        if (!this.initVector)
        {
            this.onPlateObjAr = new List<cSprite>();
            this.tileAr = new List<Quad>();
            this.previewAr = new List<cSprite>();
            this.posX = new List<float>();
            this.posY = new List<float>();
            this.itemName = new List<string>();
            this.type = new List<int>();
            this.orderList = new List<int>();
            this.availableSet = new List<List<int>>();
            this.setsLevel = new List<int>();
            this.itemPrice = new List<int>();
            this.allButtonAr = new List<TNonStateButton>();
            this.initVector = true;
        }
        this._global.assetManager.enqueue(this._global.graphicsPath + "make_dessert.ean", this._global.graphicsPath + "make_dessert.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "Make_Dessert_BG.eag");
        this._global.assetManager.enqueue("data/MakeDessert.xml");
        this._global.assetManager.loadQueue((float f1_arg1) =>
        {
            if (f1_arg1 == 1)
            {
                this.readXML();
                this.initialize();
            }
        });
    }

    public void readXML()
    {
        XML xml = this._global.assetManager.getXml("MakeDessert");
        if (this.itemName.Count <= 0)
        {
            for (int i = 0; i < xml["item"].length(); i++)
            {
                XML item = xml["item"][i];
                this.posX.Add(item.attribute("posX").toFloat());
                this.posY.Add(item.attribute("posY").toFloat());
                this.itemName.Add(item.attribute("name"));
                this.itemPrice.Add(item.attribute("worth").toInt());
                this.type.Add(item.attribute("type").toInt());
            }
        }
        this.setLevelSet();
    }

    public void setLevelSet()
    {
        XML xml = this._global.assetManager.getXml("MakeDessert");
        while (this.availableSet.Count > 0)
        {
            this.availableSet.pop();
        }
        for (int i = 0; i < xml["sets"].length(); i++)
        {
            if (xml["sets"][i].attribute("level").toFloat() <= this.level)
            {
                var set = new List<int>();
                foreach (string s in xml["sets"][i].attribute("type").toString().split(",")) set.Add(AS3.toInt(s));
                this.availableSet.Add(set);
            }
        }
    }

    public override void Kill()
    {
        this.timeline.clear();
        this.timeline = null;
        for (int i = 0; i < this.onPlateObjAr.Count; i++)
        {
            this.onPlateObjAr[i].removeFromParent(true);
            this.onPlateObjAr[i] = null;
        }
        this.onPlateObjAr.Clear();
        for (int i = 0; i < this.tileAr.Count; i++)
        {
            this.tileAr[i].removeFromParent(true);
            this.tileAr[i] = null;
        }
        this.tileAr.Clear();
        for (int i = 0; i < this.allButtonAr.Count; i++)
        {
            this.allButtonAr[i].removeEventListener(Event.TRIGGERED, this.touchButton);
            this.allButtonAr[i].removeFromParent(true);
            this.allButtonAr[i] = null;
        }
        this.allButtonAr.Clear();
        for (int i = 0; i < this.previewAr.Count; i++)
        {
            this.previewAr[i].removeFromParent(true);
            this.previewAr[i] = null;
        }
        this.previewAr.Clear();
        while (this.availableSet.Count > 0)
        {
            this.availableSet.pop();
        }
        this._background.removeFromParent(true);
        this._background = null;
        this.onPlateObj.removeFromParent(true);
        this.onPlateObj = null;
        this.tile.removeFromParent(true);
        this.tile = null;
        this.preview.removeFromParent(true);
        this.preview = null;
        this.previewPaper.removeFromParent(true);
        this.previewPaper = null;
        this.pan.removeFromParent(true);
        this.pan = null;
        this.customer.removeFromParent(true);
        this.customer = null;
        this.custBG.removeFromParent(true);
        this.custBG = null;
        this.custFill.removeFromParent(true);
        this.custFill = null;
        this.custGlass.removeFromParent(true);
        this.custGlass = null;
        this.custFillSprite.removeFromParent(true);
        this.custFillSprite = null;
        this.heart.removeFromParent(true);
        this.heart = null;
        this.pauseBtn.removeFromParent(true);
        this.pauseBtn = null;
        this.soundBtn.removeFromParent(true);
        this.soundBtn = null;
        this.mute.removeFromParent(true);
        this.mute = null;
        this.musicBtn.removeFromParent(true);
        this.musicBtn = null;
        this.muteMusic.removeFromParent(true);
        this.muteMusic = null;
        this.timeBG.removeFromParent(true);
        this.timeBG = null;
        this.timeFill.removeFromParent(true);
        this.timeFill = null;
        this.timeGlass.removeFromParent(true);
        this.timeGlass = null;
        this.timeFillSprite.removeFromParent(true);
        this.timeFillSprite = null;
        this.scorePreview.removeFromParent(true);
        this.scorePreview = null;
        this.scorePreviewText.removeFromParent(true);
        this.scorePreviewText = null;
        this.goalOrExpertPreviewText.removeFromParent(true);
        this.goalOrExpertPreviewText = null;
        this.goalOrExpertText.removeFromParent(true);
        this.goalOrExpertText = null;
        if (this.livesText != null)
        {
            this.livesText.removeFromParent(true);
        }
        this.livesText = null;
        if (this.bigInfoText != null)
        {
            this.bigInfoText.removeFromParent(true);
        }
        this.bigInfoText = null;
        this.dessertMakingAtlas.dispose();
        this.dessertMakingAtlas = null;
        this._global.assetManager.purge();
        TGlobalClass._windowDisplay.hideAllWindows(this._global.currentSceneID);
        TGlobalClass._windowDisplay.kill(this._global.currentSceneID);
        ready = false;
    }

    public void dialogueCondition(int arg1, int arg2)
    {
    }

    public override void Update()
    {
        if (!ready) return;
        if (this.hasWindow) return;
        if (this.paused) return;
        if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_11) return;
        if (!this.timesUp && !this._global.tutorial.textBox.visible)
        {
            this.time = this.time - 1;
            setClipRectX(this.timeFillSprite, (float)this.time / this.totalTime * this.TIMER_WIDTH - this.TIMER_WIDTH);
            if (this.time <= 0)
            {
                this.timesUp = true;
            }
        }
        if (this.orderInPlace && !this._global.tutorial.textBox.visible)
        {
            this.patience = this.patience - 1;
            setClipRectX(this.custFillSprite, (float)this.patience / this.totalPatience * this.CUST_FILL_WIDTH - this.CUST_FILL_WIDTH);
            if (this.patience <= 0)
            {
                if (TGlobalClass.extrasMode)
                {
                    this.lives = this.lives - 1;
                    this.livesText.text = "LIVES x" + this.lives;
                    this.livesText.scaleY = 0;
                    this.livesText.scaleX = 0;
                    if (this.lives <= 0)
                    {
                        this.timesUp = true;
                    }
                    TweenLite.to(this.livesText, 1, new Vars
                    {
                        ["scaleX"] = 1,
                        ["scaleY"] = 1,
                        ["ease"] = Back.easeOut,
                        ["onComplete"] = (Action)(() =>
                        {
                            if (this.lives <= 0)
                            {
                                TGlobalClass._windowDisplay.showWindow(TWindowState.DESSERT_LEVEL_FAILED_EXTRAS);
                            }
                        }),
                    });
                }
                this._global._soundAsset.playSFX("bell_dring");
                this.orderInPlace = false;
                this.tweenOut();
                for (int i = 0; i < this.previewAr.Count; i++)
                {
                    if (this.previewAr[i].visible)
                    {
                        this.previewAr[i].visible = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// AS3 mutated sprite.clipRect.x in place (Starling reads it at render time). Starlite only syncs its mask in the
    /// clipRect setter, so the modified rect is assigned back.
    /// </summary>
    static void setClipRectX(Sprite s, float x)
    {
        Rectangle r = s.clipRect;
        r.x = x;
        s.clipRect = r;
    }

    public override void Resume()
    {
        if (!TGlobalClass._windowDisplay._blackScreen.visible)
        {
            this.paused = false;
            base.Resume();
            if (this.timeline != null)
            {
                this.timeline.resume();
            }
        }
    }

    public override void Pause()
    {
        if (!TGlobalClass._windowDisplay._blackScreen.visible)
        {
            this.paused = true;
            if (this.timeline != null)
            {
                this.timeline.pause();
            }
            TGlobalClass._windowDisplay.showWindow(TWindowState.PAUSE_GAME);
        }
    }

    public void initialize()
    {
        this._global.ReportGameState(this._global.GAMESTATE_PLAYING);
        if (TGlobalClass.musicPlayingIdx != 3)
        {
            this._global._soundAsset.playMusic("sunny-bliss");
            TGlobalClass.musicPlayingIdx = 3;
        }
        if (!TGlobalClass.extrasMode)
        {
            GALib.TrackEvent(GALib.EVENT_START_LEVEL_STORY, TGlobalClass.currentPlayedLevel + 1, "DESSERT MAKING");
        }
        else
        {
            GALib.TrackEvent(GALib.EVENT_START_LEVEL_EXTRAS, this.level, "DESSERT MAKING");
        }
        this.timeline = new TimelineLite();
        this.earning = "a";
        this.froyoMadeCount = 0;
        this.timesUp = false;
        if (!TGlobalClass.extrasMode)
        {
            XML day = TGlobalClass.levelSetXML["day"][TGlobalClass.currentPlayedLevel];
            this.time = AS3.toInt(day["time"].toFloat() * 30);
            this.totalTime = this.time;
            this.goalTarget = day["goal"].toInt();
            this.expertTarget = day["expert"].toInt();
        }
        else
        {
            XML dessert = TGlobalClass.levelSetXML["dessert"][this.level - 1];
            this.time = AS3.toInt(dessert["time"].toFloat() * 30);
            this.totalTime = this.time;
            this.goalTarget = dessert["goal"].toInt();
        }
        this.dessertMakingAtlas = this._global.assetManager.getTextureAtlas("make_dessert");
        this._background = new cSprite(this._global.assetManager.getTexture("Make_Dessert_BG"));
        this._background.touchable = false;
        this._background.blendMode = BlendMode.NONE;
        this._global._gameS.addChild(this._background);
        this.pauseBtn = new TNonStateButton(this.dessertMakingAtlas.getTexture("pause_button"));
        this.pauseBtn.scaleY = 0.7f;
        this.pauseBtn.scaleX = 0.7f;
        this.pauseBtn.x = 5;
        this.pauseBtn.y = 5;
        this.pauseBtn.idx = PAUSE_BUTTON;
        this._global._gameS.addChild(this.pauseBtn);
        this.allButtonAr.Add(this.pauseBtn);
        this.soundBtn = new TNonStateButton(this.dessertMakingAtlas.getTexture("sound_button"));
        this.soundBtn.scaleY = 0.7f;
        this.soundBtn.scaleX = 0.7f;
        this.soundBtn.x = 48;
        this.soundBtn.y = 5;
        this.soundBtn.idx = SOUND_BUTTON;
        this._global._gameS.addChild(this.soundBtn);
        this.allButtonAr.Add(this.soundBtn);
        this.mute = new cSprite(this.dessertMakingAtlas.getTexture("mute_icon"));
        this.mute.pivotX = this.mute.width / 2;
        this.mute.pivotY = this.mute.height / 2;
        this.mute.x = this.soundBtn.x + this.soundBtn.width / 2 - this.mute.pivotX;
        this.mute.y = this.soundBtn.y + this.soundBtn.height / 2 - this.mute.pivotY;
        this.mute.touchable = false;
        this.mute.visible = this._global.MuteSound;
        this._global._gameS.addChild(this.mute);
        this.musicBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("music_icon"));
        this.musicBtn.scaleY = 0.7f;
        this.musicBtn.scaleX = 0.7f;
        this.musicBtn.x = 91;
        this.musicBtn.y = 5;
        this.musicBtn.idx = MUSIC_BUTTON;
        this._global._gameS.addChild(this.musicBtn);
        this.allButtonAr.Add(this.musicBtn);
        this.muteMusic = new cSprite(this.dessertMakingAtlas.getTexture("mute_icon"));
        this.muteMusic.pivotX = this.muteMusic.width / 2;
        this.muteMusic.pivotY = this.muteMusic.height / 2;
        this.muteMusic.x = this.musicBtn.x + this.musicBtn.width / 2 - this.muteMusic.pivotX;
        this.muteMusic.y = this.musicBtn.y + this.musicBtn.height / 2 - this.muteMusic.pivotY;
        this.muteMusic.touchable = false;
        this.muteMusic.visible = this._global.MuteMusic;
        this._global._gameS.addChild(this.muteMusic);
        this.scorePreview = new cSprite(this.dessertMakingAtlas.getTexture("score_board"));
        this.scorePreview.x = 730;
        this.scorePreview.touchable = false;
        this._global._gameS.addChild(this.scorePreview);
        this.scorePreviewText = new ExtendTextField(150, 50, "$ 0", "jenna_sue", 35);
        this.scorePreviewText.x = 870;
        this.scorePreviewText.touchable = false;
        this._global._gameS.addChild(this.scorePreviewText);
        this.goalOrExpertPreviewText = new ExtendTextField(150, 50, "$ " + this.goalTarget, "jenna_sue", 35, 16777215);
        this.goalOrExpertPreviewText.x = 725;
        this.goalOrExpertPreviewText.hAlign = "right";
        this.goalOrExpertPreviewText.touchable = false;
        this._global._gameS.addChild(this.goalOrExpertPreviewText);
        this.goalOrExpertText = new ExtendTextField(150, 50, "Goal:", "jenna_sue", 25, 16777215);
        this.goalOrExpertText.x = 740;
        this.goalOrExpertText.hAlign = "left";
        this.goalOrExpertText.touchable = false;
        this.goalOrExpertText.idx = 1;
        this._global._gameS.addChild(this.goalOrExpertText);
        if (!TGlobalClass.extrasMode && TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel])
        {
            this.goalOrExpertPreviewText.text = "$ " + this.expertTarget;
            this.goalOrExpertText.text = "EXPERT:";
            this.goalOrExpertText.idx = 2;
        }
        this.timeBG = new cSprite(this.dessertMakingAtlas.getTexture("timer_BG"));
        this.timeBG.x = 200;
        this.timeBG.y = 0;
        this.timeBG.touchable = false;
        this._global._gameS.addChild(this.timeBG);
        this.timeFill = new cSprite(this.dessertMakingAtlas.getTexture("timer_fill"));
        this.TIMER_WIDTH = this.timeFill.width;
        this.timeFill.touchable = false;
        this.timeFillSprite = new Sprite();
        this.timeFillSprite.addChild(this.timeFill);
        this.timeFillSprite.x = this.timeBG.x + 73;
        this.timeFillSprite.y = 4;
        this._global._gameS.addChild(this.timeFillSprite);
        this.timeFillSprite.clipRect = new Rectangle(0, 0, this.timeFill.width, this.timeFill.height);
        this.timeGlass = new cSprite(this.dessertMakingAtlas.getTexture("timer_glass"));
        this.timeGlass.x = this.timeBG.x + 49;
        this.timeGlass.y = -7;
        this.timeGlass.touchable = false;
        this._global._gameS.addChild(this.timeGlass);
        this.previewPaper = new cSprite(this.dessertMakingAtlas.getTexture("preview_list"));
        this.previewPaper.touchable = false;
        this.previewPaper.x = 1024 - this.previewPaper.width;
        this.previewPaper.y = 50;
        this._global._gameS.addChild(this.previewPaper);
        this.customer = new cSprite(this.dessertMakingAtlas.getTexture("char_1"));
        this.customer.touchable = false;
        this.customer.pivotY = this.customer.height;
        this.customer.y = 255;
        this.customer.x = -250;
        this._global._gameS.addChild(this.customer);
        this.custBG = new cSprite(this.dessertMakingAtlas.getTexture("cust_bar_BG"));
        this.custBG.touchable = false;
        this.custBG.y = 240;
        this.custBG.x = -250;
        this._global._gameS.addChild(this.custBG);
        this.heart = new cSprite(this.dessertMakingAtlas.getTexture("heart_cust"));
        this.heart.pivotX = this.heart.width / 2;
        this.heart.pivotY = this.heart.height / 2;
        this.heart.x = -250;
        this.heart.y = this.custBG.y + 22;
        this.heart.touchable = false;
        this._global._gameS.addChild(this.heart);
        this.custFillSprite = new Sprite();
        this.custFill = new cSprite(this.dessertMakingAtlas.getTexture("cust_fill"));
        this.CUST_FILL_WIDTH = this.custFill.width;
        this.custFillSprite.addChild(this.custFill);
        this.custFillSprite.x = -250;
        this.custFillSprite.y = this.custBG.y + 15;
        this._global._gameS.addChild(this.custFillSprite);
        this.custFillSprite.clipRect = new Rectangle(0, 0, this.custFill.width, this.custFill.height);
        this.custGlass = new cSprite(this.dessertMakingAtlas.getTexture("cust_glass"));
        this.custGlass.x = -250;
        this.custGlass.y = this.custBG.y + 15;
        this.custGlass.touchable = false;
        this._global._gameS.addChild(this.custGlass);
        this.pan = new cSprite(this.dessertMakingAtlas.getTexture("pan"));
        this.pan.touchable = false;
        this.pan.pivotX = this.pan.width / 2;
        this.pan.pivotY = this.pan.height / 2;
        this.pan.x = -1000;
        this.pan.y = 300;
        this._global._gameS.addChild(this.pan);
        for (int i = 0; i < 15; i++)
        {
            this.onPlateObj = new cSprite(this.dessertMakingAtlas.getTexture("cup_paper"));
            this.onPlateObj.pivotX = this.onPlateObj.width / 2;
            this.onPlateObj.pivotY = this.onPlateObj.height / 2;
            this.onPlateObj.x = -200;
            this.onPlateObj.touchable = false;
            this._global._gameS.addChild(this.onPlateObj);
            this.onPlateObjAr.Add(this.onPlateObj);
        }
        int col = 0;
        int row = 0;
        for (int i = 0; i < 14; i++)
        {
            this.tile = new Quad(100, 100);
            this.tile.pivotX = this.tile.width / 2;
            this.tile.pivotY = this.tile.height / 2;
            this.tile.x = 132 + col * 126;
            this.tile.y = 426 + row * 115;
            this.tile.alpha = 0;
            this.tile.name = "" + i;
            this.tile.useHandCursor = true;
            this._global._gameS.addChild(this.tile);
            this.tileAr.Add(this.tile);
            col++;
            if (col == 7)
            {
                col = 0;
                row++;
            }
        }
        for (int i = 0; i < 15; i++)
        {
            this.preview = new cSprite(this.dessertMakingAtlas.getTexture("cup_paper"));
            this.preview.pivotX = this.preview.width / 2;
            this.preview.pivotY = this.preview.height / 2;
            this.preview.x = -300;
            this.preview.idx = i;
            this.preview.touchable = false;
            this.previewAr.Add(this.preview);
            this._global._gameS.addChild(this.preview);
        }
        if (TGlobalClass.extrasMode)
        {
            this.livesText = new ExtendTextField(250, 70, "LIVES x" + this.lives, "KomikandyGold", 20, 16777215);
            this.livesText.pivotX = 125;
            this.livesText.pivotY = 35;
            this.livesText.x = 425;
            this.livesText.y = 65;
            this._global._gameS.addChild(this.livesText);
        }
        TGlobalClass._windowDisplay.initializeWindow(this._global.MAKE_DESSERT_ID);
        if (TGlobalClass.extrasMode)
        {
            this.bigInfoText = new ExtendTextField(600, 300, "", "bromine", 70, 16777215);
            this.bigInfoText.pivotX = 300;
            this.bigInfoText.pivotY = 150;
            this.bigInfoText.x = -400;
            this.bigInfoText.y = 300;
            this._global._gameS.addChild(this.bigInfoText);
        }
        this._global._gameS.setChildIndex(this._global.psCongrats, this._global._gameS.numChildren - 1);
        this._global.psCongrats.stop();
        Starling.juggler.add(this._global.psCongrats);
        TGlobalClass.currentNotifWindow = 0;
        if (!TGlobalClass.extrasMode)
        {
            TGlobalClass._windowDisplay.showBeginningWindow(TGlobalClass.currentPlayedLevel, TGlobalClass.currentNotifWindow);
        }
        else
        {
            this.showCurrentLevel();
        }
        for (int i = 0; i < this.tileAr.Count; i++)
        {
            this.tileAr[i].addEventListener(TouchEvent.TOUCH, this.touchIngredient);
        }
        for (int i = 0; i < this.allButtonAr.Count; i++)
        {
            this.allButtonAr[i].addEventListener(Event.TRIGGERED, this.touchButton);
        }
        this.orderInPlace = false;
        if (!TGlobalClass.extrasMode)
        {
            this._global.tutorial.checkTut(Tutorial.LEVEL_11, 10);
            this._global.tutorial.checkTut(Tutorial.LEVEL_25, 24, true);
        }
        this.hasWindow = true;
        this.paused = false;
        ready = true;
    }

    public void touchButton(Event arg1)
    {
        int idx = (arg1.currentTarget as TNonStateButton).idx;
        if (idx == PAUSE_BUTTON)
        {
            this.paused = true;
            TGlobalClass._windowDisplay.showWindow(TWindowState.PAUSE_GAME);
        }
        else if (idx == SOUND_BUTTON)
        {
            this._global.MuteSound = !this._global.MuteSound;
            this.mute.visible = this._global.MuteSound;
        }
        else if (idx == MUSIC_BUTTON)
        {
            this._global.MuteMusic = !this._global.MuteMusic;
            this.muteMusic.visible = this._global.MuteMusic;
        }
    }

    public void startLevel()
    {
        this.hasWindow = false;
        // the timeline created empty in initialize() has already completed; like GreenSock, Starlite re-enables it
        // when tweenIn() adds children
        this.tweenIn();
        this.timeline.vars["onComplete"] = (Action)this.checkTimesUp;
    }

    public void showCurrentLevel()
    {
        this.bigInfoText.text = "LEVEL: " + this.level;
        TGlobalClass._windowDisplay._blackScreen.visible = true;
        this.bigInfoText.x = -400;
        TweenLite.to(this.bigInfoText, 1, new Vars
        {
            ["x"] = 512,
            ["ease"] = Expo.easeOut,
            ["onComplete"] = (Action)(() =>
            {
                TweenLite.to(this.bigInfoText, 1, new Vars
                {
                    ["x"] = 1424,
                    ["ease"] = Expo.easeIn,
                    ["onComplete"] = (Action)(() =>
                    {
                        TGlobalClass._windowDisplay._blackScreen.visible = false;
                        if (this.hasWindow)
                        {
                            this.startLevel();
                            this.hasWindow = false;
                        }
                    }),
                });
            }),
        });
    }

    public void setOrder()
    {
        int toppingCount = 0;
        int beverageCount = 0;
        int slot = 0;
        bool hasSyrup = false;
        List<int> options = null;
        int pick = 0;
        int roll = 0;
        var toppings = new List<int> { 4, 5, 11, 12 };
        var beverages = new List<int> { 6, 13 };
        this.orderList.Clear();
        this.currentLayer = 0;
        this.price = 0;
        this.choosenSet = this.availableSet[TGlobalClass.RandomRange(0, this.availableSet.Count - 1)];
        this.totalPatience = 2;
        setClipRectX(this.custFillSprite, 0);
        if (this.level >= 4 && this.choosenSet.indexOf(5) == -1)
        {
            roll = TGlobalClass.RandomRange(0, 100);
            if (roll <= 100)
            {
                this.choosenSet.Add(5);
            }
            if (this.level >= 5)
            {
                roll = TGlobalClass.RandomRange(0, 100);
                if (roll <= 100)
                {
                    this.choosenSet.Add(5);
                }
            }
        }
        if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_11)
        {
            this.choosenSet = new List<int> { 1, 2, 3, 4, 5 };
        }
        for (int i = 0; i < this.choosenSet.Count; i++)
        {
            int kind = this.choosenSet[i];
            if (kind == CUP)
            {
                options = new List<int> { 0, 7 };
                if (this.level < 5)
                {
                    this.orderList.Add(0);
                }
                else
                {
                    this.orderList.Add(options[TGlobalClass.RandomRange(0, options.Count - 1)]);
                }
            }
            else if (kind == YOGURT)
            {
                options = new List<int> { 1, 2, 8 };
                if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_11)
                {
                    options = new List<int> { 1 };
                }
                this.orderList.Add(options[TGlobalClass.RandomRange(0, options.Count - 1)]);
            }
            else if (kind == SYRUP)
            {
                options = new List<int> { 3, 9, 10 };
                if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_11)
                {
                    options = new List<int> { 3 };
                }
                this.orderList.Add(options[TGlobalClass.RandomRange(0, options.Count - 1)]);
                hasSyrup = true;
            }
            else if (kind == TOPPING)
            {
                pick = TGlobalClass.RandomRange(0, toppings.Count - 1);
                if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_11)
                {
                    this.orderList.Add(4);
                }
                else
                {
                    this.orderList.Add(toppings[pick]);
                }
                toppings.splice(pick, 1);
                toppingCount++;
            }
            else if (kind == BEVERAGE)
            {
                pick = TGlobalClass.RandomRange(0, beverages.Count - 1);
                if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_11)
                {
                    this.orderList.Add(6);
                }
                else
                {
                    this.orderList.Add(beverages[pick]);
                }
                beverages.splice(pick, 1);
                beverageCount++;
            }
            this.totalPatience = this.totalPatience + 1;
        }
        this.totalPatience = this.totalPatience * 60;
        this.patience = this.totalPatience;
        slot = 0;
        int baseX = (int)(this.previewPaper.x + this.previewPaper.width / 2 + 10);
        int baseY = (int)(this.previewPaper.y + this.previewPaper.height / 2 + 10);
        if (this.type[this.orderList[0]] == CUP)
        {
            if (beverageCount > 0 && toppingCount > 0)
            {
                baseX = baseX - 80;
            }
            else if (beverageCount > 0 || toppingCount > 0)
            {
                baseX = baseX - 40;
            }
        }
        for (int i = 0; i < this.orderList.Count; i++)
        {
            cSprite p = this.previewAr[i];
            int order = this.orderList[i];
            this.price = this.price + this.itemPrice[order];
            p.texture = this.dessertMakingAtlas.getTexture(this.itemName[order] + "_preview");
            p.readjustSize();
            p.scaleX = 1;
            p.scaleY = 1;
            p.visible = true;
            p.pivotX = p.width / 2;
            p.pivotY = p.height / 2;
            p.x = baseX;
            p.y = this.previewPaper.y + this.previewAr[0].height + (order == 7 ? -10 : 10);
            if (!hasSyrup)
            {
                p.y = p.y + 40;
            }
            if (this.type[this.orderList[0]] == CUP)
            {
                int kind = this.type[order];
                if (kind == YOGURT)
                {
                    p.y = p.y + (55 + (order == 7 ? 10 : 0));
                }
                else if (kind == SYRUP)
                {
                    p.y = p.y + (120 + (order == 7 ? 10 : 0));
                }
                else if (kind == TOPPING)
                {
                    p.x = baseX + 80;
                    p.y = baseY + slot * 50 - (float)((toppingCount - 1) * 50) / 2;
                    slot++;
                    if (slot >= toppingCount)
                    {
                        slot = 0;
                    }
                }
                else if (kind == BEVERAGE)
                {
                    if (toppingCount == 0)
                    {
                        p.x = baseX + 80;
                    }
                    else
                    {
                        p.x = baseX + 160;
                    }
                    p.y = slot * 95 + baseY - (float)((beverageCount - 1) * 95) / 2;
                    slot++;
                }
            }
            else
            {
                p.y = slot * 95 + baseY - (float)((beverageCount - 1) * 95) / 2;
                slot++;
            }
            p.visible = true;
        }
        for (int i = 0; i < this.orderList.Count; i++)
        {
            this.previewAr[i].scaleY = 0;
            this.previewAr[i].scaleX = 0;
            TweenLite.delayedCall(i * 0.1f, (Action<int>)this.popOrder, new object[] { i });
        }
        this.orderInPlace = true;
    }

    public void popOrder(int arg1)
    {
        this._global._soundAsset.playSFX("other_ingredient_matched_v3");
        TweenLite.to(this.previewAr[arg1], 0.2f, new Vars { ["scaleX"] = 1, ["scaleY"] = 1, ["ease"] = Back.easeOut });
    }

    public void touchIngredient(Event arg1)
    {
        var e = (TouchEvent)arg1;
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) == null) return;
        if (!this.orderInPlace) return;
        if (this._global.tutorial.button.visible) return;
        int id = AS3.toInt((e.currentTarget as Quad).name);
        if (!this._global.tutorial.canClickThisButton(id)) return;
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "touch_ingredient", true, false,
            new Dictionary<string, object> { ["id"] = id }));
        if (id != this.orderList[this.currentLayer])
        {
            this.currentLayer = 0;
            for (int i = 0; i < this.onPlateObjAr.Count; i++)
            {
                this.onPlateObjAr[i].visible = false;
            }
            this._global._soundAsset.playSFX("put_beverage_fries_on_tray_v2");
            return;
        }
        this._global._soundAsset.playSFX("other_ingredient_matched_v3");
        cSprite obj = this.onPlateObjAr[id];
        obj.texture = this.dessertMakingAtlas.getTexture(this.itemName[id]);
        obj.readjustSize();
        obj.visible = true;
        obj.pivotX = obj.width / 2;
        obj.pivotY = obj.height / 2;
        obj.x = this.posX[id];
        int lift = (this.currentLayer != 0 && this.orderList[0] == 7 && this.orderList[this.currentLayer] != 6 &&
                    this.orderList[this.currentLayer] != 13) ? 35 : 0;
        obj.y = this.posY[id] - 40 - lift;
        obj.scaleX = 0.9f;
        obj.scaleY = 0.5f;
        TweenLite.to(obj, 0.3f, new Vars { ["scaleX"] = 1, ["scaleY"] = 1, ["y"] = this.posY[id] - lift, ["ease"] = Back.easeOut });
        if (id != 6 && id != 13)
        {
            this._global._gameS.setChildIndex(obj, this._global._gameS.getChildIndex(TGlobalClass._windowDisplay._blackScreen) - 1);
        }
        this.currentLayer = this.currentLayer + 1;
        if (this.currentLayer < this.orderList.Count) return;

        this._global._soundAsset.playSFX("money_collected_v2");
        this.froyoMadeCount = this.froyoMadeCount + 1;
        this.earning = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(this.earning) + this.price);
        this.scorePreviewText.text = "$ " + this._global._maxCrypto.STI(this.earning);
        if (this._global._maxCrypto.STI(this.earning) >= this.goalTarget)
        {
            if (!TGlobalClass.extrasMode)
            {
                if (this.goalOrExpertText.idx == 1)
                {
                    TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = true;
                    this.goalOrExpertPreviewText.text = "$ " + this.expertTarget;
                    this.goalOrExpertText.text = "EXPERT:";
                    TGlobalClass._windowDisplay.showGoalIconSmall();
                    this._global._soundAsset.playSFX("goal_reached");
                    this.goalOrExpertText.idx = this.goalOrExpertText.idx + 1;
                }
                if (this.goalOrExpertText.idx == 2 && this._global._maxCrypto.STI(this.earning) >= this.expertTarget)
                {
                    TGlobalClass._windowDisplay.showGoalIconSmall();
                    this._global._soundAsset.playSFX("expert_reached");
                    this.goalOrExpertText.idx = 3;
                }
            }
            else
            {
                this.level = this.level + 1;
                if (this.level <= 5)
                {
                    this.showCurrentLevel();
                    XML dessert = TGlobalClass.levelSetXML["dessert"][this.level - 1];
                    this.time = AS3.toInt(dessert["time"].toFloat() * 30);
                    this.totalTime = this.time;
                    this.goalTarget = dessert["goal"].toInt();
                    this.goalOrExpertPreviewText.text = "$ " + this.goalTarget;
                    this.earning = "a";
                    this.scorePreviewText.text = "$ 0";
                    this.setLevelSet();
                }
                else
                {
                    this.timesUp = true;
                    TGlobalClass._windowDisplay.showWindow(TWindowState.DESSERT_LEVEL_COMPLETE_EXTRAS);
                }
            }
        }
        this.orderInPlace = false;
        TweenLite.delayedCall(0.4f, (Action)this.tweenOut);
        for (int i = 0; i < this.previewAr.Count; i++)
        {
            if (this.previewAr[i].visible)
            {
                this.previewAr[i].visible = false;
            }
        }
        var player = TSavegame.currentPlayer;
        if (!TGlobalClass.extrasMode && !player.unlocked[Achievement.FROYO_MASTER])
        {
            player.achievementCounter[Achievement.FROYO_MASTER] = player.achievementCounter[Achievement.FROYO_MASTER] + 1;
            if (player.achievementCounter[Achievement.FROYO_MASTER] > Achievement.ALL_TARGET[Achievement.FROYO_MASTER])
            {
                player.unlocked[Achievement.FROYO_MASTER] = true;
                Achievement.tweenAchievementNotif(Achievement.FROYO_MASTER);
            }
            else if (player.achievementCounter[Achievement.FROYO_MASTER] % 50 == 0)
            {
                Achievement.tweenAchievementProgress(Achievement.FROYO_MASTER);
            }
        }
    }

    public void tweenOut()
    {
        this.timeline.restart();
        this.timeline.to(this.pan, 0.8f, new Vars { ["x"] = this.pan.x - 1000, ["ease"] = Expo.easeIn }, 0);
        this.timeline.to(this.custBG, 0.8f, new Vars { ["x"] = -250, ["ease"] = Expo.easeIn }, 0);
        this.timeline.to(this.heart, 0.8f, new Vars { ["x"] = -250, ["ease"] = Expo.easeIn }, 0);
        this.timeline.to(this.custFillSprite, 0.8f, new Vars { ["x"] = -250, ["ease"] = Expo.easeIn }, 0);
        this.timeline.to(this.custGlass, 0.8f, new Vars { ["x"] = -250, ["ease"] = Expo.easeIn }, 0);
        for (int i = 0; i < this.onPlateObjAr.Count; i++)
        {
            if (this.onPlateObjAr[i].visible)
            {
                this.timeline.to(this.onPlateObjAr[i], 0.8f, new Vars { ["x"] = this.onPlateObjAr[i].x - 1000, ["ease"] = Expo.easeIn }, 0);
            }
        }
        this.timeline.to(this.customer, 1, new Vars
        {
            ["x"] = -250,
            ["ease"] = Expo.easeIn,
            ["onComplete"] = (Action)(() =>
            {
                int charIdx = TGlobalClass.RandomRange(1, 12);
                if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_11 && this._global.tutorial.dialogueCounter > 10)
                {
                    charIdx = 2;
                }
                this.customer.texture = this.dessertMakingAtlas.getTexture("char_" + charIdx);
                this.customer.readjustSize();
                setClipRectX(this.custFillSprite, 0);
            }),
        }, 0);
        if (!this.timesUp && this.time > 30)
        {
            this.tweenIn();
        }
    }

    public void tweenIn()
    {
        this.timeline.to(this.pan, 0.8f, new Vars { ["x"] = 450, ["ease"] = Expo.easeOut }, 1);
        this.timeline.to(this.custBG, 1, new Vars { ["x"] = 0, ["ease"] = Expo.easeOut }, 1);
        this.timeline.to(this.heart, 1, new Vars { ["x"] = 25, ["ease"] = Expo.easeOut }, 1);
        this.timeline.to(this.custFillSprite, 1, new Vars { ["x"] = 47, ["ease"] = Expo.easeOut }, 1);
        this.timeline.to(this.custGlass, 1, new Vars { ["x"] = 47, ["ease"] = Expo.easeOut }, 1);
        this.timeline.to(this.customer, 1, new Vars { ["x"] = -20, ["ease"] = Expo.easeOut }, 1);
    }

    public void checkTimesUp()
    {
        int earned = 0;
        int delayedNotifs = 0;
        int prevBigBusiness = 0;
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "check_times_up"));
        if (!this.timesUp)
        {
            this.setOrder();
        }
        else if (!TGlobalClass.extrasMode)
        {
            var player = TSavegame.currentPlayer;
            int lvl = TGlobalClass.currentPlayedLevel;
            delayedNotifs = 0;
            if (player.daysEarning[lvl] == 0 && lvl == player.passedDays)
            {
                if (this._global._maxCrypto.STI(this.earning) >= this.goalTarget)
                {
                    if (!player.unlocked[Achievement.QUALIFIED_CHEF])
                    {
                        player.achievementCounter[Achievement.QUALIFIED_CHEF] = player.achievementCounter[Achievement.QUALIFIED_CHEF] + 1;
                        if (player.achievementCounter[Achievement.QUALIFIED_CHEF] >= Achievement.ALL_TARGET[Achievement.QUALIFIED_CHEF])
                        {
                            player.unlocked[Achievement.QUALIFIED_CHEF] = true;
                            Achievement.tweenAchievementNotif(Achievement.QUALIFIED_CHEF);
                            delayedNotifs++;
                        }
                        else if (player.achievementCounter[Achievement.QUALIFIED_CHEF] % 15 == 0)
                        {
                            Achievement.tweenAchievementProgress(Achievement.QUALIFIED_CHEF);
                            delayedNotifs++;
                        }
                    }
                    if (this._global._maxCrypto.STI(this.earning) >= this.expertTarget)
                    {
                        bumpInternationalChef(delayedNotifs);
                        player.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(player.money) + 6);
                        TGlobalClass.coinsGot = 6;
                    }
                    else
                    {
                        player.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(player.money) + 3);
                        TGlobalClass.coinsGot = 3;
                    }
                    player.daysEarning[lvl] = this._global._maxCrypto.STI(this.earning);
                    player.passedDays = player.passedDays + 1;
                    earned = this._global._maxCrypto.STI(this.earning);
                    TSavegame.save();
                    TGlobalClass._windowDisplay.showWindow(TWindowState.DESSERT_LEVEL_COMPLETE);
                }
                else
                {
                    TGlobalClass._windowDisplay.showWindow(TWindowState.DESSERT_LEVEL_FAILED);
                }
            }
            else if (this._global._maxCrypto.STI(this.earning) >= this.goalTarget)
            {
                if (this._global._maxCrypto.STI(this.earning) > player.daysEarning[lvl])
                {
                    if (this._global._maxCrypto.STI(this.earning) >= this.expertTarget && player.daysEarning[lvl] < this.expertTarget)
                    {
                        bumpInternationalChef(delayedNotifs);
                        player.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(player.money) + 3);
                        TGlobalClass.coinsGot = 3;
                    }
                    TGlobalClass.daysEarningPrev = player.daysEarning[lvl];
                    earned = this._global._maxCrypto.STI(this.earning) - player.daysEarning[lvl];
                    player.daysEarning[lvl] = this._global._maxCrypto.STI(this.earning);
                    TSavegame.save();
                }
                TGlobalClass._windowDisplay.showWindow(TWindowState.DESSERT_LEVEL_COMPLETE);
            }
            else
            {
                TGlobalClass._windowDisplay.showWindow(TWindowState.DESSERT_LEVEL_FAILED);
            }
            if (!player.unlocked[Achievement.BIG_BUSINESS])
            {
                prevBigBusiness = player.achievementCounter[Achievement.BIG_BUSINESS];
                player.achievementCounter[Achievement.BIG_BUSINESS] = player.achievementCounter[Achievement.BIG_BUSINESS] + earned;
                if (player.achievementCounter[Achievement.BIG_BUSINESS] >= Achievement.ALL_TARGET[Achievement.BIG_BUSINESS])
                {
                    player.unlocked[Achievement.BIG_BUSINESS] = true;
                    Achievement.tweenAchievementNotif(Achievement.BIG_BUSINESS);
                }
                else if (prevBigBusiness % 20000 > player.achievementCounter[Achievement.BIG_BUSINESS] % 20000)
                {
                    Achievement.tweenAchievementProgress(Achievement.BIG_BUSINESS);
                }
            }
            TSavegame.save();
        }
        else if (TGlobalClass.extrasMode && this.level < 6)
        {
            TGlobalClass._windowDisplay.showWindow(TWindowState.DESSERT_LEVEL_FAILED_EXTRAS);
        }
        this.timeline.clear();
    }

    /// <summary>INTERNATIONAL_CHEF counter step shared by both checkTimesUp branches (notification delayed by 6 s per earlier one).</summary>
    void bumpInternationalChef(int delayedNotifs)
    {
        var player = TSavegame.currentPlayer;
        if (player.unlocked[Achievement.INTERNATIONAL_CHEF]) return;
        player.achievementCounter[Achievement.INTERNATIONAL_CHEF] = player.achievementCounter[Achievement.INTERNATIONAL_CHEF] + 1;
        if (player.achievementCounter[Achievement.INTERNATIONAL_CHEF] >= Achievement.ALL_TARGET[Achievement.INTERNATIONAL_CHEF])
        {
            player.unlocked[Achievement.INTERNATIONAL_CHEF] = true;
            TweenLite.delayedCall(delayedNotifs * 6, (Action<int>)Achievement.tweenAchievementNotif, new object[] { Achievement.INTERNATIONAL_CHEF });
        }
        else if (player.achievementCounter[Achievement.INTERNATIONAL_CHEF] % 15 == 0)
        {
            TweenLite.delayedCall(delayedNotifs * 6, (Action<int>)Achievement.tweenAchievementProgress, new object[] { Achievement.INTERNATIONAL_CHEF });
        }
    }
}
