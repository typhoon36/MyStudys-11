using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Ctrl : MonoBehaviour
{
    public enum Mon_State { idle, trace, attack, die };

    //## 몬스터의 현재 상태를 저장할 변수 선언

    public Mon_State Curstate = Mon_State.idle;



    //## 각종 컴포넌트를 할당할 변수 선언
    Transform Mon_Tr;
    Transform Player_Tr;
    NavMeshAgent nvAgent;

    Animator animator;



    //## 몬스터의 추적 사정거리
    public float traceDist = 10.0f;

    //## 몬스터의 공격 사정거리

    public float attackDist = 2.0f;


    //## 사망여부
    bool isDie = false;

    // Start is called before the first frame update
    void Start()
    {
        //## 몬스터 위치 할당
        Mon_Tr = gameObject.GetComponent<Transform>();

        //## 플레이어의 위치를 찾아서 할당
        Player_Tr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        //## 네비게이션 에이전트 컴포넌트 할당
        nvAgent = gameObject.GetComponent<NavMeshAgent>();


        //## 애니메이터 컴포넌트 할당
        animator = gameObject.GetComponent<Animator>();


        //## 몬스터의 목적지를 플레이어로 설정
        // nvAgent.destination = Player_Tr.position;

        //## 코루틴들 시작
        StartCoroutine(CheckMon_State());
        StartCoroutine(MonAction());


    }

    // Update is called once per frame
    //void Update()
    //{
    //    // 몬스터가 플레이어를 향해 회전
    //    if (Curstate == Mon_State.attack)
    //    {
    //        Vector3 dir = Player_Tr.position - Mon_Tr.position;
    //        Quaternion rotation = Quaternion.LookRotation(dir);
    //        Mon_Tr.rotation = Quaternion.Lerp(Mon_Tr.rotation, rotation, Time.deltaTime * 5.0f);
    //    }
    //}

    //## 몬스터의 상태를 변경하는 함수

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





    //## 몬스터의 상태에 따라서 처리하는 함수

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

                    //## 공격 상태로 전환하지않음(isattack false)
                    animator.SetBool("IsAttack", false);
                    //## 추적 상태로 전환
                    animator.SetBool("IsTrace", true);
                    break;

                case Mon_State.attack:
                    //## 추적을 멈추고 공격을 시작
                    nvAgent.isStopped = true;
                    //## 공격 상태로 전환
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