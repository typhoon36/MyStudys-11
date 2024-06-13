using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;      //������ Ÿ�� ���ӿ�����Ʈ�� Transform ����
    public float dist = 10.0f;      //ī�޶���� ���� �Ÿ�
    public float height = 3.0f;     //ī�޶��� ���� ����
    public float dampTrace = 20.0f; //�ε巯�� ������ ���� ����

    Vector3 m_PlayerVec = Vector3.zero;

    //## ���콺 �ӵ�
    float rotSpeed = 10.0f;



    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) == true || Input.GetMouseButton(1)==true)

        {
            //## ���Ʒ� ���� ������ ���� ������ ����
            height -= (rotSpeed * Time.deltaTime * Input.GetAxis("Mouse Y"));

            if (height < 0.1f)
                height = 0.1f;

            if (5.7f < height)
                height = 5.7f;

        }



    }



    void LateUpdate()
    {
        m_PlayerVec = targetTr.position;
        m_PlayerVec.y += 1.2f;

        //ī�޶��� ��ġ�� ��������� dist ������ŭ �������� ��ġ�ϰ�
        //height ������ŭ ���� �ø�
        transform.position = Vector3.Lerp(transform.position,
                                            targetTr.position
                                            - (targetTr.forward * dist)
                                            + (Vector3.up * height),
                                            Time.deltaTime * dampTrace);

        //ī�޶� Ÿ�� ���ӿ�����Ʈ�� �ٶ󺸰� ����
        transform.LookAt(m_PlayerVec);
    }
}
