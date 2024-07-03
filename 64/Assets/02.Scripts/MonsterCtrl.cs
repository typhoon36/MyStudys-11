using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MonsterCtrl : MonoBehaviour
{
    //������ ���� ������ �ִ� Enumerable ���� ����
    public enum MonsterState { idle, trace, attack, die };

    //������ ���� ���� ������ ������ Enum ����
    public MonsterState monsterState = MonsterState.idle;

    //�ӵ� ����� ���� ���� ������Ʈ�� ������ �Ҵ�
    private Transform monsterTr;
    private Transform playerTr;
    //private NavMeshAgent nvAgent;
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
    Rigidbody m_Rigid = null;

    void Awake()
    {
        traceDist = 10.0f;
        attackDist = 1.5f;

        //������ Transform �Ҵ�
        monsterTr = this.gameObject.GetComponent<Transform>();
        //���� ����� Player�� Transform �Ҵ�
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        ////NavMeshAgent ������Ʈ �Ҵ�
        //nvAgent = this.gameObject.GetComponent<NavMeshAgent>();

        ////���� ����� ��ġ�� �����ϸ� �ٷ� ���� ����
        //nvAgent.destination = playerTr.position;

        //Animator ������Ʈ �Ҵ�
        animator = this.gameObject.GetComponent<Animator>();

        m_Rigid = GetComponent<Rigidbody>();    
    }

    ////�̺�Ʈ �߻��� ������ �Լ� ����
    //void OnEnable()
    //{
    //    //������ �������� ������ �ൿ ���¸� üũ�ϴ� �ڷ�ƾ �Լ� ����
    //    StartCoroutine(this.CheckMonsterState());

    //    //������ ���¿� ���� �����ϴ� ��ƾ�� �����ϴ� �ڷ�ƾ �Լ� ����
    //    StartCoroutine(this.MonsterAction());
    //}

    // Start is called before the first frame update
    void Start()
    {

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

#region -- ���� AI

    //������ �������� ������ �ൿ ���¸� üũ�ϰ� monsterState �� ����
    float m_AI_Delay = 0.0f;

    void CheckMonStateUpdate()
    {
        if (isDie == true)
            return;

        //0.1�� �ֱ�θ� üũ�ϱ� ���� ������ ��� �κ�
        m_AI_Delay -= Time.deltaTime;
        if (0.0f < m_AI_Delay)
            return;

        m_AI_Delay = 0.1f;
        //0.1�� �ֱ�θ� üũ�ϱ� ���� ������ ��� �κ�

        //���Ϳ� �÷��̾� ������ �Ÿ� ����
        float dist = Vector3.Distance(playerTr.position, monsterTr.position);
        float Ydist = Mathf.Abs(playerTr.position.y - monsterTr.position.y);
        //���ΰ��� 2���� ���� �� 1���� �ִ� ���Ͱ� �������� ���ϰ� ó�� �ϱ� ���� �ڵ�

        if (dist <= attackDist) //���ݰŸ� ���� �̳��� ���Դ��� Ȯ��
        {
            monsterState = MonsterState.attack;
        }
        else if(dist <= traceDist && Ydist < 5.0f) //�����Ÿ� ���� �̳��� ���Դ��� Ȯ��
        {
            monsterState = MonsterState.trace; //������ ���¸� �������� ����
        }
        else
        {
            monsterState = MonsterState.idle;  //������ ���¸� idle ���� ����
        }

    }//void CheckMonStateUpdate()

    //������ ���°��� ���� ������ ������ �����ϴ� �Լ�
    void MonActionUpdate()
    {
        if (isDie == true)
            return;

        switch(monsterState)
        {
            //idle ����
            case MonsterState.idle:
                //Animator�� IsTrace ������ false�� ����
                animator.SetBool("IsTrace", false);
                break;

            //��������
            case MonsterState.trace:
                {
                    //���� �̵� ����
                    float a_MoveVelocity = 2.0f;    //��� �ʴ� �̵� �ӵ�...
                    Vector3 a_MoveDir = playerTr.position - transform.position;
                    a_MoveDir.y = 0.0f;

                    if(0.0f < a_MoveDir.magnitude)
                    {
                        Vector3 a_StepVec = a_MoveDir.normalized * a_MoveVelocity * Time.deltaTime;
                        transform.Translate(a_StepVec, Space.World);

                        //--- �̵� ������ �ٶ� ������ ȸ�� ó��
                        float a_RotSpeed = 7.0f;  //�ʴ� ȸ�� �ӵ�
                        Quaternion a_TargetRot = Quaternion.LookRotation(a_MoveDir.normalized);
                        transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      a_TargetRot, Time.deltaTime * a_RotSpeed);
                        //--- �̵� ������ �ٶ� ������ ȸ�� ó��
                    }//if(0.0f < a_MoveDir.magnitude)

                    //Animator�� IsAttack ������ false�� ����
                    animator.SetBool("IsAttack", false);

                    //Animator�� IsTrace ������ true�� ����
                    animator.SetBool("IsTrace", true);
                }
                break;

            //���� ����
            case MonsterState.attack:
                {
                    //IsAttack�� true�� ������ attack State�� ����
                    animator.SetBool("IsAttack", true);

                    //--- ���Ͱ� ���ΰ��� �����ϸ鼭 �ٶ� ������ ó��
                    float a_RotSpeed = 6.0f;   //�ʴ� ȸ�� �ӵ�
                    Vector3 a_CacDir = playerTr.position - transform.position;
                    a_CacDir.y = 0.0f;
                    if(0.0f < a_CacDir.magnitude)
                    {
                        //--- AI 0.1�� ���� üũ ������ ���� �Ÿ����� �־��� ��� ��ġ ����
                        if(attackDist < a_CacDir.magnitude)
                        {
                            float a_MoveVelocity = 2.0f;
                            Vector3 a_StepVec = a_CacDir.normalized * a_MoveVelocity * Time.deltaTime;
                            transform.Translate(a_StepVec, Space.World);
                        }
                        //--- AI 0.1�� ���� üũ ������ ���� �Ÿ����� �־��� ��� ��ġ ����

                        Quaternion a_TargetRot = Quaternion.LookRotation(a_CacDir.normalized);
                        transform.rotation = Quaternion.Slerp(
                                    transform.rotation, a_TargetRot, Time.deltaTime * a_RotSpeed); 
                    }
                    //--- ���Ͱ� ���ΰ��� �����ϸ鼭 �ٶ� ������ ó��
                }
                break;
        }//switch(monsterState)

    }//void MonActionUpdate()

#endregion

  

    //Bullet�� �浹 üũ
    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "BULLET")
        {
            //���� ȿ�� �Լ� ȣ��
            CreateBloodEffect(coll.transform.position);

            //���� �Ѿ��� Damage�� ������ ���� hp ����
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
            if(hp <= 0)
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
        //nvAgent.isStopped = true;
        animator.SetTrigger("IsDie");

        m_Rigid.useGravity = false;

        //���Ϳ� �߰��� Collider�� ��Ȱ��ȭ
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach(Collider coll in gameObject.GetComponentsInChildren<SphereCollider>()) 
        {
            coll.enabled = false;
        }

        //GameMgr�� ���ھ� ������ ���ھ� ǥ�� �Լ� ȣ��
        GameMgr.Inst.DispScore(50);

        //���� ������Ʈ Ǯ�� ȯ����Ű�� �ڷ�ƾ �Լ� ȣ��
        StartCoroutine(this.PushObjectPool());

        //## �׾����� ����ȣ��
        GameMgr.Inst.SpawnCoin(this.transform.position);



    }//void MonsterDie()

    IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(3.0f);

        //���� ���� �ʱ�ȭ
        isDie = false;
        hp = 100;
        gameObject.tag = "MONSTER";
        monsterState = MonsterState.idle;

        m_Rigid.useGravity = true;

        //���Ϳ� �߰��� Collider�� �ٽ� Ȱ��ȭ
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;

        foreach(Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
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
        ////������ �����ϰ� �ִϸ��̼��� ����
        //nvAgent.isStopped = true;  //<-- nvAgent.Stop();
        if(isDie == false)
           animator.SetTrigger("IsPlayerDie");
    }
}
