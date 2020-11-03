using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;


public class LineCardMoveInfo
{
    public int originalIndex;
    public int moveToIndex;
}

class GamePanel : TPanel<GamePanel>
{
    #region 属性

    enum DrawCardCommand
    {
        /// <summary>
        /// 首摸桌牌（3/4张）
        /// </summary>
        FirstDrawDeskCard,
        /// <summary>
        /// 单摸桌牌
        /// </summary>
        SingleDrawDeskCard,
        /// <summary>
        /// 选择玩家手牌
        /// </summary>
        DrawPlayerCard,
        /// <summary>
        /// 选择猜测的牌【基本用不上，窗口只有在选择玩家手牌才会弹出】
        /// </summary>
        DrawGuessCard,
        /// <summary>
        /// 选择自己要公布的牌
        /// </summary>
        DrawMyCardToOpen
    }

    public  bool isOver;

    /// <summary>
    /// 玩家坐标数组集合 （0位当前玩家位）
    /// </summary>
    public Transform[] _4Players;

    /// <summary>
    /// 房间信息
    /// </summary>
    RoomInfo roomInfo; 

    /// <summary>
    /// 存储当前的玩家阵列
    /// </summary>
    Transform[] _Players;

    /// <summary>
    /// 我所在成员列表的下标位置
    /// </summary>
    int myIndex;

    /// <summary>
    /// 牌库位子
    /// </summary>
    public Transform cardsLibraryPos;

    /// <summary>
    /// 所有牌
    /// </summary>
    List<GameObject> sameTagCards;

    /// <summary>
    /// 猜测可能性结果(有牌被公开就要移除该牌)
    /// </summary>
    List<BaseCard> optionsCards; 

    /// <summary>
    /// 骰子位置
    /// </summary>
    public Transform dicePos;

    /// <summary>
    /// 计时器
    /// </summary>
    GameObject timer;

    /// <summary>
    /// 可能性弹窗
    /// </summary>
    Transform selectOptionsWindow;

    /// <summary>
    /// 选择选项窗口里的牌
    /// </summary>
    GameObject resultCard;

    /// <summary>
    /// 保存实例化出的插入的位置点【便于删除】
    /// </summary>
    List<GameObject> insertSeats;

    /// <summary>
    /// 万能牌移动的下标【可能存在俩张】
    /// </summary>
    List<LineCardMoveInfo> lineCardMoveInfos;

    /// <summary>
    /// LineCardMoveInfo万能牌的序号
    /// </summary>
    int num = 0;

    /// <summary>
    /// 手牌区中的万能牌
    /// </summary>
    List<GameObject> lineCards;

    /// <summary>
    /// 摸牌数
    /// </summary>
    public int drawCardCount;

    /// <summary>
    /// 自己首摸时摸得白牌和黑牌总和
    /// </summary>
    int cardsCount;

    /// <summary>
    /// 开局玩家(自己)选手牌
    /// </summary>
    public int black_Select, white_Select;

    /// <summary>
    /// 玩家手牌集
    /// 只在初始化玩家手牌时用过，所以后面使用时要注意
    /// </summary>
    public Dictionary<int, List<BaseCard>> playersCards = new Dictionary<int, List<BaseCard>>();//创建实例

    /// <summary>
    /// 我的隐藏牌的数量（在桌牌摸完的时候查找赋值）
    /// </summary>
   // Dictionary<int, int> playersHideCards;

    /// <summary>
    /// 选中的牌
    /// </summary>
    Image seletedCard;

    /// <summary>
    /// 提交按钮
    /// </summary>
    GameObject submit;

    /// <summary>
    /// 单选别的玩家的手牌
    /// </summary>
    Image selectOhtersCard;

    /// <summary>
    /// 选中的牌
    /// </summary>
    GameObject selectCard;

    /// <summary>
    /// 猜对的按钮
    /// </summary>
    GameObject guess_TBtns;

    /// <summary>
    /// 选项界面的父级位置
    /// </summary>
    public Transform selectOptionsPos;

    /// <summary>
    /// 候牌区的牌
    /// </summary>
    BaseCard waitCard;

    /// <summary>
    /// 候选区牌的贴图
    /// </summary>
    Image waitImage;

    /// <summary>
    /// 是否提交
    /// </summary>
    bool isSubmit;

    /// <summary>
    /// 临时排行
    /// </summary>
   // public List<int> tempRankList;

    /// <summary>
    /// 继续【猜对选项】
    /// </summary>
    public Button continue_Btn;

    /// <summary>
    /// 下一位【猜对选项】
    /// </summary>
    public Button next_Btn;

    /// <summary>
    /// 当前玩家
    /// </summary>
    int currPlayerInex;

    #endregion

    #region 战斗房间初始化

    /// <summary>
    /// 初始化场景（选择几人模式）
    /// </summary>
    /// <param name="count">房间人数</param>
    public void Init(RoomInfo room)
    {
        //ClearData();

        guess_TBtns = transform.Find("guess_TBtn").gameObject; 
        guess_TBtns.transform.Find("Continue_B").GetComponent<Button>().onClick.AddListener(Continue);
        guess_TBtns.transform.Find("Next_B").GetComponent<Button>().onClick.AddListener(Pass);
        //tempRankList = new List<int>();
        roomInfo = room;//赋值 便于访问
        Debug.Log("活跃人数：" + roomInfo.tempMember.Count);
        int count = room.member.Count;
        switch (count)
        {
            case 2:
                drawCardCount = 4;
                _2P(count, room);
                break;
            case 3:
                drawCardCount = 4;
                _3P(count, room);
                break;
            case 4:
                drawCardCount = 3;
                _4P(count, room);
                break;
        }
        optionsCards = room.cardsLibrary.OrderBy(u => u.CardId).ToList();
        DeskCards(room,false);
        AllowMyOperation("libraryCard", DrawCardCommand.FirstDrawDeskCard);
        CreatTimer(GameCommand._1stDrawTime, myIndex, SendSeletResult);
        //发送已准备好的命令
        GameCommand.Already(room.roomID);

        //创建并禁用，为了在其他玩家删除选项牌时我们已初始化
        CreateOptions();
        selectOptionsPos.GetChild(0).gameObject.SetActive(false);
    }

    /// <summary>
    /// 继续【+5】【信号发出去，关闭权限】
    /// </summary>
    void Continue()
    {
            GameAudio.instance.PlayAudioSourceUI("click_btn"); 
        Debug.Log("++++++++Continue++++++++");
        isSubmit = true;
        BanMyOperation(DrawCardCommand.DrawGuessCard,false);
        GameCommand.SendContinue();
        //隐藏按钮
        guess_TBtns.SetActive(false);
        isSubmit = false;
    }

    /// <summary>
    /// 下一位(过)【信号发出去，关闭权限】
    /// </summary>
    void Pass()
    {
            GameAudio.instance.PlayAudioSourceUI("click_btn"); 
        Debug.Log("+++++++++Pass+++++++");
        isSubmit = true;
        BanMyOperation(DrawCardCommand.DrawGuessCard, false);
        GameCommand.SendPass();
        //隐藏按钮
        guess_TBtns.SetActive(false);
        isSubmit = false;
    }

    /// <summary>
    /// 执行继续【接受到信号，计时器延时，激活单摸桌牌的权限】[在按钮时的时间没有处理]
    /// </summary>
    public void DoContinue()
    {
        Debug.Log("++++++++DoContinue+++++++++");
        Calculagraph calculagraph = timer.GetComponent<Calculagraph>();
        calculagraph.Paste(GameCommand._ExtraTime);//GameCommand._ExtraTime
        calculagraph.methodDelegate = null;
        //和RefreshNext的操作一样【采用猜牌阶段的托管】
            
        if (currPlayerInex==myIndex)
        {
            calculagraph.methodDelegate += () => BanMyOperation(DrawCardCommand.DrawGuessCard, true);

            //判断是否还有桌牌
            if (cardsLibraryPos.childCount==0)
            {
                AllowMyOperation("othersCard", DrawCardCommand.DrawPlayerCard);
            }
            else
            {
                AllowMyOperation("libraryCard", DrawCardCommand.SingleDrawDeskCard);
            }
        }
    }

    /// <summary>
    /// 创建定时器
    /// </summary>
    /// <param name="timeSize">时间</param>
    /// <param name="memberIndex">玩家在成员列表中的下标</param>
    /// <param name="delegateFuc">委托方法（计时器到点执行的函数）</param>
    public void CreatTimer(int timeSize,int memberIndex, TimerMethodDelegate delegateFuc) 
    {
        if (timer!=null)
        {
            DestroyImmediate(timer);
        }
        timer= Calculagraph.CreatTimer(timeSize, delegateFuc, _Players[GetIndexInPlayerPos(memberIndex)].Find("Timer").GetComponent<Transform>(),Vector3.zero);
    }

    /// <summary>
    /// 初始化成员 【找到自己在成员列表所对应的下标,然后从这里开始遍历成员列表（全部走一遍），组件属性赋值】
    /// </summary>
    /// <param name="count">人数</param>
    /// <param name="room">房间信息</param>
    /// <param name="_Players">玩家数组（预制位置）</param>
    /// <returns></returns>
    void InitMember(int count, RoomInfo room, Transform[] _Players)
    {
        //找到自己在成员列表所对应的下标
        myIndex = FindInRoomMembers(NetStart.myInfo.id);
        for (int i = 0; i < count; i++)
        {
            int index = (myIndex + i) % count;//用取余可以循环取值
            //数组对象的第一个是本人，第二个就是成员表里的我的下标的下一个人
            _Players[i].transform.name = room.member[index].id.ToString();
            _Players[i].Find("InfoPanel/Head").GetComponent<Image>().sprite = Resources.Load("UI/role_photo/" + room.member[index].icon, typeof(Sprite)) as Sprite;
            _Players[i].Find("InfoPanel/Name").GetComponent<Text>().text = room.member[index].name;
            _Players[i].Find("InfoPanel/Gold/Text").GetComponent<Text>().text = room.member[index].coin.ToString();
        }

    }

