// Ported from TWindowDisplay.as (decompiled). All popup windows: exit/confirm dialogs, player
// selection / creation, pause menu, level summaries, shop info, free coins and the level event
// windows driven by LevelEvent.xml.
using System;
using System.Collections.Generic;
using Starlite;
using Mathf = UnityEngine.Mathf;

public class TWindowDisplay : TWindow
{
    public NativeTextField inputName;
    public TextFormat inputTextFormat;
    public bool Create;
    public List<TNonStateButton> buttonAr;
    public cSprite widgetBG;
    public ExtendTextField widgetMenu;
    public ExtendTextField widgetInfo;
    public TNonStateButton buttonLeft;
    public TNonStateButton buttonRight;
    public TNonStateButton uniButton;
    public cSprite smallWidgetBG;
    public ExtendTextField smallWidgetMenu;
    public ExtendTextField smallWidgetInfo;
    public TNonStateButton smallButtonLeft;
    public TNonStateButton smallButtonRight;
    public TNonStateButton smallUniButton;
    public List<ExtendTextField> playerNameText;
    public List<Quad> underLine;
    public ExtendTextField soundText;
    public ExtendTextField musicText;
    public ExtendTextField fullScreenText;
    public TNonStateButton soundButton;
    public TNonStateButton musicButton;
    public TNonStateButton fullScreenButton;
    public ExtendTextField summaryText;
    public ExtendTextField summaryStats;
    public ExtendTextField summaryTips;
    public cSprite achievementNotif;
    public ExtendTextField achievementNotifTitle;
    public ExtendTextField achievementNotifInfo;
    public TNonStateButton submitScoreBtn;
    public TNonStateButton leaderboardBtn;
    public cSprite unlockedItem;
    public cSprite goalIcon;
    public cSprite expertBlink;
    public int activeState;
    public int choosenPlayerIdx;
    public List<cSprite> dishPreviewAr;

    public TWindowDisplay() : base()
    {
        base.startLoad();
        this.buttonAr = new List<TNonStateButton>();
        this.dishPreviewAr = AS3Extensions.filled<cSprite>(4, i => null);
        this.playerNameText = AS3Extensions.filled<ExtendTextField>(5, i => null);
        this.underLine = AS3Extensions.filled<Quad>(5, i => null);
    }

    public void reload()
    {
        base.startLoad();
    }

    public void setText(ExtendTextField arg1)
    {
        string t = arg1.text;
        arg1.text = "";
        arg1.text = t;
    }

    public void setTexture()
    {
    }

    public void kill(int arg1)
    {
        int i;
        if (arg1 == _global.MAINMENU_ID)
        {
            for (i = 0; i < 5; i++)
            {
                this.playerNameText[i].removeEventListener(TouchEvent.TOUCH, this.choosePlayer);
                this.playerNameText[i].removeFromParent(true);
                this.playerNameText[i] = null;
                this.underLine[i].removeFromParent(true);
                this.underLine[i] = null;
            }
            this.inputName.removeEventListener(KeyboardEvent.KEY_UP, this.onKeyUp);
            this.inputName.removeEventListener(KeyboardEvent.KEY_DOWN, this.onKeyDown);
            _global._stage.removeChild(this.inputName);
            this.inputName = null;
        }
        else if (arg1 == _global.GAMESCREEN_ID || arg1 == _global.HIDDEN_OBJECT_ID || arg1 == _global.MAKE_DESSERT_ID)
        {
            this.soundButton.removeFromParent(true);
            this.soundButton = null;
            this.soundText.removeFromParent(true);
            this.soundText = null;
            this.musicButton.removeFromParent(true);
            this.musicButton = null;
            this.musicText.removeFromParent(true);
            this.musicText = null;
            this.fullScreenButton.removeFromParent(true);
            this.fullScreenButton = null;
            this.fullScreenText.removeFromParent(true);
            this.fullScreenText = null;
            this.summaryStats.removeFromParent(true);
            this.summaryStats = null;
            this.summaryTips.removeFromParent(true);
            this.summaryTips = null;
            this.summaryText.removeFromParent(true);
            this.summaryText = null;
            this.goalIcon.removeFromParent(true);
            this.goalIcon = null;
            TweenLite.killTweensOf(this.expertBlink);
            this.expertBlink.removeFromParent(true);
            this.expertBlink = null;
            this.leaderboardBtn.removeFromParent(true);
            this.leaderboardBtn = null;
            this.submitScoreBtn.removeFromParent(true);
            this.submitScoreBtn = null;
            // (sic) only the first SOUND_BUTTON entry is removed from buttonAr
            for (i = 0; i < this.buttonAr.Count; i++)
            {
                if (this.buttonAr[i].idx == TNonStateButton.SOUND_BUTTON)
                {
                    this.buttonAr.splice(i, 1);
                    break;
                }
            }
        }
        if (arg1 == _global.GAMESCREEN_ID || arg1 == _global.HIDDEN_OBJECT_ID || arg1 == _global.MAKE_DESSERT_ID || arg1 == _global.KITCHEN_ID)
        {
            this.unlockedItem.removeFromParent(true);
            this.unlockedItem = null;
        }
        if (arg1 == _global.GAMESCREEN_ID)
        {
            for (i = 0; i < 4; i++)
            {
                this.dishPreviewAr[i].removeFromParent(true);
                this.dishPreviewAr[i] = null;
            }
        }
        _global.tutorial.hideTutorial();
        _global.tutorial.currentDialogueID = -1;
    }

    public void onKeyDown(Event arg1)
    {
        var e = (KeyboardEvent)arg1;
        if (this.activeState != TWindowState.CREATE_PLAYER) return;
        if (this.inputName.length <= 0)
        {
            if (e.keyCode == Keyboard.SPACE) e.preventDefault();
        }
        else if (e.keyCode == Keyboard.ENTER)
        {
            this.smallButtonLeft.visible = false;
            this.smallButtonRight.visible = false;
            this.smallWidgetBG.visible = false;
            this.smallWidgetInfo.visible = false;
            this.smallWidgetMenu.visible = false;
            this.inputName.visible = false;
            int i;
            for (i = 0; i < 5; i++)
            {
                if (this.inputName.text == TSavegame.playerName[i])
                {
                    this.showWindow(TWindowState.SAME_NAME_WARNING);
                    return;
                }
            }
            int start = -1;
            int end = 0;
            string name = this.inputName.text;
            for (i = 0; i < this.inputName.text.Length; i++)
            {
                if (name.charAt(i) == " ")
                {
                    end = i;
                }
                else
                {
                    if (start == -1) start = i;
                    end = this.inputName.text.Length;
                }
            }
            this.inputName.text = name.substring(start, end);
            for (i = 0; i < 5; i++)
            {
                if (!TSavegame.usedSlot[i])
                {
                    TSavegame.usedSlot[i] = true;
                    TSavegame.playerName[i] = this.inputName.text;
                    TSavegame.lastPlayer = i + 1;
                    TSavegame._saveGame.data["lastPlayer"] = i + 1;
                    TSavegame.loadPlayer(i);
                    TSavegame.save();
                    break;
                }
            }
            this.choosenPlayerIdx = i;
            this.playerNameText[i].color = 16711680;
            this.buttonLeft.enabled = false;
            this.showWindow(TWindowState.CHANGE_PLAYER);
        }
    }

    public void onKeyUp(Event arg1)
    {
        if (this.activeState != TWindowState.CREATE_PLAYER) return;
        if (this.inputName.text.Length > 0)
        {
            for (int i = 0; i < this.inputName.length; i++)
            {
                if (this.inputName.text.charAt(i) != " ")
                {
                    this.smallButtonRight.enabled = true;
                    return;
                }
            }
            this.smallButtonRight.enabled = false;
        }
        else
        {
            this.smallButtonRight.enabled = false;
        }
    }

