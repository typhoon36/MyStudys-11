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
            //--- ī�޶� �� �Ʒ� �ٶ󺸴� ���� ������ ���� ������ ���� �ڵ�
            height -= (rotSpeed * Time.deltaTime * Input.GetAxis("Mouse Y"));  

            if (height < 0.1f)
                height = 0.1f;

            if (5.7f < height)
                height = 5.7f;
            //--- ī�޶� �� �Ʒ� �ٶ󺸴� ���� ������ ���� ������ ���� �ڵ�
        }

    }//void Update()

    //Update �Լ� ȣ�� ���� �� ���� ȣ��Ǵ� �Լ��� LateUpdate ���
    //������ Ÿ���� �̵��� ����� ���Ŀ� ī�޶� �����ϱ� ���� LateUpdate ���
    // Update is called once per frame
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

        // �÷��̾�� ī�޶� ���̿� �ִ� ������Ʈ�� �����ϰ� ó��
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
