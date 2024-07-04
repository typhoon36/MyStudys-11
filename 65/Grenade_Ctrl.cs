using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade_Ctrl : MonoBehaviour
{
    //# ����
    public GameObject explosionEffect;

    //## ���� �ð�
    float Timer = 2.0f;

    //## �ؽ��Ĺ迭
    public Texture[] textures;

    //## �ӵ�
    float speed = 500.0f;
    Vector3 m_FDir = Vector3.zero;

    //## ȸ�� ����
    bool is_Rot = true;

    // Start is called before the first frame update
    void Start()
    {
        int Idx = Random.Range(0, textures.Length);
        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[Idx];

        //## ���� ������
        transform.forward = m_FDir;
        transform.eulerAngles = new Vector3(20.0f, transform.eulerAngles.y, transform.eulerAngles.z);

        //## ������ٵ�� �����ֱ�
        GetComponent<Rigidbody>().AddForce(m_FDir * speed);

    }

    // Update is called once per frame
    void Update()
    {
        //## �ð�
        if (0.0f < Timer)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0.0f)
            {
                //## ���� ����Ʈ ����
                ExpGrenade();
            }
        }

        //## �����Ͻ� ȸ��
        if (is_Rot == true)
        {
            transform.Rotate(new Vector3(Time.deltaTime * 190.0f, 0, 0), Space.Self);
        }

    }

    //# �浹ó��
    private void OnCollisionEnter(Collision collision)
    {
        is_Rot = false;
    }

    //# ����
    void ExpGrenade()
    {
        //## ���� ��ƼŬ
        GameObject exp = Instantiate(explosionEffect, transform.position, Quaternion.identity);

        //## ���� ����Ʈ ����
        Destroy(exp, exp.GetComponentInChildren<ParticleSystem>().main.duration + 2.0f);

        //## �ݸ��� ����
        Collider[] colls = Physics.OverlapSphere(transform.position, 10.0f);

        //## ���� ����
        MonsterCtrl a_MonCtrl = null;
        foreach (var coll in colls)
        {
            a_MonCtrl = coll.GetComponent<MonsterCtrl>();
            if (a_MonCtrl == null)
                continue;
            a_MonCtrl.TakeDamage(150);
            
        }

        //## �ڽ� ����
        Destroy(gameObject);

    }

    //# �ʱ�ȭ
    public void SetFowardDir(Vector3 a_Dir)
    {
       m_FDir = new Vector3(a_Dir.x, a_Dir.y + 0.5f, a_Dir.z);
    }


}
