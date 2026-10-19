package {
class Main extends MovieClip {
    var _global:Global;

    function Main():void {
        super();
        if (!stage) goto L21;
        this.init();
        goto L38;
      L21:
        addEventListener(Event.ADDED_TO_STAGE, this.init);
      L38:
    }

    function init(arg1:Event = null):void {
        removeEventListener(Event.ADDED_TO_STAGE, this.init);
        stage.scaleMode = StageScaleMode.NO_SCALE;
        stage.align = StageAlign.TOP_LEFT;
        Settings.initSetting();
        var _local2 = false;
        var _local3 = false;
        var _local4 = false;
        var _local5 = false;
        if (Settings.textureSize != "hd") goto L79;
        _local2 = true;
        goto L145;
      L79:
        if (Settings.textureSize != "sd") goto L98;
        _local2 = false;
        goto L145;
      L98:
        if (Settings.textureSize != "all") goto L145;
        if (!(stage.stageWidth <= 1024)) goto L130;
        _local2 = false;
        goto L145;
      L130:
        if (!(stage.stageWidth > 1024)) goto L145;
        _local2 = true;
      L145:
        if (Settings.embedAssets != "true") goto L160;
        _local3 = true;
      L160:
        if (Settings.encryptAssets != "true") goto L176;
        _local4 = true;
      L176:
        if (Settings.showStats != "true") goto L192;
        _local5 = true;
      L192:
        Starling.multitouchEnabled = true;
        Starling.handleLostContext = true;
        this._global = Global.getInstance();
        this._global.init(_local2, _local3, _local4);
        this._global._stage = stage;
        var _local6 = new Rectangle(0, 0, 1024, 600);
        this._global._Starling = new Starling(Game, stage, _local6, null, "auto", "auto");
        Starling.current.start();
        stage.addEventListener(ResizeEvent.RESIZE, this.resizeStage);
        var $t0 = new Rectangle();
        var _local7 = $t0;
        $t0.width = stage.stageWidth;
        _local7.height = stage.stageHeight;
        Starling.current.viewPort = _local7;
        Starling.current.showStats = _local5;
    }

    function resizeStage(arg1:Event):void {
        if (!TGlobalClass.minimized) goto L13;
        return;
      L13:
        this._global.resizingStage = true;
        var _local2 = new Rectangle();
        _local2.width = stage.stageWidth;
        _local2.height = stage.stageHeight;
        Starling.current.viewPort = _local2;
    }
}
}
