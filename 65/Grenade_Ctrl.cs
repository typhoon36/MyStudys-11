using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade_Ctrl : MonoBehaviour
{
    //# 폭발
    public GameObject explosionEffect;

    //## 지연 시간
    float Timer = 2.0f;

    //## 텍스쳐배열
    public Texture[] textures;

    //## 속도
    float speed = 500.0f;
    Vector3 m_FDir = Vector3.zero;

    //## 회전 여부
    bool is_Rot = true;

    // Start is called before the first frame update
    void Start()
    {
        int Idx = Random.Range(0, textures.Length);
        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[Idx];

        //## 방향 재조정
        transform.forward = m_FDir;
        transform.eulerAngles = new Vector3(20.0f, transform.eulerAngles.y, transform.eulerAngles.z);

        //## 리지드바디로 날려주기
        GetComponent<Rigidbody>().AddForce(m_FDir * speed);

    }

    // Update is called once per frame
    void Update()
    {
        //## 시간
        if (0.0f < Timer)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0.0f)
            {
                //## 폭발 이펙트 생성
                ExpGrenade();
            }
        }

        //## 연출일시 회전
        if (is_Rot == true)
        {
            transform.Rotate(new Vector3(Time.deltaTime * 190.0f, 0, 0), Space.Self);
        }

    }

    //# 충돌처리
    private void OnCollisionEnter(Collision collision)
    {
        is_Rot = false;
    }

    //# 폭발
    void ExpGrenade()
    {
        //## 폭발 파티클
        GameObject exp = Instantiate(explosionEffect, transform.position, Quaternion.identity);

        //## 폭발 이펙트 삭제
        Destroy(exp, exp.GetComponentInChildren<ParticleSystem>().main.duration + 2.0f);

        //## 콜리더 추출
        Collider[] colls = Physics.OverlapSphere(transform.position, 10.0f);

        //## 폭발 전달
        MonsterCtrl a_MonCtrl = null;
        foreach (var coll in colls)
        {
            a_MonCtrl = coll.GetComponent<MonsterCtrl>();
            if (a_MonCtrl == null)
                continue;
            a_MonCtrl.TakeDamage(150);
            
        }

        //## 자신 삭제
        Destroy(gameObject);

    }

    //# 초기화
    public void SetFowardDir(Vector3 a_Dir)
    {
       m_FDir = new Vector3(a_Dir.x, a_Dir.y + 0.5f, a_Dir.z);
    }


}
