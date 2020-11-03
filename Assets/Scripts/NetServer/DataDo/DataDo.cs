using System;
using Newtonsoft.Json;
using UnityEngine;


public class DataDo
{
    /// <summary>
    /// 当你已有重载构造函数时，要把默认构造函数写出来，因为调用的是默认构造函数（所有字段属性要是共有的）
    /// 可以解析数组、列表、字典、类， 不能解析队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataBts"></param>
    /// <returns></returns>
    public static T Json2Object<T>(byte[] dataBts)
    {
        string jsonStr = System.Text.Encoding.UTF8.GetString(dataBts);
        T jsonObj = JavaScriptConvert.DeserializeObject<T>(jsonStr);
        return jsonObj;
    }

    /// <summary>
    /// （所有字段属性要是共有的）
    ///  可以解析数组、列表、字典、类， 不能解析队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static byte[] Object2Json<T>(T obj)
    {
        string jsonStr = JavaScriptConvert.SerializeObject(obj);//从类的外部修改属性，所以属性必须是公有的
        return System.Text.Encoding.UTF8.GetBytes(jsonStr);
    }
}
