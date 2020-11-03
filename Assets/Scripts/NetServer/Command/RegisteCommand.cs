using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class RegisteCommand : Command
{
    const int TYPE = 5;

    public RegisteCommand() { }

    public override void Init(byte[] bts)
    {
        base.Init(bts);
    }

    public override void DoCommand()
    {
        NetStart.myInfo = DataDo.Json2Object<PersonalInfo>(Decode.DecodFirstContendBtye(bytes));
        //if (NetStart.myInfo.status == 1)//登录成功--进行主界面数据的获取--跳转界面
        {
            NetStart.isLogin = true;//登录标志（在线）
            SceneManager.LoadScene("Scenes/MainScence");
            UnityAction<Scene, LoadSceneMode> onLoaded = null;
            onLoaded = (Scene scene, LoadSceneMode mode) =>
            {
                MMunePanel.Open();
                WelcomePanel.Open();
                SceneManager.sceneLoaded -= onLoaded;
            };
            SceneManager.sceneLoaded += onLoaded;
        }
    }

    public static void Registe_Send(LoginInfo info)
    {
        NetStart.SendContend(Incode.IncodeFirstCommand(TYPE, DataDo.Object2Json<LoginInfo>(info)));
    }
}
