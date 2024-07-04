using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    public enum MonsterState { idle, trace, attack, shot, die };
    public MonsterState monsterState = MonsterState.idle;

    private Transform monsterTr;
    private Transform playerTr;
    private Animator animator;

    public float traceDist = 10.0f;
    public float attackDist = 1.5f;

    private bool isDie = false;

    public GameObject bloodEffect;
    public GameObject bloodDecal;

    En_FireCtrl firectrl = null;

    public GameObject bulletpos;
    float Shot_Time = 0.0f;
    float Shot_Delay = 1.5f;
    float BulletSpeed = 10.0f;

    private int hp = 100;
    Rigidbody m_Rigid = null;

    void Awake()
    {
        traceDist = 10.0f;
        attackDist = 1.5f;

        monsterTr = this.gameObject.GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        animator = this.gameObject.GetComponent<Animator>();

        m_Rigid = GetComponent<Rigidbody>();

        firectrl = GetComponent<En_FireCtrl>();
    }

    void Start()
    {
    }

    void FixedUpdate()
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;

        if (playerTr.gameObject.activeSelf == false)
            playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        CheckMonStateUpdate();
        MonActionUpdate();
    }

    #region -- ∏ÛΩ∫≈Õ AI

    float m_AI_Delay = 0.0f;

    void CheckMonStateUpdate()
    {
        if (isDie == true)
            return;

        m_AI_Delay -= Time.deltaTime;
        if (0.0f < m_AI_Delay)
            return;

        m_AI_Delay = 0.1f;

        float dist = Vector3.Distance(playerTr.position, monsterTr.position);
        float Ydist = Mathf.Abs(playerTr.position.y - monsterTr.position.y);

        if (dist <= attackDist)
        {
            monsterState = MonsterState.attack;
        }
        else if (dist <= traceDist && Ydist < 5.0f)
        {
            monsterState = MonsterState.trace;
        }
        else if (dist > traceDist)
        {
            RaycastHit hit;
            int obstacleLayerMask = 1 << LayerMask.NameToLayer("Obstacle");
            if (Physics.Raycast(monsterTr.position, playerTr.position - monsterTr.position, out hit, traceDist, obstacleLayerMask))
            {
                if (hit.collider != null)
                {
                    Debug.Log(LayerMask.NameToLayer("Obstacle"));
                    monsterState = MonsterState.idle;
                }
                else
                {
                    monsterState = MonsterState.shot;
                }
            }
            else
            {
                monsterState = MonsterState.shot;
            }
        }
        else
        {
            monsterState = MonsterState.idle;
        }
    }

    void MonActionUpdate()
    {
        if (isDie == true)
            return;

        switch (monsterState)
        {
            case MonsterState.idle:
                animator.SetBool("IsTrace", false);
                break;

            case MonsterState.trace:
                {
                    float a_MoveVelocity = 2.0f;
                    Vector3 a_MoveDir = playerTr.position - transform.position;
                    a_MoveDir.y = 0.0f;

                    if (0.0f < a_MoveDir.magnitude)
                    {
                        Vector3 a_StepVec = a_MoveDir.normalized * a_MoveVelocity * Time.deltaTime;
                        transform.Translate(a_StepVec, Space.World);

                        float a_RotSpeed = 7.0f;
                        Quaternion a_TargetRot = Quaternion.LookRotation(a_MoveDir.normalized);
                        transform.rotation = Quaternion.Slerp(transform.rotation, a_TargetRot, Time.deltaTime * a_RotSpeed);
                    }

                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);
                }
                break;

            case MonsterState.attack:
                {
                    animator.SetBool("IsAttack", true);

                    float a_RotSpeed = 6.0f;
                    Vector3 a_CacDir = playerTr.position - transform.position;
                    a_CacDir.y = 0.0f;
                    if (0.0f < a_CacDir.magnitude)
                    {
                        if (attackDist < a_CacDir.magnitude)
                        {
                            float a_MoveVelocity = 2.0f;
                            Vector3 a_StepVec = a_CacDir.normalized * a_MoveVelocity * Time.deltaTime;
                            transform.Translate(a_StepVec, Space.World);
                        }

                        Quaternion a_TargetRot = Quaternion.LookRotation(a_CacDir.normalized);
                        transform.rotation = Quaternion.Slerp(transform.rotation, a_TargetRot, Time.deltaTime * a_RotSpeed);
                    }
                }
                break;

            case MonsterState.shot:
                {
                    if (firectrl == null)
                        return;

                    float a_RotSpeed = 6.0f;
                    Vector3 a_CacDir = playerTr.position - transform.position;
                    a_CacDir.y = 0.0f;
                    if (0.0f < a_CacDir.magnitude)
                    {
                        Quaternion a_TargetRot = Quaternion.LookRotation(a_CacDir.normalized);
                        transform.rotation = Quaternion.Slerp(transform.rotation, a_TargetRot, Time.deltaTime * a_RotSpeed);
                    }

                    Shot_Time += Time.deltaTime;
                    if (Shot_Delay <= Shot_Time)
                    {
                        Shot_Time = 0.0f;
                        firectrl.Fire();
                    }
                }
                break;
        }
    }

    #endregion

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "BULLET")
        {
            CreateBloodEffect(coll.transform.position);

            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
            if (hp <= 0)
            {
                MonsterDie();
            }

            Destroy(coll.gameObject);

            animator.SetTrigger("IsHit");
        }
    }

    void MonsterDie()
    {
        gameObject.tag = "Untagged";

        StopAllCoroutines();

        isDie = true;
        monsterState = MonsterState.die;
        animator.SetTrigger("IsDie");

        m_Rigid.useGravity = false;

        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }

        GameMgr.Inst.DispScore(50);

        StartCoroutine(this.PushObjectPool());

        GameMgr.Inst.SpawnCoin(this.transform.position);
    }

    IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(3.0f);

        isDie = false;
        hp = 100;
        gameObject.tag = "MONSTER";
        monsterState = MonsterState.idle;

        m_Rigid.useGravity = true;

        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = true;
        }

        gameObject.SetActive(false);
    }

    void CreateBloodEffect(Vector3 pos)
    {
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        blood1.GetComponent<ParticleSystem>().Play();
        Destroy(blood1, 3.0f);

        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.05f);
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));

        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;

        Destroy(blood2, 5.0f);
    }

    void OnPlayerDie()
    {
        StopAllCoroutines();

        if (isDie == false)
            animator.SetTrigger("IsPlayerDie");
    }

    public void TakeDamage(int Val)
    {
        if (hp <= 0)
            return;

        hp -= Val;

        if (hp <= 0)
        {
            hp = 0;
            MonsterDie();
            return;
        }

        animator.SetTrigger("IsHit");
    }
}
