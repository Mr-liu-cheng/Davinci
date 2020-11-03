using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Linq;

public struct Info
{
    public int myId;
    public string roomId;
    public byte[] content;

    public Info(byte[] content)
    {
        this.myId = NetStart.myInfo.id;
        this.roomId = GameCommand.currentRoom.roomID;
        this.content = content;
    }
}

/// <summary>
/// 单摸桌牌信息
/// </summary>
public struct SingleDraw
{
    /// <summary>
    /// 托管判断
    /// </summary>
    public bool autoTrusteeship;
    public string roomNum;
    public int myId;
    public int myIndex;
    /// <summary>
    /// 卡牌下标【在猜错自选时是所公布牌对应的选项牌下标】
    /// </summary>
    public int cardIndex;
    /// <summary>
    /// 是否是连续猜牌
    /// </summary>
    public bool isContinueGuess;

}

/// <summary>
/// 摸牌信息
/// </summary>
public struct SelectCards 
{
    public string roomNum;
    public int myId;
    /// <summary>
    /// 黑牌数量
    /// </summary>
    public int blackCards;
    public int whiteCards;
}

/// <summary>
/// 猜牌时发送的信息
/// </summary>
public struct GuessInfo
{
    /// <summary>
    /// 要被删除牌的下标（选项窗口里的牌）
    /// </summary>
    public int delateIndex;
    /// <summary>
    /// 摸牌者的下标
    /// </summary>
    public int drawerIndex;
    /// <summary>
    /// 猜错时需要
    /// </summary>
    public BaseCard card;
    /// <summary>
    /// 被选中的玩家的下标
    /// </summary>
    public int whoIndex;
    /// <summary>
    /// 被选中牌的下标
    /// </summary>
    public int cardIndex;
    public string roomNum;
    public bool isGuessTrue;
    /// <summary>
    /// 是否是自动发送【只摸牌没猜牌时】
    /// </summary>
    public bool isAutoSend;
    /// <summary>
    /// 万能牌的插入位置
    /// </summary>
    public int lineCardSeatIndex;
}

public class GameCommand : Command
{
    const int TYPE = 8;//游戏命令
    public enum SecondCommands
    {
        /// <summary>
        /// 初始化
        /// </summary>
        INITIALIZE = 0,
        BEGINGAME ,
        /// <summary>
        /// 发牌
        /// </summary>
        DEAL,
        /// <summary>
        /// 猜先
        /// </summary>
        TOSS,
        /// <summary>
        /// 过 下一位
        /// </summary>
        PASS,
        /// <summary>
        /// 摸牌
        /// </summary>
        DRAW,
        GUESS ,
        SelectMyCardToOpen,
        AutoOpenOnesCard,
        Out,
        Over,
        CreateSelectMyCardTimer,
        Continue,
        Pass,
        MoveLineCardInFirstDraw,
        Already
    };

    /// <summary>
    /// 来的有点晚，该在初始化房间的时候就复制
    /// </summary>
    public static RoomInfo currentRoom = new RoomInfo();//当前房间

    /// <summary>
    /// 首摸时间【服务器的时间要比客户端的时间长1秒，为了更方便执行发牌命令】
    /// </summary>
    public static int _1stDrawTime;

    /// <summary>
    /// 单摸+猜牌的时间
    /// </summary>
    public static int _SingleDrawTime;

    /// <summary>
    /// 骰子动画的时间  
    /// 最好是让大家等待的时间相同，让服务器先等动画播完在开始计时，也就是将动画时间发给服务器
    /// </summary>
    public static int _DiceTime;

    /// <summary>
    /// 首摸移动万能牌的时间  
    /// </summary>
    public static int _MoveCardTime;

    /// <summary>
    /// 自选公布牌的时间
    /// </summary>
    public static int _SelectMyselfCardTime;

    /// <summary>
    /// 附加时间【猜对】
    /// </summary>
    public static int _ExtraTime;

    /// <summary>
    /// 按钮选择时间【猜对】
    /// </summary>
    public static int _ClickThinkingTime;

    public GameCommand() { }

