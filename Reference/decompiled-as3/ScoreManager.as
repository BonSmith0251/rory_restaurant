package {
class ScoreManager {
    static var _global:Global;
    static var scoreText:TextField;
    static var scoreDisplayText:TextField;
    static var updatingScore:Boolean;
    static var score:String;
    static var totalScore:String;

    // static initializer
    static {
        _global = Global.getInstance();
    }

    function ScoreManager():* {
        super();
    }

    static function initScore(arg1:int, arg2:int):void {
        totalScore = "a";
        score = "a";
        scoreDisplayText = new TextField(150, 50, "$ 0", "jenna_sue", 35);
        scoreDisplayText.x = arg1 + 30;
        scoreDisplayText.y = arg2;
        scoreDisplayText.vAlign = "center";
        scoreDisplayText.touchable = false;
    }

    static function updateScore(arg1:int):void {
        totalScore = _global._maxCrypto.ITS(_global._maxCrypto.STI(totalScore) + arg1);
        updatingScore = true;
    }

    static function updateDisplayedScore():void {
        var _local1 = int(_global._maxCrypto.STI(score));
        var _local2 = int(_global._maxCrypto.STI(totalScore));
        if (!(_local1 < _local2)) goto L102;
        score = _global._maxCrypto.ITS(_local1 + 5 + 5 * (_local2 - _local1) / 100);
        scoreDisplayText.text = "$ " + _global.addCommas(_global._maxCrypto.STI(score));
        goto L109;
      L102:
        updatingScore = false;
      L109:
        if (!(_global._maxCrypto.STI(score) > _local2)) goto L166;
        score = totalScore;
        scoreDisplayText.text = "$ " + _global.addCommas(_global._maxCrypto.STI(score));
      L166:
    }

    static function Kill():void {
        scoreDisplayText.removeFromParent(true);
        scoreDisplayText = null;
    }
}
}
