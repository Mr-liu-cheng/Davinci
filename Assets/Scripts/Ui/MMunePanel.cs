using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/****************************************************
	项 目 名 称：
    作		者：刘光和
    功		能：
	修 改 时 间：
*****************************************************/

public class MMunePanel : TPanel<MMunePanel> 
{
    public Transform myInfo;
    public Button myInfo_B;
    public Button creatRoom_B;
    public Button setting_B;
    public Button store_B;

    public static bool isLoad;//俩次设置开1.查找成功2.从游戏界面重回该界面时

    GameObject rank_preb;//排行榜预制件
    public Transform rankPanel;//排行榜面板

    GameObject friendPrefb;                  //好友项预制件
    public Transform ShowFriendPanel; //得到好友显示面板
    GameObject msgCount;//, emailCount, taskCount ;//消息条数,邮件，任务
    GameObject msgCount_preb;//条数数字预制件 
    public Transform msgPos;//msgPos, emailPos, taskPos;
    public Transform button_parentPos;//msgPos, emailPos, taskPos的父级位置,控制显示层级，不在canvas下无法显示
    public Button msgCount_B;//消息条数显示区按钮（打开聊天界面）
    Sprite myicon;

    List<GameObject> friends = new List<GameObject>();//好友列表

    void Start()//是函数，启动时我们只执行一次，里面的循环体不受影响
    {
        //Debug.Log("到主界面:");
        friendPrefb = (GameObject)Resources.Load("Prefabs/MMenu/FriendProb");//动态加载好友预制件
        msgCount_preb = (GameObject)Resources.Load("Prefabs/MMenu/msgCount");//msgCount_preb预制件
        rank_preb = (GameObject)Resources.Load("Prefabs/MMenu/Rank_item");//排行榜预制件加载
        RankCommand.Rank_Send();//获取排行榜命令
        DisplayMyInfo();

        myInfo_B.onClick.AddListener(() => {
            GameAudio.instance.PlayAudioSourceUI("click_btn");
            InfoPanel infoPanel = InfoPanel.Open();
            infoPanel.Initialize(NetStart.myInfo);
        });//转到个人的详细信息界面

        creatRoom_B.onClick.AddListener(() => {
            GameAudio.instance.PlayAudioSourceUI("click_btn");
            //执行查找房间命令
            RoomCommand.SelectRooms();
            MMunePanel.Close();
            RoomsHallPanel.Open();
        });

        msgCount_B.onClick.AddListener(() => ToChatPanel(null));

        setting_B.onClick.AddListener(() => {
            GameAudio.instance.PlayAudioSourceUI("click_btn");
            SettingPanel.Open();
        });
        store_B.onClick.AddListener(() => {
            GameAudio.instance.PlayAudioSourceUI("click_btn");
            //Close();
        });
        SelectFriendCommand.SelectFriendList();
    }

    /// <summary>
    /// 显示自己的昵称和头像及等级
    /// </summary>
    void DisplayMyInfo()
    {
        object s;
        if (NetStart.myInfo==null) Debug.Log("没有人");
        if (Resources.Load("UI/role_photo/" + NetStart.myInfo.icon))
        {
            s = Resources.Load("UI/role_photo/" + NetStart.myInfo.icon, typeof(Sprite));
        } 
        else
        {
            s = Resources.Load("UI/role_photo/default", typeof(Sprite));
        }
        myInfo.Find("icon").GetComponent<Image>().sprite = s as Sprite;

        myInfo.Find("name").GetComponent<Text>().text = NetStart.myInfo.name;
        myInfo.Find("degree").GetComponent<Text>().text = NetStart.myInfo.degree.ToString();
        myicon = Resources.Load("UI/role_photo/" + NetStart.myInfo.icon, typeof(Sprite)) as Sprite;//存起来用于消息实例避免重新加载
    }

    /// <summary>
    /// 刷新好友列表在接收命令里调用  不要用public static 全局变量来隐式传参
    /// </summary>
    public void UpdateRankList(List<PersonalInfo> rankList)
    {
        if (RankCommand.rankList != null)
        {
            if (rankPanel.childCount > 0)
            {
                for (int i = 0; i < rankPanel.childCount; i++)
                {
                    Destroy(rankPanel.GetChild(i).gameObject);//先删除之前的好友
                }
            }
            //Debug.Log("清除完毕:");
            //Debug.Log("friendList:" + DisplayFriend.friendListInfo.Count);
            for (int i = 0; i < rankList.Count; i++)//PersonalInfo friend in DisplayFriend.friendListInfo
            {
                //ShowFriend(rankList[i]);//加载好友
                                              //Debug.Log(i + "次");
                Display(rankList[i], i + 1);
            }
            //Debug.Log("完成实例化好友:");
        }
    }

    /// <summary>
    /// 排行榜显示
    /// </summary>
    /// <param name="personalInfo"></param>
    /// <param name="num"></param>
    public void Display(PersonalInfo personalInfo, int num)// 实例化显示
    {
        GameObject curItem = Instantiate(rank_preb, rankPanel);
        curItem.transform.Find("num").GetComponent<Text>().text = num.ToString();//num
        curItem.name = personalInfo.id.ToString();//id
        Image icon = curItem.GetComponent<Image>();
        if (Resources.Load("UI/role_photo/" + personalInfo.icon))
        {
            icon.sprite = Resources.Load("UI/role_photo/" + personalInfo.icon, typeof(Sprite)) as Sprite;
        }
    }

