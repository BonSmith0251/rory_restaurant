package {
class TutorialEvent extends Event {
    static const TRIGGER_NEXT:String = "trigger_next";
    var actionName:String;
    var infos:Object;

    // static initializer
    static {
        TRIGGER_NEXT = "trigger_next";
    }

    function TutorialEvent(arg1:String, arg2:String, arg3:Boolean = true, arg4:Boolean = false, arg5:Object = null):* {
        super(arg1, arg3, arg4);
        this.actionName = arg2;
        this.infos = arg5;
    }
}
}
