package rudi.sound {
class audio_Sound {
    var name:String = "";
    var sample:gxtkSample = null;
    var _soundLib:SoundLib;

    function audio_Sound():* {
        this._soundLib = SoundLib.getInstance();
        super();
    }

    function Discard():int {
        if (this.sample == null) goto L24;
        this.sample.Discard();
        this.sample = null;
      L24:
        return 0;
    }

    function Sound_new(arg1:gxtkSample):audio_Sound {
        this.sample = arg1;
        return this;
    }

    function Sound_new2():audio_Sound {
        return this;
    }

    function play(arg1:String):void {
        this._soundLib.PlaySample(this.sample, this._soundLib.getChannel(), 0, arg1);
    }
}
}
