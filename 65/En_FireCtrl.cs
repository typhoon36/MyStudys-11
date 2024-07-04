using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class En_FireCtrl : MonoBehaviour
{
    public GameObject E_bulletPrefab; // ÃÑ¾Ë ÇÁ¸®ÆÕ
    public Transform firePos; // ÃÑ¾Ë ¹ß»ç À§Ä¡

    float fireTimer = 0.0f;

    void Start()
    {
    }

    // ÃÑ¾Ë ¹ß»ç
    public void Fire()
    {
        GameObject bullet = Instantiate(E_bulletPrefab, firePos.position, firePos.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(firePos.forward * 1000.0f); // ÃÑ¾Ë ¹ß»ç ¼Óµµ ¼³Á¤
    }
}
