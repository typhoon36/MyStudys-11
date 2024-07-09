using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Store_Mgr : MonoBehaviour
{
    [Header("--- Button ---")]
    public Button Back_Btn;

    [Header("--- UserInfo ---")]
    public Text UserInfo_Txt;

   


    // Start is called before the first frame update
    void Start()
    {
        //## ���� ���� ����
        if (UserInfo_Txt != null)
            UserInfo_Txt.text = "����("+GlobalValue.g_NickName+"):�������("+GlobalValue.g_UserGold+")";

        //## �ڷ� ����
        if (Back_Btn != null)
            Back_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Lobby");
            });
    }

    // Update is called once per frame
    void Update()
    {
        // ����� ���� ����
        if (UserInfo_Txt != null)
            UserInfo_Txt.text = "����("+GlobalValue.g_NickName+"):�������("+GlobalValue.g_UserGold+")";
    }
}



