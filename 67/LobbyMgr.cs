using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMgr : MonoBehaviour
{
    [Header("--- Btn ---")]
    public Button m_Start_Btn;
    public Button Store_Btn;
    public Button Exit_Btn;
    public Button ClearSV_Btn;

    [Header("--- Text ---")]
    public Text UserInfo_Txt;

    [HideInInspector] public int m_MyRank = 0;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f; //일시정지를 원래 속도로...

        GlobalValue.LoadGameData();

        if (m_Start_Btn != null)
            m_Start_Btn.onClick.AddListener(StartBtnClick);

        if (ClearSV_Btn != null)
            ClearSV_Btn.onClick.AddListener(ClearSV_Click);

        if (Exit_Btn != null)
            Exit_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Title_Scene");
            });

        RefreshUserInfo();

    }

    public void RefreshUserInfo()
    {
        UserInfo_Txt.text = "내 정보 : 별명 ( " + GlobalValue.g_NickName +
            ") : 순위 ( " + m_MyRank.ToString() + "등 ) : 점수(" +
            GlobalValue.g_BestScore.ToString("N0") + "점) : 골드(" + GlobalValue.g_UserGold.ToString("N0") + ")";
    }

    void ClearSV_Click()
    {
        PlayerPrefs.DeleteAll();
        GlobalValue.LoadGameData();
        RefreshUserInfo();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartBtnClick()
    {
        SceneManager.LoadScene("scLevel01");
        SceneManager.LoadScene("scPlay", LoadSceneMode.Additive);
    }
}
