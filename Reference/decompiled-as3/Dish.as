package {
class Dish {
    static const VEGGIE_CROISSANT:int = 1;
    static const JUICER:int = 0;
    static const FRYER:int = 1;
    static const COOKIES:int = 2;
    var dishXML:XML;

    // static initializer
    static {
        VEGGIE_CROISSANT = 1;
        JUICER = 0;
        FRYER = 1;
        COOKIES = 2;
    }

    function Dish():* {
        super();
    }

    function init():void {
        var _local1 = new URLRequest("data/Dish.xml");
        var _local2 = new URLLoader();
        _local2.dataFormat = URLLoaderDataFormat.TEXT;
        _local2.addEventListener(Event.COMPLETE, this.dishLoadComplete);
        _local2.load(_local1);
        TGlobalClass.dishRegionWestern = new Vector.<int>();
        TGlobalClass.dishRegionChinese = new Vector.<int>();
        TGlobalClass.dishRegionMidEast = new Vector.<int>();
        TGlobalClass.westernUpgradeCost = new Vector.<Vector.<int>>(ConstClass.WESTERN_DISH_COUNT, true);
        TGlobalClass.chineseUpgradeCost = new Vector.<Vector.<int>>(ConstClass.CHINESE_DISH_COUNT, true);
        TGlobalClass.mideastUpgradeCost = new Vector.<Vector.<int>>(ConstClass.MID_EAST_DISH_COUNT, true);
        TGlobalClass.ingredientCost = new Vector.<Vector.<int>>(ConstClass.INGREDIENT_COUNT, true);
    }

    function dishLoadComplete(arg1:Event):void {
        var _local2 = 0;
        var _local3 = null;
        var _local4 = null;
        var _local5 = null;
        var _local7 = 0;
        this.dishXML = new XML(arg1.target.data);
        var _local6 = 0;
        goto L434;
      L47:
        _local3 = this.dishXML.dish[_local6].attribute("region");
        if (_local3 != "western") goto L93;
        TGlobalClass.dishRegionWestern.push(_local6);
        goto L129;
      L93:
        if (_local3 != "chinese") goto L117;
        TGlobalClass.dishRegionChinese.push(_local6);
        goto L129;
      L117:
        TGlobalClass.dishRegionMidEast.push(_local6);
      L129:
        var $t0 = this.dishXML.dish[_local6].attribute("upgradeCost");
        _local4 = $t0;
        _local5 = $t0.split(",");
        if (_local3 != "western") goto L254;
        TGlobalClass.westernUpgradeCost[_local2] = new Vector.<int>();
        _local7 = 0;
        goto L223;
      L199:
        TGlobalClass.westernUpgradeCost[_local2].push(_local5[_local7]);
        _local7++;
      L223:
        if (_local7 < _local5.length) goto L199;
        _local2++;
        if (_local2 != ConstClass.WESTERN_DISH_COUNT) goto L250;
        _local2 = 0;
      L250:
        goto L432;
      L254:
        if (_local3 != "mid_east") goto L345;
        TGlobalClass.mideastUpgradeCost[_local2] = new Vector.<int>();
        _local7 = 0;
        goto L314;
      L290:
        TGlobalClass.mideastUpgradeCost[_local2].push(_local5[_local7]);
        _local7++;
      L314:
        if (_local7 < _local5.length) goto L290;
        _local2++;
        if (_local2 != ConstClass.MID_EAST_DISH_COUNT) goto L341;
        _local2 = 0;
      L341:
        goto L432;
      L345:
        if (_local3 != "chinese") goto L432;
        TGlobalClass.chineseUpgradeCost[_local2] = new Vector.<int>();
        _local7 = 0;
        goto L405;
      L381:
        TGlobalClass.chineseUpgradeCost[_local2].push(_local5[_local7]);
        _local7++;
      L405:
        if (_local7 < _local5.length) goto L381;
        _local2++;
        if (_local2 != ConstClass.CHINESE_DISH_COUNT) goto L432;
        _local2 = 0;
      L432:
        _local6++;
      L434:
        if (_local6 < this.dishXML.dish.length()) goto L47;
        _local6 = 0;
        goto L561;
      L459:
        TGlobalClass.ingredientCost[_local6] = new Vector.<int>();
        var $t1 = this.dishXML.ingredient[_local6].attribute("upgradeCost");
        _local4 = $t1;
        _local5 = $t1.split(",");
        _local7 = 0;
        goto L548;
      L523:
        TGlobalClass.ingredientCost[_local6].push(_local5[_local7]);
        _local7++;
      L548:
        if (_local7 < _local5.length) goto L523;
        _local6++;
      L561:
        if (_local6 < this.dishXML.ingredient.length()) goto L459;
    }

    function getDishIngredientType(arg1:int):Vector.<int> {
        var _local2 = new Vector.<int>();
        var _local3 = this.dishXML.dish[arg1].attribute("ingredient");
        _local2 = Vector.<int>(_local3.split(","));
        return _local2;
    }

    function getDishTarget(arg1:int):Vector.<int> {
        var _local2 = new Vector.<int>();
        var _local3 = this.dishXML.dish[arg1].attribute("defaultTarget");
        _local2 = Vector.<int>(_local3.split(","));
        return _local2;
    }

    function getDishDefaultPrice(arg1:int):int {
        var _local2 = int(this.dishXML.dish[arg1].attribute("defaultPrice"));
        return _local2;
    }

    function getPriceIncrease(arg1:int):int {
        var _local2 = int(this.dishXML.dish[arg1].attribute("priceIncrease"));
        return _local2;
    }

    function getIngredientPriceIncrease(arg1:int):int {
        var _local2 = int(this.dishXML.ingredient[arg1].attribute("priceIncrease"));
        return _local2;
    }

    function getHardwareUpgradecost(arg1:int):Vector.<int> {
        var _local2 = new Vector.<int>();
        var _local3 = this.dishXML.hardware[arg1].attribute("upgradeCost");
        _local2 = Vector.<int>(_local3.split(","));
        return _local2;
    }

    function getHardwareDefaultPrice(arg1:int):int {
        var _local2 = int(this.dishXML.hardware[arg1].attribute("defaultPrice"));
        return _local2;
    }

    function getHardwarePriceIncrease(arg1:int):int {
        var _local2 = int(this.dishXML.hardware[arg1].attribute("priceIncrease"));
        return _local2;
    }

    function getHardwareDefaultTarget(arg1:int):int {
        var _local2 = int(this.dishXML.hardware[arg1].attribute("startingTarget"));
        return _local2;
    }

    function getHardwareReduction(arg1:int):int {
        var _local2 = int(this.dishXML.hardware[arg1].attribute("Reduction"));
        return _local2;
    }
}
}
