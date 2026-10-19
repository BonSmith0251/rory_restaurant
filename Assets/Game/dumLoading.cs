// Ported from dumLoading.as (decompiled). First screen: shows the publisher branding splashes
// (fade in, hold, fade out; a tap skips one), then the developer logo, then fades to the main menu.
//
// Port notes:
// - Flash enumerated <app>/branding with flash.filesystem.File and enqueued every .jpg/.png
//   (and read PublisherName from a "*.txt" file name). Unity has no such folder listing over Resources, so the
//   shipped branding files are listed in BRANDING_FILES (the install's branding folder holds splash1.jpg and
//   splash2.jpg, no .txt). They load from Resources/RR/branding/<name> like every other asset.
// - The developer logo was a Flash vector MovieClip (dumLoading_logoSplash, logo.swf) put on the native stage;
//   it cannot be played in Unity, so onSplashComplete() continues as if it had reached its last frame.
using System;
using System.Collections.Generic;
using Starlite;

public class dumLoading : Screen
{
    /// <summary>Port replacement for the branding folder listing (file names inside "branding\").</summary>
    public static readonly List<string> BRANDING_FILES = new List<string> { "splash1.jpg", "splash2.jpg" };

    public Global _global;
    public float splashDuration = 1.5f;
    public float splashFadeDuration = 0.5f;
    public XML splashXML;
    public int loadedCount;
    public List<cSprite> splashAr;
    public List<BitmapData> allBitmapData;
    public bool onLoadProgress;
    public MovieClip logoMC;
    public int showCount = 0;
    /// <summary>AS3: [Embed] MovieClip class dumLoading_logoSplash (Flash vector animation, not available in Unity).</summary>
    public Type logoSplash = null;

    public dumLoading()
    {
        this.splashAr = new List<cSprite>();
        this.allBitmapData = new List<BitmapData>();
    }

    public override void Start()
    {
        base.Start();
        this._global._stage.frameRate = 30; // AS3: Starling.current.nativeStage.frameRate = 30
        if (TGlobalClass.dekstopVersion)
        {
            this.onLoadProgress = true;
            try
            {
                for (int i = 0; i < BRANDING_FILES.Count; i++)
                {
                    string f = BRANDING_FILES[i];
                    string type = f.lastIndexOf(".") > -1 ? f.substring(f.lastIndexOf(".")) : ""; // File.type
                    if (type == ".jpg" || type == ".png")
                    {
                        this._global.assetManager.enqueue("branding\\" + f);
                    }
                    if (f.toLowerCase().lastIndexOf(".txt") > -1)
                    {
                        TGlobalClass.PublisherName = f.substring(0, f.Length - 11);
                    }
                }
                this.loadedCount = BRANDING_FILES.Count;
                if (this.loadedCount == 0)
                {
                    this.onSplashComplete();
                }
                else
                {
                    this._global.assetManager.loadQueue((float ratio) =>
                    {
                        if (ratio != 1) return;
                        List<Texture> textures = this._global.assetManager.getTextures();
                        for (int i = 0; i < this.loadedCount; i++)
                        {
                            Texture t = textures.at(i);
                            if (t == null)
                            {
                                // AS3: new cSprite(textures[i]) threw -> catch decremented loadedCount
                                this.loadedCount--;
                                continue;
                            }
                            var splash = new cSprite(t);
                            this.splashAr.push(splash);
                            splash.alpha = 0;
                            splash.touchable = false;
                            splash.scaleX = 1024 / splash.width;
                            splash.scaleY = 600 / splash.height;
                            splash.addEventListener(TouchEvent.TOUCH, this.onSplashTouch);
                            this._global._gameS.addChild(splash);
                        }
                        // Guard: the AS3 loop could leave loadedCount above the number of splashes when
                        // textures were missing (it then threw in showSplash); keep the counts consistent.
                        this.loadedCount = this.splashAr.Count;
                        this.showSplash();
                        this.onLoadProgress = false;
                        ready = true;
                    });
                }
            }
            catch (Exception)
            {
                this.onSplashComplete();
            }
        }
    }

    public void onSplashTouch(Event arg1)
    {
        var e = (TouchEvent)arg1;
        if (e.getTouch(this._global._game, "began") != null)
        {
            TweenLite.killDelayedCallsTo((Action)this.hideSplash);
            TweenLite.killTweensOf(e.currentTarget);
            this.hideSplash();
        }
    }

    public void showSplash()
    {
        if (this.showCount == this.loadedCount)
        {
            this.onSplashComplete();
            return;
        }
        this.splashAr[this.showCount].touchable = true;
        int idx = this.showCount;
        this.showCount = idx + 1;
        TweenLite.to(this.splashAr[idx], this.splashFadeDuration, new Vars
        {
            ["alpha"] = 1,
            ["onComplete"] = (Action)(() =>
            {
                TweenLite.delayedCall(this.splashDuration, (Action)this.hideSplash);
            }),
        });
    }

    public void hideSplash()
    {
        this.splashAr[this.showCount - 1].touchable = false;
        this.splashAr[this.showCount - 1].removeEventListener(TouchEvent.TOUCH, this.onSplashTouch);
        TweenLite.to(this.splashAr[this.showCount - 1], this.splashFadeDuration, new Vars
        {
            ["alpha"] = 0,
            ["onComplete"] = (Action)(() => { this.showSplash(); }),
        });
    }

    public override void Kill()
    {
        if (this.onLoadProgress) return;
        while (this.splashAr.Count > 0)
        {
            this.splashAr.pop().removeFromParent(true);
        }
        while (this.allBitmapData.Count > 0)
        {
            this.allBitmapData.pop().dispose();
        }
        this.splashXML = null;
        this._global.assetManager.purge();
        ready = false;
    }

    public void onSplashComplete()
    {
        // AS3: logoMC = new logoSplash(); scaled to the stage, added to the native stage, and checkFrame()
        // fades to the main menu once it reaches its last frame. The Flash MovieClip has no Unity equivalent,
        // so continue directly with what checkFrame() does at the end of the animation.
        if (this.logoMC != null)
        {
            this.logoMC.scaleX = (float)this._global._stage.stageWidth / 1024;
            this.logoMC.scaleY = (float)this._global._stage.stageHeight / 600;
            this._global._stage.addChild(this.logoMC);
            this.logoMC.addEventListener(Event.ENTER_FRAME, this.checkFrame);
            return;
        }
        this._global._game.fadeToScreen(this._global._mainMenuScreen);
    }

    public void checkFrame(Event arg1)
    {
        // Flash frames are 1-based (currentFrame == totalFrames); Starling MovieClip frames are 0-based.
        if (this.logoMC.currentFrame == this.logoMC.numFrames - 1)
        {
            this._global._game.fadeToScreen(this._global._mainMenuScreen);
            this._global._stage.removeChild(this.logoMC);
            this.logoMC.removeEventListener(Event.ENTER_FRAME, this.checkFrame);
            this.logoMC = null;
        }
    }
}
