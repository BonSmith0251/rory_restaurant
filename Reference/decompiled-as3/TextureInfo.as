package {
class TextureInfo {
    var region:Rectangle;
    var frame:Rectangle;
    var rotated:Boolean;

    function TextureInfo(arg1:Rectangle, arg2:Rectangle, arg3:Boolean):* {
        super();
        this.region = arg1;
        this.frame = arg2;
        this.rotated = arg3;
    }
}
}
