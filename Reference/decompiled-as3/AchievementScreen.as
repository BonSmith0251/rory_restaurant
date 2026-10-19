package {
class AchievementScreen extends Screen {
    const BAR:int = 1;
    const SCROLLER:int = 2;
    var _global:Global;
    var _bolehKlik:Boolean;
    var initVector:Boolean;
    var _background:cSprite;
    var achievementName:Vector.<String>;
    var achievementNameLocked:Vector.<String>;
    var achievementAtlas:TextureAtlas;
    var innerSprite:Sprite;
    var outerSprite:Sprite;
    var achievementTitle:ExtendTextField;
    var achievementInfo:ExtendTextField;
    var achievementAr:Vector.<cSprite>;
    var achievement:cSprite;
    var positionQuad:Quad;
    var positionQuadAr:Vector.<Quad>;
    var bar:cSprite;
    var scroller:cSprite;
    var backButton:cSprite;
    var barHeight:Number;
    var innerSpriteHeight:Number;
    var backToMainMenu:Boolean;

    function AchievementScreen():* {
        var $t0 = new Vector.<String>(10);
        $t0[0] = "service_exellence";
        $t0[1] = "big_business";
        $t0[2] = "western_recipe";
        $t0[3] = "chinese_recipe";
        $t0[4] = "middle_east_recipe";
        $t0[5] = "secret_ingredient";
        $t0[6] = "hawkeye";
        $t0[7] = "froyo_master";
        $t0[8] = "qualified_chef";
        $t0[9] = "international_chef";
        this.achievementName = $t0;
        var $t1 = new Vector.<String>(10);
        $t1[0] = "service_exellence";
        $t1[1] = "big_business";
        $t1[2] = "middle_east_recipe";
        $t1[3] = "middle_east_recipe";
        $t1[4] = "middle_east_recipe";
        $t1[5] = "secret_ingredient";
        $t1[6] = "hawkeye";
        $t1[7] = "froyo_master";
        $t1[8] = "qualified_chef";
        $t1[9] = "international_chef";
        this.achievementNameLocked = $t1;
        super();
    }

    override function Start():void {
        super.Start();
        this._global.currentSceneID = this._global.ACHIEVEMENTS_ID;
        if (this.initVector) goto L66;
        this.achievementAr = new Vector.<cSprite>();
        this.positionQuadAr = new Vector.<Quad>();
        this.initVector = true;
      L66:
        this._global.assetManager.enqueue(this._global.graphicsPath + "achievement.ean", this._global.graphicsPath + "achievement.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "achievement_BG.eag");
        this._global.assetManager.loadQueue(function(f1_arg1:Number):void {
            if (f1_arg1 != 1) goto L14;
            initialize();
          L14:
        });
    }

    override function Update():void {
        if (ready) goto L10;
        return;
      L10:
    }

    override function Kill():void {
        var _local1 = 0;
        var _local2 = 0;
        _local2 = int(this.positionQuadAr.length);
        _local1 = 0;
        goto L49;
      L24:
        this.positionQuadAr[_local1].removeFromParent(true);
        this.positionQuadAr[_local1] = null;
        _local1++;
      L49:
        if (_local1 < _local2) goto L24;
        this.positionQuadAr.length = 0;
        _local2 = int(this.achievementAr.length);
        _local1 = 0;
        goto L105;
      L80:
        this.achievementAr[_local1].removeFromParent(true);
        this.achievementAr[_local1] = null;
        _local1++;
      L105:
        if (_local1 < _local2) goto L80;
        this.achievementAr.length = 0;
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

    function initialize():void {
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
        var _local1 = 0;
        goto L412;
      L150:
        this.positionQuad = new Quad(475, 470);
        this.positionQuad.pivotX = 237.5;
        this.positionQuad.pivotY = 235;
        this.positionQuad.y = _local1 * 470 + 235;
        this.positionQuad.x = 237.5;
        this.positionQuad.alpha = 0;
        this.positionQuadAr.push(this.positionQuad);
        this.innerSprite.addChild(this.positionQuad);
        if (!TSavegame.currentPlayer.unlocked[_local1]) goto L293;
        this.achievement = new cSprite(this.achievementAtlas.getTexture(this.achievementName[_local1]));
        goto L324;
      L293:
        this.achievement = new cSprite(this.achievementAtlas.getTexture(this.achievementNameLocked[_local1] + "_locked"));
      L324:
        this.achievement.pivotX = this.achievement.width / 2;
        this.achievement.pivotY = this.achievement.height / 2;
        this.achievement.x = this.positionQuad.x;
        this.achievement.y = this.positionQuad.y;
        this.innerSprite.addChild(this.achievement);
        this.achievementAr.push(this.achievement);
        _local1++;
      L412:
        if (_local1 < this.achievementName.length) goto L150;
        this.innerSprite.pivotX = 237.5;
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
        this.bar.idx = this.BAR;
        this.bar.useHandCursor = true;
        this._global._gameS.addChild(this.bar);
        this.bar.addEventListener(TouchEvent.TOUCH, this.touchScroller);
        this.scroller = new cSprite(this.achievementAtlas.getTexture("scroller"));
        this.scroller.pivotX = this.scroller.width / 2;
        this.scroller.pivotY = this.scroller.height / 2;
        this.scroller.x = this.bar.x;
        this.scroller.y = this.bar.y;
        this.scroller.useHandCursor = true;
        this.scroller.idx = this.SCROLLER;
        this._global._gameS.addChild(this.scroller);
        this.scroller.addEventListener(TouchEvent.TOUCH, this.touchScroller);
        this.achievementTitle = new ExtendTextField(465, 150, Achievement.title[Achievement.SERVICE_EXELLENCE], "jenna_sue", 40, 0, true);
        this.achievementTitle.pivotX = 237.5;
        this.achievementTitle.pivotY = 75;
        this.achievementTitle.x = 512;
        this.achievementTitle.y = 145;
        this.achievementTitle.touchable = false;
        this._global._gameS.addChild(this.achievementTitle);
        this.achievementInfo = new ExtendTextField(465, 150, Achievement.infoProgress[Achievement.SERVICE_EXELLENCE], "jenna_sue", 35);
        this.achievementInfo.pivotX = 237.5;
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
        if (TSavegame.currentPlayer.unlocked[Achievement.SERVICE_EXELLENCE]) goto L1178;
        this.achievementInfo.text = Achievement.infoProgress[Achievement.SERVICE_EXELLENCE];
        this.achievementInfo.text = this.achievementInfo.text.replace("VALUE", TGlobalClass.addCommas(TSavegame.currentPlayer.achievementCounter[Achievement.SERVICE_EXELLENCE]));
        goto L1200;
      L1178:
        this.achievementInfo.text = Achievement.info[Achievement.SERVICE_EXELLENCE];
      L1200:
        ready = true;
    }

    function touchBack(arg1:TouchEvent):void {
        if (!arg1.getTouch(this._global._game, TouchPhase.BEGAN)) goto L65;
        $s0 = this._global._game;
        if (!this.backToMainMenu) goto L52;
        $s1 = this._global._mainMenuScreen;
        goto L61;
      L52:
        $s1 = this._global._kitchen;
      L61:
        $s0.fadeToScreen($s1);
      L65:
    }

    function touchScroller(arg1:TouchEvent):void {
        var _local5 = 0;
        var _local2 = int((arg1.currentTarget as cSprite).idx);
        var _local3 = arg1.getTouch(this._global._game);
        var _local4 = Number(0);
        if (!arg1.getTouch(this._global._game, TouchPhase.BEGAN)) goto L170;
        this.achievementTitle.visible = false;
        this.achievementInfo.visible = false;
        if (_local2 != this.BAR) goto L97;
        this.scroller.y = _local3.globalY;
      L97:
        _local4 = Number((this.scroller.y - this.bar.y) / this.barHeight);
        TweenLite.killTweensOf(this.innerSprite);
        TweenLite.to(this.innerSprite, 0.5, {"y": -(_local4 * this.innerSpriteHeight) + 350, "ease": Linear.easeNone});
      L170:
        if (!arg1.getTouch(this._global._game, TouchPhase.MOVED)) goto L364;
        var $t0 = this.scroller.y >= this.bar.y;
        $s0 = $t0;
        if (!$t0) goto L233;
        $s0 = this.scroller.y <= this.bar.y + this.barHeight;
      L233:
        if (!$s0) goto L248;
        this.scroller.y = _local3.globalY;
      L248:
        if (!(this.scroller.y < this.bar.y)) goto L280;
        this.scroller.y = this.bar.y;
      L280:
        if (!(this.scroller.y > this.bar.y + this.barHeight)) goto L322;
        this.scroller.y = this.bar.y + this.barHeight;
      L322:
        _local4 = Number((this.scroller.y - this.bar.y) / this.barHeight);
        this.innerSprite.y = -(_local4 * this.innerSpriteHeight) + 350;
      L364:
        if (!arg1.getTouch(this._global._game, TouchPhase.ENDED)) goto L445;
        var $t1 = int((this.scroller.y - this.bar.y) / (this.barHeight / 10));
        _local5 = $t1;
        if (!($t1 > 9)) goto L430;
        _local5 = 9;
      L430:
        if (!(_local5 < 10)) goto L445;
        this.tweenInnerSprite(_local5);
      L445:
    }

    function tweenInnerSprite(arg1:int):void {
        index = arg1;
        TweenLite.killTweensOf(this.innerSprite);
        this.achievementTitle.text = Achievement.title[index];
        if (TSavegame.currentPlayer.unlocked[index]) goto L130;
        this.achievementInfo.text = Achievement.infoProgress[index];
        this.achievementInfo.text = this.achievementInfo.text.replace("VALUE", TGlobalClass.addCommas(TSavegame.currentPlayer.achievementCounter[index]));
        goto L150;
      L130:
        this.achievementInfo.text = Achievement.info[index];
      L150:
        TweenLite.to(this.innerSprite, 0.3, {"y": -(index * 470) + 350, "ease": Linear.easeNone, "onComplete": function():void {
            achievementInfo.visible = true;
            achievementTitle.visible = true;
        }});
    }
}
}
