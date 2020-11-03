using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Label : MonoBehaviour
{
    #region 预制件路径

    /// <summary>
    /// 标准版
    /// </summary>
    public const string Standard = "Prefabs/game/labelPrefb";

    #endregion


    /// <summary>
    /// 创建提示语
    /// </summary>
    public static void CreatLabel(string resourcesPath, Transform parentTrasn, Vector3 pos, string content, Color fontColor, Sprite sprite)
    {
        GameObject labelPrefb = Resources.Load<GameObject>(resourcesPath);
        Transform label = Instantiate(labelPrefb, parentTrasn).transform;
        label.name = "label";
        label.localPosition = pos;
        Text labelT = label.GetComponentInChildren<Text>();
        labelT.text = content;
        labelT.color = fontColor;
        if (sprite != null)
        {
            label.GetComponentInChildren<Image>().sprite = sprite;
        }
        label.gameObject.AddComponent<Label>();
    }


    private void Start()
    {
        StartCoroutine(Destory(gameObject, GetComponent<CanvasGroup>()));
    }

    IEnumerator Destory(GameObject gameObject, CanvasGroup labelCGp)
    {
        yield return new WaitForSeconds(2f);//1f
        while (true)
        {
            // if (Input.GetKeyDown(KeyCode.W)) break;
            labelCGp.alpha = Mathf.Lerp(labelCGp.alpha, 0, 4 * Time.deltaTime);

            if (labelCGp.alpha < 0.05f) break;
            yield return 0;
        }
        DestroyImmediate(gameObject);
        yield break;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DestroyImmediate(gameObject);
        }
    }
}
