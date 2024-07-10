using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDropMgr : MonoBehaviour
{
    //# 상품을 위한 슬롯 배열
    public Slot_Ctrl[] m_Slots;
    //# 인벤토리를 위한 슬롯 배열
    public Slot_Ctrl[] m_InvenSlots;

    //# 드래그 중인 아이템이미지
    public Image m_MsObj = null;
    //## 드래그 중인 아이템의 인덱스
    public int m_SaveIdx = -1;

    //## 가방 사이즈
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

        //## 연출(서서히 사라지게하기)
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

    //# 마우스를 눌렀을때
    void MouseBtnUp()
    {
        //## 예외
        if (m_SaveIdx < 0 || m_Slots.Length <= m_SaveIdx)
            return;
        //## 인벤 아이템 장착
        int a_BuyIdx = -1;
        for (int i = 0; i < m_InvenSlots.Length; i++)
        {
            if (IsCollSlot(m_InvenSlots[i]) == true)
            {

                if (m_SaveIdx != i)
                {
                    ShowMessage("슬롯에는 장착할 수 없습니다!!");
                    continue;
                }

                //## 상품 구매 호출
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



    //# 마우스를 누르고있을때
    void MousePress()
    {
        if (0 <= m_SaveIdx)
            m_MsObj.transform.position = Input.mousePosition;
    }

    //# 마우스를 놨을때
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

    //# 메시지 보여주기
    void ShowMessage(string a_Msg)
    {
        if (m_Help_Txt == null)

            return;

        m_Help_Txt.text = a_Msg;
        m_Help_Txt.gameObject.SetActive(true);
        m_HelpAddTime = m_HelpDur;



    }

    //# 구매시도
    bool BuyItem(int a_ItemIdx)
    {
        //# 테이블값이 없기에 임시 구현
        int a_BuyCost = 300;

        if (a_ItemIdx == 1)
            a_BuyCost = 500;
        else if (a_ItemIdx == 2)
            a_BuyCost = 1000;

        if (GlobalValue.g_UserGold < a_BuyCost)
        {
            ShowMessage("골드가 부족합니다!!");
            return false;
        }

        int a_CurBagSize = 0;
        for (int i = 0; i < GlobalValue.g_SkillCount.Length; i++)
            a_CurBagSize += GlobalValue.g_SkillCount[i];

        if (10<= a_CurBagSize)
        {
            ShowMessage("가방이 가득 찼습니다!!");
            return false;
        }
        //## 정식 구매(다이얼로그 창 띄우기) --> 정말 구매?라고 물어보기
        //## 구매처리
        GlobalValue.g_SkillCount[a_ItemIdx]++;
        GlobalValue.g_UserGold -= a_BuyCost;

        //## 저장

        string a_MkKey = "SkItem_" + a_ItemIdx.ToString();

        PlayerPrefs.SetInt(a_MkKey, GlobalValue.g_SkillCount[a_ItemIdx]);
        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);


        RefreshUI();

        return true;
    }

    //# 마우스가 슬롯 위에 있는지 여부
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

    //# UI 갱신
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
            m_StoreMgr.m_UserInfo_Txt.text = "별명(" + GlobalValue.g_NickName +
                ") : 보유 골드( " + GlobalValue.g_UserGold + ")";

        int a_CBagsize = 0;
        for (int i = 0; i< GlobalValue.g_SkillCount.Length; i++)
            a_CBagsize+= GlobalValue.g_SkillCount[i];

        m_BagSize_Txt.text = "가방 크기 : " + a_CBagsize + "/ 10";
    }
}
