using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Incode
{
    /// <summary>
    /// 一级协议
    /// </summary>
    /// <param name="type">一级命令</param>
    /// <param name="contend">内容</param>
    /// <returns></returns>
    public static byte[] IncodeFirstCommand(int type, byte[] contend)
    {
        byte[] allBts;
        byte[] head = BitConverter.GetBytes(type);
        byte[] contdLength = BitConverter.GetBytes(contend.Length);//数据的长度
        byte[] conStr = contend;
        allBts = new byte[head.Length + contdLength.Length + conStr.Length];

        Buffer.BlockCopy(head, 0, allBts, 0, 4);
        Buffer.BlockCopy(contdLength, 0, allBts, 4, 4);
        Buffer.BlockCopy(conStr, 0, allBts, 8, conStr.Length);
        return allBts;
    }

    /// <summary>
    /// 二级协议
    /// </summary>
    /// <param name="type">一级命令</param>
    /// <param name="length"></param>
    /// <param name="secondCmd">二级命令</param>
    /// <param name="contend">内容</param>
    /// <returns></returns>
    public static byte[] IncodeSecondaryCommand(int type,int secondCmd, byte[] contend)//聊天命令的封装，应该再加一个length
    {
        byte[] allBts;
        byte[] head = BitConverter.GetBytes(type);
        byte[] lenth = BitConverter.GetBytes(contend.Length + sizeof(Int32));//数据长度
        byte[] cmd_type = BitConverter.GetBytes(secondCmd);
        byte[] conStr = contend;
        allBts = new byte[head.Length + lenth.Length + cmd_type.Length + conStr.Length];

        Buffer.BlockCopy(head, 0, allBts, 0, 4);
        Buffer.BlockCopy(lenth, 0, allBts, 4, 4);
        Buffer.BlockCopy(cmd_type, 0, allBts, 8, 4);
        Buffer.BlockCopy(conStr, 0, allBts,12, conStr.Length);
        return allBts;
    }

}
