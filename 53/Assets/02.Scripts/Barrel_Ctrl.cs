using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Effects;

public class Barrel_Ctrl : MonoBehaviour
{
    //## ���� ��ƼŬ
    public GameObject explosionEffect;

    //## ���� Ƚ��
    int hitCount = 0;

    //## Ÿ�̸�
    float timer = 0.0f;

    //## �ؽ���
    public Texture[] textures;


    // Start is called before the first frame update
    void Start()
    {
        int idx = Random.Range(0, textures.Length);
        GetComponentInChildren<MeshRenderer>().material.mainTexture =
            textures[idx];
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < timer)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                Rigidbody rd = GetComponent<Rigidbody>();
                if (rd != null)
                    rd.mass = 20.0f;

            }
        }

    }

    //## �浹 �ݹ� �Լ�
    private void OnCollisionEnter(Collision coll)
    {
        //## �浹�� ���ӿ�����Ʈ�� �±װ� "Bullet"�̸�
        if (coll.gameObject.tag == "Bullet")
        {
            Destroy(coll.gameObject);

            //## ���� Ƚ�� ���� �� ����ó��
            if (++hitCount >= 3)
                ExplosionBarrel();

        }
    }

    void ExplosionBarrel()
    {

        //## ���� ��ƼŬ ����
        GameObject Explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(Explosion, Explosion.GetComponentInChildren<ParticleSystem>().main.duration + 2.0f);

        Collider[] colls = Physics.OverlapSphere(transform.position, 10.0f);

        Barrel_Ctrl a_barrel = null;
        Rigidbody rb = null;




        foreach (var coll in colls)
        {

            a_barrel = coll.GetComponent<Barrel_Ctrl>();

            if (a_barrel == null)

                continue;


            rb = coll.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.mass = 1.0f;
                rb.AddExplosionForce(1000.0f, transform.position, 10.0f, 300.0f);
                a_barrel.timer = 0.1f;
            }




        }


        //## ���ӿ�����Ʈ ����
        Destroy(gameObject, 5.0f);



    }

}
