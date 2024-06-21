using System.Collections;
using System.Collections.Generic;
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
    private NavMeshAgent nvAgent;
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

    private void Awake()
    {
        traceDist = 100.0f;
        attackDist = 1.7f;

        //������ Transform �Ҵ�
        monsterTr = this.gameObject.GetComponent<Transform>();
        //���� ����� Player�� Transform �Ҵ�
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //NavMeshAgent ������Ʈ �Ҵ�
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();

        ////���� ����� ��ġ�� �����ϸ� �ٷ� ���� ����
        //nvAgent.destination = playerTr.position;

        //Animator ������Ʈ �Ҵ�
        animator = this.gameObject.GetComponent<Animator>();
    }


    private void OnEnable()
    {
         //������ �������� ������ �ൿ ���¸� üũ�ϴ� �ڷ�ƾ �Լ� ����
        StartCoroutine(this.CheckMonsterState());

        //������ ���¿� ���� �����ϴ� ��ƾ�� �����ϴ� �ڷ�ƾ �Լ� ����
        StartCoroutine(this.MonsterAction());
    }


    // Start is called before the first frame update
    void Start()
    {
       

       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //������ �������� ������ �ൿ ���¸� üũ�ϰ� monsterState �� ����
    IEnumerator CheckMonsterState()
    {
        while(!isDie)
        {
            //0.2�� ���� ��ٷȴٰ� �������� �Ѿ
            yield return new WaitForSeconds(0.2f);

            //���Ϳ� �÷��̾� ������ �Ÿ� ����
            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            if (dist <= attackDist)  //���ݰŸ� ���� �̳��� ���Դ��� Ȯ��
            {
                monsterState = MonsterState.attack;
            }
            else if (dist <= traceDist) //�����Ÿ� ���� �̳��� ���Դ��� Ȯ��
            {
                monsterState = MonsterState.trace; //������ ���¸� �������� ����
            }
            else
            {
                monsterState = MonsterState.idle;  //������ ���¸� idle ���� ����
            }

        }//while(!isDie)
    }//IEnumerator CheckMonsterState()

    //������ ���°��� ���� ������ ������ �����ϴ� �Լ�
    IEnumerator MonsterAction()
    {
        while(!isDie)
        {
            //if (isDie == true)
            //    yield break;  //�ڷ�ƾ �Լ��� ��� ���������� �ڵ�

            switch(monsterState)
            {
                //idle ����
                case MonsterState.idle:
                    //���� ����
                    nvAgent.isStopped = true; //<-- nvAgent.Stop();
                    //Animator�� IsTrace ������ false�� ����
                    animator.SetBool("IsTrace", false);
                    break;

                //���� ����
                case MonsterState.trace:
                    //���� ����� ��ġ�� �Ѱ���
                    nvAgent.destination = playerTr.position;
                    //������ �����
                    nvAgent.isStopped = false; //<--nvAgent.Resume();

                    //Animator�� IsAttack ������ false�� ����
                    animator.SetBool("IsAttack", false);
                    //Animator�� IsTrace �������� true�� ����
                    animator.SetBool("IsTrace", true);
                    break;

                //���� ����
                case MonsterState.attack:
                    {
                        //���� ����
                        nvAgent.isStopped = true; //<-- nvAgent.Stop();
                        //IsAttack�� true�� ������ attack State�� ����
                        animator.SetBool("IsAttack", true);

                        //--- ���Ͱ� ������ �����ϸ鼭 �ٶ� ������ ó��
                        float a_RotSpeed = 6.0f;
                        Vector3 a_CacDir = playerTr.position - transform.position;
                        a_CacDir.y = 0.0f;
                        if (0.0f < a_CacDir.magnitude)
                        {
                            Quaternion a_TargetRot = Quaternion.LookRotation(a_CacDir.normalized);
                            transform.rotation = Quaternion.Slerp(
                                    transform.rotation, a_TargetRot, Time.deltaTime * a_RotSpeed ); 
                        }
                        //--- ���Ͱ� ������ �����ϸ鼭 �ٶ� ������ ó��
                    }
                    break;
            }//switch(monsterState)

            yield return null; //<-- �� �÷����� ���� ���� ��� ���

        }//while(!isDie)
    }//IEnumerator MonsterAction()

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
        //## ����� ���� �±� ����
        this.gameObject.tag = "Untagged";


        //��� �ڷ�ƾ�� ����
        StopAllCoroutines();

        isDie = true;
        monsterState = MonsterState.die;
        nvAgent.isStopped = true;
        animator.SetTrigger("IsDie");

        //���Ϳ� �߰��� Collider�� ��Ȱ��ȭ
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach(Collider coll in gameObject.GetComponentsInChildren<SphereCollider>()) 
        {
            coll.enabled = false;
        }

        //GameMgr�� ���ھ� ������ ���ھ� ǥ�� �Լ� ȣ��
        GameMgr.Inst.DispScore(50);

        StartCoroutine(this.PushObjectPool());

    }//void MonsterDie()

    //## �ʱ�ȭ
    IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(3.0f);

        //������ ���¸� �ʱ�ȭ
        isDie = false;
        hp = 100;
        gameObject.tag = "MONSTER";
        monsterState = MonsterState.idle;

        //������ Collider�� �ٽ� Ȱ��ȭ
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = true;
        }

        //���͸� ��Ȱ��ȭ
        this.gameObject.SetActive(false);
    }






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
        nvAgent.isStopped = true;  //<-- nvAgent.Stop();
        if(isDie == false)
           animator.SetTrigger("IsPlayerDie");
    }
}
