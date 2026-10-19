// Ported from LevelSelectScreen.as (decompiled).
using System;
using System.Collections.Generic;
using Starlite;
using Mathf = UnityEngine.Mathf;

public class LevelSelectScreen : Screen
{
    public readonly int PLAY_BUTTON = 1;
    public readonly int BACK_BUTTON = 2;
    public readonly int RIGHT_BUTTON = 3;
    public readonly int LEFT_BUTTON = 4;
    public Global _global;
    public bool initVector;
    public TextureAtlas levelSelectAtlas;
    public ExtendTextField mondayQuad;
    public ExtendTextField tuesdayQuad;
    public ExtendTextField wednesdayQuad;
    public ExtendTextField thursdayQuad;
    public ExtendTextField fridayQuad;
    public ExtendTextField saturdayQuad;
    public ExtendTextField sundayQuad;
    public int currentPage;
    public int selectedDays;
    public List<DisplayObject> buttonAr;
    public List<ExtendTextField> daysAr;
    public cSprite _background;
    public TNonStateButton playButton;
    public TNonStateButton backButton;
    public TNonStateButton leftButton;
    public TNonStateButton rightButton;
    public ExtendTextField noteTitle;
    public ExtendTextField noteDescription;
    public ExtendTextField noteDescription2;
    public cSprite checkMark1;
    public cSprite checkMark2;
    public List<cSprite> badgeIcon;
    public cSprite badgeIconBig;
    public List<int> badgePosX;
    public List<int> badgePosY;

    public LevelSelectScreen()
    {
        this.badgePosX = new List<int> { 150, 386, 573, 188, 445, 651, 251 };
        this.badgePosY = new List<int> { 174, 121, 150, 332, 290, 307, 529 };
    }

