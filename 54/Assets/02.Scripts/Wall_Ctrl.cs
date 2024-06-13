using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Ctrl : MonoBehaviour
{
   
    //## ����ũ ��ƼŬ
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
            //����ũ ��ƼŬ ����
           GameObject Spark = Instantiate(sparkEffect, coll.transform.position,Quaternion.identity);

            //����ũ ��ƼŬ duration �ð� �� ����
            Destroy(Spark, Spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

          
            //�浹�� ���ӿ�����Ʈ ����
            Destroy(coll.gameObject);
        }
    
    
    
    }   
     



}
