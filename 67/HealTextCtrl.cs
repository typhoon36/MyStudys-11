using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealTextCtrl : MonoBehaviour
{
    Text m_RefText = null;
    float m_HealVal = 0.0f;
    Vector3 m_WorldPos = Vector3.zero;
    Animator m_RefAnim = null;

    //# �������ؽ�Ʈ�� ���� ����
    Transform m_RefHCanvas = null;
    RectTransform m_CanvasRect = null;
    Vector3 m_BaseWdPos = Vector3.zero;
    Vector2 m_ScreenPos = Vector2.zero;
    Vector2 m_WdScpos = Vector2.zero;
    Vector3 m_CacVec = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void LateUpdate()
    //{
    //    //## ������ǥ -> UGUI ��ǥ�� ��ȯ
    //    m_CanvasRect = m_RefHCanvas.GetComponent<RectTransform>();

    //    m_ScreenPos = Camera.main.WorldToViewportPoint(m_WorldPos);

    //    m_WdScpos.x = (m_ScreenPos.x * m_CanvasRect.sizeDelta.x) - (m_CanvasRect.sizeDelta.x * 0.5f);
    //    m_WdScpos.y = (m_ScreenPos.y * m_CanvasRect.sizeDelta.y) - (m_CanvasRect.sizeDelta.y * 0.5f);

    //    transform.GetComponent<RectTransform>().anchoredPosition = m_WdScpos;

    //    //## ī�޶� �ø�
    //    m_CacVec = m_BaseWdPos - Camera.main.transform.position;

    //    if(m_CacVec.magnitude <= 0)
    //    {
    //        //## �ؽ�Ʈ�� ī�޶�� �������� ���� �� �������ʰ� 
    //        if(m_RefText.gameObject.activeSelf == true)
    //            m_RefText.gameObject.SetActive(false);
    //    }
    //    else if(0.0f < Vector3.Dot(Camera.main.transform.forward, m_CacVec.normalized))
    //    {
    //        //## ī�޶� ���ʿ� ��ġ
    //        if (m_RefText.gameObject.activeSelf == true)
    //            m_RefText.gameObject.SetActive(true);
    //    }
    //    else //(0.0f > Vector3.Dot(Camera.main.transform.forward, m_CacVec.normalized))
    //    {
    //        //## ī�޶� ���ʿ� ��ġ
    //        if (m_RefText.gameObject.activeSelf == true)
    //            m_RefText.gameObject.SetActive(false);
    //    }

    //}

    public void InitState(int cont, Vector3 a_WSpawnPos, 
                            Transform a_Heal_Canvas, Color a_Color)
    {
        Vector3 a_StCacPos = new Vector3(a_WSpawnPos.x,
                                        a_WSpawnPos.y + 2.21f, a_WSpawnPos.z);
        transform.SetParent(a_Heal_Canvas, false);
        m_WorldPos = a_WSpawnPos;
        m_HealVal = cont;

        m_RefHCanvas = a_Heal_Canvas;
        m_BaseWdPos = a_StCacPos;



        //--- �ʱ� ��ġ ����ֱ� //---World ��ǥ�� UGUI ��ǥ�� ȯ���� �ִ� �ڵ�
        RectTransform a_CanvasRect = a_Heal_Canvas.GetComponent<RectTransform>();
        Vector2 a_ScreenPos = Camera.main.WorldToViewportPoint(a_StCacPos);
        Vector2 a_WdScPos = Vector2.zero;
        a_WdScPos.x = (a_ScreenPos.x * a_CanvasRect.sizeDelta.x) - (a_CanvasRect.sizeDelta.x * 0.5f);
        a_WdScPos.y = (a_ScreenPos.y * a_CanvasRect.sizeDelta.y) - (a_CanvasRect.sizeDelta.y * 0.5f);
        //a_CanvasRect.sizeDelta �� UI ������ ȭ�� ũ�⿡ 1280 * 720
        this.GetComponent<RectTransform>().anchoredPosition = a_WdScPos;
        //--- �ʱ� ��ġ ����ֱ� //---World ��ǥ�� UGUI ��ǥ�� ȯ���� �ִ� �ڵ�

        m_RefText = this.gameObject.GetComponentInChildren<Text>();
        if(m_RefText != null)
        {
            if (m_HealVal <= 0)
                m_RefText.text = m_HealVal.ToString() + " Dmg";
            else
                m_RefText.text = "+" + m_HealVal.ToString() + " Heal";

            m_RefText.color = a_Color;
        }

        m_RefAnim = GetComponentInChildren<Animator>();
        if(m_RefAnim != null)
        {
            AnimatorStateInfo a_AnimInfo = m_RefAnim.GetCurrentAnimatorStateInfo(0);
            float a_LifeTime = a_AnimInfo.length; //�ִϸ��̼� �÷��� �ð�
            Destroy(gameObject, a_LifeTime);
        }

    }
}