    public override void Start()
    {
        this._global.currentSceneID = this._global.LEVEL_SELECT_ID;
        if (!this.initVector)
        {
            this.buttonAr = new List<DisplayObject>();
            this.daysAr = new List<ExtendTextField>();
            this.badgeIcon = new List<cSprite>();
            this.initVector = true;
        }
        this._global.assetManager.enqueue(this._global.graphicsPath + "level_select.ean", this._global.graphicsPath + "level_select.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "level_select_BG.eag");
        this._global.assetManager.loadQueue((float f1_arg1) =>
        {
            if (f1_arg1 == 1) this.initialize();
        });
    }

    public override void Update()
    {
        if (!ready) return;
    }

    public override void Kill()
    {
        int _local1 = 0;
        int _local2 = this.buttonAr.Count;
        for (_local1 = 0; _local1 < _local2; _local1++)
        {
            this.buttonAr[_local1].removeEventListener(TouchEvent.TOUCH, this.touchButton);
            this.buttonAr[_local1].removeFromParent(true);
            this.buttonAr[_local1] = null;
        }
        this.buttonAr.Clear();
        for (_local1 = 0; _local1 < this.daysAr.Count; _local1++)
        {
            this.daysAr[_local1].removeEventListener(TouchEvent.TOUCH, this.selectDay);
            this.daysAr[_local1].removeFromParent(true);
            this.daysAr[_local1] = null;
        }
        this.daysAr.Clear();
        for (_local1 = 0; _local1 < this.badgeIcon.Count; _local1++)
        {
            this.badgeIcon[_local1].removeFromParent(true);
            this.badgeIcon[_local1] = null;
        }
        this.badgeIcon.Clear();
        this._background.removeFromParent(true);
        this._background = null;
        this.playButton.removeFromParent(true);
        this.playButton = null;
        this.backButton.removeFromParent(true);
        this.backButton = null;
        this.rightButton.removeFromParent(true);
        this.rightButton = null;
        this.leftButton.removeFromParent(true);
        this.leftButton = null;
        this.noteDescription.removeFromParent(true);
        this.noteDescription = null;
        this.noteDescription2.removeFromParent(true);
        this.noteDescription2 = null;
        this.mondayQuad.removeFromParent(true);
        this.mondayQuad = null;
        this.tuesdayQuad.removeFromParent(true);
        this.tuesdayQuad = null;
        this.wednesdayQuad.removeFromParent(true);
        this.wednesdayQuad = null;
        this.thursdayQuad.removeFromParent(true);
        this.thursdayQuad = null;
        this.fridayQuad.removeFromParent(true);
        this.fridayQuad = null;
        this.saturdayQuad.removeFromParent(true);
        this.saturdayQuad = null;
        this.sundayQuad.removeFromParent(true);
        this.sundayQuad = null;
        this.checkMark1.removeFromParent(true);
        this.checkMark1 = null;
        this.checkMark2.removeFromParent(true);
        this.checkMark2 = null;
        this.badgeIconBig.removeFromParent(true);
        this.badgeIconBig = null;
        this.levelSelectAtlas.dispose();
        this.levelSelectAtlas = null;
        this._global.assetManager.purge();
        ready = false;
    }

    public void initialize()
    {
        int _local2 = 0;
        cSprite _local3 = null;
        this._global.ReportGameState(this._global.GAMESTATE_LEVEL_SELECT);
        TGlobalClass.isPlayingSpecialLevel = false;
        this.levelSelectAtlas = this._global.assetManager.getTextureAtlas("level_select");
        this._background = new cSprite(this._global.assetManager.getTexture("level_select_BG"));
        this._background.blendMode = BlendMode.NONE;
        this._background.touchable = false;
        this._global._gameS.addChild(this._background);
        float _local1 = Mathf.PI / 180;
        this.mondayQuad = new ExtendTextField(200, 150, "Day\n1", "bromine", 40, 0, true);
        this.mondayQuad.x = 80;
        this.mondayQuad.y = 70;
        this.mondayQuad.rotation = -20 * _local1;
        this._global._gameS.addChild(this.mondayQuad);
        this.daysAr.Add(this.mondayQuad);
        this.tuesdayQuad = new ExtendTextField(170, 110, "Day\n2", "bromine", 40, 0, true);
        this.tuesdayQuad.x = 340;
        this.tuesdayQuad.y = 40;
        this.tuesdayQuad.rotation = -6 * _local1;
        this.tuesdayQuad.skewY = 0.35f;
        this.tuesdayQuad.skewX = -0.2f;
        this._global._gameS.addChild(this.tuesdayQuad);
        this.daysAr.Add(this.tuesdayQuad);
        this.wednesdayQuad = new ExtendTextField(150, 100, "Day\n3", "bromine", 35, 0, true);
        this.wednesdayQuad.x = 540;
        this.wednesdayQuad.y = 97;
        this.wednesdayQuad.rotation = -10 * _local1;
        this.wednesdayQuad.skewX = -0.17f;
        this._global._gameS.addChild(this.wednesdayQuad);
        this.daysAr.Add(this.wednesdayQuad);
        this.thursdayQuad = new ExtendTextField(200, 150, "Day\n4", "bromine", 40, 0, true);
        this.thursdayQuad.x = 130;
        this.thursdayQuad.y = 240;
        this.thursdayQuad.rotation = -20 * _local1;
        this._global._gameS.addChild(this.thursdayQuad);
        this.daysAr.Add(this.thursdayQuad);
        this.fridayQuad = new ExtendTextField(180, 110, "Day\n5", "bromine", 40, 0, true);
        this.fridayQuad.x = 390;
        this.fridayQuad.y = 200;
        this.fridayQuad.rotation = -6 * _local1;
        this.fridayQuad.skewY = 0.29f;
        this.fridayQuad.skewX = -0.2f;
        this._global._gameS.addChild(this.fridayQuad);
        this.daysAr.Add(this.fridayQuad);
        this.saturdayQuad = new ExtendTextField(160, 120, "Day\n6", "bromine", 40, 0, true);
        this.saturdayQuad.x = 590;
        this.saturdayQuad.y = 230;
        this.saturdayQuad.rotation = -11 * _local1;
        this.saturdayQuad.skewX = -0.17f;
        this._global._gameS.addChild(this.saturdayQuad);
        this.daysAr.Add(this.saturdayQuad);
        this.sundayQuad = new ExtendTextField(200, 160, "Day\n7", "bromine", 40, 0, true);
        this.sundayQuad.x = 200;
        this.sundayQuad.y = 420;
        this.sundayQuad.rotation = -15 * _local1;
        this._global._gameS.addChild(this.sundayQuad);
        this.daysAr.Add(this.sundayQuad);
        for (_local2 = 0; _local2 < this.daysAr.Count; _local2++)
        {
            _local3 = new cSprite(TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal"));
            _local3.alignPivot();
            _local3.scaleY = 0.3f;
            _local3.scaleX = 0.3f;
            _local3.x = this.badgePosX[_local2];
            _local3.y = this.badgePosY[_local2];
            _local3.touchable = false;
            this._global._gameS.addChild(_local3);
            this.badgeIcon.Add(_local3);
        }
        this.badgeIconBig = new cSprite(TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal"));
        this.badgeIconBig.x = 670;
        this.badgeIconBig.y = 380;
        this.badgeIconBig.touchable = false;
        this.badgeIconBig.scaleY = 0.75f;
        this.badgeIconBig.scaleX = 0.75f;
        this._global._gameS.addChild(this.badgeIconBig);
        this.noteDescription = new ExtendTextField(350, 150, "", "jenna_sue", 30);
        this.noteDescription.hAlign = "left";
        this.noteDescription.vAlign = "top";
        this.noteDescription.touchable = false;
        this.noteDescription.x = 430;
        this.noteDescription.y = 370;
        this.noteDescription.rotation = -5 * Mathf.PI / 180;
        this.noteDescription.skewY = 0.18f;
        this.noteDescription.skewX = -0.3f;
        this._global._gameS.addChild(this.noteDescription);
        this.noteDescription2 = new ExtendTextField(150, 150, "", "jenna_sue", 30);
        this.noteDescription2.hAlign = "left";
        this.noteDescription2.vAlign = "top";
        this.noteDescription2.touchable = false;
        this.noteDescription2.x = 683;
        this.noteDescription2.y = 393;
        this.noteDescription2.rotation = -5 * Mathf.PI / 180;
        this.noteDescription2.skewY = -0.1f;
        this.noteDescription2.skewX = -0.3f;
        this._global._gameS.addChild(this.noteDescription2);
        this.checkMark1 = new cSprite(TGlobalClass._windowDisplay.windowsAtlas.getTexture("check_mark"));
        this.checkMark1.x = 635;
        this.checkMark1.y = 413;
        this.checkMark1.scaleY = 0.5f;
        this.checkMark1.scaleX = 0.5f;
        this._global._gameS.addChild(this.checkMark1);
        this.checkMark2 = new cSprite(TGlobalClass._windowDisplay.windowsAtlas.getTexture("check_mark"));
        this.checkMark2.x = 622;
        this.checkMark2.y = 446;
        this.checkMark2.scaleY = 0.5f;
        this.checkMark2.scaleX = 0.5f;
        this._global._gameS.addChild(this.checkMark2);
        this.playButton = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_green"), "PLAY", "bromine", 30, 16777215);
        this.playButton.pivotX = this.playButton.width / 2;
        this.playButton.pivotY = this.playButton.height / 2;
        this.playButton._text.touchable = false;
        this.playButton.x = 512;
        this.playButton.y = 550;
        this.playButton._image.name = AS3.str(this.PLAY_BUTTON);
        this._global._gameS.addChild(this.playButton);
        this.buttonAr.Add(this.playButton._image);
        this.backButton = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_blue"), "Back", "bromine", 30, 16777215);
        this.backButton.pivotX = this.backButton.width / 2;
        this.backButton.pivotY = this.backButton.height / 2;
        this.backButton._text.touchable = false;
        this.backButton.x = 880;
        this.backButton.y = 50;
        this.backButton._image.name = AS3.str(this.BACK_BUTTON);
        this._global._gameS.addChild(this.backButton);
        this.buttonAr.Add(this.backButton._image);
        this.rightButton = new TNonStateButton(this.levelSelectAtlas.getTexture("arrow_button"));
        this.rightButton.pivotX = this.rightButton.width / 2;
        this.rightButton.pivotY = this.rightButton.height / 2;
        this.rightButton.x = 920;
        this.rightButton.y = 400;
        this.rightButton.name = AS3.str(this.RIGHT_BUTTON);
        this._global._gameS.addChild(this.rightButton);
        this.buttonAr.Add(this.rightButton);
        this.leftButton = new TNonStateButton(this.levelSelectAtlas.getTexture("arrow_button"));
        this.leftButton.pivotX = this.leftButton.width / 2;
        this.leftButton.pivotY = this.leftButton.height / 2;
        this.leftButton.x = 80;
        this.leftButton.y = 400;
        this.leftButton.scaleX = -1;
        this.leftButton.name = AS3.str(this.LEFT_BUTTON);
        this._global._gameS.addChild(this.leftButton);
        this.buttonAr.Add(this.leftButton);
        for (_local2 = 0; _local2 < this.buttonAr.Count; _local2++)
        {
            this.buttonAr[_local2].addEventListener(TouchEvent.TOUCH, this.touchButton);
        }
        for (_local2 = 0; _local2 < this.daysAr.Count; _local2++)
        {
            this.daysAr[_local2].useHandCursor = true;
            this.daysAr[_local2].addEventListener(TouchEvent.TOUCH, this.selectDay);
        }
        TGlobalClass.currentPlayedLevel = TSavegame.currentPlayer.passedDays;
        if (!(TGlobalClass.currentPlayedLevel == TGlobalClass.TOTAL_DAYS)
            && TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] == true)
        {
            TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = false;
        }
        this.currentPage = (int)Mathf.Floor((float)TSavegame.currentPlayer.passedDays / 7);
        if (this.currentPage >= (float)TGlobalClass.TOTAL_DAYS / 7)
        {
            this.currentPage = (int)((float)TGlobalClass.TOTAL_DAYS / 7 - 1);
        }
        this.setAvailableDays();
        ready = true;
    }

    public void setAvailableDays()
    {
        int _local1 = 0;
        int _local2 = 0;
        for (_local1 = 0; _local1 < 7; _local1++)
        {
            _local2 = this.currentPage * 7 + _local1;
            if (_local2 > TSavegame.currentPlayer.passedDays)
            {
                this.daysAr[_local1].text = "";
                this.daysAr[_local1].touchable = false;
                this.badgeIcon[_local1].visible = false;
            }
            else
            {
                this.daysAr[_local1].text = "Day\n" + (_local2 + 1);
                this.daysAr[_local1].name = "" + _local2;
                this.daysAr[_local1].touchable = true;
                this.selectedDays = _local2;
                if (TSavegame.currentPlayer.goalAchieved[_local2])
                {
                    if (TSavegame.currentPlayer.daysEarning[_local2] >= TGlobalClass.levelSetXML["day"][_local2]["expert"].toInt())
                    {
                        this.badgeIcon[_local1].texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
                    }
                    else
                    {
                        this.badgeIcon[_local1].texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal");
                    }
                    this.badgeIcon[_local1].visible = true;
                }
                else
                {
                    this.badgeIcon[_local1].visible = false;
                }
            }
        }
        float passedPages = Mathf.Floor((float)TSavegame.currentPlayer.passedDays / 7);
        if (this.currentPage >= passedPages) this.rightButton.visible = false;
        else this.rightButton.visible = true;
        if (this.currentPage <= 0) this.leftButton.visible = false;
        else this.leftButton.visible = true;
        if (TSavegame.currentPlayer.passedDays == TGlobalClass.TOTAL_DAYS
            && this.currentPage + 1 >= Mathf.Floor((float)TSavegame.currentPlayer.passedDays / 7))
        {
            this.rightButton.visible = false;
        }
        if (this.selectedDays > TGlobalClass.TOTAL_DAYS - 1)
        {
            this.selectedDays = TGlobalClass.TOTAL_DAYS - 1;
        }
        if (this.selectedDays < (int)((float)TSavegame.currentPlayer.passedDays / 7) * 7)
        {
            this.selectedDays = this.selectedDays - 6;
        }
        if (TSavegame.currentPlayer.passedDays == TGlobalClass.TOTAL_DAYS
            && this.currentPage == (float)TGlobalClass.TOTAL_DAYS / 7 - 1)
        {
            this.selectedDays = TGlobalClass.TOTAL_DAYS - 1;
        }
        this.daysAr[this.selectedDays % 7].color = 16711680;
        this.showDescription();
    }

    public void selectDay(Event arg1)
    {
        var e = (TouchEvent)arg1;
        int _local2 = 0;
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) == null) return;
        _local2 = AS3.toInt((e.currentTarget as ExtendTextField).name);
        this.daysAr[this.selectedDays % 7].color = 0;
        this.selectedDays = _local2;
        this.daysAr[this.selectedDays % 7].color = 16711680;
        this.showDescription();
        this.checkMark2.visible = false;
        this.checkMark1.visible = false;
        if (TSavegame.currentPlayer.goalAchieved[_local2])
        {
            if (TSavegame.currentPlayer.daysEarning[_local2] >= TGlobalClass.levelSetXML["day"][_local2]["expert"].toInt())
            {
                this.checkMark2.visible = true;
            }
            this.checkMark1.visible = true;
        }
    }

