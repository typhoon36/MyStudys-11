using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//클래스에 System.Serializable 이라는 어트리뷰트(Attribute)를 명시해야
//Inspector 뷰에 노출됨
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

    //이동 속도 변수
    public float moveSpeed = 10.0f;

    //회전 속도 변수
    public float rotSpeed = 100.0f;
    Vector3 m_CacVec = Vector3.zero;

    //인스펙터뷰에 표시할 애니메이션 클래스 변수
    public Anim anim;

    //아래에 있는 3D 모델의 Animation 컴포넌트에 접근하기 위한 변수
    public Animation _animation;

    //## 플레이어의 생명
    public int hp = 100;

    public bool isDie = false;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        //자신의 하위에 있는 Animation 컴포넌트를 찾아와 변수에 할당
        _animation = GetComponentInChildren<Animation>();

        //Animation 컴포넌트의 애니메이션 클립을 지정하고 실행
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

     

        //전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        if (1.0f < moveDir.magnitude)
            moveDir.Normalize();

        //Translate(이동방향 * Time.deltaTime * 변위값 * 속도, 기준좌표)
        transform.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);
        //기준좌표 옵션의 기본값은 Space relativeTo = Space.Self

        if (Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
        {
            //Vector3.up 축을 기준으로 rotSpeed만큼의 속도로 회전
            transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X") * 3.0f);
            //transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxisRaw("Mouse X") * 3.0f);
            //m_CacVec = transform.eulerAngles;
            //m_CacVec.y += (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse X") * 10.0f);
            //transform.eulerAngles = m_CacVec;
        }

        //키보드 입력값을 기준으로 동작할 애니메이션 수행
        if(v >= 0.01f)
        { //전진 애니메이션
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if(v <= -0.01f)
        { //후진 애니메이션
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if(h >= 0.01f)
        { //오른쪽 이동 애니메이션
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else if(h <= -0.01f)
        { //왼쪽 이동 애니메이션
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else
        { //정지 idle 애니메이션
            _animation.CrossFade(anim.idle.name, 0.3f);
        }
        

     
    
    }

    //## 충돌처리
    void OnTriggerEnter(Collider coll)
    {

        if (coll.gameObject.tag == "Punch")
        {
           


            //## 피격 처리
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


        //## 플레이어 사망 처리
        Debug.Log("Player Die!!");

        //## 애니메이션 처리
        _animation.Stop();

        //### 몬스터 태그 게임오브젝트 모두 찾아오기
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");


        //### 순차적으로 몬스터들에게 플레이어 사망을 알림
        foreach (GameObject monster in monsters)
        {
            
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        }



    }



}
