using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//# Ŭ���� �ִϸ��̼� �߰�

[System.Serializable]
public class  Anim
{
    public AnimationClip Idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
}


public class Player_Ctrl : MonoBehaviour
{
    float h = 0.0f;
    float v = 0.0f;



    //## �̵� �ӵ�
    public float moveSpeed = 10.0f;


    //## ȸ�� �ӵ�
    public float rotSpeed = 100.0f;

    //## �ִϸ��̼� Ŭ���� ����
    public Anim anim;

    //##3d���� �ִϸ��̼� ������Ʈ ����.
    public Animation _animation;


    


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        QualitySettings.vSyncCount = 0;

        //## �ִϸ��̼� ������Ʈ ã�� �Ҵ�
        _animation = GetComponentInChildren<Animation>();

        //### �ִϸ��̼� ������Ʈ ���� Ŭ�� ����
        _animation.clip = anim.Idle;

        _animation.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //## �÷��̾� �̵�
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //## �����¿� �̵� ����
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        if (1.0f <= moveDir.magnitude)
            moveDir.Normalize();



        //### translate(�̵�) �Լ��� �̿��� �̵�

        transform.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);


        //## ����(�ӽ�)
        if (Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
        {
            //## rotate(ȸ��) �Լ��� �̿��� ȸ��

            transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X") * 3.0f);

          

        }
      

            //## Ű���� �Է� ���� ���� �ִϸ��̼� ����

            //����
            if ( v >= 0.1f)
        {
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }

        //����
        else if(v <= -0.1f)
        {
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }

        //������ �̵�
        else if(h >= 0.1f)
        {
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }


        //���� �̵�
        else if(h <= -0.1f)
        {
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }


        //����(���)
        else
        {
            _animation.CrossFade(anim.Idle.name, 0.3f);
        }


    
    }
}
