using System.Collections;
using System.Collections.Generic;
using UnityEngine;





//# 컴포넌트 삭제 방지 속성
[RequireComponent(typeof(AudioSource))]
public class Fire_Ctrl : MonoBehaviour
{
    //## 총알 

    public GameObject bullet;


    //총알 발사위치
    public Transform firePos;

    //## 딜레이
    float fireTime = 0.0f;

    //## 총알 사운드
    public AudioClip fireSfx;

    //## 오디오 컴포넌트
    AudioSource audioSource = null;

    //## muzzleFlash
    public MeshRenderer muzzleFlash;

    //## 플레이어 참조
    PlayerCtrl playerCtrl;

    // Start is called before the first frame update
    void Start()
    {
       audioSource = GetComponent<AudioSource>();
    
        //## muzzleFlash 비활성화
        muzzleFlash.enabled = false;
    

        playerCtrl = GameObject.FindWithTag("Player").GetComponent<PlayerCtrl>();

    }

    // Update is called once per frame
    void Update()
    {
        //## 주기 발생
        fireTime -= Time.deltaTime;

        // 플레이어가 사망한 경우 총 발사 입력 무시
        if (playerCtrl.isDie)
        {
            return;
        }

        if (fireTime <= 0.0f)
        {
            fireTime = 0.0f;

            //## 입력
            if (Input.GetMouseButton(0))
            {
                Fire();

                fireTime = 0.11f;
            }
        }

    }
    
    void Fire()
    {
        //생성 메서드 호출
        CreateBullet();

        //사운드 재생
        audioSource.PlayOneShot(fireSfx, 0.9f) ;

        //## 코루틴 
        StartCoroutine(ShowMuzzleFlash());
    }

    void CreateBullet()
    {
        //총알 생성
        Instantiate(bullet, firePos.position, firePos.rotation);
    }

    IEnumerator ShowMuzzleFlash()
    {
        //##스케일 변경
        float scale = Random.Range(1.0f, 2.0f);


        muzzleFlash.transform.localScale = Vector3.one * scale;

        //## z축 회전
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360.0f));

        muzzleFlash.transform.localRotation = rot;

        //## 활성화
            muzzleFlash.enabled= true;

        //## 시간동안 비활성화
        yield return new WaitForSeconds(Random.Range(0.01f, 0.03f));

        //## 비활성화
        muzzleFlash.enabled = false;
    }




}




