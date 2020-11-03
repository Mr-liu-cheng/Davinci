using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/****************************************************
	项 目 名 称：
    作		者：刘光和
    功		能：序列化与反序列化只是需要该类型（包含属性[对字段的限定]、字段）去解析数据包，里面的方法不参与数据传输，所以需要用到的copy服务器该类的相关函数
	修 改 时 间：
*****************************************************/

/// <summary>
/// 客户端房间的成员是不做修改的
/// </summary>
public class RoomInfo  
{
    public string roomID;
    public const int MaxSIZE = 4;//房间最大容量
    public List<PersonalInfo> member;//成员id
    bool isbegin;
    public int host_Id;//房主id//也作为房间的id
    public string roomName;
    public string icon;
    private int timer = 0;//时间

    /// <summary>
    /// 临时成员，玩家出局就从列表中移除
    /// </summary>
    public List<int> tempMember;

    /// <summary>
    /// 记录当前出牌玩家的下标
    /// </summary>
    public int curr_playerIndex;

    /// <summary>
    /// 是否结束出牌，切换至下一家
    /// </summary>
    public bool isSwitch;


    public bool isStartTime;


    /// <summary>
    /// 记录有多少人已发送发牌的请求
    /// </summary>
    public List<int> sealers;

    /// <summary>
    /// 牌库（所有牌）（根据牌的归属划分）
    /// </summary>
    public List<BaseCard> cardsLibrary;

    /// <summary>
    /// 玩家手牌
    /// </summary>
    public List<BaseCard> playersCards;

    Thread thread;

    /// <summary>
    /// 计时器
    /// </summary>
    public int Time
    {
        get => timer;
        set
        {
            timer = value;
            //if (timer == GameCommand._SingleDrawTime)
            //{

            //    DoNext();
            //    Timer = 0;
            //}
        }
    }

    public bool Isbegin { get => isbegin; set => isbegin = value; }
}
