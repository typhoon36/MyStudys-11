using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkInven_Node : MonoBehaviour
{
    [HideInInspector] public SkillType m_skillType;
    [HideInInspector] public Text m_SkCount_Txt;




    // Start is called before the first frame update
    void Start()
    {

        Button a_BtnCom = this.GetComponentInChildren<Button>();
        if (a_BtnCom != null)
            a_BtnCom.onClick.AddListener(() =>
            {
                if (GlobalValue.g_SkillCount[(int)m_skillType] <= 0)
                    return;

                PlayerCtrl a_Py = GameObject.FindObjectOfType<PlayerCtrl>();


                if (a_Py != null)
                    a_Py.UseSkill_Item(m_skillType);

                if (m_SkCount_Txt != null)
                    m_SkCount_Txt.text = GlobalValue.g_SkillCount[(int)m_skillType].ToString();

            });

    }

    //# √ ±‚»≠ 
    public void InitState(SkillType a_SkType)
    {
        m_skillType = a_SkType;
        m_SkCount_Txt = GetComponentInChildren<Text>();

        if (m_SkCount_Txt != null)
            m_SkCount_Txt.text = GlobalValue.g_SkillCount[(int)m_skillType].ToString();

    }

}
