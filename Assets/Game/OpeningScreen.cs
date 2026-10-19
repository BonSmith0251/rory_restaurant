// Ported from OpeningScreen.as (decompiled). Intro story slideshow shown before the first kitchen day.
using System;
using System.Collections.Generic;
using Starlite;

public class OpeningScreen : Screen
{
    public Global _global;
    public TextureAtlas openingAtlas;
    public Quad touchQuad;
    public cSprite firstSlide;
    public cSprite lastSlide;
    public cSprite lightBulb;
    public cSprite smile;
    public cSprite cry;
    public cSprite think;
    public cSprite studentLoan;
    public cSprite brosur;
    public cSprite mask;
    public Quad textbox;
    public ExtendTextField textfield;
    public cSprite bubbleText;
    public ExtendTextField bubbleTextField;
    public int currentStory;
    public bool canProceed;

    public OpeningScreen()
    {
    }

    public override void Start()
    {
        base.Start();
        this._global.currentSceneID = this._global.OPENING_ID;
        this._global.assetManager.enqueue(this._global.graphicsPath + "opening.ean", this._global.graphicsPath + "opening.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "first_slide_opening.eag");
        this._global.assetManager.enqueue(this._global.graphicsPath + "last_slide_opening.eag");
        this._global.assetManager.enqueue(this._global.graphicsPath + "mask.ean");
        this._global.assetManager.loadQueue((float ratio) =>
        {
            if (ratio == 1) this.initialize();
        });
    }

    public override void Kill()
    {
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

    public override void Update()
    {
        if (!ready) return;
    }

    public void initialize()
    {
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
        this.textbox.alpha = 0.7f;
        this.textbox.touchable = false;
        this._global._gameS.addChild(this.textbox);
        this.textfield = new ExtendTextField((int)(this.textbox.width - 20), (int)this.textbox.height, "", "pfennig", 30, 0);
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
        this.bubbleTextField = new ExtendTextField((int)(this.bubbleText.width - 30), (int)(this.bubbleText.height - 28), "AAAAA", "bromine", 25);
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

    public void nextStory(Event arg1)
    {
        var e = (TouchEvent)arg1;
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) == null) return;
        if (!this.canProceed) return;
        this.currentStory++;
        this.canProceed = false;
        this.tweenBubbleTextOut();
        if (this.currentStory == 1)
        {
            this.textfield.text = "She just received her Chef Certificate after graduating from Culinary School.";
            this.smile.x = 1200;
            this.smile.visible = true;
            TweenLite.to(this.smile, 0.6f, new Vars
            {
                ["x"] = 1024 - this.smile.width,
                ["ease"] = Linear.easeNone,
                ["onComplete"] = (Action)(() => { this.canProceed = true; }),
            });
        }
        else if (this.currentStory == 2)
        {
            this.textfield.text = "But wait, what is that small note attached below?";
            this.mask.alpha = 0;
            this.mask.visible = true;
            TweenLite.to(this.mask, 0.7f, new Vars
            {
                ["alpha"] = 1,
                ["ease"] = Linear.easeNone,
                ["onComplete"] = (Action)(() => { this.canProceed = true; }),
            });
        }
        else if (this.currentStory == 3)
        {
            this.textfield.text = "Ow, Rory has a huge debt from her Student Loan!";
            this.studentLoan.x = -this.studentLoan.width;
            this.cry.alpha = 0;
            this.studentLoan.visible = true;
            this.cry.visible = true;
            TweenLite.to(this.studentLoan, 0.7f, new Vars { ["x"] = 0, ["ease"] = Linear.easeNone });
            TweenLite.to(this.cry, 1.4f, new Vars
            {
                ["alpha"] = 1,
                ["ease"] = Linear.easeNone,
                ["onComplete"] = (Action)(() =>
                {
                    this.smile.visible = false;
                    this.canProceed = true;
                }),
            });
        }
        else if (this.currentStory == 4)
        {
            this.textfield.text = "And what’s this? A new restaurant just started out its business.. near Rory’s place!";
            this.brosur.alpha = 0;
            this.brosur.visible = true;
            TweenLite.to(this.brosur, 1, new Vars
            {
                ["alpha"] = 1,
                ["ease"] = Linear.easeNone,
                ["onComplete"] = (Action)(() =>
                {
                    this.studentLoan.visible = false;
                    this.canProceed = true;
                }),
            });
        }
        else if (this.currentStory == 5)
        {
            this.textfield.text = "Seems like Rory has got an idea to solve her financial problem.";
            this.think.alpha = 0;
            this.think.visible = true;
            this.tweenBubbleText("HMM...");
            TweenLite.to(this.think, 0.7f, new Vars
            {
                ["alpha"] = 1,
                ["ease"] = Linear.easeNone,
                ["onComplete"] = (Action)(() =>
                {
                    this.cry.visible = false;
                    this.canProceed = true;
                }),
            });
        }
        else if (this.currentStory == 6)
        {
            this.lastSlide.x = 1024;
            this.lastSlide.visible = true;
            TweenLite.to(this.lastSlide, 0.5f, new Vars { ["x"] = 0, ["ease"] = Linear.easeNone });
            TweenLite.delayedCall(1.2f, (Action)(() =>
            {
                this.bubbleText.x = 450;
                this.bubbleText.y = 300;
                this.bubbleTextField.x = this.bubbleText.x + 15;
                this.bubbleTextField.y = this.bubbleText.y + 28;
                this.tweenBubbleText("I’ll open a mini restaurant to pay off my debts!");
                this.textfield.text = "Genius idea! Rory now has a sound plan to pay back her debts!";
                this.canProceed = true;
                this.lightBulb.visible = true;
            }));
        }
        else if (this.currentStory == 7)
        {
            TweenLite.killTweensOf(this.bubbleText);
            TSavegame.currentPlayer.openingViewed = true;
            TSavegame.save();
            this._global._game.fadeToScreen(this._global._kitchen);
        }
    }

    public void tweenBubbleText(string arg1)
    {
        this.bubbleText.visible = true;
        TweenLite.to(this.bubbleText, 0.3f, new Vars
        {
            ["scaleX"] = 1,
            ["scaleY"] = 1,
            ["ease"] = Linear.easeNone,
            ["onComplete"] = (Action<string>)((string text) =>
            {
                this.bubbleTextField.text = text;
                this.bubbleTextField.visible = true;
            }),
            ["onCompleteParams"] = new object[] { arg1 },
        });
    }

    public void tweenBubbleTextOut()
    {
        this.bubbleTextField.visible = false;
        TweenLite.to(this.bubbleText, 0.3f, new Vars
        {
            ["scaleX"] = 0,
            ["scaleY"] = 0,
            ["ease"] = Linear.easeNone,
            ["onComplete"] = (Action)(() => { this.bubbleText.visible = false; }),
        });
    }
}
