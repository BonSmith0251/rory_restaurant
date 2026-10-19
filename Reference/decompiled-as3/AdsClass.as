package {
class AdsClass {
    static var _global:Global;
    static var redQuad:Quad;
    static var adsInfo:ExtendTextField;
    static var onCheckConnection:Boolean;
    static var connectionProgressCount:int;
    static var loader:URLLoader;
    static var displayingAds:Boolean = false;
    static const PING_WEBSITE_URL:String = "http://www.google.com";
    static const PING_DURATION:int = 240;
    static var monitor:URLMonitor;

    // static initializer
    static {
        displayingAds = false;
        PING_WEBSITE_URL = "http://www.google.com";
        PING_DURATION = 240;
    }

    function AdsClass():* {
        super();
    }

    static function initAds(arg1:int, arg2:int):void {
        _global = Global.getInstance();
        displayingAds = false;
        redQuad = new Quad(300, 250);
        redQuad.color = Color.RED;
        redQuad.visible = false;
        _global._gameS.addChild(redQuad);
        adsInfo = new ExtendTextField(350, 40, "Check out our other games:", "bromine", 18, 0);
        adsInfo.visible = false;
        adsInfo.pivotX = 175;
        adsInfo.pivotY = 20;
        adsInfo.hAlign = "center";
        _global._gameS.addChild(adsInfo);
        redQuad.x = arg1 - 150;
        redQuad.y = arg2 - 125;
        adsInfo.x = arg1;
        adsInfo.y = arg2 - 160;
    }

    static function checkConnection():void {
        if (TGlobalClass.EnableCrossPromo) goto L13;
        return;
      L13:
        if (onCheckConnection) goto L96;
        onCheckConnection = true;
        connectionProgressCount = 0;
        loader = new URLLoader();
        loader.addEventListener(Event.COMPLETE, onSuccess);
        loader.addEventListener(IOErrorEvent.IO_ERROR, onFailure);
        loader.load(new URLRequest(PING_WEBSITE_URL));
      L96:
    }

    static function checkTimeOut():void {
        if (!onCheckConnection) goto L83;
        var _local1 = connectionProgressCount;
        var _local2 = connectionProgressCount + 1;
        _local1.connectionProgressCount = _local2;
        if (!(connectionProgressCount > PING_DURATION)) goto L83;
        onCheckConnection = false;
        if (!loader) goto L83;
        loader.removeEventListener(Event.COMPLETE, onSuccess);
        loader.removeEventListener(IOErrorEvent.IO_ERROR, onFailure);
      L83:
    }

    static function forceCancle():void {
        onCheckConnection = false;
        if (!loader) goto L47;
        loader.removeEventListener(Event.COMPLETE, onSuccess);
        loader.removeEventListener(IOErrorEvent.IO_ERROR, onFailure);
      L47:
    }

    static function onFailure(arg1:Event):void {
        onCheckConnection = false;
        loader.removeEventListener(Event.COMPLETE, onSuccess);
        loader.removeEventListener(IOErrorEvent.IO_ERROR, onFailure);
    }

    static function onSuccess(arg1:Event):void {
        onCheckConnection = false;
        loader.removeEventListener(Event.COMPLETE, onSuccess);
        loader.removeEventListener(IOErrorEvent.IO_ERROR, onFailure);
        ShowAds();
    }

    static function kill():void {
        RemoveAds();
        redQuad.removeFromParent(true);
        redQuad = null;
        adsInfo.removeFromParent(true);
        adsInfo = null;
        if (!loader) goto L84;
        loader.removeEventListener(Event.COMPLETE, onSuccess);
        loader.removeEventListener(IOErrorEvent.IO_ERROR, onFailure);
        loader = null;
      L84:
    }

    static function ShowAds():void {
        if (displayingAds) goto L30;
        displayingAds = true;
        redQuad.visible = true;
        adsInfo.visible = true;
      L30:
    }

    static function RemoveAds():void {
        if (!displayingAds) goto L30;
        displayingAds = false;
        redQuad.visible = false;
        adsInfo.visible = false;
      L30:
    }
}
}
