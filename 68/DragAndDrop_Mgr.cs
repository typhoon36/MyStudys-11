using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DragAndDrop_Mgr : MonoBehaviour
{
    public Slot_Ctrl[] m_SlotSc;
    public Image m_MsObj;
    public Image m_MsIcon;
    public Sprite[] MsSpIcon;

    private int m_SaveIndex;
    private int m_DrtIndex;
    private float AniDuring;
    private float m_CacTime;
    private float m_AddTimer;
    private Color m_Color;

    [Header("--- Message TextUI ---")]
    public Text m_Message_Txt;
    private float m_HelpDuring;
    private float m_HelpTimer;

    [Header("--- Bag TextUI ---")]
    public Text m_BagCnt_Txt;

    [Header("--- Slot Cnt---")]
    public Text[] Item_Cnt;

    void Start()
    {
        // 초기화 시 BagItemCount 텍스트 업데이트
        if (m_BagCnt_Txt != null)
            m_BagCnt_Txt.text = "가방 크기 : " + GlobalValue.g_BagItemCount.ToString() + " / " +
                GlobalValue.g_BagItemLimit.ToString();

        if (Item_Cnt != null)
        {
            for (int i = 0; i < Item_Cnt.Length; i++)
            {
                Item_Cnt[i].text = GlobalValue.g_SkillCount[i].ToString();
            }
        }
    }

    void Update()
    {
        //## 마우스 클릭처리
        if (Input.GetMouseButtonDown(0) == true)
        {
            BuyMouseBtnDown();
        }

        if (Input.GetMouseButton(0) == true)
        {
            BuyMouseBtnPress();
        }

        if (Input.GetMouseButtonUp(0) == true)
        {
            BuyMouseBtnUp();
        }

        BuyDirection();
    }

    bool IsCollSlot(GameObject a_CkObj)  //마우스가 UI 슬롯 오브젝트 위에 있으냐? 판단하는 함수
    {
        Vector3[] v = new Vector3[4];
        a_CkObj.GetComponent<RectTransform>().GetWorldCorners(v);

        if (v[0].x <= Input.mousePosition.x && Input.mousePosition.x <= v[2].x &&
            v[0].y <= Input.mousePosition.y && Input.mousePosition.y <= v[2].y)
        {
            return true;
        }

        return false;
    }

    private void BuyMouseBtnDown()
    {
        m_SaveIndex = -1;

        for (int i = 0; i < m_SlotSc.Length; i++)
        {
            if (m_SlotSc[i].ItemImg.gameObject.activeSelf == true &&
                IsCollSlot(m_SlotSc[i].gameObject) == true)
            {
                m_SaveIndex = i;
                m_SlotSc[i].ItemImg.gameObject.SetActive(false);
                m_MsObj.gameObject.SetActive(true);

                m_MsObj.transform.position = Input.mousePosition;

                // 슬롯 인덱스에 따라 마우스 아이콘 이미지 변경
                int spriteIndex = (i / 2) % MsSpIcon.Length;
                m_MsIcon.sprite = MsSpIcon[spriteIndex];

                break;
            }
        }
    }

    private void BuyMouseBtnPress()
    {
        if (0 <= m_SaveIndex)
        {
            m_MsObj.transform.position = Input.mousePosition;
        }
    }

    private void BuyMouseBtnUp()
    {
        if (m_SaveIndex < 0)
            return;

        for (int i = 0; i < m_SlotSc.Length; i++)
        {
            if (m_SaveIndex == i)  //자기 자리에 놓은 경우 구매 불가
                continue;

            if (IsCollSlot(m_SlotSc[i].gameObject) == true)
            {
                //--- 구매 제한 확인
                if (GlobalValue.g_BagItemCount >= GlobalValue.g_BagItemLimit)
                {
                    // 가방이 가득 찼을 경우
                    m_Message_Txt.text = "가방이 꽉찼습니다.";
                    m_Message_Txt.gameObject.SetActive(true);
                    m_HelpTimer = m_HelpDuring;
                    break;
                }

                //--- 구매 허가
                int purchasePrice = GetPurchasePrice(i); // 해당 슬롯의 구매 금액을 가져옴
                if (purchasePrice <= GlobalValue.g_UserGold)
                {
                    m_SlotSc[i].ItemImg.gameObject.SetActive(true);
                    m_SlotSc[i].ItemImg.color = Color.white;
                    m_DrtIndex = i;
                    m_AddTimer = AniDuring;
                    m_MsObj.gameObject.SetActive(false);

                    GlobalValue.g_UserGold -= purchasePrice; // 슬롯별 구매 금액 차감
                    GlobalValue.g_BagItemCount++; // 가방 아이템 개수 증가

                    // 인덱스가 유효한지 확인
                    if (i == 1 || i == 3 || i == 5)
                    {
                        int skillCountIndex = GetSkillCountIndex(i);
                        GlobalValue.g_SkillCount[skillCountIndex]++; // 스킬 카운트 증가

                        if (Item_Cnt[skillCountIndex] != null)
                            Item_Cnt[skillCountIndex].text = GlobalValue.g_SkillCount[skillCountIndex].ToString();

                        PlayerPrefs.SetInt("SkillCount" + skillCountIndex.ToString(), GlobalValue.g_SkillCount[skillCountIndex]); // SkillCount 저장
                    }

                    if (m_BagCnt_Txt != null)
                        m_BagCnt_Txt.text = "가방 크기 : " + GlobalValue.g_BagItemCount.ToString() + " / " + GlobalValue.g_BagItemLimit.ToString();

                    Debug.Log(GlobalValue.g_BagItemCount);

                    PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
                    PlayerPrefs.SetInt("BagItemCount", GlobalValue.g_BagItemCount); // BagItemCount 저장
                    m_Message_Txt.gameObject.SetActive(true);
                    m_Message_Txt.text = "<color=#ffff00>구매성공!</color>";
                }
                else  //구매 불가
                {
                    m_Message_Txt.gameObject.SetActive(true);
                    m_Message_Txt.color = Color.red;
                    m_HelpTimer = m_HelpDuring;
                }

                break;
            }
        }

        if (0 <= m_SaveIndex)
        {
            m_SlotSc[m_SaveIndex].ItemImg.gameObject.SetActive(true);
            m_SaveIndex = -1;
            m_MsObj.gameObject.SetActive(false);
        }
    }

    // 슬롯 인덱스에 따른 구매 금액을 반환하는 메서드
    private int GetPurchasePrice(int slotIndex)
    {
        switch (slotIndex)
        {
            case 1:
                return 300; // 두 번째 슬롯
            case 3:
                return 500; // 네 번째 슬롯
            case 5:
                return 1000; // 여섯 번째 슬롯
            default:
                return 0; // 기본값 혹은 제외된 슬롯
        }
    }

    // 슬롯 인덱스에 따른 스킬 카운트 인덱스를 반환하는 메서드
    private int GetSkillCountIndex(int slotIndex)
    {
        switch (slotIndex)
        {
            case 1:
                return 0; // 첫 번째 텍스트 배열
            case 3:
                return 1; // 두 번째 텍스트 배열
            case 5:
                return 2; // 세 번째 텍스트 배열
            default:
                return -1; // 유효하지 않은 인덱스
        }
    }

    //## 구매 연출
    void BuyDirection()
    {
        //## 페이드 아웃
        if (0.0f < m_AddTimer)
        {
            m_AddTimer -= Time.deltaTime;
            m_CacTime = m_AddTimer / AniDuring;
            m_Color = m_SlotSc[m_DrtIndex].ItemImg.color;
            m_Color.a = m_CacTime;
            m_SlotSc[m_DrtIndex].ItemImg.color = m_Color;

            if (m_AddTimer <= 0.0f)
            {
                m_SlotSc[m_DrtIndex].ItemImg.gameObject.SetActive(false);
            }
        }
    }
}


