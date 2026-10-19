package lib {
class GALib {
    static var secretKey:String = "cfdab1cab280af149c8c61cb04720cf0fc029748";
    static var gameKey:String = "747fb6ec5c8bcad8af22c1a1d148c5f7";
    static var buildName:String;
    static const EVENT_SESSION_BEGIN:String = "Session:Begin";
    static const EVENT_CHECK_CREDITS:String = "Game:CheckCredits";
    static const EVENT_PRESS_HINT:String = "Game:PressHint";
    static const EVENT_SUBMIT_SCORE:String = "Game:SubmitScore";
    static const EVENT_START_LEVEL_STORY:String = "Game:StartLevelStory";
    static const EVENT_COMPLETE_STORY:String = "Game:CompleteStory";
    static const EVENT_LOSE_STORY:String = "Game:LoseStory";
    static const EVENT_START_LEVEL_EXTRAS:String = "Game:StartLevelExtras";
    static const EVENT_COMPLETE_EXTRAS:String = "Game:CompleteExtras";
    static const EVENT_LOSE_EXTRAS:String = "Game:LoseExtras";
    static const ENTERED_ENDING_SCREEN:String = "EnteredEndingScreen";
    static const ENTERED_ACHIEVEMENT_ROOM:String = "EnteredAchievementRoom";
    static const COMPLETED_AN_ACHIEVEMENT:String = "CompletedAnAchievement";
    static const BUY_OR_UPGRADE_RECIPE:String = "BuyOrUpgradeRecipe";
    static const EVENT_UNLOCK_FULL_VERSION:String = "Game:UnlockFullVersion";
    static var initGALib:Boolean = false;
    static var _analyticsState:SharedObject;

    // static initializer
    static {
        secretKey = "cfdab1cab280af149c8c61cb04720cf0fc029748";
        gameKey = "747fb6ec5c8bcad8af22c1a1d148c5f7";
        buildName = Settings.Version;
        EVENT_SESSION_BEGIN = "Session:Begin";
        EVENT_CHECK_CREDITS = "Game:CheckCredits";
        EVENT_PRESS_HINT = "Game:PressHint";
        EVENT_SUBMIT_SCORE = "Game:SubmitScore";
        EVENT_START_LEVEL_STORY = "Game:StartLevelStory";
        EVENT_COMPLETE_STORY = "Game:CompleteStory";
        EVENT_LOSE_STORY = "Game:LoseStory";
        EVENT_START_LEVEL_EXTRAS = "Game:StartLevelExtras";
        EVENT_COMPLETE_EXTRAS = "Game:CompleteExtras";
        EVENT_LOSE_EXTRAS = "Game:LoseExtras";
        ENTERED_ENDING_SCREEN = "EnteredEndingScreen";
        ENTERED_ACHIEVEMENT_ROOM = "EnteredAchievementRoom";
        COMPLETED_AN_ACHIEVEMENT = "CompletedAnAchievement";
        BUY_OR_UPGRADE_RECIPE = "BuyOrUpgradeRecipe";
        EVENT_UNLOCK_FULL_VERSION = "Game:UnlockFullVersion";
        initGALib = false;
        _analyticsState = SharedObject.getLocal("Rorys_analytics");
    }

    function GALib():* {
        super();
    }

    static function Init():void {
        goto L103;
      L6:
        secretKey = "cfdab1cab280af149c8c61cb04720cf0fc029748";
        gameKey = "747fb6ec5c8bcad8af22c1a1d148c5f7";
        goto L200;
      L29:
        secretKey = "f8fcbf66d2bf68094bb5a4fe392789b2256fd219";
        gameKey = "5f5e4974da5cef5830ae618b40eb7955";
        goto L200;
      L52:
        secretKey = "d47567967c8ffeb17cf2bd741c744d36d128d357";
        gameKey = "a1c2e81b128fbc3b5c3d22dbd8395298";
        goto L200;
      L75:
        secretKey = "b9fadddb4a063b386892c0f32548f28e03362c4b";
        gameKey = "c101e11289fc6baeec4f61083a44b465";
        goto L200;
      L98:
        goto L200;
      L103:
        var _local1 = TGlobalClass.PublisherName;
        if ("gamehouse" !== _local1) goto L124;
        $s0 = 0;
        goto L178;
      L124:
        if ("iwin" !== _local1) goto L138;
        $s0 = 1;
        goto L178;
      L138:
        if ("wildtangent" !== _local1) goto L152;
        $s0 = 2;
        goto L178;
      L152:
        if ("bfg" !== _local1) goto L166;
        $s0 = 3;
        goto L178;
      L166:
        goto L176;
      L176:
        $s0 = 4;
      L178:
        switch ($s0) { case 0: goto L6; case 1: goto L29; case 2: goto L52; case 3: goto L75; case 4: goto L98; default: goto L98; }
      L200:
        if (_analyticsState.data.userID != null) goto L260;
        GameAnalytics.init(secretKey, gameKey, Settings.Version);
        _analyticsState.data.userID = GameAnalytics.getUserId();
        _analyticsState.flush();
        goto L289;
      L260:
        GameAnalytics.init(secretKey, gameKey, Settings.Version, null, _analyticsState.data.userID);
      L289:
        GameAnalytics.DEBUG_MODE = true;
        GameAnalytics.RUN_IN_EDITOR_MODE = false;
        TrackEvent(EVENT_SESSION_BEGIN);
    }

    static function TrackEvent(arg1:String, arg2:int = 0, arg3:String = ""):void {
        goto L26;
      L6:
        GameAnalytics.newDesignEvent(arg1, arg2, arg3);
        goto L303;
      L21:
        goto L303;
      L26:
        var _local4 = arg1;
        if (EVENT_SESSION_BEGIN !== _local4) goto L44;
        $s0 = 0;
        goto L251;
      L44:
        if (EVENT_CHECK_CREDITS !== _local4) goto L59;
        $s0 = 1;
        goto L251;
      L59:
        if (EVENT_PRESS_HINT !== _local4) goto L74;
        $s0 = 2;
        goto L251;
      L74:
        if (EVENT_SUBMIT_SCORE !== _local4) goto L89;
        $s0 = 3;
        goto L251;
      L89:
        if (EVENT_START_LEVEL_STORY !== _local4) goto L104;
        $s0 = 4;
        goto L251;
      L104:
        if (EVENT_LOSE_STORY !== _local4) goto L119;
        $s0 = 5;
        goto L251;
      L119:
        if (EVENT_COMPLETE_STORY !== _local4) goto L134;
        $s0 = 6;
        goto L251;
      L134:
        if (EVENT_START_LEVEL_EXTRAS !== _local4) goto L149;
        $s0 = 7;
        goto L251;
      L149:
        if (EVENT_LOSE_EXTRAS !== _local4) goto L164;
        $s0 = 8;
        goto L251;
      L164:
        if (EVENT_COMPLETE_EXTRAS !== _local4) goto L179;
        $s0 = 9;
        goto L251;
      L179:
        if (ENTERED_ACHIEVEMENT_ROOM !== _local4) goto L194;
        $s0 = 10;
        goto L251;
      L194:
        if (ENTERED_ENDING_SCREEN !== _local4) goto L209;
        $s0 = 11;
        goto L251;
      L209:
        if (COMPLETED_AN_ACHIEVEMENT !== _local4) goto L224;
        $s0 = 12;
        goto L251;
      L224:
        if (BUY_OR_UPGRADE_RECIPE !== _local4) goto L239;
        $s0 = 13;
        goto L251;
      L239:
        goto L249;
      L249:
        $s0 = 14;
      L251:
        switch ($s0) { case 0: goto L6; case 1: goto L6; case 2: goto L6; case 3: goto L6; case 4: goto L6; case 5: goto L6; case 6: goto L6; case 7: goto L6; case 8: goto L6; case 9: goto L6; case 10: goto L6; case 11: goto L6; case 12: goto L6; case 13: goto L6; case 14: goto L21; default: goto L21; }
      L303:
    }
}
}