    /// <summary>
    /// 俩人对战
    /// </summary>
    /// <param name="count"></param>
    /// <param name="room"></param>
    void _2P(int count, RoomInfo room)
    {
        _Players = new Transform[2] { _4Players[0], _4Players[2] };//赋值 存储当前的玩家阵列
        InitMember(count, room, _Players);
        //禁用无关玩家
        _4Players[1].gameObject.SetActive(false);
        _4Players[3].gameObject.SetActive(false);
    }

    /// <summary>
    /// 三人对战
    /// </summary>
    /// <param name="count"></param>
    /// <param name="room"></param>
    void _3P(int count, RoomInfo room)
    {
        _Players = new Transform[3] { _4Players[0], _4Players[1], _4Players[3] };//赋值 存储当前的玩家阵列
        InitMember(count, room, _Players);
        //禁用无关玩家
        _4Players[2].gameObject.SetActive(false);
    }

    /// <summary>
    /// 四人对战
    /// </summary>
    /// <param name="count"></param>
    /// <param name="room"></param>
    void _4P(int count, RoomInfo room)
    {
        _Players = _4Players;//赋值 存储当前的玩家阵列
        InitMember(count, room, _Players);
    }

    void ClearData()
    {
        playersCards.Clear();
        isOver = false;
        drawCardCount = 0;
        black_Select = 0;
        white_Select = 0;
    }

    /// <summary>
    /// 桌牌（牌库）
    /// </summary>
    void DeskCards(RoomInfo room,bool isAfterFirstDraw)
    {
        GameObject cardPrefbs = (GameObject)Resources.Load("Prefabs/game/Card");//卡牌预制件加载
        foreach (var item in room.cardsLibrary)
        {
            GameObject card;
            card = Instantiate(cardPrefbs, cardsLibraryPos);//实例牌
            card.transform.GetChild(0).name = item.CardWeight.ToString();
            card.GetComponent<Cardcs>().card = item;
            if (item.CardColor == CardColors.BLACK)//黑牌
            {
                card.name = "blackCard(B)(Clone)"; 
                card.GetComponent<Image>().sprite = Resources.Load("UI/cards/b_beside", typeof(Sprite)) as Sprite;
            }
            else if (item.CardColor == CardColors.WHITE)//白牌
            {
                card.name = "whiteCard(B)(Clone)";
                card.GetComponent<Image>().sprite = Resources.Load("UI/cards/b_beside_w", typeof(Sprite)) as Sprite;
            }
            Toggle toggle = card.GetComponent<Toggle>();
            if (isAfterFirstDraw)
            {
                toggle.onValueChanged.AddListener((bool isOn) => { Debug.Log("DeskCards----SingleDrawDeskCard"); DrawCards(card, isOn, DrawCardCommand.SingleDrawDeskCard); });
                toggle.group = cardsLibraryPos.GetComponent<ToggleGroup>();
            }
            else
            {
                toggle.onValueChanged.AddListener((bool isOn) => { Debug.Log("DeskCards----FirstDrawDeskCard"); DrawCards(card, isOn, DrawCardCommand.FirstDrawDeskCard); });
            }
        }
    }

    #endregion

    #region 首摸

    /// <summary>
    /// 处理牌
    /// 删除 添加到各自玩家手牌集合中去，排序，显示
    /// </summary>
    /// <param name="room"></param>
    public void DealWithCards(RoomInfo room)
    {
        Debug.Log("--------DealWithCards---------");

        Vector3 pos = cardsLibraryPos.localPosition - new Vector3(0, 30, 0);
        Label.CreatLabel(Label.Standard, transform, pos, "若手牌中有万能牌则可以选定移动位置，没有则等待其他玩家选择", Color.white, null);
        CreatTimer(GameCommand._MoveCardTime, myIndex, () =>
        {
            DestroyInsertSeat();
            if (seletedCard!=null)
            {
                seletedCard.enabled = false;
                seletedCard = null;
            }
         
            if (lineCardMoveInfos.Count>0 && lineCardMoveInfos[0].moveToIndex > -1)
            {
                GameCommand.SendLineCardIndexs(lineCardMoveInfos);
            }
        });

        DeleteCards(room);

        AllotCards(room);

        DisplayHandCard(room);

        FindHandLineCard();
    }

    /// <summary>
    /// 删除摸了的桌牌
    /// </summary>
    /// <param name="room"></param>
    public void DeleteCards(RoomInfo room)
    {
        for (int i = 0; i < cardsLibraryPos.childCount; i++)
        {
            Destroy(cardsLibraryPos.GetChild(i).gameObject);
        }
        Debug.Log(room.cardsLibrary.Count + " ：桌牌 | 手牌： " + room.playersCards.Count); ;
        //重新实例化桌牌
        DeskCards(room,true);
    }

    /// <summary>
    /// 分配玩家手牌
    /// </summary>
    /// <param name="room"></param>
    public void AllotCards(RoomInfo room)
    {
        #region 添加玩家手牌
        List<BaseCard> cards;//对应玩家的手牌集合
        int id;
        Debug.Log("玩家handCardsCount：    " + room.playersCards.Count);

        foreach (var item in room.playersCards)
        {
            id = room.member[(int)item.CardBelongTo - 1].id;
            playersCards.TryGetValue(id, out cards);

            if (cards != null)
            {
                cards.Add(item);
            }
            else
            {
                cards = new List<BaseCard>();
                cards.Add(item);
                playersCards.Add(id, cards);
            }
        }
        #endregion

        #region 排序 （不用分情况，对家的牌实例化是对齐右下角即可）
        foreach (var item in playersCards)
        {
            //先按照权值排序在根据颜色排序 正序  具体情况 实例化方向（对齐） 在客户端的对象组件上设置 减少程序消耗
            item.Value.Sort(delegate (BaseCard x, BaseCard y)
            {
                int a = x.CardWeight.CompareTo(y.CardWeight);//升序 
                if (a == 0)
                    a = (x.CardColor).CompareTo(y.CardColor);//升序
                return a;
            });
        }
        #endregion
    }

