using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ctrl : MonoBehaviour
{
    //## �Ѿ� ������
    public int Damage = 20;

    //## �Ѿ� �ӵ�
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
