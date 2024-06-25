using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public GameObject[] CharObjs;
    //캐릭터 오브젝트를 저장할 배열 변수

    int CharType = 0;


    //## 벽투명화
    Vector3 a_CacVLen = Vector3.zero;
    Vector3 a_CacVDir = Vector3.zero;

    LayerMask m_WallLyMask = -1;
    List<WallCtrl> m_WCList = new List<WallCtrl>();



    public Transform targetTr;      //추적할 타깃 게임오브젝트의 Transform 변수
    public float dist = 10.0f;      //카메라와의 일정 거리
    public float height = 3.0f;     //카메라의 높이 설정
    public float dampTrace = 20.0f; //부드러운 추적을 위한 변수

    Vector3 m_PlayerVec = Vector3.zero;
    float rotSpeed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;

        //## 리스트 
        m_WallLyMask = 1 << LayerMask.NameToLayer("SideWall");

        //## 레이어만 체크하기 위한 변수 생성
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
                //--- 카메라 위 아래 바라보는 각도 조절을 위한 높낮이 변경 코드
                height -= (rotSpeed * Time.deltaTime * Input.GetAxis("Mouse Y"));

                if (height < 0.1f)
                    height = 0.1f;

                if (5.7f < height)
                    height = 5.7f;
                //--- 카메라 위 아래 바라보는 각도 조절을 위한 높낮이 변경 코드
            }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CharChange();
        }




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


        //## 벽이 카메라를 가릴때
        a_CacVLen = transform.position - m_PlayerVec;

        //## 주인공에서 카메라를 향하는 벡터 생성 -- 캐릭터쪽에서 감지하는것이 빠름.(감도차이)
        a_CacVDir = a_CacVLen.normalized;

        //## 레이캐스트
        GameObject a_FindObj = null;
        RaycastHit a_HitInfo;
        if (Physics.Raycast(m_PlayerVec + (-a_CacVDir * 1.0f),
            a_CacVDir, out a_HitInfo, a_CacVLen.magnitude + 4.0f,
                m_WallLyMask.value))
        {
            a_FindObj = a_HitInfo.collider.gameObject;

        }

        //## 리스트에서 찾은 오브젝트와 비교
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

    //## 캐릭터 변경 함수
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
