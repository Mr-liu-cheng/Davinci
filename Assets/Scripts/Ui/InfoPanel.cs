using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/****************************************************
	项 目 名 称：
    作		者：刘光和
    功		能：
	修 改 时 间：
*****************************************************/

public class InfoPanel : TPanel<InfoPanel>
{
    public Button Info_ExitButton;//好友信息界面关闭  
    public Image icon,sex;
    public Text _name, id, age, coinCount, winRate, serialWin, gameNum, degree;
    public InputField markName;

    public void Initialize (PersonalInfo personalInfo) 
	{
        GetInfoPanel(personalInfo);
        Info_ExitButton.onClick.AddListener(() =>
        {
            GameAudio.instance.PlayAudioSourceUI("close_btn");
            Debug.Log("关闭个人信息界面");
            Close();
        });
    }
    public void GetInfoPanel(PersonalInfo personalInfo)
    {
        icon.sprite = Resources.Load("UI/role_photo/" + personalInfo.icon, typeof(Sprite)) as Sprite;
        id.text = "ID:  " + personalInfo.id.ToString();
        age.text = "年龄:  " + personalInfo.age.ToString();
        sex.sprite = Resources.Load<Sprite>("UI/MMenu/Info/sex" + personalInfo.sex);
        markName.text = personalInfo.markName;
        _name.text = "昵称:  " + personalInfo.name;
        gameNum.text = "总对局:  " + personalInfo.gameNum.ToString();
        serialWin.text = "最高连胜:  " + personalInfo.serialWin.ToString();
        winRate.text = "胜率:  " + personalInfo.winRate.ToString();
        degree.text = "段位:  " + personalInfo.degree.ToString();
        coinCount.text = "  " + personalInfo.coin.ToString();
    }
}
