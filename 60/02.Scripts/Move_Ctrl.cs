using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Move_Ctrl : MonoBehaviour
{
    //## 접근은 클래스 내에서만 접근 -- 접근시 클래스 이름과 enum이름을 붙여 접근.
    public enum MoveType
    {
        WayPoint,
        LookAP,
        DayDream
    }

    public MoveType moveTy = MoveType.LookAP;

    //## 이동속도
    public float speed = 1.0f;

    //## 회전속도
    public float damping = 3.0f;

    //## Transform & CharacterController
    Transform tr;
    Transform CamTr;
    CharacterController cc;




    //## 웨이포인트 배열
    Transform[] points;

    //## 위치 인수
    int nextIdx = 1;


    //## 스태틱
    public static bool isStopped = false;

    

    void Start()
    {
        tr = GetComponent<Transform>();

        //## 웨이포인트 게임오브젝트를 검색
        points = GameObject.Find("WayPoint_group").GetComponentsInChildren<Transform>();
        
        //## 카메라 및 캐릭터 컨트롤러 컴포넌트 할당
        CamTr = Camera.main.GetComponent<Transform>();
        cc = GetComponent<CharacterController>();

    }

    void Update()
    {
        if (isStopped )
        {
            return;
        }
        switch (moveTy)
        {
            case MoveType.WayPoint:
                MoveWayPoint();
                break;
            case MoveType.LookAP:
                MoveLookAt();
                break;
            case MoveType.DayDream:
           
                break;
        }
    }

    void MoveWayPoint()
    {


        // 벡터 계산
        Vector3 direction = points[nextIdx].position - tr.position;
        // 회전  각도 계산
        Quaternion rot = Quaternion.LookRotation(direction);
        // 현재 각도에서 회전해야 할 각도까지 부드럽게 회전 처리 
        tr.rotation = Quaternion.Slerp(tr.rotation, rot, Time.deltaTime * damping);
        // 전진 방향으로 이동 처리 
        tr.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    //## 캐릭터 컨트롤러를 이용한 이동
    void MoveLookAt()
    {
        //## 카메라의 방향으로 이동
        Vector3 dir = CamTr.TransformDirection(Vector3.forward);
        //## 벡터방향으로 이동
        cc.SimpleMove(dir * speed);
    }


    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("WAY_POINT"))
        {
            nextIdx = (++nextIdx >= points.Length) ? 1 : nextIdx;
        }
    }

   








}



