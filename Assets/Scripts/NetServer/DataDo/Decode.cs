using System;
using System.Collections.Generic;
using System.Linq;

public class Decode
{
    /// <summary>
    /// 解析一级协议命令的内容
    /// </summary>
    /// <param name="bts"></param>
    /// <returns></returns>
    public static Byte[] DecodFirstContendBtye(Byte[] bts)
    {
        Byte[] contend = bts.Skip(8).ToArray();
        return contend;
    }

    /// <summary>
    /// 解析二级协议的内容
    /// </summary>
    /// <param name="bts"></param>
    /// <returns></returns>
    public static Byte[] DecodSecondContendBtye(Byte[] bts)
    {
        Byte[] contend = bts.Skip(12).ToArray();
        return contend;
    }
}
