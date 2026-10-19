package {
class ConstClass {
    static const WESTERN_DISH:String = "W";
    static const CHINESE_DISH:String = "C";
    static const MID_EAST_DISH:String = "ME";
    static const WESTERN_DISH_COUNT:int = 6;
    static const CHINESE_DISH_COUNT:int = 6;
    static const MID_EAST_DISH_COUNT:int = 6;
    static const INGREDIENT_COUNT:int = 13;
    static const GAME_TYPE_M3:String = "M3";
    static const GAME_TYPE_HO:String = "HO";
    static const GAME_TYPE_SPECIAL_M3:String = "M3S";
    static const GAME_TYPE_DESSERT_MAKING:String = "DM";

    // static initializer
    static {
        WESTERN_DISH = "W";
        CHINESE_DISH = "C";
        MID_EAST_DISH = "ME";
        WESTERN_DISH_COUNT = 6;
        CHINESE_DISH_COUNT = 6;
        MID_EAST_DISH_COUNT = 6;
        INGREDIENT_COUNT = 13;
        GAME_TYPE_M3 = "M3";
        GAME_TYPE_HO = "HO";
        GAME_TYPE_SPECIAL_M3 = "M3S";
        GAME_TYPE_DESSERT_MAKING = "DM";
    }

    function ConstClass():* {
        super();
    }
}
}
