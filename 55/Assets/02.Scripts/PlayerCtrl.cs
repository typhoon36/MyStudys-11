using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //ȸ�� �ӵ� ����
    public float rotSpeed = 100.0f;
    Vector3 m_CacVec = Vector3.zero;

    //�ν����ͺ信 ǥ���� �ִϸ��̼� Ŭ���� ����
    public Anim anim;

    //�Ʒ��� �ִ� 3D ���� Animation ������Ʈ�� �����ϱ� ���� ����
    public Animation _animation;

    //## �÷��̾��� ����
    public int hp = 100;

    public bool isDie = false;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        //�ڽ��� ������ �ִ� Animation ������Ʈ�� ã�ƿ� ������ �Ҵ�
        _animation = GetComponentInChildren<Animation>();

        //Animation ������Ʈ�� �ִϸ��̼� Ŭ���� �����ϰ� ����
        _animation.clip = anim.idle;
        _animation.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDie == true)
        {
            return;
        }


        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

     

        //�����¿� �̵� ���� ���� ���
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        if (1.0f < moveDir.magnitude)
            moveDir.Normalize();

        //Translate(�̵����� * Time.deltaTime * ������ * �ӵ�, ������ǥ)
        transform.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);
        //������ǥ �ɼ��� �⺻���� Space relativeTo = Space.Self

        if (Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
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
        

     
    
    }

    //## �浹ó��
    void OnTriggerEnter(Collider coll)
    {

        if (coll.gameObject.tag == "Punch")
        {
           


            //## �ǰ� ó��
            hp -= 10;
            
          
            if (hp <= 0)
            {
                   
                PlayerDie();
            }



        }
    }

    void PlayerDie()
    {
        isDie = true;


        //## �÷��̾� ��� ó��
        Debug.Log("Player Die!!");

        //## �ִϸ��̼� ó��
        _animation.Stop();

        //### ���� �±� ���ӿ�����Ʈ ��� ã�ƿ���
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");


        //### ���������� ���͵鿡�� �÷��̾� ����� �˸�
        foreach (GameObject monster in monsters)
        {
            
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        }



    }



}