    /// <summary>
    /// 显示手牌
    /// </summary>
    /// <param name="room"></param>
    public void DisplayHandCard(RoomInfo room)
    {
        int count = room.member.Count;
        int otherIndex;//找到其他成员列表所对应的下标
        GameObject cardPrefbs = (GameObject)Resources.Load("Prefabs/game/Card");//卡牌预制件加载
        string blackPath = "", whitePath = "";//卡牌路径
        for (int i = 0; i < count; i++)
        {
            otherIndex = room.member.FindIndex(it =>
            {
                if (it.id == room.member[i].id) return true;
                else return false;
            });
        
            int index = GetIndexInPlayerPos(FindInRoomMembers(room.member[i].id));



            //数组对象的第一个是本人，第二个就是成员表里的我的下标的下一个人
            if (!playersCards.ContainsKey(room.member[i].id))
            {
                Debug.Log("不存在该成员");
                return;
            }

            foreach (var item in playersCards[room.member[i].id])
            {
                //位置
                GameObject currcard = Instantiate(cardPrefbs, _Players[index].Find("cardLabry").transform);
                currcard.name = item.CardName;
                currcard.transform.GetChild(1).name = item.CardId.ToString();//以id做名字
                currcard.transform.GetChild(0).name =((int) item.CardWeight).ToString()+"+" +item.CardWeight.ToString();

                Toggle toggle = currcard.GetComponent<Toggle>();
                
                #region 其他玩家手牌显示
                if (room.member[i].id != NetStart.myInfo.id)
                {
                    //添加到对应玩家手牌区的组中 -----为实现单选
                    toggle.group = _Players[index].Find("cardLabry").GetComponent<ToggleGroup>();

                    currcard.tag = "othersCard";
                    //还涉及到摊牌
                    if (item.CardDisplay == CardDisplay.True)//显示
                    {
                        NomalDisplayCard(blackPath, whitePath, item, currcard);
                    }
                    else //隐藏
                    {
                        // 对面玩家 2/4人组位置分情况考虑
                        if ((room.member.Count == 2 && index == 1) || (room.member.Count == 4 && index == 2))
                        {
                            CardSetImg(item, currcard, "UI/cards/top", "UI/cards/top_w");
                        }
                        // 非对面玩家
                        else
                        {
                            CardSetImg(item, currcard, "UI/cards/b_beside", "UI/cards/b_beside_w");
                        }
                        //为其他玩家手牌添加点击响应事件
                        toggle.onValueChanged.AddListener((bool isOn) => { Debug.Log("DisplayHandCard---DrawPlayerCard"); DrawCards(currcard, isOn, DrawCardCommand.DrawPlayerCard); });
                    }
                }
                #endregion

                #region 自己手牌显示
                else
                {
                    toggle.group = _Players[0].Find("cardLabry").GetComponent<ToggleGroup>();
                    currcard.tag = "myCard";
                    //还涉及到摊牌
                    if (item.CardDisplay == CardDisplay.True)//显示
                    {
                        NomalDisplayCard(blackPath, whitePath, item, currcard);

                        currcard.transform.Rotate(new Vector3(45, -12, 0.5f));
                    }
                    else//隐藏
                    {
                        NomalDisplayCard(blackPath, whitePath, item, currcard);
                        toggle.onValueChanged.AddListener((bool isOn) => { Debug.Log("DisplayHandCard---DrawMyCard isOn:" + isOn); DrawCards(currcard, isOn, DrawCardCommand.DrawMyCardToOpen); });
                    }
                }
                #endregion
            }
            if (i != myIndex)
            {
                _Players[index].Find("cardLabry").gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 卡牌的正常显示（公开的）
    /// </summary>
    /// <param name="blackPath"></param>
    /// <param name="whitePath"></param>
    /// <param name="item"></param>
    /// <param name="currcard"></param>
    void NomalDisplayCard(string blackPath, string whitePath, BaseCard item, GameObject currcard)
    {
        blackPath = "UI/cards/weight/" + item.CardWeight.ToString();
        whitePath = "UI/cards/weight/" + item.CardWeight.ToString() + "_w";
        CardSetImg(item, currcard, blackPath, whitePath);
    }

    /// <summary>
    /// 设置牌的UI贴图
    /// </summary>
    void CardSetImg(BaseCard item, GameObject currcard, string blackPath, string whitePath)
    {
        if (item.CardColor == CardColors.BLACK)//黑牌
        {
            //Debug.Log("黑牌");
            currcard.GetComponent<Image>().sprite = Resources.Load(blackPath, typeof(Sprite)) as Sprite;
            currcard.transform.GetChild(0).tag = "black";
        }
        else if (item.CardColor == CardColors.WHITE)//白牌
        {
            //Debug.Log("白牌");
            currcard.GetComponent<Image>().sprite = Resources.Load(whitePath, typeof(Sprite)) as Sprite;
            currcard.transform.GetChild(0).tag = "white";
        }
    }

    /// <summary>
    /// 根据id得到在成员列表中的下标
    /// </summary>
    /// <param name="id">玩家id</param>
    /// <param name="room">可以不要,用roomInfo成员便字段代替</param>
    /// <returns></returns>
    int FindInRoomMembers(int id)
    {
        int otherIndex = roomInfo.member.FindIndex(it =>
        {
            if (it.id == id) return true;
            else return false;
        });
        return otherIndex;
    }

    /// <summary>
    /// 发送首牌选择结果(到时间就发送)
    /// </summary>
    public void SendSeletResult()
    {
        //删牌是在显示手牌前处理的
        BanMyOperation(DrawCardCommand.FirstDrawDeskCard, false);
        //没有做实时选择，只是将数目结果发给服务器
        SelectCards selectCards = new SelectCards
        {
            roomNum = NetStart.myInfo.roomNum,
            myId = NetStart.myInfo.id,
            blackCards = black_Select,
            whiteCards = white_Select
        };
        GameCommand.SendSeletFirstHandCards(selectCards);
        //AddToggleGroupAndAddListener();
    }

    #endregion

    #region 猜先/下一位

    /// <summary>
    /// 创建骰子
    /// </summary>
    public void CreatDice(int[] index)
    {
        GameObject dicePrefb = (GameObject)Resources.Load("Prefabs/game/Touzi");//卡牌（背面）预制件加载
        GameObject dice = Instantiate(dicePrefb, dicePos);//实例黑牌
        dice.GetComponent<Dice>().dices = index;
    }

    /// <summary>
    /// 猜先 
    /// </summary>
    public void SetForthgoer(int index)
    {
        for (int i = 0; i < _Players.Length; i++)
        {
            _Players[i].Find("cardLabry").gameObject.SetActive(true);
        }
        StartCoroutine(WaitForDice(index));
    }

    /// <summary>
    /// 等骰子执行完实例秒表和指定先行者  注意：服务器也要等这么长时间
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IEnumerator WaitForDice(int index)
    {
        yield return new WaitForSeconds(GameCommand._DiceTime);
        int randomIndex = index % roomInfo.member.Count;
        Debug.Log(index + "：骰子=====下标：" + randomIndex);
        //RefreshNext(roomInfo.member[randomIndex].id, GameCommand._SingleDrawTime);
    }

    /// <summary>
    /// 获得对应玩家在位子集合里的下标
    /// </summary>
    /// <param name="memberIndex">玩家在成员列表的下标</param>
    /// <returns></returns>
    int GetIndexInPlayerPos(int memberIndex)
    {
        //根据在成员列表的下标差距来计算出该玩家在场景中的位置（因为第一个是本人）
        int posIndex = (memberIndex - myIndex + roomInfo.member.Count) % roomInfo.member.Count;
        return posIndex;
    }

    /// <summary>
    /// 下一位刷新
    /// </summary>
    /// <param name="id"></param>
    public void RefreshNext(int id,int time)
    {
        Debug.Log("<color=red>------RefreshNext   剩余牌数：" + cardsLibraryPos.childCount+ "</color>");
       
        currPlayerInex = FindInRoomMembers(id);

        if (id == NetStart.myInfo.id)
        {
            Debug.Log("<color=green>下一位是自己</color>   ");
            //没桌牌了直接猜
            //因为服务器的计时器和客户端几乎同时进行（但是服务器还是要快一点），
            //RefreshNext要先于客户端托管系统（要执行许多复杂程序）【都是到点执行】
            if (cardsLibraryPos.childCount==0)//<=1
            {
                Debug.Log("<color=green>没牌</color>   ");
                AllowMyOperation("othersCard", DrawCardCommand.DrawPlayerCard);
            }
            else
            {
                Debug.Log("<color=green>有牌</color>   ");
                AllowMyOperation("libraryCard", DrawCardCommand.SingleDrawDeskCard);
            }
            //超时系统自己随机删除一个++++++++++++++++++++++++++++++++++++
            CreatTimer(time, currPlayerInex, () => { BanMyOperation(DrawCardCommand.DrawGuessCard, false); });//实例计时器到对应位置
        }
        else
        {
            Debug.Log("下一位是" + id);
            CreatTimer(time, currPlayerInex, null);//实例计时器到对应位置
        }
    }

    #endregion

    #region 摸牌/猜牌/公布自己的牌 操作

    /// <summary>
    /// 选择手牌(摸牌)  1.首摸  2.单摸  3.选择玩家手牌  4.选择猜测可能
    /// 错误：首摸（主动选）完后单摸桌牌有时会出现选框不显示的情况
    /// </summary>
    /// <param name="card">牌</param>
    /// <param name="isOn">开关</param>
    /// <param name="toggle">控件</param>
    /// <param name="isSingleDraw">是否为单摸</param>
    /// <param name="isGuessing">是否为猜牌</param>
    void DrawCards(GameObject card, bool isOn,DrawCardCommand status)
    {
        GameAudio.instance.PlayAudioSourceUI("draw");
        cardsCount = black_Select + white_Select;
        seletedCard = card.transform.GetChild(0).GetComponent<Image>();//--------原因是对象被删除，预制件放函数外
        if (isOn)//加
        {
            Debug.Log("----------DrawCards---isOn--true----");
            switch (status)
            {
                case DrawCardCommand.FirstDrawDeskCard://多摸
                    if (cardsCount < drawCardCount)
                    {
                        if (card.name == "blackCard(B)(Clone)")
                        {
                            black_Select++;
                        }
                        else if (card.name == "whiteCard(B)(Clone)")
                        {
                            white_Select++;
                        }
                        seletedCard.enabled = true;//复选框激活
                    }
                    Debug.Log("black_Select:" + black_Select + "     white_Select:" + white_Select);
                    break;
                case DrawCardCommand.DrawPlayerCard://选玩家的手牌
                    seletedCard.enabled = true;//复选框激活
                    selectOhtersCard = seletedCard;
                    selectCard = card;
                    if (submit == null)
                    {
                        CreateOptions();
                        CreateSubmitBtn(DrawCardCommand.DrawGuessCard);
                    }
                    break;
                default ://单摸
                    //selectCard = card;
                    seletedCard.enabled = true;//复选框激活
                    break;
            }
        }
        else//减
        {
            Debug.Log("----------DrawCards---isOn--false----");

            switch (status)
            {

                case DrawCardCommand.FirstDrawDeskCard:
                    if (seletedCard.enabled)
                    {
                        if (cardsCount >= 1)
                        {
                            if (card.name == "blackCard(B)(Clone)")
                            {
                                black_Select--;
                            }
                            else if (card.name == "whiteCard(B)(Clone)")
                            {
                                white_Select--;
                            }
                            seletedCard.enabled = false;//禁用复选框
                        }
                    }
                    Debug.Log("black_Select:" + black_Select + "     white_Select:" + white_Select);
                    break;
                default:
                    seletedCard.enabled = false;//禁用复选框
                    break;
            }
        }
        
    }

    /// <summary>
    /// 找到选择的手牌下标【改进在点击牌时就将用一个成员变量保存，点击其他的牌是覆盖其值】
    /// </summary>
    int FindSelectedCards(Transform transform)
    {
        int index=-1 ;
        //如果将桌牌的name改成下标 实时同步桌牌可这么做获取下标  不然只有遍历子集
        for (int i = 0; i < transform.childCount; i++)
        {
            Toggle toggle = transform.GetChild(i).GetComponent<Toggle>();
            if (toggle!=null && toggle.isOn == true)
            {
                index = i;
            }
        }
        if (index == -1) Debug.Log("没选牌");
        //Debug.Log("所选牌："+ index);
        return index;
    }

    /// <summary>
    /// 创建 提交/确认 按钮  “只有俩个可能执行此函数 1.猜牌中……2.猜牌结束”
    /// </summary>
    /// <param name="isGuessing">是否是猜牌，否则是isGuessed(false)</param>
    void CreateSubmitBtn(DrawCardCommand status)
    {
        GameObject submit_B_Prefb = (GameObject)Resources.Load("Prefabs/game/submit_B");//确认提交按钮
        submit = Instantiate(submit_B_Prefb, _Players[0].Find("submit_Pos"));
        if (status == DrawCardCommand.DrawGuessCard || status == DrawCardCommand.DrawMyCardToOpen)
        {
            submit.GetComponent<RectTransform>().localPosition = new Vector3(241, -215, 0);
        }
        //获得选择的牌下标

        submit.GetComponent<Button>().onClick.AddListener((UnityEngine.Events.UnityAction)(() => {
            GameAudio.instance.PlayAudioSourceUI((string)"click_btn");
            GetSelectCardIndexAndSend(status); }));
    }

    /// <summary>
    /// 获取单摸的牌的下标  分情况处理发送的命令（单摸、单摸玩家手牌、单选可能性） 
    /// 还要处理定时器的关闭（客户端）与重置（服务器）
    /// </summary>
    void GetSelectCardIndexAndSend(DrawCardCommand status)
    {
        int deskCardIndex;//被选中的桌牌
        int guessCardIndex;//被选中的可能性牌
        int playerCardIndex;//被选中的玩家手牌
        Debug.Log("  ----点击按钮---- ");
        switch (status)
        {
            case DrawCardCommand.SingleDrawDeskCard:
                #region 单摸桌牌
                deskCardIndex = FindSelectedCards(cardsLibraryPos);

                //resultCard = cardsLibraryPos.GetChild(deskCardIndex).gameObject;
                if (deskCardIndex >= 0)
                {
                    //发送单摸选择
                    SingleDraw singleDraw = new SingleDraw()
                    {
                        autoTrusteeship = false,
                        myId = NetStart.myInfo.id,
                        myIndex = myIndex,
                        roomNum = roomInfo.roomID,
                        cardIndex = deskCardIndex,
                    };

                    Debug.Log("  主动发送单摸桌牌 ");
                    GameCommand.SendSingleDrow((int)GameCommand.SecondCommands.DRAW, singleDraw);//发送摸排选择（单选）

                    BanMyOperation(status, false);
                }
                else
                {
                    //没选
                    Vector3 pos = cardsLibraryPos.localPosition - new Vector3(0, 30, 0);
                    Label.CreatLabel(Label.Standard, transform, pos, "没摸桌牌", Color.white, null);
                    return;
                }
                #endregion
                break;
            case DrawCardCommand.DrawGuessCard:
                #region 猜牌
                if (cardsLibraryPos.childCount > 0)
                {
                    Destroy(submit);
                }
                guessCardIndex = FindSelectedCards(selectOptionsWindow);
                if (guessCardIndex < 0)
                {
                    Vector3 pos = cardsLibraryPos.localPosition - new Vector3(0, 30, 0);
                    Label.CreatLabel(Label.Standard, transform, pos, "没选可能性牌", Color.white, null);
                    return;
                }
                resultCard = selectOptionsWindow.GetChild(guessCardIndex).gameObject;
                int playerId = int.Parse(selectCard.transform.parent.parent.name);
                playerCardIndex = FindSelectedCards(_Players[GetIndexInPlayerPos(FindInRoomMembers(playerId))].Find("cardLabry"));
          
                if (playerCardIndex<0)
                {
                    Vector3 pos = cardsLibraryPos.localPosition - new Vector3(0, 30, 0);
                    Label.CreatLabel(Label.Standard, transform, pos, "没选玩家未知手牌", Color.white, null);
                    return;
                }

                if (waitCard == null)
                {
                    Debug.Log("没桌牌了，直接判断所选的牌猜测结果");

                    JudgeGuess(guessCardIndex, playerCardIndex, playerId, status, -1);
                }
                else
                {
                    if (waitCard.CardWeight == CardWeight.Line)
                    {
                        DisplayEnableSeat(guessCardIndex, playerCardIndex, playerId, status);
                    }
                    else
                    {
                        FindLineCardInHand(guessCardIndex, playerCardIndex, playerId, status);
                    }
                }

                #endregion
                break;
            case DrawCardCommand.DrawMyCardToOpen:
                #region 选择自己的牌公布
                if (FindMyHideCards() == 1)//最后一张牌，意味着结束
                {
                    GameCommand.SendMyID((int)GameCommand.SecondCommands.Out);
                }
                int selectCardIndex = FindSelectedCards(_Players[0].Find("cardLabry"));//
                if (selectCardIndex >= 0)
                {
                    isSubmit = true;
                    //发送单摸选择
                    SingleDraw singleDraw = new SingleDraw()
                    {
                        myId = NetStart.myInfo.id,
                        myIndex = myIndex,
                        roomNum = roomInfo.roomID,
                        cardIndex = selectCardIndex
                    };
                    Debug.Log("  主动发送自选手牌 ");
                    GameCommand.SendSingleDrow((int)GameCommand.SecondCommands.SelectMyCardToOpen, singleDraw);//发送摸排选择（单选）
                    BanMyOperation(status, false);
                }
                else
                {
                    //没选
                    Vector3 pos = cardsLibraryPos.localPosition - new Vector3(0, 30, 0);
                    Label.CreatLabel(Label.Standard, transform, pos, "没选出自己手牌中要公布的牌", Color.white, null);
                    return;
                }
                #endregion 
                break;
            default:  //DrawCardCommand.DrawPlayerCard    DrawCardCommand.FirstDrawDeskCard
                break;
        }
      
    }

    /// <summary>
    /// 判断猜牌，并发送结果
    /// </summary>
    /// <param name="guessCardIndex"></param>
    /// <param name="playerCardIndex"></param>
    /// <param name="playerId"></param>
    /// <param name="status"></param>
    /// <param name="lineCardSeatIndex"></param>
    void JudgeGuess(int guessCardIndex, int playerCardIndex, int playerId, DrawCardCommand status, int lineCardSeatIndex)
    {
        Debug.Log("  ----lineCardSeatIndex---- " + lineCardSeatIndex);
        if (selectCard.name == resultCard.name)//猜对
        {
            GameAudio.instance.PlayAudioSourceUI("right");

            GuessInfo guess = new GuessInfo()
            {
                delateIndex = guessCardIndex,
                drawerIndex = myIndex,
                cardIndex = playerCardIndex,
                whoIndex = FindInRoomMembers(playerId),
                roomNum = roomInfo.roomID,
                isGuessTrue = true,
                lineCardSeatIndex= lineCardSeatIndex
            };
            Debug.Log("猜对*********相同");
            GameCommand.SendGuessCard(guess);//发送别人的牌信息
            guess_TBtns.SetActive(true);
        }
        else//猜错
        {
            GameAudio.instance.PlayAudioSourceUI("fault");

            Debug.Log("猜错*********不同");
            if (waitCard != null)//候选区有牌且猜错了
            {
                //获取候选区牌对应在选窗的位置下标,目的将其删除
                //候选牌（刚摸得）
                //报错
                int WaitCardIndex = selectOptionsPos.GetChild(0).Find(waitCard.CardName.ToString()).GetSiblingIndex();
                GuessInfo guess = new GuessInfo()
                {
                    delateIndex = WaitCardIndex,
                    drawerIndex = myIndex,
                    card = waitCard,//可能会报错 因为没桌牌时就是没赋值
                    whoIndex = myIndex,
                    roomNum = roomInfo.roomID,
                    isGuessTrue = false,
                    lineCardSeatIndex = lineCardSeatIndex
                };
                GameCommand.SendGuessCard(guess);//发送自己的牌信息
            }
            else//候选区没牌了且猜错了
            {
                isSubmit = true;
                BanMyOperation(status, false);
                //要发送  通知其他玩家我在选牌（创建秒表）
                GameCommand.SelectMyCardTimer();
                AllowMyOperation("myCard", DrawCardCommand.DrawMyCardToOpen);
                return;
            }
        }
        isSubmit = true;
        BanMyOperation(status, false);
    }

    /// <summary>
    /// 创建自选牌时的计时器
    /// 1.猜错 自己选择要公布哪一张牌  
    /// 2.自己创建计时器、按钮、操作权限 ， 服务器延时
    /// 3.托管 超时将自动发送（按托管方式）
    /// </summary>
    /// <param name="timeSize"></param>
    /// <param name="memberIndex"></param>
    /// <param name="delegateFuc"></param>
    public void CreatSelectSelfCardTimer()
    {
        if (currPlayerInex==myIndex)
        {
            
            CreatTimer(GameCommand._SelectMyselfCardTime, myIndex, () => BanMyOperation(DrawCardCommand.DrawMyCardToOpen, false));
        }
        else
        {
            CreatTimer(GameCommand._SelectMyselfCardTime, currPlayerInex, null);
        }
    }

    /// <summary>
    /// 创建提示语
    /// </summary>
    void CreatLabel(string resourcesPath,Transform parentTrasn,Vector3 pos,string content)
    {
        GameObject labelPrefb = Resources.Load<GameObject>(resourcesPath);
        Transform label = Instantiate(labelPrefb, parentTrasn).transform;
        label.position = pos;
        label.GetComponentInChildren<Text>().text = content;
    }

    /// <summary>
    /// 显示单摸牌结果(单摸桌牌)
    /// </summary>
    /// <param name="cardIndex"></param>
    /// <param name="card"></param>
    public void DisplayDraw(int cardIndex, BaseCard card)
    {

        //执行删除
        Destroy(cardsLibraryPos.GetChild(cardIndex).gameObject);
        waitCard = card;
        //Debug.Log("-----------------DisplayDraw------------------");
        //自动摸牌（托管）
        if (card.CardDisplay==CardDisplay.True)
        {
            //采取猜错的方式公布牌（将自己刚摸得牌公布）  查找下标
            KitheCard_F((int)card.CardBelongTo, selectOptionsPos.GetChild(0).Find(waitCard.CardName.ToString()).GetSiblingIndex(),-1);
           Debug.Log("-----------------DisplayDraw----------： " + (int)card.CardBelongTo +"  id: "+ roomInfo.member[(int)card.CardBelongTo].id);
        }
        else
        {
            GameAudio.instance.PlayAudioSourceUI("move");
            //显示
            int drawerIndexInPos = GetIndexInPlayerPos((int)card.CardBelongTo);
            Transform newAdd = _Players[drawerIndexInPos].Find("NewAdd");
            waitImage = newAdd.GetComponent<Image>();
            waitImage.enabled = true;


            //刚摸的牌
            if ((roomInfo.member.Count == 2 && drawerIndexInPos == 1) || (roomInfo.member.Count == 4 && drawerIndexInPos == 2))
            {
                CardSetImg(waitCard, newAdd.gameObject, "UI/cards/top", "UI/cards/top_w");
            }
            else if (drawerIndexInPos == 0)
            {
                NomalDisplayCard("", "", waitCard, newAdd.gameObject);
            }
            // 非对面玩家
            else
            {
                CardSetImg(waitCard, newAdd.gameObject, "UI/cards/b_beside", "UI/cards/b_beside_w");
            }
            //允许自己选择玩家手牌
            if (currPlayerInex==myIndex)
            {
                AllowMyOperation("othersCard", DrawCardCommand.DrawPlayerCard);
            }
        }

       
       
    }

    /// <summary>
    /// 创建选项
    /// </summary>
    void CreateOptions()
    {

        Debug.Log("-------CreateOptions---");

        if (selectOptionsPos.childCount > 0) 
        {
            Vector3 pos = cardsLibraryPos.localPosition - new Vector3(0, 30, 0);
            Label.CreatLabel(Label.Standard, transform, pos, "单选可能性牌作为你的猜测", Color.white, null);
            selectOptionsPos.GetChild(0).gameObject.SetActive(true);
            //CreateSubmitBtn(DrawCardCommand.DrawGuessCard);
            return;//意思是如果这个选项框已实例化就不再重新生成
        }
        GameObject optionWindowPrefb = Resources.Load<GameObject>("Prefabs/game/selectOptions");
        selectOptionsWindow = Instantiate(optionWindowPrefb, selectOptionsPos).transform;
        GameObject cardPrefb = Resources.Load<GameObject>("Prefabs/game/Card");
        
        foreach (var item in optionsCards)
        {
            GameObject card = Instantiate(cardPrefb, selectOptionsWindow);
            card.name = item.CardName;
            card.transform.GetChild(1).name = item.CardId.ToString();//以id做名字
            Image image = card.GetComponent<Image>();
            if (item.CardColor == CardColors.BLACK)
            {
                image.sprite = Resources.Load<Sprite>("UI/cards/weight/" + item.CardWeight);
            }
            else
            {
                image.sprite = Resources.Load<Sprite>("UI/cards/weight/" + item.CardWeight + "_w");
            }
            Toggle toggle = card.GetComponent<Toggle>();
            toggle.interactable = true;
            toggle.group = selectOptionsWindow.gameObject.GetComponent<ToggleGroup>();
            toggle.onValueChanged.AddListener((bool isOn) => { Debug.Log("CreateOptions---DrawGuessCard"); DrawCards(card, isOn, DrawCardCommand.DrawGuessCard); });
        }
    }

    #endregion

    #region 玩家权限管理/物件显示开关

    /// <summary>
    /// 允许玩家操作（"othersCard" ，"libraryCard" ， "myCard"）
    /// 1.首摸(x,x,x)2.单摸(o,x,x)3.单选玩家牌（o,o,x）[guessing] 4.单选猜测可能性（o,x,o）[guessed]
    /// </summary>
    /// <param name="which">哪一方的标签名</param>
    /// <param name="isSingleDraw">是否为单摸</param>
    /// <param name="isGuessing">正在猜牌（选玩家手牌）</param>
    /// <param name="isGuessed">结束猜牌（选择可能性）</param>
    void AllowMyOperation(string which, DrawCardCommand status)
    {
        Vector3 pos = cardsLibraryPos.localPosition - new Vector3(0, 30, 0);
        string str="";
        switch (status)
        {
            case DrawCardCommand.FirstDrawDeskCard:
                str = "首摸摸桌牌【（2/3人）/4张，4人/3张】";
                break;
            case DrawCardCommand.SingleDrawDeskCard:
                str = "单摸桌牌";
                break;
            case DrawCardCommand.DrawPlayerCard:
                str = "单选玩家未知手牌->猜测";
                break;
            case DrawCardCommand.DrawMyCardToOpen:
                str = "选自己未知手牌->公布";
                break;
        }
        
        Label.CreatLabel(Label.Standard, transform, pos, str, Color.white, null);

        Debug.Log(which + "--------------AllowMyOperation");
        // 允许 选择哪一方的牌（自己的/牌库的/别人的）
        //"othersCard" "libraryCard"  "myCard"
        sameTagCards = GameObject.FindGameObjectsWithTag(which).ToList();//将tag**的牌放到cards
        //Debug.Log(which + "的牌数：" + sameTagCards.Count);
        if (sameTagCards.Count > 0)
        {
            for (int i = 0; i < sameTagCards.Count; i++)
            {
                //默认都是关闭，需要时才打开
                Toggle toggle = sameTagCards[i].GetComponent<Toggle>();
                if (toggle != null)
                {
                    toggle.interactable = true;
                }

            }
        }
        if (status == DrawCardCommand.SingleDrawDeskCard || status == DrawCardCommand.DrawGuessCard || status == DrawCardCommand.DrawMyCardToOpen)
        {
            CreateSubmitBtn(status);
        }
    }

    /// <summary>
    /// 结束/禁止 自己的一切操作  
    /// </summary>
    /// <param name="status"></param>
    /// <param name="isContinueGuess">是否为继续猜牌</param>
    void BanMyOperation(DrawCardCommand status, bool isContinueGuess)
    {
        /// <summary>
        /// 禁用玩家选择
        /// 关闭之前选择的选框 关闭toggle【玩家手牌+可能性牌】
        /// 禁用候选区牌并赋空
        /// 删除提交按钮
        /// 关闭选择窗口
        /// </summary>

        DestroyInsertSeat();//考虑首摸摸到万能牌-------------------
        Debug.Log("  ---BanMyOperation---  ");
        if (status != DrawCardCommand.FirstDrawDeskCard)//单摸时
        {
            if (!isSubmit && (status == DrawCardCommand.DrawGuessCard || status == DrawCardCommand.DrawMyCardToOpen))
            {
                //如果到点了还没有提交，则触发托管【猜牌+自选】
                AutoTrusteeship(isContinueGuess);
                if (guess_TBtns.activeSelf)//托管执行跳过
                {
                    Pass();
                }
            }

            if (selectOhtersCard && selectCard)//关闭对手选框
            {
                selectOhtersCard.enabled = false;
                Toggle toggle = selectCard.GetComponent<Toggle>();
                if (toggle != null)
                {
                    selectCard.GetComponent<Toggle>().isOn = false;
                }
            }

            if (waitImage && waitImage.enabled && status == DrawCardCommand.DrawGuessCard)
            {
                waitImage.enabled = false;
            }

            if (selectOptionsPos.childCount > 0)
            {
                //关闭选择窗口3  关闭选框  
                Transform selectOptionsWin = selectOptionsPos.GetChild(0);
                int index = FindSelectedCards(selectOptionsWin);
                if (index > -1)
                {
                    selectOptionsWin.GetChild(index).GetChild(0).GetComponent<Image>().enabled = false;
                }
                selectOptionsPos.GetChild(0).gameObject.SetActive(false);
            }

            if (seletedCard && seletedCard.enabled)//可能性选项时
            {
                seletedCard.enabled = false;
                seletedCard = null;
            }

            //删除 提交按钮 （存在一定问题“ 实例化可能性选项时 是打算让按钮禁用而非删除 ”）
            if (submit)//因为单选玩家手牌时不会实例化按钮
            {
                for (int i = 0; i < submit.transform.parent.childCount; i++)
                {
                    Destroy(submit.transform.parent.GetChild(i).gameObject);//为了防止起冲突还是加list存起来再在集合里遍历DestroyImmediate
                }
            }
            isSubmit = false;
        }

        // 禁止 选择哪一方的牌（cards是之前启用是就赋值了，减少查询）
        if (sameTagCards.Count > 0)
        {
            for (int i = 0; i < sameTagCards.Count; i++)
            {
                //默认都是关闭，需要时才打开
                //if (cards[i] == null) return;
                if (sameTagCards[i])
                {
                    Toggle toggle = sameTagCards[i].GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        toggle.interactable = false;
                    }
                }
            }
        }

        //如果到点还没点击确认（且提交有内容），那么就发送服务器托管命令
    }

    /// <summary>
    /// 检查要删除的对象
    /// </summary>
    void CheckDelete()
    {
        //删除万能牌移动位置
        DestroyInsertSeat();

        //删除按钮
        if (submit)
        {
            for (int i = 0; i < submit.transform.parent.childCount; i++)
            {
                Destroy(submit.transform.parent.GetChild(i).gameObject);//为了防止起冲突还是加list存起来再在集合里遍历DestroyImmediate
            }
        }

        //删除计时器
        if (timer)
        {
            DestroyImmediate(timer);
        }

        //删除可能性选窗
        if (selectOptionsPos.childCount > 0)
        {
            //关闭选择窗口3  关闭选框  
            Transform selectOptionsWin = selectOptionsPos.GetChild(0);
            int index = FindSelectedCards(selectOptionsWin);
            if (index > -1)
            {
                selectOptionsWin.GetChild(index).GetChild(0).GetComponent<Image>().enabled = false;
            }
            selectOptionsPos.GetChild(0).gameObject.SetActive(false);

        }

        //关闭自己选框
        if (seletedCard && seletedCard.enabled)//可能性选项时
        {
            seletedCard.enabled = false;
            seletedCard = null;
        }

        //关闭对手选框
        if (selectOhtersCard && selectCard)//关闭对手选框
        {
            selectOhtersCard.enabled = false;
            Toggle toggle = selectCard.GetComponent<Toggle>();
            if (toggle != null)
            {
                selectCard.GetComponent<Toggle>().isOn = false;
            }
        }

        //关闭骰子
        if (dicePos.childCount > 0)
        {
            DestroyImmediate(dicePos.GetChild(0).gameObject);
        }
    }

    #endregion

    #region 托管

    /// <summary>
    /// 托管   俩种情况：1.在线   2.掉线（不执行等待【不实例化秒表】）
    /// 单摸桌牌的托管被猜牌的的托管所替代，因为虽然是不同的操作但却是在同一个计时器下执行的任务
    /// 托管类型根据 waitCard 桌牌数来确定
    /// </summary>
    /// <param name="isContinueGuess">是否是连续猜牌</param>
    void AutoTrusteeship(bool isContinueGuess)
    {
        Debug.Log("  ---AutoTrusteeship---  ");
        //不是实例化摸得牌   摸得牌直接公开
        if (cardsLibraryPos.childCount==0)//没牌了
        {
            Debug.Log("  ---没牌了---  ");
            FindMyHideCards();
            AutoSendOpenMyCard();
            return;
        }

        //只摸牌没猜牌的情况：
        if (waitCard != null)
        {
            Debug.Log("  ---只摸牌没猜牌的情况---  ");
            //获取候选区牌对应在选窗的位置下标,目的将其删除
            int WaitCardIndex = selectOptionsPos.GetChild(0).Find(waitCard.CardName.ToString()).GetSiblingIndex();
            GuessInfo guess = new GuessInfo()
            {
                delateIndex = WaitCardIndex,
                drawerIndex = myIndex,
                card = waitCard,//可能会报错 因为没桌牌时就是没赋值
                whoIndex = myIndex,
                roomNum = roomInfo.roomID,
                isGuessTrue = false,
                isAutoSend = true,
                lineCardSeatIndex = -1
            };
            GameCommand.SendGuessCard(guess);//发送自己的牌信息
        }
        else//没摸牌【托管自动摸牌】
        {
            SingleDraw singleDraw = new SingleDraw()
            {
                autoTrusteeship = true,
                myId = NetStart.myInfo.id,
                myIndex = myIndex,
                roomNum = roomInfo.roomID,
                cardIndex = 0,
                isContinueGuess= isContinueGuess
            };
            Debug.Log("  ---被动发送单摸桌牌命令---  ");
            GameCommand.SendSingleDrow((int)GameCommand.SecondCommands.DRAW,singleDraw);//发送摸排选择（单选）
        }
    }

    /// <summary>
    /// 查找玩家未知手牌数目
    /// </summary>
    int FindMyHideCards()
    {
        //if (playersHideCards != null)
        //{
        //    return;
        //}
        //playersHideCards = new Dictionary<int, int>();
        ////当桌牌牌摸完了可以查一次

        //for (int j = 0; j < roomInfo.member.Count; j++)
        //{
        //    Transform playerCardLabry = _Players[j].Find("cardLabry");
        //    int hidecCards = 0;

        //    for (int i = 0; i < playerCardLabry.childCount; i++)//手牌
        //    {
        //        if (playerCardLabry.GetChild(i).GetChild(1).tag == "hide")
        //        {
        //            hidecCards++;
        //        }
        //    }
        //    playersHideCards.Add(int.Parse(_Players[j].name), hidecCards);
        //}
        Transform playerCardLabry = _Players[0].Find("cardLabry");
        int hidecCards = 0;

        for (int i = 0; i < playerCardLabry.childCount; i++)//手牌
        {
            if (playerCardLabry.GetChild(i).GetChild(1).tag == "hide")
            {
                hidecCards++;
            }
        }
        return hidecCards;
    }

    /// <summary>
    /// 自动发送公布自己牌的命令信号
    /// </summary>
    void AutoSendOpenMyCard()
    {
        int count = FindMyHideCards();
        CheckOut(count);
        if (count > 1)
        {
            GameCommand.SendMyID((int)GameCommand.SecondCommands.AutoOpenOnesCard);
        }
    }

    void CheckOut(int count)
    {
        Debug.Log("------查找隐藏牌-------count:" + count);
        if (count == 1)//没隐藏牌
        {
            //如果我没有未知手牌就出局 (当公布 到最后一张牌时，说明我已经没有未知手牌了)
            //发送至服务器告知其他玩家
            GameCommand.SendMyID((int)GameCommand.SecondCommands.Out);
            Debug.Log("-------------出局命令发送");
        }
    }

    #endregion

    #region 出局

    public void OffLine(int id)
    {
        _Players[GetIndexInPlayerPos(FindInRoomMembers(id))].Find("InfoPanel/Head").GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1);
    }

    /// <summary>
    /// 公布手牌
    /// 每次公布玩牌都要检查该玩家手牌中是否还有未知牌（是否会被淘汰）,并且发送出局命令
    /// </summary>
    public void OpenCards(int id)
    {
        //遍历自己的手牌  将第一张未知的手牌公布
        int index = GetIndexInPlayerPos(FindInRoomMembers(id));
        Transform myCardLabry = _Players[index].Find("cardLabry");

        for (int i = 0; i < myCardLabry.childCount; i++)
        {
            Transform currcard = myCardLabry.GetChild(i);
            if (currcard.GetChild(1).tag == "hide")
            {

                DestroyOption(selectOptionsPos.GetChild(0).Find(currcard.name).GetSiblingIndex());
                //公布
                if (index == 0)//如果被选择方是自己
                {
                    //如果牌时自己的就调整牌的旋转和颜色 作为公示
                    currcard.eulerAngles = new Vector3(47.7f, 21.1f, 1.8f);
                    SetMyCardToOpen(currcard.gameObject);
                    //myHideCards -= 1;//公布自己隐藏的手牌
                }
                else
                {
                    string colorTag = currcard.GetChild(0).tag;
                    if (colorTag == "black")
                    {
                        currcard.GetComponent<Image>().sprite = Resources.Load("UI/cards/weight/" + currcard.GetChild(0).name.Split('+')[1], typeof(Sprite)) as Sprite;
                    }
                    else//出问题了黑牌会在其他玩家那显示白牌
                    {
                        currcard.GetComponent<Image>().sprite = Resources.Load("UI/cards/weight/" + currcard.GetChild(0).name.Split('+')[1] + "_w", typeof(Sprite)) as Sprite;
                    }
                }
                currcard.GetChild(1).tag = "open";
                DestroyImmediate(currcard.GetComponent<Toggle>());//删除Toggle组件
                break;
            }
        }
    }

    /// <summary>
    /// 出局
    /// </summary>
    /// <param name="id"></param>
    public void DoOut(int id)
    {
        int indexPos = GetIndexInPlayerPos(FindInRoomMembers(id));
        _Players[indexPos].Find("InfoPanel/Head").GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1);//头像
        //从房间内移除
        roomInfo.tempMember.Remove(id);
        Debug.Log("-----------------------------存活人数：" + roomInfo.tempMember.Count);
        Debug.Log("-----------------------------玩家" + id + "出局");
    }

