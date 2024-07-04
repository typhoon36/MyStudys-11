using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Ŭ������ System.Serializable �̶�� ��Ʈ����Ʈ(Attribute)�� ����ؾ�
//Inspector �信 �����
[System.Serializable]
public class Anim
{
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
}

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;


    //�̵� �ӵ� ����
    public float moveSpeed = 10.0f;


    //## �߷�& ���� ����
    float m_GRtTime = 0.69f;
    float m_GSpeed = 36.2f;
    float m_VelY = -12.0f; //�߷� �ִ�
    float m_JumpPw = 13.0f; //���� ����


    //## �������� 
    // bool m_DJump = false;


    //ȸ�� �ӵ� ����
    public float rotSpeed = 100.0f;
    Vector3 m_CacVec = Vector3.zero;
    //�ν����ͺ信 ǥ���� �ִϸ��̼� Ŭ���� ����
    public Anim anim;

    //�Ʒ��� �ִ� 3D ���� Animation ������Ʈ�� �����ϱ� ���� ����
    public Animation _animation;

    //Player�� ���� ����
    public int hp = 100;
    //Plyaer�� ���� �ʱⰪ
    private int initHp;
    //Player�� Health bar �̹���
    public Image imgHpbar;

    CharacterController m_ChrCtrl;  //���� ĳ���Ͱ� ������ �ִ� ĳ���� ��Ʈ�ѷ� ���� ����

    //## �Ѿ� ��ũ��Ʈ
    FireCtrl m_FireCtrl = null;

    [Header("Sound")]
    public AudioClip Coinsfx;
    public AudioClip Diamondsfx;
    AudioSource Ad_Source = null;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        moveSpeed = 7.0f;   //�̵��ӵ� �ʱ�ȭ

        m_GSpeed = (m_JumpPw + (-m_VelY)) / m_GRtTime;

        //���� �ʱⰪ ����
        initHp = hp;

        //�ڽ��� ������ �ִ� Animation ������Ʈ�� ã�ƿ� ������ �Ҵ�
        _animation = GetComponentInChildren<Animation>();

        //Animation ������Ʈ�� �ִϸ��̼� Ŭ���� �����ϰ� ����
        _animation.clip = anim.idle;
        _animation.Play();

        m_ChrCtrl = GetComponent<CharacterController>();

        m_FireCtrl = GetComponent<FireCtrl>();

        Transform a_PMesh = transform.Find("PlayerModel");

        if (a_PMesh != null)
            Ad_Source = a_PMesh.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //�����¿� �̵� ���� ���� ���
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        if (1.0f < moveDir.magnitude)
            moveDir.Normalize();

        ////Translate(�̵����� * Time.deltaTime * ������ * �ӵ�, ������ǥ)
        //transform.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);
        ////������ǥ �ɼ��� �⺻���� Space relativeTo = Space.Self

        if (m_ChrCtrl != null)
        {
            //���͸� ���� ��ǥ�� ���ؿ��� ���� ��ǥ�� �������� ��ȯ�Ѵ�.
            //����󿡼� ĳ���Ͱ� �̵��ؾ� �ϴ� ���� ���ͷ� ȯ����
            moveDir = transform.TransformDirection(moveDir);

            moveDir = moveDir * moveSpeed;

            //### �ٴ��� ����
            if (m_ChrCtrl.isGrounded == true)
            {
                if (Input.GetKeyDown(KeyCode.Space)==true)
                {
                    m_VelY = m_JumpPw;
                    //m_DJump = true;
                }
            }

            //else
            //{
            //    if(Input.GetKeyDown(KeyCode.Space)==true && m_DJump == true)
            //    {
            //        m_VelY += m_JumpPw;
            //        m_DJump =false;
            //    }
            //}

            if (-12.0f < m_VelY)

                m_VelY -= m_GSpeed * Time.deltaTime;

            moveDir.y = m_VelY;

            //ĳ���Ϳ� �߷��� ����Ǵ� �̵��Լ�
            // m_ChrCtrl.SimpleMove(moveDir * moveSpeed);

            m_ChrCtrl.Move(moveDir*Time.deltaTime);
        }

        if (Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
            if (GameMgr.IsPointerOverUIObject() == false)
            {
                //Vector3.up ���� �������� rotSpeed��ŭ�� �ӵ��� ȸ��
                transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X") * 3.0f);
                //transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxisRaw("Mouse X") * 3.0f);
                //m_CacVec = transform.eulerAngles;
                //m_CacVec.y += (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse X") * 10.0f);
                //transform.eulerAngles = m_CacVec;
            }

        //Ű���� �Է°��� �������� ������ �ִϸ��̼� ����
        if (v >= 0.01f)
        { //���� �ִϸ��̼�
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if (v <= -0.01f)
        { //���� �ִϸ��̼�
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if (h >= 0.01f)
        { //������ �̵� �ִϸ��̼�
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else if (h <= -0.01f)
        { //���� �̵� �ִϸ��̼�
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else
        { //���� idle �ִϸ��̼�
            _animation.CrossFade(anim.idle.name, 0.3f);
        }
    }

    //�浹�� Collider�� IsTrigger �ɼ��� üũ���� �� �߻�
    void OnTriggerEnter(Collider coll)
    {
        //�浹�� Collider�� ������ PUNCH�̸� Player�� HP ����
        if (coll.gameObject.tag == "PUNCH")
        {
            hp -= 10;

            //Image UI �׸��� fillAmount �Ӽ��� ������ ���� ������ �� ����
            imgHpbar.fillAmount = (float)hp / (float)initHp;

            //Debug.Log("Player HP = " + hp.ToString());

            //Player�� ������ 0�����̸� ��� ó��
            if (hp <= 0)
            {
                PlayerDie();
            }
        }

        else if (coll.gameObject.name.Contains("CoinPrefab")== true)
        {
            int a_CurGold = 10;

            GameMgr.Inst.AddGold(a_CurGold);

            if (Ad_Source != null && Coinsfx !=null)
                Ad_Source.PlayOneShot(Coinsfx, 0.3f);

            Destroy(coll.gameObject);
        }
    }

    //Player�� ��� ó�� ��ƾ
    void PlayerDie()
    {
        //Debug.Log("Player Die !!");

        //MONSTER��� Tag�� ���� ��� ���ӿ�����Ʈ�� ã�ƿ�
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");

        //��� ������ OnPlayerDie �Լ��� ���������� ȣ��
        foreach (GameObject monster in monsters)
        {
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
            //MonsterCtrl a_MonCtrl = monster.GetComponent<MonsterCtrl>();
            //a_MonCtrl.OnPlayerDie();
        }

        _animation.Stop();  //�ִϸ����� ������Ʈ�� �ִϸ��̼� ���� �Լ�

        GameMgr.s_GameState = GameState.GameEnd;
        //GameMgr�� �̱��� �ν��Ͻ��� ������ isGameOver �������� ����
        GameMgr.Inst.isGameOver = true;

    }//void PlayerDie()

    //# ��ų ���
    public void UseSkill(SkillType a_SkType)
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;
        //## 30% ����
        if (a_SkType == SkillType.Skill_0)
        {

            if (hp < initHp)
            {
                hp += 30;
                if (initHp < hp)
                    hp = initHp;

                imgHpbar.fillAmount = (float)hp / (float)initHp;
            }

        }


        //## ����ź
        else if (a_SkType == SkillType.Skill_1)
        {
            if (m_FireCtrl != null)
            {
                m_FireCtrl.F_Grenade();
            }



        }

    }



}
