using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    //몬스터의 상태 정보가 있는 Enumerable 변수 선언
    public enum MonsterState { idle, trace, attack, die };

    //몬스터의 현재 상태 정보를 저장할 Enum 변수
    public MonsterState monsterState = MonsterState.idle;

    //속도 향상을 위해 각종 컴포넌트를 변수에 할당
    private Transform monsterTr;
    private Transform playerTr;
    private Animator animator;

    //추적 사정거리
    public float traceDist = 10.0f;
    //공격 사정거리
    public float attackDist = 1.5f; //2.0f;

    //몬스터의 사망 여부
    private bool isDie = false;

    //혈흔 효과 프리팹
    public GameObject bloodEffect;
    //혈흔 데칼 효과 프리팹
    public GameObject bloodDecal;

    //몬스터 생명 변수
    private int hp = 100;

    //리지드바디
    Rigidbody m_riBody = null;


    void Awake()
    {
        traceDist = 10.0f;
        attackDist = 1.5f;

        //몬스터의 Transform 할당
        monsterTr = this.gameObject.GetComponent<Transform>();
        //추적 대상인 Player의 Transform 할당
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();




        //Animator 컴포넌트 할당
        animator = this.gameObject.GetComponent<Animator>();
        
        m_riBody = GetComponent<Rigidbody>();
    }

    //이벤트 발생시 수행할 함수 연결 -- 일반함수로 변경





    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate() //## 물리 이동
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;

        if (playerTr.gameObject.activeSelf == false)
            playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        CheckMonState();
        MonAction();

        //## 활동중 중력값 아래로 주기
        if (isDie == false)
        {
            m_riBody.AddForce(Vector3.down * 100.0f);
        }

    }

    #region # 몬스터 AI (대체된 일반함수)
    //## 일정 간격 몬스터 행동상태 체크 및 몬스터상태 변경
    float m_AI_delay = 0;

    void CheckMonState()
    {
        if (isDie == true) return;

        //## 0.1초 주기로 딜레이
        m_AI_delay -= Time.deltaTime;
        if (0.0f < m_AI_delay)
            return;


        m_AI_delay = 0.1f;

        //## 몬스터와 플레이어 사이 측정
        float dist = Vector3.Distance(playerTr.position, monsterTr.position);
        float Ydist = Mathf.Abs(playerTr.position.y - monsterTr.position.y);
        //2층 범위시 1층 추적 체크

        // 공격범위 시
        if (dist <= attackDist)
        {
            monsterState = MonsterState.attack;
        }

        //추적거리 시
        else if (dist <= traceDist && Ydist < 5.0f)
        {
            monsterState = MonsterState.trace;
        }

        //둘다 아니면 
        else
        {
            monsterState = MonsterState.idle;
        }

    }

    //## 몬스터 상태에 따라 동작
    void MonAction()
    {
        if (isDie == true) return;

        switch (monsterState)
        {

            case MonsterState.idle:
                //## 대기상태에서 Animator 변수 변경
                animator.SetBool("IsTrace", false);
                break;

            case MonsterState.trace:
                {
                    //## 추적 이동
                    float a_MoveVel = 2.0f;
                    Vector3 a_Dir = playerTr.position - transform.position;
                    a_Dir.y = 0.0f;

                    if (0.0f < a_Dir.magnitude)
                    {
                        Vector3 a_Step = a_Dir.normalized * a_MoveVel * Time.deltaTime;
                        transform.Translate(a_Step, Space.World);

                        //## 이동시 그 방향을 바라보도록
                        float a_RotSpeed = 7.0f;
                        Quaternion a_Target = Quaternion.LookRotation(a_Dir.normalized);

                        transform.rotation = Quaternion.Slerp(transform.rotation, a_Target, a_RotSpeed * Time.deltaTime);

                    }

                    //## 애니메이터 변수 설정
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);

                }
                break;

            case MonsterState.attack:
                {
                    //## 공격 애니메이션
                    animator.SetBool("IsAttack", true);

                    //## 공격시 바라보도록
                    float a_RotSpd = 6.0f;

                    Vector3 a_CacDir = playerTr.position - transform.position;

                    a_CacDir.y = 0.0f;

                    if (0.0f < a_CacDir.magnitude)
                    {
                        //## 딜레이로 인해 공격 거리에서 멀어진 경우의 보정
                        //-- 무슨 의미? 주인공에게 콜리더가 안닿게 됨.
                        if (attackDist < a_CacDir.magnitude)
                        {
                            float a_Vel = 2.0f;
                            Vector3 a_Step = a_CacDir.normalized * a_Vel * Time.deltaTime;
                            transform.Translate(a_Step, Space.World);
                        }

                        Quaternion a_Target = Quaternion.LookRotation(a_CacDir.normalized);
                        transform.rotation = Quaternion.Slerp(transform.rotation, a_Target, Time.deltaTime * a_RotSpd);
                    }
                }
                break;

        }

    }





    #endregion








    //##충돌 처리
    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "BULLET")
        {
            //혈흔 효과 함수 호출
            CreateBloodEffect(coll.transform.position);

            //맞은 총알의 Damage를 추출해 몬스터 hp 차감
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
            if (hp <= 0)
            {
                MonsterDie();
            }

            //Bullet 삭제
            Destroy(coll.gameObject);

            //IsHit Trigger를 발생시키면 Any State에서 gothit로 전이됨
            animator.SetTrigger("IsHit");
        }
    }

    //몬스터 사망시 처리 루틴
    void MonsterDie()
    {
        //사망한 몬스터의 태그를 Untagged로 변경
        gameObject.tag = "Untagged";

        //모든 코루틴을 정지
        StopAllCoroutines();

        isDie = true;
        monsterState = MonsterState.die;
        
        animator.SetTrigger("IsDie");


        GameMgr.Inst.SpawnCoin(transform.position);


        //몬스터에 추가된 Collider를 비활성화
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }

        //GameMgr의 스코어 누적과 스코어 표시 함수 호출
        GameMgr.Inst.DispScore(50);


        //몬스터 오브젝트 풀로 환원시키는 코루틴 함수 호출
        StartCoroutine(this.PushObjectPool());

    }//void MonsterDie()

    IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(3.0f);

        //각종 변수 초기화
        isDie = false;
        hp = 100;
        gameObject.tag = "MONSTER";
        monsterState = MonsterState.idle;

      // m_riBody.useGravity = true;

        //몬스터에 추가된 Collider을 다시 활성화
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = true;
        }

        //몬스터를 비활성화
        gameObject.SetActive(false);

    }// IEnumerator PushObjectPool()

    void CreateBloodEffect(Vector3 pos)
    {
        //혈흔 효과 생성
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        blood1.GetComponent<ParticleSystem>().Play();
        Destroy(blood1, 3.0f);

        //데칼 생성 위치 - 바닥에서 조금 올린 위치 산출
        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.05f);
        //데칼의 회전값을 무작위로 설정
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));

        //데칼 프리팹 생성
        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        //데칼의 크기도 불규칙적으로 나타나게끔 스케일 조정
        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;

        //5초 후에 혈흔효과 프리팹을 삭제
        Destroy(blood2, 5.0f);

    }//void CreateBloodEffect(Vector3 pos)

    //플레이어가 사망했을 때 실행되는 함수
    void OnPlayerDie()
    {
        //몬스터의 상태를 체크하는 코루틴 함수를 모두 정지시킴
        StopAllCoroutines();
        //추적을 정지하고 애니메이션을 수행
        //nvAgent.isStopped = true;  //<-- nvAgent.Stop();
        if (isDie == false)
            animator.SetTrigger("IsPlayerDie");
    }
}