    public void showDescription()
    {
        string _local1 = null;
        string _local2 = null;
        this.checkMark2.visible = false;
        this.checkMark1.visible = false;
        this.badgeIconBig.visible = false;
        if (TSavegame.currentPlayer.goalAchieved[this.selectedDays])
        {
            if (TSavegame.currentPlayer.daysEarning[this.selectedDays] >= TGlobalClass.levelSetXML["day"][this.selectedDays]["expert"].toInt())
            {
                this.checkMark2.visible = true;
            }
            this.checkMark1.visible = true;
            if (TSavegame.currentPlayer.daysEarning[this.selectedDays] >= TGlobalClass.levelSetXML["day"][this.selectedDays]["expert"].toInt())
            {
                this.badgeIconBig.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
            }
            else
            {
                this.badgeIconBig.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal");
            }
            this.badgeIconBig.visible = true;
        }
        XML _local3 = TGlobalClass.levelSetXML["day"][this.selectedDays];
        string _local4 = _local3["goal"];
        string _local5 = _local3["expert"];
        switch (_local3["region"].toString())
        {
            case "W": _local1 = "Western Cuisine"; break;
            case "C": _local1 = "Asian Cuisine"; break;
            default: _local1 = "Middle East Cuisine"; break;
        }
        switch (_local3["gameType"].toString())
        {
            case "HO":
                _local2 = "Hidden Objects";
                _local1 = "Cleaning up";
                _local4 = "find " + _local4 + " items";
                _local5 = _local5 + " items";
                break;
            case "DM":
                _local2 = "Minigame";
                _local1 = "Froyo Business";
                _local4 = "earn $" + _local4;
                _local5 = "earn $" + _local5;
                break;
            default:
                _local2 = "Match 3";
                _local4 = "serve " + _local4 + ((this.selectedDays + 1) % 7 == 0 ? " food critic" : " customer(s)");
                _local5 = "earn $ " + _local5;
                break;
        }
        if (_local4 == "")
        {
            _local4 = AS3.str((int)(this._global._minigameHO.totalItemPerLevel[(int)((float)this.selectedDays / 14)] * 0.75f));
        }
        if (_local5 == "")
        {
            _local5 = AS3.str(this._global._minigameHO.totalItemPerLevel[(int)((float)this.selectedDays / 14)]);
        }
        this.noteDescription.text = _local1 + " (" + _local2 + ")\nGoal: " + _local4 + "\nExpert: " + _local5
            + "\nYour Best Score: " + TSavegame.currentPlayer.daysEarning[this.selectedDays];
    }

