using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Store_Mgr : MonoBehaviour
{
    public Button m_Back_Btn = null;
    public Text m_UserInfo_Txt = null;


    // Start is called before the first frame update
    void Awake()
    {
        GlobalValue.LoadGameData();
    }

    private void Start()
    {
        if(m_Back_Btn != null)
           m_Back_Btn.onClick.AddListener(() =>
           {
               SceneManager.LoadScene("Lobby");
           });

        if(m_UserInfo_Txt != null)
        {
            m_UserInfo_Txt.text = "별명(" + GlobalValue.g_NickName + ") : 보유 골드( " + GlobalValue.g_UserGold+")";
        }
        
    }

}
