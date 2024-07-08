using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title_Mgr : MonoBehaviour
{
    [Header("--- Login ---")]
    public GameObject m_Login_Panel;
    public Button Login_Btn;
    public Button m_CreateOpen_Btn;
    public Button m_Exit_Btn;


    // Start is called before the first frame update
    void Start()
    {
        //## 게임데이터 로드
        GlobalValue.LoadGameData();

        //## 로그인 버튼
        if (Login_Btn != null)
            Login_Btn.onClick.AddListener(LoginBtn_Click);
        
    }

    void LoginBtn_Click()
    {
       SceneManager.LoadScene("Lobby");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
