using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************
	项 目 名 称：
    作		者：刘光和
    功		能：
	修 改 时 间：
*****************************************************/

public class MessageInfo
{
    public string roomNum;//群聊房间号  接到消息先判断是否为群发在分析发送对象为空则是私发  也是多余的可以根据type判读私发群发，群发在获取自己所在房间号
    public int sendId;          //发送者的id
    public int toIds;     //接受者的id
    //public List<int> toIds;     //接受者的id
    public bool isReaded;   //是否已读
    public string content;//内容
    public float disposeTime;//处理时间
    //public static List<MessageInfo> unReadMsg = new List<MessageInfo>(); //未读邮件 
    public static Dictionary<int, List<MessageInfo>> unReadMsg = new Dictionary<int, List<MessageInfo>>(); //未读邮件 
    public static Dictionary<int, List<MessageInfo>> massageAll = new Dictionary<int, List<MessageInfo>>();//所有消息缓存 int好友id
}
