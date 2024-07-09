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
        // �ʱ�ȭ �� BagItemCount �ؽ�Ʈ ������Ʈ
        if (m_BagCnt_Txt != null)
            m_BagCnt_Txt.text = "���� ũ�� : " + GlobalValue.g_BagItemCount.ToString() + " / " +
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
        //## ���콺 Ŭ��ó��
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

    bool IsCollSlot(GameObject a_CkObj)  //���콺�� UI ���� ������Ʈ ���� ������? �Ǵ��ϴ� �Լ�
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

                // ���� �ε����� ���� ���콺 ������ �̹��� ����
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
            if (m_SaveIndex == i)  //�ڱ� �ڸ��� ���� ��� ���� �Ұ�
                continue;

            if (IsCollSlot(m_SlotSc[i].gameObject) == true)
            {
                //--- ���� ���� Ȯ��
                if (GlobalValue.g_BagItemCount >= GlobalValue.g_BagItemLimit)
                {
                    // ������ ���� á�� ���
                    m_Message_Txt.text = "������ ��á���ϴ�.";
                    m_Message_Txt.gameObject.SetActive(true);
                    m_HelpTimer = m_HelpDuring;
                    break;
                }

                //--- ���� �㰡
                int purchasePrice = GetPurchasePrice(i); // �ش� ������ ���� �ݾ��� ������
                if (purchasePrice <= GlobalValue.g_UserGold)
                {
                    m_SlotSc[i].ItemImg.gameObject.SetActive(true);
                    m_SlotSc[i].ItemImg.color = Color.white;
                    m_DrtIndex = i;
                    m_AddTimer = AniDuring;
                    m_MsObj.gameObject.SetActive(false);

                    GlobalValue.g_UserGold -= purchasePrice; // ���Ժ� ���� �ݾ� ����
                    GlobalValue.g_BagItemCount++; // ���� ������ ���� ����

                    // �ε����� ��ȿ���� Ȯ��
                    if (i == 1 || i == 3 || i == 5)
                    {
                        int skillCountIndex = GetSkillCountIndex(i);
                        GlobalValue.g_SkillCount[skillCountIndex]++; // ��ų ī��Ʈ ����

                        if (Item_Cnt[skillCountIndex] != null)
                            Item_Cnt[skillCountIndex].text = GlobalValue.g_SkillCount[skillCountIndex].ToString();

                        PlayerPrefs.SetInt("SkillCount" + skillCountIndex.ToString(), GlobalValue.g_SkillCount[skillCountIndex]); // SkillCount ����
                    }

                    if (m_BagCnt_Txt != null)
                        m_BagCnt_Txt.text = "���� ũ�� : " + GlobalValue.g_BagItemCount.ToString() + " / " + GlobalValue.g_BagItemLimit.ToString();

                    Debug.Log(GlobalValue.g_BagItemCount);

                    PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
                    PlayerPrefs.SetInt("BagItemCount", GlobalValue.g_BagItemCount); // BagItemCount ����
                    m_Message_Txt.gameObject.SetActive(true);
                    m_Message_Txt.text = "<color=#ffff00>���ż���!</color>";
                }
                else  //���� �Ұ�
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

    // ���� �ε����� ���� ���� �ݾ��� ��ȯ�ϴ� �޼���
    private int GetPurchasePrice(int slotIndex)
    {
        switch (slotIndex)
        {
            case 1:
                return 300; // �� ��° ����
            case 3:
                return 500; // �� ��° ����
            case 5:
                return 1000; // ���� ��° ����
            default:
                return 0; // �⺻�� Ȥ�� ���ܵ� ����
        }
    }

    // ���� �ε����� ���� ��ų ī��Ʈ �ε����� ��ȯ�ϴ� �޼���
    private int GetSkillCountIndex(int slotIndex)
    {
        switch (slotIndex)
        {
            case 1:
                return 0; // ù ��° �ؽ�Ʈ �迭
            case 3:
                return 1; // �� ��° �ؽ�Ʈ �迭
            case 5:
                return 2; // �� ��° �ؽ�Ʈ �迭
            default:
                return -1; // ��ȿ���� ���� �ε���
        }
    }

    //## ���� ����
    void BuyDirection()
    {
        //## ���̵� �ƿ�
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


