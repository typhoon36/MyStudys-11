using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Follow_Cam : MonoBehaviour
{

    //## 카메라가 따라갈 대상
    public Transform targetTr;

    //## 카메라와의 거리 및 높이
    public float dist = 10.0f;
    public float height = 3.0f;

    //## 추적 
    public float dampTrace = 20.0f;


    //## 플레이어 벡터
    Vector3 m_PlayerVec = Vector3.zero; 

    //## 마우스 회전 속도
    public float mouseSpeed = 0.5f; //마우스감도

    public float MinY = 1.0f;
    public float MaxY = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        //## 카메라의 초기 위치 설정
        dist = 3.4f;
        height = 2.8f;




    }


    //# 호출 이후 한번만 실행하는 이벤트 함수
    void LateUpdate()
    {
        m_PlayerVec= targetTr.position;

        m_PlayerVec.y += 1.2f;


        if(Input.GetMouseButton(0))
        {
            float rotY = Input.GetAxis("Mouse Y");

            height -= rotY * mouseSpeed;
            height = Mathf.Clamp(height, MinY, MaxY);
        }
        // 마우스 스크롤 휠 입력에 따른 카메라 높이 조절
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        height -= scroll * mouseSpeed;
        height = Mathf.Clamp(height, MinY, MaxY); // dist 값이 MinY와 MaxY 사이의 값이 되도록 제한

        // 카메라의 위치를 추적대상의 dist 변수만큼 뒤쪽 배치 후 height 변수만큼 위로 상승
        transform.position = Vector3.Lerp(transform.position,
            targetTr.position - (targetTr.forward * dist) + (Vector3.up * height),
            Time.deltaTime * dampTrace);

        transform.LookAt(m_PlayerVec);

    }
}
