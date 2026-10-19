package {
class TWindowState {
    static const EXIT_WINDOW:int = 1;
    static const CHANGE_PLAYER:int = 2;
    static const DELETE_PLAYER:int = 3;
    static const CREATE_PLAYER:int = 4;
    static const SAME_NAME_WARNING:int = 5;
    static const NEW_GAME:int = 6;
    static const CONT_OR_NEW:int = 7;
    static const NEW_GAME_CONFIRM:int = 8;
    static const PAUSE_GAME:int = 101;
    static const STORY_LEVELCOMPLETE:int = 102;
    static const STORY_COMPLETE_ALL:int = 103;
    static const STORY_LEVELFAILED:int = 104;
    static const HO_LEVEL_COMPLETE:int = 105;
    static const HO_LEVEL_FAILED:int = 199;
    static const DESSERT_LEVEL_COMPLETE:int = 106;
    static const DESSERT_LEVEL_FAILED:int = 107;
    static const DESSERT_LEVEL_COMPLETE_EXTRAS:int = 108;
    static const DESSERT_LEVEL_FAILED_EXTRAS:int = 109;
    static const HO_LEVEL_COMPLETE_EXTRAS:int = 110;
    static const HO_LEVEL_FAILED_EXTRAS:int = 111;
    static const HO_LEVEL_COMPLETE_EXTRAS_ALL:int = 112;
    static const CONFIRM_MAINMENU:int = 151;
    static const CONFIRM_KITCHEN:int = 152;
    static const EXTRAS_MENU:int = 153;
    static const SHOW_EXPERT_NEEDED_INFO:int = 154;
    static const UNLOCK_FULL_VERSION:int = 155;
    static const SHOW_FREE_COINS:int = 156;
    static const VIDEO_ADS_NOT_AVAILABLE:int = 157;
    static const COINS_REWARDED:int = 158;

    // static initializer
    static {
        EXIT_WINDOW = 1;
        CHANGE_PLAYER = 2;
        DELETE_PLAYER = 3;
        CREATE_PLAYER = 4;
        SAME_NAME_WARNING = 5;
        NEW_GAME = 6;
        CONT_OR_NEW = 7;
        NEW_GAME_CONFIRM = 8;
        PAUSE_GAME = 101;
        STORY_LEVELCOMPLETE = 102;
        STORY_COMPLETE_ALL = 103;
        STORY_LEVELFAILED = 104;
        HO_LEVEL_COMPLETE = 105;
        HO_LEVEL_FAILED = 199;
        DESSERT_LEVEL_COMPLETE = 106;
        DESSERT_LEVEL_FAILED = 107;
        DESSERT_LEVEL_COMPLETE_EXTRAS = 108;
        DESSERT_LEVEL_FAILED_EXTRAS = 109;
        HO_LEVEL_COMPLETE_EXTRAS = 110;
        HO_LEVEL_FAILED_EXTRAS = 111;
        HO_LEVEL_COMPLETE_EXTRAS_ALL = 112;
        CONFIRM_MAINMENU = 151;
        CONFIRM_KITCHEN = 152;
        EXTRAS_MENU = 153;
        SHOW_EXPERT_NEEDED_INFO = 154;
        UNLOCK_FULL_VERSION = 155;
        SHOW_FREE_COINS = 156;
        VIDEO_ADS_NOT_AVAILABLE = 157;
        COINS_REWARDED = 158;
    }

    function TWindowState():* {
        super();
    }
}
}
