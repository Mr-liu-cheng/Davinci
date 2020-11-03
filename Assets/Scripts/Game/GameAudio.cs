using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class GameAudio : MonoBehaviour
{
    private static GameAudio _instance;
    /// <summary>
    /// 状态值控制各种音效的开关【背景音乐、音效、喊话】
    /// </summary>
    /// 音量控制开关【另写一个settingPanel脚本用静态变量存储】
    private float mainVolume;
    private float bgVolume;
    private float audioVolume;

    AudioMixerGroup audioMixerGroup;
    //AudioMixerGroup mainMixerGroup;
    //AudioMixerGroup bgMixerGroup;
    AudioMixer audioMixer;

    public static GameAudio instance
    {
        get
        {
            return _instance;
        }
    }

    public float MainVolume { 
        get => mainVolume; 
        set { 
            mainVolume = value;
            audioMixer.SetFloat("mainVolume", value);
        } }
    public float BgVolume { 
        get => bgVolume;
        set { 
            bgVolume = value;
            audioMixer.SetFloat("bgVolume", value);
        }
    }
    public float AudioVolume {
        get => audioVolume; 
        set { 
            audioVolume = value; 
            audioMixer.SetFloat("audioVolume", value);
        }
    }


    private Dictionary<string, AudioClip> audios;  //创建一个词典 存放我们的所有声音

    private AudioSource[] audioSources; //包含 背景音效  循环音效 【固定不会删掉的组件】
    private AudioSource audioSourceBG;
    private AudioSource audioSourceLoop;//循环音效【暂定计时器专属】

    void Awake()
    {
        _instance = this;
        
        audios = new Dictionary<string, AudioClip>();
        //动态加载 资源文件必须在Resources目录下哦
        AudioClip[] audioArray = Resources.LoadAll<AudioClip>("Audio"); //取得指定目录下所有声音并加入声音数组
        audioSources = GetComponents<AudioSource>();

        audioSourceBG = audioSources[0]; //得到默认的BG音效组件
        audioSourceLoop= audioSources[1];
        audioSourceBG.loop = true;
        audioSourceLoop.loop = true;
        // 用foreach 遍历声音数组里面的声音  并放入字典 
        foreach (AudioClip a in audioArray)
        {
            audios.Add(a.name, a);            
        }
        PlayAudioSourceBG("Main");
        // audioMixerGroup = Resources.LoadAll<AudioMixerGroup>("Audio/AudioMixer");//此方法无法访问到嵌套的物体
        audioMixer = Resources.Load<AudioMixer>("Audio/AudioMixer");
        audioMixerGroup = audioMixer.FindMatchingGroups("Master/audio")[0];
        //mainMixerGroup = audioMixer.FindMatchingGroups("Master/Master")[0];
        //bgMixerGroup = audioMixer.FindMatchingGroups("Master/bg")[0];
        Debug.Log("-------" + audioArray.Length);
    }

    private void Start()
    {
        BgVolume = 5;
        AudioVolume = 9;
    }

    public void PlayAudioSourceBG(string name)
    {
        if (audios.ContainsKey(name))
        {
            audioSourceBG.clip = audios[name];
            audioSourceBG.Play();
        }
    }

    /// <summary>
    /// 循环音效
    /// </summary>
    public void PlayAudioSourceLoop(string name,bool _switch)
    {
        if (audios.ContainsKey(name))
        {
            audioSourceLoop.clip = audios[name];
            if (_switch)
            {
                audioSourceLoop.Play();
            }
            else
            {
                audioSourceLoop.Stop();
            }
        }
    }



    /// <summary>
    /// 接收到别人发的快捷语音包
    /// </summary>
    /// <param name="name"></param>
    public void PlayAudioSourceUI(string name) 
    {
        if (audios.ContainsKey(name))
        {
            AudioSource otherRoleAudioSource = gameObject.AddComponent<AudioSource>();
            otherRoleAudioSource.outputAudioMixerGroup=audioMixerGroup;
          
            otherRoleAudioSource.clip = audios[name];
            otherRoleAudioSource.Play();
            StartCoroutine(DestoryAudioSource(otherRoleAudioSource));
        }
    }

    /// <summary>
    /// 播放完后删除【待测试】
    /// </summary>
    /// <param name="otherRoleAudioSource"></param>
    /// <returns></returns>
    IEnumerator DestoryAudioSource(AudioSource otherRoleAudioSource)
    {
        yield return new WaitForSeconds(otherRoleAudioSource.clip.length);
        Destroy(otherRoleAudioSource);
    }

   
}

