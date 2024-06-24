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

    public AnimationClip RunF;
    public AnimationClip RunB;
    public AnimationClip RunR;
    public AnimationClip RunL;
    public AnimationClip Idle;

}

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;

    //�̵� �ӵ� ����
    public float moveSpeed = 10.0f;

    //ȸ�� �ӵ� ����
    public float rotSpeed = 100.0f;
    Vector3 m_CacVec = Vector3.zero;

    //�ν����ͺ信 ǥ���� �ִϸ��̼� Ŭ���� ����
    public Anim anim;
    public Anim anim_2;

    //�Ʒ��� �ִ� 3D ���� Animation ������Ʈ�� �����ϱ� ���� ����
    public Animation _animation;
    public Animation _animation_2;

    //Player�� ���� ����
    public int hp = 100;
    //Plyaer�� ���� �ʱⰪ
    private int initHp;
    //Player�� Health bar �̹���
    public Image imgHpbar;


    //## �ι�° �÷��̾�
    public GameObject Player2;
    //## ù��° �÷��̾�
    public GameObject Player1;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        //���� �ʱⰪ ����
        initHp = hp;

        //�ڽ��� ������ �ִ� Animation ������Ʈ�� ã�ƿ� ������ �Ҵ�
        _animation = Player1.GetComponentInChildren<Animation>();
        _animation_2 = Player2.GetComponentInChildren<Animation>();

        //Animation ������Ʈ�� �ִϸ��̼� Ŭ���� �����ϰ� ����
        if (Player1.activeSelf)
        {
            _animation.clip = anim.idle;
            _animation.Play();
        }
        else if(Player2.activeSelf)
        {
            _animation_2.clip = anim_2.Idle;
            _animation_2.Play();
        }


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

        //Translate(�̵����� * Time.deltaTime * ������ * �ӵ�, ������ǥ)
        transform.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);
        //������ǥ �ɼ��� �⺻���� Space relativeTo = Space.Self

        if (Input.GetKeyDown(KeyCode.C))
        {
            bool isPlayer1Active = Player1.activeSelf;
            Player1.SetActive(!isPlayer1Active);
            Player2.SetActive(isPlayer1Active);

            if (isPlayer1Active)
            {
                _animation_2.clip = anim_2.Idle;
                _animation_2.Play();
            }
            else
            {
                _animation.clip = anim.idle;
                _animation.Play();
            }
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
            _animation_2.CrossFade("RunF", 0.3f);
        }
        else if (v <= -0.01f)
        { //���� �ִϸ��̼�
            _animation.CrossFade(anim.runBackward.name, 0.3f);
            _animation_2.CrossFade("RunB", 0.3f);
        }
        else if (h >= 0.01f)
        { //������ �̵� �ִϸ��̼�
            _animation.CrossFade(anim.runRight.name, 0.3f);
            _animation_2.CrossFade("RunR", 0.3f);
        }
        else if (h <= -0.01f)
        { //���� �̵� �ִϸ��̼�
            _animation.CrossFade(anim.runLeft.name, 0.3f);
            _animation_2.CrossFade("RunL", 0.3f);
        }
        else
        { //���� idle �ִϸ��̼�
            _animation.CrossFade(anim.idle.name, 0.3f);
            _animation_2.CrossFade("Idle", 0.3f);
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
}
