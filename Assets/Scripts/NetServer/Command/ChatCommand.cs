using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ChatCommand : Command
{
    const int TYPE = 2;//聊天命令
    const int PrivateChat = 1;//表示私聊
    const int RoomChat = 2;//表示群聊  实例消息在在当前房间

    public ChatCommand() { }
    
    public override void Init(byte[] bts)
    {
        base.Init(bts);
    }

    public override void DoCommand()
    {
        Debug.Log("接收到消息");

        GameAudio.instance.PlayAudioSourceUI("receiveMsg");
        MessageInfo info = DataDo.Json2Object<MessageInfo>(Decode.DecodSecondContendBtye(bytes));

        List<MessageInfo> msgList;
        info.isReaded = false;
        if (MessageInfo.unReadMsg.TryGetValue(info.sendId, out msgList)) msgList.Add(info);
        else
        {
            msgList = new List<MessageInfo>();
            msgList.Add(info);
            MessageInfo.unReadMsg.Add(info.sendId, msgList);
        }

        if (MessageInfo.massageAll.TryGetValue(info.sendId, out msgList)) msgList.Add(info);
        else
        {
            msgList = new List<MessageInfo>();
            msgList.Add(info);
            MessageInfo.massageAll.Add(info.sendId, msgList);
        }

        //在接收消息的时候将发件人添加到联系人中区便于统一实例化
        if (!NetStart.chatmanlist.Contains(info.sendId))
        {
            NetStart.chatmanlist.Add(info.sendId);
        }

        ChatPanel chatPanel = ChatPanel.Get();
        if (chatPanel)
        {
            chatPanel.UpdateMsg();
        }
    }

    public static void Send(MessageInfo message)
    {
        NetStart.SendContend(Incode.IncodeSecondaryCommand(TYPE, PrivateChat, DataDo.Object2Json(message)));
    }
}