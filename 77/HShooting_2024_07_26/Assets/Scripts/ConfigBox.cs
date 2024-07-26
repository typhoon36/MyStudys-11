using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigBox : MonoBehaviour
{
    public delegate void CFG_Response();    //<--- 델리게이트 데이터(옵션)형 하나 선언
    public CFG_Response DltMethod = null;   //<--- 델리게이트 변수 선언(소켓 역할)

    public Button m_Ok_Btn = null;
    public Button m_Close_Btn = null;

    public InputField NickInputField = null;

    public Toggle m_Sound_Toggle = null;
    public Slider m_Sound_Slider = null;

    HeroCtrl m_RefHero = null;

    // Start is called before the first frame update
    void Start()
    {
        if (m_Ok_Btn != null)
            m_Ok_Btn.onClick.AddListener(OkBtnClick);

        if (m_Close_Btn != null)
            m_Close_Btn.onClick.AddListener(CloseBtnClick);

        if (m_Sound_Toggle != null) 
            m_Sound_Toggle.onValueChanged.AddListener(SoundOnOff);
        //체크 상태가 변경되었을 때 호출되는 함수를 대기하는 코드

        if (m_Sound_Slider != null)
            m_Sound_Slider.onValueChanged.AddListener(SliderChanged);
        //슬라이드 상태가 변경 되었을 때 호출되는 함수 대기하는 코드

        m_RefHero = FindObjectOfType<HeroCtrl>();
        //Hierarchy쪽에서 HeroCtrl 컴포넌트가 붙어있는 게임오브젝트를 찾아서 객체를 찾아오는 방법

        //--- 체크상태, 슬라이드상태, 닉네임 로딩 후 UI 컨트롤에 적용
        int a_SoundOnOff = PlayerPrefs.GetInt("SoundOnOff", 1);
        if(m_Sound_Toggle != null)
        {
            //if (a_SoundOnOff == 1)
            //    m_Sound_Toggle.isOn = true;
            //else
            //    m_Sound_Toggle.isOn = false;

            m_Sound_Toggle.isOn = (a_SoundOnOff == 1) ? true : false;   
        }

        if (m_Sound_Slider != null)
            m_Sound_Slider.value = PlayerPrefs.GetFloat("SoundVolume", 1.0f);

        //Text a_Placehoder = null;
        if (NickInputField != null)
        {
            //Transform a_PLHTr = NickInputField.transform.Find("Placeholder");
            //a_Placehoder = a_PLHTr.GetComponent<Text>();
            //if (a_Placehoder != null)
            //    a_Placehoder.text = PlayerPrefs.GetString("UserNick", "사냥꾼");
            NickInputField.text = PlayerPrefs.GetString("NickName", "SBS영웅");
        }
        //--- 체크상태, 슬라이드상태, 닉네임 로딩 후 UI 컨트롤에 적용

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OkBtnClick()
    {
        //--- 닉네임 주인공 머리위에 적용
        if (NickInputField != null)
        {
            string a_NickStr = NickInputField.text;
            a_NickStr = a_NickStr.Trim();   //앞뒤 공백을 제거해 주는 함수
            if (string.IsNullOrEmpty(a_NickStr) == true)
            {
                Debug.Log("별명은 빈칸 없이 입력해 주세요.");
                return;
            }

            if ((2 <= a_NickStr.Length && a_NickStr.Length < 16) == false)
            {
                Debug.Log("별명은 2글자 이상 15글자 이하로 작성해 주세요.");
                return;
            }

            GlobalValue.g_NickName = a_NickStr;
            PlayerPrefs.SetString("NickName", a_NickStr);

            if (DltMethod != null)
                DltMethod();

        }//if(NickInputField != null)
        //--- 닉네임 주인공 머리위에 적용

        Time.timeScale = 1.0f;  //일시정지 풀어주기
        Destroy(gameObject);
    }

    private void CloseBtnClick()
    {
        Time.timeScale = 1.0f;  //일시정지 풀어주기
        Destroy(gameObject);
    }

    private void SoundOnOff(bool value) //체크 상태가 변경되었을 때 호출되는 함수
    {
        //--- 체크 상태 저장
        //if (value == true)
        //    PlayerPrefs.SetInt("SoundOnOff", 1);
        //else
        //    PlayerPrefs.SetInt("SoundOnOff", 0);

        int a_IntV = (value == true) ? 1 : 0;
        PlayerPrefs.SetInt("SoundOnOff", a_IntV);

        Sound_Mgr.Inst.SoundOnOff(value);    //사운드 켜 / 꺼
        //--- 체크 상태 저장
    }

    private void SliderChanged(float value) 
    { //value 0.0f ~ 1.0f 슬라이드 상태가 변경 되었을 때 호출되는 함수
        PlayerPrefs.SetFloat("SoundVolume", value);
        Sound_Mgr.Inst.SoundVolume(value);
    }
}
