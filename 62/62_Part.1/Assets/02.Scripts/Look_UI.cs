using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Look_UI : MonoBehaviour,
    IPointerEnterHandler, //## 포인터 인터페이스들 상속
    IPointerExitHandler
{
    void Start()
    {

    }

    void LateUpdate()
    {
        Vector3 a_Pos = Camera.main.WorldToViewportPoint(transform.position);

        a_Pos.z = 0.5f;

        a_Pos.x = Mathf.Clamp(a_Pos.x, 0.2f, 0.8f);
        a_Pos.y = Mathf.Clamp(a_Pos.y, 0.2f, 0.8f);

        transform.position = Camera.main.ViewportToWorldPoint(a_Pos);

        //## 빌보드 효과
        transform.forward = Camera.main.transform.forward;


    }


    //## 이벤트 함수
    public void OnPointerEnter(PointerEventData eventData)
    {
        MoveCtrl.isStopped = !MoveCtrl.isStopped; 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //기능 없음//
    }
}
