package rudi.sound {
class SoundLib {
    static var _instance:SoundLib;
    var music:gxtkSample;
    var channels:Array;
    var loop_kludge:int = 1;
    var channelCnt:int = 0;
    var startTimer:int;
    var soundCompleted:Boolean;
    var completeSoundName:String;

    function SoundLib(arg1:PrivateClass):* {
        this.channels = new Array(33);
        super();
        var _local2 = 0;
        goto L43;
      L25:
        this.channels[_local2] = new gxtkChannel();
        _local2++;
      L43:
        if (_local2 < 33) goto L25;
    }

    static function getInstance():SoundLib {
        if (SoundLib._instance != null) goto L29;
        SoundLib._instance = new this(new PrivateClass());
      L29:
        return SoundLib._instance;
    }

    function getChannel():int {
        if (!(this.channelCnt < 31)) goto L34;
        var _local1 = this;
        var _local2 = this.channelCnt + 1;
        _local1.channelCnt = _local2;
        return this.channelCnt;
      L34:
        this.channelCnt = 0;
        return this.channelCnt;
    }

    function OnSuspend():void {
        var _local2 = null;
        var _local1 = 0;
        goto L67;
      L14:
        _local2 = this.channels[_local1];
        if (_local2.channel == null) goto L65;
        if (_local2.state != 1) goto L65;
        _local2.pausepos = _local2.channel.position;
        _local2.channel.stop();
      L65:
        _local1++;
      L67:
        if (_local1 < 33) goto L14;
    }

    function OnResume():void {
        var _local2 = null;
        var _local1 = 0;
        goto L138;
      L14:
        _local2 = this.channels[_local1];
        if (_local2.state != 1) goto L136;
        if (!this.loop_kludge) goto L109;
        _local2.channel = _local2.sample.sound.play(_local2.pausepos, 0, _local2.transform);
        if (_local2.channel == null) goto L105;
        if (!_local2.loops) goto L105;
        _local2.channel.addEventListener(Event.SOUND_COMPLETE, this.SoundComplete);
      L105:
        goto L136;
      L109:
        _local2.channel = _local2.sample.sound.play(_local2.pausepos, _local2.loops, _local2.transform);
      L136:
        _local1++;
      L138:
        if (_local1 < 33) goto L14;
    }

    function SoundComplete(arg1:Event):void {
        var _local3 = null;
        if (this.loop_kludge) goto L16;
        return;
      L16:
        var _local2 = 0;
        goto L232;
      L23:
        _local3 = this.channels[_local2];
        var $t0 = _local3.state == 1;
        $s0 = $t0;
        if (!$t0) goto L58;
        $s0 = _local3.channel == arg1.target;
      L58:
        if (!$s0) goto L69;
        $s0 = Boolean(_local3.loops);
      L69:
        if (!$s0) goto L133;
        _local3.channel = _local3.sample.sound.play(this.startTimer, 0, _local3.transform);
        if (_local3.channel == null) goto L125;
        _local3.channel.addEventListener(Event.SOUND_COMPLETE, this.SoundComplete);
      L125:
        goto L239;
      L133:
        var $t1 = _local3.state == 1;
        $s0 = $t1;
        if (!$t1) goto L155;
        $s0 = _local3.channel == arg1.target;
      L155:
        if (!$s0) goto L166;
        $s0 = !_local3.loops;
      L166:
        if (!$s0) goto L230;
        this.soundCompleted = true;
        this.completeSoundName = _local3.soundName;
        var $t2 = this.completeSoundName == "bonusCompleteSFX";
        $s0 = $t2;
        if ($t2) goto L205;
        $s0 = this.completeSoundName == "completeSFX";
      L205:
        if ($s0) goto L219;
        $s0 = this.completeSoundName == "bonusRoundSFX";
      L219:
        if (!$s0) goto L230;
        this.SetMusicVolume(1);
      L230:
        _local2++;
      L232:
        if (_local2 < 33) goto L23;
      L239:
    }

    function StopAll():void {
        var _local2 = null;
        var _local1 = 0;
        goto L67;
      L14:
        _local2 = this.channels[_local1];
        if (_local2.channel == null) goto L65;
        if (_local2.state != 1) goto L65;
        _local2.pausepos = _local2.channel.position;
        _local2.channel.stop();
      L65:
        _local1++;
      L67:
        if (_local1 < 32) goto L14;
    }

    function LoadSample(arg1:String):gxtkSample {
        var _local2 = this.loadSound(arg1);
        if (!_local2) goto L25;
        return new gxtkSample(_local2);
      L25:
        return null;
    }

    function PlaySample(arg1:gxtkSample, arg2:int, arg3:int, arg4:String):int {
        var $t0 = this.channels[arg2];
        var _local5 = $t0;
        if ($t0.channel == null) goto L44;
        if (_local5.state == 0) goto L44;
        _local5.channel.stop();
      L44:
        _local5.sample = arg1;
        $s0 = _local5;
        if (!arg3) goto L63;
        $s1 = 2147483647;
        goto L65;
      L63:
        $s1 = 0;
      L65:
        $s0.loops = $s1;
        _local5.state = 1;
        _local5.soundName = arg4;
        if (!this.loop_kludge) goto L144;
        _local5.channel = arg1.sound.play(0, 0, _local5.transform);
        if (_local5.channel == null) goto L140;
        _local5.channel.addEventListener(Event.SOUND_COMPLETE, this.SoundComplete);
      L140:
        goto L169;
      L144:
        _local5.channel = arg1.sound.play(0, _local5.loops, _local5.transform);
      L169:
        return 0;
    }

    function StopChannel(arg1:int):int {
        var _local2 = this.channels[arg1];
        if (_local2.state == 0) goto L57;
        if (_local2.channel == null) goto L41;
        _local2.channel.stop();
      L41:
        _local2.channel = null;
        _local2.sample = null;
        _local2.state = 0;
      L57:
        return 0;
    }

    function PauseChannel(arg1:int):int {
        var _local2 = this.channels[arg1];
        if (_local2.channel != null) goto L24;
        return;
      L24:
        if (_local2.state != 1) goto L59;
        _local2.pausepos = _local2.channel.position;
        _local2.channel.stop();
        _local2.state = 2;
      L59:
        return 0;
    }

    function ResumeChannel(arg1:int):int {
        var _local2 = this.channels[arg1];
        if (_local2.state != 2) goto L121;
        if (!_local2.loops) goto L88;
        _local2.channel = _local2.sample.sound.play(_local2.pausepos, 0, _local2.transform);
        if (_local2.channel == null) goto L84;
        _local2.channel.addEventListener(Event.SOUND_COMPLETE, this.SoundComplete);
      L84:
        goto L115;
      L88:
        _local2.channel = _local2.sample.sound.play(_local2.pausepos, _local2.loops, _local2.transform);
      L115:
        _local2.state = 1;
      L121:
        return 0;
    }

    function ChannelState(arg1:int):int {
        return -1;
    }

    function SetVolume(arg1:int, arg2:Number):int {
        var _local3 = this.channels[arg1];
        if (_local3.channel != null) goto L24;
        return;
      L24:
        _local3.transform.volume = arg2;
        if (_local3.state == 0) goto L53;
        _local3.channel.soundTransform = _local3.transform;
      L53:
        return 0;
    }

    function SetPan(arg1:int, arg2:Number):int {
        var _local3 = this.channels[arg1];
        if (_local3.channel != null) goto L24;
        return;
      L24:
        _local3.transform.pan = arg2;
        if (_local3.state == 0) goto L53;
        _local3.channel.soundTransform = _local3.transform;
      L53:
        return 0;
    }

    function SetRate(arg1:int, arg2:Number):int {
        return -1;
    }

    function PlayMusic(arg1:audio_Sound, arg2:int = 1):int {
        this.StopMusic();
        this.music = arg1.sample;
        if (this.music) goto L26;
        return -1;
      L26:
        this.PlaySample(this.music, 32, arg2, "BGM");
        return 0;
    }

    function StopMusic():int {
        this.StopChannel(32);
        if (!this.music) goto L22;
        this.music = null;
      L22:
        return 0;
    }

    function PauseMusic():int {
        this.PauseChannel(32);
        return 0;
    }

    function ResumeMusic():int {
        this.ResumeChannel(32);
        return 0;
    }

    function MusicState():int {
        return this.ChannelState(32);
    }

    function SetMusicVolume(arg1:Number):int {
        this.SetVolume(32, arg1);
        return 0;
    }

    function loadSound(arg1:String):Sound {
        return null;
    }
}
}
