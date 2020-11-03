using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

/****************************************************
	项 目 名 称：
    作		者：刘光和
    功		能：
	修 改 时 间：
*****************************************************/

public class RoomPanel : TPanel<RoomPanel> 
{
    public Button close_B;
    public Button start_B;
    public Button inviteList_B;
    public InputField inPutField;
    public Transform playerPos;//玩家实例的位置
    GameObject playerPrefb;//玩家预制件
    public Transform labelPos;//标签实例的位置
    public static RoomInfo room;

    void Start () 
	{
        close_B.onClick.AddListener(ExitRoom);
        start_B.onClick.AddListener(StartGame);
    }

    /// <summary>
    /// 退出房间
    /// </summary>
    void ExitRoom()
    {
        GameAudio.instance.PlayAudioSourceUI("close_btn");
        MMunePanel.Open();
        Close();
        RoomCommand.ExitRoom(NetStart.myInfo.roomNum, NetStart.myInfo.id);//发送退出命令给服务器
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    void StartGame()
    {
        GameAudio.instance.PlayAudioSourceUI("click_btn");
        if (playerPos.childCount>1)
        {
            bool enableStart = true;
            for (int i = 0; i < room.member.Count; i++)
            {
                Debug.Log("成员状态："+ room.member[i].IsInWaitRoom);
                if (room.member[i].IsInWaitRoom == false)
                {
                    enableStart = false;
                    Label.CreatLabel(Label.Standard, labelPos, Vector3.zero, room.member[i].name+ "玩家没有退出战斗不能开启游戏", Color.white, null);
                    break;
                }
            }
           
            if (enableStart)
            {
                GameCommand.Begin(NetStart.myInfo.roomNum);
            }
        }
        else
        {
             Label.CreatLabel(Label.Standard, labelPos,Vector3.zero, "开始游戏满足条件：房间人数 2-4人", Color.white, null);
        }
    }

    /// <summary>
    /// 显示可邀请的好友列表（实例化窗口预制件再实例化好友）
    /// </summary>
    public void ForeachPlayerList(RoomInfo roomInfo)
    {
        if (roomInfo.host_Id != NetStart.myInfo.id)//非房主的禁用开始游戏按钮
        {
            start_B.gameObject.SetActive(false);
        }
        else
        {
            start_B.gameObject.SetActive(true);
        }

        playerPrefb = (GameObject)Resources.Load("Prefabs/room/Player_Prefb");//成员预制件加载
        if (roomInfo.member.Count > 0)
        {
            if (playerPos.childCount > 0)
            {
                for (int i = 0; i < playerPos.childCount; i++)
                {
                    Destroy(playerPos.GetChild(i).gameObject);//先删除之前的好友
                }
            }
            //Debug.Log("清除完毕:");
            for (int i = 0; i < roomInfo.member.Count; i++)
            {
                Display(roomInfo.member[i]);
            }
        }
    }

    /// <summary>
    /// 显示房间成员
    /// </summary>
    /// <param name="personalInfo"></param>
    public void Display(PersonalInfo personalInfo)
    {
        if (!playerPrefb)
        {
            playerPrefb = (GameObject)Resources.Load("Prefabs/room/Player_Prefb");
        }
        GameObject player = Instantiate(playerPrefb, playerPos);
        Image status = player.transform.Find("status").GetComponent<Image>();

        if (personalInfo.IsInWaitRoom)
        {
            status.color = Color.yellow;
        }
        else
        {
            status.color = Color.red;
        }
 
        player.transform.Find("playerName").GetComponent<Text>().text = personalInfo.name;
        // player.transform.Find("out_B").GetComponent<Button>().onClick.AddListener(() => );
        //player.transform.Find("addFri_B").GetComponent<Button>().onClick.AddListener(() =>));
    }
}
