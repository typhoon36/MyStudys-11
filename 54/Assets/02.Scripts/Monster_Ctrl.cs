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



    //## ������ ���� �����Ÿ�
    public float traceDist = 10.0f;

    //## ������ ���� �����Ÿ�

    public float attackDist = 2.0f;


    //## �������
    bool isDie = false;

    // Start is called before the first frame update
    void Start()
    {
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

    // Update is called once per frame
    //void Update()
    //{
    //    // ���Ͱ� �÷��̾ ���� ȸ��
    //    if (Curstate == Mon_State.attack)
    //    {
    //        Vector3 dir = Player_Tr.position - Mon_Tr.position;
    //        Quaternion rotation = Quaternion.LookRotation(dir);
    //        Mon_Tr.rotation = Quaternion.Lerp(Mon_Tr.rotation, rotation, Time.deltaTime * 5.0f);
    //    }
    //}

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
                    //## ������ ���߰� ������ ����
                    nvAgent.isStopped = true;
                    //## ���� ���·� ��ȯ
                    animator.SetBool("IsAttack", true);

                    Vector3 dir = Player_Tr.position - Mon_Tr.position;
                    Quaternion rotation = Quaternion.LookRotation(dir);
                    Mon_Tr.rotation = Quaternion.Lerp(Mon_Tr.rotation, rotation, Time.deltaTime * 5.0f);


                    break;



            }
            yield return null;



        }


    }


}