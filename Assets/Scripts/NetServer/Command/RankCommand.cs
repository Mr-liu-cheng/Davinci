using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankCommand : Command
{
    const int TYPE =6;//排行命令

    public static List<PersonalInfo> rankList;

    public RankCommand() { }

    public override void Init(byte[] bts)
    {
        base.Init(bts);
    }

    public override void DoCommand()//接收排行榜
    {
        //Rank.loadRank = true;
        //服务器是封装的是什么类型，客户端解析的时候就用什么类型去解
        rankList = DataDo.Json2Object<List<PersonalInfo>>(Decode.DecodFirstContendBtye(bytes));
        if (MMunePanel.Get())  MMunePanel.Get().UpdateRankList(rankList);
    }

    /// <summary>
    /// 排行榜查询
    /// </summary>
    public static void Rank_Send()
    {
        byte[] byt = System.Text.Encoding.UTF8.GetBytes("");
        NetStart.SendContend(Incode.IncodeFirstCommand(TYPE, byt));//获取查询排行的命令
        //Debug.Log("发送排行榜命令");
    }
}
