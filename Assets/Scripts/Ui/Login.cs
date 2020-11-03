using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public Button login_B, registe_b;
    public InputField input_Id, passWord;
    public GameObject loginS, registerS;
    public static bool changeScence;//跳转场景

    void Start()
    {
        loginS.SetActive(true);
        registerS.SetActive(false);
        LoginCommand login_Command = new LoginCommand(new LoginInfo {id="1",passWord="1" });
        login_Command.Send();

        login_B.onClick.AddListener(Login_In);
        registe_b.onClick.AddListener(Registe);
    }

    public void Login_In()
    {
        GameAudio.instance.PlayAudioSourceUI("click_btn");
        LoginInfo info = new LoginInfo();
        info.id = input_Id.text;
        info.passWord = passWord.text;
        LoginCommand login_Command = new LoginCommand(info);
        login_Command.Send();
    }

    public void Registe()
    {
        GameAudio.instance.PlayAudioSourceUI("click_btn");
        Debug.Log("Registe");
        registerS.SetActive(true);
        loginS.SetActive(false);
    }
}
