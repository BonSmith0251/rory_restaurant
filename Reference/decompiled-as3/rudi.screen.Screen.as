package rudi.screen {
class Screen {
    var name:String = "";
    var autoFadeIn:Boolean = false;
    var autoFadeInTime:Number = 50;
    var autoFadeInSound:Boolean = false;
    var autoFadeInMusic:Boolean = false;
    var ready:Boolean = false;

    function Screen():* {
        super();
    }

    function OnTouchHit(arg1:int, arg2:int, arg3:int):void {
    }

    function OnTouchClick(arg1:int, arg2:int, arg3:int):void {
    }

    function OnTouchFling(arg1:int, arg2:int, arg3:Number, arg4:Number, arg5:Number, arg6:int):void {
    }

    function OnTouchReleased(arg1:int, arg2:int, arg3:int):void {
    }

    function OnTouchDragged(arg1:int, arg2:int, arg3:int, arg4:int, arg5:int):void {
    }

    function OnTouchLongPress(arg1:int, arg2:int, arg3:int):void {
    }

    function OnAnyKeyHit():void {
    }

    function OnKeyHit(arg1:int):void {
    }

    function OnAnyKeyDown():void {
    }

    function OnKeyDown(arg1:int):void {
    }

    function OnAnyKeyReleased():void {
    }

    function OnKeyReleased(arg1:int):void {
    }

    function OnMouseHit(arg1:int, arg2:int, arg3:int):void {
    }

    function OnMouseDown(arg1:int, arg2:int, arg3:int):void {
    }

    function OnMouseReleased(arg1:int, arg2:int, arg3:int):void {
    }

    function Kill():void {
    }

    function Start():void {
        gameEngine.getInstance()._currentScreen = this;
    }

    function PreStart():void {
        gameEngine.getInstance()._currentScreen = this;
    }

    function PostFadeOut():void {
        this.Kill();
    }

    function PostFadeIn():void {
    }

    function Update():void {
    }

    function Suspend():void {
    }

    function Resume():void {
    }

    function Pause():void {
    }
}
}
