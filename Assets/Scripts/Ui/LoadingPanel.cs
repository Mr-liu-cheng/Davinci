using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class LoadingPanel : TPanel<LoadingPanel>
{

    public Image progressUI;  //进度条
    public Text progressValue;       //进度值
    private AsyncOperation prog;    //异步加载对象


    void Start()
    {
        DontDestroyOnLoad(gameObject);
        GameObject.Find("Canvas").SetActive(false);//必须关闭不然跳转场景还会闪现出来
        StartCoroutine(LoadingScene());
    }




    private IEnumerator LoadingScene()
    {
        //yield return new WaitForSeconds(8.0f);
         
        AsyncOperation op = SceneManager.LoadSceneAsync("Scenes/MainScence");
        UnityAction<Scene, LoadSceneMode> onLoaded = null;
        onLoaded = (Scene scene, LoadSceneMode mode) =>
        {
            MMunePanel.Open();
            SceneManager.sceneLoaded -= onLoaded;
        };
        SceneManager.sceneLoaded += onLoaded;
        op.allowSceneActivation = false;

        int displayProgress = 0;     //当前展示进度
        int toProgress = 100;          //总进度
        displayProgress = (int)op.progress * 100;//化整
        while (displayProgress < toProgress )
        {
            displayProgress++;
            progressUI.fillAmount = op.progress;
            Debug.Log("++++++++++++  "+ displayProgress);
            yield return new WaitForEndOfFrame();
        }
        op.allowSceneActivation = true;
       
    }


    //public Image fill;//滑动条
    //public Text text;//文本
    //// 加载进度
    //float loadPro = 0;

    //// 用以接受异步加载的返回值
    //AsyncOperation AsyncOp = null;

    //public void LoadTo(string scenceName)
    //{
    //    fill.fillAmount = 0;
    //    AsyncOp = SceneManager.LoadSceneAsync("Scenes/" + scenceName, LoadSceneMode.Single);//异步加载场景名为"Demo Valley"的场景,LoadSceneMode.Single表示不保留现有场景
    //    AsyncOp.allowSceneActivation = false;//allowSceneActivation =true表示场景加载完成后自动跳转,经测,此值默认为true
    //}

    //void Update()
    //{
    //    Debug.Log("++++++++++++++  " + AsyncOp.progress);
    //    if (AsyncOp != null)//如果已经开始加载
    //    {
    //        loadPro = AsyncOp.progress; //获取加载进度,此处特别注意:加载场景的progress值最大为0.9!!!
    //    }
    //    if (loadPro >= 0.9f)//因为progress值最大为0.9,所以我们需要强制将其等于1
    //    {
    //        loadPro = 1;
    //    }
    //    fill.fillAmount = Mathf.Lerp(fill.fillAmount, loadPro, 1 * Time.deltaTime);//滑动块的value以插值的方式紧跟进度值
    //    if (fill.fillAmount > 0.99f)
    //    {
    //        fill.fillAmount = 1;
    //        AsyncOp.allowSceneActivation = true;
    //        MMunePanel.Open();
    //    }
    //    text.text = string.Format("{0:F0}%", fill.fillAmount * 100);//文本中以百分比的格式显示加载进度
    //}
}
