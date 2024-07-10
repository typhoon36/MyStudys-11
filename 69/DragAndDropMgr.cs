using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDropMgr : MonoBehaviour
{
    //# ��ǰ�� ���� ���� �迭
    public Slot_Ctrl[] m_Slots;
    //# �κ��丮�� ���� ���� �迭
    public Slot_Ctrl[] m_InvenSlots;

    //# �巡�� ���� �������̹���
    public Image m_MsObj = null;
    //## �巡�� ���� �������� �ε���
    public int m_SaveIdx = -1;

    //## ���� ������
    public Text m_BagSize_Txt;
    public Text m_Help_Txt;
    float m_HelpDur = 2.0f;
    float m_HelpAddTime = 0.0f;
    float m_CacTime = 0.0f;
    Color m_Color;

    Store_Mgr m_StoreMgr = null;

    // Start is called before the first frame update
    void Start()
    {
        m_StoreMgr = GameObject.FindObjectOfType<Store_Mgr>();

        RefreshUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)==true)
        {
            MouseBtnDown();
        }

        if (Input.GetMouseButton(0) == true)
        {
            MousePress();
        }

        if (Input.GetMouseButtonUp(0) == true)
        {
            MouseBtnUp();
        }

        //## ����(������ ��������ϱ�)
        if (0 < m_HelpAddTime)
        {
            m_HelpAddTime -= Time.deltaTime;
            m_CacTime = m_HelpAddTime / (m_HelpDur -1.0f);
            if (1.0 < m_CacTime)
                m_CacTime = 1.0f;

            m_Color = m_Help_Txt.color;
            m_Color.a = m_CacTime;
            m_Help_Txt.color = m_Color;

            if (m_HelpAddTime <= 0.0f)
                m_Help_Txt.gameObject.SetActive(false);



        }

    }

    //# ���콺�� ��������
    void MouseBtnUp()
    {
        //## ����
        if (m_SaveIdx < 0 || m_Slots.Length <= m_SaveIdx)
            return;
        //## �κ� ������ ����
        int a_BuyIdx = -1;
        for (int i = 0; i < m_InvenSlots.Length; i++)
        {
            if (IsCollSlot(m_InvenSlots[i]) == true)
            {

                if (m_SaveIdx != i)
                {
                    ShowMessage("���Կ��� ������ �� �����ϴ�!!");
                    continue;
                }

                //## ��ǰ ���� ȣ��
                if (BuyItem(m_SaveIdx) == true)
                {
                    a_BuyIdx = i;
                    break;
                }



            }
        }


        if (0 <= a_BuyIdx)
        {
            Sprite a_MsIcon = null;
            Transform a_ChildIcon = m_MsObj.transform.Find("MsIconImg");

            if (a_ChildIcon != null)
                a_MsIcon = a_ChildIcon.GetComponent<Image>().sprite;

            m_InvenSlots[a_BuyIdx].Item_Icon.sprite = a_MsIcon;
            m_InvenSlots[a_BuyIdx].Item_Icon.gameObject.SetActive(true);
            m_InvenSlots[a_BuyIdx].m_CurItemIdx = a_BuyIdx;
        }

        //else
        //{
        //    m_Slots[m_SaveIdx].Item_Icon.gameObject.SetActive(true);
        //}

        m_SaveIdx = -1;
        m_MsObj.gameObject.SetActive(false);





    }



    //# ���콺�� ������������
    void MousePress()
    {
        if (0 <= m_SaveIdx)
            m_MsObj.transform.position = Input.mousePosition;
    }

    //# ���콺�� ������
    void MouseBtnDown()
    {
        m_SaveIdx = -1;

        for (int i = 0; i < m_Slots.Length; i++)
        {
            if (m_Slots[i].Item_Icon.gameObject.activeSelf == true &&
                IsCollSlot(m_Slots[i]) == true)
            {
                m_SaveIdx = i;
                Transform a_ChildIcon = m_MsObj.transform.Find("MsIconImg");
                if (a_ChildIcon != null)
                    a_ChildIcon.GetComponent<Image>().sprite =
                        m_Slots[i].Item_Icon.sprite;

                //m_Slots[i].Item_Icon.gameObject.SetActive(false);
                m_MsObj.gameObject.SetActive(true);
                break;

            }
        }
    }

    //# �޽��� �����ֱ�
    void ShowMessage(string a_Msg)
    {
        if (m_Help_Txt == null)

            return;

        m_Help_Txt.text = a_Msg;
        m_Help_Txt.gameObject.SetActive(true);
        m_HelpAddTime = m_HelpDur;



    }

    //# ���Žõ�
    bool BuyItem(int a_ItemIdx)
    {
        //# ���̺��� ���⿡ �ӽ� ����
        int a_BuyCost = 300;

        if (a_ItemIdx == 1)
            a_BuyCost = 500;
        else if (a_ItemIdx == 2)
            a_BuyCost = 1000;

        if (GlobalValue.g_UserGold < a_BuyCost)
        {
            ShowMessage("��尡 �����մϴ�!!");
            return false;
        }

        int a_CurBagSize = 0;
        for (int i = 0; i < GlobalValue.g_SkillCount.Length; i++)
            a_CurBagSize += GlobalValue.g_SkillCount[i];

        if (10<= a_CurBagSize)
        {
            ShowMessage("������ ���� á���ϴ�!!");
            return false;
        }
        //## ���� ����(���̾�α� â ����) --> ���� ����?��� �����
        //## ����ó��
        GlobalValue.g_SkillCount[a_ItemIdx]++;
        GlobalValue.g_UserGold -= a_BuyCost;

        //## ����

        string a_MkKey = "SkItem_" + a_ItemIdx.ToString();

        PlayerPrefs.SetInt(a_MkKey, GlobalValue.g_SkillCount[a_ItemIdx]);
        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);


        RefreshUI();

        return true;
    }

    //# ���콺�� ���� ���� �ִ��� ����
    bool IsCollSlot(Slot_Ctrl a_CKslot)
    {
        if (a_CKslot == null)
        {
            return false;
        }



        Vector3[] v = new Vector3[4];
        a_CKslot.GetComponent<RectTransform>().GetWorldCorners(v);


        if (v[0].x < Input.mousePosition.x && v[2].x > Input.mousePosition.x)
        {
            if (v[0].y < Input.mousePosition.y && v[1].y > Input.mousePosition.y)
            {
                return true;
            }
        }

        return false;
    }

    //# UI ����
    void RefreshUI()
    {
        for (int i = 0; i < m_InvenSlots.Length; i++)
        {
            if (0 < GlobalValue.g_SkillCount[i])
            {
                m_InvenSlots[i].ItemCnt_Txt.text = GlobalValue.g_SkillCount[i].ToString();
                m_InvenSlots[i].Item_Icon.sprite = m_Slots[i].Item_Icon.sprite;
                m_InvenSlots[i].Item_Icon.gameObject.SetActive(true);
                m_InvenSlots[i].m_CurItemIdx = i;
            }
            else
            {
                m_InvenSlots[i].ItemCnt_Txt.text = "0";
                m_InvenSlots[i].Item_Icon.gameObject.SetActive(false);

            }
        }

        if (m_StoreMgr != null && m_StoreMgr.m_UserInfo_Txt != null)
            m_StoreMgr.m_UserInfo_Txt.text = "����(" + GlobalValue.g_NickName +
                ") : ���� ���( " + GlobalValue.g_UserGold + ")";

        int a_CBagsize = 0;
        for (int i = 0; i< GlobalValue.g_SkillCount.Length; i++)
            a_CBagsize+= GlobalValue.g_SkillCount[i];

        m_BagSize_Txt.text = "���� ũ�� : " + a_CBagsize + "/ 10";
    }
}
