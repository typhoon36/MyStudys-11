using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    //총알의 파괴력
    public int damage = 20;
    //총알 발사 속도
    public float speed = 1000.0f;

    //## 스파크 파티클
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

    //# 충돌처리
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

        //## 나중에 추가할 E_BULLET
        if(coll.collider.tag == "E_BULLET")
            return;
    
        //## 스파크 생성
        GameObject Spark = Instantiate(SparkEffect, transform.position , Quaternion.identity);

        //파티클 수행 후 삭제 처리
        Destroy(Spark,Spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

        //충돌시 객체 삭제
        Destroy(gameObject);
    
    }



}
