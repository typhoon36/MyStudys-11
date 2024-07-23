using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Mgr : MonoBehaviour
{
    //## 몬스터 변수
    Monster_Ctrl m_Monster = null;
    Player5_Ctrl m_Player = null;

    [Header("UI")]
    public Button m_RedShadeSwitch_Btn = null;
    public Button m_GrayShadeSwitch_Btn = null;

    // Start is called before the first frame update
    void Start()
    {
        //## 몬스터 변수 초기화
        m_Monster = FindObjectOfType<Monster_Ctrl>();
        m_Player = FindObjectOfType<Player5_Ctrl>();

        if (m_RedShadeSwitch_Btn != null)
            m_RedShadeSwitch_Btn.onClick.AddListener(() =>
            {
                if (m_Monster != null)
                    m_Monster.ColorSwitch();
            });

        if (m_GrayShadeSwitch_Btn != null)
            m_GrayShadeSwitch_Btn.onClick.AddListener(() =>
            {
                if(m_Player != null)
                    m_Player.StoneSwitch();
            });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
