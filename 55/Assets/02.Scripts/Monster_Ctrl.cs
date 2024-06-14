using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Ctrl : MonoBehaviour
{
    public enum Mon_State { idle, trace, attack, die };

    //## ������ ���� ���¸� ������ ���� ����

    public Mon_State Curstate = Mon_State.idle;



    //## ���� ������Ʈ�� �Ҵ��� ���� ����
    Transform Mon_Tr;
    Transform Player_Tr;
    NavMeshAgent nvAgent;

    Animator animator;

    
    //## ���� ������ 
    public GameObject bloodEffect;
    //## ���� decal ������
    public GameObject bloodDecal;



    //## ������ ���� �����Ÿ�
    public float traceDist = 10.0f;

    //## ������ ���� �����Ÿ�

    public float attackDist = 1.8f;


    //## �������
    bool isDie = false;



    //## ���� ü��
    public int hp = 100;



    // Start is called before the first frame update
    void Start()
    {

        attackDist = 1.8f;

        //## ���� ��ġ �Ҵ�
        Mon_Tr = gameObject.GetComponent<Transform>();

        //## �÷��̾��� ��ġ�� ã�Ƽ� �Ҵ�
        Player_Tr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        //## �׺���̼� ������Ʈ ������Ʈ �Ҵ�
        nvAgent = gameObject.GetComponent<NavMeshAgent>();


        //## �ִϸ����� ������Ʈ �Ҵ�
        animator = gameObject.GetComponent<Animator>();


        //## ������ �������� �÷��̾�� ����
        // nvAgent.destination = Player_Tr.position;

        //## �ڷ�ƾ�� ����
        StartCoroutine(CheckMon_State());
        StartCoroutine(MonAction());


    }



    //## ������ ���¸� �����ϴ� �Լ�

    IEnumerator CheckMon_State()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(Player_Tr.position, Mon_Tr.position);
            if (dist <= attackDist)
            {
                Curstate = Mon_State.attack;
            }
            else if (dist <= traceDist)
            {
                Curstate = Mon_State.trace;
            }
            else
            {
                Curstate = Mon_State.idle;
            }




        }
    }




    //## ������ ���¿� ���� ó���ϴ� �Լ�

    IEnumerator MonAction()
    {
        while (!isDie)
        {
            switch (Curstate)
            {
                case Mon_State.idle:
                    nvAgent.isStopped = true;
                    animator.SetBool("IsTrace", false);

                    break;

                case Mon_State.trace:
                    nvAgent.destination = Player_Tr.position;
                    nvAgent.isStopped = false;

                    //## ���� ���·� ��ȯ��������(isattack false)
                    animator.SetBool("IsAttack", false);
                    //## ���� ���·� ��ȯ
                    animator.SetBool("IsTrace", true);
                    break;

                case Mon_State.attack:
                    {
                        //## ������ ���߰� ������ ����
                        nvAgent.isStopped = true;
                        //## ���� ���·� ��ȯ
                        animator.SetBool("IsAttack", true);
                        
                        //## ȸ��ó��
                        float a_RotSpeed = 6.0f;
                        
                        Vector3 a_CacDir = Player_Tr.position - transform.position;
                        a_CacDir.y = 0.0f;

                        if(0.0f < a_CacDir.magnitude)
                        {
                           //### �ٶ󺸰� �ϱ�
                           Quaternion a_Rot = Quaternion.LookRotation(a_CacDir);
                            transform.rotation = Quaternion.Slerp(transform.rotation, a_Rot, a_RotSpeed * Time.deltaTime);
                        }

                    }

                    break;



            }
            yield return null;



        }


    }

    //## �浹ó��
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == "Bullet")
        {

            //## ����ȿ�� �Լ� ȣ��
            CreateBloodEffect(coll.transform.position);

            //## �ǰ� ó��
            hp -= coll.gameObject.GetComponent<Bullet_Ctrl>().Damage;
            if (hp <= 0)
                MonsterDie();


            Destroy(coll.gameObject);
            animator.SetTrigger("IsHit");

        }




    }



    void CreateBloodEffect(Vector3 Pos)
    {
        //## ����ȿ�� ����
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, Pos, Quaternion.identity);
        blood1.GetComponent<ParticleSystem>().Play();
        Destroy(blood1, 3.0f);


        //##��Į ��ġ 
       
        Vector3 decalPos = Mon_Tr.position + (Vector3.up * 0.05f);

        //## ��Į ȸ����
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));


        //## ���絥Į������ ����
        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);

        //## ���絥Į�� ũ�Ⱚ�� �������� ����
        float decalscale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * decalscale;

        //## 5�ʵ� ����
        Destroy(blood2, 5.0f);

    }

    
    void OnPlayerDie()
    {
        //## �ڷ�ƾ ����
        StopAllCoroutines();

        //## ���� ����
        nvAgent.isStopped = true;

        //## �÷��̾��� �ִϸ��̼�

        animator.SetTrigger("IsPlayerDie");


    }


    void MonsterDie()
    {
        //## �ڷ�ƾ ����
        StopAllCoroutines();

        isDie = true;
        Curstate = Mon_State.die;
        nvAgent.isStopped = true;
        animator.SetTrigger("IsDie");

        //## ���� �ݸ��� ��Ȱ��ȭ
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        foreach(Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }
        
    }



}