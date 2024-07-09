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
        //## 유저 정보 갱신
        if (UserInfo_Txt != null)
            UserInfo_Txt.text = "별명("+GlobalValue.g_NickName+"):보유골드("+GlobalValue.g_UserGold+")";

        //## 뒤로 가기
        if (Back_Btn != null)
            Back_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Lobby");
            });
    }

    // Update is called once per frame
    void Update()
    {
        // 사용자 정보 갱신
        if (UserInfo_Txt != null)
            UserInfo_Txt.text = "별명("+GlobalValue.g_NickName+"):보유골드("+GlobalValue.g_UserGold+")";
    }
}



