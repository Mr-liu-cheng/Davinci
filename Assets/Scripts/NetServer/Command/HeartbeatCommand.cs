using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartbeatCommand : Command
{
    const int TYPE = 4;//心跳命令
    const int _HeartBeat = 0;//心跳命令
    const int _ReLink = 1;//断线重连


    HeartbeatCommand() { } 

    public override void Init(byte[] bts)
    {
        base.Init(bts);
    }

    public override void DoCommand()  {  }

    /// <summary>
    /// 心跳命令发送
    /// </summary>
    public static void Heartbeat_Send()
    {
        NetStart.SendContend(Incode.IncodeSecondaryCommand(TYPE, _HeartBeat, System.BitConverter.GetBytes(NetStart.myInfo.id)));
        //Debug.Log("发送心跳命令");
    }

    /// <summary>
    /// 断线重连
    /// </summary>
    public static void ReLink()
    {
        NetStart.SendContend(Incode.IncodeSecondaryCommand(TYPE, _ReLink, System.BitConverter.GetBytes(NetStart.myInfo.id)));
        Debug.Log("----------发送重连命令");

    }
}
