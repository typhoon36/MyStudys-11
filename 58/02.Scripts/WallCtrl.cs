using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCtrl : MonoBehaviour
{
    //����ũ ��ƼŬ ������ ������ ����
    public GameObject sparkEffect;


    //##���� �޽�������
    MeshRenderer Ms;

    // Start is called before the first frame update
    void Start()
    {
       Ms = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //�浹�� ������ �� �߻��ϴ� �̺�Ʈ
    void OnCollisionEnter(Collision coll)
    {
        //�浹�� ���ӿ�����Ʈ�� �±װ� ��
        if(coll.collider.tag == "BULLET")
        {
            //����ũ ��ƼŬ�� �������� ����
            GameObject spark = Instantiate(sparkEffect, coll.transform.position, Quaternion.identity);

            //ParticleSystem ������Ʈ�� ����ð�(duration)�� ���� �� ���� ó��
            Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

            //�浹�� ���ӿ�����Ʈ ����
            Destroy(coll.gameObject);
        }
    }
}
