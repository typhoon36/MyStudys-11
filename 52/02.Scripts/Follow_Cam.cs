using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Follow_Cam : MonoBehaviour
{

    //## ī�޶� ���� ���
    public Transform targetTr;

    //## ī�޶���� �Ÿ� �� ����
    public float dist = 10.0f;
    public float height = 3.0f;

    //## ���� 
    public float dampTrace = 20.0f;


    //## �÷��̾� ����
    Vector3 m_PlayerVec = Vector3.zero; 

    //## ���콺 ȸ�� �ӵ�
    public float mouseSpeed = 0.5f; //���콺����

    public float MinY = 1.0f;
    public float MaxY = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        //## ī�޶��� �ʱ� ��ġ ����
        dist = 3.4f;
        height = 2.8f;




    }


    //# ȣ�� ���� �ѹ��� �����ϴ� �̺�Ʈ �Լ�
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
        // ���콺 ��ũ�� �� �Է¿� ���� ī�޶� ���� ����
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        height -= scroll * mouseSpeed;
        height = Mathf.Clamp(height, MinY, MaxY); // dist ���� MinY�� MaxY ������ ���� �ǵ��� ����

        // ī�޶��� ��ġ�� ��������� dist ������ŭ ���� ��ġ �� height ������ŭ ���� ���
        transform.position = Vector3.Lerp(transform.position,
            targetTr.position - (targetTr.forward * dist) + (Vector3.up * height),
            Time.deltaTime * dampTrace);

        transform.LookAt(m_PlayerVec);

    }
}
