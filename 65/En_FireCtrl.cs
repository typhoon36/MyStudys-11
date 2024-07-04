using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class En_FireCtrl : MonoBehaviour
{
    public GameObject E_bulletPrefab; // �Ѿ� ������
    public Transform firePos; // �Ѿ� �߻� ��ġ

    float fireTimer = 0.0f;

    void Start()
    {
    }

    // �Ѿ� �߻�
    public void Fire()
    {
        GameObject bullet = Instantiate(E_bulletPrefab, firePos.position, firePos.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(firePos.forward * 1000.0f); // �Ѿ� �߻� �ӵ� ����
    }
}
