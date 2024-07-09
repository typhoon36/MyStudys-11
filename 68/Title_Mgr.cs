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

        //## 게임종료
        if (m_Exit_Btn != null)
            m_Exit_Btn.onClick.AddListener(()=>
            {
#if UNITY_EDITOR
                //## 에디터에서 게임종료 동작
                Debug.Log("에디터에서는 플레이 모드를 종료합니다.");
                UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 빌드된 게임에서의 동작
        Application.Quit();
#endif
            });
        
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
