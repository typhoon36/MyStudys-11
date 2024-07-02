using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    //������ ���� ������ �ִ� Enumerable ���� ����
    public enum MonsterState { idle, trace, attack, die };

    //������ ���� ���� ������ ������ Enum ����
    public MonsterState monsterState = MonsterState.idle;

    //�ӵ� ����� ���� ���� ������Ʈ�� ������ �Ҵ�
    private Transform monsterTr;
    private Transform playerTr;
    private Animator animator;

    //���� �����Ÿ�
    public float traceDist = 10.0f;
    //���� �����Ÿ�
    public float attackDist = 1.5f; //2.0f;

    //������ ��� ����
    private bool isDie = false;

    //���� ȿ�� ������
    public GameObject bloodEffect;
    //���� ��Į ȿ�� ������
    public GameObject bloodDecal;

    //���� ���� ����
    private int hp = 100;

    //������ٵ�
    Rigidbody m_riBody = null;


    void Awake()
    {
        traceDist = 10.0f;
        attackDist = 1.5f;

        //������ Transform �Ҵ�
        monsterTr = this.gameObject.GetComponent<Transform>();
        //���� ����� Player�� Transform �Ҵ�
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();




        //Animator ������Ʈ �Ҵ�
        animator = this.gameObject.GetComponent<Animator>();
        
        m_riBody = GetComponent<Rigidbody>();
    }

    //�̺�Ʈ �߻��� ������ �Լ� ���� -- �Ϲ��Լ��� ����





    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate() //## ���� �̵�
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;

        if (playerTr.gameObject.activeSelf == false)
            playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        CheckMonState();
        MonAction();

        //## Ȱ���� �߷°� �Ʒ��� �ֱ�
        if (isDie == false)
        {
            m_riBody.AddForce(Vector3.down * 100.0f);
        }

    }

    #region # ���� AI (��ü�� �Ϲ��Լ�)
    //## ���� ���� ���� �ൿ���� üũ �� ���ͻ��� ����
    float m_AI_delay = 0;

    void CheckMonState()
    {
        if (isDie == true) return;

        //## 0.1�� �ֱ�� ������
        m_AI_delay -= Time.deltaTime;
        if (0.0f < m_AI_delay)
            return;


        m_AI_delay = 0.1f;

        //## ���Ϳ� �÷��̾� ���� ����
        float dist = Vector3.Distance(playerTr.position, monsterTr.position);
        float Ydist = Mathf.Abs(playerTr.position.y - monsterTr.position.y);
        //2�� ������ 1�� ���� üũ

        // ���ݹ��� ��
        if (dist <= attackDist)
        {
            monsterState = MonsterState.attack;
        }

        //�����Ÿ� ��
        else if (dist <= traceDist && Ydist < 5.0f)
        {
            monsterState = MonsterState.trace;
        }

        //�Ѵ� �ƴϸ� 
        else
        {
            monsterState = MonsterState.idle;
        }

    }

    //## ���� ���¿� ���� ����
    void MonAction()
    {
        if (isDie == true) return;

        switch (monsterState)
        {

            case MonsterState.idle:
                //## �����¿��� Animator ���� ����
                animator.SetBool("IsTrace", false);
                break;

            case MonsterState.trace:
                {
                    //## ���� �̵�
                    float a_MoveVel = 2.0f;
                    Vector3 a_Dir = playerTr.position - transform.position;
                    a_Dir.y = 0.0f;

                    if (0.0f < a_Dir.magnitude)
                    {
                        Vector3 a_Step = a_Dir.normalized * a_MoveVel * Time.deltaTime;
                        transform.Translate(a_Step, Space.World);

                        //## �̵��� �� ������ �ٶ󺸵���
                        float a_RotSpeed = 7.0f;
                        Quaternion a_Target = Quaternion.LookRotation(a_Dir.normalized);

                        transform.rotation = Quaternion.Slerp(transform.rotation, a_Target, a_RotSpeed * Time.deltaTime);

                    }

                    //## �ִϸ����� ���� ����
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);

                }
                break;

            case MonsterState.attack:
                {
                    //## ���� �ִϸ��̼�
                    animator.SetBool("IsAttack", true);

                    //## ���ݽ� �ٶ󺸵���
                    float a_RotSpd = 6.0f;

                    Vector3 a_CacDir = playerTr.position - transform.position;

                    a_CacDir.y = 0.0f;

                    if (0.0f < a_CacDir.magnitude)
                    {
                        //## �����̷� ���� ���� �Ÿ����� �־��� ����� ����
                        //-- ���� �ǹ�? ���ΰ����� �ݸ����� �ȴ�� ��.
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








    //##�浹 ó��
    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "BULLET")
        {
            //���� ȿ�� �Լ� ȣ��
            CreateBloodEffect(coll.transform.position);

            //���� �Ѿ��� Damage�� ������ ���� hp ����
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
            if (hp <= 0)
            {
                MonsterDie();
            }

            //Bullet ����
            Destroy(coll.gameObject);

            //IsHit Trigger�� �߻���Ű�� Any State���� gothit�� ���̵�
            animator.SetTrigger("IsHit");
        }
    }

    //���� ����� ó�� ��ƾ
    void MonsterDie()
    {
        //����� ������ �±׸� Untagged�� ����
        gameObject.tag = "Untagged";

        //��� �ڷ�ƾ�� ����
        StopAllCoroutines();

        isDie = true;
        monsterState = MonsterState.die;
        
        animator.SetTrigger("IsDie");


        GameMgr.Inst.SpawnCoin(transform.position);


        //���Ϳ� �߰��� Collider�� ��Ȱ��ȭ
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }

        //GameMgr�� ���ھ� ������ ���ھ� ǥ�� �Լ� ȣ��
        GameMgr.Inst.DispScore(50);


        //���� ������Ʈ Ǯ�� ȯ����Ű�� �ڷ�ƾ �Լ� ȣ��
        StartCoroutine(this.PushObjectPool());

    }//void MonsterDie()

    IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(3.0f);

        //���� ���� �ʱ�ȭ
        isDie = false;
        hp = 100;
        gameObject.tag = "MONSTER";
        monsterState = MonsterState.idle;

      // m_riBody.useGravity = true;

        //���Ϳ� �߰��� Collider�� �ٽ� Ȱ��ȭ
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = true;
        }

        //���͸� ��Ȱ��ȭ
        gameObject.SetActive(false);

    }// IEnumerator PushObjectPool()

    void CreateBloodEffect(Vector3 pos)
    {
        //���� ȿ�� ����
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        blood1.GetComponent<ParticleSystem>().Play();
        Destroy(blood1, 3.0f);

        //��Į ���� ��ġ - �ٴڿ��� ���� �ø� ��ġ ����
        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.05f);
        //��Į�� ȸ������ �������� ����
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));

        //��Į ������ ����
        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        //��Į�� ũ�⵵ �ұ�Ģ������ ��Ÿ���Բ� ������ ����
        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;

        //5�� �Ŀ� ����ȿ�� �������� ����
        Destroy(blood2, 5.0f);

    }//void CreateBloodEffect(Vector3 pos)

    //�÷��̾ ������� �� ����Ǵ� �Լ�
    void OnPlayerDie()
    {
        //������ ���¸� üũ�ϴ� �ڷ�ƾ �Լ��� ��� ������Ŵ
        StopAllCoroutines();
        //������ �����ϰ� �ִϸ��̼��� ����
        //nvAgent.isStopped = true;  //<-- nvAgent.Stop();
        if (isDie == false)
            animator.SetTrigger("IsPlayerDie");
    }
}
