using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/****************************************************
	项 目 名 称：
    作		者：刘光和
    功		能：
	修 改 时 间：
*****************************************************/

public class RoomsHallPanel : TPanel<RoomsHallPanel>
{
    public Button close_B;
    public Button creatRoom_B;
    public Transform RoomList;//房间实例的场地
    GameObject roomPrefb;//房间预制件

    void Start () 
	{
        close_B.onClick.AddListener(() => { 
            GameAudio.instance.PlayAudioSourceUI("close_btn"); 
            MMunePanel.Open();
            Close(); });
        creatRoom_B.onClick.AddListener(InitRoom);
        //roomPrefb = (GameObject)Resources.Load("Prefabs/room");

    }

    /// <summary>
    /// 响应按钮事件 初始化房间的操作
    /// </summary>
    void InitRoom()
    {
        GameAudio.instance.PlayAudioSourceUI("click_btn");
        NetStart.myInfo.roomNum = CreatRoom();//自己的信息修改
        RoomPanel.Open().Display(NetStart.myInfo);
        RoomPanel.Open().inPutField.text = NetStart.myInfo.id + "的房间";
        //RoomCommand.RefreshThisRoomInfo(NetStart.myInfo.roomNum);
        Close();
    }


    /// <summary>
    /// 初始化房间 
    /// </summary>
    public void OperateRoom()
    {
        roomPrefb = (GameObject)Resources.Load("Prefabs/RoomHall/room");
        if (RoomList.childCount > 0)
        {
            for (int i = 0; i < RoomList.childCount; i++)
            {
                Destroy(RoomList.GetChild(i).gameObject);//先删除之前的房间
            }
        }
        Debug.Log("清除完毕:");
        if (RoomCommand.rooms.Count > 0)
        {
            foreach (var item in RoomCommand.rooms)
            {
                DisplayAllRooms(item.Value);
            }
            //for (int i = 0; i < RoomCommand.rooms.Count; i++)
            //{
            //    DisplayAllRooms(RoomCommand.rooms[i]);
            //}
        }
    }

    /// <summary>
    /// 创建房间，发送房间信息到服务器进行添加 返回一个房间的ID（string）
    /// </summary>
    public string CreatRoom()
    {
        List<PersonalInfo> roomMember = new List<PersonalInfo>();
        List<int> roomTempMember = new List<int>();
        NetStart.myInfo.IsInWaitRoom = true;
        roomMember.Add(NetStart.myInfo);
        roomTempMember.Add(NetStart.myInfo.id);

        RoomInfo room = new RoomInfo
        {
            roomID = NetStart.myInfo.id.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss"),
            host_Id = NetStart.myInfo.id,
            member = roomMember,
            tempMember = roomTempMember,
            icon = NetStart.myInfo.icon,
            roomName = NetStart.myInfo.id + "的房间",
            Isbegin = false,
            sealers = new List<int>(),
        };
        Debug.Log("room.roomID:" + room.roomID);
        RoomCommand.CreatRoom(room);
        return room.roomID;
    }

    /// <summary>
    /// 实例化房间
    /// </summary>
    public void DisplayAllRooms(RoomInfo roomItem)
    {
        GameObject room = Instantiate(roomPrefb, RoomList);

        if (Resources.Load("UI/role_photo/" + roomItem.icon))
        {
            room.transform.Find("image").GetComponent<Image>().sprite = Resources.Load("UI/role_photo/" + roomItem.icon, typeof(Sprite)) as Sprite;
        }
        room.name = roomItem.host_Id.ToString();
        room.transform.Find("roomName").GetComponent<Text>().text = roomItem.roomName;
        room.transform.Find("num").GetComponent<Text>().text = roomItem.member.Count.ToString();
        room.GetComponent<Button>().onClick.AddListener(()=> { 
            GameAudio.instance.PlayAudioSourceUI("close_btn"); 
            RoomCommand.JoinRoom(roomItem.roomID, NetStart.myInfo); });//按钮事件 加入房间
    }
}
