using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//# 클래스 애니메이션 추가

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



    //## 이동 속도
    public float moveSpeed = 10.0f;


    //## 회전 속도
    public float rotSpeed = 100.0f;

    //## 애니메이션 클래스 변수
    public Anim anim;

    //##3d모델의 애니메이션 컴포넌트 접근.
    public Animation _animation;


    


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        QualitySettings.vSyncCount = 0;

        //## 애니메이션 컴포넌트 찾고 할당
        _animation = GetComponentInChildren<Animation>();

        //### 애니메이션 컴포넌트 지정 클립 실행
        _animation.clip = anim.Idle;

        _animation.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //## 플레이어 이동
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //## 전후좌우 이동 벡터
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        if (1.0f <= moveDir.magnitude)
            moveDir.Normalize();



        //### translate(이동) 함수를 이용한 이동

        transform.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);


        //## 제한(임시)
        if (Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
        {
            //## rotate(회전) 함수를 이용한 회전

            transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X") * 3.0f);

          

        }
      

            //## 키보드 입력 값에 따라 애니메이션 변경

            //전진
            if ( v >= 0.1f)
        {
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }

        //후진
        else if(v <= -0.1f)
        {
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }

        //오른쪽 이동
        else if(h >= 0.1f)
        {
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }


        //왼쪽 이동
        else if(h <= -0.1f)
        {
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }


        //정지(대기)
        else
        {
            _animation.CrossFade(anim.Idle.name, 0.3f);
        }


    
    }
}
