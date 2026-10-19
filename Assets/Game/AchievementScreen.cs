// Ported from AchievementScreen.as (decompiled). Scrollable list of the 10 achievement badges.
using System;
using System.Collections.Generic;
using Starlite;

public class AchievementScreen : Screen
{
    public const int BAR = 1;
    public const int SCROLLER = 2;
    public Global _global;
    public bool _bolehKlik;
    public bool initVector;
    public cSprite _background;
    public List<string> achievementName;
    public List<string> achievementNameLocked;
    public TextureAtlas achievementAtlas;
    public Sprite innerSprite;
    public Sprite outerSprite;
    public ExtendTextField achievementTitle;
    public ExtendTextField achievementInfo;
    public List<cSprite> achievementAr;
    public cSprite achievement;
    public Quad positionQuad;
    public List<Quad> positionQuadAr;
    public cSprite bar;
    public cSprite scroller;
    public cSprite backButton;
    public float barHeight;
    public float innerSpriteHeight;
    public bool backToMainMenu;

    public AchievementScreen()
    {
        this.achievementName = new List<string>
        {
            "service_exellence", "big_business", "western_recipe", "chinese_recipe", "middle_east_recipe",
            "secret_ingredient", "hawkeye", "froyo_master", "qualified_chef", "international_chef",
        };
        this.achievementNameLocked = new List<string>
        {
            "service_exellence", "big_business", "middle_east_recipe", "middle_east_recipe", "middle_east_recipe",
            "secret_ingredient", "hawkeye", "froyo_master", "qualified_chef", "international_chef",
        };
    }