    /// <summary>
    /// 游戏结束（服务器检查发送过来的结果）
    /// </summary>
    public void GameOver(List<int> list)
    {
        CheckDelete();
        OpenAllCards();
        if (list != null)
        {
            Debug.Log("-----------------------------离线");
            GameAudio.instance.PlayAudioSourceUI("over");
            GameOverPanel.Open().tempRankList = list;
            GameOverPanel.Get().RoomInfo = roomInfo;
        }
        else
        {
            Debug.Log("-----------------------------淘汰出局");
        }
        Debug.Log("-----------------------------结束");
        //加载化界面

    }

    /// <summary>
    /// 公布所有牌
    /// </summary>
    void OpenAllCards()
    {
        GameObject[] hideCards = GameObject.FindGameObjectsWithTag("hide");
        Debug.Log("hideCards-------------:" + hideCards.Length);
        if (hideCards.Length > 0)
        {
            for (int i = 0; i < hideCards.Length; i++)
            {
                GameObject card = hideCards[i].transform.parent.gameObject;
                Debug.Log("card.name----:" + card.name);
                if (card.name== "blackCard(B)(Clone)" || card.name == "whiteCard(B)(Clone)")
                {
                    continue;
                }
                if (card.transform.parent.parent.name == NetStart.myInfo.id.ToString())//如果被选择方是自己
                {
                    //如果牌时自己的就调整牌的旋转和颜色 作为公示
                    SetMyCardToOpen(card);
                    //将猜对可能性移除  以后再做（因为每个玩家的可能性项不同）
                    //实例化玩家选项（继续猜（默认，打卡操作权限）/下一位（按钮 ，发送服务器））
                }
                else
                {
                    //思路：玩家手牌gameobject.name=CardWeight  ，根据当前手牌选框的tag(“颜色”)来决定实例化
                    string str = card.transform.GetChild(0).name;
                    string[] sArray = str.Split('+');
                    if (card.transform.GetChild(0).tag == "black")
                    {
                        card.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/cards/weight/" + sArray[1]);
                    }
                    else
                    {
                        card.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/cards/weight/" + sArray[1] + "_w");
                    }
                }
            }
        }
    }