    public void initializeWindow(int arg1)
    {
        int i;
        if (_global.tutorial == null)
        {
            _global.tutorial = new Tutorial();
            _global.tutorial.init();
        }
        else
        {
            _global.tutorial.bringTutorialToTop();
        }
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.numChildren - 1);
        if (!this.Create)
        {
            this.widgetBG = new cSprite(windowsAtlas.getTexture("widget_BG"));
            this.widgetBG.pivotX = this.widgetBG.width / 2;
            this.widgetBG.pivotY = this.widgetBG.height / 2;
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetBG.visible = false;
            _global._gameS.addChild(this.widgetBG);
            this.widgetMenu = new ExtendTextField((int)(this.widgetBG.width - 100), 100, "SUMMARY", "jenna_sue", 60, 0, true);
            this.widgetMenu.pivotX = this.widgetMenu.width / 2;
            this.widgetMenu.pivotY = 50;
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.widgetMenu.visible = false;
            _global._gameS.addChild(this.widgetMenu);
            this.widgetInfo = new ExtendTextField((int)(this.widgetBG.width - 100), 300, "", "bromine", 25, 0);
            this.widgetInfo.pivotX = this.widgetInfo.width / 2;
            this.widgetInfo.pivotY = 150;
            this.widgetInfo.x = 512;
            this.widgetInfo.y = this.widgetBG.y - 30;
            this.widgetInfo.visible = false;
            _global._gameS.addChild(this.widgetInfo);
            this.buttonLeft = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "Test", "bromine", 20, 0);
            this.buttonLeft.pivotX = this.buttonLeft.width / 2;
            this.buttonLeft.pivotY = this.buttonLeft.height / 2;
            this.buttonLeft.scaleY = 0.8f;
            this.buttonLeft.scaleX = 0.8f;
            this.buttonLeft.textureName = "small_btn_blue";
            this.buttonLeft.x = this.widgetBG.x - 170;
            this.buttonLeft.y = this.widgetBG.y + 200;
            this.buttonLeft.visible = false;
            _global._gameS.addChild(this.buttonLeft);
            this.buttonAr.push(this.buttonLeft);
            this.buttonRight = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "Test", "bromine", 20, 0);
            this.buttonRight.pivotX = this.buttonRight.width / 2;
            this.buttonRight.pivotY = this.buttonRight.height / 2;
            this.buttonRight.scaleY = 0.8f;
            this.buttonRight.scaleX = 0.8f;
            this.buttonRight.textureName = "small_btn_blue";
            this.buttonRight.x = this.widgetBG.x + 170;
            this.buttonRight.y = this.widgetBG.y + 200;
            this.buttonRight.visible = false;
            _global._gameS.addChild(this.buttonRight);
            this.buttonAr.push(this.buttonRight);
            this.uniButton = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "New Game", "bromine", 20, 0);
            this.uniButton.pivotX = this.uniButton.width / 2;
            this.uniButton.pivotY = this.uniButton.height / 2;
            this.uniButton.scaleY = 0.8f;
            this.uniButton.scaleX = 0.8f;
            this.uniButton.textureName = "small_btn_blue";
            this.uniButton.x = this.widgetBG.x;
            this.uniButton.y = this.widgetBG.y + 200;
            this.uniButton.visible = false;
            _global._gameS.addChild(this.uniButton);
            this.buttonAr.push(this.uniButton);
        }
        else
        {
            _global._gameS.setChildIndex(this.widgetBG, _global._gameS.numChildren - 1);
            _global._gameS.setChildIndex(this.widgetMenu, _global._gameS.numChildren - 1);
            _global._gameS.setChildIndex(this.widgetInfo, _global._gameS.numChildren - 1);
            _global._gameS.setChildIndex(this.buttonLeft, _global._gameS.numChildren - 1);
            _global._gameS.setChildIndex(this.buttonRight, _global._gameS.numChildren - 1);
            _global._gameS.setChildIndex(this.uniButton, _global._gameS.numChildren - 1);
        }
        if (arg1 == _global.MAINMENU_ID)
        {
            for (i = 0; i < 5; i++)
            {
                this.playerNameText[i] = new ExtendTextField(380, 70, "Empty", "jenna_sue", 30, 0);
                this.playerNameText[i].pivotX = this.playerNameText[i].width / 2;
                this.playerNameText[i].pivotY = this.playerNameText[i].height / 2;
                this.playerNameText[i].x = 512;
                this.playerNameText[i].y = i * 70 + 145;
                this.playerNameText[i].visible = false;
                this.playerNameText[i].idx = i;
                _global._gameS.addChild(this.playerNameText[i]);
                this.playerNameText[i].addEventListener(TouchEvent.TOUCH, this.choosePlayer);
                this.underLine[i] = new Quad(380, 5, 0);
                this.underLine[i].pivotX = this.underLine[i].width / 2;
                this.underLine[i].pivotY = this.underLine[i].height / 2;
                this.underLine[i].x = 512;
                this.underLine[i].y = this.playerNameText[i].y + 15;
                this.underLine[i].visible = false;
                this.underLine[i].touchable = false;
                _global._gameS.addChild(this.underLine[i]);
            }
        }
        if (arg1 == _global.GAMESCREEN_ID || arg1 == _global.HIDDEN_OBJECT_ID || arg1 == _global.MAKE_DESSERT_ID)
        {
            // NOTE: smallWidgetBG is created further below; on the very first call it is still null here
            // (same as the AS3 original, which relies on the main menu having created it first).
            this.soundText = new ExtendTextField(150, 50, "SOUND : ", "bromine", 25, 0);
            this.soundText.touchable = false;
            this.soundText.x = this.smallWidgetBG.x - 100;
            this.soundText.y = this.smallWidgetBG.y - 30;
            this.soundText.visible = false;
            _global._gameS.addChild(this.soundText);
            this.soundButton = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "ON", "bromine", 20);
            this.soundButton.x = this.smallWidgetBG.x + 100;
            this.soundButton.y = this.soundText.y;
            this.soundButton.visible = false;
            this.soundButton.idx = TNonStateButton.SOUND_BUTTON;
            _global._gameS.addChild(this.soundButton);
            this.buttonAr.push(this.soundButton);
            this.musicText = new ExtendTextField(150, 50, "MUSIC : ", "bromine", 25, 0);
            this.musicText.touchable = false;
            this.musicText.x = this.smallWidgetBG.x - 100;
            this.musicText.y = this.smallWidgetBG.y + 40;
            this.musicText.visible = false;
            _global._gameS.addChild(this.musicText);
            this.musicButton = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "ON", "bromine", 20);
            this.musicButton.x = this.smallWidgetBG.x + 100;
            this.musicButton.y = this.musicText.y;
            this.musicButton.visible = false;
            this.musicButton.idx = TNonStateButton.MUSIC_BUTTON;
            _global._gameS.addChild(this.musicButton);
            this.buttonAr.push(this.musicButton);
            this.fullScreenText = new ExtendTextField(250, 50, "FULL SCREEN : ", "bromine", 25, 0);
            this.fullScreenText.touchable = false;
            this.fullScreenText.x = this.smallWidgetBG.x - 100;
            this.fullScreenText.y = this.smallWidgetBG.y + 80;
            this.fullScreenText.visible = false;
            _global._gameS.addChild(this.fullScreenText);
            this.fullScreenButton = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "ON", "bromine", 20);
            this.fullScreenButton.x = this.smallWidgetBG.x + 100;
            this.fullScreenButton.y = this.fullScreenText.y;
            this.fullScreenButton.visible = false;
            this.fullScreenButton.idx = TNonStateButton.FULLSCREEN_BUTTON;
            _global._gameS.addChild(this.fullScreenButton);
            this.buttonAr.push(this.fullScreenButton);
            this.summaryText = new ExtendTextField((int)(this.widgetBG.width / 2), 350, "Customers Served\nBeverage Served\nFrench Fries Served\n\nToday’s Earnings\nTotal Earning", "bromine", 20);
            this.summaryText.pivotX = this.summaryText.width / 2;
            this.summaryText.pivotY = this.summaryText.height / 2;
            this.summaryText.hAlign = "left";
            this.summaryText.x = this.widgetBG.x - 100;
            this.summaryText.y = this.widgetBG.y;
            this.summaryText.touchable = false;
            this.summaryText.visible = false;
            _global._gameS.addChild(this.summaryText);
            this.summaryStats = new ExtendTextField((int)(this.widgetBG.width / 2), 350, "", "bromine", 20);
            this.summaryStats.pivotX = this.summaryStats.width / 2;
            this.summaryStats.pivotY = this.summaryStats.height / 2;
            this.summaryStats.hAlign = "right";
            this.summaryStats.x = this.widgetBG.x + 80;
            this.summaryStats.y = this.widgetBG.y;
            this.summaryStats.touchable = false;
            this.summaryText.visible = false; // (sic) original hides summaryText again, not summaryStats
            _global._gameS.addChild(this.summaryStats);
            this.summaryTips = new ExtendTextField((int)(this.widgetBG.width / 2 + 40), 100, "", "bromine", 20);
            this.summaryTips.pivotX = this.summaryTips.width / 2;
            this.summaryTips.pivotY = this.summaryTips.height / 2;
            this.summaryTips.hAlign = "center";
            this.summaryTips.x = this.widgetBG.x;
            this.summaryTips.y = this.widgetBG.y + 125;
            this.summaryTips.touchable = false;
            this.summaryTips.visible = false;
            _global._gameS.addChild(this.summaryTips);
            this.goalIcon = new cSprite(windowsAtlas.getTexture("goal"));
            this.goalIcon.pivotX = this.goalIcon.width / 2;
            this.goalIcon.pivotY = this.goalIcon.height / 2;
            this.goalIcon.x = 780;
            this.goalIcon.y = 500;
            this.goalIcon.visible = false;
            _global._gameS.addChild(this.goalIcon);
            this.expertBlink = new cSprite(windowsAtlas.getTexture("expert_blink"));
            this.expertBlink.pivotX = this.expertBlink.width / 2;
            this.expertBlink.pivotY = this.expertBlink.height / 2;
            this.expertBlink.x = this.goalIcon.x;
            this.expertBlink.y = this.goalIcon.y + 10;
            this.expertBlink.visible = false;
            _global._gameS.addChild(this.expertBlink);
            this.leaderboardBtn = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "Leaderboard", "bromine", 20, 0);
            this.leaderboardBtn.pivotX = this.leaderboardBtn.width / 2;
            this.leaderboardBtn.pivotY = this.leaderboardBtn.height / 2;
            this.leaderboardBtn.scaleY = 0.8f;
            this.leaderboardBtn.scaleX = 0.8f;
            this.leaderboardBtn.x = 640;
            this.leaderboardBtn.y = 550;
            this.leaderboardBtn.visible = false;
            this.leaderboardBtn.idx = TNonStateButton.LEADERBOARD_BUTTON;
            _global._gameS.addChild(this.leaderboardBtn);
            this.buttonAr.push(this.leaderboardBtn);
            this.submitScoreBtn = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "Submit Score", "bromine", 20, 0);
            this.submitScoreBtn.pivotX = this.submitScoreBtn.width / 2;
            this.submitScoreBtn.pivotY = this.submitScoreBtn.height / 2;
            this.submitScoreBtn.scaleY = 0.8f;
            this.submitScoreBtn.scaleX = 0.8f;
            this.submitScoreBtn.x = 375;
            this.submitScoreBtn.y = 550;
            this.submitScoreBtn.visible = false;
            this.submitScoreBtn.idx = TNonStateButton.SUBMIT_BUTTON;
            _global._gameS.addChild(this.submitScoreBtn);
            this.buttonAr.push(this.submitScoreBtn);
        }
        if (arg1 == _global.GAMESCREEN_ID || arg1 == _global.HIDDEN_OBJECT_ID || arg1 == _global.MAKE_DESSERT_ID || arg1 == _global.KITCHEN_ID)
        {
            this.unlockedItem = new cSprite(windowsAtlas.getTexture("small_btn_blue"));
            this.unlockedItem.pivotX = this.unlockedItem.width / 2;
            this.unlockedItem.pivotY = this.unlockedItem.height / 2;
            this.unlockedItem.x = this.widgetBG.x;
            this.unlockedItem.y = this.widgetBG.y - 30;
            this.unlockedItem.touchable = false;
            this.unlockedItem.visible = false;
            _global._gameS.addChild(this.unlockedItem);
        }
        if (arg1 == _global.GAMESCREEN_ID)
        {
            for (i = 0; i < 4; i++)
            {
                this.dishPreviewAr[i] = new cSprite(_global.gameScreen.gameAtlas.getTexture("veggie_croissant"));
                this.dishPreviewAr[i].x = 512 - (i - 2) * (this.dishPreviewAr[i].width - 10);
                this.dishPreviewAr[i].y = 300;
                this.dishPreviewAr[i].touchable = false;
                this.dishPreviewAr[i].visible = false;
                _global._gameS.addChild(this.dishPreviewAr[i]);
            }
        }
        if (!this.Create)
        {
            this.smallWidgetBG = new cSprite(windowsAtlas.getTexture("small_widget_BG"));
            this.smallWidgetBG.pivotX = this.smallWidgetBG.width / 2;
            this.smallWidgetBG.pivotY = this.smallWidgetBG.height / 2;
            this.smallWidgetBG.x = 512;
            this.smallWidgetBG.y = 300;
            this.smallWidgetBG.visible = false;
            _global._gameS.addChild(this.smallWidgetBG);
            this.smallWidgetMenu = new ExtendTextField((int)(this.smallWidgetBG.width - 80), 70, "Exit?", "jenna_sue", 60, 0);
            this.smallWidgetMenu.pivotX = this.smallWidgetMenu.width / 2;
            this.smallWidgetMenu.pivotY = this.smallWidgetMenu.height / 2;
            this.smallWidgetMenu.x = 525;
            this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
            this.smallWidgetMenu.visible = false;
            _global._gameS.addChild(this.smallWidgetMenu);
            this.smallWidgetInfo = new ExtendTextField((int)(this.smallWidgetBG.width - 80), 250, "Are You Sure Want To Exit?", "jenna_sue", 40, 0);
            this.smallWidgetInfo.pivotX = this.smallWidgetInfo.width / 2;
            this.smallWidgetInfo.pivotY = this.smallWidgetInfo.height / 2;
            this.smallWidgetInfo.x = 525;
            this.smallWidgetInfo.y = this.smallWidgetBG.y - 30;
            this.smallWidgetInfo.visible = false;
            _global._gameS.addChild(this.smallWidgetInfo);
            this.smallButtonLeft = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "Test", "bromine", 20, 0);
            this.smallButtonLeft.textureName = "small_btn_blue";
            this.smallButtonLeft.pivotX = this.smallButtonLeft.width / 2;
            this.smallButtonLeft.pivotY = this.smallButtonLeft.height / 2;
            this.smallButtonLeft.x = this.smallWidgetBG.x - 100;
            this.smallButtonLeft.y = this.smallWidgetBG.y + 130;
            this.smallButtonLeft.scaleX = 0.9f;
            this.smallButtonLeft.scaleY = 0.9f;
            this.smallButtonLeft.visible = false;
            _global._gameS.addChild(this.smallButtonLeft);
            this.buttonAr.push(this.smallButtonLeft);
            this.smallButtonRight = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "Test", "bromine", 20, 0);
            this.smallButtonRight.textureName = "small_btn_blue";
            this.smallButtonRight.pivotX = this.smallButtonRight.width / 2;
            this.smallButtonRight.pivotY = this.smallButtonRight.height / 2;
            this.smallButtonRight.x = this.smallWidgetBG.x + 100;
            this.smallButtonRight.y = this.smallWidgetBG.y + 130;
            this.smallButtonRight.visible = false;
            this.smallButtonRight.scaleX = 0.9f;
            this.smallButtonRight.scaleY = 0.9f;
            _global._gameS.addChild(this.smallButtonRight);
            this.buttonAr.push(this.smallButtonRight);
            this.smallUniButton = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "Test", "bromine", 20, 0);
            this.smallUniButton.textureName = "small_btn_blue";
            this.smallUniButton.pivotX = this.smallUniButton.width / 2;
            this.smallUniButton.pivotY = this.smallUniButton.height / 2;
            this.smallUniButton._image.visible = false;
            this.smallUniButton.x = this.smallWidgetBG.x;
            this.smallUniButton.y = this.smallWidgetBG.y + 130;
            this.smallUniButton.visible = false;
            _global._gameS.addChild(this.smallUniButton);
            this.buttonAr.push(this.smallUniButton);
        }
        else
        {
            _global._gameS.setChildIndex(this.smallWidgetBG, _global._gameS.numChildren - 1);
            _global._gameS.setChildIndex(this.smallWidgetMenu, _global._gameS.numChildren - 1);
            _global._gameS.setChildIndex(this.smallWidgetInfo, _global._gameS.numChildren - 1);
            _global._gameS.setChildIndex(this.smallButtonLeft, _global._gameS.numChildren - 1);
            _global._gameS.setChildIndex(this.smallButtonRight, _global._gameS.numChildren - 1);
            _global._gameS.setChildIndex(this.smallUniButton, _global._gameS.numChildren - 1);
        }
        if (arg1 == _global.MAINMENU_ID)
        {
            // flash.text.TextField overlay -> Starlite NativeTextField on the (always 1024x600) stage;
            // the original stageWidth/1024 and stageHeight/600 scale factors are dropped.
            this.inputTextFormat = new TextFormat("jenna_sue", 30, 0);
            this.inputTextFormat.align = "center";
            this.inputName = new NativeTextField();
            this.inputName.width = this.smallWidgetBG.width - 100;
            this.inputName.height = 40;
            this.inputName.maxChars = 12;
            this.inputName.defaultTextFormat = this.inputTextFormat;
            this.inputName.type = TextFieldType.INPUT;
            this.inputName.needsSoftKeyboard = true;
            this.inputName.border = true;
            this.inputName.embedFonts = true;
            this.inputName.restrict = "a-zA-Z ";
            this.inputName.visible = false;
            this.inputName.x = _global._stage.stageWidth / 2f - this.inputName.width / 2;
            this.inputName.y = this.smallWidgetBG.y;
            _global._stage.addChild(this.inputName);
            this.inputName.addEventListener(KeyboardEvent.KEY_UP, this.onKeyUp);
            this.inputName.addEventListener(KeyboardEvent.KEY_DOWN, this.onKeyDown);
        }
        if (!this.Create)
        {
            this.achievementNotif = new cSprite(windowsAtlas.getTexture("widget_BG"));
            this.achievementNotif.pivotX = this.achievementNotif.width / 2;
            this.achievementNotif.pivotY = this.achievementNotif.height / 2;
            this.achievementNotif.x = 1500;
            this.achievementNotif.y = 200;
            this.achievementNotif.scaleX = 0.5f;
            this.achievementNotif.scaleY = 0.2f;
            this.achievementNotif.touchable = false;
            _global._gameS.addChild(this.achievementNotif);
            this.achievementNotifTitle = new ExtendTextField((int)(this.achievementNotif.width - 30), (int)(this.achievementNotif.height / 2 - 20), "MASTER", "bromine", 20, (int)Color.RED);
            this.achievementNotifTitle.pivotX = this.achievementNotifTitle.width / 2;
            this.achievementNotifTitle.pivotY = this.achievementNotifTitle.height / 2;
            this.achievementNotifTitle.x = this.achievementNotif.x;
            this.achievementNotifTitle.y = this.achievementNotif.y - 30;
            this.achievementNotifTitle.touchable = false;
            _global._gameS.addChild(this.achievementNotifTitle);
            this.achievementNotifInfo = new ExtendTextField((int)(this.achievementNotif.width - 30), (int)(this.achievementNotif.height - 10), "You've served 100 fully satisfied customers!", "bromine", 15, (int)Color.BLACK);
            this.achievementNotifInfo.pivotX = this.achievementNotifInfo.width / 2;
            this.achievementNotifInfo.pivotY = this.achievementNotifInfo.height / 2;
            this.achievementNotifInfo.x = this.achievementNotif.x;
            this.achievementNotifInfo.y = this.achievementNotif.y + 10;
            this.achievementNotifInfo.touchable = false;
            _global._gameS.addChild(this.achievementNotifInfo);
        }
        else
        {
            _global._gameS.setChildIndex(this.achievementNotif, _global._gameS.numChildren - 1);
            _global._gameS.setChildIndex(this.achievementNotifTitle, _global._gameS.numChildren - 1);
            _global._gameS.setChildIndex(this.achievementNotifInfo, _global._gameS.numChildren - 1);
        }
        for (i = 0; i < this.buttonAr.Count; i++)
        {
            this.buttonAr[i].addEventListener(TouchEvent.TOUCH, this.touchButton);
        }
        this.Create = true;
    }

    public void choosePlayer(Event arg1)
    {
        var e = (TouchEvent)arg1;
        if (e.getTouch(_global._game, TouchPhase.BEGAN) == null) return;
        this.choosenPlayerIdx = (e.currentTarget as ExtendTextField).idx;
        for (int i = 0; i < 5; i++)
        {
            this.playerNameText[i].color = 0;
        }
        if (TSavegame.usedSlot[this.choosenPlayerIdx])
        {
            this.playerNameText[this.choosenPlayerIdx].color = 16711680;
            this.buttonLeft.enabled = true;
            this.uniButton.idx = TNonStateButton.SELECT_BUTTON;
            this.uniButton._text.text = "Select";
        }
        else
        {
            this.uniButton._text.text = "Close";
            this.uniButton.idx = TNonStateButton.CANCEL_BUTTON;
            this.buttonLeft.enabled = false;
        }
    }

    public void touchButton(Event arg1)
    {
        var e = (TouchEvent)arg1;
        int i = 0;
        if (e.getTouch(_global._game, TouchPhase.BEGAN) == null) return;
        int btn = (e.currentTarget as TNonStateButton).idx;
        if (btn == TNonStateButton.YES_BUTTON)
        {
            if (this.activeState == TWindowState.EXIT_WINDOW)
            {
                NativeApplication.nativeApplication.exit();
            }
            else if (this.activeState == TWindowState.CONFIRM_MAINMENU)
            {
                if (_global.currentSceneID == _global.GAMESCREEN_ID && !TGlobalClass.extrasMode
                    && TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] == 0
                    && TGlobalClass.currentPlayedLevel == TSavegame.currentPlayer.passedDays)
                {
                    TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = false;
                }
                _global._game.fadeToScreen(_global._mainMenuScreen);
            }
            else if (this.activeState == TWindowState.CONFIRM_KITCHEN)
            {
                if (_global.currentSceneID == _global.GAMESCREEN_ID && !TGlobalClass.extrasMode
                    && TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] == 0
                    && TGlobalClass.currentPlayedLevel == TSavegame.currentPlayer.passedDays)
                {
                    TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = false;
                }
                TGlobalClass.coinsGot = 0;
                _global._game.fadeToScreen(_global._kitchen);
            }
        }
        else if (btn == TNonStateButton.NO_BUTTON)
        {
            if (this.activeState == TWindowState.EXIT_WINDOW)
            {
                _blackScreen.visible = false;
                this.smallWidgetBG.visible = false;
                this.smallWidgetMenu.visible = false;
                this.smallWidgetInfo.visible = false;
                this.smallUniButton.visible = false;
                this.smallButtonLeft.visible = false;
                this.smallButtonRight.visible = false;
            }
            if (this.activeState == TWindowState.CONFIRM_MAINMENU || this.activeState == TWindowState.CONFIRM_KITCHEN)
            {
                this.smallWidgetBG.visible = false;
                this.smallWidgetMenu.visible = false;
                this.smallWidgetInfo.visible = false;
                this.smallUniButton.visible = false;
                this.smallButtonLeft.visible = false;
                this.smallButtonRight.visible = false;
                this.showWindow(TWindowState.PAUSE_GAME);
            }
        }
        else if (btn == TNonStateButton.DELETE_BUTTON)
        {
            if (this.activeState == TWindowState.CHANGE_PLAYER)
            {
                this.showWindow(TWindowState.DELETE_PLAYER);
            }
            else if (this.activeState == TWindowState.DELETE_PLAYER)
            {
                _blackScreen.visible = false;
                this.smallWidgetBG.visible = false;
                this.smallButtonLeft.visible = false;
                this.smallButtonRight.visible = false;
                this.smallWidgetMenu.visible = false;
                this.smallWidgetInfo.visible = false;
                this.buttonLeft.enabled = false;
                // (sic) the original indexes with an unused local that is always 0 here
                this.playerNameText[i].color = 0;
                this.playerNameText[i].text = "Empty";
                if (this.choosenPlayerIdx == TSavegame.lastPlayer - 1)
                {
                    _global._mainMenuScreen.playerNameText.text = "Guest";
                }
                TSavegame.clear(this.choosenPlayerIdx);
                this.showWindow(TWindowState.CHANGE_PLAYER);
            }
        }
        else if (btn == TNonStateButton.CREATE_BUTOTN)
        {
            this.showWindow(TWindowState.CREATE_PLAYER);
        }
        else if (btn == TNonStateButton.CANCEL_BUTTON)
        {
            if (this.activeState == TWindowState.CHANGE_PLAYER)
            {
                this.widgetBG.visible = false;
                _blackScreen.visible = false;
                this.widgetMenu.visible = false;
                this.widgetInfo.visible = false;
                this.buttonLeft.visible = false;
                this.buttonRight.visible = false;
                this.uniButton.visible = false;
                for (i = 0; i < 5; i++)
                {
                    this.playerNameText[i].visible = false;
                    this.underLine[i].visible = false;
                }
            }
            else if (this.activeState == TWindowState.CREATE_PLAYER)
            {
                this.smallButtonLeft.visible = false;
                this.smallButtonRight.visible = false;
                this.smallWidgetBG.visible = false;
                this.smallWidgetInfo.visible = false;
                this.smallWidgetMenu.visible = false;
                this.inputName.visible = false;
                this.smallButtonRight.enabled = true;
                this.showWindow(TWindowState.CHANGE_PLAYER);
            }
            else if (this.activeState == TWindowState.DELETE_PLAYER)
            {
                this.smallButtonLeft.visible = false;
                this.smallButtonRight.visible = false;
                this.smallWidgetBG.visible = false;
                this.smallWidgetInfo.visible = false;
                this.smallWidgetMenu.visible = false;
                this.showWindow(TWindowState.CHANGE_PLAYER);
            }
            else if (this.activeState == TWindowState.EXTRAS_MENU)
            {
                _blackScreen.visible = false;
                this.widgetBG.visible = false;
                this.widgetMenu.visible = false;
                this.buttonLeft.visible = false;
                this.buttonRight.visible = false;
                this.uniButton.visible = false;
                this.buttonLeft.scaleX = 0.8f;
                this.buttonLeft.scaleY = 0.8f;
                this.buttonRight.scaleX = 0.8f;
                this.buttonRight.scaleY = 0.8f;
                this.uniButton.scaleX = 0.8f;
                this.uniButton.scaleY = 0.8f;
            }
            else if (this.activeState == TWindowState.UNLOCK_FULL_VERSION)
            {
                _blackScreen.visible = false;
                this.smallButtonLeft.visible = false;
                this.smallButtonRight.visible = false;
                this.smallWidgetBG.visible = false;
                this.smallWidgetInfo.visible = false;
                this.smallWidgetMenu.visible = false;
            }
            else if (this.activeState == TWindowState.SHOW_FREE_COINS)
            {
                _blackScreen.visible = false;
                this.widgetBG.visible = false;
                this.widgetMenu.visible = false;
                this.widgetInfo.visible = false;
                this.buttonLeft.visible = false;
                this.buttonRight.visible = false;
            }
        }
        else if (btn == TNonStateButton.DONE_CREATE_PLAYER)
        {
            this.smallButtonLeft.visible = false;
            this.smallButtonRight.visible = false;
            this.smallWidgetBG.visible = false;
            this.smallWidgetInfo.visible = false;
            this.smallWidgetMenu.visible = false;
            this.inputName.visible = false;
            for (i = 0; i < 5; i++)
            {
                if (this.inputName.text == TSavegame.playerName[i])
                {
                    this.showWindow(TWindowState.SAME_NAME_WARNING);
                    return;
                }
            }
            int start = -1;
            int end = 0;
            string name = this.inputName.text;
            for (i = 0; i < this.inputName.text.Length; i++)
            {
                if (name.charAt(i) == " ")
                {
                    end = i;
                }
                else
                {
                    if (start == -1) start = i;
                    end = this.inputName.text.Length;
                }
            }
            this.inputName.text = name.substring(start, end);
            for (i = 0; i < 5; i++)
            {
                if (!TSavegame.usedSlot[i])
                {
                    TSavegame.usedSlot[i] = true;
                    TSavegame.playerName[i] = this.inputName.text;
                    TSavegame.lastPlayer = i + 1;
                    TSavegame._saveGame.data["lastPlayer"] = i + 1;
                    TSavegame.loadPlayer(i);
                    TSavegame.save();
                    break;
                }
            }
            this.choosenPlayerIdx = i;
            this.playerNameText[i].color = 16711680;
            this.buttonLeft.enabled = false;
            this.showWindow(TWindowState.CHANGE_PLAYER);
        }
        else if (btn == TNonStateButton.OK_BUTTON)
        {
            if (this.activeState == TWindowState.SAME_NAME_WARNING)
            {
                this.smallWidgetBG.visible = false;
                this.smallUniButton.visible = false;
                this.smallWidgetInfo.visible = false;
                this.smallWidgetMenu.visible = false;
                this.showWindow(TWindowState.CREATE_PLAYER);
            }
            else if (this.activeState == TWindowState.SHOW_FREE_COINS)
            {
                if (TGlobalClass.VideoReady == true)
                {
                    this.widgetBG.visible = false;
                    _blackScreen.visible = false;
                    this.widgetMenu.visible = false;
                    this.widgetInfo.visible = false;
                    this.buttonLeft.visible = false;
                    this.buttonRight.visible = false;
                    TSavegame.currentPlayer.money = _global._maxCrypto.ITS(_global._maxCrypto.STI(TSavegame.currentPlayer.money) + 3);
                    TSavegame.save();
                    this.showWindow(TWindowState.COINS_REWARDED);
                }
                else
                {
                    this.showWindow(TWindowState.VIDEO_ADS_NOT_AVAILABLE);
                }
            }
            else if (this.activeState == TWindowState.VIDEO_ADS_NOT_AVAILABLE)
            {
                this.smallWidgetBG.visible = false;
                this.smallUniButton.visible = false;
                this.smallWidgetInfo.visible = false;
                this.smallWidgetMenu.visible = false;
                this.showWindow(TWindowState.SHOW_FREE_COINS);
            }
            else
            {
                _blackScreen.visible = false;
                this.smallWidgetBG.visible = false;
                this.smallUniButton.visible = false;
                this.smallWidgetInfo.visible = false;
                this.smallWidgetMenu.visible = false;
            }
        }
        else if (btn == TNonStateButton.SELECT_BUTTON)
        {
            this.buttonRight.enabled = true;
            this.widgetBG.visible = false;
            _blackScreen.visible = false;
            this.widgetMenu.visible = false;
            this.widgetInfo.visible = false;
            this.buttonLeft.visible = false;
            this.buttonRight.visible = false;
            this.uniButton.visible = false;
            for (i = 0; i < 5; i++)
            {
                this.playerNameText[i].visible = false;
                this.underLine[i].visible = false;
            }
            TSavegame.lastPlayer = this.choosenPlayerIdx + 1;
            _global._mainMenuScreen.playerNameText.text = TSavegame.playerName[this.choosenPlayerIdx];
            TSavegame.loadPlayer(this.choosenPlayerIdx);
        }
        else if (btn == TNonStateButton.RESUME_BUTTON)
        {
            _blackScreen.visible = false;
            this.widgetBG.visible = false;
            this.widgetInfo.visible = false;
            this.widgetMenu.visible = false;
            this.buttonLeft.visible = false;
            this.buttonRight.visible = false;
            this.soundText.visible = false;
            this.musicText.visible = false;
            this.fullScreenText.visible = false;
            this.soundButton.visible = false;
            this.musicButton.visible = false;
            this.fullScreenButton.visible = false;
            if (_global.currentSceneID == _global.GAMESCREEN_ID)
            {
                _global.gameScreen.paused = false;
            }
            else if (_global.currentSceneID == _global.HIDDEN_OBJECT_ID)
            {
                _global._minigameHO.paused = false;
            }
            else if (_global.currentSceneID == _global.MAKE_DESSERT_ID)
            {
                _global._minigameDessert.paused = false;
                _global._minigameDessert.Resume();
            }
        }
        else if (btn == TNonStateButton.MAINMENU_BUTTON)
        {
            if (TGlobalClass.extrasMode)
            {
                _global._game.fadeToScreen(_global._mainMenuScreen);
            }
            else
            {
                this.showWindow(TWindowState.CONFIRM_MAINMENU);
            }
        }
        else if (btn == TNonStateButton.SOUND_BUTTON)
        {
            _global.MuteSound = !_global.MuteSound;
            if (_global.MuteSound)
            {
                this.soundButton._text.text = "OFF";
                this.soundButton._image.texture = windowsAtlas.getTexture("small_btn_red");
            }
            else
            {
                this.soundButton._text.text = "ON";
                this.soundButton._image.texture = windowsAtlas.getTexture("small_btn_green");
            }
            if (_global.currentSceneID == _global.GAMESCREEN_ID)
            {
                _global.gameScreen.mute_icon.visible = _global.MuteSound;
            }
            else if (_global.currentSceneID == _global.MAKE_DESSERT_ID)
            {
                _global._minigameDessert.mute.visible = _global.MuteSound;
            }
            else if (_global.currentSceneID == _global.HIDDEN_OBJECT_ID)
            {
                _global._minigameHO.mute.visible = _global.MuteSound;
            }
        }
        else if (btn == TNonStateButton.MUSIC_BUTTON)
        {
            _global.MuteMusic = !_global.MuteMusic;
            if (_global.MuteMusic)
            {
                this.musicButton._text.text = "OFF";
                this.musicButton._image.texture = windowsAtlas.getTexture("small_btn_red");
            }
            else
            {
                this.musicButton._text.text = "ON";
                this.musicButton._image.texture = windowsAtlas.getTexture("small_btn_green");
            }
            if (_global.currentSceneID == _global.GAMESCREEN_ID)
            {
                _global.gameScreen.mute_icon_music.visible = _global.MuteMusic;
            }
            else if (_global.currentSceneID == _global.MAKE_DESSERT_ID)
            {
                _global._minigameDessert.muteMusic.visible = _global.MuteMusic;
            }
            else if (_global.currentSceneID == _global.HIDDEN_OBJECT_ID)
            {
                _global._minigameHO.muteMusic.visible = _global.MuteMusic;
            }
        }
        else if (btn == TNonStateButton.KITCHEN)
        {
            TGlobalClass.coinsGot = 0;
            _global._game.fadeToScreen(_global._kitchen);
        }
        else if (btn == TNonStateButton.KITCHEN_WITH_CONFIRMATION)
        {
            this.showWindow(TWindowState.CONFIRM_KITCHEN);
        }
        else if (btn == TNonStateButton.ENDING)
        {
            _global._game.fadeToScreen(_global._ending);
        }
        else if (btn == TNonStateButton.NEXT_WINDOW)
        {
            this.widgetBG.scaleY = 1;
            this.widgetBG.scaleX = 1;
            TGlobalClass.currentNotifWindow = TGlobalClass.currentNotifWindow + 1;
            if (TSavegame.currentPlayer.passedDays < 60
                && TGlobalClass.levelEventXML["day"][TSavegame.currentPlayer.passedDays]?["kitchen"][TGlobalClass.currentNotifWindow] != null
                && _global.currentSceneID == _global.KITCHEN_ID)
            {
                this.showKitchenWindow(TSavegame.currentPlayer.passedDays, TGlobalClass.currentNotifWindow);
            }
            else if (TSavegame.currentPlayer.passedDays < 60
                && TGlobalClass.levelEventXML["day"][TGlobalClass.currentPlayedLevel]?["window"][TGlobalClass.currentNotifWindow] != null
                && !(_global.currentSceneID == _global.KITCHEN_ID))
            {
                this.showBeginningWindow(TGlobalClass.currentPlayedLevel, TGlobalClass.currentNotifWindow);
            }
            else
            {
                if (_global.currentSceneID == _global.GAMESCREEN_ID)
                {
                    _global.gameScreen.paused = false;
                    if (!_global.tutorial.button.visible)
                    {
                        _global.gameScreen.startLevel();
                    }
                }
                else if (_global.currentSceneID == _global.HIDDEN_OBJECT_ID)
                {
                    _global._minigameHO.startLevel();
                }
                else if (_global.currentSceneID == _global.MAKE_DESSERT_ID)
                {
                    if (!_global.tutorial.button.visible)
                    {
                        _global._minigameDessert.startLevel();
                    }
                }
                if (this.widgetBG.visible)
                {
                    _blackScreen.visible = false;
                    this.widgetBG.visible = false;
                    this.widgetInfo.visible = false;
                    this.widgetMenu.visible = false;
                    this.buttonLeft.visible = false;
                    this.buttonRight.visible = false;
                    this.uniButton.visible = false;
                    if (_global.currentSceneID == _global.GAMESCREEN_ID)
                    {
                        for (i = 0; i < 4; i++)
                        {
                            this.dishPreviewAr[i].visible = false;
                        }
                    }
                }
                else if (this.smallWidgetBG.visible)
                {
                    _blackScreen.visible = false;
                    this.smallWidgetBG.visible = false;
                    this.smallWidgetInfo.visible = false;
                    this.smallWidgetMenu.visible = false;
                    this.smallButtonLeft.visible = false;
                    this.smallButtonRight.visible = false;
                    this.smallUniButton.visible = false;
                    for (i = 0; i < 4; i++)
                    {
                        this.dishPreviewAr[i].visible = false;
                    }
                }
            }
        }
        else if (btn == TNonStateButton.HO_BUTTON)
        {
            TGlobalClass.extrasMode = true;
            _global._minigameHO.bonusMoney = "a";
            _global._game.fadeToScreen(_global._minigameHO);
        }
        else if (btn == TNonStateButton.DM_BUTTON)
        {
            TGlobalClass.extrasMode = true;
            _global._game.fadeToScreen(_global._minigameDessert);
        }
        else if (btn == TNonStateButton.NEXT_LEVEL)
        {
            _global._minigameHO.currentLevel = _global._minigameHO.currentLevel + 1;
            _global._game.fadeToScreen(_global._minigameHO);
        }
        else if (btn == TNonStateButton.SUBMIT_BUTTON)
        {
            if (!TGlobalClass.extrasMode)
            {
                GALib.TrackEvent(GALib.EVENT_SUBMIT_SCORE, TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS], "STORY MODE");
            }
            else if (_global.currentSceneID == _global.MAKE_DESSERT_ID)
            {
                GALib.TrackEvent(GALib.EVENT_SUBMIT_SCORE, _global._maxCrypto.STI(_global._minigameDessert.earning), "DESSERT MAKING MODE");
            }
            else
            {
                GALib.TrackEvent(GALib.EVENT_SUBMIT_SCORE, _global._maxCrypto.STI(_global._minigameHO.bonusMoney), "HIDDEN OBJECTS MODE");
            }
        }
        else if (btn == TNonStateButton.LEADERBOARD_BUTTON)
        {
        }
        else if (btn == TNonStateButton.UNLOCK_BUTTON)
        {
        }
        else if (btn == TNonStateButton.FULLSCREEN_BUTTON)
        {
            TSavegame.fullScreen = !TSavegame.fullScreen;
            _global._stage.displayState = TSavegame.fullScreen ? StageDisplayState.FULL_SCREEN_INTERACTIVE : StageDisplayState.NORMAL;
            TSavegame._saveGame.data["fullScreen"] = TSavegame.fullScreen;
            if (!TSavegame.fullScreen)
            {
                this.fullScreenButton._text.text = "OFF";
                this.fullScreenButton._image.texture = windowsAtlas.getTexture("small_btn_red");
            }
            else
            {
                this.fullScreenButton._text.text = "ON";
                this.fullScreenButton._image.texture = windowsAtlas.getTexture("small_btn_green");
            }
        }
    }

    public void hideAllWindows(int arg1)
    {
        if (arg1 == _global.MAINMENU_ID)
        {
            for (int i = 0; i < 5; i++)
            {
                this.playerNameText[i].visible = false;
                this.underLine[i].visible = false;
            }
            this.inputName.visible = false;
        }
        if (arg1 == _global.GAMESCREEN_ID || arg1 == _global.HIDDEN_OBJECT_ID || arg1 == _global.MAKE_DESSERT_ID)
        {
            this.soundButton.visible = false;
            this.soundText.visible = false;
            this.musicButton.visible = false;
            this.musicText.visible = false;
            this.fullScreenButton.visible = false;
            this.fullScreenText.visible = false;
            this.summaryStats.visible = false;
            this.summaryTips.visible = false;
            this.summaryText.visible = false;
            this.goalIcon.visible = false;
            this.expertBlink.visible = false;
        }
        this.widgetBG.visible = false;
        this.widgetInfo.visible = false;
        this.widgetMenu.visible = false;
        this.uniButton.visible = false;
        this.buttonLeft.visible = false;
        this.buttonRight.visible = false;
        this.smallButtonLeft.visible = false;
        this.smallButtonRight.visible = false;
        this.smallUniButton.visible = false;
        this.smallWidgetBG.visible = false;
        this.smallWidgetInfo.visible = false;
        this.smallWidgetMenu.visible = false;
        _blackScreen.visible = false;
    }

    /// <summary>"-" when the hardware (juicer / fryer) is not bought yet, otherwise the served count.</summary>
    public string servedOrDash(int hardware, int served)
    {
        return TSavegame.currentPlayer.hardwareLevel[hardware] == 0 ? "-" : AS3.str(served);
    }

    public void showWindow(int arg1)
    {
        int i;
        int usedCount = 0;
        bool expert;
        string earning;
        this.activeState = arg1;
        if (arg1 == TWindowState.EXIT_WINDOW)
        {
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
            this.smallWidgetBG.visible = true;
            this.smallWidgetMenu.visible = true;
            this.smallWidgetInfo.visible = true;
            this.smallUniButton.visible = true;
            this.smallButtonLeft.visible = true;
            this.smallButtonRight.visible = true;
            this.smallButtonLeft._text.text = "No";
            this.smallButtonRight._text.text = "Yes";
            this.smallUniButton._text.text = "More Games";
            this.smallWidgetMenu.text = "Exit?";
            this.smallWidgetInfo.text = "Are you sure want to exit game?";
            this.smallWidgetBG.x = 512;
            this.smallWidgetBG.y = 300;
            this.smallWidgetMenu.x = 525;
            this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
            this.smallWidgetInfo.x = 525;
            this.smallWidgetInfo.y = this.smallWidgetBG.y - 30;
            this.smallButtonLeft.x = this.smallWidgetBG.x - 100;
            this.smallButtonLeft.y = this.smallWidgetBG.y + 130;
            this.smallButtonRight.x = this.smallWidgetBG.x + 100;
            this.smallButtonRight.y = this.smallWidgetBG.y + 130;
            this.smallUniButton.x = 525;
            this.smallUniButton.y = this.smallWidgetBG.y + 80;
            this.smallUniButton.idx = TNonStateButton.MOREGAMES_BUTTON;
            this.smallButtonLeft.idx = TNonStateButton.NO_BUTTON;
            this.smallButtonRight.idx = TNonStateButton.YES_BUTTON;
        }
        else if (arg1 == TWindowState.CHANGE_PLAYER)
        {
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetMenu.visible = true;
            this.buttonLeft.visible = true;
            this.buttonRight.visible = true;
            this.uniButton.visible = true;
            for (i = 0; i < 5; i++)
            {
                this.underLine[i].visible = true;
                this.playerNameText[i].visible = true;
                this.playerNameText[i].idx = i;
                this.playerNameText[i].text = "Empty";
                this.playerNameText[i].color = 0;
                if (TSavegame.usedSlot[i])
                {
                    this.playerNameText[i].text = TSavegame.playerName[i];
                    usedCount++;
                }
            }
            this.buttonLeft.enabled = false;
            this.uniButton._text.text = "Close";
            this.uniButton.idx = TNonStateButton.CANCEL_BUTTON;
            if (usedCount >= 5)
            {
                this.buttonRight.enabled = false;
            }
            else
            {
                this.buttonRight.enabled = true;
            }
            int last = TSavegame.lastPlayer;
            if (last > 0)
            {
                this.playerNameText[last - 1].color = 16711680;
                this.choosenPlayerIdx = last - 1;
                this.buttonLeft.enabled = true;
                this.uniButton._text.text = "Select";
                this.uniButton.idx = TNonStateButton.SELECT_BUTTON;
            }
            this.widgetMenu.text = "Select Player";
            this.buttonLeft._text.text = "Delete";
            this.buttonRight._text.text = "Create";
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.buttonLeft.x = this.widgetBG.x - 170;
            this.buttonLeft.y = this.widgetBG.y + 200;
            this.buttonRight.x = this.widgetBG.x + 170;
            this.buttonRight.y = this.widgetBG.y + 200;
            this.uniButton.x = this.widgetBG.x;
            this.uniButton.y = this.widgetBG.y + 200;
            this.buttonLeft.idx = TNonStateButton.DELETE_BUTTON;
            this.buttonRight.idx = TNonStateButton.CREATE_BUTOTN;
        }
        else if (arg1 == TWindowState.CREATE_PLAYER)
        {
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
            this.smallWidgetBG.visible = true;
            this.smallButtonLeft.visible = true;
            this.smallButtonRight.visible = true;
            this.smallWidgetMenu.visible = true;
            this.smallWidgetInfo.visible = true;
            this.inputName.visible = true;
            this.smallWidgetMenu.text = "Create Player";
            this.smallWidgetInfo.text = "Enter Your Name :";
            this.smallButtonLeft._text.text = "Cancel";
            this.smallButtonRight._text.text = "Done";
            this.inputName.text = "";
            this.smallButtonRight.enabled = false;
            // Starling.current.nativeStage.focus = inputName: NativeTextField takes keyboard focus by itself while visible.
            this.smallWidgetBG.x = 512;
            this.smallWidgetBG.y = 300;
            this.smallWidgetMenu.x = 525;
            this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
            this.smallWidgetInfo.x = 525;
            this.smallWidgetInfo.y = this.smallWidgetBG.y - 30;
            this.smallButtonLeft.x = this.smallWidgetBG.x - 100;
            this.smallButtonLeft.y = this.smallWidgetBG.y + 130;
            this.smallButtonRight.x = this.smallWidgetBG.x + 100;
            this.smallButtonRight.y = this.smallWidgetBG.y + 130;
            // stage scale factors dropped (Starlite stage is always 1024x600)
            this.inputName.x = _global._stage.stageWidth / 2f - (this.inputName.width / 2 - 10);
            this.inputName.y = this.smallWidgetBG.y;
            this.smallButtonRight.idx = TNonStateButton.DONE_CREATE_PLAYER;
            this.smallButtonLeft.idx = TNonStateButton.CANCEL_BUTTON;
        }
        else if (arg1 == TWindowState.SAME_NAME_WARNING)
        {
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
            this.smallWidgetBG.visible = true;
            this.smallUniButton.visible = true;
            this.smallWidgetMenu.visible = true;
            this.smallWidgetInfo.visible = true;
            this.smallWidgetMenu.text = "Try Again!";
            this.smallWidgetInfo.text = "Oops, the name you entered is already exist. Please try again!";
            this.smallUniButton._text.text = "OK";
            this.smallWidgetBG.x = 512;
            this.smallWidgetBG.y = 300;
            this.smallWidgetMenu.x = 525;
            this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
            this.smallWidgetInfo.x = 525;
            this.smallWidgetInfo.y = this.smallWidgetBG.y - 30;
            this.smallUniButton.x = this.smallWidgetBG.x;
            this.smallUniButton.y = this.smallWidgetBG.y + 130;
            this.smallUniButton.idx = TNonStateButton.OK_BUTTON;
        }
        else if (arg1 == TWindowState.DELETE_PLAYER)
        {
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
            this.smallWidgetBG.visible = true;
            this.smallButtonLeft.visible = true;
            this.smallButtonRight.visible = true;
            this.smallWidgetMenu.visible = true;
            this.smallWidgetInfo.visible = true;
            this.smallWidgetMenu.text = "Delete Player?";
            this.smallWidgetInfo.text = "Are you sure want to delete this player? This process can't be undone.";
            this.smallButtonLeft._text.text = "Cancel";
            this.smallButtonRight._text.text = "Delete";
            this.smallWidgetBG.x = 512;
            this.smallWidgetBG.y = 300;
            this.smallWidgetMenu.x = 525;
            this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
            this.smallWidgetInfo.x = 525;
            this.smallWidgetInfo.y = this.smallWidgetBG.y - 30;
            this.smallButtonLeft.x = this.smallWidgetBG.x - 100;
            this.smallButtonLeft.y = this.smallWidgetBG.y + 130;
            this.smallButtonRight.x = this.smallWidgetBG.x + 100;
            this.smallButtonRight.y = this.smallWidgetBG.y + 130;
            this.smallButtonLeft.idx = TNonStateButton.CANCEL_BUTTON;
            this.smallButtonRight.idx = TNonStateButton.DELETE_BUTTON;
        }
        else if (arg1 == TWindowState.NEW_GAME)
        {
        }
        else if (arg1 == TWindowState.PAUSE_GAME)
        {
            if (_global.gameScreen.tmpDrink != null)
            {
                _global._gameS.setChildIndex(_global.gameScreen.tmpDrink, _global._gameS.getChildIndex(this.widgetBG) - 1);
            }
            if (_global.gameScreen.tmpFries != null)
            {
                _global._gameS.setChildIndex(_global.gameScreen.tmpFries, _global._gameS.getChildIndex(this.widgetBG) - 1);
            }
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetInfo.visible = true;
            this.widgetMenu.visible = true;
            this.buttonLeft.visible = true;
            this.buttonRight.visible = true;
            this.soundText.visible = true;
            this.soundButton.visible = true;
            this.musicText.visible = true;
            this.musicButton.visible = true;
            this.fullScreenText.visible = true;
            this.fullScreenButton.visible = true;
            if (TGlobalClass.extrasMode)
            {
                this.widgetInfo.visible = false;
            }
            this.widgetMenu.text = "Paused";
            this.buttonLeft._text.text = TGlobalClass.extrasMode ? "Main Menu" : "Kitchen";
            this.buttonRight._text.text = "Resume";
            this.soundText.text = "SOUND :";
            if (_global.MuteSound)
            {
                this.soundButton._text.text = "OFF";
                this.soundButton._image.texture = windowsAtlas.getTexture("small_btn_red");
            }
            else
            {
                this.soundButton._text.text = "ON";
                this.soundButton._image.texture = windowsAtlas.getTexture("small_btn_green");
            }
            if (_global.MuteMusic)
            {
                this.musicButton._text.text = "OFF";
                this.musicButton._image.texture = windowsAtlas.getTexture("small_btn_red");
            }
            else
            {
                this.musicButton._text.text = "ON";
                this.musicButton._image.texture = windowsAtlas.getTexture("small_btn_green");
            }
            // (sic) the original repeats the music button block
            if (_global.MuteMusic)
            {
                this.musicButton._text.text = "OFF";
                this.musicButton._image.texture = windowsAtlas.getTexture("small_btn_red");
            }
            else
            {
                this.musicButton._text.text = "ON";
                this.musicButton._image.texture = windowsAtlas.getTexture("small_btn_green");
            }
            if (!TSavegame.fullScreen)
            {
                this.fullScreenButton._text.text = "OFF";
                this.fullScreenButton._image.texture = windowsAtlas.getTexture("small_btn_red");
            }
            else
            {
                this.fullScreenButton._text.text = "ON";
                this.fullScreenButton._image.texture = windowsAtlas.getTexture("small_btn_green");
            }
            this.soundButton.x = this.widgetBG.x;
            this.soundButton.y = this.widgetBG.y - 110;
            this.soundText.x = this.widgetBG.x - 200;
            this.soundText.y = this.widgetBG.y - 100;
            this.musicButton.x = this.widgetBG.x;
            this.musicButton.y = this.widgetBG.y - 40;
            this.musicText.x = this.widgetBG.x - 200;
            this.musicText.y = this.widgetBG.y - 40;
            this.fullScreenButton.x = this.widgetBG.x;
            this.fullScreenButton.y = this.widgetBG.y + 30;
            this.fullScreenText.x = this.widgetBG.x - 240;
            this.fullScreenText.y = this.widgetBG.y + 30;
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.widgetInfo.x = 512;
            this.widgetInfo.y = this.widgetBG.y + 130;
            this.buttonLeft.x = this.widgetBG.x - 150;
            this.buttonLeft.y = this.widgetBG.y + 200;
            this.buttonRight.x = this.widgetBG.x + 140;
            this.buttonRight.y = this.widgetBG.y + 200;
            this.soundButton.idx = TNonStateButton.SOUND_BUTTON;
            this.musicButton.idx = TNonStateButton.MUSIC_BUTTON;
            this.fullScreenButton.idx = TNonStateButton.FULLSCREEN_BUTTON;
            this.buttonLeft.idx = TGlobalClass.extrasMode ? TNonStateButton.MAINMENU_BUTTON : TNonStateButton.KITCHEN_WITH_CONFIRMATION;
            this.buttonRight.idx = TNonStateButton.RESUME_BUTTON;
        }
        else if (arg1 == TWindowState.STORY_LEVELCOMPLETE)
        {
            _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
            GALib.TrackEvent(GALib.EVENT_COMPLETE_STORY, TGlobalClass.currentPlayedLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetInfo.visible = true;
            this.widgetMenu.visible = true;
            this.uniButton.visible = true;
            this.summaryStats.visible = true;
            this.summaryTips.visible = true;
            this.summaryText.visible = true;
            expert = this.showGoalIcon();
            earning = AS3.str(ScoreManager.totalScore);
            if (TGlobalClass.isPlayingSpecialLevel)
            {
                this.summaryText.text = "Customers Served\t\t :\nBeverage Served\t\t :\nFrench Fries Served\t :\nVIP Rating Bonus\t\t :\n\nToday’s Earnings\t\t :\nTotal Coins\t\t\t :";
                this.summaryStats.text = _global.gameScreen.servedCustomerCount + "\n"
                    + this.servedOrDash(Dish.JUICER, TGlobalClass.juiceServed) + "\n"
                    + this.servedOrDash(Dish.FRYER, TGlobalClass.friesServed) + "\n"
                    + _global.gameScreen.VIPServeBonus + "\n\n$" + _global._maxCrypto.STI(earning) + "\n"
                    + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
            }
            else
            {
                this.summaryText.text = "Customers Served\t\t :\nBeverage Served\t\t :\nFrench Fries Served\t :\n\nToday's Earnings\t\t :\nTotal Coins\t\t\t :";
                this.summaryStats.text = _global.gameScreen.servedCustomerCount + "\n"
                    + this.servedOrDash(Dish.JUICER, TGlobalClass.juiceServed) + "\n"
                    + this.servedOrDash(Dish.FRYER, TGlobalClass.friesServed) + "\n\n$" + _global._maxCrypto.STI(earning) + "\n"
                    + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
            }
            this.widgetMenu.text = "Congratulations!";
            this.widgetInfo.text = "You've reached today's goal!";
            this.uniButton._text.text = "Kitchen";
            this.summaryTips.text = expert ? "Awesome! Couldn't have done it better myself!" : "Well done! Try to reach Expert target to earn even more coins!";
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.widgetInfo.x = 512;
            this.widgetInfo.y = this.widgetBG.y - 170;
            this.uniButton.x = 512;
            this.uniButton.y = this.widgetBG.y + 200;
            this.uniButton.idx = TNonStateButton.KITCHEN;
            _global.psCongrats.start();
            TweenLite.delayedCall(6, (Action)_global.psCongrats.pause);
            this.leaderboardBtn.visible = this.submitScoreBtn.visible = TGlobalClass.EnableLeaderboardBtn;
        }
        else if (arg1 == TWindowState.STORY_LEVELFAILED)
        {
            GALib.TrackEvent(GALib.EVENT_LOSE_STORY, TGlobalClass.currentPlayedLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
            _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetInfo.visible = true;
            this.widgetMenu.visible = true;
            this.uniButton.visible = true;
            this.summaryStats.visible = true;
            this.summaryTips.visible = true;
            this.summaryText.visible = true;
            this.summaryText.text = "Customers Served\t\t :\nBeverage Served\t\t :\nFrench Fries Served\t :\n\nTotal Coins\t\t\t :";
            this.summaryStats.text = _global.gameScreen.servedCustomerCount + "\n"
                + this.servedOrDash(Dish.JUICER, TGlobalClass.juiceServed) + "\n"
                + this.servedOrDash(Dish.FRYER, TGlobalClass.friesServed) + "\n\n"
                + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
            this.widgetMenu.text = "Level Failed!";
            this.widgetInfo.text = "You've failed to meet today's goal!";
            this.uniButton._text.text = "Kitchen";
            this.summaryTips.text = "If you reach the normal Daily Goal you will receive 3 coins, but when you reach the Expert Goal you'll earn 6 coins.";
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.widgetInfo.x = 512;
            this.widgetInfo.y = this.widgetBG.y - 180;
            this.uniButton.x = 512;
            this.uniButton.y = this.widgetBG.y + 200;
            this.uniButton.idx = TNonStateButton.KITCHEN;
        }
        else if (arg1 == TWindowState.STORY_COMPLETE_ALL)
        {
            GALib.TrackEvent(GALib.EVENT_COMPLETE_STORY, TGlobalClass.currentPlayedLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
            _global.ReportGameState(_global.GAMESTATE_GAME_END);
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetInfo.visible = true;
            this.widgetMenu.visible = true;
            this.uniButton.visible = true;
            this.summaryText.visible = true;
            this.summaryStats.visible = true;
            this.showGoalIcon();
            earning = AS3.str(ScoreManager.totalScore);
            this.summaryText.text = "Customers Served\t\t :\nBeverage Served\t\t :\nFrench Fries Served\t :\n\nToday's Earnings\t\t :\nTotal Coins\t\t\t :";
            this.summaryStats.text = _global.gameScreen.servedCustomerCount + "\n"
                + this.servedOrDash(Dish.JUICER, TGlobalClass.juiceServed) + "\n"
                + this.servedOrDash(Dish.FRYER, TGlobalClass.friesServed) + "\n\n$" + _global._maxCrypto.STI(earning) + "\n"
                + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
            this.widgetMenu.text = "Congratulations!";
            this.widgetInfo.text = "You've reached today's goal and finished all levels!";
            this.uniButton._text.text = "Huraay!!";
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.widgetInfo.x = 512;
            this.widgetInfo.y = this.widgetBG.y - 165;
            this.uniButton.x = 512;
            this.uniButton.y = this.widgetBG.y + 200;
            if (!TSavegame.currentPlayer.endingViewed)
            {
                this.uniButton.idx = TNonStateButton.ENDING;
            }
            else
            {
                this.uniButton.idx = TNonStateButton.KITCHEN;
            }
            _global.psCongrats.start();
            TweenLite.delayedCall(6, (Action)_global.psCongrats.pause);
            this.leaderboardBtn.visible = this.submitScoreBtn.visible = TGlobalClass.EnableLeaderboardBtn;
        }
        else if (arg1 == TWindowState.HO_LEVEL_COMPLETE)
        {
            GALib.TrackEvent(GALib.EVENT_COMPLETE_STORY, TGlobalClass.currentPlayedLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
            _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetInfo.visible = true;
            this.widgetMenu.visible = true;
            this.uniButton.visible = true;
            this.summaryStats.visible = true;
            this.summaryTips.visible = true;
            this.summaryText.visible = true;
            expert = this.showGoalIcon();
            earning = _global._minigameHO.bonusMoney;
            this.summaryText.text = "Objects Found\t\t:\nToday's Earning\t:\n\nTotal Coins\t\t:";
            this.summaryStats.text = _global._minigameHO._HOClass.itemFoundCount + "\n$" + _global._maxCrypto.STI(earning) + "\n\n"
                + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
            this.widgetMenu.text = "Summary";
            this.widgetInfo.text = "You've completed this level.";
            this.uniButton._text.text = "Kitchen";
            this.summaryTips.text = expert ? "Awesome! Couldn't have done it better myself!" : "Well done! Try to reach Expert target to earn even more coins!";
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.widgetInfo.x = 512;
            this.widgetInfo.y = this.widgetBG.y - 180;
            this.uniButton.x = 512;
            this.uniButton.y = this.widgetBG.y + 200;
            this.uniButton.idx = TNonStateButton.KITCHEN;
            _global.psCongrats.start();
            TweenLite.delayedCall(6, (Action)_global.psCongrats.pause);
            this.leaderboardBtn.visible = this.submitScoreBtn.visible = TGlobalClass.EnableLeaderboardBtn;
        }
        else if (arg1 == TWindowState.HO_LEVEL_FAILED)
        {
            GALib.TrackEvent(GALib.EVENT_LOSE_STORY, TGlobalClass.currentPlayedLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
            _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetInfo.visible = true;
            this.widgetMenu.visible = true;
            this.uniButton.visible = true;
            this.summaryStats.visible = true;
            this.summaryTips.visible = true;
            this.summaryText.visible = true;
            earning = _global._minigameHO.bonusMoney;
            this.summaryText.text = "Objects Found\t:\nToday's Earning\t:\n\nTotal Coins\t:";
            this.summaryStats.text = _global._minigameHO._HOClass.itemFoundCount + "\n$" + _global._maxCrypto.STI(earning) + "\n\n"
                + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
            this.widgetMenu.text = "Level Failed";
            this.widgetInfo.text = "You've failed this level.";
            this.uniButton._text.text = "Kitchen";
            this.summaryTips.text = "If you reach the normal Daily Goal you will receive 3 coins, but when you reach the Expert Goal you'll earn 6 coins.";
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.widgetInfo.x = 512;
            this.widgetInfo.y = this.widgetBG.y - 180;
            this.uniButton.x = 512;
            this.uniButton.y = this.widgetBG.y + 200;
            this.uniButton.idx = TNonStateButton.KITCHEN;
            _global.psCongrats.start();
            TweenLite.delayedCall(6, (Action)_global.psCongrats.pause);
        }
        else if (arg1 == TWindowState.HO_LEVEL_COMPLETE_EXTRAS)
        {
            GALib.TrackEvent(GALib.EVENT_COMPLETE_EXTRAS, _global._minigameHO.currentLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
            _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetInfo.visible = true;
            this.widgetMenu.visible = true;
            this.uniButton.visible = true;
            this.summaryStats.visible = true;
            this.summaryText.visible = true;
            this.summaryText.text = "Objects Found\t:\nScore\t\t:";
            this.summaryStats.text = _global._minigameHO._HOClass.itemFoundCount + "\n" + _global._maxCrypto.STI(_global._minigameHO.bonusMoney);
            this.widgetMenu.text = "Summary";
            this.widgetInfo.text = "You've completed this level.";
            this.uniButton._text.text = "Next Level";
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.widgetInfo.x = 512;
            this.widgetInfo.y = this.widgetBG.y - 180;
            this.uniButton.x = 512;
            this.uniButton.y = this.widgetBG.y + 200;
            this.uniButton.idx = TNonStateButton.NEXT_LEVEL;
            _global.psCongrats.start();
            TweenLite.delayedCall(6, (Action)_global.psCongrats.pause);
            this.leaderboardBtn.visible = this.submitScoreBtn.visible = TGlobalClass.EnableLeaderboardBtn;
        }
        else if (arg1 == TWindowState.HO_LEVEL_FAILED_EXTRAS)
        {
            GALib.TrackEvent(GALib.EVENT_LOSE_EXTRAS, _global._minigameHO.currentLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
            _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetInfo.visible = true;
            this.widgetMenu.visible = true;
            this.uniButton.visible = true;
            this.summaryStats.visible = true;
            this.summaryText.visible = true;
            this.summaryText.text = "Objects Found\t:\nScore\t\t:";
            this.summaryStats.text = _global._minigameHO._HOClass.itemFoundCount + "\n" + _global._maxCrypto.STI(_global._minigameHO.bonusMoney);
            this.widgetMenu.text = "Level Failed!";
            this.widgetInfo.text = "You've failed this level.";
            this.uniButton._text.text = "Main Menu";
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.widgetInfo.x = 512;
            this.widgetInfo.y = this.widgetBG.y - 180;
            this.uniButton.x = 512;
            this.uniButton.y = this.widgetBG.y + 200;
            this.uniButton.idx = TNonStateButton.MAINMENU_BUTTON;
        }
        else if (arg1 == TWindowState.HO_LEVEL_COMPLETE_EXTRAS_ALL)
        {
            GALib.TrackEvent(GALib.EVENT_COMPLETE_EXTRAS, _global._minigameHO.currentLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
            _global.ReportGameState(_global.GAMESTATE_GAME_END);
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetInfo.visible = true;
            this.widgetMenu.visible = true;
            this.uniButton.visible = true;
            this.summaryStats.visible = true;
            this.summaryText.visible = true;
            this.summaryText.text = "Objects Found\t:\nScore\t\t:";
            this.summaryStats.text = _global._minigameHO._HOClass.itemFoundCount + "\n" + _global._maxCrypto.STI(_global._minigameHO.bonusMoney);
            this.widgetMenu.text = "Congratulations!";
            this.widgetInfo.text = "You've completed all levels.";
            this.uniButton._text.text = "Main Menu";
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.widgetInfo.x = 512;
            this.widgetInfo.y = this.widgetBG.y - 180;
            this.uniButton.x = 512;
            this.uniButton.y = this.widgetBG.y + 200;
            this.uniButton.idx = TNonStateButton.MAINMENU_BUTTON;
            _global.psCongrats.start();
            TweenLite.delayedCall(6, (Action)_global.psCongrats.pause);
            this.leaderboardBtn.visible = this.submitScoreBtn.visible = TGlobalClass.EnableLeaderboardBtn;
        }
        else if (arg1 == TWindowState.DESSERT_LEVEL_COMPLETE)
        {
            GALib.TrackEvent(GALib.EVENT_COMPLETE_STORY, TGlobalClass.currentPlayedLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
            _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetInfo.visible = true;
            this.widgetMenu.visible = true;
            this.uniButton.visible = true;
            this.summaryStats.visible = true;
            this.summaryText.visible = true;
            this.summaryTips.visible = true;
            expert = this.showGoalIcon();
            earning = _global._minigameDessert.earning;
            this.summaryText.text = "Froyo Served\t\t:\nToday's Earning\t:\n\nTotal Coins\t\t:";
            this.summaryStats.text = _global._minigameDessert.froyoMadeCount + "\n$" + _global._maxCrypto.STI(earning) + "\n\n"
                + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
            this.widgetMenu.text = "Level Complete!";
            this.widgetInfo.text = "You've completed this level.";
            this.summaryTips.text = expert ? "Awesome! Couldn't have done it better myself!" : "Well done! Try to reach Expert target to earn even more coins!";
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.widgetInfo.x = 512;
            this.widgetInfo.y = this.widgetBG.y - 180;
            this.uniButton.x = 512;
            this.uniButton.y = this.widgetBG.y + 200;
            this.uniButton._text.text = "Kitchen";
            this.uniButton.idx = TNonStateButton.KITCHEN;
            _global.psCongrats.start();
            TweenLite.delayedCall(6, (Action)_global.psCongrats.pause);
            this.leaderboardBtn.visible = this.submitScoreBtn.visible = TGlobalClass.EnableLeaderboardBtn;
        }
        else if (arg1 == TWindowState.DESSERT_LEVEL_COMPLETE_EXTRAS)
        {
            GALib.TrackEvent(GALib.EVENT_COMPLETE_EXTRAS, _global._minigameDessert.level, _global.getCurrentSceneNameByID(_global.currentSceneID));
            _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetInfo.visible = true;
            this.widgetMenu.visible = true;
            this.uniButton.visible = true;
            this.summaryStats.visible = true;
            this.summaryText.visible = true;
            earning = _global._minigameDessert.earning;
            this.summaryText.text = "Froyo Served\t\t:";
            this.summaryStats.text = _global._minigameDessert.froyoMadeCount + "";
            this.widgetMenu.text = "Congratulations!";
            this.widgetInfo.text = "You've completed this game.";
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.widgetInfo.x = 512;
            this.widgetInfo.y = this.widgetBG.y - 180;
            this.uniButton.x = 512;
            this.uniButton.y = this.widgetBG.y + 200;
            this.uniButton._text.text = "Main Menu";
            this.uniButton.idx = TNonStateButton.MAINMENU_BUTTON;
            _global.psCongrats.start();
            TweenLite.delayedCall(6, (Action)_global.psCongrats.pause);
            this.leaderboardBtn.visible = this.submitScoreBtn.visible = TGlobalClass.EnableLeaderboardBtn;
        }
        else if (arg1 == TWindowState.DESSERT_LEVEL_FAILED)
        {
            GALib.TrackEvent(GALib.EVENT_LOSE_STORY, TGlobalClass.currentPlayedLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
            _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetInfo.visible = true;
            this.widgetMenu.visible = true;
            this.uniButton.visible = true;
            this.summaryStats.visible = true;
            this.summaryTips.visible = true;
            this.summaryText.visible = true;
            this.summaryText.text = "Froyo Served\t:\nToday's Earning\t:\n\nTotal Coins\t:";
            this.summaryStats.text = _global._minigameDessert.froyoMadeCount + "\n$" + _global._maxCrypto.STI(_global._minigameDessert.earning) + "\n\n"
                + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
            this.widgetMenu.text = "Level Failed!";
            this.widgetInfo.text = "You've failed this level.";
            this.summaryTips.text = "If you reach the normal Daily Goal you will receive 3 coins, but when you reach the Expert Goal you'll earn 6 coins.";
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.widgetInfo.x = 512;
            this.widgetInfo.y = this.widgetBG.y - 180;
            this.uniButton.x = 512;
            this.uniButton.y = this.widgetBG.y + 200;
            this.uniButton._text.text = "Kitchen";
            this.uniButton.idx = TNonStateButton.KITCHEN;
        }
        else if (arg1 == TWindowState.DESSERT_LEVEL_FAILED_EXTRAS)
        {
            GALib.TrackEvent(GALib.EVENT_LOSE_EXTRAS, _global._minigameDessert.level, _global.getCurrentSceneNameByID(_global.currentSceneID));
            _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetInfo.visible = true;
            this.widgetMenu.visible = true;
            this.uniButton.visible = true;
            this.summaryStats.visible = true;
            this.summaryText.visible = true;
            this.summaryText.text = "Froyo Served\t:";
            this.summaryStats.text = _global._minigameDessert.froyoMadeCount + "";
            this.widgetMenu.text = "You Failed!";
            this.widgetInfo.text = "You've failed this game.";
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetMenu.x = 512;
            this.widgetMenu.y = this.widgetBG.y - 220;
            this.widgetInfo.x = 512;
            this.widgetInfo.y = this.widgetBG.y - 180;
            this.uniButton.x = 512;
            this.uniButton.y = this.widgetBG.y + 200;
            this.uniButton._text.text = "Main Menu";
            this.uniButton.idx = TNonStateButton.MAINMENU_BUTTON;
        }
        else if (arg1 == TWindowState.CONFIRM_MAINMENU)
        {
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
            this.smallWidgetBG.visible = true;
            this.smallWidgetMenu.visible = true;
            this.smallWidgetInfo.visible = true;
            this.smallButtonLeft.visible = true;
            this.smallButtonRight.visible = true;
            this.smallButtonLeft._text.text = "No";
            this.smallButtonLeft._image.visible = true;
            this.smallButtonRight._image.visible = true;
            this.smallButtonLeft._image.texture = windowsAtlas.getTexture("small_btn_blue");
            this.smallButtonRight._image.texture = windowsAtlas.getTexture("small_btn_blue");
            this.smallButtonRight._text.text = "Yes";
            this.smallWidgetMenu.text = "Main Menu?";
            this.smallWidgetInfo.text = "Are you sure want to exit to the Main Menu? Your progress in this level will not be saved.";
            this.smallWidgetBG.x = 512;
            this.smallWidgetBG.y = 300;
            this.smallWidgetMenu.x = 525;
            this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
            this.smallWidgetInfo.x = 525;
            this.smallWidgetInfo.y = this.smallWidgetBG.y - 15;
            this.smallButtonLeft.x = this.smallWidgetBG.x - 100;
            this.smallButtonLeft.y = this.smallWidgetBG.y + 130;
            this.smallButtonRight.x = this.smallWidgetBG.x + 100;
            this.smallButtonRight.y = this.smallWidgetBG.y + 130;
            this.smallButtonLeft.idx = TNonStateButton.NO_BUTTON;
            this.smallButtonRight.idx = TNonStateButton.YES_BUTTON;
        }
        else if (arg1 == TWindowState.CONFIRM_KITCHEN)
        {
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
            this.smallWidgetBG.visible = true;
            this.smallWidgetMenu.visible = true;
            this.smallWidgetInfo.visible = true;
            this.smallButtonLeft.visible = true;
            this.smallButtonRight.visible = true;
            this.smallButtonLeft._text.text = "No";
            this.smallButtonLeft._image.visible = true;
            this.smallButtonRight._image.visible = true;
            this.smallButtonLeft._image.texture = windowsAtlas.getTexture("small_btn_blue");
            this.smallButtonRight._image.texture = windowsAtlas.getTexture("small_btn_blue");
            this.smallButtonRight._text.text = "Yes";
            this.smallWidgetMenu.text = "Go to kitchen?";
            this.smallWidgetInfo.text = "Are you sure want to exit to the Kitchen? Your progress in this level will not be saved.";
            this.smallWidgetBG.x = 512;
            this.smallWidgetBG.y = 300;
            this.smallWidgetMenu.x = 525;
            this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
            this.smallWidgetInfo.x = 525;
            this.smallWidgetInfo.y = this.smallWidgetBG.y - 15;
            this.smallButtonLeft.x = this.smallWidgetBG.x - 100;
            this.smallButtonLeft.y = this.smallWidgetBG.y + 130;
            this.smallButtonRight.x = this.smallWidgetBG.x + 100;
            this.smallButtonRight.y = this.smallWidgetBG.y + 130;
            this.smallButtonLeft.idx = TNonStateButton.NO_BUTTON;
            this.smallButtonRight.idx = TNonStateButton.YES_BUTTON;
        }
        else if (arg1 == TWindowState.EXTRAS_MENU)
        {
            _global.ReportGameState(_global.GAMESTATE_LEVEL_SELECT);
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.buttonRight.visible = true;
            this.buttonLeft.visible = true;
            this.widgetMenu.visible = true;
            this.uniButton.visible = true;
            this.buttonLeft.enabled = true;
            this.buttonLeft.scaleX = 1.2f;
            this.buttonLeft.scaleY = 1.2f;
            this.buttonRight.scaleX = 1.2f;
            this.buttonRight.scaleY = 1.2f;
            this.uniButton.scaleX = 1.2f;
            this.uniButton.scaleY = 1.2f;
            this.widgetMenu.text = "Select Game";
            this.buttonLeft._text.text = "Hidden Object";
            this.buttonRight._text.text = "Dessert Making";
            this.uniButton._text.text = "Cancel";
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetMenu.x = 525;
            this.widgetMenu.y = this.widgetBG.y - 200;
            this.buttonLeft.x = this.widgetBG.x;
            this.buttonLeft.y = this.widgetBG.y - 50;
            this.buttonRight.x = this.widgetBG.x;
            this.buttonRight.y = this.widgetBG.y + 40;
            this.uniButton.x = this.widgetBG.x;
            this.uniButton.y = this.widgetBG.y + 130;
            this.buttonLeft.idx = TNonStateButton.HO_BUTTON;
            this.buttonRight.idx = TNonStateButton.DM_BUTTON;
            this.uniButton.idx = TNonStateButton.CANCEL_BUTTON;
        }
        else if (arg1 == TWindowState.SHOW_EXPERT_NEEDED_INFO)
        {
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
            this.smallWidgetBG.visible = true;
            this.smallUniButton.visible = true;
            this.smallWidgetMenu.visible = true;
            this.smallWidgetInfo.visible = true;
            this.smallWidgetMenu.text = "Locked!";
            this.smallWidgetInfo.text = "To unlock this recipe, you need to collect " + _global._kitchen.buyButton.idx + " expert badges.\nCurrent Expert Badges: "
                + TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF];
            this.smallUniButton._text.text = "OK";
            this.smallWidgetBG.x = 512;
            this.smallWidgetBG.y = 300;
            this.smallWidgetMenu.x = 525;
            this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
            this.smallWidgetInfo.x = 525;
            this.smallWidgetInfo.y = this.smallWidgetBG.y + 10;
            this.smallUniButton.x = this.smallWidgetBG.x;
            this.smallUniButton.y = this.smallWidgetBG.y + 130;
            this.smallUniButton.idx = TNonStateButton.OK_BUTTON;
        }
        else if (arg1 == TWindowState.UNLOCK_FULL_VERSION)
        {
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
            this.smallWidgetBG.visible = true;
            this.smallWidgetMenu.visible = true;
            this.smallWidgetInfo.visible = true;
            this.smallButtonLeft.visible = true;
            this.smallButtonRight.visible = true;
            this.smallButtonLeft._text.text = "Cancel";
            this.smallButtonLeft._image.visible = true;
            this.smallButtonRight._image.visible = true;
            this.smallButtonLeft._image.texture = windowsAtlas.getTexture("small_btn_blue");
            this.smallButtonRight._image.texture = windowsAtlas.getTexture("small_btn_blue");
            this.smallButtonRight._text.text = "Unlock";
            this.smallWidgetMenu.text = "Unlock Full Version";
            this.smallWidgetInfo.text = "Get Premium access to all Extra minigames, remove Ads and support development for this game!";
            this.smallWidgetBG.x = 512;
            this.smallWidgetBG.y = 300;
            this.smallWidgetMenu.x = 525;
            this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
            this.smallWidgetInfo.x = 525;
            this.smallWidgetInfo.y = this.smallWidgetBG.y;
            this.smallButtonLeft.x = this.smallWidgetBG.x - 100;
            this.smallButtonLeft.y = this.smallWidgetBG.y + 130;
            this.smallButtonRight.x = this.smallWidgetBG.x + 100;
            this.smallButtonRight.y = this.smallWidgetBG.y + 130;
            this.smallButtonLeft.idx = TNonStateButton.CANCEL_BUTTON;
            this.smallButtonRight.idx = TNonStateButton.UNLOCK_BUTTON;
        }
        else if (arg1 == TWindowState.SHOW_FREE_COINS)
        {
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetMenu.visible = true;
            this.widgetInfo.visible = true;
            this.buttonLeft.visible = true;
            this.buttonRight.visible = true;
            this.buttonLeft._text.text = "OK!";
            this.buttonLeft._image.visible = true;
            this.buttonRight._image.visible = true;
            this.buttonLeft._image.texture = windowsAtlas.getTexture("small_btn_blue");
            this.buttonRight._image.texture = windowsAtlas.getTexture("small_btn_blue");
            this.buttonRight._text.text = "Cancel";
            this.widgetMenu.text = "Free Coins";
            this.widgetInfo.text = "Your Coins: " + _global._maxCrypto.STI(TSavegame.currentPlayer.money)
                + "\n\n\nIt's raining coins! Watch an interesting Ad and you'll get rewarded with 3 coins afterwards!\n(Please note that you must watch the whole video ad and there may not be any Ad available at all times)";
            this.widgetBG.x = 512;
            this.widgetBG.y = 300;
            this.widgetMenu.x = 525;
            this.widgetMenu.y = this.widgetBG.y - 200;
            this.widgetInfo.x = 525;
            this.widgetInfo.y = this.widgetBG.y - 20;
            this.buttonLeft.x = this.widgetBG.x - 100;
            this.buttonLeft.y = this.widgetBG.y + 160;
            this.buttonRight.x = this.widgetBG.x + 100;
            this.buttonRight.y = this.widgetBG.y + 160;
            this.buttonLeft.idx = TNonStateButton.OK_BUTTON;
            this.buttonRight.idx = TNonStateButton.CANCEL_BUTTON;
        }
        else if (arg1 == TWindowState.VIDEO_ADS_NOT_AVAILABLE)
        {
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
            this.smallWidgetBG.visible = true;
            this.smallUniButton.visible = true;
            this.smallWidgetMenu.visible = true;
            this.smallWidgetInfo.visible = true;
            this.smallWidgetMenu.text = "Free Coins";
            this.smallWidgetInfo.text = "Sorry, no video ad is available at this moment. Please try again in a moment.";
            this.smallUniButton._text.text = "OK";
            this.smallWidgetBG.x = 512;
            this.smallWidgetBG.y = 300;
            this.smallWidgetMenu.x = 525;
            this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
            this.smallWidgetInfo.x = 525;
            this.smallWidgetInfo.y = this.smallWidgetBG.y + 10;
            this.smallUniButton.x = this.smallWidgetBG.x;
            this.smallUniButton.y = this.smallWidgetBG.y + 130;
            this.smallUniButton.idx = TNonStateButton.OK_BUTTON;
        }
        else if (arg1 == TWindowState.COINS_REWARDED)
        {
            _blackScreen.visible = true;
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
            this.smallWidgetBG.visible = true;
            this.smallUniButton.visible = true;
            this.smallWidgetMenu.visible = true;
            this.smallWidgetInfo.visible = true;
            this.smallWidgetMenu.text = "Free Coins";
            this.smallWidgetInfo.text = "We have awarded you 3 coins.";
            this.smallUniButton._text.text = "OK";
            this.smallWidgetBG.x = 512;
            this.smallWidgetBG.y = 300;
            this.smallWidgetMenu.x = 525;
            this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
            this.smallWidgetInfo.x = 525;
            this.smallWidgetInfo.y = this.smallWidgetBG.y + 10;
            this.smallUniButton.x = this.smallWidgetBG.x;
            this.smallUniButton.y = this.smallWidgetBG.y + 130;
            this.smallUniButton.idx = TNonStateButton.OK_BUTTON;
        }
    }

    public void showGoalIconSmall()
    {
        int expertTarget = 0;
        this.goalIcon.visible = true;
        if (_global.currentSceneID == _global.GAMESCREEN_ID)
        {
            expertTarget = _global.gameScreen.expertTarget;
        }
        else if (_global.currentSceneID == _global.MAKE_DESSERT_ID)
        {
            expertTarget = _global._minigameDessert.expertTarget;
        }
        else
        {
            expertTarget = _global._minigameHO.expertTarget;
        }
        if (_global.currentSceneID == _global.GAMESCREEN_ID)
        {
            if (_global._maxCrypto.STI(ScoreManager.totalScore) >= expertTarget)
            {
                this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
                this.goalIcon.readjustSize();
            }
            else
            {
                this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal");
            }
        }
        else if (_global.currentSceneID == _global.MAKE_DESSERT_ID)
        {
            if (_global._maxCrypto.STI(_global._minigameDessert.earning) >= expertTarget)
            {
                this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
                this.goalIcon.readjustSize();
            }
            else
            {
                this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal");
            }
        }
        else if (_global.currentSceneID == _global.HIDDEN_OBJECT_ID)
        {
            if (_global._minigameHO._HOClass.itemFoundCount + 1 >= expertTarget)
            {
                this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
                this.goalIcon.readjustSize();
            }
            else
            {
                this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal");
            }
        }
        this.goalIcon.scaleY = 0.6f;
        this.goalIcon.scaleX = 0.6f;
        this.goalIcon.alpha = 0.5f;
        this.goalIcon.x = 363;
        this.goalIcon.y = 25;
        if (_global.currentSceneID == _global.HIDDEN_OBJECT_ID)
        {
            this.goalIcon.x = 90;
            this.goalIcon.y = 95;
        }
        else if (_global.currentSceneID == _global.MAKE_DESSERT_ID)
        {
            this.goalIcon.x = 810;
            this.goalIcon.y = 25;
        }
        TweenLite.to(this.goalIcon, 0.8f, new Vars
        {
            ["alpha"] = 1, ["scaleX"] = 0.35f, ["scaleY"] = 0.35f, ["ease"] = Back.easeIn,
            ["onComplete"] = (Action)(() =>
            {
                TweenLite.delayedCall(1, (Action)(() =>
                {
                    if (this.goalIcon.scaleX < 1)
                    {
                        this.goalIcon.visible = false;
                    }
                }));
            })
        });
    }

    public bool showGoalIcon()
    {
        bool expert = false;
        int expertTarget = 0;
        this.goalIcon.visible = true;
        if (_global.currentSceneID == _global.GAMESCREEN_ID)
        {
            expertTarget = _global.gameScreen.expertTarget;
        }
        else if (_global.currentSceneID == _global.MAKE_DESSERT_ID)
        {
            expertTarget = _global._minigameDessert.expertTarget;
        }
        else
        {
            expertTarget = _global._minigameHO.expertTarget;
        }
        if (_global.currentSceneID == _global.GAMESCREEN_ID && _global._maxCrypto.STI(ScoreManager.totalScore) >= expertTarget)
        {
            expert = true;
            _global._soundAsset.playSFX("expert_reached");
            this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
            this.goalIcon.readjustSize();
        }
        else if (_global.currentSceneID == _global.MAKE_DESSERT_ID && _global._maxCrypto.STI(_global._minigameDessert.earning) >= expertTarget)
        {
            expert = true;
            _global._soundAsset.playSFX("expert_reached");
            this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
            this.goalIcon.readjustSize();
        }
        else if (_global.currentSceneID == _global.HIDDEN_OBJECT_ID && _global._minigameHO._HOClass.totalItems == 0)
        {
            expert = true;
            _global._soundAsset.playSFX("expert_reached");
            this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
            this.goalIcon.readjustSize();
        }
        else
        {
            _global._soundAsset.playSFX("goal_reached");
            this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal");
        }
        this.goalIcon.scaleY = 2;
        this.goalIcon.scaleX = 2;
        this.goalIcon.alpha = 0.5f;
        this.goalIcon.x = 780;
        this.goalIcon.y = 500;
        this.expertBlink.x = this.goalIcon.x;
        this.expertBlink.y = this.goalIcon.y + 10;
        TweenLite.to(this.goalIcon, 0.8f, new Vars
        {
            ["alpha"] = 1, ["scaleX"] = 1, ["scaleY"] = 1, ["ease"] = Back.easeIn,
            ["onComplete"] = (Action<bool>)((bool f1_arg1) =>
            {
                if (f1_arg1 && this.expertBlink != null)
                {
                    this.expertBlink.visible = true;
                    this.tweenExpertBlink();
                }
            }),
            ["onCompleteParams"] = new object[] { expert }
        });
        return expert;
    }

    public void tweenExpertBlink()
    {
        TweenLite.to(this.expertBlink, 0.4f, new Vars
        {
            ["alpha"] = 0,
            ["onComplete"] = (Action)(() =>
            {
                TweenLite.to(this.expertBlink, 0.4f, new Vars { ["alpha"] = 1, ["onComplete"] = (Action)this.tweenExpertBlink });
            })
        });
    }

    public void showKitchenWindow(int arg1, int arg2)
    {
        XML ev = TGlobalClass.levelEventXML["day"][arg1]["kitchen"][arg2];
        _blackScreen.visible = true;
        if (ev.attribute("size").toString() == "big")
        {
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetInfo.visible = ev["info"].attribute("visible").toString() == "true";
            this.widgetMenu.visible = ev["title"].attribute("visible").toString() == "true";
            this.buttonLeft.visible = ev["leftButton"].attribute("visible").toString() == "true";
            this.buttonRight.visible = ev["rightButton"].attribute("visible").toString() == "true";
            this.uniButton.visible = ev["uniButton"].attribute("visible").toString() == "true";
            if (ev["texture"].toString() != "")
            {
                this.unlockedItem.visible = true;
                this.unlockedItem.texture = _global.gameScreen.gameAtlas.getTexture(ev["texture"].toString());
                this.unlockedItem.readjustSize();
                this.unlockedItem.pivotX = this.unlockedItem.width / 2;
                this.unlockedItem.pivotY = this.unlockedItem.height / 2;
            }
            else
            {
                this.unlockedItem.visible = false;
            }
            this.widgetInfo.text = ev["info"].toString();
            this.widgetMenu.text = ev["title"].toString();
            this.buttonLeft._text.text = ev["leftButton"].toString();
            this.buttonRight._text.text = ev["rightButton"].toString();
            this.uniButton._text.text = ev["uniButton"].toString();
            this.widgetInfo.x = this.widgetBG.x + ev["info"].attribute("offsetX").toInt();
            this.widgetMenu.x = this.widgetBG.x + ev["title"].attribute("offsetX").toInt();
            this.buttonLeft.x = this.widgetBG.x + ev["leftButton"].attribute("offsetX").toInt();
            this.buttonRight.x = this.widgetBG.x + ev["rightButton"].attribute("offsetX").toInt();
            this.uniButton.x = this.widgetBG.x + ev["uniButton"].attribute("offsetX").toInt();
            this.widgetInfo.y = this.widgetBG.y + ev["info"].attribute("offsetY").toInt();
            this.widgetMenu.y = this.widgetBG.y + ev["title"].attribute("offsetY").toInt();
            this.buttonLeft.y = this.widgetBG.y + ev["leftButton"].attribute("offsetY").toInt();
            this.buttonRight.y = this.widgetBG.y + ev["rightButton"].attribute("offsetY").toInt();
            this.uniButton.y = this.widgetBG.y + ev["uniButton"].attribute("offsetY").toInt();
            this.uniButton.idx = TNonStateButton.NEXT_WINDOW;
        }
        else if (ev.attribute("size").toString() == "small")
        {
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
            this.smallWidgetBG.visible = true;
            this.smallWidgetInfo.visible = ev["info"].attribute("visible").toString() == "true";
            this.smallWidgetMenu.visible = ev["title"].attribute("visible").toString() == "true";
            this.smallButtonLeft.visible = ev["leftButton"].attribute("visible").toString() == "true";
            this.smallButtonRight.visible = ev["rightButton"].attribute("visible").toString() == "true";
            this.smallUniButton.visible = ev["uniButton"].attribute("visible").toString() == "true";
            if (ev["texture"].toString() != "")
            {
                this.unlockedItem.visible = true;
                this.unlockedItem.texture = _global.gameScreen.gameAtlas.getTexture(ev["texture"].toString());
                this.unlockedItem.readjustSize();
                this.unlockedItem.pivotX = this.unlockedItem.width / 2;
                this.unlockedItem.pivotY = this.unlockedItem.height / 2;
            }
            else
            {
                this.unlockedItem.visible = false;
            }
            this.smallWidgetInfo.text = ev["info"].toString();
            this.smallWidgetMenu.text = ev["title"].toString();
            this.smallButtonLeft._text.text = ev["leftButton"].toString();
            this.smallButtonRight._text.text = ev["rightButton"].toString();
            this.smallUniButton._text.text = ev["uniButton"].toString();
            this.smallWidgetInfo.x = this.smallWidgetBG.x + ev["info"].attribute("offsetX").toInt();
            this.smallWidgetMenu.x = this.smallWidgetBG.x + ev["title"].attribute("offsetX").toInt();
            this.smallButtonLeft.x = this.smallWidgetBG.x + ev["leftButton"].attribute("offsetX").toInt();
            this.smallButtonRight.x = this.smallWidgetBG.x + ev["rightButton"].attribute("offsetX").toInt();
            this.smallUniButton.x = this.smallWidgetBG.x + ev["uniButton"].attribute("offsetX").toInt();
            this.smallWidgetInfo.y = this.smallWidgetBG.y + ev["info"].attribute("offsetY").toInt();
            this.smallWidgetMenu.y = this.smallWidgetBG.y + ev["title"].attribute("offsetY").toInt();
            this.smallButtonLeft.y = this.smallWidgetBG.y + ev["leftButton"].attribute("offsetY").toInt();
            this.smallButtonRight.y = this.smallWidgetBG.y + ev["rightButton"].attribute("offsetY").toInt();
            this.smallUniButton.y = this.smallWidgetBG.y + ev["uniButton"].attribute("offsetY").toInt();
            this.smallUniButton.idx = TNonStateButton.NEXT_WINDOW;
        }
    }

    public void showBeginningWindow(int arg1, int arg2)
    {
        XML ev = TGlobalClass.levelEventXML["day"][arg1]["window"][arg2];
        _blackScreen.visible = true;
        if (ev.attribute("size").toString() == "big")
        {
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
            this.widgetBG.visible = true;
            this.widgetBG.scaleY = 0.7f;
            this.widgetInfo.visible = ev["info"].attribute("visible").toString() == "true";
            this.widgetMenu.visible = ev["title"].attribute("visible").toString() == "true";
            this.buttonLeft.visible = ev["leftButton"].attribute("visible").toString() == "true";
            this.buttonRight.visible = ev["rightButton"].attribute("visible").toString() == "true";
            this.uniButton.visible = ev["uniButton"].attribute("visible").toString() == "true";
            if (ev["texture"].toString() != "")
            {
                this.unlockedItem.visible = true;
                this.unlockedItem.texture = _global.gameScreen.gameAtlas.getTexture(ev["texture"].toString());
                this.unlockedItem.readjustSize();
                this.unlockedItem.pivotX = this.unlockedItem.width / 2;
                this.unlockedItem.pivotY = this.unlockedItem.height / 2;
                this.unlockedItem.scaleX = 1.2f;
                this.unlockedItem.scaleY = 1.2f;
                if (ev["texture"].toString().indexOf("char_") > -1 && TGlobalClass.currentPlayedLevel > 0)
                {
                    List<string> parts = ev["texture"].toString().split("_");
                    int custIdx = AS3.toInt(parts[parts.Count - 1]);
                    if (custIdx < 13)
                    {
                        TSavegame.currentPlayer.lastCustIdx = _global.gameScreen.customerInUse.indexOf(custIdx);
                        TSavegame.save();
                        TGlobalClass.tempCustUse = _global.gameScreen.customerInUse.slice(0, TSavegame.currentPlayer.lastCustIdx + 1);
                    }
                }
            }
            else
            {
                this.unlockedItem.visible = false;
            }
            this.widgetInfo.text = ev["info"].toString();
            this.widgetMenu.text = ev["title"].toString();
            this.buttonLeft._text.text = ev["leftButton"].toString();
            this.buttonRight._text.text = ev["rightButton"].toString();
            this.uniButton._text.text = ev["uniButton"].toString();
            this.widgetInfo.x = this.widgetBG.x + ev["info"].attribute("offsetX").toInt();
            this.widgetMenu.x = this.widgetBG.x + ev["title"].attribute("offsetX").toInt();
            this.buttonLeft.x = this.widgetBG.x + ev["leftButton"].attribute("offsetX").toInt();
            this.buttonRight.x = this.widgetBG.x + ev["rightButton"].attribute("offsetX").toInt();
            this.uniButton.x = this.widgetBG.x + ev["uniButton"].attribute("offsetX").toInt();
            this.widgetInfo.y = this.widgetBG.y + ev["info"].attribute("offsetY").toInt();
            this.widgetMenu.y = this.widgetBG.y + ev["title"].attribute("offsetY").toInt();
            this.buttonLeft.y = this.widgetBG.y + ev["leftButton"].attribute("offsetY").toInt();
            this.buttonRight.y = this.widgetBG.y + ev["rightButton"].attribute("offsetY").toInt();
            this.uniButton.y = this.widgetBG.y + ev["uniButton"].attribute("offsetY").toInt();
            string custLabel = " customer(s)";
            if (TGlobalClass.isPlayingSpecialLevel)
            {
                custLabel = " food critic";
            }
            if (this.widgetMenu.text == "General Tips")
            {
                this.widgetMenu.text = "Today's Preparation";
                this.widgetInfo.text = "Goal : serve " + _global.gameScreen.goalTarget + custLabel + "\nExpert : earn $" + _global.gameScreen.expertTarget;
                var shown = new List<int>();
                int dishCount = 0;
                int n = 0;
                int k;
                if (TGlobalClass.currentRegionDish == ConstClass.WESTERN_DISH)
                {
                    dishCount = TGlobalClass.dishRegionWestern.Count;
                }
                else if (TGlobalClass.currentRegionDish == ConstClass.CHINESE_DISH)
                {
                    dishCount = TGlobalClass.dishRegionChinese.Count;
                }
                else if (TGlobalClass.currentRegionDish == ConstClass.MID_EAST_DISH)
                {
                    dishCount = TGlobalClass.dishRegionMidEast.Count;
                }
                for (k = 0; k < dishCount; k++)
                {
                    if (TGlobalClass.currentRegionDish == ConstClass.WESTERN_DISH)
                    {
                        if (shown.indexOf(TGlobalClass.dishRegionWestern[k]) > -1) continue;
                        this.dishPreviewAr[n].texture = _global.gameScreen.gameAtlas.getTexture(_global.gameScreen.westernDishName[TGlobalClass.dishRegionWestern[k]]);
                        shown.push(TGlobalClass.dishRegionWestern[k]);
                    }
                    else if (TGlobalClass.currentRegionDish == ConstClass.CHINESE_DISH)
                    {
                        if (shown.indexOf(TGlobalClass.dishRegionChinese[k]) > -1) continue;
                        this.dishPreviewAr[n].texture = _global.gameScreen.gameAtlas.getTexture(_global.gameScreen.chineseDishName[TGlobalClass.dishRegionChinese[k] - TGlobalClass.TOTAL_WESTERN_DISH]);
                        shown.push(TGlobalClass.dishRegionChinese[k]);
                    }
                    else if (TGlobalClass.currentRegionDish == ConstClass.MID_EAST_DISH)
                    {
                        if (shown.indexOf(TGlobalClass.dishRegionMidEast[k]) > -1) continue;
                        this.dishPreviewAr[n].texture = _global.gameScreen.gameAtlas.getTexture(_global.gameScreen.mideastDishName[TGlobalClass.dishRegionMidEast[k] - TGlobalClass.TOTAL_WESTERN_DISH - TGlobalClass.TOTAL_CHINESE_DISH]);
                        shown.push(TGlobalClass.dishRegionMidEast[k]);
                    }
                    this.dishPreviewAr[n].readjustSize();
                    this.dishPreviewAr[n].scaleX = 1.2f;
                    this.dishPreviewAr[n].scaleY = 1.2f;
                    this.dishPreviewAr[n].pivotY = this.dishPreviewAr[n].height / this.dishPreviewAr[n].scaleY;
                    this.dishPreviewAr[n].visible = true;
                    n++;
                }
                for (k = 0; k < n; k++)
                {
                    if (n % 2 == 0)
                    {
                        this.dishPreviewAr[k].x = 512 + (k - (float)n / 2) * (this.dishPreviewAr[k].width + 10);
                    }
                    else
                    {
                        this.dishPreviewAr[k].x = 512 + (k - Mathf.Floor((float)n / 2)) * (this.dishPreviewAr[k].width + 10) - this.dishPreviewAr[k].width / 2;
                    }
                }
            }
            this.uniButton.idx = TNonStateButton.NEXT_WINDOW;
        }
        else if (ev.attribute("size").toString() == "small")
        {
            _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
            this.smallWidgetBG.visible = true;
            this.smallWidgetInfo.visible = ev["info"].attribute("visible").toString() == "true";
            this.smallWidgetMenu.visible = ev["title"].attribute("visible").toString() == "true";
            this.smallButtonLeft.visible = ev["leftButton"].attribute("visible").toString() == "true";
            this.smallButtonRight.visible = ev["rightButton"].attribute("visible").toString() == "true";
            this.smallUniButton.visible = ev["uniButton"].attribute("visible").toString() == "true";
            if (ev["texture"].toString() != "")
            {
                this.unlockedItem.visible = true;
                this.unlockedItem.texture = _global.gameScreen.gameAtlas.getTexture(ev["texture"].toString());
                this.unlockedItem.readjustSize();
                this.unlockedItem.pivotX = this.unlockedItem.width / 2;
                this.unlockedItem.pivotY = this.unlockedItem.height / 2;
            }
            else
            {
                this.unlockedItem.visible = false;
            }
            this.smallWidgetInfo.text = ev["info"].toString();
            this.smallWidgetMenu.text = ev["title"].toString();
            this.smallButtonLeft._text.text = ev["leftButton"].toString();
            this.smallButtonRight._text.text = ev["rightButton"].toString();
            this.smallUniButton._text.text = ev["uniButton"].toString();
            this.smallWidgetInfo.x = this.smallWidgetBG.x + ev["info"].attribute("offsetX").toInt();
            this.smallWidgetMenu.x = this.smallWidgetBG.x + ev["title"].attribute("offsetX").toInt();
            this.smallButtonLeft.x = this.smallWidgetBG.x + ev["leftButton"].attribute("offsetX").toInt();
            this.smallButtonRight.x = this.smallWidgetBG.x + ev["rightButton"].attribute("offsetX").toInt();
            this.smallUniButton.x = this.smallWidgetBG.x + ev["uniButton"].attribute("offsetX").toInt();
            this.smallWidgetInfo.y = this.smallWidgetBG.y + ev["info"].attribute("offsetY").toInt();
            this.smallWidgetMenu.y = this.smallWidgetBG.y + ev["title"].attribute("offsetY").toInt();
            this.smallButtonLeft.y = this.smallWidgetBG.y + ev["leftButton"].attribute("offsetY").toInt();
            this.smallButtonRight.y = this.smallWidgetBG.y + ev["rightButton"].attribute("offsetY").toInt();
            this.smallUniButton.y = this.smallWidgetBG.y + ev["uniButton"].attribute("offsetY").toInt();
            this.smallUniButton.idx = TNonStateButton.NEXT_WINDOW;
        }
    }
}
