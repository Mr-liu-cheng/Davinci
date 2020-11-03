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

public class FriendPreb : MonoBehaviour                     
{
    public Button friendInfo_B, send_B;
    int id;
    string status, icon, nane;
	
	void Start () 
	{
        friendInfo_B.onClick.AddListener(()=> {
            GameAudio.instance.PlayAudioSourceUI("click_btn"); 
            ToPersonalInformationS(); });
        send_B.onClick.AddListener(() => { 
            GameAudio.instance.PlayAudioSourceUI("click_btn"); 
            ToChatRoom(); });

    }

    void Update () 
	{
		
	}

    void ToPersonalInformationS()
    {
        Debug.Log("asdsa154");
    }

    void ToChatRoom()
    {

    }
}
