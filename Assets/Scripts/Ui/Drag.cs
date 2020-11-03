using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/****************************************************
	项 目 名 称：
    作		者：刘光和
    功		能：
	修 改 时 间：
*****************************************************/

public class Drag : MonoBehaviour
{
    ScrollRect rect;
    string thisName;

    void Start()
    {
        rect = transform.GetComponent<ScrollRect>();
        thisName = rect.gameObject.transform.name;
    }

    void Update () 
	{
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.currentSelectedGameObject)
            {
                string name = EventSystem.current.currentSelectedGameObject.transform.name;
                //Debug.Log("点击到UGUI的UI界面:"+ name+"，会返回true");
                if (name == "exit") { }
                else if(name =="Item")
                {
                    GameObject obj = EventSystem.current.currentSelectedGameObject;
                    if (obj.transform.parent.parent.parent.name != thisName) 
                    StartCoroutine(Backto());
                }
                else StartCoroutine(Backto());
            }
            else StartCoroutine(Backto());
        }
    }

    IEnumerator Backto()
    {
        float time = 0;//当前时间
        float duration = 0.5f;//执行时长
        float startPos = rect.horizontalNormalizedPosition;//当前位置
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);//最后一帧可能会大于1  移动的幅度
            rect.horizontalNormalizedPosition = Mathf.Lerp(startPos, 0, t);
            yield return null;
        }
    }
}