    public override void Init(byte[] bts)
    {
        base.Init(bts);
    }
    //界面打开  初始化  建一副牌 发牌 掷骰子（选首玩家） 开始执行逻辑（摸排（放置一边） 
    //猜牌（选牌，选指定值，判定） 摊牌（归位） 下一位） 没牌直接猜 没有未知手牌（pass） 结束回合（回到房间界面） 
    public override void DoCommand()
    {
        int command = BitConverter.ToInt32(bytes, 8);
        switch (command)
        {
            case 0:
                Initialize();
                break;
            case 1:
                OpenGameScene();
                Debug.Log("开始游戏，跳转场景");
                break;
            case 2:
                byte[] sd = Decode.DecodSecondContendBtye(bytes);
                currentRoom = DataDo.Json2Object<RoomInfo>(sd);//必须在此时获取一次，里面携带玩家手牌信息
                GamePanel.Get().DealWithCards(currentRoom);
                Debug.Log("显示手牌");
                break;
            case 3:
                int[] dice = new int[2];
                dice[0] = BitConverter.ToInt32(bytes, 12);
                dice[1] = BitConverter.ToInt32(bytes, 16);
                Debug.Log("dice[0] :" + dice[0]+ "  dice[1] :" + dice[1]);
                GamePanel.Get().SetForthgoer(dice[0] + dice[1]);
                GamePanel.Get().CreatDice(dice);
                break;

            case 4:
                Next();//房间独立线程发出的
                break;
            case 5:
                Debug.Log("接收到摸排结果");
                byte[] content= Decode.DecodSecondContendBtye(bytes);
                GamePanel.Get().DisplayDraw(BitConverter.ToInt32(content, 0), DataDo.Json2Object<BaseCard>(content.Skip(4).ToArray()));
                break;
            case 6:
                Debug.Log("猜对了  将该牌公布");
                GamePanel.Get().KitheCard_T(DataDo.Json2Object<GuessInfo>(Decode.DecodSecondContendBtye(bytes)));
                break;
            case 7:
                Debug.Log("猜错了  将该牌公布");
                GuessInfo guessInfo = DataDo.Json2Object<GuessInfo>(Decode.DecodSecondContendBtye(bytes));
                GamePanel.Get().KitheCard_F(guessInfo.whoIndex, guessInfo.delateIndex, guessInfo.lineCardSeatIndex);
                break;
            case 8:
                Debug.Log("-------------------公布玩家自选的手牌");
                GamePanel.Get().OpenSelfSelect(DataDo.Json2Object<SingleDraw>(Decode.DecodSecondContendBtye(bytes)));
                break;
            case 9:
                Debug.Log("公布牌");
                GamePanel.Get().OpenCards(BitConverter.ToInt32(bytes,12));
                break;
            case 10:
                Debug.Log("-----------------执行出局命令");
                GamePanel.Get().DoOut(BitConverter.ToInt32(bytes, 12));
                break;
            case 11:
                Debug.Log("-------------------游戏结束");
                List<int> exitList = DataDo.Json2Object<List<int>>(Decode.DecodSecondContendBtye(bytes));
                GamePanel.Get().GameOver(exitList);
                break;
            case 12:
                Debug.Log("-------------------创建自选计时器");
                GamePanel.Get().CreatSelectSelfCardTimer();
                break;
            case 13:
                Debug.Log("-------------------继续猜牌");
                GamePanel.Get().DoContinue();
                break;
            case 14:
               
                break;
            case 15:
                Debug.Log("-------------------接收到移动万能牌的命令");
                Info info = DataDo.Json2Object<Info>(Decode.DecodSecondContendBtye(bytes));
                GamePanel.Get().DoMoveLineCard(info.myId, DataDo.Json2Object<List<LineCardMoveInfo>>(info.content));
                break;
            case 16:
                Debug.Log("-------------------玩家离线");
                GamePanel.Get().OffLine(BitConverter.ToInt32(bytes, 12));
                
                break;
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void Initialize()
    {
        Dictionary<string, int> timeList = DataDo.Json2Object<Dictionary<string, int>>(Decode.DecodSecondContendBtye(bytes));
        timeList.TryGetValue("_1stDrawTime", out _1stDrawTime);
        timeList.TryGetValue("_SingleDrawTime", out _SingleDrawTime);
        timeList.TryGetValue("_DiceTime", out _DiceTime);
        timeList.TryGetValue("_SelectMyselfCardTime", out _SelectMyselfCardTime);
        timeList.TryGetValue("_ExtraTime", out _ExtraTime);
        timeList.TryGetValue("_ClickThinkingTime", out _ClickThinkingTime);
        timeList.TryGetValue("_MoveCardTime", out _MoveCardTime);
    }

    /// <summary>
    /// 打开游戏场景
    /// </summary>
    void OpenGameScene()
    {
        RoomPanel.Close();
        GameAudio.instance.PlayAudioSourceBG("Guess");
        GameAudio.instance.PlayAudioSourceUI("start");

        //开始游戏就将房间信息保存，后面的命令（除玩家退出）基本就不会获取房间信息了
        currentRoom = DataDo.Json2Object<RoomInfo>(Decode.DecodSecondContendBtye(bytes));
        SceneManager.LoadScene("GameScene");
        UnityAction<Scene, LoadSceneMode> onLoaded = null;
        onLoaded = (Scene scene, LoadSceneMode mode) =>
        {
            GamePanel.Open().Init(currentRoom);
            SceneManager.sceneLoaded -= onLoaded;
        };
        SceneManager.sceneLoaded += onLoaded;
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    public static void Begin(string roomNum)
    {
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, (int)SecondCommands.BEGINGAME, System.Text.Encoding.UTF8.GetBytes(roomNum));
        NetStart.SendContend(byt);
    }

    public static void Already(string roomNum)
    {
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, (int)SecondCommands.Already, System.Text.Encoding.UTF8.GetBytes(roomNum));
        NetStart.SendContend(byt);
    }

    /// <summary>
    /// 发送手牌选择（开局首摸）
    /// </summary>
    public static void SendSeletFirstHandCards(SelectCards selectCards)
    {
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, (int)SecondCommands.DEAL,DataDo.Object2Json(selectCards));
        NetStart.SendContend(byt);
    }

