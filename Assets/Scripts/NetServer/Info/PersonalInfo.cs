using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum PersonStatus
{
    OffLine,
    OnLine,
    Combine,
    Fighting
}
public class PersonalInfo
//id PassWord  name mileage  icon sex age winRate serialWin  gameNum degree status roomNum
{
    public int id;
    public string password;
    public string name;
    public string markName;
    public string icon;
    public int sex;//0女1男，null未知
    public int age;
    public float winRate;
    public int serialWin;//最高连胜
    public int gameNum;//对局场次
    public int degree;//等级
    public int status;
    public string roomNum;
    public int coin;
    public bool IsInWaitRoom;

}

