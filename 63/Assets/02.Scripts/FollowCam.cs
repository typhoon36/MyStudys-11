using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FollowCam : MonoBehaviour
{
    public GameObject[] CharObjs;   //캐릭터 종류
    int CharType = 0;

    public Transform targetTr;      //추적할 타깃 게임오브젝트의 Transform 변수
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
    }

    void Update()
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;

        if(Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
        if (GameMgr.IsPointerOverUIObject() == false)
        {
            //--- 카메라 위 아래 바라보는 각도 조절을 위한 높낮이 변경 코드
            height -= (rotSpeed * Time.deltaTime * Input.GetAxis("Mouse Y"));  

            if (height < 0.1f)
                height = 0.1f;

            if (5.7f < height)
                height = 5.7f;
            //--- 카메라 위 아래 바라보는 각도 조절을 위한 높낮이 변경 코드
        }

        //if(Input.GetKeyDown(KeyCode.C))
        //{
        //    CharacterChange();
        //}

    }//void Update()

    //Update 함수 호출 이후 한 번씩 호출되는 함수인 LateUpdate 사용
    //추적할 타깃의 이동이 종료된 이후에 카메라가 추적하기 위해 LateUpdate 사용
    // Update is called once per frame
    void LateUpdate()
    {
        m_PlayerVec = targetTr.position;
        m_PlayerVec.y += 1.2f;

        //카메라의 위치를 추적대상의 dist 변수만큼 뒤쪽으로 배치하고
        //height 변수만큼 위로 올림
        transform.position = Vector3.Lerp(transform.position,
                                            targetTr.position
                                            - (targetTr.forward * dist)
                                            + (Vector3.up * height),
                                            Time.deltaTime * dampTrace);

        //카메라가 타깃 게임오브젝트를 바라보게 설정
        transform.LookAt(m_PlayerVec);

        //--- Wall 카메라 가릴 때 투명 처리 부분
        a_CacVLen = transform.position - m_PlayerVec;
        //주인공에서 카메라를 향하는 방향 벡터 
        //레이저 광선이 벽의 안쪽이 먼저 닿는지 체크하는 게 더 감도가 좋아서...
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

    }//void LateUpdate()

    void CharacterChange()
    {
        Vector3 a_Pos = CharObjs[CharType].transform.position;
        Quaternion a_Rot = CharObjs[CharType].transform.rotation;
        CharObjs[CharType].SetActive(false);
        CharType++;
        if (1 < CharType)
            CharType = 0;
        CharObjs[CharType].SetActive(true);
        CharObjs[CharType].transform.position = a_Pos;
        CharObjs[CharType].transform.rotation = a_Rot;
        targetTr = CharObjs[CharType].transform;
    }
}
