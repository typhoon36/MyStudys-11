using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//반드시 필요한 컴포넌트를 명시해 해당 컴포넌트가 삭제되는 것을 방지하는 Attribute
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    //총알 프리팹
    public GameObject bullet;
    //총알 발사좌표
    public Transform firePos;

    float fireTimer = 0.0f;

    //충알 발사 사운드
    public AudioClip fireSfx;
    //AudioSource 컴포넌트를 저장할 변수
    private AudioSource source = null;
    //MuzzleFlash의 MeshRenderer 컴포넌트 연결 변수
    public MeshRenderer muzzleFlash;


    //# Laser Pointer 
    public GameObject LaserPointer = null;
    LayerMask m_LaserMask = -1;

    //# 수류탄 투척
    public GameObject grenade;




    // Start is called before the first frame update
    void Start()
    {
        //AudioSource 컴포넌트를 추출한 후 변수에 할당
        source = GetComponent<AudioSource>();
        //최초에 MuzzleFlash MeshRenderer를 비활성화
        muzzleFlash.enabled = false;

        m_LaserMask = 1 << LayerMask.NameToLayer("BULLET");
        m_LaserMask |= 1 << LayerMask.NameToLayer("E_BULLET");
        m_LaserMask = ~m_LaserMask;

    }

    // Update is called once per frame
    void Update()
    {
        if (GameMgr.s_GameState == GameState.GameEnd)
            return;

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0.0f)
        {
            fireTimer = 0.0f;

            //마우스 왼쪽 버튼을 클릭했을 때 Fire 함수 호출
            if (Input.GetMouseButton(0))
            {
                if (GameMgr.IsPointerOverUIObject() == false)
                {
                    Fire();
                }

                fireTimer = 0.11f;
            }
        }//if (fireTimer <= 0.0f)

        #region ## laser pointer display
        if (Physics.Raycast(firePos.position, FollowCam.m_RDir.normalized, out RaycastHit hit, 60.0f, m_LaserMask) == true)
        {
            //### 거리 지정
            float a_Dist = Vector3.Distance(firePos.position, hit.point);
            //### 스케일 조정
            float a_CurScale = a_Dist * 0.3f;
            if (a_CurScale > 5.0f)

                a_CurScale = 5.0f;

            if (a_CurScale < 1.5f)

                a_CurScale = 1.5f;


            //### 오프셋 조정
            float a_CurOffset = a_Dist / 30.0f;
            if (a_CurOffset > 0.1f)
                a_CurOffset = 0.1f;
            if (0.5f < a_CurOffset)
                a_CurOffset = 0.5f;

            //### 위치 조정
            LaserPointer.transform.position = hit.point + (-FollowCam.m_RDir.normalized * a_CurOffset);

            //### 스케일 조정
            LaserPointer.transform.localScale = new Vector3(a_CurScale, a_CurScale, a_CurScale);

            //### 빌보드 처리
            LaserPointer.transform.forward = Camera.main.transform.forward;


        }
        else //60m 이상 장애물시 
        {
            LaserPointer.transform.position = firePos.position +
                FollowCam.m_RDir.normalized * 59.0f;

            LaserPointer.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);


            //### 빌보드 처리
            LaserPointer.transform.forward = Camera.main.transform.forward;
        }




        #endregion


    }

    //# 수류탄 투척
    public void F_Grenade()
    {
        GameObject a_Grenade = Instantiate(grenade, firePos.position, firePos.rotation);

        if (a_Grenade != null)
        {
            Grenade_Ctrl a_Grd = a_Grenade.GetComponent<Grenade_Ctrl>();

            if (a_Grd != null)
            {
                a_Grd.SetFowardDir(FollowCam.m_RDir.normalized);
            }
        }
    }


 


    void Fire()
    {
        //if (GameMgr.IsPointerOverUIObject() == true)
        //    return;

        //동적으로 총알을 생성하는 함수
        CreateBullet();
        //사운드 발생 함수
        source.PlayOneShot(fireSfx, 0.9f);
        //잠시 기다리는 루틴을 위해 코루틴 함수로 호출
        StartCoroutine(this.ShowMuzzleFlash());
    }

    void CreateBullet()
    {
        //Bullet 프리팹을 동적으로 생성
        Instantiate(bullet, firePos.position, firePos.rotation);
    }

    //MuzzleFlash 활성 / 비활성화를 짧은 시간 동안 반복
    IEnumerator ShowMuzzleFlash()
    {
        //MuzzleFlash 스케일을 불규칙하게 변경
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        //MuzzleFlash를 Z축을 기준으로 불규칙하게 회전시킴
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360.0f));
        muzzleFlash.transform.localRotation = rot;

        //활성화해서 보이게 함
        muzzleFlash.enabled = true;

        //불규칙적인 시간 동안 Delay한 다음 MeshRenderer를 비활성화
        yield return new WaitForSeconds(Random.Range(0.01f, 0.03f));

        //비활성화해서 보이지 않게 함
        muzzleFlash.enabled = false;

    }//IEnumerator ShowMuzzleFlash()
}