    #endregion

    #region 显示归位

    /// <summary>
    /// 删除被公布的选项牌
    /// </summary>
    private void DestroyOption(int index)
    {
        if (index >= 0)
        {
            DestroyImmediate(selectOptionsPos.GetChild(0).GetChild(index).gameObject);//-----
           // Debug.Log("删除++++++" + index + "---余：" + selectOptionsPos.childCount);

        }
    }

    /// <summary>
    /// 排序
    /// </summary>
    int Sort(Transform playerCardLabry)
    {
        int index = -1;
        for (int i = 0; i < playerCardLabry.childCount; i++)
        {
            string weightTxt = playerCardLabry.GetChild(i).GetChild(0).name;
            string[] weight = weightTxt.Split('+');
            if (int.Parse(weight[0]) == (int)CardWeight.Line)
            {
                continue;
            }
            if (int.Parse(weight[0]) == (int)waitCard.CardWeight)
            {
                if (waitCard.CardColor == CardColors.WHITE)
                {
                    index = i + 1;
                }
                else
                {
                    index = i;
                }
                break;
            }
            if (int.Parse(weight[0]) < (int)waitCard.CardWeight)
            {
                index = i + 1;
            }

            if (int.Parse(weight[0]) > (int)waitCard.CardWeight)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    /// <summary>
    /// 判断手牌中要插入位子的前一张是否是万能牌，并且实例化可选位置
    /// </summary>
    /// <param name="guessCardIndex"></param>
    /// <param name="playerCardIndex"></param>
    /// <param name="playerId"></param>
    /// <param name="status"></param>
    /// <param name="lineCardSeatIndex"></param>
    void FindLineCardInHand(int guessCardIndex, int playerCardIndex, int playerId, DrawCardCommand status)
    {
        Transform playerCardLabry = _Players[0].Find("cardLabry");
        int insertIndex = Sort(playerCardLabry);
        //所要插入的牌的前一张如果是万能牌那么就要让玩家选择安放的位置

        //会报错2020.1.16
        Debug.Log("  ----FindLineCardInHand---- insertIndex:" + insertIndex);
        if (insertIndex==0)
        {
            JudgeGuess(guessCardIndex, playerCardIndex, playerId, status, insertIndex);
            return ;
        }
        Debug.Log("insertIndex-1.name:" + playerCardLabry.GetChild(insertIndex - 1).GetChild(0).name.Split('+')[0]);

        if (playerCardLabry.GetChild(insertIndex - 1).GetChild(0).name.Split('+')[1] == CardWeight.Line.ToString())
        {
            insertSeats = new List<GameObject>();
            GameObject cardPrefb = (GameObject)Resources.Load("Prefabs/game/Card");//卡牌预制件加载
            for (int i = 0; i < 2; i++)
            {
                //实例化
                GameObject curCard = Instantiate(cardPrefb, playerCardLabry);
                //插入【序号】
                int index = insertIndex - 1 + i;//万能牌前后
                curCard.transform.SetSiblingIndex(insertIndex - 1 + 2 * i);
                //添加button
                DestroyImmediate(curCard.GetComponent<Toggle>());
                curCard.AddComponent<Button>().onClick.AddListener((UnityEngine.Events.UnityAction)(() => {

                    GameAudio.instance.PlayAudioSourceUI((string)"draw");
                    JudgeGuess(guessCardIndex, playerCardIndex, playerId, status, index);
                    //DestroyInsertSeat();
                }));
                //用列表存起来
                insertSeats.Add(curCard);
            }
        }
        else
        {
            JudgeGuess(guessCardIndex, playerCardIndex, playerId, status, insertIndex);
        }
        Debug.Log("  ----FindLineCardInHand---- ");
    }

    /// <summary>
    /// 公布卡牌（猜对）
    /// </summary>
    /// <param name="whoIndex">玩家下标</param>
    /// <param name="cardIndex">牌的下标</param>
    public void KitheCard_T(GuessInfo guessInfo)
    {
        GameAudio.instance.PlayAudioSourceUI("move1");

        //计时器延时
        Calculagraph calculagraph = timer.GetComponent<Calculagraph>();
        calculagraph.Crop(GameCommand._ClickThinkingTime);
        calculagraph.methodDelegate = null;
        if (currPlayerInex == myIndex)
        {
            calculagraph.methodDelegate += Pass;
        }

        int whoIndex = guessInfo.whoIndex;
        int cardIndex = guessInfo.cardIndex;

        //被猜的人
        int indexInPos = (whoIndex + roomInfo.member.Count) % roomInfo.member.Count;
        Transform playerTrans = _Players[GetIndexInPlayerPos(indexInPos)];
        Transform playerCardLabry = playerTrans.Find("cardLabry");
        GameObject card = playerCardLabry.GetChild(cardIndex).gameObject;

        if (waitCard != null)//如果waitCard存在值则说明刚刚摸了牌，就可以执行以下操作，反之只将玩家的手牌（被猜的）公布处理
        {
            //摸牌者  计算有问题  我是按二人算的----------------------------
            int drawerIndexInMember = (guessInfo.drawerIndex + roomInfo.member.Count) % roomInfo.member.Count;
            int drawerIndexInPos = GetIndexInPlayerPos(drawerIndexInMember);
            Transform drawerTrans = _Players[drawerIndexInPos];
            Transform drawerCardLabry = drawerTrans.Find("cardLabry");

            //禁用候牌区
            Image image = drawerTrans.Find("NewAdd").GetComponent<Image>();
            image.enabled = false;

            //删除选项牌
            DestroyOption(guessInfo.delateIndex);

            GameObject cardPrefbs = (GameObject)Resources.Load("Prefabs/game/Card");//卡牌预制件加载
            GameObject curCard = Instantiate(cardPrefbs, drawerCardLabry);
            // 对面玩家 2/4人组位置分情况考虑

            //标签事件
            Toggle toggle = curCard.GetComponent<Toggle>();
            toggle.group = drawerCardLabry.GetComponent<ToggleGroup>();
            if (drawerIndexInPos == 0)//摸牌者是自己
            {
                toggle.onValueChanged.AddListener((bool isOn) => { Debug.Log("KitheCard_T---DrawMyCard"); DrawCards(curCard, isOn, DrawCardCommand.DrawMyCardToOpen); });
            }
            else
            {
                toggle.onValueChanged.AddListener((bool isOn) => { Debug.Log("KitheCard_T---DrawPlayerCard"); DrawCards(curCard, isOn, DrawCardCommand.DrawPlayerCard); });
            }
            curCard.name = waitCard.CardName;
            curCard.transform.GetChild(1).name = waitCard.CardId.ToString();//以id做名字
            curCard.transform.GetChild(1).tag = "hide";
            curCard.transform.GetChild(0).name = ((int)waitCard.CardWeight).ToString() + "+" + waitCard.CardWeight.ToString();


            //对面（要分情况了）
            if ((roomInfo.member.Count == 2 && drawerIndexInPos == 1) || (roomInfo.member.Count == 4 && drawerIndexInPos == 2))
            {
                CardSetImg(waitCard, curCard, "UI/cards/top", "UI/cards/top_w");
                curCard.tag = "othersCard";

            }
            else if (drawerIndexInPos == 0)//自己
            {
                NomalDisplayCard("", "", waitCard, curCard);
                curCard.tag = "myCard";
            }
            // 非对面玩家
            else
            {
                curCard.tag = "othersCard";
                CardSetImg(waitCard, curCard, "UI/cards/b_beside", "UI/cards/b_beside_w");
            }

            InsertCard(guessInfo.lineCardSeatIndex, playerCardLabry, curCard);


            waitCard = null;//置空
        }

        //-------------------被猜方-------------------

        if (whoIndex == myIndex)//如果被选择方是自己
        {
            //检查未知手牌【出局】
            int count = FindMyHideCards();
            CheckOut(count);
            SetMyCardToOpen(card);
            card.transform.GetChild(1).tag = "open";
        }
        else
        {
            card.transform.GetChild(1).tag = "open";

            //思路：玩家手牌gameobject.name=CardWeight  ，根据当前手牌选框的tag(“颜色”)来决定实例化
            string str = card.transform.GetChild(0).name;
            string[] sArray = str.Split('+');
            if (card.transform.GetChild(0).tag == "black")
            {
                card.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/cards/weight/" + sArray[1]);
            }
            else
            {
                card.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/cards/weight/" + sArray[1] + "_w");
            }
        }
    }

    /// <summary>
    /// 调整自己被公布的牌【没桌牌时】
    /// </summary>
    void SetMyCardToOpen(GameObject card)
    {
        // 被公布的牌调整颜色
        card.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 1);
        card.transform.eulerAngles = new Vector3(47.7f, 21.1f, 1.8f);//旋转
    }

    /// <summary>
    /// 公布卡牌(猜错)
    /// </summary>
    /// <param name="whoIndex">卡牌归属者</param>
    public void KitheCard_F(int whoIndex, int delateIndex, int lineCardSeatIndex)
    {
        GameAudio.instance.PlayAudioSourceUI("move1");

        #region 排序（刚摸的牌）
        //暂时不做继续猜牌设定（每轮每人只猜一次牌）


        //获取对应玩家的候选牌
        Transform playerTrans = _Players[GetIndexInPlayerPos(whoIndex)];
        //Debug.Log("--------GetIndexInPlayerPos(whoIndex)-----" + GetIndexInPlayerPos(whoIndex));///-----------------------报错

        Transform playerCardLabry = playerTrans.Find("cardLabry");

        //删除选项牌
        DestroyOption(delateIndex);


        //禁用候牌区
        Image image = playerTrans.Find("NewAdd").GetComponent<Image>();
        image.enabled = false;

        GameObject cardPrefbs = (GameObject)Resources.Load("Prefabs/game/Card");//卡牌预制件加载

        GameObject curCard = Instantiate(cardPrefbs, playerCardLabry);
        DestroyImmediate(curCard.GetComponent<Toggle>());//删除Toggle组件

        curCard.name = waitCard.CardName;
        curCard.transform.GetChild(0).name = ((int)waitCard.CardWeight).ToString() + "+" + waitCard.CardWeight.ToString();
        curCard.transform.GetChild(1).tag = "open";
        curCard.transform.GetChild(1).name = waitCard.CardId.ToString();//以id做名字



        if (waitCard.CardColor == CardColors.BLACK)
        {
            curCard.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/cards/weight/" + waitCard.CardWeight.ToString());
        }
        else
        {
            curCard.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/cards/weight/" + waitCard.CardWeight.ToString() + "_w");
        }

        InsertCard(lineCardSeatIndex, playerCardLabry, curCard);


        if (whoIndex == myIndex)//如果被选择方是自己
        {
            curCard.tag = "myCard";
            //如果牌时自己的就调整牌的旋转和颜色 作为公示
            SetMyCardToOpen(curCard);
        }

        waitCard = null;//置空
        #endregion
    }

    /// <summary>
    /// 公布自选的牌【猜错且没桌牌时】
    /// </summary>
    public void OpenSelfSelect(SingleDraw singleDraw)
    {
        GameAudio.instance.PlayAudioSourceUI("move1");

        Transform cardLabryTrans = _Players[GetIndexInPlayerPos(FindInRoomMembers(singleDraw.myId))].Find("cardLabry");
        Transform card = cardLabryTrans.GetChild(singleDraw.cardIndex);
        //删除选项牌
        DestroyOption(selectOptionsPos.GetChild(0).Find(card.name).GetSiblingIndex());
        //删除Toggle组件
        DestroyImmediate(card.GetComponent<Toggle>());
        card.transform.GetChild(1).tag = "open";

        if (card.GetChild(0).tag == CardColors.BLACK.ToString())
        {
            card.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/cards/weight/" + card.GetChild(0).name.Split('+')[1]);
        }
        else
        {
            card.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/cards/weight/" + card.GetChild(0).name.Split('+')[1] + "_w");
        }

        if (singleDraw.myId == NetStart.myInfo.id)//如果被选择方是自己
        {
            //如果牌时自己的就调整牌的旋转和颜色 作为公示
            SetMyCardToOpen(card.gameObject);
        }
    }

    /// <summary>
    /// 插入刚摸得牌到手牌区
    /// </summary>
    /// <param name="lineCardSeatIndex"></param>
    /// <param name="playerCardLabry"></param>
    /// <param name="curCard"></param>
    void InsertCard(int lineCardSeatIndex, Transform playerCardLabry, GameObject curCard)
    {
        //移动位置
        int index;
        if (lineCardSeatIndex > -1)
        {
            index = lineCardSeatIndex;
        }
        else
        {
            index = Sort(playerCardLabry);
        }
        if (index >= 0)
        {
            curCard.transform.SetSiblingIndex(index);
        }
        Debug.Log("  ----InsertCard---- " + index);
    }

    /// <summary>
    /// 实例化可安插的位置【摸到候选牌】黑白万能牌顺序也不能颠倒？？？
    /// </summary>
    void DisplayEnableSeat(int guessCardIndex, int playerCardIndex, int playerId, DrawCardCommand status)
    {
        insertSeats = new List<GameObject>();
        Transform playerCardLabry = _Players[0].Find("cardLabry");
        //当前玩家对象节点上添加的特殊组件，只控制实例化出来的可选的位子
        int currCount = playerCardLabry.childCount;
        Debug.Log("  ----currCount---- " + currCount);
        GameObject cardPrefb = (GameObject)Resources.Load("Prefabs/game/Card");//卡牌预制件加载

        for (int i = 0; i <= currCount; i++)//两端都有
        {
            //实例化
            GameObject curCard = Instantiate(cardPrefb, playerCardLabry);
            //插入【序号】
            int index = i;//不能提出去
            curCard.transform.SetSiblingIndex(2 * i);
            //添加button
            DestroyImmediate(curCard.GetComponent<Toggle>());
            curCard.AddComponent<Button>().onClick.AddListener((UnityEngine.Events.UnityAction)(() => {
                GameAudio.instance.PlayAudioSourceUI((string)"draw");
                JudgeGuess(guessCardIndex, playerCardIndex, playerId, status, index);
                //DestroyInsertSeat();
            }));
            //用列表存起来
            insertSeats.Add(curCard);
        }
        Debug.Log("  ----DisplayEnableSeat---- ");
    }

    /// <summary>
    /// 删除可插入的位置
    /// </summary>
    void DestroyInsertSeat()
    {
        if (insertSeats != null && insertSeats.Count > 0)
        {
            for (int i = 0; i < insertSeats.Count; i++)
            {
                Destroy(insertSeats[i]);
            }
        }
    }

    /// <summary>
    /// 查找手牌的的万能牌
    /// </summary>
    void FindHandLineCard()
    {
        //查找万能牌
        Transform playerCardLabry = _Players[0].Find("cardLabry");
        Debug.Log("  ----FindLineCardInHand---- insertIndex:");
        int currCount = playerCardLabry.childCount;
        lineCards = new List<GameObject>();
        lineCardMoveInfos = new List<LineCardMoveInfo>();
        Transform card;

        for (int i = 0; i < currCount; i++)
        {
            card = playerCardLabry.GetChild(i);
            if (card.GetChild(0).name.Split('+')[1] == CardWeight.Line.ToString())
            {
                lineCards.Add(card.gameObject);
                lineCardMoveInfos.Add(new LineCardMoveInfo { originalIndex = i, moveToIndex = -1 });
            }
        }

        if (lineCardMoveInfos.Count == 0)
        {
            return;
        }

        insertSeats = new List<GameObject>();
        MoveHandLineCard(playerCardLabry, currCount, lineCardMoveInfos, insertSeats);
    }

    /// <summary>
    /// 实例化万能牌可移动的位置【首摸时手牌区的万能牌】
    /// </summary>
    /// <param name="playerCardLabry"></param>
    /// <param name="currCount"></param>
    /// <param name="lineCardMoveInfos"></param>
    /// <param name="insertSeats"></param>
    void MoveHandLineCard(Transform playerCardLabry, int currCount, List<LineCardMoveInfo> lineCardMoveInfos, List<GameObject> insertSeats)
    {
        GameObject cardPrefb = (GameObject)Resources.Load("Prefabs/game/Card");//卡牌预制件加载
        seletedCard = lineCards[0].transform.GetChild(0).GetComponent<Image>();
        seletedCard.enabled = true;

        if (insertSeats.Count > 0)
        {
            return;
        }
        for (int i = 0; i <= currCount; i++)//两端都有
        {
            //实例化
            GameObject curCard = Instantiate(cardPrefb, playerCardLabry);
            //插入【序号】
            int index = i;//不能提出去
            curCard.transform.SetSiblingIndex(2 * i);
            //添加button
            DestroyImmediate(curCard.GetComponent<Toggle>());

            curCard.AddComponent<Button>().onClick.AddListener((UnityEngine.Events.UnityAction)(() => {
                GameAudio.instance.PlayAudioSourceUI((string)"draw");
                curCard.GetComponent<Image>().sprite = lineCards[0].GetComponent<Image>().sprite;
                curCard.GetComponent<Image>().color += new Color(0, 0, 0, -0.5f);
                seletedCard.enabled = false;
                lineCards.Remove(lineCards[0]);
                lineCardMoveInfos[num].moveToIndex = index;
                if (lineCards.Count > 0)
                {
                    num++;
                    MoveHandLineCard(playerCardLabry, currCount, lineCardMoveInfos, insertSeats);
                    curCard.GetComponent<Button>().onClick.RemoveAllListeners();
                }
                else
                {
                    //发送
                    DestroyInsertSeat();
                }

            }));
            //用列表存起来
            insertSeats.Add(curCard);
        }
    }

    /// <summary>
    /// 执行移动万能牌操作
    /// </summary>
    /// <param name="whoId"></param>
    /// <param name="lineCardMoveInfos"></param>
    public void DoMoveLineCard(int whoId, List<LineCardMoveInfo> lineCardMoveInfos)
    {
        Transform cardLabry = _Players[GetIndexInPlayerPos(FindInRoomMembers(whoId))].Find("cardLabry");

        for (int i = 0; i < lineCardMoveInfos.Count; i++)
        {
            cardLabry.GetChild(lineCardMoveInfos[i].originalIndex).SetSiblingIndex(lineCardMoveInfos[i].moveToIndex);
        }
        Debug.Log("-----------DoMoveLineCard-----------");
    }
    #endregion

}