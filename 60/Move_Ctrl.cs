using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Move_Ctrl : MonoBehaviour
{
    //## ������ Ŭ���� �������� ���� -- ���ٽ� Ŭ���� �̸��� enum�̸��� �ٿ� ����.
    public enum MoveType
    {
        WayPoint,
        LookAP,
        DayDream
    }

    public MoveType moveTy = MoveType.LookAP;

    //## �̵��ӵ�
    public float speed = 1.0f;

    //## ȸ���ӵ�
    public float damping = 3.0f;

    //## Transform & CharacterController
    Transform tr;
    Transform CamTr;
    CharacterController cc;




    //## ��������Ʈ �迭
    Transform[] points;

    //## ��ġ �μ�
    int nextIdx = 1;


    //## ����ƽ
    public static bool isStopped = false;

    

    void Start()
    {
        tr = GetComponent<Transform>();

        //## ��������Ʈ ���ӿ�����Ʈ�� �˻�
        points = GameObject.Find("WayPoint_group").GetComponentsInChildren<Transform>();
        
        //## ī�޶� �� ĳ���� ��Ʈ�ѷ� ������Ʈ �Ҵ�
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


        // ���� ���
        Vector3 direction = points[nextIdx].position - tr.position;
        // ȸ��  ���� ���
        Quaternion rot = Quaternion.LookRotation(direction);
        // ���� �������� ȸ���ؾ� �� �������� �ε巴�� ȸ�� ó�� 
        tr.rotation = Quaternion.Slerp(tr.rotation, rot, Time.deltaTime * damping);
        // ���� �������� �̵� ó�� 
        tr.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    //## ĳ���� ��Ʈ�ѷ��� �̿��� �̵�
    void MoveLookAt()
    {
        //## ī�޶��� �������� �̵�
        Vector3 dir = CamTr.TransformDirection(Vector3.forward);
        //## ���͹������� �̵�
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



