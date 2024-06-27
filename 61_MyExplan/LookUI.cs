using UnityEngine;

using UnityEngine.EventSystems;

public class LookUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform Cam;

    public Transform Target;
   

    void Start()
    {
        // 카메라 위치 가져오기
        Cam = Camera.main.transform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        Move_Ctrl.isStopped = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        Move_Ctrl.isStopped = false;
    }
    
    void LateUpdate()
    {
        // ## 빌보드 효과
        transform.LookAt(transform.position + Cam.forward, Cam.up);

        // 화면 내 위치 제한
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.z = 0.5f; // Z 위치 조정

        if (pos.x < 0.2f) pos.x = 0.2f;
        
        if (pos.x > 0.8f) pos.x = 0.8f;
        
        if (pos.y < 0.2f) pos.y = 0.2f;
        if (pos.y > 0.8f) pos.y = 0.8f;

       transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
}
