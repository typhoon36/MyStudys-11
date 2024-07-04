using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FollowCam : MonoBehaviour
{
    public GameObject[] CharObjs;   //캐릭터 종류
    

    public Transform targetTr;      //추적할 타깃 게임오브젝트의 Transform 변수

    FireCtrl m_fireCtrl = null;

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

    //# 카메라 위치 제한 
    float m_RotV = 0.0f;
    float m_DefaultRotV = 25.2f;
    float m_MarginRotV = 22.3f;
    
    //## 위 아래 각도 
    float m_MinV = -17.9f;
    float m_MaxV = 52.9f;
    //##  줌인아웃 거리 제한
    float m_MaxDist = 4.0f;
    float m_MinDist = 2.0f;
    //### 줌 속도
    float m_ZoomSpeed = 0.7f;

    //# 카메라 회전
    Quaternion m_BuffRot;
    //회전 위치
    Vector3 m_BuffPos;
    // 위치
    Vector3 m_Pos = Vector3.zero;

    //# 총조준 위치 
    public static Vector3 m_RDir = Vector3.zero;
    Quaternion m_RFCurRot;
    Vector3 m_RFCurPos = Vector3.forward;


    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;

        //--- Side Wall 리스트 만들기...
        m_WallLyMask = 1 << LayerMask.NameToLayer("SideWall");
        //"SideWall" 레이어만 Lay 체크 하기 위한 마스크 변수 생성

        GameObject[] a_SideWalls = GameObject.FindGameObjectsWithTag("SideWall");
        for(int i = 0; i < a_SideWalls.Length; i++)
        {
            WallCtrl a_WCtrl = a_SideWalls[i].GetComponent<WallCtrl>();
            a_WCtrl.m_IsColl = false;
            a_WCtrl.WallAlphaOnOff(false);  //불투명화로 시작
            m_SW_List.Add(a_WCtrl);
        }
        //--- Side Wall 리스트 만들기...

        //## 카메라 위치 
        m_RotV = m_DefaultRotV;




    }

    void Update()
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;

        if(Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
        if (GameMgr.IsPointerOverUIObject() == false)
        {
                //## 구좌표계에 의한 수직 회전
                float a_AddRotSpeed = 235.0f;
                rotSpeed = a_AddRotSpeed;

                m_RotV -= (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse Y"));

                if(m_RotV < m_MinV)
                    m_RotV = m_MinV;
                if (m_MaxV < m_RotV)
                    m_RotV = m_MaxV;

        }

        //# 카메라 줌인 줌아웃
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

        //## 구좌표 -> 직각좌표계
        m_BuffRot = Quaternion.Euler(m_RotV, targetTr.eulerAngles.y, 0.0f);
        m_Pos.x = 0.0f;
        m_Pos.y = 0.0f;
        m_Pos.z = -dist;
        m_BuffPos = m_PlayerVec + (m_BuffRot * m_Pos);
        transform.position = Vector3.Lerp(transform.position, m_BuffPos,Time.deltaTime * dampTrace);


        //카메라가 타깃 게임오브젝트를 바라보게 설정
        transform.LookAt(m_PlayerVec);

        //--- Wall 카메라 가릴 때 투명 처리 부분
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
        }//for(int i = 0; i < m_SW_List.Count; i++) 
         //--- Wall 카메라 가릴 때 투명 처리 부분


        //## 총 방향계산


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
