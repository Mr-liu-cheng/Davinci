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

/// <summary>
/// Calculagraph 计时器委托声明
/// </summary>
public delegate void TimerMethodDelegate();//多种参数/返回类型 用泛型委托或者switch方法

/// <summary>
/// 计时器
/// </summary>
public class Calculagraph : MonoBehaviour
{
     Text timeText;
     Image fillImage;
    /// <summary>
    /// 时间
    /// </summary>
     int time;
    /// <summary>
    /// 裁剪前的时间点
    /// </summary>
    int lastTime;

    /// <summary>
    /// 声明委托方法的实例  在创建计时器时创建对应的实例
    /// </summary>
    public TimerMethodDelegate methodDelegate;

    /// <summary>
    /// 创建定时器
    /// </summary>
    /// <param name="timeSize">时间</param>
    /// <param name="memberIndex">玩家在成员列表中的下标</param>
    /// <param name="delegateFuc">委托方法（计时器到点执行的函数）</param>
    public static GameObject CreatTimer(int timeSize, TimerMethodDelegate delegateFuc, Transform transform, Vector3 pos)
    {
        //问题有时候会同时出现多个计时器   ---根据保存返回的对象是否为空来控制局部单例计时器
        GameObject timerPrefbs = (GameObject)Resources.Load("Prefabs/game/Timer");//卡牌（背面）预制件加载
        GameObject timer = Instantiate(timerPrefbs, transform);
        timer.transform.localPosition = pos;
        Calculagraph calculagraph= timer.AddComponent<Calculagraph>();
        if (delegateFuc != null)
        {
            calculagraph.methodDelegate += delegateFuc;//new TimerMethodDelegate(delegateFuc);//创建委托实例
        }
        calculagraph.time = timeSize;
        //Debug.Log("创建定时器");
        return timer;
    }

    /// <summary>
    /// 延期【倒计时延时+时间】crop
    /// </summary>
    public void Extension(int value)
    {
        Debug.Log("--------Extension------------");
        time += value;
        timeText.text = value.ToString();
    }

    /// <summary>
    /// 裁剪时间【指定时长】
    /// </summary>
    public void Crop(int value)
    {
        lastTime = time;
        fillImage.fillAmount = 0;
        time = value;
        timeText.text = time.ToString();
        Debug.Log(lastTime + "--------Crop------------:" + value);
    }

    /// <summary>
    /// 粘贴时间【计时器自身在Crop时保存的值】
    /// </summary>
    public void Paste(int value)
    {
        fillImage.fillAmount = 0;
        time =time+ value+lastTime;
        timeText.text = time.ToString();
        Debug.Log("--------Paste---------粘贴---" + time + value + lastTime);
    }

    void Start()
    {
        timeText = GetComponentInChildren<Text>();
        fillImage = transform.GetChild(0).GetComponent<Image>();

        timeText.text = time.ToString();
        StartCoroutine(CalculTime());
        StartCoroutine(Transition());
    }

    /// <summary>
    /// 计时器
    /// </summary>
    IEnumerator CalculTime()
    {
        GameAudio.instance.PlayAudioSourceLoop("second", true);

        while (time > 0)
        {
            if (time==3)
            {
                GameAudio.instance.PlayAudioSourceUI("3second");
                
                GameAudio.instance.PlayAudioSourceLoop("second", false);
            }
            
            //暂停一秒
            yield return new WaitForSeconds(1);
            time--;
            timeText.text = time.ToString();
            //Debug.Log("倒计时：" + time);
        }
        //计时结束 执行对应方法
        if (methodDelegate!=null)//先判断委托的函数是否为空
        {
            methodDelegate();// 使用委托对象调用方法
        }

        DestroyImmediate(this.gameObject);//自毁
        //Debug.Log("--------自毁------------");
    }

    /// <summary>
    /// 过渡动画
    /// </summary>
    IEnumerator Transition()
    {
        while (time > 0)
        {
            yield return 0;
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, 1, 1.0f / time*Time.deltaTime );
        }
    }
}