    public override void Start()
    {
        base.Start();
        this._global.currentSceneID = this._global.ACHIEVEMENTS_ID;
        if (!this.initVector)
        {
            this.achievementAr = new List<cSprite>();
            this.positionQuadAr = new List<Quad>();
            this.initVector = true;
        }
        this._global.assetManager.enqueue(this._global.graphicsPath + "achievement.ean", this._global.graphicsPath + "achievement.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "achievement_BG.eag");
        this._global.assetManager.loadQueue((float ratio) =>
        {
            if (ratio == 1) this.initialize();
        });
    }

    public override void Update()
    {
        if (!ready) return;
    }

    public override void Kill()
    {
        int i;
        int n = this.positionQuadAr.Count;
        for (i = 0; i < n; i++)
        {
            this.positionQuadAr[i].removeFromParent(true);
            this.positionQuadAr[i] = null;
        }
        this.positionQuadAr.Clear();
        n = this.achievementAr.Count;
        for (i = 0; i < n; i++)
        {
            this.achievementAr[i].removeFromParent(true);
            this.achievementAr[i] = null;
        }
        this.achievementAr.Clear();
        TweenLite.killTweensOf(this.innerSprite);
        this._background.removeFromParent(true);
        this._background = null;
        this.innerSprite.removeFromParent(true);
        this.innerSprite = null;
        this.outerSprite.removeFromParent(true);
        this.outerSprite = null;
        this.positionQuad.removeFromParent(true);
        this.positionQuad = null;
        this.achievement.removeFromParent(true);
        this.achievement = null;
        this.bar.removeEventListener(TouchEvent.TOUCH, this.touchScroller);
        this.bar.removeFromParent(true);
        this.bar = null;
        this.scroller.removeEventListener(TouchEvent.TOUCH, this.touchScroller);
        this.scroller.removeFromParent(true);
        this.scroller = null;
        this.achievementInfo.removeFromParent(true);
        this.achievementInfo = null;
        this.achievementTitle.removeFromParent(true);
        this.achievementTitle = null;
        this.backButton.removeEventListener(TouchEvent.TOUCH, this.touchBack);
        this.backButton.removeFromParent(true);
        this.backButton = null;
        this.achievementAtlas.dispose();
        this.achievementAtlas = null;
        this._global.assetManager.purge();
        ready = false;
    }

    public void initialize()
    {
        GALib.TrackEvent(GALib.ENTERED_ACHIEVEMENT_ROOM);
        this.achievementAtlas = this._global.assetManager.getTextureAtlas("achievement");
        this._background = new cSprite(this._global.assetManager.getTexture("achievement_BG"));
        this._background.blendMode = BlendMode.NONE;
        this._background.touchable = false;
        this._global._gameS.addChild(this._background);
        this.outerSprite = new Sprite();
        this._global._gameS.addChild(this.outerSprite);
        this.innerSprite = new Sprite();
        this.outerSprite.addChild(this.innerSprite);
        for (int i = 0; i < this.achievementName.Count; i++)
        {
            this.positionQuad = new Quad(475, 470);
            this.positionQuad.pivotX = 237.5f;
            this.positionQuad.pivotY = 235;
            this.positionQuad.y = i * 470 + 235;
            this.positionQuad.x = 237.5f;
            this.positionQuad.alpha = 0;
            this.positionQuadAr.push(this.positionQuad);
            this.innerSprite.addChild(this.positionQuad);
            if (TSavegame.currentPlayer.unlocked[i])
            {
                this.achievement = new cSprite(this.achievementAtlas.getTexture(this.achievementName[i]));
            }
            else
            {
                this.achievement = new cSprite(this.achievementAtlas.getTexture(this.achievementNameLocked[i] + "_locked"));
            }
            this.achievement.pivotX = this.achievement.width / 2;
            this.achievement.pivotY = this.achievement.height / 2;
            this.achievement.x = this.positionQuad.x;
            this.achievement.y = this.positionQuad.y;
            this.innerSprite.addChild(this.achievement);
            this.achievementAr.push(this.achievement);
        }
        this.innerSprite.pivotX = 237.5f;
        this.innerSprite.pivotY = 235;
        this.innerSprite.x = 512;
        this.innerSprite.y = 350;
        this.innerSpriteHeight = this.innerSprite.height - 470;
        this.outerSprite.clipRect = new Rectangle(260, 100, 480, 470);
        this.bar = new cSprite(this.achievementAtlas.getTexture("scroll_bar"));
        this.bar.pivotX = this.bar.width / 2;
        this.bar.x = 770;
        this.bar.y = 120;
        this.barHeight = this.bar.height;
        this.bar.idx = BAR;
        this.bar.useHandCursor = true;
        this._global._gameS.addChild(this.bar);
        this.bar.addEventListener(TouchEvent.TOUCH, this.touchScroller);
        this.scroller = new cSprite(this.achievementAtlas.getTexture("scroller"));
        this.scroller.pivotX = this.scroller.width / 2;
        this.scroller.pivotY = this.scroller.height / 2;
        this.scroller.x = this.bar.x;
        this.scroller.y = this.bar.y;
        this.scroller.useHandCursor = true;
        this.scroller.idx = SCROLLER;
        this._global._gameS.addChild(this.scroller);
        this.scroller.addEventListener(TouchEvent.TOUCH, this.touchScroller);
        this.achievementTitle = new ExtendTextField(465, 150, Achievement.title[Achievement.SERVICE_EXELLENCE], "jenna_sue", 40, 0, true);
        this.achievementTitle.pivotX = 237.5f;
        this.achievementTitle.pivotY = 75;
        this.achievementTitle.x = 512;
        this.achievementTitle.y = 145;
        this.achievementTitle.touchable = false;
        this._global._gameS.addChild(this.achievementTitle);
        this.achievementInfo = new ExtendTextField(465, 150, Achievement.infoProgress[Achievement.SERVICE_EXELLENCE], "jenna_sue", 35);
        this.achievementInfo.pivotX = 237.5f;
        this.achievementInfo.pivotY = 75;
        this.achievementInfo.x = 512;
        this.achievementInfo.y = 500;
        this.achievementInfo.touchable = false;
        this._global._gameS.addChild(this.achievementInfo);
        this.backButton = new cSprite(this.achievementAtlas.getTexture("arrow"));
        this.backButton.pivotX = this.backButton.width / 2;
        this.backButton.pivotY = this.backButton.height / 2;
        this.backButton.x = 60;
        this.backButton.y = 550;
        this.backButton.useHandCursor = true;
        this.backButton.scaleX = -1;
        this._global._gameS.addChild(this.backButton);
        this.backButton.addEventListener(TouchEvent.TOUCH, this.touchBack);
        if (!TSavegame.currentPlayer.unlocked[Achievement.SERVICE_EXELLENCE])
        {
            this.achievementInfo.text = Achievement.infoProgress[Achievement.SERVICE_EXELLENCE];
            this.achievementInfo.text = this.achievementInfo.text.replace("VALUE",
                TGlobalClass.addCommas(TSavegame.currentPlayer.achievementCounter[Achievement.SERVICE_EXELLENCE]));
        }
        else
        {
            this.achievementInfo.text = Achievement.info[Achievement.SERVICE_EXELLENCE];
        }
        ready = true;
    }

    public void touchBack(Event arg1)
    {
        var e = (TouchEvent)arg1;
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) != null)
        {
            this._global._game.fadeToScreen(this.backToMainMenu ? (Screen)this._global._mainMenuScreen : this._global._kitchen);
        }
    }

    public void touchScroller(Event arg1)
    {
        var e = (TouchEvent)arg1;
        int index = 0;
        int targetIdx = ((cSprite)e.currentTarget).idx;
        Touch touch = e.getTouch(this._global._game);
        float ratio = 0;
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) != null)
        {
            this.achievementTitle.visible = false;
            this.achievementInfo.visible = false;
            if (targetIdx == BAR)
            {
                this.scroller.y = touch.globalY;
            }
            ratio = (this.scroller.y - this.bar.y) / this.barHeight;
            TweenLite.killTweensOf(this.innerSprite);
            TweenLite.to(this.innerSprite, 0.5f, new Vars
            {
                ["y"] = -(ratio * this.innerSpriteHeight) + 350,
                ["ease"] = Linear.easeNone,
            });
        }
        if (e.getTouch(this._global._game, TouchPhase.MOVED) != null)
        {
            if (this.scroller.y >= this.bar.y && this.scroller.y <= this.bar.y + this.barHeight)
            {
                this.scroller.y = touch.globalY;
            }
            if (this.scroller.y < this.bar.y)
            {
                this.scroller.y = this.bar.y;
            }
            if (this.scroller.y > this.bar.y + this.barHeight)
            {
                this.scroller.y = this.bar.y + this.barHeight;
            }
            ratio = (this.scroller.y - this.bar.y) / this.barHeight;
            this.innerSprite.y = -(ratio * this.innerSpriteHeight) + 350;
        }
        if (e.getTouch(this._global._game, TouchPhase.ENDED) != null)
        {
            index = (int)((this.scroller.y - this.bar.y) / (this.barHeight / 10));
            if (index > 9)
            {
                index = 9;
            }
            if (index < 10)
            {
                this.tweenInnerSprite(index);
            }
        }
    }

    public void tweenInnerSprite(int arg1)
    {
        int index = arg1;
        TweenLite.killTweensOf(this.innerSprite);
        this.achievementTitle.text = Achievement.title[index];
        if (!TSavegame.currentPlayer.unlocked[index])
        {
            this.achievementInfo.text = Achievement.infoProgress[index];
            this.achievementInfo.text = this.achievementInfo.text.replace("VALUE",
                TGlobalClass.addCommas(TSavegame.currentPlayer.achievementCounter[index]));
        }
        else
        {
            this.achievementInfo.text = Achievement.info[index];
        }
        TweenLite.to(this.innerSprite, 0.3f, new Vars
        {
            ["y"] = -(index * 470) + 350,
            ["ease"] = Linear.easeNone,
            ["onComplete"] = (Action)(() =>
            {
                this.achievementInfo.visible = true;
                this.achievementTitle.visible = true;
            }),
        });
    }
}
