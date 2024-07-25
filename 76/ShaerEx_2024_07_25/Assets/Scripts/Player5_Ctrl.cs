using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player5_Ctrl : MonoBehaviour
{
    Animation anim;
    //## ¸öÅë ½ºÅ² ·»´õ·¯
    SkinnedMeshRenderer[] m_SMRList = null;
    //## ÃÑ±âÀÇ ·»´õ·¯
    MeshRenderer[] m_WRList = null;

    Shader m_DefShade = null;
    Shader m_WeaponShade = null;
    Shader m_GrayShade = null;

    //## ¿©ºÎ
    bool IsStoned = false;

    // Start is called before the first frame update
    void Start()
    {
        //## ¸öÅë ½ºÅ² ·»´õ·¯
        m_SMRList = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (m_SMRList != null && m_SMRList.Length > 0)
            m_DefShade = m_SMRList[0].material.shader;
        //## ÃÑ±âÀÇ ·»´õ·¯
        m_WRList = GetComponentsInChildren<MeshRenderer>();
        if (m_WRList != null && m_WRList.Length > 0)
            m_WeaponShade = m_WRList[0].material.shader;

        //## ±×·¹ÀÌ ½¦ÀÌ´õ
        m_GrayShade = Shader.Find("Custom/GrayColor");

        //## ¾Ö´Ï¸ÞÀÌ¼Ç
        anim = gameObject.GetComponent<Animation>();
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        StonedSwitch();
    //    }
    //}

    public void StonedSwitch()
    {
        IsStoned = !IsStoned;

        if (IsStoned)
        {
            Material[] mats;

            for (int i = 0; i < m_SMRList.Length; i++)
            {
                mats = m_SMRList[i].materials;
                for (int a = 0; a < mats.Length; a++)
                {
                    mats[a].shader = m_GrayShade;
                }
            }

            for (int i = 0; i < m_WRList.Length; i++)
            {
                mats = m_WRList[i].materials;
                for (int j = 0; j < mats.Length; j++)
                {
                    mats[j].shader = m_GrayShade;
                }
            }

            if (anim != null)
            {
                anim.Stop();
            }
        }
        else
        {
            Material[] mats;

            for (int i = 0; i < m_SMRList.Length; i++)
            {
                mats = m_SMRList[i].materials;
                for (int a = 0; a < mats.Length; a++)
                {
                    mats[a].shader = m_DefShade;
                }
            }

            for (int i = 0; i < m_WRList.Length; i++)
            {
                mats = m_WRList[i].materials;
                for (int a = 0; a < mats.Length; a++)
                {
                    mats[a].shader = m_WeaponShade;
                }
            }

            if (anim != null)
            {
                anim.Play();
            }
        }
    }
}
