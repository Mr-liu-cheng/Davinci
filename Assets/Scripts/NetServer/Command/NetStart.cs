using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SceneManagement;

public class NetStart : MonoBehaviour
{
    public static List<int> chatmanlist;//实例化出来的聊天成员列表
    static Socket clientS;
    public static PersonalInfo myInfo;
    public static bool isLogin;//是否登陆成功
    float lastTime = -5;//上一次发送心跳包的时间
    const float heartBeatTime = 5;//心跳间隔时间（s）

    public const int BUF_SIZE = 1024*1024*10;
    public static byte[] readBuff = new byte[BUF_SIZE];
    public static int buffCount = 0;
    //沾包分包
    public static Int32 msgLength = 0;//数据长度
    public static byte[] lenBytes = new byte[sizeof(Int32)];//数据长度的数组
    public static byte[] whole;//完整消息
    static int msgTotalLength;//消息总长（消息的头长度+数据长度）
    const int HEADLENGTH = 8;//消息的头长度（命令+数据长度）

    /// <summary>
    /// 服务器连接
    /// </summary>
    public static void Connection(bool isReLink)
    {
        //判断当前界面
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            LoginCommand.LoadMMenuScene();
        }
        clientS = new Socket(AddressFamily.InterNetwork,
          SocketType.Stream, ProtocolType.Tcp);

        IPHostEntry host = Dns.GetHostEntry("3x24118452.oicp.vip");
        foreach (var va in host.AddressList)
        {
            Debug.Log(va.ToString());
        }
        IPAddress ips = IPAddress.Parse(host.AddressList[0].ToString());
        //IPAddress ips = IPAddress.Parse("127.0.0.1");
        IPEndPoint ipp = new IPEndPoint(ips, 57144);
        clientS.Connect(ipp);
        clientS.BeginReceive(readBuff, buffCount,BuffReamin(), SocketFlags.None,ReceiveMsg, clientS);
        Debug.Log("----重新连接服务器----");
        //if (isReLink)
        //{
        //    HeartbeatCommand.ReLink();
        //}

    }

    void Start()
    {
        chatmanlist = new List<int>();
        TypeDo.AddType();
        GameObject.DontDestroyOnLoad(this.gameObject);
        Connection(false);
    }

    /// <summary>
    /// 心跳包 查好友 查排行 发送
    /// </summary>
    void Refresh_Send()
    {
        if (isLogin)
        {
            if (Time.time - lastTime >= heartBeatTime)
            {
                HeartbeatCommand.Heartbeat_Send();//心跳包发送
                if (MMunePanel.Get())
                {
                    RankCommand.Rank_Send();//获取查询排行的命令
                    SelectFriendCommand.SelectFriendList();//查找所有好友
                }
                if (RoomsHallPanel.Get())
                {
                    RoomCommand.SelectRooms();//获取所有房间
                }
                //if (RoomPanel.Get())
                //{
                //    RoomCommand.RefreshThisRoomInfo(myInfo.roomNum);//刷新当前房间信息
                //}
                lastTime = Time.time;
            }
        }
    }

    /// <summary>
    /// 发送消息 检测是否正常
    /// </summary>
    /// <param name="bts"></param>
    public static void SendContend(byte[] bts)
    {
        try
        {
            clientS.Send(bts);//发送失败重连 &&  此事发送的消息服务器读取不到？？？
        }
        catch (Exception e)
        {
            Connection(true);
            // throw e;//不注释掉此事发送失败的消息就会显示在控制台，相反则会抛出错误不显示此条信息；（反正服务器都读不到这条）
        }
    }

    /// <summary>
    /// 计算消息可存长度
    /// </summary>
    /// <returns></returns>
    public static int BuffReamin()
    {
        return BUF_SIZE - buffCount;
    }

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="ar"></param>
    static void ReceiveMsg(IAsyncResult ar)
    {
        try
        {
            //接收消息时也要抓错从而决定是否重连
            Socket client = (Socket)ar.AsyncState;
            int count = clientS.EndReceive(ar);
   
            buffCount += count;
            if (count <= 0)//掉线重连
            {
                Connection(true);
            }
            DealthMsg();
            clientS.BeginReceive(readBuff, buffCount,BuffReamin(), SocketFlags.None,ReceiveMsg, clientS);
        }
        catch (Exception e)
        {
            Connection(true);
        }
    }

    /// <summary>
    /// 消息处理
    /// </summary>
    private static void DealthMsg()
    {
        //沾包分包处理
        if (buffCount < HEADLENGTH) return;

        //消息长度
        msgLength = BitConverter.ToInt32(readBuff, 4);//根据从数组第4位开始获取int（4字节长度）的对应int值获取长度值
        //Debug.Log("消息大小：" + msgLength);
        msgTotalLength = msgLength + HEADLENGTH;
        if (buffCount < msgTotalLength)
        {
            Console.WriteLine("buffCount < msgLength + 8");
            return;
        }
        
        //处理消息
        whole = new byte[msgTotalLength];//完整的消息长度
       // Debug.Log("数据大小：" +msgTotalLength);

        Array.Copy(readBuff, whole, msgTotalLength);
        TypeDo.DoType(whole);  //命令处理

        //清除已处理的消息
        int count = buffCount - msgTotalLength;
        Array.Copy(readBuff, msgTotalLength, readBuff, 0, count);
        buffCount = count;
        if (buffCount > 0)
        {
            DealthMsg();
        }
    }

    void Update()
    {
        CommandDo();
        Refresh_Send();

    }

    /// <summary>
    /// 命令执行(命令处理“主线程”)
    /// </summary>
    void CommandDo()
    {
        TypeDo.ProcessAllCommand();
    }
}
