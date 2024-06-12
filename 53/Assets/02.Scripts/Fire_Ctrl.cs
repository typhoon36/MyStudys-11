using System.Collections;
using System.Collections.Generic;
using UnityEngine;





//# ������Ʈ ���� ���� �Ӽ�
[RequireComponent(typeof(AudioSource))]
public class Fire_Ctrl : MonoBehaviour
{
    //## �Ѿ� 

    public GameObject bullet;


    //�Ѿ� �߻���ġ
    public Transform firePos;

    //## ������
    float fireTime = 0.0f;

    //## �Ѿ� ����
    public AudioClip fireSfx;

    //## ����� ������Ʈ
    AudioSource audioSource = null;

    //## muzzleFlash
    public MeshRenderer muzzleFlash;



    // Start is called before the first frame update
    void Start()
    {
       audioSource = GetComponent<AudioSource>();
    
        //## muzzleFlash ��Ȱ��ȭ
        muzzleFlash.enabled = false;
    
    }

    // Update is called once per frame
    void Update()
    {
        //## �ֱ� �߻�
        fireTime -= Time.deltaTime;

        if (fireTime <= 0.0f)
        {
            fireTime = 0.0f;

            //## �Է�
            if (Input.GetMouseButton(0))
            {
                Fire();

                fireTime = 0.11f;
            }
        }

    }

    void Fire()
    {
        //���� �޼��� ȣ��
        CreateBullet();

        //���� ���
        audioSource.PlayOneShot(fireSfx, 0.9f) ;

        //## �ڷ�ƾ 
        StartCoroutine(ShowMuzzleFlash());
    }

    void CreateBullet()
    {
        //�Ѿ� ����
        Instantiate(bullet, firePos.position, firePos.rotation);
    }

    IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.enabled= true;

        //## �ð����� ��Ȱ��ȭ
        yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));

        //## ��Ȱ��ȭ
        muzzleFlash.enabled = false;
    }




}
