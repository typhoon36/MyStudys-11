using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public GameObject[] CharObjs;
    //ĳ���� ������Ʈ�� ������ �迭 ����

    int CharType = 0;


    //## ������ȭ
    Vector3 a_CacVLen = Vector3.zero;
    Vector3 a_CacVDir = Vector3.zero;

    LayerMask m_WallLyMask = -1;
    List<WallCtrl> m_WCList = new List<WallCtrl>();



    public Transform targetTr;      //������ Ÿ�� ���ӿ�����Ʈ�� Transform ����
    public float dist = 10.0f;      //ī�޶���� ���� �Ÿ�
    public float height = 3.0f;     //ī�޶��� ���� ����
    public float dampTrace = 20.0f; //�ε巯�� ������ ���� ����

    Vector3 m_PlayerVec = Vector3.zero;
    float rotSpeed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;

        //## ����Ʈ 
        m_WallLyMask = 1 << LayerMask.NameToLayer("SideWall");

        //## ���̾ üũ�ϱ� ���� ���� ����
        GameObject[] a_SideWalls = GameObject.FindGameObjectsWithTag("SideWall");

        for (int i = 0; i < a_SideWalls.Length; i++)
        {
            WallCtrl a_WC = a_SideWalls[i].GetComponent<WallCtrl>();
            a_WC.m_IsColl = false;
            a_WC.AlphaOnOff(false);
            m_WCList.Add(a_WC);
        }


    }

    void Update()
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;

        if (Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
            if (GameMgr.IsPointerOverUIObject() == false)
            {
                //--- ī�޶� �� �Ʒ� �ٶ󺸴� ���� ������ ���� ������ ���� �ڵ�
                height -= (rotSpeed * Time.deltaTime * Input.GetAxis("Mouse Y"));

                if (height < 0.1f)
                    height = 0.1f;

                if (5.7f < height)
                    height = 5.7f;
                //--- ī�޶� �� �Ʒ� �ٶ󺸴� ���� ������ ���� ������ ���� �ڵ�
            }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CharChange();
        }




    }//void Update()

    //Update �Լ� ȣ�� ���� �� ���� ȣ��Ǵ� �Լ��� LateUpdate ���
    //������ Ÿ���� �̵��� ����� ���Ŀ� ī�޶� �����ϱ� ���� LateUpdate ���
    // Update is called once per frame
    void LateUpdate()
    {
        m_PlayerVec = targetTr.position;
        m_PlayerVec.y += 1.2f;

        //ī�޶��� ��ġ�� ��������� dist ������ŭ �������� ��ġ�ϰ�
        //height ������ŭ ���� �ø�
        transform.position = Vector3.Lerp(transform.position,
                                            targetTr.position
                                            - (targetTr.forward * dist)
                                            + (Vector3.up * height),
                                            Time.deltaTime * dampTrace);

        //ī�޶� Ÿ�� ���ӿ�����Ʈ�� �ٶ󺸰� ����
        transform.LookAt(m_PlayerVec);


        //## ���� ī�޶� ������
        a_CacVLen = transform.position - m_PlayerVec;

        //## ���ΰ����� ī�޶� ���ϴ� ���� ���� -- ĳ�����ʿ��� �����ϴ°��� ����.(��������)
        a_CacVDir = a_CacVLen.normalized;

        //## ����ĳ��Ʈ
        GameObject a_FindObj = null;
        RaycastHit a_HitInfo;
        if (Physics.Raycast(m_PlayerVec + (-a_CacVDir * 1.0f),
            a_CacVDir, out a_HitInfo, a_CacVLen.magnitude + 4.0f,
                m_WallLyMask.value))
        {
            a_FindObj = a_HitInfo.collider.gameObject;

        }

        //## ����Ʈ���� ã�� ������Ʈ�� ��
        for (int i = 0; i < m_WCList.Count; i++)
        {
            if (m_WCList[i].gameObject == a_FindObj)
            {
                if (m_WCList[i].m_IsColl == false)
                {
                    m_WCList[i].AlphaOnOff(true);
                    m_WCList[i].m_IsColl = true;
                }
            }

            else
            {

                if (m_WCList[i].m_IsColl == true)
                {
                    m_WCList[i].AlphaOnOff(false);
                    m_WCList[i].m_IsColl = false;
                }

            }


        }




    }

    //## ĳ���� ���� �Լ�
    void CharChange()
    {
        Vector3 a_Pos = CharObjs[CharType].transform.position;
        Quaternion a_Rot = CharObjs[CharType].transform.rotation;
        CharObjs[CharType].SetActive(false);
        CharType++;
        if (1 <  CharType)
            CharType = 0;

        CharObjs[CharType].SetActive(true);
        CharObjs[CharType].transform.position = a_Pos;
        CharObjs[CharType].transform.rotation = a_Rot;

        targetTr = CharObjs[CharType].transform;

    }





}
