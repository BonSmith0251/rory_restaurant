package rudi {
class gameEngine {
    static var _instance:gameEngine;
    var _currentScreen:Screen;
    var _nextScreen:Screen;

    function gameEngine(arg1:PrivateClass):* {
        super();
    }

    static function getInstance():gameEngine {
        if (gameEngine._instance != null) goto L29;
        gameEngine._instance = new this(new PrivateClass());
      L29:
        return gameEngine._instance;
    }
}
}
