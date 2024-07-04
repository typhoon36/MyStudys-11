using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FollowCam : MonoBehaviour
{
    public GameObject[] CharObjs;   //ĳ���� ����
    

    public Transform targetTr;      //������ Ÿ�� ���ӿ�����Ʈ�� Transform ����

    FireCtrl m_fireCtrl = null;

    public float dist = 10.0f;      //ī�޶���� ���� �Ÿ�
    public float height = 3.0f;     //ī�޶��� ���� ����
    public float dampTrace = 20.0f; //�ε巯�� ������ ���� ����

    Vector3 m_PlayerVec = Vector3.zero;
    float rotSpeed = 10.0f;

    //--- Wall ����ȭ ó���� ���� ����Ʈ ���� ����
    Vector3 a_CacVLen = Vector3.zero;
    Vector3 a_CacDirVec = Vector3.zero;

    LayerMask m_WallLyMask = -1;
    List<WallCtrl> m_SW_List = new List<WallCtrl>();
    //--- Wall ����ȭ ó���� ���� ����Ʈ ���� ����

    //# ī�޶� ��ġ ���� 
    float m_RotV = 0.0f;
    float m_DefaultRotV = 25.2f;
    float m_MarginRotV = 22.3f;
    
    //## �� �Ʒ� ���� 
    float m_MinV = -17.9f;
    float m_MaxV = 52.9f;
    //##  ���ξƿ� �Ÿ� ����
    float m_MaxDist = 4.0f;
    float m_MinDist = 2.0f;
    //### �� �ӵ�
    float m_ZoomSpeed = 0.7f;

    //# ī�޶� ȸ��
    Quaternion m_BuffRot;
    //ȸ�� ��ġ
    Vector3 m_BuffPos;
    // ��ġ
    Vector3 m_Pos = Vector3.zero;

    //# ������ ��ġ 
    public static Vector3 m_RDir = Vector3.zero;
    Quaternion m_RFCurRot;
    Vector3 m_RFCurPos = Vector3.forward;


    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;

        //--- Side Wall ����Ʈ �����...
        m_WallLyMask = 1 << LayerMask.NameToLayer("SideWall");
        //"SideWall" ���̾ Lay üũ �ϱ� ���� ����ũ ���� ����

        GameObject[] a_SideWalls = GameObject.FindGameObjectsWithTag("SideWall");
        for(int i = 0; i < a_SideWalls.Length; i++)
        {
            WallCtrl a_WCtrl = a_SideWalls[i].GetComponent<WallCtrl>();
            a_WCtrl.m_IsColl = false;
            a_WCtrl.WallAlphaOnOff(false);  //������ȭ�� ����
            m_SW_List.Add(a_WCtrl);
        }
        //--- Side Wall ����Ʈ �����...

        //## ī�޶� ��ġ 
        m_RotV = m_DefaultRotV;




    }

    void Update()
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;

        if(Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
        if (GameMgr.IsPointerOverUIObject() == false)
        {
                //## ����ǥ�迡 ���� ���� ȸ��
                float a_AddRotSpeed = 235.0f;
                rotSpeed = a_AddRotSpeed;

                m_RotV -= (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse Y"));

                if(m_RotV < m_MinV)
                    m_RotV = m_MinV;
                if (m_MaxV < m_RotV)
                    m_RotV = m_MaxV;

        }

        //# ī�޶� ���� �ܾƿ�
        if(Input.GetAxis("Mouse ScrollWheel") < 0 && dist < m_MaxDist)
        {
            dist += m_ZoomSpeed;
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0 && dist > m_MinDist) 
        { 
            dist -= m_ZoomSpeed; 
        }

     
    }//void Update()

   
    void LateUpdate()
    {
        m_PlayerVec = targetTr.position;
        m_PlayerVec.y += 1.2f;

        //## ����ǥ -> ������ǥ��
        m_BuffRot = Quaternion.Euler(m_RotV, targetTr.eulerAngles.y, 0.0f);
        m_Pos.x = 0.0f;
        m_Pos.y = 0.0f;
        m_Pos.z = -dist;
        m_BuffPos = m_PlayerVec + (m_BuffRot * m_Pos);
        transform.position = Vector3.Lerp(transform.position, m_BuffPos,Time.deltaTime * dampTrace);


        //ī�޶� Ÿ�� ���ӿ�����Ʈ�� �ٶ󺸰� ����
        transform.LookAt(m_PlayerVec);

        //--- Wall ī�޶� ���� �� ���� ó�� �κ�
        a_CacVLen = transform.position - m_PlayerVec;
 
        a_CacDirVec = a_CacVLen.normalized;
        GameObject a_FindObj = null;
        RaycastHit a_HitInfo;
        if(Physics.Raycast(m_PlayerVec + (-a_CacDirVec * 1.0f),
                           a_CacDirVec, out a_HitInfo, a_CacVLen.magnitude + 4.0f,
                           m_WallLyMask.value))
        {
            a_FindObj = a_HitInfo.collider.gameObject;
        }

        for(int i = 0; i < m_SW_List.Count; i++) 
        {
            if (m_SW_List[i].gameObject == a_FindObj)
            {
                if (m_SW_List[i].m_IsColl == false)
                {
                    m_SW_List[i].WallAlphaOnOff(true);  //����ȭ
                    m_SW_List[i].m_IsColl = true;
                }
            }
            else
            {
                if (m_SW_List[i].m_IsColl == true)
                {
                    m_SW_List[i].WallAlphaOnOff(false);  //������ȭ
                    m_SW_List[i].m_IsColl = false;
                }
            }
        }//for(int i = 0; i < m_SW_List.Count; i++) 
         //--- Wall ī�޶� ���� �� ���� ó�� �κ�


        //## �� ������


        if(m_fireCtrl == null)
            m_fireCtrl = targetTr.GetComponent<FireCtrl>();


        Vector3 a_cPos = Vector3.zero;
        if(m_RotV < 6.0f)
        {
            a_cPos = m_fireCtrl.firePos.localPosition;
            a_cPos.y = 1.53f;
            m_fireCtrl.firePos.localPosition = a_cPos;
        }
        else
        {
            a_cPos = m_fireCtrl.firePos.localPosition;
            a_cPos.y = 1.42f;
            m_fireCtrl.firePos.localPosition= a_cPos;   
        }

        m_RFCurRot = Quaternion.Euler(Camera.main.transform.eulerAngles.x - m_MarginRotV,
            targetTr.eulerAngles.y,
            0.0f);

        m_RDir = m_RFCurRot * m_RFCurPos;




    }//void LateUpdate()

  
}
