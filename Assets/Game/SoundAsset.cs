// Ported from rudi.SoundAsset and rudi.sound.* (SoundLib, SoundBank, audio_Sound, gxtkChannel, gxtkSample).
// 33 channels: 0..31 rotate for sound effects, 32 is music. Looping music restarts on SOUND_COMPLETE ("loop kludge").
using System.Collections.Generic;
using Starlite;

public class SoundAsset
{
    public string path = "data/sounds/";
    public SoundBank _soundBank;
    public SoundLib _soundLib;
    public bool _firstInit;
    public maxCrypto _maxCrypto;
    public bool mute = false;

    public void firstInit()
    {
        if (_firstInit) return;
        _firstInit = true;
        _soundBank = new SoundBank();
        _soundLib = SoundLib.getInstance();
        _maxCrypto = maxCrypto.getInstance();
    }

    public void addSound(string arg1, Sound arg2) => _soundBank.Load(arg1, arg2);

    public void playMusic(string arg1)
    {
        var s = _soundBank.Find(arg1);
        if (s != null) _soundLib.PlayMusic(s);
    }

    public void SetMusicVolume(float arg1) => _soundLib.SetMusicVolume(arg1);
    public void PauseMusic() => _soundLib.PauseMusic();
    public void ResumeMusic() => _soundLib.ResumeMusic();
    public void onSuspend() => _soundLib.OnSuspend();
    public void onResume() => _soundLib.OnResume();

    public void playSFX(string arg1)
    {
        if (Global.getInstance().MuteSound) return;
        var s = _soundBank.Find(arg1);
        if (s != null) s.play(arg1);
    }

    public void PauseSFX()
    {
        for (int i = 0; i < 32; i++) _soundLib.PauseChannel(i);
    }

    public void SetSFXVolume(float arg1)
    {
        for (int i = 0; i < 32; i++) _soundLib.SetVolume(i, arg1);
    }
}

public class gxtkSample
{
    public Sound sound;
    public gxtkSample(Sound s) { sound = s; }
    public void Discard() { sound = null; }
}

public class gxtkChannel
{
    public SoundChannel channel;
    public gxtkSample sample;
    public int loops;
    public SoundTransform transform = new SoundTransform();
    public float pausepos;
    public int state;
    public string soundName;
}

public class audio_Sound
{
    public string name = "";
    public gxtkSample sample = null;
    public SoundLib _soundLib = SoundLib.getInstance();

    public int Discard()
    {
        if (sample != null) { sample.Discard(); sample = null; }
        return 0;
    }

    public audio_Sound Sound_new(gxtkSample s) { sample = s; return this; }
    public void play(string arg1) => _soundLib.PlaySample(sample, _soundLib.getChannel(), 0, arg1);
}

public class SoundBank
{
    public static string path;
    readonly Dictionary<string, audio_Sound> mMap = new Dictionary<string, audio_Sound>();

    public void Load(string arg1, Sound arg2)
    {
        string key = arg1.ToUpperInvariant();
        if (mMap.ContainsKey(key)) return;
        var snd = new audio_Sound();
        snd.Sound_new(new gxtkSample(arg2));
        snd.name = key;
        mMap[key] = snd;
    }

    public audio_Sound Find(string arg1) => arg1 != null && mMap.TryGetValue(arg1.ToUpperInvariant(), out var s) ? s : null;
    public bool Contains(string key) => mMap.ContainsKey(key);
}

public class SoundLib
{
    static SoundLib _instance;
    public gxtkSample music;
    public List<gxtkChannel> channels = new List<gxtkChannel>();
    public int loop_kludge = 1;
    public int channelCnt = 0;
    public int startTimer;
    public bool soundCompleted;
    public string completeSoundName;

    SoundLib()
    {
        for (int i = 0; i < 33; i++) channels.Add(new gxtkChannel());
    }

    public static SoundLib getInstance() => _instance ?? (_instance = new SoundLib());

    public int getChannel()
    {
        if (channelCnt < 31) return ++channelCnt;
        channelCnt = 0;
        return channelCnt;
    }

    public void OnSuspend()
    {
        foreach (var c in channels)
        {
            if (c.channel != null && c.state == 1)
            {
                c.pausepos = c.channel.position;
                c.channel.stop();
            }
        }
    }

