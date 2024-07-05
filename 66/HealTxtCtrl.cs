using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealTxtCtrl : MonoBehaviour
{
    public Text m_RefTxt = null;
    float m_HVal = 0.0f;
    Vector3 m_Wpos = Vector3.zero;
    Animator m_RefAnim = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int cont, Vector3 a_WSpawnPos,  Transform H_Canvas, Color a_color)
    {

        Vector3 a_StPos = new Vector3(a_WSpawnPos.x, a_WSpawnPos.y + 2.21f, a_WSpawnPos.z);
        
        transform.SetParent(H_Canvas,false);     

        m_Wpos = a_WSpawnPos;
        m_HVal = cont;


        //## 위치 ---> World 좌표 ---> UGUI 좌표
        RectTransform a_HRect = H_Canvas.GetComponent<RectTransform>();
        Vector2 a_ScPos = Camera.main.WorldToViewportPoint(a_StPos);
        Vector2 a_WPos = Vector2.zero;
        a_WPos.x = (a_ScPos.x  * a_HRect.sizeDelta.x) - (a_HRect.sizeDelta.x * 0.5f);
        a_WPos.y = (a_ScPos.y  * a_HRect.sizeDelta.y) - (a_HRect.sizeDelta.y * 0.5f);

        this.GetComponent<RectTransform>().anchoredPosition = a_WPos;


        m_RefTxt = this.gameObject.GetComponentInChildren<Text>();

        if(m_RefTxt != null)
        {
            if(m_HVal <= 0) 
                m_RefTxt.text = "-" + m_HVal.ToString() + "Dmg";
            else 
                m_RefTxt.text = "+" + m_HVal.ToString() + "Heal";

            m_RefTxt.color = a_color;
        }

        m_RefAnim = GetComponentInChildren<Animator>();
        
        //## 애니메이션 재생 후 삭제
        if(m_RefAnim != null)
        {
            AnimatorStateInfo a_AnimInfo = m_RefAnim.GetCurrentAnimatorStateInfo(0);
            float a_LTime = a_AnimInfo.length;
            Destroy(this.gameObject, a_LTime);
        }

    }
}
