using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MonsterCtrl : MonoBehaviour
{
    //몬스터의 상태 정보가 있는 Enumerable 변수 선언
    public enum MonsterState { idle, trace, attack, die };

    //몬스터의 현재 상태 정보를 저장할 Enum 변수
    public MonsterState monsterState = MonsterState.idle;

    //속도 향상을 위해 각종 컴포넌트를 변수에 할당
    private Transform monsterTr;
    private Transform playerTr;
    //private NavMeshAgent nvAgent;
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
    Rigidbody m_Rigid = null;

    //# 총알
    public GameObject bullet;
    float m_BLTime = 0.0f;
    LayerMask m_Ray = -1;



    void Awake()
    {
        traceDist = 10.0f;
        attackDist = 1.5f;

        //몬스터의 Transform 할당
        monsterTr = this.gameObject.GetComponent<Transform>();
        //추적 대상인 Player의 Transform 할당
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        ////NavMeshAgent 컴포넌트 할당
        //nvAgent = this.gameObject.GetComponent<NavMeshAgent>();

        ////추적 대상의 위치를 설정하면 바로 추적 시작
        //nvAgent.destination = playerTr.position;

        //Animator 컴포넌트 할당
        animator = this.gameObject.GetComponent<Animator>();

        m_Rigid = GetComponent<Rigidbody>();
    }



    // Start is called before the first frame update
    void Start()
    {
        //# 벽감지
        m_Ray = 1 << LayerMask.NameToLayer("Default");
        

    }

    // Update is called once per frame
    void FixedUpdate()  //void Update()
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;

        if (playerTr.gameObject.activeSelf == false)
            playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        CheckMonStateUpdate();
        MonActionUpdate();

    }

    #region -- 몬스터 AI

    //일정한 간격으로 몬스터의 행동 상태를 체크하고 monsterState 값 변경
    float m_AI_Delay = 0.0f;

    void CheckMonStateUpdate()
    {
        if (isDie == true)
            return;

        //0.1초 주기로만 체크하기 위한 딜레이 계산 부분
        m_AI_Delay -= Time.deltaTime;
        if (0.0f < m_AI_Delay)
            return;

        m_AI_Delay = 0.1f;
        //0.1초 주기로만 체크하기 위한 딜레이 계산 부분

        //몬스터와 플레이어 사이의 거리 측정
        float dist = Vector3.Distance(playerTr.position, monsterTr.position);
        float Ydist = Mathf.Abs(playerTr.position.y - monsterTr.position.y);
        //주인공이 2층에 있을 때 1층에 있는 몬스터가 추적하지 못하게 처리 하기 위한 코드

        if (dist <= attackDist) //공격거리 범위 이내로 들어왔는지 확인
        {
            monsterState = MonsterState.attack;
        }
        else if (dist <= traceDist && Ydist < 5.0f) //추적거리 범위 이내로 들어왔는지 확인
        {
            monsterState = MonsterState.trace; //몬스터의 상태를 추적으로 설정
        }
        else
        {
            monsterState = MonsterState.idle;  //몬스터의 상태를 idle 모드로 설정
        }

    }//void CheckMonStateUpdate()

    //몬스터의 상태값에 따라 적절한 동적을 수행하는 함수
    void MonActionUpdate()
    {
        if (isDie == true)
            return;

        switch (monsterState)
        {
            //idle 상태
            case MonsterState.idle:
                //Animator의 IsTrace 변수를 false로 설정
                animator.SetBool("IsTrace", false);
                break;

            //추적상태
            case MonsterState.trace:
                {
                    //추적 이동 구현
                    float a_MoveVelocity = 2.0f;    //평면 초당 이동 속도...
                    Vector3 a_MoveDir = playerTr.position - transform.position;
                    a_MoveDir.y = 0.0f;

                    if (0.0f < a_MoveDir.magnitude)
                    {
                        Vector3 a_StepVec = a_MoveDir.normalized * a_MoveVelocity * Time.deltaTime;
                        transform.Translate(a_StepVec, Space.World);

                        //--- 이동 방향을 바라 보도록 회전 처리
                        float a_RotSpeed = 7.0f;  //초당 회전 속도
                        Quaternion a_TargetRot = Quaternion.LookRotation(a_MoveDir.normalized);
                        transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      a_TargetRot, Time.deltaTime * a_RotSpeed);
                        //--- 이동 방향을 바라 보도록 회전 처리
                    }//if(0.0f < a_MoveDir.magnitude)

                    //Animator의 IsAttack 변수를 false로 설정
                    animator.SetBool("IsAttack", false);

                    //Animator의 IsTrace 변수를 true로 설정
                    animator.SetBool("IsTrace", true);
                }
                break;

            //공격 상태
            case MonsterState.attack:
                {
                    //IsAttack을 true로 설정해 attack State로 전이
                    animator.SetBool("IsAttack", true);

                    //--- 몬스터가 주인공을 공격하면서 바라 보도록 처리
                    float a_RotSpeed = 6.0f;   //초당 회전 속도
                    Vector3 a_CacDir = playerTr.position - transform.position;
                    a_CacDir.y = 0.0f;
                    if (0.0f < a_CacDir.magnitude)
                    {
                        //--- AI 0.1초 간격 체크 때문에 공격 거리에서 멀어진 경우 위치 보정
                        if (attackDist < a_CacDir.magnitude)
                        {
                            float a_MoveVelocity = 2.0f;
                            Vector3 a_StepVec = a_CacDir.normalized * a_MoveVelocity * Time.deltaTime;
                            transform.Translate(a_StepVec, Space.World);
                        }
                        //--- AI 0.1초 간격 체크 때문에 공격 거리에서 멀어진 경우 위치 보정

                        Quaternion a_TargetRot = Quaternion.LookRotation(a_CacDir.normalized);
                        transform.rotation = Quaternion.Slerp(
                                    transform.rotation, a_TargetRot, Time.deltaTime * a_RotSpeed);
                    }
                    //--- 몬스터가 주인공을 공격하면서 바라 보도록 처리
                }
                break;
        }//switch(monsterState)

        FireUpdate();

    }//void MonActionUpdate()

    //## 주기적 총알 발사
    void FireUpdate()
    {
        //플레이어 위치 계산 & 몬스터 위치 계산
        Vector3 a_PPos = playerTr.position;
        a_PPos.y = a_PPos.y + 1.5f;
        Vector3 a_MPos = transform.position;
        a_MPos.y = a_MPos.y + 1.5f;

        // 플레이어와 몬스터 사이의 거리 계산
        Vector3 a_CurDir = a_PPos - a_MPos;
        // 총알 제한
        float a_LayLimit = 3.0f;

        //## 총알 주기
        m_BLTime = m_BLTime - Time.deltaTime;

        if (m_BLTime <= 0)
        {
            m_BLTime = 0.0f;
        }

        //### 추적거리 이내에 있을 때 제외
        if (traceDist <= a_CurDir.magnitude)
            return;
        //### 높이제한
        if ((-a_LayLimit <= a_CurDir.y && a_CurDir.y <= a_LayLimit) == false)
            return;

        bool IsRay = false;
        if(Physics.Raycast(a_MPos, a_CurDir, out RaycastHit hit , 100.0f, m_Ray) == true)
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                IsRay = true;
            }
        }
        
        //### 시야에 주인공 없으면 return
        if (IsRay == false)
            return;


        //### 몬스터가 플레이어 바라보게 회전
        Vector3 a_CurVLen = playerTr.position - transform.position;

        a_CurVLen.y = 0.0f;

        Quaternion a_TargetRot = Quaternion.LookRotation(a_CurVLen.normalized);

        transform.rotation = Quaternion.Slerp(transform.rotation, a_TargetRot, Time.deltaTime * 6.0f);


        //#### 도달했을때 총알발사
        if (m_BLTime <= 0)
        {
            Vector3 a_StPos = a_MPos + a_CurDir.normalized * 1.5f;
            GameObject a_Bullet = Instantiate(bullet, a_StPos, Quaternion.identity);

            a_Bullet.layer = LayerMask.NameToLayer("E_BULLET");
            a_Bullet.tag = "E_BULLET";
            a_Bullet.transform.forward = a_CurDir.normalized;

            m_BLTime = 2.0f;

        }



    }




    #endregion



    //Bullet과 충돌 체크
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
        //nvAgent.isStopped = true;
        animator.SetTrigger("IsDie");

        m_Rigid.useGravity = false;

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

        //보상으로 아이템 드롭
        GameMgr.Inst.SpawnCoin(this.transform.position);

    }//void MonsterDie()

    IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(3.0f);

        //각종 변수 초기화
        isDie = false;
        hp = 100;
        gameObject.tag = "MONSTER";
        monsterState = MonsterState.idle;

        m_Rigid.useGravity = true;

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
        ////추적을 정지하고 애니메이션을 수행
        //nvAgent.isStopped = true;  //<-- nvAgent.Stop();
        if (isDie == false)
            animator.SetTrigger("IsPlayerDie");
    }

    public void TakeDamage(int a_Value)
    {
        if (hp <= 0.0f)     //이렇게 하면 사망 처리는 한번만 될 것임
            return;

        //혈흔 효과 함수 호출
        CreateBloodEffect(transform.position);

        hp -= a_Value;
        if (hp <= 0)
        {
            hp = 0;
            MonsterDie();
            return;
        }

        animator.SetTrigger("IsHit");
    }
}
