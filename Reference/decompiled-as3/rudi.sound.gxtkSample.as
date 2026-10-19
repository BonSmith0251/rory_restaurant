package rudi.sound {
class gxtkSample {
    var sound:Sound;

    function gxtkSample(arg1:Sound):* {
        super();
        this.sound = arg1;
    }

    function Discard():int {
        this.sound = null;
        return 0;
    }
}
}
