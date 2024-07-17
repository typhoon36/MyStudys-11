using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class Game_Mgr : MonoBehaviour
{
    [Header("Buttons")]
    public Button StensilBuff_Btn;
    public Button GrayShade_Btn;
    public Button RedShade_Btn;
    public Button PostProcess_Btn;

    [Header("Sliders")]
    public Slider Post_Slider;
    public Slider GrayShade_Slider;
    float m_AlphaVal = 1.0f;

    bool IsStensilBuff = false;
    bool IsRedShade = false;

    Material material_1;
    Material material_2;

    //## 회색음영
    Material[] mts;
    GameObject m_Chr = null;

    SkinnedMeshRenderer[] m_SMRList = null;
    bool IsGrayShade = false;

    public static Shader g_DShader = null;
    public static Shader g_MyGrayShader = null;


    //## Post Processing
     bool m_IsPostProcess = false;


    //## 색
    Color m_color;
    

    //## 총구 
    GameObject m_Gun = null;
    Material[] mts2;




    void Start()
    {
        // Resources 폴더 내의 Materials 폴더에서 AlwaysVisible_Mtrl 머티리얼 로드
        material_1 = Resources.Load<Material>("Materials/AlwaysVisible_Mtrl");

        material_2 = Resources.Load<Material>("Materials/Mob_Red_Mtrl");


        g_DShader = Shader.Find("Standard");

        g_MyGrayShader = Shader.Find("Custom/MyGrayTransparent");

     
        m_color = new Color(1.0f, 0.0f, 0.0f, 1.0f);

        m_Chr = GameObject.Find("Player");

        m_Gun = GameObject.Find("main_weapon001");

        if (m_Chr != null)
            m_SMRList = m_Chr.GetComponentsInChildren<SkinnedMeshRenderer>();

        //## StensilBuff
        StensilBuff_Btn.onClick.AddListener(() =>
        {
            if (material_1 != null)
            {
                IsStensilBuff = !IsStensilBuff;
                material_1.color = IsStensilBuff ? Color.red : Color.white;
            }
        });

        //## RedShade
        RedShade_Btn.onClick.AddListener(() =>
        {
            if (material_2 != null)
            {
                IsRedShade = !IsRedShade;
                material_2.color = IsRedShade ? Color.red : Color.white;
            }
        });



        //## Post Processing
        if (PostProcess_Btn != null)
            PostProcess_Btn.onClick.AddListener(PostProcess_Click);

        if (m_IsPostProcess == true)
        {
            Camera.main.GetComponent<PostProcessVolume>().enabled = true;
        }
        else
        {
            Camera.main.GetComponent<PostProcessVolume>().enabled = false;
        }

        if (Post_Slider != null)
        {
            Bloom bloomlayer = null;
            PostProcessVolume a_Volume = Camera.main.GetComponent<PostProcessVolume>();
            a_Volume.profile.TryGetSettings(out bloomlayer);
            Post_Slider.onValueChanged.AddListener(On_PostPocess);
            Post_Slider.value = bloomlayer.intensity.value / 30.0f;
        }


        //## GrayShade
        if(GrayShade_Btn != null)
        GrayShade_Btn.onClick.AddListener(ChangeGray_Click);
      
        //## 알파값 조절
        if (GrayShade_Slider != null)
        {
            GrayShade_Slider.onValueChanged.AddListener(OnAlphaSlide);
            GrayShade_Slider.value = m_AlphaVal;
        }

    }

    void Update()
    {

    }


    void OnAlphaSlide(float value)
    {
        m_AlphaVal = value;

        if (IsGrayShade == true)
        {
            // 캐릭터의 알파값 조정
            for (int i = 0; i < m_SMRList.Length; i++)
            {
                Material[] mts = m_SMRList[i].materials;

                for (int a = 0; a < mts.Length; a++)
                {
                    mts[a].SetFloat("_AlphaValue", m_AlphaVal);
                }
            }

            // 총구의 알파값 조정
            if (m_Gun != null)
            {
                Renderer gunRenderer = m_Gun.GetComponent<Renderer>();
                if (gunRenderer != null)
                {
                    Material[] gunMaterials = gunRenderer.materials;
                    for (int i = 0; i < gunMaterials.Length; i++)
                    {
                        gunMaterials[i].SetFloat("_AlphaValue", m_AlphaVal);
                    }
                }
            }
        }
    }




    void ChangeGray_Click()
    {
        IsGrayShade = !IsGrayShade;

        //## 회색음영 적용
        if (IsGrayShade == true)
        {
            Time.timeScale = 0.0f;
            for (int i = 0; i < m_SMRList.Length; i++)
            {
                Material[] mts = m_SMRList[i].materials;

                for (int a = 0; a < mts.Length; a++)
                {
                    if (Game_Mgr.g_MyGrayShader != null && mts[a].shader != Game_Mgr.g_MyGrayShader)
                        mts[a].shader = Game_Mgr.g_MyGrayShader;
                }
            }

            // 총구에도 회색음영 적용
            if (m_Gun != null)
            {
                Renderer gunRenderer = m_Gun.GetComponent<Renderer>();
                if (gunRenderer != null)
                {
                    Material[] gunMaterials = gunRenderer.materials;
                    for (int i = 0; i < gunMaterials.Length; i++)
                    {
                        if (Game_Mgr.g_MyGrayShader != null && gunMaterials[i].shader != Game_Mgr.g_MyGrayShader)
                            gunMaterials[i].shader = Game_Mgr.g_MyGrayShader;
                    }
                }
            }
        }
        //## 음영 적용 x -- reset
        else
        {
            Time.timeScale = 1.0f;
            for (int i = 0; i < m_SMRList.Length; i++)
            {
                Material[] mts = m_SMRList[i].materials;

                for (int a = 0; a < mts.Length; a++)
                {
                    if (Game_Mgr.g_DShader != null && mts[a].shader != Game_Mgr.g_DShader)
                        mts[a].shader = Game_Mgr.g_DShader;
                }
            }

            // 총구의 음영도 reset
            if (m_Gun != null)
            {
                Renderer gunRenderer = m_Gun.GetComponent<Renderer>();
                if (gunRenderer != null)
                {
                    Material[] gunMaterials = gunRenderer.materials;
                    for (int i = 0; i < gunMaterials.Length; i++)
                    {
                        if (Game_Mgr.g_DShader != null && gunMaterials[i].shader != Game_Mgr.g_DShader)
                            gunMaterials[i].shader = Game_Mgr.g_DShader;
                    }
                }
            }
        }
    }


  

    

    //## Post Processing button
    void PostProcess_Click()
    {
        m_IsPostProcess = !m_IsPostProcess;

        if (m_IsPostProcess == true)
        {
            //## Post Processing 적용
            Camera.main.GetComponent<PostProcessVolume>().enabled = true;

        }
        else
        {

            Camera.main.GetComponent<PostProcessVolume>().enabled = false;
        }
    }


    //## Post Processing Slider
    void On_PostPocess(float value)
    {
        Bloom bloomlayer = null;

        PostProcessVolume a_Volume = Camera.main.GetComponent<PostProcessVolume>();
        a_Volume.profile.TryGetSettings(out bloomlayer);
        bloomlayer.intensity.value = value * 30.0f;

    }




}
