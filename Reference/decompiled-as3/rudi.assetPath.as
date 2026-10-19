package rudi {
class assetPath {
    static var imageAr:Array;
    static var xmlAr:Array;
    static var soundAr:Array;
    static var soundNameAr:Array;
    static var skeletonAr:Array;
    static var skeletonXmlAr:Array;
    static var skeletonTextureAr:Array;
    static var imageArCnt:int = 0;
    static var xmlArCnt:int = 0;
    static var soundArCnt:int = 0;
    static var soundNameArCnt:int = 0;
    static var skeletonArCnt:int = 0;
    static var skeletonXmlArCnt:int = 0;
    static var skeletonTextureArCnt:int = 0;
    static var graphicsPath:String;
    static var soundsPath:String;
    static var skeletonPath:String;
    static var tName:String;

    // static initializer
    static {
        imageAr = [];
        xmlAr = [];
        soundAr = [];
        soundNameAr = [];
        skeletonAr = [];
        skeletonXmlAr = [];
        skeletonTextureAr = [];
        imageArCnt = 0;
        xmlArCnt = 0;
        soundArCnt = 0;
        soundNameArCnt = 0;
        skeletonArCnt = 0;
        skeletonXmlArCnt = 0;
        skeletonTextureArCnt = 0;
    }

    function assetPath():* {
        super();
    }

    static function start(arg1:String, arg2:String):void {
        graphicsPath = arg1;
        soundsPath = arg2;
    }

    static function startSkeleton(arg1:String):void {
        skeletonPath = arg1;
    }

    static function getGraphics():String {
        var $t0 = imageArCnt < imageAr.length;
        $s0 = $t0;
        if (!$t0) goto L28;
        $s0 = !(imageAr.length == 0);
      L28:
        if (!$s0) goto L117;
        imageArCnt = imageArCnt + 1;
        if (!Global.getInstance().assetEncrypt) goto L93;
        tName = maxCrypto.getInstance().StripExt(imageAr[imageArCnt - 1]) + ".OrigamingI";
        goto L109;
      L93:
        tName = imageAr[imageArCnt - 1];
      L109:
        return graphicsPath + tName;
      L117:
        return "";
    }

    static function getSkeleton():String {
        var $t0 = skeletonArCnt < skeletonAr.length;
        $s0 = $t0;
        if (!$t0) goto L28;
        $s0 = !(skeletonAr.length == 0);
      L28:
        if (!$s0) goto L117;
        skeletonArCnt = skeletonArCnt + 1;
        if (!Global.getInstance().assetEncrypt) goto L93;
        tName = maxCrypto.getInstance().StripExt(skeletonAr[skeletonArCnt - 1]) + ".OrigamingSk";
        goto L109;
      L93:
        tName = skeletonAr[skeletonArCnt - 1];
      L109:
        return skeletonPath + tName;
      L117:
        return "";
    }

    static function getSkeletonTextureXml():String {
        var $t0 = skeletonTextureArCnt < skeletonTextureAr.length;
        $s0 = $t0;
        if (!$t0) goto L28;
        $s0 = !(skeletonTextureAr.length == 0);
      L28:
        if (!$s0) goto L117;
        skeletonTextureArCnt = skeletonTextureArCnt + 1;
        if (!Global.getInstance().assetEncrypt) goto L93;
        tName = maxCrypto.getInstance().StripExt(skeletonTextureAr[skeletonTextureArCnt - 1]) + ".OrigamingSkTX";
        goto L109;
      L93:
        tName = skeletonTextureAr[skeletonTextureArCnt - 1];
      L109:
        return skeletonPath + tName;
      L117:
        return "";
    }

    static function getSkeletonXml():String {
        var $t0 = skeletonXmlArCnt < skeletonXmlAr.length;
        $s0 = $t0;
        if (!$t0) goto L28;
        $s0 = !(skeletonXmlAr.length == 0);
      L28:
        if (!$s0) goto L117;
        skeletonXmlArCnt = skeletonXmlArCnt + 1;
        if (!Global.getInstance().assetEncrypt) goto L93;
        tName = maxCrypto.getInstance().StripExt(skeletonXmlAr[skeletonXmlArCnt - 1]) + ".OrigamingSkX";
        goto L109;
      L93:
        tName = skeletonXmlAr[skeletonXmlArCnt - 1];
      L109:
        return skeletonPath + tName;
      L117:
        return "";
    }

    static function getXml():String {
        var $t0 = xmlArCnt < xmlAr.length;
        $s0 = $t0;
        if (!$t0) goto L28;
        $s0 = !(xmlAr.length == 0);
      L28:
        if (!$s0) goto L117;
        xmlArCnt = xmlArCnt + 1;
        if (!Global.getInstance().assetEncrypt) goto L93;
        tName = maxCrypto.getInstance().StripExt(xmlAr[xmlArCnt - 1]) + ".OrigamingX";
        goto L109;
      L93:
        tName = xmlAr[xmlArCnt - 1];
      L109:
        return graphicsPath + tName;
      L117:
        return "";
    }

    static function getSounds():String {
        var $t0 = soundArCnt < soundAr.length;
        $s0 = $t0;
        if (!$t0) goto L28;
        $s0 = !(soundAr.length == 0);
      L28:
        if (!$s0) goto L117;
        soundArCnt = soundArCnt + 1;
        if (!Global.getInstance().assetEncrypt) goto L93;
        tName = maxCrypto.getInstance().StripExt(soundAr[soundArCnt - 1]) + ".OrigamingS";
        goto L109;
      L93:
        tName = soundAr[soundArCnt - 1];
      L109:
        return soundsPath + tName;
      L117:
        return "";
    }

    static function getSoundsName():String {
        var $t0 = soundNameArCnt < soundNameAr.length;
        $s0 = $t0;
        if (!$t0) goto L28;
        $s0 = !(soundNameAr.length == 0);
      L28:
        if (!$s0) goto L55;
        soundNameArCnt = soundNameArCnt + 1;
        return soundNameAr[soundNameArCnt - 1];
      L55:
        return "";
    }

    static function reset():void {
        imageArCnt = 0;
        xmlArCnt = 0;
        soundArCnt = 0;
        soundNameArCnt = 0;
        skeletonArCnt = 0;
        skeletonTextureArCnt = 0;
        skeletonXmlArCnt = 0;
        imageAr = [];
        xmlAr = [];
        soundAr = [];
        soundNameAr = [];
        skeletonAr = [];
        skeletonTextureAr = [];
        skeletonXmlAr = [];
    }
}
}
