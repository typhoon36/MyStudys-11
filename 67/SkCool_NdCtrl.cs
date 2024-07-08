using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkCool_NdCtrl : MonoBehaviour
{
    [HideInInspector] public SkillType m_SkType;
    float Skill_Time = 0.0f;
    float Skill_Delay = 0.0f;
    public Image Time_Img = null;
    public Image Icon_Img = null;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Skill_Time -= Time.deltaTime;
        Time_Img.fillAmount = Skill_Time / Skill_Delay;
        
        if(Skill_Time <= 0.0f)
           Destroy(gameObject);


        

    }

    public void InitState(float a_Time , float a_Delay)
    {
        Skill_Time = a_Time;
        Skill_Delay = a_Delay;
    }


}
