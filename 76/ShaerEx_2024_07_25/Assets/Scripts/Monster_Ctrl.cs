using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Ctrl : MonoBehaviour
{
    SkinnedMeshRenderer[] m_SMRList = null;

    Shader m_DefShader = null;
    Shader m_AddTexShader = null;

    Color m_CacColor;

    bool IsDamage = false;

    // Start is called before the first frame update
    void Start()
    {
        
        m_SMRList = GetComponentsInChildren<SkinnedMeshRenderer>();

        if(m_SMRList != null && 0 < m_SMRList.Length)
            m_DefShader = m_SMRList[0].material.shader;

        m_AddTexShader = Shader.Find("Custom/AddTexColor");

        m_CacColor = new Color(1.0f, 0.2f, 0.2f, 1.0f);
        


    }

    // Update is called once per frame
    //void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Space))
    //    {
    //        ColorSwitch();
    //    }
    //}

    public void ColorSwitch()
    {
        IsDamage = !IsDamage;

        if(IsDamage == true)
        {
            for(int i = 0; i < m_SMRList.Length; i++)
            {
                m_SMRList[i].material.shader = m_AddTexShader;
                m_SMRList[i].material.SetColor("_AddColor", m_CacColor);
            }
        }
        else
        {
            Material[] mts;
            for(int i = 0; i < m_SMRList.Length; i++)
            {
                mts = m_SMRList[i].materials;
                for(int a = 0; a < mts.Length; a++)
                {
                    if(m_DefShader != null && mts[a] != m_DefShader)
                        mts[a].shader = m_DefShader;
                }
            }
        }
    }

}
