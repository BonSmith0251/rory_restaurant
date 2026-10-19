// Ported from MiniGameHO.as (decompiled). Hidden-object minigame screen (zoom/pan container, timer, hints, UI).
using System;
using System.Collections.Generic;
using Starlite;
using Mathf = UnityEngine.Mathf;

public class MiniGameHO : Screen
{
    // [Embed] MiniGameHO_FireConfig / MiniGameHO_FireParticle are loaded via AS3.embeddedXml / AS3.embeddedTexture.
    public Global _global;
    public bool initVector;
    public cSprite _background;
    public BitmapData bitmapData;
    public bool paused;
    public bool hasWindow;
    public TextureAtlas uiAtlas;
    public List<TweenLite> tweenAr;
    public const int HINT_RECOVER_TIME = 450;
    public const int PAUSE_BUTTON_IDX = 1;
    public const int SOUND_BUTTON_IDX = 2;
    public const int HINT_BUTTON_IDX = 3;
    public const int MUSIC_BUTTON_IDX = 4;
    public cSprite uiBG;
    public TNonStateButton soundBtn;
    public TNonStateButton musicBtn;
    public TNonStateButton pauseBtn;
    public cSprite mute;
    public cSprite muteMusic;
    public cSprite hintFill;
    public cSprite hintBG;
    public cSprite timeBG;
    public cSprite timeFill;
    public cSprite timeGlass;
    public Sprite timeFillSprite;
    public Sprite hintFillSprite;
    public TNonStateButton hintText;
    public ExtendTextField moneyText;
    public List<DisplayObject> buttonAr;
    public HOClass _HOClass;
    public Sprite zoomContainer;
    public bool touchDragged;
    public bool hasHint;
    public int hintTimer;
    public int timer;
    public int HINT_HEIGHT;
    public int TIMER_WIDTH;
    public int TOTAL_TIME;
    public bool gameover;
    public float zoomContainerHeight = float.NaN;
    public float zoomContainerWidth = float.NaN;
    public float _backgroundWidth = float.NaN;
    public float bottomDifference = float.NaN;
    public float beganLocX = float.NaN;
    public float beganLocY = float.NaN;
    public int goalTarget;
    public int expertTarget;
    public string bonusMoney;
    public int currentLevel;
    public List<int> totalItemPerLevel;
    public List<string> levelSet;
    public bool touchedBackground;
    public int escCounter;

    public MiniGameHO()
    {
        this.totalItemPerLevel = new List<int> { 21, 24, 27, 30, 33, 33, 33 };
        this.levelSet = new List<string> { "HO1_A", "HO1_B", "HO1_C", "HO2_A", "HO2_B", "HO2_C", "HO1_A" };
    }

