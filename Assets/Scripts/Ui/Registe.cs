using UnityEngine;
using UnityEngine.UI;

class Registe : MonoBehaviour
{
    public Button registe_B;
    public InputField myName, psd, psd2;

    void Start()
    {
        registe_B.onClick.AddListener(Submit);
    }

    void Submit()
    {
        GameAudio.instance.PlayAudioSourceUI("click_btn");

        if (myName.text=="")
        {
            Debug.Log("昵称为空");
        }
        if (psd.text != psd2.text)
        {
            Debug.Log("密码不一致");
        }
        if(psd.text == psd2.text && myName.text != "")
        {
            LoginInfo info = new LoginInfo();
            info.userName = myName.text;//昵称
            info.passWord = psd.text;

            RegisteCommand.Registe_Send(info);
            Debug.Log("submit");
        }
    }
}

