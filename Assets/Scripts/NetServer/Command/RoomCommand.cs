using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


/// <summary>
/// 房间消息的部分封装类
/// </summary>
public struct RoomMsg
{
    public string roomNum;
    public PersonalInfo my;
    public int otherId;
    public int myId;
}

public class RoomCommand : Command
{
    public static Dictionary<string,RoomInfo> rooms = new Dictionary<string, RoomInfo>();//所有房间
    const int TYPE = 7;//房间命令

    /// <summary>
    /// 房间命令的枚举
    /// </summary>
    enum SecondCommands  
    {
        SELECTROOM = 1, CREATEROOM, TurnBackROOM, JOINROOM, EXITROOM, HOSTRANSFER, DISMISSROOM,
        INVITEROOM, REMOVEFROMROOM, ADDFRIEND, GAINFRIENDLIST, AMENDROOMNAME
    };
    //查找 开房 回到房间【战斗结束】 加入 退出  房主转让  解散房间  邀请加入  踢出房间  添加好友  获取可以邀请好友列表  修改房间名 

    public RoomCommand() { }

    public override void Init(byte[] bts)
    {
        base.Init(bts);
    }

    public override void DoCommand()
    {
        int command = BitConverter.ToInt32(bytes, 8);
        switch (command)
        {
            case 1://处理将闲置房间列发反馈给客户端
                rooms = DataDo.Json2Object<Dictionary<string, RoomInfo>>(Decode.DecodSecondContendBtye(bytes));//查找数据报错，解析成对象时
                if (RoomsHallPanel.Get()) RoomsHallPanel.Get().OperateRoom();
                Debug.Log("查找成功， 房间数：" + rooms.Count);
                break;
            case 2:
                Debug.Log("开房成功");

                break;
            case 3:
                //返回房间界面
                RoomInfo room = DataDo.Json2Object<RoomInfo>(Decode.DecodSecondContendBtye(bytes));
                RoomPanel.room = room;
                SceneManager.LoadScene("Scenes/MainScence");
                UnityAction<Scene, LoadSceneMode> onLoaded = null;
                onLoaded = (Scene scene, LoadSceneMode mode) =>
                {
                    RoomPanel.Open().ForeachPlayerList(room);
                    RoomPanel.Get().inPutField.text = room.roomName;
                    SceneManager.sceneLoaded -= onLoaded;
                };
                SceneManager.sceneLoaded += onLoaded;

               
                break;
            case 4:
                Debug.Log("加入房间：");
                RoomsHallPanel.Close();
                RoomInfo roomInfos = DataDo.Json2Object<RoomInfo>(Decode.DecodSecondContendBtye(bytes));
                RoomPanel.room = roomInfos;

                //判断是否是在房间界面
                if (SceneManager.GetActiveScene().name== "MainScence")
                {
                    RoomPanel.Open().ForeachPlayerList(roomInfos);
                    RoomPanel.Get().inPutField.text = roomInfos.roomName;
                    NetStart.myInfo.roomNum = roomInfos.roomID;
                }
                break;
            case 5:
                RoomInfo roomInfo5 = DataDo.Json2Object<RoomInfo>(Decode.DecodSecondContendBtye(bytes));
                RoomPanel.room = roomInfo5;
                RoomPanel roomPanel = RoomPanel.Get();
                if (roomPanel)
                {
                    roomPanel.inPutField.text = roomInfo5.roomName;
                    roomPanel.ForeachPlayerList(roomInfo5);
                }
                Debug.Log("已退出房间"+ roomInfo5.member.Count);
                break;
            case 6:
                Debug.Log("房主转让" );
                //被动接受
                break;
            case 8:
                Debug.Log("邀请加入");

                break;
            case 9:
                Debug.Log("踢出房间" );

                break;
            case 10:
                Debug.Log("添加好友" );

                break;
            case 11:
                Debug.Log("获取可以邀请好友列表" );

                break;
            case 12:
                Debug.Log("修改房间名");

                break;
        }
    }

    /// <summary>
    /// 获取所有闲置房间的列表
    /// </summary>
    public static void SelectRooms()
    {
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, (int) SecondCommands.SELECTROOM, System.Text.Encoding.UTF8.GetBytes (""));
        NetStart.SendContend(byt);
    }

    /// <summary>
    /// 开房
    /// </summary>
    /// <param name="room"></param>
    public static void CreatRoom(RoomInfo room)
    {
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, (int)SecondCommands.CREATEROOM, DataDo.Object2Json(room));
        NetStart.SendContend(byt);
    }

    /// <summary>
    /// 战斗结束后返回房间界面
    /// </summary>
    /// <param name="roomNum">房间号</param>
    public static void TurnBackCurrRoom(string roomNum) 
    {
        Info info = new Info
        {
            myId = NetStart.myInfo.id,
            roomId = roomNum,
            content = new byte[0]
        };

        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, (int)SecondCommands.TurnBackROOM , DataDo.Object2Json(info));
        NetStart.SendContend(byt);
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    /// <param name="roomNum">房间号</param>
    public static void JoinRoom(string roomNum, PersonalInfo my)
    {
        RoomMsg roomMsg = new RoomMsg
        {
            roomNum = roomNum,
            my= my,
        };
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, (int)SecondCommands.JOINROOM,DataDo.Object2Json(roomMsg));
        NetStart.SendContend(byt);
    }

    /// <summary>
    /// 退出房间
    /// </summary>
    /// <param name="roomNum">房间号</param>
    public static void ExitRoom(string roomNum, int peopleId)
    {
        RoomMsg roomMsg = new RoomMsg
        {
            roomNum = roomNum,
            otherId = peopleId,
        };
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, (int)SecondCommands.EXITROOM, DataDo.Object2Json(roomMsg));
        NetStart.SendContend(byt);
    }

    #region 未开发功能模块

    /// <summary>
    /// 房主转让
    /// </summary>
    public static void HostTransfer(string roomNum, int otherId)
    {
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, (int)SecondCommands.EXITROOM, System.Text.Encoding.UTF8.GetBytes(roomNum));
        NetStart.SendContend(byt);
    }

    /// <summary>
    /// 邀请加入
    /// </summary>
    public static void InviteToRoom(string roomNum, PersonalInfo my, int otherId)
    {
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, (int)SecondCommands.EXITROOM, System.Text.Encoding.UTF8.GetBytes(roomNum));
        NetStart.SendContend(byt);
    }

    /// <summary>
    /// 踢出房间(与退出房间一个事件)
    /// </summary>
    public static void RemoveFromRoom(string roomNum, int people)
    {
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, (int)SecondCommands.EXITROOM, System.Text.Encoding.UTF8.GetBytes(roomNum));
        NetStart.SendContend(byt);
    }

    /// <summary>
    /// 添加好友
    /// </summary>
    public static void AddFriendFromRoom(int otherId)
    {

    }

    /// <summary>
    /// 获取可以邀请好友列表
    /// </summary>
    public static void GainFriendList(int myid)
    {

    }

    /// <summary>
    /// 修改房间名
    /// </summary>
    public static void AmendRoomName(string roomNum)
    {
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, (int)SecondCommands.EXITROOM, System.Text.Encoding.UTF8.GetBytes(roomNum));
        NetStart.SendContend(byt);
    }
    #endregion
}

