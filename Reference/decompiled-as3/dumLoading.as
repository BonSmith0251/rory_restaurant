package {
class dumLoading extends Screen {
    var _global:Global;
    var splashDuration:Number = 1.5;
    var splashFadeDuration:Number = 0.5;
    var splashXML:XML;
    var loadedCount:int;
    var splashAr:Vector.<cSprite>;
    var allBitmapData:Vector.<BitmapData>;
    var onLoadProgress:Boolean;
    var logoMC:MovieClip;
    var showCount:int = 0;
    var logoSplash:Class;

    function dumLoading():void {
        this.splashAr = new Vector.<cSprite>();
        this.allBitmapData = new Vector.<BitmapData>();
        this.logoSplash = dumLoading_logoSplash;
        super();
    }

    override function Start():void {
        docs = null;
        dirs = null;
        i = 0;
        f = null;
        super.Start();
        Starling.current.nativeStage.frameRate = 30;
        if (!TGlobalClass.dekstopVersion) goto L388;
        this.onLoadProgress = true;
        // try { (catch e:Error -> L365)
        docs = new File(File.applicationDirectory.nativePath);
        docs = docs.resolvePath("branding");
        if (docs.exists) goto L127;
        this.onSplashComplete();
        return;
      L127:
        dirs = docs.getDirectoryListing();
        goto L302;
      L145:
        f = dirs[i] as File;
        var $t0 = !f.isDirectory;
        $s0 = $t0;
        if (!$t0) goto L210;
        var $t1 = f.type == ".jpg";
        $s0 = $t1;
        if ($t1) goto L210;
        $s0 = f.type == ".png";
      L210:
        if (!$s0) goto L236;
        this._global.assetManager.enqueue("branding\\" + f.name);
      L236:
        if (!(f.name.toLowerCase().lastIndexOf(".txt") > -1)) goto L292;
        TGlobalClass.PublisherName = f.name.substring(0, f.name.length - 11);
      L292:
        i = i + 1;
      L302:
        if (i < dirs.length) goto L145;
        this.loadedCount = dirs.length;
        if (this.loadedCount != 0) goto L347;
        this.onSplashComplete();
        goto L361;
      L347:
        this._global.assetManager.loadQueue(function(f1_arg1:Number):void {
            textures = null;
            ratio = f1_arg1;
            if (ratio != 1) goto L303;
            textures = _global.assetManager.getTextures();
            i = 0;
            goto L272;
          L54:
            // try { (catch e:Error -> L83)
            splashAr.push(new cSprite(textures[i]));
            // } end try (handler L83)
            goto L127;
          L83: // catch (e:Error)
            var _local3 = $catchscope;
            $catchscope.slot1 = $exception;
            var _local4 = loadedCount;
            var _local5 = loadedCount - 1;
            _local4.loadedCount = _local5;
            goto L251;
          L127:
            splashAr[i].alpha = 0;
            splashAr[i].touchable = false;
            splashAr[i].scaleX = 1024 / splashAr[i].width;
            splashAr[i].scaleY = 600 / splashAr[i].height;
            splashAr[i].addEventListener(TouchEvent.TOUCH, onSplashTouch);
            _global._gameS.addChild(splashAr[i]);
          L251:
            _local3 = i;
            _local4 = i + 1;
            _local3.i = _local4;
          L272:
            if (i < loadedCount) goto L54;
            showSplash();
            onLoadProgress = false;
            ready = true;
          L303:
        });
        // } end try (handler L365)
      L361:
        goto L388;
      L365: // catch (e:Error)
        var _local2 = $catchscope;
        $catchscope.slot1 = $exception;
        onSplashComplete();
      L388:
    }

    function onSplashTouch(arg1:TouchEvent):void {
        if (!arg1.getTouch(this._global._game, "began")) goto L48;
        TweenLite.killDelayedCallsTo(this.hideSplash);
        TweenLite.killTweensOf(arg1.currentTarget);
        this.hideSplash();
      L48:
    }

    function showSplash():void {
        if (this.showCount != this.loadedCount) goto L24;
        this.onSplashComplete();
        return;
      L24:
        this.splashAr[this.showCount].touchable = true;
        var _local2 = this;
        var $t0 = this.showCount;
        var _local3 = $t0 + 1;
        _local2.showCount = _local3;
        TweenLite.to(this.splashAr[$t0], this.splashFadeDuration, {"alpha": 1, "onComplete": function():void {
            TweenLite.delayedCall(splashDuration, hideSplash);
        }});
    }

    function hideSplash():void {
        this.splashAr[this.showCount - 1].touchable = false;
        this.splashAr[this.showCount - 1].removeEventListener(TouchEvent.TOUCH, this.onSplashTouch);
        TweenLite.to(this.splashAr[this.showCount - 1], this.splashFadeDuration, {"alpha": 0, "onComplete": function():void {
            showSplash();
        }});
    }

    override function Kill():void {
        if (!this.onLoadProgress) goto L11;
        return;
      L11:
        goto L29;
      L15:
        this.splashAr.pop().removeFromParent(true);
      L29:
        if (this.splashAr.length) goto L15;
        goto L57;
      L44:
        this.allBitmapData.pop().dispose();
      L57:
        if (this.allBitmapData.length) goto L44;
        this.splashXML = null;
        this._global.assetManager.purge();
        ready = false;
    }

    function onSplashComplete():void {
        this.logoMC = new this.logoSplash();
        this.logoMC.scaleX = this._global._stage.stageWidth / 1024;
        this.logoMC.scaleY = this._global._stage.stageHeight / 600;
        this._global._stage.addChild(this.logoMC);
        this.logoMC.addEventListener(Event.ENTER_FRAME, this.checkFrame);
    }

    function checkFrame(arg1:Event):void {
        if (this.logoMC.currentFrame != this.logoMC.totalFrames) goto L76;
        this._global._game.fadeToScreen(this._global._mainMenuScreen);
        this._global._stage.removeChild(this.logoMC);
        this.logoMC.removeEventListener(Event.ENTER_FRAME, this.checkFrame);
        this.logoMC = null;
      L76:
    }
}
}
