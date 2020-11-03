using System;
using System.Collections.Generic;
using UnityEngine;

class TypeDo
{
    public static Dictionary<int, string> Types = new Dictionary<int, string>();
    private static Queue<Command> CommandQueue = new Queue<Command>();

    /// <summary>
    /// 命令添加初始化
    /// </summary>
    public static void AddType()
    {
        Types.Add(1, "LoginCommand");
        Types.Add(2, "ChatCommand");
        Types.Add(3, "SelectFriendCommand");
        Types.Add(4, "HeartbeatCommand");
        Types.Add(5, "RegisteCommand");
        Types.Add(6, "RankCommand");
        Types.Add(7, "RoomCommand");
        Types.Add(8, "GameCommand");
    }

    /// <summary>
    /// 将接收到的命令加到队列，加锁，然后在主线程处理
    /// </summary>
    /// <param name="bts"></param>
    public static void DoType(Byte[] bts)
    {
        string strClass;
        Types.TryGetValue(BitConverter.ToInt32(bts,0), out strClass);
        //Debug.Log(BitConverter.ToInt32(bts, 0) + ": 命令 :" + strClass);
        Type t = Type.GetType(strClass);
        Command command = Activator.CreateInstance(t, true) as Command;
        command.Init(bts);

        lock (CommandQueue)
        {
            CommandQueue.Enqueue(command);//入队
        }
    }

    /// <summary>
    /// 处理所有命令
    /// </summary>
    public static void ProcessAllCommand()
    {
        while (true)
        {
            Command command = null;
            lock (CommandQueue)
            {
                if (CommandQueue.Count > 0)
                    command = CommandQueue.Dequeue();//出队
            }
            if (command != null)
                command.DoCommand();
            else
                break;
        }
    }
}
