using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFriendCommand : Command
{
    const int TYPE = 3;//查找好友

    public static List<PersonalInfo> friendListInfo;

    public SelectFriendCommand() { }

    public override void Init(byte[] bts)
    {
        base.Init(bts);
    }
    public override void DoCommand()
    {
        friendListInfo = DataDo.Json2Object<List<PersonalInfo>>(Decode.DecodFirstContendBtye(bytes));
        //Debug.Log("查找到好友人数:" + friendListInfo.Count);
        if (MMunePanel.Get())
            MMunePanel.Get().UpdateFriendList(friendListInfo);
    }

    /// <summary>
    /// 查找好友
    /// </summary>
    public static void SelectFriendList()//加载在线好友列表 
    {
        byte[] select = Incode.IncodeFirstCommand(TYPE, BitConverter.GetBytes(NetStart.myInfo.id));
        NetStart.SendContend(select);//发送查好友命令到服务器
        Debug.Log("查找好友命令发送");
    }
}
