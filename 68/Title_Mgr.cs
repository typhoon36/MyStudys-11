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
        //## ���ӵ����� �ε�
        GlobalValue.LoadGameData();

        //## �α��� ��ư
        if (Login_Btn != null)
            Login_Btn.onClick.AddListener(LoginBtn_Click);

        //## ��������
        if (m_Exit_Btn != null)
            m_Exit_Btn.onClick.AddListener(()=>
            {
#if UNITY_EDITOR
                //## �����Ϳ��� �������� ����
                Debug.Log("�����Ϳ����� �÷��� ��带 �����մϴ�.");
                UnityEditor.EditorApplication.isPlaying = false;
#else
        // ���� ����� ���ӿ����� ����
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
