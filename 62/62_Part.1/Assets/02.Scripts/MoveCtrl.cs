using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCtrl : MonoBehaviour
{
    public enum MoveType
    {
        WAY_POINT,
        LOOK_AT,
        DAYDREAM
    }
    //�̵� ���
    public MoveType moveType = MoveType.WAY_POINT;

    //�̵� �ӵ�
    public float speed = 1.0f;
    //ȸ�� �� ȸ�� �ӵ��� ������ ���
    public float damping = 3.0f;

    private Transform camTr;
    private CharacterController cc;

    //��������Ʈ�� ������ �迭
    private Transform[] points;
    //������ �̵��ؾ� �� ��ġ �ε��� ����
    private int nextIdx = 1;

    public static bool isStopped = false;

    // Start is called before the first frame update
    void Start()
    {
        camTr = Camera.main.GetComponent<Transform>();
        cc = GetComponent<CharacterController>();

        //WayPointGroup ���ӿ�����Ʈ �Ʒ��� �ִ� ��� Point�� Transform ������Ʈ�� ����
        points = GameObject.Find("WayPointGroup").GetComponentsInChildren<Transform>();
    }

    bool m_IsStop = false;
    // Update is called once per frame
    void Update()
    {
        if(isStopped == true)
            return;

        switch(moveType)
        {
            case MoveType.WAY_POINT:
                MoveWayPoint();
                break;

            case MoveType.LOOK_AT:
                MoveLookAt();
                break;

            case MoveType.DAYDREAM:
                break;
        }

    }//void Update()

    void MoveWayPoint()
    {
        //���� ��ġ���� ���� ��������Ʈ�� �ٶ󺸴� ���͸� ���
        Vector3 direction = points[nextIdx].position - transform.position;
        //����� ������ ȸ�� ������ ���ʹϾ� Ÿ������ ����
        Quaternion rot = Quaternion.LookRotation(direction);
        //���� �������� ȸ���ؾ� �� �������� �ε巴�� ȸ�� ó��
        transform.rotation = Quaternion.Slerp(transform.rotation, rot,
                                                Time.deltaTime * damping);

        //���� �������� �̵� ó��
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    void MoveLookAt()
    {
        //����ī�޶� �ٶ󺸴� ����
        Vector3 dir = camTr.TransformDirection(Vector3.forward); //���� --> ���� ��ǥ���...
        //Vector3 dir = camTr.forward;
        //dir ������ �߾����� �ʴ� speed ��ŭ�� �̵�
        cc.SimpleMove(dir * speed);
    }

    void OnTriggerEnter(Collider coll)
    {
        //��������Ʈ(Point ���ӿ�����Ʈ)�� �浹 ���� �Ǵ�
        if(coll.CompareTag("WAY_POINT"))
        {
            //�� ������ ��������Ʈ�� �������� �� ó�� �ε����� ����
            nextIdx = (++nextIdx >= points.Length) ? 1 : nextIdx;
        }
    }//void OnTriggerEnter(Collider coll)
}
