package {
class TStringButton extends ExtendTextField {
    var hover:Boolean;

    function TStringButton(arg1:int, arg2:int, arg3:String, arg4:String = "Verdana", arg5:Number = 12, arg6:uint = 0, arg7:Boolean = false):* {
        super(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        this.addEventListener(TouchEvent.TOUCH, this.touchText);
        this.useHandCursor = true;
    }

    function touchText(arg1:TouchEvent):void {
        var _local2 = arg1.currentTarget as TStringButton;
        var _local3 = arg1.getTouch(_local2, "hover");
        if (!_local3) goto L70;
        if (_local2.hover) goto L66;
        _local2.scaleX = _local2.scaleX * 1.1;
        _local2.scaleY = _local2.scaleY * 1.1;
        _local2.hover = true;
      L66:
        goto L111;
      L70:
        if (!_local2.hover) goto L111;
        _local2.scaleX = _local2.scaleX * (10 / 11);
        _local2.scaleY = _local2.scaleY * (10 / 11);
        _local2.hover = false;
      L111:
    }
}
}
