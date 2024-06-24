using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
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

        // 플레이어와 카메라 사이에 있는 오브젝트를 투명하게 처리
        Vector3 direction = (targetTr.position - transform.position).normalized;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, Vector3.Distance(transform.position, targetTr.position), LayerMask.GetMask("Walls"));

        foreach (RaycastHit hit in hits)
        {
            TransparentObject[] objs = hit.transform.GetComponentsInChildren<TransparentObject>();
            foreach (var obj in objs)
            {
                obj?.BecomeTransparent();
            }
        }


    }
}
