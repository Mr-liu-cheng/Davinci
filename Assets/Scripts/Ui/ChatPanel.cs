using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/****************************************************
	项 目 名 称：
    作		者：刘光和
    功		能：
	修 改 时 间：
*****************************************************/

public class ChatPanel : TPanel<ChatPanel>
{
    public InputField write;           //发送内容 
    public Button sendMassage;         //发送消息按钮

    public Transform messagePanel;//消息面板 
    public ScrollRect scrollRect;//滚动窗口组件
    GameObject chatfriendPrefb;                  //聊好友项预制件
    public GameObject ShowChatManPanel;         //得到聊天好友显示面板
    GameObject msg_you_Prefb, msg_me_Prefb;      //msg预制件
    public Button exit_B;//聊天界面关闭按钮
    /// <summary>
    /// 当前选中好友的id  用于跟新当前聊天界面的消息记录【小于0时则只打开面板不实例化消息和联系人】
    /// </summary>
    public static int selectedID=-1;
    public static Sprite myicon;

    public void Start()//是函数，启动时我们只执行一次，里面的循环体不受影响
    {
        MMunePanel.Close();
        chatfriendPrefb = (GameObject)Resources.Load("Prefabs/chat/chatManPreb");//动态加载聊天好友预制件
        msg_you_Prefb = (GameObject)Resources.Load("Prefabs/chat/msg_you");//msg_you预制件
        msg_me_Prefb = (GameObject)Resources.Load("Prefabs/chat/msg_me");//msg_me预制件

        exit_B.onClick.AddListener(() =>
        {
            GameAudio.instance.PlayAudioSourceUI("close_btn");
            MMunePanel.Open();
            Close();
        });
        //DisplayChatManList();
        UpdateMsg();
    }

    /// <summary>
    /// 联系人的实例化
    /// </summary>
    //void DisplayChatManList()
    //{
    //    for (int i = 0; i < NetStart.chatmanlist.Count; i++)
    //    {
    //        //为好友才能聊天
    //        PersonalInfo friend = SelectFriendCommand.friendListInfo.Find(listItem =>
    //        {
    //            if (listItem.id == NetStart.chatmanlist[i])
    //            {
    //                return true;
    //            }
    //            else return false;
    //        });
    //        if (friend != null) DisplayChatMan(friend);
    //    }
    //}

    /// <summary>
    /// 实例联系人(属性赋值)
    /// 点击好友聊天出来的  关闭时会删除所有，所以每次打开要重新遍历联系人和聊天记录
    /// </summary>
    /// <param name="friend">朋友信息</param>
    /// <param name="isSingle">好友连天按钮 / 消息按钮</param>
    void DisplayChatMan(PersonalInfo friend)
    {
        GameObject nowchatfriend = Instantiate(chatfriendPrefb, ShowChatManPanel.transform);
        nowchatfriend.name = friend.id.ToString();
        nowchatfriend.GetComponentInChildren<Text>().text = friend.name;
        Button exit = nowchatfriend.GetComponentInChildren<Button>();
        exit.onClick.AddListener(() =>
        {
            GameAudio.instance.PlayAudioSourceUI("close_btn");
            Destroy(nowchatfriend);
            NetStart.chatmanlist.Remove(friend.id);
        });//删除当前联系人

        Image icon = nowchatfriend.transform.Find("Viewport/Content/Item/Icon").GetComponent<Image>();
        Toggle select_tog = nowchatfriend.GetComponentInChildren<Toggle>();
        select_tog.group = ShowChatManPanel.GetComponent<ToggleGroup>();//添加组
        Color color = select_tog.GetComponent<Image>().color;

        if (Resources.Load("UI/role_photo/" + friend.icon))
            icon.sprite = Resources.Load("UI/role_photo/" + friend.icon, typeof(Sprite)) as Sprite;
        else Debug.Log("加载不到头资源……");

        Image msgCount_G = nowchatfriend.transform.Find("Viewport/Content/Item/count").GetComponent<Image>();

        select_tog.onValueChanged.AddListener((bool isOn) => { 
            OnToggleClick(friend, select_tog, isOn, color, msgCount_G); });//进入对应消息框

        Debug.Log("Message.unReadMsg数目：" + MessageInfo.unReadMsg.Count);

        if (friend.id==selectedID)
        {
            DealWithDeafuatMan(friend, select_tog, color, msgCount_G);
        }
    }

    /// <summary>
    /// 处理默认选中的人
    /// </summary>
    void DealWithDeafuatMan(PersonalInfo personalInfo, Toggle select_tog, Color color, Image msgCount_G)
    {
        select_tog.GetComponent<Image>().color = new Color(color.r, color.g, color.b + 50, color.a - 0.5f);
        Sprite sprite = select_tog.gameObject.transform.Find("Icon").GetComponent<Image>().sprite;
        msgCount_G.gameObject.GetComponentInChildren<Text>().text = 0.ToString();
        msgCount_G.gameObject.SetActive(false);//禁用（关闭显示）
        sendMassage.onClick.AddListener(delegate () { 
            GameAudio.instance.PlayAudioSourceUI("sendMsg");
            Debug.Log("点击……");
            SendMsg(myicon, selectedID);
        });//发送消息
    }

    /// <summary>
    /// 跳转到选中好友的聊天面板
    /// </summary>
    void OnToggleClick(PersonalInfo personalInfo, Toggle select_tog, bool isOn, Color color, Image msgCount_T)
    {
        if (selectedID == personalInfo.id)
        {
            return;
        }
        if (isOn)
        {
            selectedID = personalInfo.id;
            if (MessageInfo.unReadMsg.ContainsKey(selectedID))
            {
                MessageInfo.unReadMsg.Remove(selectedID);
            }
            GameAudio.instance.PlayAudioSourceUI("select");
            Debug.Log("跳转到选中好友的聊天面板");
            DealWithDeafuatMan(personalInfo, select_tog, color, msgCount_T);
            DisplayMassage(true);
        }
        else
        {
            select_tog.GetComponent<Image>().color = color;
            //Debug.Log("加载不到……");
        }
    }//跳转到选中好友的聊天面板


