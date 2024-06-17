using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby_Mgr : MonoBehaviour
{
    public Button Start_Btn = null;


    // Start is called before the first frame update
    void Start()
    {   
        //## �Ͻ����� ����
        Time.timeScale = 1.0f;

        //## ���ӽ���
        if(Start_Btn != null)
        {
            Start_Btn.onClick.AddListener(StartBtnClick);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartBtnClick()
    {
        SceneManager.LoadScene("ScPlay");
    }

}
