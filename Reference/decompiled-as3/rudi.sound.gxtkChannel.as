package rudi.sound {
class gxtkChannel {
    var channel:SoundChannel;
    var sample:gxtkSample;
    var loops:int;
    var transform:SoundTransform;
    var pausepos:Number;
    var state:int;
    var soundName:String;

    function gxtkChannel():* {
        this.transform = new SoundTransform();
        super();
    }
}
}
