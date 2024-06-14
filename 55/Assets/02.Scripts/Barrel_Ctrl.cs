using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Effects;

public class Barrel_Ctrl : MonoBehaviour
{
    //## 폭발 파티클
    public GameObject explosionEffect;

    //## 맞은 횟수
    int hitCount = 0;

    //## 타이머
    float timer = 0.0f;

    //## 텍스쳐
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

    //## 충돌 콜백 함수
    private void OnCollisionEnter(Collision coll)
    {
        //## 충돌한 게임오브젝트의 태그가 "Bullet"이면
        if (coll.gameObject.tag == "Bullet")
        {
            Destroy(coll.gameObject);

            //## 맞은 횟수 증가 및 폭발처리
            if (++hitCount >= 3)
                ExplosionBarrel();

        }
    }

    void ExplosionBarrel()
    {

        //## 폭발 파티클 생성
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


        //## 게임오브젝트 삭제
        Destroy(gameObject, 5.0f);



    }

}
