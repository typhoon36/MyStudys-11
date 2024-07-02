using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    { 
        Destroy(gameObject, 10);

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0.0f, Time.deltaTime * 200.0f, 0.0f);
    }
}
