package rudi.sound {
class SoundBank extends StringMap {
    static var path:String;

    function SoundBank():* {
        super();
    }

    function Load(arg1:String, arg2:Sound):void {
        var _local3 = arg1.toUpperCase();
        if (!this.Contains(_local3)) goto L20;
        return;
      L20:
        var _local4 = new audio_Sound();
        var _local5 = new gxtkSample(arg2);
        _local4.Sound_new(_local5);
        _local4.name = _local3;
        this.Set(_local4.name, _local4);
    }

    function Find(arg1:String):audio_Sound {
        arg1 = arg1.toUpperCase();
        var _local2 = this.Get(arg1);
        if (_local2 != null) goto L24;
      L24:
        return _local2;
    }
}
}
