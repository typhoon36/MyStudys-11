using UnityEngine;

using UnityEngine.EventSystems;

public class LookUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform Cam;

    public Transform Target;
   

    void Start()
    {
        // ī�޶� ��ġ ��������
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
        // ## ������ ȿ��
        transform.LookAt(transform.position + Cam.forward, Cam.up);

        // ȭ�� �� ��ġ ����
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.z = 0.5f; // Z ��ġ ����

        if (pos.x < 0.2f) pos.x = 0.2f;
        
        if (pos.x > 0.8f) pos.x = 0.8f;
        
        if (pos.y < 0.2f) pos.y = 0.2f;
        if (pos.y > 0.8f) pos.y = 0.8f;

       transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
}
