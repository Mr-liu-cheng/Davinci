using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Newtonsoft.Json;

/****************************************************
	项 目 名 称：
    作		者：刘光和
    功		能：
	修 改 时 间：
*****************************************************/

public class MyClass
{
    public int ID;
    public string Name;
    public BaseCard Card;
    bool Sex;

    public MyClass() { }

    public MyClass(int iD, string name, BaseCard card, bool sex)
    {
        ID = iD;
        Name = name;
        Card = card;
        Sex = sex;
    }
}

public class KY : MonoBehaviour 
{
    Image seletedCard;
    GameObject GameObject;
    private GameObject guess_TBtns;

    void Start () 
	{
        guess_TBtns = transform.Find("66").gameObject;
        Debug.Log("+++++++++++++++++++"+guess_TBtns.transform.Find("1").GetSiblingIndex());

        Image cardSelected = GetComponent<Image>();
        seletedCard = cardSelected;
        //cardSelected.color = new Color(0.5f, 0.5f, 0.5f, 1);
        Debug.Log(transform.childCount);
        //transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        Debug.Log(transform.GetChild(0).GetChild(0).name);
        transform.Find("66/1").GetComponent<Image>().enabled = true;

        MyClass myClass = new MyClass {
            ID = 1,
            Name="沙克",
        };

        MyClass myClass1= DataDo.Json2Object<MyClass>(DataDo.Object2Json(myClass));
        Debug.Log(myClass1.ID+ myClass1.Name+myClass.Card);

        Test();
        Toggletest();
    }

    void Toggletest()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Toggle>().onValueChanged.AddListener((bool isOn)=> { Debug.Log("++++++++++++++++++"); });
            transform.GetChild(i).GetComponent<Toggle>().onValueChanged.RemoveAllListeners();

            transform.GetChild(i).GetComponent<Toggle>().onValueChanged.AddListener((bool isOn) => { Debug.Log("---------------"); });
        }
    }

    void Test()
    {
        Stack tempRankList = new Stack();
        tempRankList.Push(1);
        tempRankList.Push(2);
        RoomInfo roomInfo = new RoomInfo();
        roomInfo.roomID = 1545435.ToString();
        roomInfo.member = new List<PersonalInfo>();
        PersonalInfo personalInfo = new PersonalInfo();
        personalInfo.id = 1;
        roomInfo.member.Add(personalInfo);
        PersonalInfo personalInfo1 = new PersonalInfo();
        personalInfo.id = 2;

        roomInfo.member.Add(personalInfo1);

        int id;
        for (int i = 0; i < 2; i++)//tempRankList.Count
        {
            id = (int)tempRankList.Pop();
            Debug.Log("-------id：" + id);
            PersonalInfo personal = roomInfo.member.Find(it =>
            {
                if (it.id == id)//tempRankList空的
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }
    }
	
	void Update () 
	{
        if (Input.GetKeyDown(KeyCode.A))
        {
            seletedCard.enabled = true;//复选框激活
            GameObject gameObject = Resources.Load<GameObject>("Test");
            GameObject = Instantiate(gameObject, transform);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            seletedCard.enabled = false;//复选框激活
            if (GameObject!=null)
            {
                DestroyImmediate(GameObject);

            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            DestroyImmediate(transform.GetChild(0).GetChild(0).gameObject);
        }
    }
}