    /// <summary>
    /// 下一位
    /// </summary>
    void Next()
    {
        //if (GamePanel.isOver)
        //{
        //    return;
        //}
        GamePanel.Get().RefreshNext(BitConverter.ToInt32(bytes, 12), _SingleDrawTime);
    }

    /// <summary>
    /// 发送单摸结果
    /// </summary>
    public static void SendSingleDrow(int secondCommands, SingleDraw singleDraw)
    {
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, secondCommands, DataDo.Object2Json(singleDraw));
        NetStart.SendContend(byt);
    }

    /// <summary>
    /// 发送猜牌时的结果
    /// </summary>
    public static void SendGuessCard(GuessInfo guessInfo)
    {
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, (int)SecondCommands.GUESS, DataDo.Object2Json(guessInfo));
        NetStart.SendContend(byt);
    }

    /// <summary>
    /// 发送猜错了创建自选计时器//正在自选要公布的手牌
    /// </summary>
    public static void SelectMyCardTimer()
    {
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, (int)SecondCommands.CreateSelectMyCardTimer, System.Text.Encoding.UTF8.GetBytes(currentRoom.roomID));
        NetStart.SendContend(byt);
    }

    /// <summary>
    /// 发送自己的id
    /// </summary>
    public static void SendMyID(int secondCommand) 
    {
        byte[] data_1 = BitConverter.GetBytes(NetStart.myInfo.id);
        byte[] data_2 = System.Text.Encoding.UTF8.GetBytes(currentRoom.roomID); //BitConverter.GetBytes(int.Parse(currentRoom.roomID));
        byte[] datas = new byte[data_1.Length + data_2.Length];
        Buffer.BlockCopy(data_1, 0, datas, 0, 4);
        Buffer.BlockCopy(data_2, 0, datas, 4, data_2.Length);
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, secondCommand, datas);
        NetStart.SendContend(byt);
    }

    /// <summary>
    /// 发送单纯的信号
    /// </summary>
    /// <param name="secondCommand"></param>
    public static void SendSign(int secondCommand)
    {
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE, secondCommand, System.Text.Encoding.UTF8.GetBytes(currentRoom.roomID));
        NetStart.SendContend(byt);
    }

    public static void SendContinue()
    {
        SendSign((int)SecondCommands.Continue);
    }

    public static void SendPass()
    {
        SendSign((int)SecondCommands.Pass);
    }


    /// <summary>
    /// 发送单摸结果
    /// </summary>
    public static void SendLineCardIndexs(List<LineCardMoveInfo> lineCardMoveInfos)
    {
        Info info = new Info(DataDo.Object2Json(lineCardMoveInfos));
        byte[] byt = Incode.IncodeSecondaryCommand(TYPE,(int)SecondCommands.MoveLineCardInFirstDraw, DataDo.Object2Json(info));
        NetStart.SendContend(byt);
        Debug.Log("--------SendLineCardIndexs---------");
    }
}

