using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 骰子
/// </summary>
public class Dice : MonoBehaviour
{
    private UGUIAnimation _dice1;
    private UGUIAnimation _dice2;

    public int[] dices;

    /// <summary>
    /// 1号骰子
    /// </summary>
    public UGUIAnimation Dice1
    {
        get { return _dice1; }
        set { _dice1 = value; }
    }

    /// <summary>
    /// 2号骰子
    /// </summary>
    public UGUIAnimation Dice2
    {
        get { return _dice2; }
        set { _dice2 = value; }
    }

    private void Awake()
    {
        Dice1 = transform.Find("1").GetComponent<UGUIAnimation>();
        Dice2 = transform.Find("2").GetComponent<UGUIAnimation>();
    }

    private void Start()
    {
        StartCoroutine(InitMatch());
    }

    public void PlayerAllAnimation()
    {
        Dice1.Play();
        Dice2.PlayReverse();
    }

    public void TouziShow(bool value)
    {
        Dice1.gameObject.SetActive(value);
        Dice2.gameObject.SetActive(value);
    }

    /// <summary>
    /// 获取当前的庄家并播放骰子动画
    /// </summary>
    private void SetHitPoint()
    {
        //通过摇骰子设置拿牌位置  随机数由服务器控制
        int touziOne = dices[0];//Random.Range(1, 6);
        int touziTwo = dices[1];//Random.Range(1, 6);
        GameAudio.instance.PlayAudioSourceUI("dice");

        Dice1.LastImage = Resources.Load("UI/Touzi/touzi" + touziOne, typeof(Sprite)) as Sprite;
        Dice2.LastImage = Resources.Load("UI/Touzi/touzi" + touziTwo, typeof(Sprite)) as Sprite;
        //GameAudio.instance.PlayaudioSourceRole("touzi");
        PlayerAllAnimation();
    }

    IEnumerator InitMatch()
    {
        TouziShow(true);
        Debug.Log("+++++++++++++++++   播放   +++++++++++++++");
        SetHitPoint();//通过丢骰子设置摸牌的位置
        yield return new WaitForSeconds(GameCommand._DiceTime);
        TouziShow(false);
        Debug.Log("+++++++++++++++++   删除骰子   +++++++++++++++");
        DestroyImmediate(gameObject);
    }

}