    /// <summary>
    /// 刷新好友列表在接收命令里调用  不要用public static 全局变量来隐式传参
    /// </summary>
    public void UpdateFriendList(List<PersonalInfo> friendListInfo)
    {
        if (ShowFriendPanel.childCount > 0)
        {
            for (int i = 0; i < ShowFriendPanel.childCount; i++)
            {
                Destroy(ShowFriendPanel.GetChild(i).gameObject);//先删除之前的好友
            }
        }
        //Debug.Log("清除完毕:");
        //Debug.Log("friendList:" + DisplayFriend.friendListInfo.Count);
        for (int i = 0; i < friendListInfo.Count; i++)//PersonalInfo friend in DisplayFriend.friendListInfo
        {
            ShowFriend(friendListInfo[i]);//加载好友
            //Debug.Log(i + "次");
        }
        //Debug.Log("完成实例化好友:");
    }

    void Update()
    {
        UpdateMsgCount();
    }

    void ShowFriend(PersonalInfo friend)// 实例化显示好友  
    {
        GameObject nowfriend = Instantiate(friendPrefb, ShowFriendPanel);
        nowfriend.name = friend.id.ToString();
        nowfriend.GetComponentInChildren<Text>().text = friend.markName;
        Image icon = nowfriend.transform.Find("Icon").gameObject.GetComponent<Image>();
        Image status = nowfriend.transform.Find("Status").gameObject.GetComponent<Image>();
        Button toChatPanel_B = nowfriend.transform.Find("sendMsgBtn").gameObject.GetComponent<Button>();//转到聊天界面
        Button toInfoPanel_B = nowfriend.transform.Find("Icon").gameObject.GetComponent<Button>();
        toChatPanel_B.onClick.AddListener(() => ToChatPanel(friend));
        toInfoPanel_B.onClick.AddListener(()=>ToInfoPanel(friend));//转到个人信息界面

        Sprite sprite = Resources.Load<Sprite>("UI/role_photo/" + friend.icon);
        if (sprite)
        {
            icon.sprite = sprite;
        }
        else
        {
            icon.sprite = Resources.Load<Sprite>("UI/role_photo/default");
            Debug.Log("加载不到头资源……,启用默认的头像");
        }
        SetStatus(status, friend);
        friends.Add(nowfriend);
    }

    void ToChatPanel(PersonalInfo friend)
    {
        GameAudio.instance.PlayAudioSourceUI("click_btn");
        ChatPanel.Open();
        if (ChatPanel.myicon==null)
        {
            ChatPanel.myicon = myicon;
        }
        if (friend != null)//玩家消息按钮
        {
            if (!NetStart.chatmanlist.Contains(friend.id))
            {
                NetStart.chatmanlist.Add(friend.id);
            }
            ChatPanel.selectedID = friend.id;
        }
        else//消息条数按钮
        {
            //有未读的取第一条为默认，否则仍然是之前
            if (MessageInfo.unReadMsg.Count > 0)
            {
                foreach (var item in MessageInfo.unReadMsg)
                {
                    ChatPanel.selectedID = item.Key;
                    break;
                }
            }
        }
    }

    void ToInfoPanel(PersonalInfo friend)
    {
        GameAudio.instance.PlayAudioSourceUI("click_btn");
        InfoPanel infoPanel = InfoPanel.Open();
        infoPanel.Initialize(friend);
    }

    /// <summary>
    /// 设置状态
    /// </summary>
    /// <param name="status"></param>
    /// <param name="friend"></param>
    public static void SetStatus(Image status, PersonalInfo friend)
    {
        Sprite statusIcon = Resources.Load<Sprite>("UI/MMenu/friend/status"); //Resources.Load("UI/status/s" + friend.status);+ friend.status
        if (statusIcon)
        {
            //Debug.Log("加载到" + Resources.Load("Icon/status/s" + friend.status).name);
            status.sprite = statusIcon;//as Sprite;
            switch (friend.status)
            {
                case 0:
                    status.color = Color.gray;//离线
                    break;
                case 1:
                    status.color = Color.green;//在线
                    break;
                case 2:
                    status.color = Color.yellow;//房间中未开局
                    break;
                case 3:
                    status.color = Color.red;//游戏中
                    break;
            }
        }
        else Debug.Log("加载不到状态资源……");
    }

    /// <summary>
    /// 更新消息条数
    /// </summary>
    void UpdateMsgCount()//更新消息条数
    {
        if (MessageInfo.unReadMsg.Count > 0)
        {
            if (!msgCount) msgCount = Instantiate(msgCount_preb, msgPos);
            Text msgCount_Txt = msgCount.GetComponentInChildren<Text>();
            int unreadmsgCount=0;
            foreach (var item in MessageInfo.unReadMsg)
            {
                unreadmsgCount += item.Value.Count;
            }
            msgCount_Txt.text = unreadmsgCount.ToString();
        }
        if (MessageInfo.unReadMsg.Count == 0) Destroy(msgCount);
    }


}
