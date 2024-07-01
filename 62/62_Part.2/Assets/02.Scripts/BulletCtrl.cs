using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    //�Ѿ��� �ı���
    public int damage = 20;
    //�Ѿ� �߻� �ӵ�
    public float speed = 1000.0f;

    //## ����ũ ��ƼŬ
    public GameObject SparkEffect;

    // Start is called before the first frame update
    void Start()
    {
        speed = 3000.0f;

        GetComponent<Rigidbody>().AddForce(transform.forward * speed);

        Destroy(gameObject, 4.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //# �浹ó��
    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.name.Contains("Player") == true)
            return;

        if (coll.gameObject.name.Contains("Barrel") == true)
            return;

        if (coll.gameObject.name.Contains("Monster_") == true)
            return;

        if (coll.collider.tag == "SideWall")
            return;

        if(coll.collider.tag == "BULLET")
            return;

        //## ���߿� �߰��� E_BULLET
        if(coll.collider.tag == "E_BULLET")
            return;
    
        //## ����ũ ����
        GameObject Spark = Instantiate(SparkEffect, transform.position , Quaternion.identity);

        //��ƼŬ ���� �� ���� ó��
        Destroy(Spark,Spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

        //�浹�� ��ü ����
        Destroy(gameObject);
    
    }



}
