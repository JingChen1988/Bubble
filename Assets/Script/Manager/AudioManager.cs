using UnityEngine;
/// <summary>
/// 音效管理器
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioSource BGM;//音乐播放器
    public static AudioSource[] Sounds;//音效播发器集合

    public AudioClip BGMClip;//背景音乐
    public int SoundCount;//音效数量

    const float BGMVolume = .4f;
    const float SoundVolume = .8f;

    #region 初始化
    void Start()
    {
        //初始化背景音乐播放器
        Transform camera = Camera.main.transform;
        GameObject BGMobj = new GameObject("BGM");
        BGMobj.transform.parent = camera;
        BGMobj.transform.localPosition = Vector3.zero;
        BGM = BGMobj.AddComponent<AudioSource>();
        BGM.loop = true;
        BGM.clip = BGMClip;
        if (BGMClip) BGM.Play();

        //初始化音效播放器
        Sounds = new AudioSource[SoundCount];
        for (int i = 0; i < SoundCount; i++)
        {
            GameObject obj = new GameObject("Sound" + i);
            Sounds[i] = obj.AddComponent<AudioSource>();
            Sounds[i].reverbZoneMix = 0;//2D音效
            obj.transform.parent = camera;
            obj.transform.localPosition = Vector3.zero;
        }

        //加载系统设置
        LoadOption();
    }

    //加载设置
    public static void LoadOption()
    {
        Data.UserInfo userInfo = Data.UsersInfo;
        BGM.volume = userInfo.BGM ? BGMVolume : 0;
        for (int i = 0, len = Sounds.Length; i < len; i++)
            Sounds[i].volume = userInfo.Sound ? SoundVolume : 0;
    }
    #endregion

    //播放音效
    public static void Play(AudioClip clip, float volume = 1)
    {
        if (!Data.UsersInfo.Sound) return;
        for (int i = 0, len = Sounds.Length; i < len; i++)
        {
            AudioSource audio = Sounds[i];
            if (!audio.loop && !audio.isPlaying)
            {
                audio.volume = volume * SoundVolume;
                audio.PlayOneShot(clip);
                break;
            }
        }
    }

    //播放音效（唯一）
    public static void PlayUnique(AudioClip clip, float volume = 1)
    {
        if (!Data.UsersInfo.Sound) return;
        AudioSource audio = Sounds[Sounds.Length - 1];
        if (!audio.isPlaying)
        {
            audio.volume = volume * SoundVolume;
            audio.PlayOneShot(clip);
        }
    }

    //持续音效
    public static AudioSource Keep(AudioClip clip)
    {
        AudioSource source = null;
        for (int i = 0, len = Sounds.Length; i < len; i++)
            if (!Sounds[i].isPlaying)
            {
                source = Sounds[i];
                break;
            }
        if (source != null) Keep(source, clip);
        return source;
    }

    //停止持续音效
    public static void Stop(AudioClip clip)
    {
        for (int i = 0, len = Sounds.Length; i < len; i++)
        {
            AudioSource sound = Sounds[i];
            if (sound.clip == clip && sound.isPlaying)
            {
                Stop(sound);
                break;
            }
        }
    }

    //恢复所有持续音效
    public static void PlayAll()
    {
        for (int i = 0, len = Sounds.Length; i < len; i++)
        {
            AudioSource sound = Sounds[i];
            if (sound.loop)
                sound.Play();
        }
    }

    //暂停所有持续音效
    public static void PauseAll()
    {
        for (int i = 0, len = Sounds.Length; i < len; i++)
        {
            AudioSource sound = Sounds[i];
            if (sound.loop && sound.isPlaying)
                sound.Pause();
        }
    }

    //停止所有音效
    public static void StopAll()
    {
        for (int i = 0, len = Sounds.Length; i < len; i++)
        {
            AudioSource sound = Sounds[i];
            if (sound.isPlaying)
                Stop(sound);
        }
    }

    //持续音效
    static void Keep(AudioSource source, AudioClip clip)
    {
        source.loop = true;
        source.clip = clip;
        source.volume = SoundVolume;
        if (Data.UsersInfo.Sound) source.Play();
    }

    //停止音效
    public static void Stop(AudioSource source)
    {
        source.loop = false;
        source.clip = null;
        source.Stop();
    }

    //开关背景音乐
    public static void SwitchBGM()
    {
        if (Data.UsersInfo.BGM) BGM.Play();
        else BGM.Pause();
    }

    //开关音效
    public static void SwitchSound()
    {
        if (Data.UsersInfo.Sound) PlayAll();
        else PauseAll();
    }
}
