using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Ctrl : MonoBehaviour
{
   
    //## 스파크 파티클
    public GameObject sparkEffect;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision coll)
    {
    
        if(coll.gameObject.tag == "Bullet")
        {
            //스파크 파티클 생성
           GameObject Spark = Instantiate(sparkEffect, coll.transform.position,Quaternion.identity);

            //스파크 파티클 duration 시간 후 삭제
            Destroy(Spark, Spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

          
            //충돌한 게임오브젝트 삭제
            Destroy(coll.gameObject);
        }
    
    
    
    }   
     



}
