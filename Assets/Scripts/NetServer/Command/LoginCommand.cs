using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine;

public class LoginCommand : Command
{
    int type = 1;
    LoginInfo info;

    public LoginCommand(LoginInfo myInfo)
    {
        info = myInfo;
    }
    public LoginCommand() { }
    public byte[] MyIncode()
    {
        return Incode.IncodeFirstCommand(type, DataDo.Object2Json<LoginInfo>(info));
    }

    public void Send()
    {
        string byt = System.Text.Encoding.UTF8.GetString(Decode.DecodFirstContendBtye(MyIncode()));
        LoginInfo info= DataDo.Json2Object<LoginInfo>(Decode.DecodFirstContendBtye(MyIncode()));
        NetStart.SendContend(MyIncode());
    }

    public override void Init(byte[] bts)
    {
        base.Init(bts);
    }

    public override void DoCommand()//登陆成功验证
    {
        NetStart.myInfo = DataDo.Json2Object<PersonalInfo>(Decode.DecodFirstContendBtye(bytes));
        if (NetStart.myInfo.status == (int)PersonStatus.OnLine)//登录成功--进行主界面数据的获取--跳转界面
        {
            NetStart.isLogin = true;
            LoadMMenuScene();

        }
    }

    public static void LoadMMenuScene()
    {
        // LoadingPanel.Open();//.LoadTo("MainScence");

        SceneManager.LoadScene("Scenes/MainScence");
        UnityAction<Scene, LoadSceneMode> onLoaded = null;
        onLoaded = (Scene scene, LoadSceneMode mode) =>
        {
            MMunePanel.Open();
            SceneManager.sceneLoaded -= onLoaded;
        };
        SceneManager.sceneLoaded += onLoaded;
    }
}
