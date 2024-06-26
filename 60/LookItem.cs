using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookItem : MonoBehaviour
{
    public void OnLookItemBox(bool isLookAt)
    {
        Debug.Log("LookItem : " + isLookAt);
        Move_Ctrl.isStopped = isLookAt;
    }
}