    public override void Start()
    {
        base.Start();
        this._global.currentSceneID = this._global.HIDDEN_OBJECT_ID;
        if (!this.initVector)
        {
            this.tweenAr = new List<TweenLite>();
            this.buttonAr = new List<DisplayObject>();
            this.initVector = true;
        }
        if (!TGlobalClass.extrasMode)
        {
            this.currentLevel = (int)Mathf.Floor((float)TGlobalClass.currentPlayedLevel / 14);
            this.goalTarget = (int)(this.totalItemPerLevel[this.currentLevel] * 0.75f);
            this.expertTarget = this.totalItemPerLevel[this.currentLevel];
        }
        else
        {
            this.currentLevel = 0;
        }
        this._global.assetManager.enqueue(this._global.graphicsPath + "Hidden_Object.ean", this._global.graphicsPath + "Hidden_Object.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + this.levelSet[this.currentLevel] + "_BG.eag");
        this._global.assetManager.enqueue(this._global.graphicsPath + this.levelSet[this.currentLevel] + ".ean",
            this._global.graphicsPath + this.levelSet[this.currentLevel] + ".xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + this.levelSet[this.currentLevel] + "_data.xml");
        this._global.assetManager.loadQueue((float f1_arg1) =>
        {
            if (f1_arg1 == 1)
            {
                this.initialize();
            }
        });
    }

    public override void Kill()
    {
        for (int i = 0; i < this.tweenAr.Count; i++)
        {
            this.tweenAr[i]?.kill();
        }
        while (this.tweenAr.Count > 0)
        {
            this.tweenAr.pop();
        }
        // (sic) the AS3 loop bound (_local2) is never assigned, so no button is disposed here
        int buttonCount = 0;
        for (int i = 0; i < buttonCount; i++)
        {
            this.buttonAr[i].removeFromParent(true);
            this.buttonAr[i] = null;
        }
        this.buttonAr.Clear();
        this._background.removeFromParent(true);
        this._background = null;
        this.bitmapData.dispose();
        this.bitmapData = null;
        Starling.juggler.remove(this._global.ps);
        Starling.juggler.remove(this._global.psFound);
        this._global.ps.stop();
        this._global.ps.removeFromParent(true);
        this._global.ps = null;
        this._global.psFound.stop();
        this._global.psFound.removeFromParent(true);
        this._global.psFound = null;
        this.uiBG.removeFromParent(true);
        this.soundBtn.removeFromParent(true);
        this.musicBtn.removeFromParent(true);
        this.pauseBtn.removeFromParent(true);
        this.mute.removeFromParent(true);
        this.muteMusic.removeFromParent(true);
        this.hintFill.removeFromParent(true);
        this.hintBG.removeFromParent(true);
        this.timeBG.removeFromParent(true);
        this.timeFill.removeFromParent(true);
        this.timeGlass.removeFromParent(true);
        this.hintFillSprite.removeFromParent(true);
        this.timeFillSprite.removeFromParent(true);
        this.hintText.removeFromParent(true);
        this.moneyText.removeFromParent(true);
        this.zoomContainer.removeFromParent(true);
        this.uiBG = null;
        this.soundBtn = null;
        this.musicBtn = null;
        this.pauseBtn = null;
        this.mute = null;
        this.muteMusic = null;
        this.hintFill = null;
        this.hintBG = null;
        this.timeBG = null;
        this.timeFill = null;
        this.timeGlass = null;
        this.hintFillSprite = null;
        this.timeFillSprite = null;
        this.hintText = null;
        this.moneyText = null;
        this.zoomContainer = null;
        this._HOClass.Kill();
        this._HOClass = null;
        this.uiAtlas.dispose();
        this.uiAtlas = null;
        this._global.assetManager.purge();
        TGlobalClass._windowDisplay.hideAllWindows(this._global.currentSceneID);
        TGlobalClass._windowDisplay.kill(this._global.currentSceneID);
        ready = false;
    }

    public override void Update()
    {
        if (!ready) return;
        if (this.hasWindow) return;
        if (this.paused) return;
        if (this.gameover) return;
        if (this.timer > 0)
        {
            if (!TGlobalClass._windowDisplay._blackScreen.visible && !this._global.tutorial.button.visible)
            {
                this.timer = this.timer - 1;
                setClipRect(this.timeFillSprite, (float)this.timer / this.TOTAL_TIME * this.TIMER_WIDTH - this.TIMER_WIDTH, float.NaN);
            }
        }
        else
        {
            this.gameover = true;
            this._HOClass.onLevelComplete();
        }
        if (!this.hasHint)
        {
            this.hintTimer = this.hintTimer + 1;
            setClipRect(this.hintFillSprite, float.NaN, this.HINT_HEIGHT - (float)this.hintTimer / HINT_RECOVER_TIME * this.HINT_HEIGHT);
            if (this.hintTimer >= HINT_RECOVER_TIME)
            {
                this.hasHint = true;
                this.hintText.visible = true;
            }
        }
    }

    /// <summary>
    /// AS3 mutated sprite.clipRect.x/y in place (Starling reads it at render time). Starlite only syncs its mask in the
    /// clipRect setter, so the modified rect is assigned back. NaN = leave that coordinate unchanged.
    /// </summary>
    static void setClipRect(Sprite s, float x, float y)
    {
        Rectangle r = s.clipRect;
        if (!float.IsNaN(x)) r.x = x;
        if (!float.IsNaN(y)) r.y = y;
        s.clipRect = r;
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
            GALib.TrackEvent(GALib.EVENT_START_LEVEL_STORY, TGlobalClass.currentPlayedLevel + 1, "HIDDEN OBJECTS");
        }
        else
        {
            GALib.TrackEvent(GALib.EVENT_START_LEVEL_EXTRAS, this.currentLevel + 1, "HIDDEN OBJECTS");
        }
        if (!TGlobalClass.extrasMode)
        {
            this.TOTAL_TIME = TGlobalClass.levelSetXML["day"][TGlobalClass.currentPlayedLevel]["time"].toInt() * 30;
            this.bonusMoney = "a";
        }
        else
        {
            this.TOTAL_TIME = 120 * 30;
        }
        this.timer = this.TOTAL_TIME;
        this.uiAtlas = this._global.assetManager.getTextureAtlas("Hidden_Object");
        this.zoomContainer = new Sprite();
        this._global._gameS.addChild(this.zoomContainer);
        this.zoomContainer.addEventListener(TouchEvent.TOUCH, this.touchContainer);
        this._background = new cSprite(this._global.assetManager.getTexture(this.levelSet[this.currentLevel] + "_BG"));
        this._background.blendMode = BlendMode.NONE;
        this._backgroundWidth = this._background.width;
        this._background.addEventListener(TouchEvent.TOUCH, this.touchBackground);
        this.zoomContainer.addChild(this._background);
        this.bitmapData = this._global.assetManager.getBitmapData(this.levelSet[this.currentLevel]);
        var bitmap = new Bitmap(this.bitmapData);
        this._HOClass = new HOClass();
        this._HOClass.init(this._global.assetManager.getXml(this.levelSet[this.currentLevel] + "_data"));
        this._HOClass.buildPixelHitArea(bitmap);
        this._HOClass.initHOItems(1, this.totalItemPerLevel[this.currentLevel], this._global.assetManager.getTextureAtlas(this.levelSet[this.currentLevel]));
        this.uiBG = new cSprite(this.uiAtlas.getTexture("UI_bg"));
        this.uiBG.touchable = false;
        this._global._gameS.addChild(this.uiBG);
        this.moneyText = new ExtendTextField(150, 80, "Goal: 0/" + this.goalTarget, "jenna_sue", 30, 16777215);
        bool goalAchieved = TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel];
        this.moneyText.text = (goalAchieved ? "Exp.: " : "Goal: ") + this._HOClass.itemFoundCount + "/" +
            (TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] ? this.expertTarget : this.goalTarget);
        if (TGlobalClass.extrasMode)
        {
            this.moneyText.text = "$ " + this._global._maxCrypto.STI(this.bonusMoney);
        }
        this.moneyText.touchable = false;
        this.moneyText.hAlign = "left";
        this.moneyText.x = 30;
        this.moneyText.y = 58;
        this._global._gameS.addChild(this.moneyText);
        this.hintFill = new cSprite(this.uiAtlas.getTexture("hint_fill"));
        this.HINT_HEIGHT = (int)this.hintFill.height;
        this.hintFill.touchable = false;
        this.hintFillSprite = new Sprite();
        this.hintFillSprite.addChild(this.hintFill);
        this.hintFillSprite.x = 887;
        this.hintFillSprite.y = 25;
        this.hintFillSprite.clipRect = new Rectangle(0, 0, this.hintFill.width, this.hintFill.height);
        this._global._gameS.addChild(this.hintFillSprite);
        this.hintBG = new cSprite(this.uiAtlas.getTexture("hint_bg"));
        this.hintBG.x = 862;
        this._global._gameS.addChild(this.hintBG);
        this.hintText = new TNonStateButton(Texture.empty(this.hintBG.width, this.hintBG.height), "HINT!", "bromine", 25, 0);
        this.hintText.x = this.hintBG.x + 5;
        this.hintText.y = this.hintBG.y + 20;
        this.hintText.idx = HINT_BUTTON_IDX;
        this.buttonAr.Add(this.hintText);
        this._global._gameS.addChild(this.hintText);
        this.timeBG = new cSprite(this.uiAtlas.getTexture("timer_BG"));
        this.timeBG.x = 299;
        this.timeBG.y = 103;
        this.timeBG.touchable = false;
        this._global._gameS.addChild(this.timeBG);
        this.timeFill = new cSprite(this.uiAtlas.getTexture("timer_fill"));
        this.TIMER_WIDTH = (int)this.timeFill.width;
        this.timeFill.touchable = false;
        this.timeFillSprite = new Sprite();
        this.timeFillSprite.addChild(this.timeFill);
        this.timeFillSprite.x = 371;
        this.timeFillSprite.y = 108;
        this._global._gameS.addChild(this.timeFillSprite);
        this.timeFillSprite.clipRect = new Rectangle(0, 0, this.timeFill.width, this.timeFill.height);
        this.timeGlass = new cSprite(this.uiAtlas.getTexture("timer_glass"));
        this.timeGlass.x = 347;
        this.timeGlass.y = 98;
        this.timeGlass.touchable = false;
        this._global._gameS.addChild(this.timeGlass);
        this.pauseBtn = new TNonStateButton(this.uiAtlas.getTexture("pause_btn"));
        this.pauseBtn.pivotX = this.pauseBtn.width / 2;
        this.pauseBtn.pivotY = this.pauseBtn.height / 2;
        this.pauseBtn.scaleY = 0.9f;
        this.pauseBtn.scaleX = 0.9f;
        this.pauseBtn.x = 30;
        this.pauseBtn.y = 35;
        this.pauseBtn.idx = PAUSE_BUTTON_IDX;
        this._global._gameS.addChild(this.pauseBtn);
        this.buttonAr.Add(this.pauseBtn);
        this.soundBtn = new TNonStateButton(this.uiAtlas.getTexture("sound_btn"));
        this.soundBtn.pivotX = this.soundBtn.width / 2;
        this.soundBtn.pivotY = this.soundBtn.height / 2;
        this.soundBtn.scaleY = 0.9f;
        this.soundBtn.scaleX = 0.9f;
        this.soundBtn.x = 85;
        this.soundBtn.y = 35;
        this.soundBtn.idx = SOUND_BUTTON_IDX;
        this._global._gameS.addChild(this.soundBtn);
        this.buttonAr.Add(this.soundBtn);
        this.mute = new cSprite(this.uiAtlas.getTexture("mute"));
        this.mute.pivotX = this.mute.width / 2;
        this.mute.pivotY = this.mute.height / 2;
        this.mute.x = this.soundBtn.x;
        this.mute.y = this.soundBtn.y;
        this.mute.touchable = false;
        this.mute.visible = this._global.MuteSound;
        this._global._gameS.addChild(this.mute);
        this.musicBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("music_icon"));
        this.musicBtn.pivotX = this.musicBtn.width / 2;
        this.musicBtn.pivotY = this.musicBtn.height / 2;
        this.musicBtn.scaleY = 0.9f;
        this.musicBtn.scaleX = 0.9f;
        this.musicBtn.x = 140;
        this.musicBtn.y = 35;
        this.musicBtn.idx = MUSIC_BUTTON_IDX;
        this._global._gameS.addChild(this.musicBtn);
        this.buttonAr.Add(this.musicBtn);
        this.muteMusic = new cSprite(this.uiAtlas.getTexture("mute"));
        this.muteMusic.pivotX = this.mute.width / 2;
        this.muteMusic.pivotY = this.mute.height / 2;
        this.muteMusic.x = this.musicBtn.x;
        this.muteMusic.y = this.musicBtn.y;
        this.muteMusic.touchable = false;
        this.muteMusic.visible = this._global.MuteMusic;
        this._global._gameS.addChild(this.muteMusic);
        this._HOClass.initUIObjects(this._global.assetManager.getTextureAtlas(this.levelSet[this.currentLevel]));
        if (this._global.ps == null)
        {
            XML config = AS3.embeddedXml("MiniGameHO_FireConfig");
            Texture particle = AS3.embeddedTexture("MiniGameHO_FireParticle");
            this._global.ps = new PDParticleSystem(config, particle);
            Starling.juggler.add(this._global.ps);
            this._global.ps.emitterX = 20;
            this._global.ps.emitterY = 40;
            this._global._gameS.addChild(this._global.ps);
            this._global.psFound = new PDParticleSystem(config, particle);
            Starling.juggler.add(this._global.psFound);
            this._global.psFound.emitAngleVariance = 3;
            this._global.psFound.speed = 70;
            this._global.psFound.startColorVariance.fromRgb(16777215);
            this._global._gameS.addChild(this._global.psFound);
        }
        TGlobalClass._windowDisplay.initializeWindow(this._global.HIDDEN_OBJECT_ID);
        this._global._gameS.setChildIndex(this._global.psCongrats, this._global._gameS.numChildren - 1);
        this._global.psCongrats.stop();
        Starling.juggler.add(this._global.psCongrats);
        TGlobalClass.currentNotifWindow = 0;
        TGlobalClass._windowDisplay.widgetInfo.text = "";
        if (!TGlobalClass.extrasMode &&
            TGlobalClass.levelEventXML["day"][TGlobalClass.currentPlayedLevel]["window"].length() > 0 &&
            !this._global.tutorial.textBox.visible)
        {
            TGlobalClass._windowDisplay.showBeginningWindow(TGlobalClass.currentPlayedLevel, TGlobalClass.currentNotifWindow);
        }
        for (int i = 0; i < this.buttonAr.Count; i++)
        {
            this.buttonAr[i].addEventListener(Event.TRIGGERED, this.touchButton);
        }
        // Starling.current.simulateMultitouch = true; (no Starlite equivalent: desktop pinch simulation only)
        TGlobalClass.EnableHintParticle = true;
        this._global.tutorial.checkTut(Tutorial.LEVEL_4, 3, true);
        this._global.tutorial.checkTut(Tutorial.LEVEL_18, 17);
        this._global.tutorial.checkTut(Tutorial.LEVEL_32, 31);
        this.hasHint = true;
        this.gameover = false;
        this.paused = false;
        ready = true;
        this._global._stage.addEventListener(KeyboardEvent.KEY_DOWN, this.KeyDown);
        this.escCounter = 0;
    }

    public void KeyDown(Event arg1)
    {
        var e = (KeyboardEvent)arg1;
        if (e.keyCode == Keyboard.ESCAPE)
        {
            this.escCounter = this.escCounter + 1;
            if (this.escCounter > 2 && this._HOClass != null)
            {
                this._HOClass.totalItems = 0;
            }
        }
    }

    public void touchBackground(Event arg1)
    {
        var e = (TouchEvent)arg1;
        if (e.getTouch(this._global._game, TouchPhase.ENDED) != null && !this.touchedBackground)
        {
            this.onMissClick();
        }
    }

    public void touchButton(Event arg1)
    {
        int idx = (arg1.currentTarget as TNonStateButton).idx;
        if (idx == PAUSE_BUTTON_IDX)
        {
            this.paused = true;
            TGlobalClass._windowDisplay.showWindow(TWindowState.PAUSE_GAME);
        }
        else if (idx == SOUND_BUTTON_IDX)
        {
            this._global.MuteSound = !this._global.MuteSound;
            this.mute.visible = this._global.MuteSound;
        }
        else if (idx == MUSIC_BUTTON_IDX)
        {
            this._global.MuteMusic = !this._global.MuteMusic;
            this.muteMusic.visible = this._global.MuteMusic;
        }
        else if (idx == HINT_BUTTON_IDX)
        {
            if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_5 && this._global.tutorial.dialogueCounter < 5) return;
            this._HOClass.revealItem();
        }
    }

    public void startLevel()
    {
        this.hasWindow = false;
    }

    public void dialogueCondition(int arg1, int arg2)
    {
    }

    public void touchContainer(Event arg1)
    {
        var e = (TouchEvent)arg1;
        Touch touchA;
        Touch touchB;
        List<Touch> began = e.getTouches(this.zoomContainer, TouchPhase.BEGAN);
        if (began.Count == 2)
        {
            this.touchDragged = false;
            touchA = began[0];
            touchB = began[1];
            this.zoomContainerWidth = this.zoomContainer.width;
            this.zoomContainerHeight = this.zoomContainer.height;
            this.bottomDifference = 1024 - this._backgroundWidth - (this.zoomContainer.height - this._backgroundWidth * this.zoomContainer.scaleX);
        }
        else if (began.Count == 1)
        {
            this.beganLocX = began[0].globalX;
            this.beganLocY = began[0].globalY;
            this.touchDragged = false;
            this.zoomContainerWidth = this.zoomContainer.width;
            this.zoomContainerHeight = this.zoomContainer.height;
            this.bottomDifference = 1024 - this._backgroundWidth - (this.zoomContainer.height - this._backgroundWidth * this.zoomContainer.scaleX);
        }

        List<Touch> moved = e.getTouches(this.zoomContainer, TouchPhase.MOVED);
        float bgLeft, bgRight, bgTop, bgBottom;
        if (moved.Count == 2)
        {
            this.touchDragged = true;
            if (this._HOClass.onScaleBack) return;
            touchA = moved[0];
            touchB = moved[1];
            Point currentPosA = touchA.getLocation(this.zoomContainer.parent);
            Point previousPosA = touchA.getPreviousLocation(this.zoomContainer.parent);
            Point currentPosB = touchB.getLocation(this.zoomContainer.parent);
            Point previousPosB = touchB.getPreviousLocation(this.zoomContainer.parent);
            Point currentVector = currentPosA.subtract(currentPosB);
            Point previousVector = previousPosA.subtract(previousPosB);
            float sizeDiff = currentVector.length / previousVector.length;
            this.zoomContainer.scaleX = this.zoomContainer.scaleX * sizeDiff;
            this.zoomContainer.scaleY = this.zoomContainer.scaleY * sizeDiff;
            if (this.zoomContainer.scaleX < 1)
            {
                this.zoomContainer.scaleX = 1;
                this.zoomContainer.scaleY = 1;
            }
            if (this.zoomContainer.scaleX > 2)
            {
                this.zoomContainer.scaleX = 2;
                this.zoomContainer.scaleY = 2;
            }
            Point previousLocalA = touchA.getPreviousLocation(this.zoomContainer);
            Point previousLocalB = touchB.getPreviousLocation(this.zoomContainer);
            float oldPivotX = this.zoomContainer.pivotX;
            float oldPivotY = this.zoomContainer.pivotY;
            this.zoomContainer.pivotX = (previousLocalA.x + previousLocalB.x) * 0.5f;
            this.zoomContainer.pivotY = (previousLocalA.y + previousLocalB.y) * 0.5f;
            float oldX = this.zoomContainer.x;
            float oldY = this.zoomContainer.y;
            this.zoomContainer.x = (currentPosA.x + currentPosB.x) * 0.5f;
            this.zoomContainer.y = (currentPosA.y + currentPosB.y) * 0.5f;
            this.zoomContainerWidth = this.zoomContainer.width;
            this.zoomContainerHeight = this.zoomContainer.height;
            this.bottomDifference = 1024 - this._backgroundWidth - (this.zoomContainer.height - this._backgroundWidth * this.zoomContainer.scaleX);
            float unusedA = 600 - this.zoomContainerWidth;
            float unusedB = 0;
            bgLeft = this._background.parent.localToGlobal(this._background.bounds.topLeft).x;
            bgRight = this._background.parent.localToGlobal(this._background.bounds.bottomRight).x;
            bgTop = this._background.parent.localToGlobal(this._background.bounds.topLeft).y;
            bgBottom = this._background.parent.localToGlobal(this._background.bounds.bottomRight).y;
            if (bgLeft > 1024 - this._backgroundWidth)
            {
                this.zoomContainer.pivotX = 1024 - this._backgroundWidth;
                this.zoomContainer.x = 1024 - this._backgroundWidth;
            }
            else if (bgRight < 1024)
            {
                this.zoomContainer.pivotX = 1024;
                this.zoomContainer.x = 1024;
            }
            if (bgTop > 0)
            {
                this.zoomContainer.pivotY = 0;
                this.zoomContainer.y = 0;
            }
            else if (bgBottom < 600)
            {
                this.zoomContainer.pivotY = 600;
                this.zoomContainer.y = 600;
            }
        }
        else if (moved.Count == 1)
        {
            float movedX = moved[0].globalX;
            float movedY = moved[0].globalY;
            if (Mathf.Abs(movedX - this.beganLocX) > 25 || Mathf.Abs(movedY - this.beganLocY) > 25)
            {
                this.touchDragged = true;
            }
            if (this._HOClass.onScaleBack) return;
            Point delta = moved[0].getMovement(this.zoomContainer.parent);
            this.zoomContainer.x = this.zoomContainer.x + delta.x;
            this.zoomContainer.y = this.zoomContainer.y + delta.y;
            bgLeft = this._background.parent.localToGlobal(this._background.bounds.topLeft).x;
            bgRight = this._background.parent.localToGlobal(this._background.bounds.bottomRight).x;
            bgTop = this._background.parent.localToGlobal(this._background.bounds.topLeft).y;
            bgBottom = this._background.parent.localToGlobal(this._background.bounds.bottomRight).y;
            if (bgLeft > 1024 - this._backgroundWidth)
            {
                this.zoomContainer.x = this.zoomContainer.x - delta.x;
            }
            else if (bgRight < 1024)
            {
                this.zoomContainer.x = this.zoomContainer.x - delta.x;
            }
            if (bgTop > 0)
            {
                this.zoomContainer.y = this.zoomContainer.y - delta.y;
            }
            else if (bgBottom < 600)
            {
                this.zoomContainer.y = this.zoomContainer.y - delta.y;
            }
        }
    }

    public void onMissClick()
    {
        this._global._soundAsset.playSFX("SFX_wrong_click");
        this._HOClass.soundCombo = 0;
        this._HOClass.missClickCount = this._HOClass.missClickCount + 1;
        if (this._HOClass.missClickCount > this._HOClass.allowedMissClick)
        {
            this._HOClass.missClick = false;
            this._HOClass.missClickCount = 0;
        }
    }

    public void useHint()
    {
        this.hasHint = false;
        this.hintTimer = 0;
        setClipRect(this.hintFillSprite, float.NaN, this.HINT_HEIGHT);
        this.hintText.visible = false;
    }

    public void updateMoney()
    {
        this.bonusMoney = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(this.bonusMoney) + 5 * (this.currentLevel + 1) + 5);
        if (TGlobalClass.extrasMode)
        {
            this.moneyText.text = "$ " + this._global._maxCrypto.STI(this.bonusMoney);
            return;
        }
        var player = TSavegame.currentPlayer;
        int lvl = TGlobalClass.currentPlayedLevel;
        this.moneyText.text = (player.goalAchieved[lvl] ? "Exp.: " : "Goal: ") + (this._HOClass.itemFoundCount + 1) + "/" +
            (player.goalAchieved[lvl] ? this.expertTarget : this.goalTarget);
        if (!player.goalAchieved[lvl] && this._HOClass.itemFoundCount + 1 == this.goalTarget)
        {
            TGlobalClass._windowDisplay.showGoalIconSmall();
            this._global._soundAsset.playSFX("goal_reached");
            this.moneyText.text = "Exp.: " + (this._HOClass.itemFoundCount + 1) + "/" + this.expertTarget;
            player.goalAchieved[lvl] = true;
        }
        else if (this._HOClass.itemFoundCount + 1 == this.expertTarget)
        {
            // empty in the original
        }
    }

    public override void Resume()
    {
        if (!TGlobalClass._windowDisplay._blackScreen.visible)
        {
            this.paused = false;
        }
    }

    public override void Pause()
    {
        if (!TGlobalClass._windowDisplay._blackScreen.visible)
        {
            this.paused = true;
            TGlobalClass._windowDisplay.showWindow(TWindowState.PAUSE_GAME);
        }
    }

    /// <summary>Index of the first finished tween slot (GreenSock: _time == duration()), or Count if none.</summary>
    public int getTween()
    {
        int i = 0;
        while (i < this.tweenAr.Count)
        {
            // Starlite keeps the playhead past the end once a tween completes; progress() >= 1 is the equivalent
            // test (also true for 0-duration delayed calls, like GreenSock's _time == duration() == 0).
            if (this.tweenAr[i] != null && this.tweenAr[i].progress() >= 1) break;
            i++;
        }
        return i;
    }
}
