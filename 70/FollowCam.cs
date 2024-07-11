using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class FollowCam : MonoBehaviour
{
    public GameObject[] CharObjs;   //ĳ���� ����
    int CharType = 0;

    public Transform targetTr;      //������ Ÿ�� ���ӿ�����Ʈ�� Transform ����
    FireCtrl m_FireCtrl = null;     //������ Ÿ���� ���� �ִ� FireCtrl ��ũ��Ʈ ���� ����
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

    //--- ī�޶� ��ġ ���� ����
    float m_RotV = 0.0f;            //���콺 ���� ���۰� ���� ����
    float m_DefaultRotV = 25.2f;    //���� ������ ȸ�� ����
    float m_MarginRotV = 22.3f;    //�ѱ����� ���� ����
    float m_MinLimitV = -17.9f;   //�� �Ʒ� ���� ����
    float m_MaxLimitV = 52.9f;    //�� �Ʒ� ���� ����
    float m_MaxDist = 4.0f;     //���콺 �� �ƿ� �ִ� �Ÿ� ���� ��
    float m_MinDist = 2.0f;     //���콺 �� �� �ִ� �Ÿ� ���� ��
    float m_ZoomSpeed = 0.7f;     //���콺 �� ���ۿ� ���� �� �� �ƿ� ���ǵ� ���� ��

    Quaternion m_BuffRot;           //ī�޶� ȸ�� ���� ����
    Vector3 m_BuffPos;              //ī�޶� ȸ���� ���� ��ġ ��ǥ ���� ����
    Vector3 m_BasicPos = Vector3.zero; //��ġ ���� ����
                                       //--- ī�޶� ��ġ ���� ����

    //--- �� ���� ���� ���� ����
    public static Vector3 m_RifleDir = Vector3.zero;    //�� ���� ����
    Quaternion m_RFCacRot;
    Vector3 m_RFCacPos = Vector3.forward;
    //--- �� ���� ���� ���� ����

    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;

        //--- Side Wall ����Ʈ �����...
        m_WallLyMask = 1 << LayerMask.NameToLayer("SideWall");
        //"SideWall" ���̾ Lay üũ �ϱ� ���� ����ũ ���� ����

        GameObject[] a_SideWalls = GameObject.FindGameObjectsWithTag("SideWall");
        for (int i = 0; i < a_SideWalls.Length; i++)
        {
            WallCtrl a_WCtrl = a_SideWalls[i].GetComponent<WallCtrl>();
            a_WCtrl.m_IsColl = false;
            a_WCtrl.WallAlphaOnOff(false);  //������ȭ�� ����
            m_SW_List.Add(a_WCtrl);
        }
        //--- Side Wall ����Ʈ �����...

        //--- ī�޶� ��ġ ���
        m_RotV = m_DefaultRotV;
        //--- ī�޶� ��ġ ���

        if (SceneManager.GetActiveScene().name == "scLevel02")
            m_RotV = 10.2f;
    }

    void Update()
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;

        // Ŀ���� ������ ���� ���� ���콺 �����ӿ� ���� ī�޶� ������
        if (Cursor.visible == false && GameMgr.IsPointerOverUIObject() == true)
        {
            //--- (����ǥ�踦 �̿��� ���� ȸ�� ó�� �ڵ�)
            float a_AddRotSpeed = 235.0f;
            rotSpeed = a_AddRotSpeed;
            m_RotV -= (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse Y"));
            if (m_RotV < m_MinLimitV)
                m_RotV = m_MinLimitV;
            if (m_MaxLimitV < m_RotV)
                m_RotV = m_MaxLimitV;
            //--- (����ǥ�踦 �̿��� ���� ȸ�� ó�� �ڵ�)
        }
        else 
        {
            float mouseX = Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotSpeed * Time.deltaTime;

            m_RotV -= mouseY;
            m_RotV = Mathf.Clamp(m_RotV, m_MinLimitV, m_MaxLimitV);

            targetTr.Rotate(Vector3.up * mouseX);
          
        }

        //--- ī�޶� ���� �ܾƿ�
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && dist < m_MaxDist)
        {
            dist += m_ZoomSpeed;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && dist > m_MinDist)
        {
            dist -= m_ZoomSpeed;
        }
        //--- ī�޶� ���� �ܾƿ�
    }

    void LateUpdate()
    {
        m_PlayerVec = targetTr.position;
        m_PlayerVec.y += 1.2f;

        //--- (����ǥ�踦 ������ǥ��� ȯ���ؼ� ī�޶��� ��ġ�� ����ִ� �ڵ�)
        m_BuffRot = Quaternion.Euler(m_RotV, targetTr.eulerAngles.y, 0.0f);
        m_BasicPos.x = 0.0f;
        m_BasicPos.y = 0.0f;
        m_BasicPos.z = -dist;
        m_BuffPos = m_PlayerVec + (m_BuffRot * m_BasicPos);
        transform.position = Vector3.Lerp(transform.position, m_BuffPos,
                                                  Time.deltaTime * dampTrace);
        //--- (����ǥ�踦 ������ǥ��� ȯ���ؼ� ī�޶��� ��ġ�� ����ִ� �ڵ�)

        //ī�޶� Ÿ�� ���ӿ�����Ʈ�� �ٶ󺸰� ����
        transform.LookAt(m_PlayerVec);

        //--- Wall ī�޶� ���� �� ���� ó�� �κ�
        a_CacVLen = transform.position - m_PlayerVec;
        a_CacDirVec = a_CacVLen.normalized;
        GameObject a_FindObj = null;
        RaycastHit a_HitInfo;
        if (Physics.Raycast(m_PlayerVec + (-a_CacDirVec * 1.0f),
                           a_CacDirVec, out a_HitInfo, a_CacVLen.magnitude + 4.0f,
                           m_WallLyMask.value))
        {
            a_FindObj = a_HitInfo.collider.gameObject;
        }

        for (int i = 0; i < m_SW_List.Count; i++)
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
        }

        //--- Rifle ���� ���
        if (m_FireCtrl == null)
            m_FireCtrl = targetTr.GetComponent<FireCtrl>();

        Vector3 a_cPos = Vector3.zero;
        if (m_RotV < 6.0f)
        {
            a_cPos = m_FireCtrl.firePos.localPosition;
            a_cPos.y = 1.53f;
            m_FireCtrl.firePos.localPosition = a_cPos;
        }
        else
        {
            a_cPos = m_FireCtrl.firePos.localPosition;
            a_cPos.y = 1.42f;
            m_FireCtrl.firePos.localPosition = a_cPos;
        }

        m_RFCacRot = Quaternion.Euler(
            Camera.main.transform.eulerAngles.x - m_MarginRotV,
            targetTr.eulerAngles.y,
            0.0f);

        m_RifleDir = m_RFCacRot * m_RFCacPos;
        //--- Rifle ���� ���
    }
}
