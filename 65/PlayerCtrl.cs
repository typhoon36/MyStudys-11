using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


    //## 중력& 점프 변수
    float m_GRtTime = 0.69f;
    float m_GSpeed = 36.2f;
    float m_VelY = -12.0f; //중력 최대
    float m_JumpPw = 13.0f; //점프 강도


    //## 더블점프 
    // bool m_DJump = false;


    //회전 속도 변수
    public float rotSpeed = 100.0f;
    Vector3 m_CacVec = Vector3.zero;
    //인스펙터뷰에 표시할 애니메이션 클래스 변수
    public Anim anim;

    //아래에 있는 3D 모델의 Animation 컴포넌트에 접근하기 위한 변수
    public Animation _animation;

    //Player의 생명 변수
    public int hp = 100;
    //Plyaer의 생명 초기값
    private int initHp;
    //Player의 Health bar 이미지
    public Image imgHpbar;

    CharacterController m_ChrCtrl;  //현재 캐릭터가 가지고 있는 캐릭터 컨트롤러 참조 변수

    //## 총알 스크립트
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

        moveSpeed = 7.0f;   //이동속도 초기화

        m_GSpeed = (m_JumpPw + (-m_VelY)) / m_GRtTime;

        //생명 초기값 설정
        initHp = hp;

        //자신의 하위에 있는 Animation 컴포넌트를 찾아와 변수에 할당
        _animation = GetComponentInChildren<Animation>();

        //Animation 컴포넌트의 애니메이션 클립을 지정하고 실행
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

        //전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        if (1.0f < moveDir.magnitude)
            moveDir.Normalize();

        ////Translate(이동방향 * Time.deltaTime * 변위값 * 속도, 기준좌표)
        //transform.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);
        ////기준좌표 옵션의 기본값은 Space relativeTo = Space.Self

        if (m_ChrCtrl != null)
        {
            //벡터를 로컬 좌표계 기준에서 월드 좌표계 기준으로 변환한다.
            //월드상에서 캐릭터가 이동해야 하는 방향 벡터로 환산함
            moveDir = transform.TransformDirection(moveDir);

            moveDir = moveDir * moveSpeed;

            //### 바닥인 상태
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

            //캐릭터에 중력이 적용되는 이동함수
            // m_ChrCtrl.SimpleMove(moveDir * moveSpeed);

            m_ChrCtrl.Move(moveDir*Time.deltaTime);
        }

        if (Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
            if (GameMgr.IsPointerOverUIObject() == false)
            {
                //Vector3.up 축을 기준으로 rotSpeed만큼의 속도로 회전
                transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X") * 3.0f);
                //transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxisRaw("Mouse X") * 3.0f);
                //m_CacVec = transform.eulerAngles;
                //m_CacVec.y += (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse X") * 10.0f);
                //transform.eulerAngles = m_CacVec;
            }

        //키보드 입력값을 기준으로 동작할 애니메이션 수행
        if (v >= 0.01f)
        { //전진 애니메이션
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if (v <= -0.01f)
        { //후진 애니메이션
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if (h >= 0.01f)
        { //오른쪽 이동 애니메이션
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else if (h <= -0.01f)
        { //왼쪽 이동 애니메이션
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else
        { //정지 idle 애니메이션
            _animation.CrossFade(anim.idle.name, 0.3f);
        }
    }

    //충돌한 Collider의 IsTrigger 옵션이 체크됐을 때 발생
    void OnTriggerEnter(Collider coll)
    {
        //충돌한 Collider가 몬스터의 PUNCH이면 Player의 HP 차감
        if (coll.gameObject.tag == "PUNCH")
        {
            hp -= 10;

            //Image UI 항목의 fillAmount 속성을 조절해 생명 게이지 값 조절
            imgHpbar.fillAmount = (float)hp / (float)initHp;

            //Debug.Log("Player HP = " + hp.ToString());

            //Player의 생명이 0이하이면 사망 처리
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

    //Player의 사망 처리 루틴
    void PlayerDie()
    {
        //Debug.Log("Player Die !!");

        //MONSTER라는 Tag를 가진 모든 게임오브젝트를 찾아옴
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");

        //모든 몬스터의 OnPlayerDie 함수를 순차적으로 호출
        foreach (GameObject monster in monsters)
        {
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
            //MonsterCtrl a_MonCtrl = monster.GetComponent<MonsterCtrl>();
            //a_MonCtrl.OnPlayerDie();
        }

        _animation.Stop();  //애니메이터 컴포넌트의 애니메이션 중지 함수

        GameMgr.s_GameState = GameState.GameEnd;
        //GameMgr의 싱글턴 인스턴스를 접근해 isGameOver 변숫값을 변경
        GameMgr.Inst.isGameOver = true;

    }//void PlayerDie()

    //# 스킬 사용
    public void UseSkill(SkillType a_SkType)
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;
        //## 30% 힐링
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


        //## 수류탄
        else if (a_SkType == SkillType.Skill_1)
        {
            if (m_FireCtrl != null)
            {
                m_FireCtrl.F_Grenade();
            }



        }

    }



}
