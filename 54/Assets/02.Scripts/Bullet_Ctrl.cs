using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ctrl : MonoBehaviour
{
    //## ÃÑ¾Ë µ¥¹ÌÁö
    public int Damage = 20;

    //## ÃÑ¾Ë ¼Óµµ
    public float speed = 1000.0f;



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
}
