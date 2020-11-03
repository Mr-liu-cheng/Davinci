using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameOverPanel : TPanel<GameOverPanel>
{
    public Transform content;
    public Button Continue_Btn;
    public Button Exit_Btn;

    /// <summary>
    /// 要加载的数据
    /// </summary>
    public RoomInfo RoomInfo;

    /// <summary>
    /// 临时排行
    /// </summary>
    public List<int> tempRankList;

    void Start()
    {
        GameAudio.instance.PlayAudioSourceLoop("second", false);
        //加载成员
        GameObject gameOverItemPrefb = Resources.Load<GameObject>("Prefabs/game/GameOverItem");

        Debug.Log("tempRankList.Count：" + tempRankList.Count + "        RoomInfo.member.Count：" + RoomInfo.member.Count);
        int id;
        for (int i = RoomInfo.member.Count - 1; i >= 0; i--)
        {
            Debug.Log("-------循环执行次数：" + i);

            id = tempRankList[i];
            Debug.Log("-------id：" + id);
            PersonalInfo personal = RoomInfo.member.Find(it =>
             {
                 if (it.id == id)//tempRankList空的
                 {
                     return true;
                 }
                 else
                 {
                     return false;
                 }
             });

            if (personal != null)
            {
                Transform item = Instantiate(gameOverItemPrefb, content).transform;
                item.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/role_photo/" + personal.icon);
                //item.Find("Text").GetComponent<Text>().text = (i+1).ToString();
                item.Find("Text").GetComponent<Text>().text = (RoomInfo.member.Count - i).ToString();
                item.Find("Name").GetComponent<Text>().text = personal.name;
            }
        }


        //添加按钮事件
        Continue_Btn.onClick.AddListener(Continue);
        Exit_Btn.onClick.AddListener(Exit);
    }


    void Continue()
    {

        GameAudio.instance.PlayAudioSourceUI("click_btn");
        GameAudio.instance.PlayAudioSourceBG("Main");

        Close();
        GamePanel.Close();
        //发送回到房间界面的命令
        RoomCommand.TurnBackCurrRoom(GameCommand.currentRoom.roomID);
    }

    void Exit()
    {
        GameAudio.instance.PlayAudioSourceUI("close_btn");
        GameAudio.instance.PlayAudioSourceBG("Main");


        Close();
        GamePanel.Close();

        //退出房间
        RoomCommand.ExitRoom(GameCommand.currentRoom.roomID,NetStart.myInfo.id);

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
