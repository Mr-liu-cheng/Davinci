using UnityEngine.UI;

public class WelcomePanel : TPanel<WelcomePanel>
{
    public Button Exit_Btn;

    void Start()
    {
        Exit_Btn.onClick.AddListener(Exit);
    }

    void Exit()
    {
        GameAudio.instance.PlayAudioSourceUI("close_btn");
        Close();
    }
}
