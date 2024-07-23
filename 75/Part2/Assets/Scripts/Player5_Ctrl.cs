using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player5_Ctrl : MonoBehaviour
{
    Animation m_Anim;

    SkinnedMeshRenderer[] m_SMRList = null;
    MeshRenderer[] m_WMRList = null;

    //Hero 기본 쉐이더
    Shader m_DefShader = null;
    //## 무기 기본 쉐이더
    Shader m_WeaponShader = null;
    //그레이 쉐이더
    Shader m_GrayColorShader = null;

    bool m_IsStone = false;

    // Start is called before the first frame update
    void Start()
    {
        m_SMRList = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (m_SMRList != null && 0 < m_SMRList.Length)
            m_DefShader = m_SMRList[0].material.shader;

        m_WMRList = GetComponentsInChildren<MeshRenderer>();
        if (m_WMRList != null && 0 < m_WMRList.Length)
            m_WeaponShader = m_WMRList[0].material.shader;

        m_GrayColorShader = Shader.Find("Custom/GrayColor");

        m_Anim = gameObject.GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StoneSwitch();
        }
    }

    public void StoneSwitch()
    {
        m_IsStone = !m_IsStone;

        if (m_IsStone == true)
        {
            Material[] mts;
            for (int i = 0; i < m_SMRList.Length; i++)
            {
                mts = m_SMRList[i].materials;
                for (int a = 0; a < mts.Length; a++)
                {
                    if (m_GrayColorShader != null && mts[a].shader != m_GrayColorShader)
                    {
                        mts[a].shader = m_GrayColorShader;
                    }
                }
            }//for(int i = 0; i < m_SMRList.Length; i++)

            for (int i = 0; i < m_WMRList.Length; i++)
            {
                mts = m_WMRList[i].materials;
                for (int a = 0; a < mts.Length; a++)
                {
                    if (m_GrayColorShader != null && mts[a].shader != m_GrayColorShader)
                    {
                        mts[a].shader = m_GrayColorShader;
                    }
                }
            }//for(int i = 0; i < m_WMRList.Length; i++)

            if (m_Anim != null)
                //m_Anim["idle"].speed = 0.0f;
                m_Anim.Stop();

        }//if(m_IsStone == true)
        else
        {
            Material[] mts;
            for (int i = 0; i < m_SMRList.Length; i++)
            {
                mts = m_SMRList[i].materials;
                for (int a = 0; a < mts.Length; a++)
                {
                    if (m_DefShader != null && mts[a].shader != m_DefShader)
                    {
                        mts[a].shader = m_DefShader;
                    }
                }
            }//for(int i = 0; i < m_SMRList.Length; i++)

            for (int i = 0; i < m_WMRList.Length; i++)
            {
                mts = m_WMRList[i].materials;
                for (int a = 0; a < mts.Length; a++)
                {
                    if (m_WeaponShader != null && mts[a].shader != m_WeaponShader)
                    {
                        mts[a].shader = m_WeaponShader;
                    }
                }
            }//for(int i = 0; i < m_WMRList.Length; i++)

            if (m_Anim != null)
                //m_Anim["idle"].speed = 1.0f;
                m_Anim.Play();
        }
    }//public void StoneOnOff()
}