    public void touchButton(Event arg1)
    {
        var e = (TouchEvent)arg1;
        int _local2 = 0;
        string _local3 = null;
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) == null) return;
        _local2 = AS3.toInt((e.currentTarget as DisplayObject).name);
        if (_local2 == this.PLAY_BUTTON)
        {
            _local3 = TGlobalClass.levelSetXML["day"][this.selectedDays]["gameType"];
            TGlobalClass.currentRegionDish = TGlobalClass.levelSetXML["day"][this.selectedDays]["region"].toString();
            if (_local3 == ConstClass.GAME_TYPE_M3)
            {
                this._global._game.fadeToScreen(this._global.gameScreen);
            }
            else if (_local3 == ConstClass.GAME_TYPE_HO)
            {
                this._global._game.fadeToScreen(this._global._minigameHO);
            }
            else if (_local3 == ConstClass.GAME_TYPE_SPECIAL_M3)
            {
                TGlobalClass.isPlayingSpecialLevel = true;
                this._global._game.fadeToScreen(this._global.gameScreen);
            }
            else if (_local3 == ConstClass.GAME_TYPE_DESSERT_MAKING)
            {
                this._global._game.fadeToScreen(this._global._minigameDessert);
            }
            TGlobalClass.currentPlayedLevel = this.selectedDays;
        }
        else if (_local2 == this.BACK_BUTTON)
        {
            this._global._game.fadeToScreen(this._global._kitchen);
        }
        else if (_local2 == this.RIGHT_BUTTON)
        {
            this.daysAr[this.selectedDays % 7].color = 0 + 0;
            this.currentPage = this.currentPage + 1;
            this.setAvailableDays();
        }
        else if (_local2 == this.LEFT_BUTTON)
        {
            this.daysAr[this.selectedDays % 7].color = 0;
            this.currentPage = this.currentPage - 1;
            this.setAvailableDays();
        }
    }
}
