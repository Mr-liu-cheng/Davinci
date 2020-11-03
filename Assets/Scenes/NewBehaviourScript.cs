using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public Button btn;
    public GameObject login;
    public GameObject net;
    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(sa);
        //LoginCommand login_Command = new LoginCommand(new LoginInfo { id = "1", passWord = "1" });
        //login_Command.Send();
    }

    // Update is called once per frame
    void sa()
    {
        login.GetComponent<Login>().enabled=true;
        net.GetComponent<NetStart>().enabled=true;
    }
}
