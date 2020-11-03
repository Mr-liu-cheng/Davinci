using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// f负责显示界面，控制音量在另一个管理脚本里
/// </summary>
public class SettingPanel : TPanel<SettingPanel>
{
    public Button Exit_Btn;
    public Slider main_Volume;
    public Slider bg_Volume;
    public Slider audio_Volume;

    void Start()
    {
        gameObject.transform.SetAsLastSibling();
        main_Volume.value = GameAudio.instance.MainVolume;
        bg_Volume.value = GameAudio.instance.BgVolume;
        audio_Volume.value = GameAudio.instance.AudioVolume;

        main_Volume.onValueChanged.AddListener(value => GameAudio.instance.MainVolume = value);
        bg_Volume.onValueChanged.AddListener(value => GameAudio.instance.BgVolume = value);
        audio_Volume.onValueChanged.AddListener(value => GameAudio.instance.AudioVolume = value);

        Exit_Btn.onClick.AddListener(Exit);
    }

    void Exit()
    {
        GameAudio.instance.PlayAudioSourceUI("close_btn");
        Close();
    }
}
