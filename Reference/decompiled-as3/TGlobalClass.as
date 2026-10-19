package {
class TGlobalClass {
    static var _window:TWindow;
    static var _windowDisplay:TWindowDisplay;
    static var totalCust:int;
    static var currentCust:int;
    static var customerFocus:int;
    static var daysEarningPrev:int;
    static var coinsGot:int;
    static var gameMode:uint;
    static var loadGame:Boolean;
    static var extrasMode:Boolean;
    static var startedStrategy:Boolean;
    static var startedClassic:Boolean;
    static var startedIngredient:Boolean;
    static var startedEndless:Boolean;
    static var startedTarget:Boolean;
    static var gameModeName:String;
    static var musicPlayingIdx:int;
    static var isPlayingSpecialLevel:Boolean;
    static var isPlayingSFX:Boolean;
    static var VIPServedRating:Number;
    static var currentPlayedLevel:uint;
    static var lastKitchenWindows:Boolean;
    static var currentRegionDish:String;
    static var dishRegionWestern:Vector.<int>;
    static var dishRegionChinese:Vector.<int>;
    static var dishRegionMidEast:Vector.<int>;
    static var westernUpgradeCost:Vector.<Vector.<int>>;
    static var chineseUpgradeCost:Vector.<Vector.<int>>;
    static var mideastUpgradeCost:Vector.<Vector.<int>>;
    static var ingredientCost:Vector.<Vector.<int>>;
    static var tempCustUse:Vector.<int>;
    static var MAX_DISH_LEVEL:int = 4;
    static var MAX_HARDWARE_LEVEL:int = 6;
    static var MAX_INGREDIENT_LEVEL:int = 11;
    static var TOTAL_INGREDIENT:int = 13;
    static var TOTAL_HARDWARE:int = 3;
    static var TOTAL_WESTERN_DISH:int = 6;
    static var TOTAL_CHINESE_DISH:int = 6;
    static var TOTAL_MIDEAST_DISH:int = 6;
    static var TOTAL_DAYS:int = 91;
    static var TILE_NEEDED_FOR_TORCH:int = 50;
    static const UNLOCK_TORCH_AT:int = 26;
    static const UNLOCK_FROZEN_AT:int = 25;
    static const UNLOCK_COOKIES_AT:int = 30;
    static const UNLOCK_CHINESE_DISH_AT:int = 7;
    static const UNLOCK_MID_EAST_DISH_AT:int = 21;
    static const UNLOCK_RECIPE_SHOP:int = 2;
    static const UNLOCK_INGREDIENT_SHOP:int = 8;
    static const UNLOCK_HARDWARE_SHOP:int = 34;
    static const UNLOCK_MATCH_BONUS:int = 15;
    static const UNLOCK_BEVERAGE_AT:int = 11;
    static const UNLOCK_FRIES_AT:int = 16;
    static const MATCH_3_SPECIAL_EVERY:int = 7;
    static var showLevelSelect:Boolean;
    static var customerXML:XML;
    static var levelSetXML:XML;
    static var levelEventXML:XML;
    static var dialogueXML:XML;
    static var currentNotifWindow:int;
    static var cookiesUsed:int;
    static var juiceServed:int;
    static var friesServed:int;
    static var PublisherName:String = "";
    static var FreeVersion:Boolean = false;
    static var EnableLeaderboardBtn:Boolean = false;
    static var EnableQuitButton:Boolean = true;
    static var EnableMoreGames:Boolean = false;
    static var EnableFoundItemParticle:Boolean;
    static var EnableHintParticle:Boolean;
    static var EnableCrossPromo:Boolean = false;
    static var EnableFreeCoins:Boolean = false;
    static var VideoReady:Boolean = true;
    static var deactivate:Boolean;
    static var dekstopVersion:Boolean = true;
    static var minimized:Boolean;

    // static initializer
    static {
        MAX_DISH_LEVEL = 4;
        MAX_HARDWARE_LEVEL = 6;
        MAX_INGREDIENT_LEVEL = 11;
        TOTAL_INGREDIENT = 13;
        TOTAL_HARDWARE = 3;
        TOTAL_WESTERN_DISH = 6;
        TOTAL_CHINESE_DISH = 6;
        TOTAL_MIDEAST_DISH = 6;
        TOTAL_DAYS = 91;
        TILE_NEEDED_FOR_TORCH = 50;
        UNLOCK_TORCH_AT = 26;
        UNLOCK_FROZEN_AT = 25;
        UNLOCK_COOKIES_AT = 30;
        UNLOCK_CHINESE_DISH_AT = 7;
        UNLOCK_MID_EAST_DISH_AT = 21;
        UNLOCK_RECIPE_SHOP = 2;
        UNLOCK_INGREDIENT_SHOP = 8;
        UNLOCK_HARDWARE_SHOP = 34;
        UNLOCK_MATCH_BONUS = 15;
        UNLOCK_BEVERAGE_AT = 11;
        UNLOCK_FRIES_AT = 16;
        MATCH_3_SPECIAL_EVERY = 7;
        PublisherName = "";
        FreeVersion = false;
        EnableLeaderboardBtn = false;
        EnableQuitButton = true;
        EnableMoreGames = false;
        EnableCrossPromo = false;
        EnableFreeCoins = false;
        VideoReady = true;
        dekstopVersion = true;
    }

    function TGlobalClass():* {
        super();
    }

    static function readByteArrayToBitmap(arg1:ByteArray):Bitmap {
        var _local2 = "crazy_face";
        var _local3 = new ARC4(Hex.toArray(Hex.fromString(_local2)));
        _local3.decrypt(arg1);
        arg1.uncompress();
        var _local4 = int(arg1.readUnsignedInt());
        var _local5 = int((arg1.length - 4) / 4 / _local4);
        var $t0 = new BitmapData(_local4, _local5, true, 0);
        var _local6 = $t0;
        $t0.setPixels($t0.rect, arg1);
        var $t1 = new Bitmap(_local6);
        var _local7 = $t1;
        return $t1;
    }

    static function RandomRange(arg1:int, arg2:int):int {
        return Math.floor(Math.random() * (1 + arg2 - arg1)) + arg1;
    }

    static function addCommas(arg1:Number):String {
        var _local5 = NaN;
        var _local6 = null;
        var _local7 = NaN;
        var _local2 = "";
        if (!(arg1 < 0)) goto L36;
        _local2 = "-";
        arg1 = Number(Math.abs(arg1));
      L36:
        var _local3 = String(arg1);
        var $t0 = _local3.split(new RegExp("\\."));
        var _local4 = $t0;
        _local3 = $t0[0];
        if (!(_local3.length > 3)) goto L252;
        _local5 = Number(_local3.length % 3);
        _local6 = _local3.substr(0, _local5);
        _local7 = Number(_local5);
        goto L168;
      L112:
        var $t1 = _local5 == 0;
        $s0 = _local6;
        $s1 = $t1;
        if (!$t1) goto L131;
        $s1 = _local7 == 0;
      L131:
        if (!$s1) goto L142;
        $s1 = "";
        goto L146;
      L142:
        $s1 = ",";
      L146:
        _local6 = $s0 + ($s1 + _local3.substr(_local7, 3));
        _local7 = Number(_local7 + 3);
      L168:
        if (_local7 < _local3.length) goto L112;
        if (!(_local4.length > 1)) goto L247;
        if (_local4[1].length != 1) goto L226;
        return _local2 + _local6 + "." + _local4[1] + "0";
      L226:
        return _local2 + _local6 + "." + _local4[1];
      L247:
        return _local2 + _local6;
      L252:
        if (!(_local4.length > 1)) goto L319;
        if (_local4[1].length != 1) goto L299;
        return _local2 + _local3 + "." + _local4[1] + "0";
      L299:
        return _local2 + _local3 + "." + _local4[1];
      L319:
        return _local2 + _local3;
    }
}
}