    public void OnResume()
    {
        foreach (var c in channels)
        {
            if (c.state != 1) continue;
            if (loop_kludge != 0)
            {
                c.channel = c.sample.sound.play(c.pausepos, 0, c.transform);
                if (c.channel != null && c.loops != 0) c.channel.addEventListener(Event.SOUND_COMPLETE, SoundComplete);
            }
            else c.channel = c.sample.sound.play(c.pausepos, c.loops, c.transform);
        }
    }

    public void SoundComplete(Event arg1)
    {
        if (loop_kludge == 0) return;
        foreach (var c in channels)
        {
            if (c.state == 1 && c.channel == arg1.target && c.loops != 0)
            {
                c.channel = c.sample.sound.play(startTimer, 0, c.transform);
                if (c.channel != null) c.channel.addEventListener(Event.SOUND_COMPLETE, SoundComplete);
                return;
            }
            if (c.state == 1 && c.channel == arg1.target && c.loops == 0)
            {
                soundCompleted = true;
                completeSoundName = c.soundName;
                if (completeSoundName == "bonusCompleteSFX" || completeSoundName == "completeSFX" || completeSoundName == "bonusRoundSFX")
                    SetMusicVolume(1);
            }
        }
    }

    public void StopAll()
    {
        for (int i = 0; i < 32; i++)
        {
            var c = channels[i];
            if (c.channel != null && c.state == 1)
            {
                c.pausepos = c.channel.position;
                c.channel.stop();
            }
        }
    }

    public int PlaySample(gxtkSample arg1, int arg2, int arg3, string arg4)
    {
        if (arg1 == null || arg1.sound == null) return -1;
        var c = channels[arg2];
        if (c.channel != null && c.state != 0) c.channel.stop();
        c.sample = arg1;
        c.loops = arg3 != 0 ? int.MaxValue : 0;
        c.state = 1;
        c.soundName = arg4;
        if (loop_kludge != 0)
        {
            c.channel = arg1.sound.play(0, 0, c.transform);
            if (c.channel != null) c.channel.addEventListener(Event.SOUND_COMPLETE, SoundComplete);
        }
        else c.channel = arg1.sound.play(0, c.loops, c.transform);
        return 0;
    }

    public int StopChannel(int arg1)
    {
        var c = channels[arg1];
        if (c.state != 0)
        {
            if (c.channel != null) c.channel.stop();
            c.channel = null;
            c.sample = null;
            c.state = 0;
        }
        return 0;
    }

    public int PauseChannel(int arg1)
    {
        var c = channels[arg1];
        if (c.channel == null) return 0;
        if (c.state == 1)
        {
            c.pausepos = c.channel.position;
            c.channel.stop();
            c.state = 2;
        }
        return 0;
    }

    public int ResumeChannel(int arg1)
    {
        var c = channels[arg1];
        if (c.state == 2)
        {
            if (c.loops != 0)
            {
                c.channel = c.sample.sound.play(c.pausepos, 0, c.transform);
                if (c.channel != null) c.channel.addEventListener(Event.SOUND_COMPLETE, SoundComplete);
            }
            else c.channel = c.sample.sound.play(c.pausepos, c.loops, c.transform);
            c.state = 1;
        }
        return 0;
    }

    public int ChannelState(int arg1) => -1;

    public int SetVolume(int arg1, float arg2)
    {
        var c = channels[arg1];
        if (c.channel == null) return 0;
        c.transform.volume = arg2;
        if (c.state != 0) c.channel.soundTransform = c.transform;
        return 0;
    }

    public int SetPan(int arg1, float arg2)
    {
        var c = channels[arg1];
        if (c.channel == null) return 0;
        c.transform.pan = arg2;
        if (c.state != 0) c.channel.soundTransform = c.transform;
        return 0;
    }

    public int SetRate(int arg1, float arg2) => -1;

    public int PlayMusic(audio_Sound arg1, int arg2 = 1)
    {
        StopMusic();
        music = arg1.sample;
        if (music == null) return -1;
        PlaySample(music, 32, arg2, "BGM");
        return 0;
    }

    public int StopMusic()
    {
        StopChannel(32);
        music = null;
        return 0;
    }

    public int PauseMusic() { PauseChannel(32); return 0; }
    public int ResumeMusic() { ResumeChannel(32); return 0; }
    public int MusicState() => ChannelState(32);
    public int SetMusicVolume(float arg1) { SetVolume(32, arg1); return 0; }
}
