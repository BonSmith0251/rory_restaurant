package rudi {
class SoundAsset {
    var path:String = "data/sounds/";
    var _soundBank:SoundBank;
    var _soundLib:SoundLib;
    var _firstInit:Boolean;
    var _maxCrypto:maxCrypto;
    var mute:Boolean = false;

    function SoundAsset():* {
        super();
    }

    function firstInit():void {
        if (this._firstInit) goto L48;
        this._firstInit = true;
        this._soundBank = new SoundBank();
        this._soundLib = SoundLib.getInstance();
        this._maxCrypto = maxCrypto.getInstance();
      L48:
    }

    function addSound(arg1:String, arg2:Sound):void {
        this._soundBank.Load(arg1, arg2);
    }

    function playMusic(arg1:String):void {
        if (this._soundBank.Find(arg1) == null) goto L33;
        this._soundLib.PlayMusic(this._soundBank.Find(arg1));
      L33:
    }

    function SetMusicVolume(arg1:Number):void {
        this._soundLib.SetMusicVolume(arg1);
    }

    function PauseMusic():void {
        this._soundLib.PauseMusic();
    }

    function ResumeMusic():void {
        this._soundLib.ResumeMusic();
    }

    function onSuspend():void {
        this._soundLib.OnSuspend();
    }

    function onResume():void {
        this._soundLib.OnResume();
    }

    function playSFX(arg1:String):void {
        if (!Global.getInstance().MuteSound) goto L17;
        return;
      L17:
        if (this._soundBank.Find(arg1) == null) goto L45;
        this._soundBank.Find(arg1).play(arg1);
      L45:
    }

    function PauseSFX():void {
        var _local1 = 0;
        goto L21;
      L9:
        this._soundLib.PauseChannel(_local1);
        _local1++;
      L21:
        if (_local1 < 32) goto L9;
    }

    function SetSFXVolume(arg1:Number):void {
        var _local2 = 0;
        goto L22;
      L9:
        this._soundLib.SetVolume(_local2, arg1);
        _local2++;
      L22:
        if (_local2 < 32) goto L9;
    }
}
}