    /// <summary>
    /// 发送消息的显示[自带显示]
    /// </summary>
    void SendMsg(Sprite myicon, int selted_id)//显示消息
    {
        if (write.text != "" && write.text.Trim().Length != 0)
        {
            Debug.Log("输入bu为空");
            GameObject chatMassage = Instantiate(msg_me_Prefb, messagePanel);
            if (!myicon)
            {
                Debug.Log("头像失去");
                return;
            }
            chatMassage.transform.Find("Image").GetComponent<Image>().sprite = myicon;
            chatMassage.transform.Find("rim/ChatText").GetComponent<Text>().text = write.text;

            MessageInfo msg = new MessageInfo//封装消息
            {
                sendId = NetStart.myInfo.id,
                toIds = selted_id,
                content = write.text
            };

            List<MessageInfo> msglist;
            MessageInfo.massageAll.TryGetValue(selted_id, out msglist);
            if (msglist == null)
            {
                msglist = new List<MessageInfo>();
                msglist.Add(msg);
                MessageInfo.massageAll.Add(selted_id, msglist);//添加消息
            }
            else msglist.Add(msg);
            ChatCommand.Send(msg);
        }
        else Debug.Log("输入为空");
        write.text = "";//清空输入框
    }

    /// <summary>
    /// 实例化消息记录
    /// id是消息存放索引（发送者id或者房间id）
    /// </summary>
    /// <param name="isDeleteAll">是否清楚消息内容重新实例化</param>
    void DisplayMassage(bool isDeleteAll)//自己发的显示，接收显示，群聊显示 自己的头像，发送者（n）的头像
    {
        int id = selectedID;
        Sprite icon = ShowChatManPanel.transform.Find(selectedID.ToString()+ "/Viewport/Content/Item/Icon").GetComponent<Image>().sprite;
        if (isDeleteAll)
        {
            for (int i = 0; i < messagePanel.childCount; i++)
            {
                Destroy(messagePanel.GetChild(i).gameObject);//清空
            }
        }
        List<MessageInfo> messages;
        MessageInfo.massageAll.TryGetValue(id, out messages);
        if (messages != null)
        {
            GameObject msg=null;
            for (int i = 0; i < messages.Count; i++)//实例生成消息
            {
                GameObject chatMassage;
                if (messages[i].sendId == id)//对方发过来
                {
                    chatMassage = Instantiate(msg_you_Prefb, messagePanel);
                    chatMassage.transform.Find("Image").GetComponent<Image>().sprite = icon;
                }
                else//自己发的
                {
                    chatMassage = Instantiate(msg_me_Prefb, messagePanel);
                    chatMassage.transform.Find("Image").GetComponent<Image>().sprite = myicon;
                }
                if (i== messages.Count-1)
                {
                    msg = chatMassage;
                }

                chatMassage.transform.Find("rim/ChatText").GetComponent<Text>().text = messages[i].content;
            }
        }
    }

    /// <summary>
    /// 跟新显示消息
    /// </summary>
    public void UpdateMsg()
    {
        if (NetStart.chatmanlist.Count > 0)
        {
            foreach (var item in NetStart.chatmanlist)
            {
                //为当前联系人实例化消息
                if (item == selectedID)
                {
                    Transform chatman = ShowChatManPanel.transform.Find(item.ToString());
                    if (chatman == null)
                    {
                        PersonalInfo friend = SelectFriendCommand.friendListInfo.Find(person =>
                        {
                            if (person.id == selectedID)
                            {
                                return true;
                            }
                            return false;
                        });

                        if (friend != null) DisplayChatMan(friend);
                    }
                    DisplayMassage(false);

                }
                else//非当前的联系人则实例化消息条数
                {
                    //对应id的联系人
                    Transform chatman = ShowChatManPanel.transform.Find(item.ToString());
                    if (chatman == null)//新增联系人
                    {
                        PersonalInfo personalInfo = SelectFriendCommand.friendListInfo.Find(info =>
                         {
                             if (info.id == item)
                             {
                                 return true;
                             }
                             else return false;
                         });
                        if (personalInfo != null) DisplayChatMan(personalInfo);//显示（实例化联系人）
                        chatman = ShowChatManPanel.transform.Find(item.ToString());
                    }

                    if (MessageInfo.unReadMsg.ContainsKey(item))
                    {
                        //显示未读条数
                        Image image = chatman.transform.Find("Viewport/Content/Item/count").GetComponent<Image>();
                        {
                            if (image.gameObject.activeSelf == false)
                            {
                                image.gameObject.SetActive(true);
                            }

                            //此处在实例联系人点击监听处理过
                            int msgcount = Convert.ToInt32(image.gameObject.GetComponentInChildren<Text>().text);
                            Debug.Log("msgcount:" + msgcount);
                            List<MessageInfo> unreadmessages;
                            if (MessageInfo.unReadMsg.TryGetValue(item, out unreadmessages))
                            {
                                msgcount += unreadmessages.Count;//找出各个id对应的消息条数
                            }
                            image.gameObject.GetComponentInChildren<Text>().text = msgcount.ToString();
                        }
                    }
                }
            }
            if (MessageInfo.unReadMsg.ContainsKey(selectedID))
            {
                MessageInfo.unReadMsg.Remove(selectedID);
            }
        }
    }
}