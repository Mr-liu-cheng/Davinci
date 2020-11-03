using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAdjust : MonoBehaviour
{
    /// <summary>
    /// 文本内容的RectTransform组件
    /// </summary>
    public RectTransform textRectTransform;
    /// <summary>
    /// 文本内容的ContentSizeFitter组件
    /// </summary>
    public ContentSizeFitter TextSizeFitter;
    /// <summary>
    /// 气泡的ContentSizeFitter组件
    /// </summary>
    public ContentSizeFitter bubbleSizeFitter;
    /// <summary>
    /// 消息的RectTransform组件
    /// </summary>
    RectTransform msgRectTransform;
    const int MAX_WIDTH=651;

    void Start()
    {
        msgRectTransform = GetComponent<RectTransform>();
        StartCoroutine(Judge());
    }

    IEnumerator Judge()
    {
        yield return new WaitForEndOfFrame();
        yield return null;

        //单行 默认是多行
        if (textRectTransform.sizeDelta.x > MAX_WIDTH)//超宽文本限制宽度【宽度超出】
        {
            //文本大小扩展
            TextSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            TextSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            textRectTransform.sizeDelta = new Vector2(MAX_WIDTH, textRectTransform.sizeDelta.y);
            //文本气泡大小扩展
            yield return null;
            bubbleSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            bubbleSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            //消息项所占长度扩展
            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate(msgRectTransform);
        }
        else//根据当前的宽度适配长度【宽度没超出，长度超出一行】
        {
            TextSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            TextSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            yield return null;
            bubbleSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            bubbleSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;


            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate(msgRectTransform);
        }
        //消息滚动窗内容大小及位置刷新
        yield return null;//等一帧
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        transform.parent.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;//至于滚动视图底部
    }
}

