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
    float m_GReturnTime = 0.69f; //m_JumpPower�� �� ��Ƹ԰� (-m_VelocityY)�� �����ؾ� �ϴµ� �ɸ��� �ð�
    float m_GravitySpeed = 36.2f;
    float m_VelocityY = -12.0f;  //�߷�(������ ���� ������ ��), �߷� ���ӵ��� �ִ�ġ : -12.0f
    float m_JumpPower = 13.0f;   //������ �پ� ������ ��
    //bool m_CanDoubleJump = false;

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

    [Header("--- Sound ---")]
    public AudioClip CoinSfx;
    public AudioClip DiamondSfx;
    AudioSource Ad_Source = null;

    FireCtrl m_FireCtrl = null;
    public GameObject bloodEffect;  //���� ȿ�� ������

    //## ��ȣ�� ����
    float m_ShieldDur= 20.0f;
    float m_ShieldOnTime = 0.0f;
    public GameObject m_ShieldObj = null;


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        moveSpeed = 7.0f;   //�̵��ӵ� �ʱ�ȭ
        m_GravitySpeed = (m_JumpPower + (-m_VelocityY)) / m_GReturnTime;
        //m_GReturnTime�� ���� m_JumpPower�� �� ��Ƹ԰� (-m_VelocityY)�� �����ؾ� �ϴ� �ӵ�

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
        if(a_PMesh != null)
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

        if(m_ChrCtrl != null)
        {
            //���͸� ���� ��ǥ�� ���ؿ��� ���� ��ǥ�� �������� ��ȯ�Ѵ�.
            //����󿡼� ĳ���Ͱ� �̵��ؾ� �ϴ� ���� ���ͷ� ȯ����
            moveDir = transform.TransformDirection(moveDir);
            moveDir = moveDir * moveSpeed;

            if (m_ChrCtrl.isGrounded == true) //�߹ٴ��� �ٴڿ� ����� ��
            {
                if (Input.GetKeyDown(KeyCode.Space) == true)
                {
                    m_VelocityY = m_JumpPower;
                    //m_CanDoubleJump = true;
                }
            }

            if (-12.0f < m_VelocityY)
                m_VelocityY -= m_GravitySpeed * Time.deltaTime;

            moveDir.y = m_VelocityY;

            //ĳ���Ϳ� �߷��� ����Ǵ� �̵��Լ�
            //m_ChrCtrl.SimpleMove(moveDir * moveSpeed);
            m_ChrCtrl.Move(moveDir * Time.deltaTime);
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
        if(v >= 0.01f)
        { //���� �ִϸ��̼�
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if(v <= -0.01f)
        { //���� �ִϸ��̼�
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if(h >= 0.01f)
        { //������ �̵� �ִϸ��̼�
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else if(h <= -0.01f)
        { //���� �̵� �ִϸ��̼�
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else
        { //���� idle �ִϸ��̼�
            _animation.CrossFade(anim.idle.name, 0.3f);
        }


        //## ��ȣ�� ������Ʈ
        SheidlUpdate();
    }

    //�浹�� Collider�� IsTrigger �ɼ��� üũ���� �� �߻�
    void OnTriggerEnter(Collider coll)
    {
        //�浹�� Collider�� ������ PUNCH�̸� Player�� HP ����
        if(coll.gameObject.tag == "PUNCH")
        {
            //## ����� ����
            if(0.0f < m_ShieldOnTime)
                return;

            //## ���ó���� ����
            if (hp <= 0.0f)
                return;

            hp -= 10;

            //Image UI �׸��� fillAmount �Ӽ��� ������ ���� ������ �� ����
            imgHpbar.fillAmount = (float)hp / (float)initHp;

            //Debug.Log("Player HP = " + hp.ToString());

            //Player�� ������ 0�����̸� ��� ó��
            if(hp <= 0)
            {
                PlayerDie();
            }
        }
        else if(coll.gameObject.name.Contains("CoinPrefab") == true)
        {
            int a_CacGold = 10;

            GameMgr.Inst.AddGold(a_CacGold);

            if (Ad_Source != null && CoinSfx != null)
                Ad_Source.PlayOneShot(CoinSfx, 0.3f);

            Destroy(coll.gameObject);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "E_BULLET")
        {
            //--���� ȿ�� ����
            GameObject blood = (GameObject)Instantiate(bloodEffect,
                                        coll.transform.position, Quaternion.identity);

            blood.GetComponent<ParticleSystem>().Play();
            Destroy(blood, 3.0f);
            //--���� ȿ�� ����

            Destroy(coll.gameObject); //E_BULLET ����

            if (hp <= 0.0f)
                return;

            hp -= 10;

            if (imgHpbar == null)
                imgHpbar = GameObject.Find("Hp_Image").GetComponent<Image>();

            if (imgHpbar != null)
                imgHpbar.fillAmount = (float)hp / (float)initHp;

            if(hp <= 0)
            {
                PlayerDie();
            }
        }//if(coll.gameObject.tag == "E_BULLET")

    }//void OnCollisionEnter(Collision coll)

    //Player�� ��� ó�� ��ƾ
    void PlayerDie()
    {
        //Debug.Log("Player Die !!");

        //MONSTER��� Tag�� ���� ��� ���ӿ�����Ʈ�� ã�ƿ�
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");

        //��� ������ OnPlayerDie �Լ��� ���������� ȣ��
        foreach(GameObject monster in monsters)
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


    //## ��ȣ�� ������Ʈ
    void SheidlUpdate()
    {
        //## ��ȣ�� ON_OFF
        if (0.0f < m_ShieldOnTime)
        {
            m_ShieldOnTime -= Time.deltaTime;

            if (m_ShieldObj != null && m_ShieldObj.activeSelf == false)
                m_ShieldObj.SetActive(true);
        }
        else
        {
            if(m_ShieldObj != null && m_ShieldObj.activeSelf == true)
                m_ShieldObj.SetActive(false);
        }
    }


    public void UseSkill_Item(SkillType a_SkType)
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;

        if (a_SkType == SkillType.Skill_0)  //30% ���� ������ ��ų
        {
            //�Ӹ��� �ؽ�Ʈ ����
            GameMgr.Inst.SpawnHealText((int)(initHp * 0.3f), transform.position, Color.white);

            hp += (int)(initHp * 0.3f);


            if (initHp < hp)
                hp = initHp;

            if(imgHpbar != null)
                imgHpbar.fillAmount = hp / (float)initHp;


        }
        else if(a_SkType == SkillType.Skill_1)  //����ź
        {
            if (m_FireCtrl != null)
                m_FireCtrl.FireGrenade();
        }
        else if(a_SkType == SkillType.Skill_2)
        {
            if (0.0f < m_ShieldOnTime)
                return;

            m_ShieldOnTime = m_ShieldDur;
           
            GameMgr.Inst.SkillTimeMethod(m_ShieldOnTime,m_ShieldDur);

        }

        int a_SkIdx = (int)a_SkType;
        GlobalValue.g_SkillCount[a_SkIdx]--;
        string a_MkKey = "SkillCount" + a_SkIdx.ToString();
        PlayerPrefs.SetInt(a_MkKey, GlobalValue.g_SkillCount[a_SkIdx]);

    }//public void UseSkill_Item(SkillType a_SkType)
}
