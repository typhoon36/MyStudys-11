using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ݵ�� �ʿ��� ������Ʈ�� ����� �ش� ������Ʈ�� �����Ǵ� ���� �����ϴ� Attribute
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    //�Ѿ� ������
    public GameObject bullet;
    //�Ѿ� �߻���ǥ
    public Transform firePos;

    float fireTimer = 0.0f;

    //��� �߻� ����
    public AudioClip fireSfx;
    //AudioSource ������Ʈ�� ������ ����
    private AudioSource source = null;
    //MuzzleFlash�� MeshRenderer ������Ʈ ���� ����
    public MeshRenderer muzzleFlash;


    //# Laser Pointer 
    public GameObject LaserPointer = null;
    LayerMask m_LaserMask = -1;

    //# ����ź ��ô
    public GameObject grenade;




    // Start is called before the first frame update
    void Start()
    {
        //AudioSource ������Ʈ�� ������ �� ������ �Ҵ�
        source = GetComponent<AudioSource>();
        //���ʿ� MuzzleFlash MeshRenderer�� ��Ȱ��ȭ
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

            //���콺 ���� ��ư�� Ŭ������ �� Fire �Լ� ȣ��
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
            //### �Ÿ� ����
            float a_Dist = Vector3.Distance(firePos.position, hit.point);
            //### ������ ����
            float a_CurScale = a_Dist * 0.3f;
            if (a_CurScale > 5.0f)

                a_CurScale = 5.0f;

            if (a_CurScale < 1.5f)

                a_CurScale = 1.5f;


            //### ������ ����
            float a_CurOffset = a_Dist / 30.0f;
            if (a_CurOffset > 0.1f)
                a_CurOffset = 0.1f;
            if (0.5f < a_CurOffset)
                a_CurOffset = 0.5f;

            //### ��ġ ����
            LaserPointer.transform.position = hit.point + (-FollowCam.m_RDir.normalized * a_CurOffset);

            //### ������ ����
            LaserPointer.transform.localScale = new Vector3(a_CurScale, a_CurScale, a_CurScale);

            //### ������ ó��
            LaserPointer.transform.forward = Camera.main.transform.forward;


        }
        else //60m �̻� ��ֹ��� 
        {
            LaserPointer.transform.position = firePos.position +
                FollowCam.m_RDir.normalized * 59.0f;

            LaserPointer.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);


            //### ������ ó��
            LaserPointer.transform.forward = Camera.main.transform.forward;
        }




        #endregion


    }

    //# ����ź ��ô
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

        //�������� �Ѿ��� �����ϴ� �Լ�
        CreateBullet();
        //���� �߻� �Լ�
        source.PlayOneShot(fireSfx, 0.9f);
        //��� ��ٸ��� ��ƾ�� ���� �ڷ�ƾ �Լ��� ȣ��
        StartCoroutine(this.ShowMuzzleFlash());
    }

    void CreateBullet()
    {
        //Bullet �������� �������� ����
        Instantiate(bullet, firePos.position, firePos.rotation);
    }

    //MuzzleFlash Ȱ�� / ��Ȱ��ȭ�� ª�� �ð� ���� �ݺ�
    IEnumerator ShowMuzzleFlash()
    {
        //MuzzleFlash �������� �ұ�Ģ�ϰ� ����
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        //MuzzleFlash�� Z���� �������� �ұ�Ģ�ϰ� ȸ����Ŵ
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360.0f));
        muzzleFlash.transform.localRotation = rot;

        //Ȱ��ȭ�ؼ� ���̰� ��
        muzzleFlash.enabled = true;

        //�ұ�Ģ���� �ð� ���� Delay�� ���� MeshRenderer�� ��Ȱ��ȭ
        yield return new WaitForSeconds(Random.Range(0.01f, 0.03f));

        //��Ȱ��ȭ�ؼ� ������ �ʰ� ��
        muzzleFlash.enabled = false;

    }//IEnumerator ShowMuzzleFlash()
}
