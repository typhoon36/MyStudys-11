using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class FollowCam : MonoBehaviour
{
    public GameObject[] CharObjs;   //캐릭터 종류
    int CharType = 0;

    public Transform targetTr;      //추적할 타깃 게임오브젝트의 Transform 변수
    FireCtrl m_FireCtrl = null;     //추적할 타깃이 갖고 있는 FireCtrl 스크립트 참조 변수
    public float dist = 10.0f;      //카메라와의 일정 거리
    public float height = 3.0f;     //카메라의 높이 설정
    public float dampTrace = 20.0f; //부드러운 추적을 위한 변수

    Vector3 m_PlayerVec = Vector3.zero;
    float rotSpeed = 10.0f;

    //--- Wall 투명화 처리를 위한 리스트 관련 변수
    Vector3 a_CacVLen = Vector3.zero;
    Vector3 a_CacDirVec = Vector3.zero;

    LayerMask m_WallLyMask = -1;
    List<WallCtrl> m_SW_List = new List<WallCtrl>();
    //--- Wall 투명화 처리를 위한 리스트 관련 변수

    //--- 카메라 위치 계산용 변수
    float m_RotV = 0.0f;            //마우스 상하 조작값 계산용 변수
    float m_DefaultRotV = 25.2f;    //높이 기준의 회전 각도
    float m_MarginRotV = 22.3f;    //총구와의 마진 각도
    float m_MinLimitV = -17.9f;   //위 아래 각도 제한
    float m_MaxLimitV = 52.9f;    //위 아래 각도 제한
    float m_MaxDist = 4.0f;     //마우스 줌 아웃 최대 거리 제한 값
    float m_MinDist = 2.0f;     //마우스 줌 인 최대 거리 제한 값
    float m_ZoomSpeed = 0.7f;     //마우스 휠 조작에 대한 줌 인 아웃 스피드 설정 값

    Quaternion m_BuffRot;           //카메라 회전 계산용 변수
    Vector3 m_BuffPos;              //카메라 회전에 대한 위치 좌표 계산용 변수
    Vector3 m_BasicPos = Vector3.zero; //위치 계산용 변수
                                       //--- 카메라 위치 계산용 변수

    //--- 총 조준 방향 계산용 변수
    public static Vector3 m_RifleDir = Vector3.zero;    //총 조준 방향
    Quaternion m_RFCacRot;
    Vector3 m_RFCacPos = Vector3.forward;
    //--- 총 조준 방향 계산용 변수

    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;

        //--- Side Wall 리스트 만들기...
        m_WallLyMask = 1 << LayerMask.NameToLayer("SideWall");
        //"SideWall" 레이어만 Lay 체크 하기 위한 마스크 변수 생성

        GameObject[] a_SideWalls = GameObject.FindGameObjectsWithTag("SideWall");
        for (int i = 0; i < a_SideWalls.Length; i++)
        {
            WallCtrl a_WCtrl = a_SideWalls[i].GetComponent<WallCtrl>();
            a_WCtrl.m_IsColl = false;
            a_WCtrl.WallAlphaOnOff(false);  //불투명화로 시작
            m_SW_List.Add(a_WCtrl);
        }
        //--- Side Wall 리스트 만들기...

        //--- 카메라 위치 계산
        m_RotV = m_DefaultRotV;
        //--- 카메라 위치 계산

        if (SceneManager.GetActiveScene().name == "scLevel02")
            m_RotV = 10.2f;
    }

    void Update()
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;

        // 커서가 보이지 않을 때만 마우스 움직임에 따라 카메라가 움직임
        if (Cursor.visible == false && GameMgr.IsPointerOverUIObject() == true)
        {
            //--- (구좌표계를 이용한 수직 회전 처리 코드)
            float a_AddRotSpeed = 235.0f;
            rotSpeed = a_AddRotSpeed;
            m_RotV -= (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse Y"));
            if (m_RotV < m_MinLimitV)
                m_RotV = m_MinLimitV;
            if (m_MaxLimitV < m_RotV)
                m_RotV = m_MaxLimitV;
            //--- (구좌표계를 이용한 수직 회전 처리 코드)
        }
        else 
        {
            float mouseX = Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotSpeed * Time.deltaTime;

            m_RotV -= mouseY;
            m_RotV = Mathf.Clamp(m_RotV, m_MinLimitV, m_MaxLimitV);

            targetTr.Rotate(Vector3.up * mouseX);
          
        }

        //--- 카메라 줌인 줌아웃
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && dist < m_MaxDist)
        {
            dist += m_ZoomSpeed;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && dist > m_MinDist)
        {
            dist -= m_ZoomSpeed;
        }
        //--- 카메라 줌인 줌아웃
    }

    void LateUpdate()
    {
        m_PlayerVec = targetTr.position;
        m_PlayerVec.y += 1.2f;

        //--- (구좌표계를 직각좌표계로 환산해서 카메라의 위치를 잡아주는 코드)
        m_BuffRot = Quaternion.Euler(m_RotV, targetTr.eulerAngles.y, 0.0f);
        m_BasicPos.x = 0.0f;
        m_BasicPos.y = 0.0f;
        m_BasicPos.z = -dist;
        m_BuffPos = m_PlayerVec + (m_BuffRot * m_BasicPos);
        transform.position = Vector3.Lerp(transform.position, m_BuffPos,
                                                  Time.deltaTime * dampTrace);
        //--- (구좌표계를 직각좌표계로 환산해서 카메라의 위치를 잡아주는 코드)

        //카메라가 타깃 게임오브젝트를 바라보게 설정
        transform.LookAt(m_PlayerVec);

        //--- Wall 카메라 가릴 때 투명 처리 부분
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
                    m_SW_List[i].WallAlphaOnOff(true);  //투명화
                    m_SW_List[i].m_IsColl = true;
                }
            }
            else
            {
                if (m_SW_List[i].m_IsColl == true)
                {
                    m_SW_List[i].WallAlphaOnOff(false);  //불투명화
                    m_SW_List[i].m_IsColl = false;
                }
            }
        }

        //--- Rifle 방향 계산
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
        //--- Rifle 방향 계산
    }
}
