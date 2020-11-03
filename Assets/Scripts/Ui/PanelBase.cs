using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/****************************************************
	项 目 名 称：面板
    作		者：刘光和
    功		能：PanelBase TPanel<T> PanelManege 面板基类和管理类（开，关，获取）
    用		法：每个面板都要自带canvas组件就不用设置位置和父节点，预制件位置在panel,名称要和类名对应，
        	   类要继承TPanel<T> T为自己类名 通过类名.方法获取面板的控制权。
	修 改 时 间：
*****************************************************/
/// <summary>
/// 面板祖类
/// </summary>
public class PanelBase : MonoBehaviour { }

/// <summary>
/// TPanel<T>: PanelBase，具体面板T又继承TPanel<T>，所以要约束具体面板T : PanelBase
/// TPanel<T>是过渡类，面板基类，其上还有父类 该方法目的是让每个面板都拥有自己的控制权
/// </summary>
/// <typeparam 具体面板="T"></typeparam>
public class TPanel<T>: PanelBase where T : PanelBase  
{
    /// <summary>
    /// T类型的面板
    /// </summary>
    public static T Open()
    {
        return PanelManege.OpenPanel(typeof(T)) as T;
    }
    public static T Get()
    {
        return PanelManege.GetPanel(typeof(T)) as T;
    }
    public static void Close()
    {
        PanelManege.ClosePanel(typeof(T));
    }
}

public class PanelManege
{
    /// <summary>
    /// 所有开启的面板集合
    /// </summary>
    static Dictionary<Type, PanelBase> panels = new Dictionary<Type, PanelBase>();

    public static PanelBase OpenPanel(Type type)
    {
        GameObject uiPrefb = Resources.Load<GameObject>(string.Format("Prefabs/panel/{0}", type.Name));//加载UI预制件
        if (!uiPrefb)
        {
            Debug.Log("没有找到面板预制件");
            return null;
        }
        PanelBase panel = GetPanel(type);
        if (!panel)
        {
            panel = GameObject.Instantiate(uiPrefb).GetComponent<PanelBase>();
            panels.Add(type, panel);//type使得每种面板都只能出现一次
        }
        //Debug.Log("打开的面板数："+ panels.Count);
        return panel;
    }

    public static PanelBase GetPanel(Type type) 
    {
        PanelBase panel;
        panels.TryGetValue(type,out panel);
        if (!panel)
        {
           // Debug.Log("没找到面板");
            return null;
        }
        return panel;
    }

    public static void ClosePanel(Type type)
    {
        PanelBase panel = GetPanel(type);
        if (panel)
        {
            GameObject.Destroy(panel.gameObject);
            panels.Remove(type);
        }
        //Debug.Log("打开的面板数：" + panels.Count);
    }
}


