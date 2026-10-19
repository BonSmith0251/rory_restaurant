package {
class Settings {
    static const settingXm:Class;
    static var settingXML:XML;
    static var productLength:int = 7;
    static var Name:String;
    static var Version:String;
    static var packageName:String;
    static var textureSize:String;
    static var embedAssets:String;
    static var encryptAssets:String;
    static var handleDeactivate:String;
    static var publishingLength:int = 4;
    static var platform:String;
    static var publisher:String;
    static var networkAds:String;
    static var externalAds:String;
    static var debugLength:int = 1;
    static var showStats:String;
    static var supportLength:int = 3;
    static var devName:String;
    static var devWeb:String;
    static var devSupport:String;

    // static initializer
    static {
        settingXm = Settings_settingXm;
        productLength = 7;
        publishingLength = 4;
        debugLength = 1;
        supportLength = 3;
    }

    function Settings():* {
        super();
    }

    static function initSetting():void {
        var _local1 = new settingXm() as ByteArray;
        var _local2 = new XML(_local1.readUTFBytes(_local1.length));
        settingXML = new XML(_local2);
        readXML();
    }

    static function readXML():void {
        Name = settingXML.children()[0].children()[0];
        Version = settingXML.children()[0].children()[1];
        packageName = settingXML.children()[0].children()[2];
        textureSize = settingXML.children()[0].children()[3];
        embedAssets = settingXML.children()[0].children()[4];
        encryptAssets = settingXML.children()[0].children()[5];
        handleDeactivate = settingXML.children()[0].children()[6];
        platform = settingXML.children()[1].children()[0];
        publisher = settingXML.children()[1].children()[1];
        networkAds = settingXML.children()[1].children()[2];
        externalAds = settingXML.children()[1].children()[3];
        showStats = settingXML.children()[2].children()[0];
        devName = settingXML.children()[3].children()[0];
        devWeb = settingXML.children()[3].children()[1];
        devSupport = settingXML.children()[3].children()[2];
    }

    static function onXMLLoad(arg1:Event):void {
        settingXML = new XML(arg1.target.data);
    }
}
}
