package {
class OpeningScreen extends Screen {
    var _global:Global;
    var openingAtlas:TextureAtlas;
    var touchQuad:Quad;
    var firstSlide:cSprite;
    var lastSlide:cSprite;
    var lightBulb:cSprite;
    var smile:cSprite;
    var cry:cSprite;
    var think:cSprite;
    var studentLoan:cSprite;
    var brosur:cSprite;
    var mask:cSprite;
    var textbox:Quad;
    var textfield:ExtendTextField;
    var bubbleText:cSprite;
    var bubbleTextField:ExtendTextField;
    var currentStory:int;
    var canProceed:Boolean;

    function OpeningScreen():* {
        super();
    }

    override function Start():void {
        super.Start();
        this._global.currentSceneID = this._global.OPENING_ID;
        this._global.assetManager.enqueue(this._global.graphicsPath + "opening.ean", this._global.graphicsPath + "opening.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "first_slide_opening.eag");
        this._global.assetManager.enqueue(this._global.graphicsPath + "last_slide_opening.eag");
        this._global.assetManager.enqueue(this._global.graphicsPath + "mask.ean");
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
        this.lastSlide.removeFromParent(true);
        this.lastSlide = null;
        this.mask.removeFromParent(true);
        this.mask = null;
        this.smile.removeFromParent(true);
        this.smile = null;
        this.cry.removeFromParent(true);
        this.cry = null;
        this.think.removeFromParent(true);
        this.think = null;
        this.studentLoan.removeFromParent(true);
        this.studentLoan = null;
        this.brosur.removeFromParent(true);
        this.brosur = null;
        this.lightBulb.removeFromParent(true);
        this.lightBulb = null;
        this.textfield.removeFromParent(true);
        this.textfield = null;
        this.textbox.removeFromParent(true);
        this.textbox = null;
        this.bubbleText.removeFromParent(true);
        this.bubbleText = null;
        this.bubbleTextField.removeFromParent(true);
        this.bubbleTextField = null;
        this.openingAtlas.dispose();
        this.openingAtlas = null;
        this._global.assetManager.purge();
        ready = false;
    }

    override function Update():void {
        if (ready) goto L10;
        return;
      L10:
    }

    function initialize():void {
        this.openingAtlas = this._global.assetManager.getTextureAtlas("opening");
        this.touchQuad = new Quad(1024, 600);
        this.touchQuad.alpha = 0;
        this._global._gameS.addChild(this.touchQuad);
        this.touchQuad.addEventListener(TouchEvent.TOUCH, this.nextStory);
        this.firstSlide = new cSprite(this._global.assetManager.getTexture("first_slide_opening"));
        this.firstSlide.blendMode = BlendMode.NONE;
        this.firstSlide.touchable = false;
        this._global._gameS.addChild(this.firstSlide);
        this.mask = new cSprite(this._global.assetManager.getTexture("mask"));
        this.mask.touchable = false;
        this.mask.visible = false;
        this._global._gameS.addChild(this.mask);
        this.smile = new cSprite(this.openingAtlas.getTexture("smile"));
        this.smile.touchable = false;
        this.smile.x = 1024 - this.smile.width;
        this.smile.visible = false;
        this._global._gameS.addChild(this.smile);
        this.cry = new cSprite(this.openingAtlas.getTexture("cry"));
        this.cry.touchable = false;
        this.cry.x = this.smile.x;
        this.cry.visible = false;
        this._global._gameS.addChild(this.cry);
        this.think = new cSprite(this.openingAtlas.getTexture("think"));
        this.think.x = this.smile.x;
        this.think.touchable = false;
        this.think.visible = false;
        this._global._gameS.addChild(this.think);
        this.studentLoan = new cSprite(this.openingAtlas.getTexture("student_loan"));
        this.studentLoan.x = 10;
        this.studentLoan.y = 10;
        this.studentLoan.visible = false;
        this.studentLoan.touchable = false;
        this._global._gameS.addChild(this.studentLoan);
        this.brosur = new cSprite(this.openingAtlas.getTexture("brosur"));
        this.brosur.y = 10;
        this.brosur.visible = false;
        this.brosur.touchable = false;
        this._global._gameS.addChild(this.brosur);
        this.lastSlide = new cSprite(this._global.assetManager.getTexture("last_slide_opening"));
        this.lastSlide.visible = false;
        this.lastSlide.touchable = false;
        this._global._gameS.addChild(this.lastSlide);
        this.lightBulb = new cSprite(this.openingAtlas.getTexture("lightbulb"));
        this.lightBulb.x = 250;
        this.lightBulb.y = 20;
        this.lightBulb.visible = false;
        this.lightBulb.touchable = false;
        this._global._gameS.addChild(this.lightBulb);
        this.textbox = new Quad(1024, 50);
        this.textbox.y = 600 - this.textbox.height;
        this.textbox.alpha = 0.7;
        this.textbox.touchable = false;
        this._global._gameS.addChild(this.textbox);
        this.textfield = new ExtendTextField(this.textbox.width - 20, this.textbox.height, "", "pfennig", 30, 0);
        this.textfield.x = 10;
        this.textfield.y = this.textbox.y;
        this.textfield.touchable = false;
        this.textfield.hAlign = "left";
        this.textfield.vAlign = "top";
        this._global._gameS.addChild(this.textfield);
        this.bubbleText = new cSprite(this.openingAtlas.getTexture("bubble_text"));
        this.bubbleText.pivotX = 269;
        this.bubbleText.x = this.cry.x + 100;
        this.bubbleText.y = this.cry.y + this.cry.height - 50;
        this.bubbleText.visible = false;
        this.bubbleText.touchable = false;
        this._global._gameS.addChild(this.bubbleText);
        this.bubbleTextField = new ExtendTextField(this.bubbleText.width - 30, this.bubbleText.height - 28, "AAAAA", "bromine", 25);
        this.bubbleTextField.pivotX = 269;
        this.bubbleTextField.x = this.bubbleText.x + 15;
        this.bubbleTextField.y = this.bubbleText.y + 28;
        this.bubbleTextField.visible = false;
        this.bubbleTextField.touchable = false;
        this._global._gameS.addChild(this.bubbleTextField);
        this.bubbleText.scaleX = 0;
        this.bubbleText.scaleY = 0;
        this.textfield.text = "One early morning, our sweet Rory is checking up on her mails..";
        this.currentStory = 0;
        this.canProceed = true;
        ready = true;
    }

    function nextStory(arg1:TouchEvent):void {
        e = arg1;
        if (!e.getTouch(this._global._game, TouchPhase.BEGAN)) goto L653;
        if (this.canProceed) goto L45;
        return;
      L45:
        var _local3 = this;
        var _local4 = this.currentStory + 1;
        _local3.currentStory = _local4;
        this.canProceed = false;
        this.tweenBubbleTextOut();
        if (this.currentStory != 1) goto L160;
        this.textfield.text = "She just received her Chef Certificate after graduating from Culinary School.";
        this.smile.x = 1200;
        this.smile.visible = true;
        TweenLite.to(this.smile, 0.6, {"x": 1024 - this.smile.width, "ease": Linear.easeNone, "onComplete": function():void {
            canProceed = true;
        }});
        goto L653;
      L160:
        if (this.currentStory != 2) goto L236;
        this.textfield.text = "But wait, what is that small note attached below?";
        this.mask.alpha = 0;
        this.mask.visible = true;
        TweenLite.to(this.mask, 0.7, {"alpha": 1, "ease": Linear.easeNone, "onComplete": function():void {
            canProceed = true;
        }});
        goto L653;
      L236:
        if (this.currentStory != 3) goto L364;
        this.textfield.text = "Ow, Rory has a huge debt from her Student Loan!";
        this.studentLoan.x = -this.studentLoan.width;
        this.cry.alpha = 0;
        this.studentLoan.visible = true;
        this.cry.visible = true;
        TweenLite.to(this.studentLoan, 0.7, {"x": 0, "ease": Linear.easeNone});
        TweenLite.to(this.cry, 1.4, {"alpha": 1, "ease": Linear.easeNone, "onComplete": function():void {
            smile.visible = false;
            canProceed = true;
        }});
        goto L653;
      L364:
        if (this.currentStory != 4) goto L440;
        this.textfield.text = "And what’s this? A new restaurant just started out its business.. near Rory’s place!";
        this.brosur.alpha = 0;
        this.brosur.visible = true;
        TweenLite.to(this.brosur, 1, {"alpha": 1, "ease": Linear.easeNone, "onComplete": function():void {
            studentLoan.visible = false;
            canProceed = true;
        }});
        goto L653;
      L440:
        if (this.currentStory != 5) goto L524;
        this.textfield.text = "Seems like Rory has got an idea to solve her financial problem.";
        this.think.alpha = 0;
        this.think.visible = true;
        this.tweenBubbleText("HMM...");
        TweenLite.to(this.think, 0.7, {"alpha": 1, "ease": Linear.easeNone, "onComplete": function():void {
            cry.visible = false;
            canProceed = true;
        }});
        goto L653;
      L524:
        if (this.currentStory != 6) goto L597;
        this.lastSlide.x = 1024;
        this.lastSlide.visible = true;
        TweenLite.to(this.lastSlide, 0.5, {"x": 0, "ease": Linear.easeNone});
        TweenLite.delayedCall(1.2, function():void {
            bubbleText.x = 450;
            bubbleText.y = 300;
            bubbleTextField.x = bubbleText.x + 15;
            bubbleTextField.y = bubbleText.y + 28;
            tweenBubbleText("I’ll open a mini restaurant to pay off my debts!");
            textfield.text = "Genius idea! Rory now has a sound plan to pay back her debts!";
            canProceed = true;
            lightBulb.visible = true;
        });
        goto L653;
      L597:
        if (this.currentStory != 7) goto L653;
        TweenLite.killTweensOf(this.bubbleText);
        TSavegame.currentPlayer.openingViewed = true;
        TSavegame.save();
        this._global._game.fadeToScreen(this._global._kitchen);
      L653:
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
