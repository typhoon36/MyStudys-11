using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class Game_Mgr : MonoBehaviour
{
    public Button TakeDmg_Btn = null;

    public Button ChrImgChange_Btn = null;
    
    public Button ChangeGray_Btn = null;


    //## ť�� 
    GameObject Cube_Unlit_Trans = null;
    //### ť���� �޽������� �迭
    MeshRenderer[] m_CubeMRList = null;

    //### �⺻ ���̴�(ť��)
    public static Shader g_CubeDShader = null;
    public static Shader g_MyTexAddColorShader = null;


    Color m_Color;
    float m_DmgTimer = 0.0f;
    bool m_IsImg = true;

    //## ĳ���� ����
    GameObject m_Chr = null;
    SkinnedMeshRenderer[] m_SMRList = null;

    public Slider Alpha_Slide;
    float m_AlphaVal = 1.0f;

    public static Shader g_DShader = null;
    public static Shader g_MyGrayShader = null;

    bool m_IsGray = false;

    //## Post Processing
    public Button PostProcess_Btn = null;   
    public Slider PostProcess_Slider = null;
    bool m_IsPostProcess = false;

    // Start is called before the first frame update
    void Start()
    {
        //## ť�� ã��
        Cube_Unlit_Trans = GameObject.Find("Unlit_Trans_Cube (1)");

        //## �޽������� ã��
        if (Cube_Unlit_Trans != null)
            m_CubeMRList = Cube_Unlit_Trans.GetComponentsInChildren<MeshRenderer>();

        //## �⺻ ���̴� ã��
        //g_CubeDShader = Shader.Find("Unlit/Transparent");//��1

        //### ��� 2
        if (m_CubeMRList != null && 0 < m_CubeMRList.Length)
            g_CubeDShader = m_CubeMRList[0].material.shader;

        //### ���� ���� ���̴� ã��
        g_MyTexAddColorShader = Shader.Find("Custom/MyTextAddColor");
        // resources folder �� ����

        //## �ʱ� ����
        m_Color = new Color(1.0f, 0.0f, 0.0f, 1.0f);



        //## buttons
        if (TakeDmg_Btn != null)
            TakeDmg_Btn.onClick.AddListener(TakeDmg_Click);

        if (ChrImgChange_Btn != null)
            ChrImgChange_Btn.onClick.AddListener(ChrChange_Click);

        if (ChangeGray_Btn != null)
            ChangeGray_Btn.onClick.AddListener(ChangeGray_Click);


        //## ĳ���� ã��
        m_Chr = GameObject.Find("Pc_Jojo_Skin_01 (1)");

        if (m_Chr != null)
            m_SMRList = m_Chr.GetComponentsInChildren<SkinnedMeshRenderer>();

        if(m_SMRList != null && 0 < m_SMRList.Length)
        
            g_DShader = m_SMRList[0].material.shader;
         
        g_MyGrayShader = Shader.Find("Custom/MyGrayTransparent");
        
        //## �����̴�
        if (Alpha_Slide != null)
        {
            Alpha_Slide.onValueChanged.AddListener(OnAlphaSlide);
            Alpha_Slide.value = m_AlphaVal;
        }

        //## Post Processing
        if (PostProcess_Btn != null)
            PostProcess_Btn.onClick.AddListener(PostProcess_Click);

        if(m_IsPostProcess == true)
        {
            Camera.main.GetComponent<PostProcessVolume>().enabled = true;
        }
        else
        {
            Camera.main.GetComponent<PostProcessVolume>().enabled = false;
        }

        if(PostProcess_Slider != null)
        {
            Bloom bloomlayer = null;
            PostProcessVolume a_Volume = Camera.main.GetComponent<PostProcessVolume>();
            a_Volume.profile.TryGetSettings(out bloomlayer);
            PostProcess_Slider.onValueChanged.AddListener(On_PostPocess);
            PostProcess_Slider.value = bloomlayer.intensity.value / 30.0f;
        }



    }


    // Update is called once per frame
    void Update()
    {
        UpdateTakDmg();
    }


    void OnAlphaSlide(float value)
    {
        m_AlphaVal = value;

        if(m_IsGray == true)
        {
            Material[] mts;
            for(int i = 0; i < m_SMRList.Length; i++)
            {
                mts = m_SMRList[i].materials;

                for(int a = 0; a < mts.Length; a++)
                {
                    mts[a].SetFloat("_AlphaValue", m_AlphaVal);
                }
            }
        }
    }



    void ChrChange_Click()
    {
        m_IsImg = !m_IsImg;

        Texture a_TmpSprite = null;
        if (m_IsImg == true)
            a_TmpSprite = Resources.Load("Images/m0423") as Texture;
        else
            a_TmpSprite = Resources.Load("Images/m0367") as Texture;

        if (m_CubeMRList !=null)
        {
            for (int i = 0; i < m_CubeMRList.Length; i++)
            {
                m_CubeMRList[i].material.SetTexture("_MainTex", a_TmpSprite);
                //m_CubeMRList[i].material.mainTexture = a_TmpSprite;
            }
        }



    }

    void ChangeGray_Click()
    {
        m_IsGray = !m_IsGray;

        //## ȸ������ ����
        if(m_IsGray == true)
        {
            Material[] mts;
            for(int i = 0; i < m_SMRList.Length; i++)
            {
                mts = m_SMRList[i].materials;

                for(int a = 0; a < mts.Length; a++)
                {
                    if(Game_Mgr.g_MyGrayShader != null && mts[a].shader != Game_Mgr.g_MyGrayShader)
                        mts[a].shader = Game_Mgr.g_MyGrayShader;
                    
                }
            }
        }
        //## ���� ���� x -- reset
        else
        {
            Material[] mts;
            for (int i = 0; i < m_SMRList.Length; i++)
            {
                mts = m_SMRList[i].materials;

                for (int a = 0; a < mts.Length; a++)
                {
                    if (Game_Mgr.g_DShader != null && mts[a].shader != Game_Mgr.g_DShader)
                        mts[a].shader = Game_Mgr.g_DShader;

                }
            }
        }


    }


    void TakeDmg_Click()
    {
        m_DmgTimer = 0.15f;

        //## ���̴� ����
        //for (int i = 0; i < m_CubeMRList.Length; i++)
        //{
        //    if (g_MyTexAddColorShader != null)
        //    {
        //        if (m_CubeMRList[i].material.shader != g_MyTexAddColorShader)
        //            m_CubeMRList[i].material.shader = g_MyTexAddColorShader;

        //        m_CubeMRList[i].material.SetColor("_AddColor", m_Color);

        //    }

        //}

        //## material ������ shader ��ü
        Material a_Mtrl = Resources.Load("Materials/MyTexAddColorMtrl") as Material;

        if (a_Mtrl != null)
        {
            if (a_Mtrl.shader != Game_Mgr.g_MyTexAddColorShader)
                a_Mtrl.shader = Game_Mgr.g_MyTexAddColorShader;

            a_Mtrl.SetColor("_AddColor", m_Color);
        }


    }

    void UpdateTakDmg()
    {
        if (0 < m_DmgTimer)
        {

            m_DmgTimer -= Time.deltaTime;

            if (m_DmgTimer <= 0)
            {

                //## ���̴� ����
                //for (int i = 0; i < m_CubeMRList.Length; i++)
                //{
                //    if (g_CubeDShader != null && m_CubeMRList[i].material.shader != g_CubeDShader)
                //    {

                //        m_CubeMRList[i].material.shader = g_CubeDShader;
                //    }
                //}

                //## material ������ shader ��ü
                Material a_Mtrl = Resources.Load("Materials/MyTexAddColorMtrl") as Material;

                if (a_Mtrl != null)
                {
                    if (g_CubeDShader !=null && a_Mtrl.shader != Game_Mgr.g_CubeDShader)
                        a_Mtrl.shader = Game_Mgr.g_CubeDShader;
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
            //## Post Processing ����
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
