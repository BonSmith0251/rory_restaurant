package {
class ExtendTextField extends TextField {
    var aktif:Boolean;
    var idx:int;
    var itemIsFound:Boolean;
    var placeNumber:int;

    function ExtendTextField(arg1:int, arg2:int, arg3:String, arg4:String = "Verdana", arg5:Number = 12, arg6:uint = 0, arg7:Boolean = false):* {
        super(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }
}
}
