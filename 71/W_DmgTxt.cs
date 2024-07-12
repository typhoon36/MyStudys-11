using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class W_DmgTxt : MonoBehaviour
{
    Transform m_CamTr = null;
    Animator m_Anim = null;
    Text m_DmgTxt = null;
    float m_DmgVal = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        m_CamTr = Camera.main.transform;
    }

    void LateUpdate()
    {
        //## ºôº¸µå È¿°ú
        transform.forward = m_CamTr.forward;

    }

    public void InitState(int cont , Vector3 a_WSPos, Color a_Color)
    {
        m_Anim = GetComponentInChildren<Animator>();

        if(m_Anim != null)
        {
            AnimatorStateInfo a_Info = m_Anim.GetCurrentAnimatorStateInfo(0);
            float a_LTime = a_Info.length;

            Destroy(gameObject ,a_LTime);
        }

        transform.position = a_WSPos;

        m_DmgVal = cont;

        m_DmgTxt = gameObject.GetComponentInChildren<Text>();

        if(m_DmgTxt != null)
        {
            if (m_DmgVal <= 0)
                m_DmgTxt.text = m_DmgVal.ToString() + "Dmg";

            else
                m_DmgTxt.text = "+" + m_DmgVal.ToString() + "Heal";

            m_DmgTxt.color = a_Color;
        }


    }




}
