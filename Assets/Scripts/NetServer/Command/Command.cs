using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Command
    {
    public byte[] bytes;

    public static bool command_Status = false;

    /// <summary>
    /// 命令初始化 赋值
    /// </summary>
    /// <param name="bts"></param>
    public virtual void Init(byte[] bts) { bytes = bts; }

    /// <summary>
    /// 执行命令
    /// </summary>
    public virtual void DoCommand() { }
}
