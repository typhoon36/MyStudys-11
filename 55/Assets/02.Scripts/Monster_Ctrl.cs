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

    
    //## 혈흔 프리팹 
    public GameObject bloodEffect;
    //## 혈흔 decal 프리팹
    public GameObject bloodDecal;



    //## 몬스터의 추적 사정거리
    public float traceDist = 10.0f;

    //## 몬스터의 공격 사정거리

    public float attackDist = 1.8f;


    //## 사망여부
    bool isDie = false;



    //## 몬스터 체력
    public int hp = 100;



    // Start is called before the first frame update
    void Start()
    {

        attackDist = 1.8f;

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
                    {
                        //## 추적을 멈추고 공격을 시작
                        nvAgent.isStopped = true;
                        //## 공격 상태로 전환
                        animator.SetBool("IsAttack", true);
                        
                        //## 회전처리
                        float a_RotSpeed = 6.0f;
                        
                        Vector3 a_CacDir = Player_Tr.position - transform.position;
                        a_CacDir.y = 0.0f;

                        if(0.0f < a_CacDir.magnitude)
                        {
                           //### 바라보게 하기
                           Quaternion a_Rot = Quaternion.LookRotation(a_CacDir);
                            transform.rotation = Quaternion.Slerp(transform.rotation, a_Rot, a_RotSpeed * Time.deltaTime);
                        }

                    }

                    break;



            }
            yield return null;



        }


    }

    //## 충돌처리
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == "Bullet")
        {

            //## 혈흔효과 함수 호출
            CreateBloodEffect(coll.transform.position);

            //## 피격 처리
            hp -= coll.gameObject.GetComponent<Bullet_Ctrl>().Damage;
            if (hp <= 0)
                MonsterDie();


            Destroy(coll.gameObject);
            animator.SetTrigger("IsHit");

        }




    }



    void CreateBloodEffect(Vector3 Pos)
    {
        //## 혈흔효과 생성
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, Pos, Quaternion.identity);
        blood1.GetComponent<ParticleSystem>().Play();
        Destroy(blood1, 3.0f);


        //##데칼 위치 
       
        Vector3 decalPos = Mon_Tr.position + (Vector3.up * 0.05f);

        //## 데칼 회전값
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));


        //## 혈흔데칼프리팹 생성
        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);

        //## 혈흔데칼의 크기값을 랜덤으로 설정
        float decalscale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * decalscale;

        //## 5초뒤 삭제
        Destroy(blood2, 5.0f);

    }

    
    void OnPlayerDie()
    {
        //## 코루틴 정지
        StopAllCoroutines();

        //## 정지 상태
        nvAgent.isStopped = true;

        //## 플레이어사망 애니메이션

        animator.SetTrigger("IsPlayerDie");


    }


    void MonsterDie()
    {
        //## 코루틴 정지
        StopAllCoroutines();

        isDie = true;
        Curstate = Mon_State.die;
        nvAgent.isStopped = true;
        animator.SetTrigger("IsDie");

        //## 몬스터 콜리더 비활성화
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        foreach(Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }
        
    }



}