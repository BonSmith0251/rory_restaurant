package {
class EndingClass extends Screen {
    var _global:Global;
    var endingAtlas:TextureAtlas;
    var touchQuad:Quad;
    var firstSlide:cSprite;
    var secondSlide:cSprite;
    var smile:cSprite;
    var wonder:cSprite;
    var bubble:cSprite;
    var restaurant:cSprite;
    var toFrench:cSprite;
    var higherEdu:cSprite;
    var finalSlide:cSprite;
    var textbox:Quad;
    var textfield:ExtendTextField;
    var bubbleText:cSprite;
    var bubbleTextField:ExtendTextField;
    var currentStory:int;
    var canProceed:Boolean;

    function EndingClass():* {
        super();
    }

    override function Start():void {
        super.Start();
        this._global.currentSceneID = this._global.ENDING_ID;
        this._global.assetManager.enqueue(this._global.graphicsPath + "ending.ean", this._global.graphicsPath + "ending.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "first_slide_ending.eag");
        this._global.assetManager.enqueue(this._global.graphicsPath + "second_slide_ending.eag");
        this._global.assetManager.loadQueue(function(f1_arg1:Number):void {
            if (f1_arg1 != 1) goto L14;
            initialize();
          L14:
        });
    }

    override function Kill():void {
        this.touchQuad.removeEventListener(TouchEvent.TOUCH, this.nextStory);
        this.touchQuad.removeFromParent(true);
        this.touchQuad = null;
        this.firstSlide.removeFromParent(true);
        this.firstSlide = null;
        this.secondSlide.removeFromParent(true);
        this.secondSlide = null;
        this.smile.removeFromParent(true);
        this.smile = null;
        this.wonder.removeFromParent(true);
        this.wonder = null;
        this.bubble.removeFromParent(true);
        this.bubble = null;
        this.restaurant.removeFromParent(true);
        this.restaurant = null;
        this.toFrench.removeFromParent(true);
        this.toFrench = null;
        this.higherEdu.removeFromParent(true);
        this.higherEdu = null;
        this.finalSlide.removeFromParent(true);
        this.finalSlide = null;
        this.textbox.removeFromParent(true);
        this.textbox = null;
        this.textfield.removeFromParent(true);
        this.textfield = null;
        this.bubbleText.removeFromParent(true);
        this.bubbleText = null;
        this.bubbleTextField.removeFromParent(true);
        this.bubbleTextField = null;
        this.endingAtlas.dispose();
        this.endingAtlas = null;
        this._global.assetManager.purge();
        ready = false;
    }

    override function Update():void {
        if (ready) goto L10;
        return;
      L10:
    }

    function initialize():void {
        GALib.TrackEvent(GALib.ENTERED_ENDING_SCREEN);
        TSavegame.currentPlayer.endingViewed = true;
        TSavegame.save();
        this.endingAtlas = this._global.assetManager.getTextureAtlas("ending");
        this.touchQuad = new Quad(1024, 600);
        this.touchQuad.alpha = 0;
        this._global._gameS.addChild(this.touchQuad);
        this.touchQuad.addEventListener(TouchEvent.TOUCH, this.nextStory);
        this.firstSlide = new cSprite(this._global.assetManager.getTexture("first_slide_ending"));
        this.firstSlide.touchable = false;
        this._global._gameS.addChild(this.firstSlide);
        this.secondSlide = new cSprite(this._global.assetManager.getTexture("second_slide_ending"));
        this.secondSlide.touchable = false;
        this.secondSlide.visible = false;
        this._global._gameS.addChild(this.secondSlide);
        this.smile = new cSprite(this.endingAtlas.getTexture("happy"));
        this.smile.x = 1024 - this.smile.width;
        this.smile.touchable = false;
        this.smile.visible = false;
        this._global._gameS.addChild(this.smile);
        this.wonder = new cSprite(this.endingAtlas.getTexture("wonder"));
        this.wonder.y = 600 - this.wonder.height;
        this.wonder.touchable = false;
        this.wonder.visible = false;
        this._global._gameS.addChild(this.wonder);
        this.bubble = new cSprite(this.endingAtlas.getTexture("bubble"));
        this.bubble.x = 36;
        this.bubble.y = 46;
        this.bubble.touchable = false;
        this.bubble.visible = false;
        this._global._gameS.addChild(this.bubble);
        this.restaurant = new cSprite(this.endingAtlas.getTexture("big_restaurant"));
        this.restaurant.pivotX = this.restaurant.width / 2;
        this.restaurant.pivotY = this.restaurant.height / 2;
        this.restaurant.x = 172;
        this.restaurant.y = 156.5;
        this.restaurant.touchable = false;
        this.restaurant.visible = false;
        this._global._gameS.addChild(this.restaurant);
        this.toFrench = new cSprite(this.endingAtlas.getTexture("to_french"));
        this.toFrench.pivotX = this.toFrench.width / 2;
        this.toFrench.pivotY = this.toFrench.height / 2;
        this.toFrench.x = 169;
        this.toFrench.y = 152.5;
        this.toFrench.touchable = false;
        this.toFrench.visible = false;
        this._global._gameS.addChild(this.toFrench);
        this.higherEdu = new cSprite(this.endingAtlas.getTexture("higher_edu"));
        this.higherEdu.pivotX = this.higherEdu.width / 2;
        this.higherEdu.pivotY = this.higherEdu.height / 2;
        this.higherEdu.x = 172;
        this.higherEdu.y = 153;
        this.higherEdu.touchable = false;
        this.higherEdu.visible = false;
        this._global._gameS.addChild(this.higherEdu);
        this.finalSlide = new cSprite(this.endingAtlas.getTexture("final"));
        this.finalSlide.pivotX = this.finalSlide.width / 2;
        this.finalSlide.x = 512;
        this.finalSlide.touchable = false;
        this.finalSlide.visible = false;
        this._global._gameS.addChild(this.finalSlide);
        this.textbox = new Quad(1024, 50);
        this.textbox.y = 600 - this.textbox.height;
        this.textbox.alpha = 0.7;
        this.textbox.touchable = false;
        this._global._gameS.addChild(this.textbox);
        this.textbox.addEventListener(TouchEvent.TOUCH, this.nextStory);
        this.textfield = new ExtendTextField(this.textbox.width, this.textbox.height, "", "pfennig", 24, 0);
        this.textfield.x = this.textbox.x + 10;
        this.textfield.y = this.textbox.y + 5;
        this.textfield.touchable = false;
        this.textfield.hAlign = "left";
        this.textfield.vAlign = "top";
        this._global._gameS.addChild(this.textfield);
        this.bubbleText = new cSprite(this.endingAtlas.getTexture("bubble_text"));
        this.bubbleText.pivotX = 269;
        this.bubbleText.x = this.smile.x + 100;
        this.bubbleText.y = this.smile.y + this.smile.height - 50;
        this.bubbleText.touchable = false;
        this.bubbleText.visible = false;
        this._global._gameS.addChild(this.bubbleText);
        this.bubbleTextField = new ExtendTextField(this.bubbleText.width - 30, this.bubbleText.height - 28, "AAAAA", "bromine", 16);
        this.bubbleTextField.pivotX = 269;
        this.bubbleTextField.x = this.bubbleText.x + 15;
        this.bubbleTextField.y = this.bubbleText.y + 28;
        this.bubbleTextField.touchable = false;
        this.bubbleTextField.visible = false;
        this._global._gameS.addChild(this.bubbleTextField);
        this.bubbleText.scaleX = 0;
        this.bubbleText.scaleY = 0;
        this.textfield.text = "And there she goes, Rory’s little restaurant gets more crowded day by day.";
        this.currentStory = 0;
        this.canProceed = true;
        ready = true;
    }

    function nextStory(arg1:TouchEvent):void {
        e = arg1;
        if (!e.getTouch(this._global._game, TouchPhase.BEGAN)) goto L769;
        if (this.canProceed) goto L45;
        return;
      L45:
        var _local3 = this;
        var _local4 = this.currentStory + 1;
        _local3.currentStory = _local4;
        this.canProceed = false;
        this.tweenBubbleTextOut();
        if (this.currentStory != 1) goto L140;
        this.secondSlide.alpha = 0;
        this.secondSlide.visible = true;
        TweenLite.to(this.secondSlide, 1, {"alpha": 1, "ease": Linear.easeNone, "onComplete": function():void {
            textfield.text = "One night, Rory is counting her profit.";
            canProceed = true;
        }});
        goto L769;
      L140:
        if (this.currentStory != 2) goto L233;
        this.bubbleText.x = this.smile.x;
        this.bubbleTextField.x = this.bubbleText.x + 15;
        TweenLite.delayedCall(1, this.tweenBubbleText, ["30.."]);
        TweenLite.delayedCall(2, this.tweenBubbleText, ["30..40.."]);
        TweenLite.delayedCall(3, function():void {
            tweenBubbleText("30..40..50..");
            canProceed = true;
        });
        goto L769;
      L233:
        if (this.currentStory != 3) goto L317;
        this.smile.visible = true;
        this.smile.x = 1024 + this.smile.width;
        TweenLite.to(this.smile, 1, {"x": 1024 - this.smile.width, "ease": Linear.easeNone, "onComplete": function():void {
            bubbleText.x = smile.x + 100;
            bubbleTextField.x = bubbleText.x + 15;
            tweenBubbleText("50 thousand dollars and more! I have earned more than enough to pay off my Student Loan!");
            textfield.text = "Thanks to you, now she has enough money to pay her debts!";
            canProceed = true;
        }});
        goto L769;
      L317:
        if (this.currentStory != 4) goto L416;
        this.textfield.text = "But what goal will she pursue next? What does Rory dream of now?";
        this.wonder.x = -this.wonder.width;
        this.wonder.visible = true;
        this.bubble.alpha = 0;
        this.bubble.visible = true;
        TweenLite.to(this.wonder, 1, {"x": 0, "ease": Linear.easeNone, "onComplete": function():void {
            TweenLite.to(bubble, 0.5, {"alpha": 1, "ease": Linear.easeNone, "onComplete": function():void {
                canProceed = true;
            }});
        }});
        goto L769;
      L416:
        if (this.currentStory != 5) goto L479;
        this.restaurant.alpha = 0;
        this.restaurant.visible = true;
        TweenLite.to(this.restaurant, 1, {"alpha": 1, "ease": Linear, "onComplete": function():void {
            textfield.text = "Own a Grand restaurant.";
            canProceed = true;
        }});
        goto L769;
      L479:
        if (this.currentStory != 6) goto L574;
        this.toFrench.alpha = 0;
        this.toFrench.visible = true;
        TweenLite.to(this.restaurant, 0.7, {"alpha": 0, "ease": Linear.easeNone});
        TweenLite.to(this.toFrench, 1, {"alpha": 1, "ease": Linear.easeNone, "onComplete": function():void {
            textfield.text = "Maybe start a living in French.";
            restaurant.visible = false;
            canProceed = true;
        }});
        goto L769;
      L574:
        if (this.currentStory != 7) goto L669;
        this.higherEdu.alpha = 0;
        this.higherEdu.visible = true;
        TweenLite.to(this.toFrench, 0.7, {"alpha": 0, "ease": Linear.easeNone});
        TweenLite.to(this.higherEdu, 1, {"alpha": 1, "ease": Linear.easeNone, "onComplete": function():void {
            textfield.text = "Or pursue higher culinary education.";
            toFrench.visible = false;
            canProceed = true;
        }});
        goto L769;
      L669:
        if (this.currentStory != 8) goto L733;
        this.finalSlide.y = 600;
        this.finalSlide.visible = true;
        TweenLite.to(this.finalSlide, 1.5, {"y": 0, "ease": Linear, "onComplete": function():void {
            textfield.text = "Anyway now she already owns a very successful restaurant, let's help her grow it even bigger!";
            canProceed = true;
        }});
        goto L769;
      L733:
        if (this.currentStory != 9) goto L769;
        TGlobalClass.coinsGot = 0;
        this._global._game.fadeToScreen(this._global._kitchen);
      L769:
    }

    function tweenBubbleText(arg1:String):void {
        text = arg1;
        this.bubbleText.visible = true;
        TweenLite.to(this.bubbleText, 0.3, {"scaleX": 1, "scaleY": 1, "ease": Linear.easeNone, "onComplete": function(f1_arg1:String):void {
            bubbleTextField.text = f1_arg1;
            bubbleTextField.visible = true;
        }, "onCompleteParams": [text]});
    }

    function tweenBubbleTextOut():void {
        this.bubbleTextField.visible = false;
        TweenLite.to(this.bubbleText, 0.3, {"scaleX": 0, "scaleY": 0, "ease": Linear.easeNone, "onComplete": function():void {
            bubbleText.visible = false;
        }});
    }
}
}
