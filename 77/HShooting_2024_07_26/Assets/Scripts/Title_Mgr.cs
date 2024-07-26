using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class Title_Mgr : MonoBehaviour
{
    public Button StartBtn;

    [Header("LoginPanel")]
    public GameObject m_LoginPanel;
    public InputField IdInputField;     //Email 로 받을 것임
    public InputField PassInputField;
    public Button m_LoginBtn;
    public Button m_CreateAccOpenBtn;

    [Header("CreateAccountPanel")]
    public GameObject m_CreateAccPanel;
    public InputField New_IdInputField;
    public InputField New_PassInputField;
    public InputField New_NickInputField;
    public Button m_CreateAccountBtn;
    public Button m_CancelBtn;

    [Header("Normal")]
    public Text MessageText;
    float ShowMsTimer = 0.0f;

    bool invalidEmailType = false;       // 이메일 포맷이 올바른지 체크
    bool isValidFormat = false;          // 올바른 형식인지 아닌지 체크

    // Start is called before the first frame update
    void Start()
    {
        StartBtn.onClick.AddListener(StartClick);

        //--- LoginPanel
        if (m_LoginBtn != null)
            m_LoginBtn.onClick.AddListener(LoginBtn);

        if (m_CreateAccOpenBtn != null)
            m_CreateAccOpenBtn.onClick.AddListener(OpenCreateAccBtn);

        //--- CreateAccountPanel
        if (m_CancelBtn != null)
            m_CancelBtn.onClick.AddListener(CreateCancelBtn);

        if (m_CreateAccountBtn != null)
            m_CreateAccountBtn.onClick.AddListener(CreateAccountBtn);

        Sound_Mgr.Inst.PlayBGM("sound_bgm_title_001", 1.0f);
        Sound_Mgr.Inst.m_AudioSrc.clip = null;  //배경음 플레이 끄기
    }

    void Update()
    {
        MessageUpdate();
    }

    void StartClick()
    {
        bool IsFadeOk = false;
        if (Fade_Mgr.Inst != null)
            IsFadeOk = Fade_Mgr.Inst.SceneOutReserve("LobbyScene");
        if (IsFadeOk == false)
            SceneManager.LoadScene("LobbyScene");

        Sound_Mgr.Inst.PlayGUISound("Pop", 1.0f);
    }

    void LoginBtn()
    {
        string a_IdStr = IdInputField.text;
        string a_PwStr = PassInputField.text;

        a_IdStr = a_IdStr.Trim();
        a_PwStr = a_PwStr.Trim();

        if (string.IsNullOrEmpty(a_IdStr) == true ||
           string.IsNullOrEmpty(a_PwStr) == true)
        {
            ShowMessage("Id, Pw 빈칸 없이 입력해 주세요.");
            return;
        }

        if (!(6 <= a_IdStr.Length && a_IdStr.Length <= 20))  // 6 ~ 20
        {
            ShowMessage("Id는 6글자부터 20글자까지 작성해 주세요.");
            return;
        }

        if (!(6 <= a_PwStr.Length && a_PwStr.Length <= 20))
        {
            ShowMessage("비밀번호는 6글자부터 20글자까지 작성해 주세요.");
            return;
        }

        if (!CheckEmailAddress(a_IdStr))
        {
            ShowMessage("Email 형식이 맞지 않습니다.");
            return;
        }

        var option = new GetPlayerCombinedInfoRequestParams()
        {
            GetPlayerProfile = true,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
            }
        };

        var request = new LoginWithEmailAddressRequest()
        {
            Email = a_IdStr,
            Password = a_PwStr,
            InfoRequestParameters = option
        };

        PlayFabClientAPI.LoginWithEmailAddress(request,
                                    OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        ShowMessage("로그인 성공"+Color.green);

        if (result.InfoResultPayload != null)
        {
            GlobalValue.g_NickName = result.InfoResultPayload.PlayerProfile.DisplayName;
        }

        bool IsFadeOk = false;
        if (Fade_Mgr.Inst != null)
            IsFadeOk = Fade_Mgr.Inst.SceneOutReserve("LobbyScene");
        if (IsFadeOk == false)
            SceneManager.LoadScene("LobbyScene");
    }

    void OnLoginFailure(PlayFabError error)
    {
        if (error.GenerateErrorReport().Contains("User not found") == true)
        {
            ShowMessage("로그인 실패 : 해당 Id가 존재하지 않습니다.");
        }
        else if (error.GenerateErrorReport().Contains("Invalid email address or password") == true)
        {
            ShowMessage("로그인 실패 : 패스워드가 일치하지 않습니다.");
        }
        else
        {
            ShowMessage("로그인 실패 : " + error.GenerateErrorReport());
        }
    }

    void OpenCreateAccBtn()
    {
        if (m_LoginPanel != null)
            m_LoginPanel.SetActive(false);

        if (m_CreateAccPanel != null)
            m_CreateAccPanel.SetActive(true);
    }

    void CreateCancelBtn()
    {
        if (m_LoginPanel != null)
            m_LoginPanel.SetActive(true);

        if (m_CreateAccPanel != null)
            m_CreateAccPanel.SetActive(false);
    }

    void CreateAccountBtn()
    {
        Debug.Log("CreateAccountBtn clicked"); // 디버그 로그 추가
        string a_IdStr = New_IdInputField.text;
        string a_PwStr = New_PassInputField.text;
        string a_NickStr = New_NickInputField.text;

        a_IdStr = a_IdStr.Trim();
        a_PwStr = a_PwStr.Trim();
        a_NickStr = a_NickStr.Trim();

        if (string.IsNullOrEmpty(a_IdStr) == true ||
           string.IsNullOrEmpty(a_PwStr) == true ||
           string.IsNullOrEmpty(a_NickStr) == true)
        {
            ShowMessage("Id, Pw, 별명은 빈칸 없이 입력해 주세요.");
            return;
        }

        if (!(6 <= a_IdStr.Length && a_IdStr.Length <= 20))  // 6 ~ 20
        {
            ShowMessage("Id는 6글자부터 20글자까지 작성해 주세요.");
            return;
        }

        if (!(6 <= a_PwStr.Length && a_PwStr.Length <= 20))
        {
            ShowMessage("비밀번호는 6글자부터 20글자까지 작성해 주세요.");
            return;
        }

        if (!(2 <= a_NickStr.Length && a_NickStr.Length <= 20))
        {
            ShowMessage("별명은 2글자부터 20글자까지 작성해 주세요.");
            return;
        }

        if (!CheckEmailAddress(a_IdStr))
        {
            ShowMessage("Email 형식이 맞지 않습니다.");
            return;
        }

        var request = new RegisterPlayFabUserRequest()
        {
            Email = a_IdStr,
            Password = a_PwStr,
            DisplayName = a_NickStr,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, RegisterSuccess, RegisterFailure);
    }

    void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        ShowMessage("가입 성공" + Color.green);
    }

    void RegisterFailure(PlayFabError error)
    {
        if (error.GenerateErrorReport().Contains("Email address already exists"))
        {
            ShowMessage("가입 실패 : " + "이미 존재하는 Id 입니다.");
        }
        else if (error.GenerateErrorReport().Contains("The display name entered is not available"))
        {
            ShowMessage("가입 실패 : " + "이미 존재하는 별명 입니다.");
        }
        else
        {
            ShowMessage("가입 실패 : " + error.GenerateErrorReport());
        }
    }

    private bool CheckEmailAddress(string EmailStr)
    {
        if (string.IsNullOrEmpty(EmailStr)) isValidFormat = false;

        EmailStr = Regex.Replace(EmailStr, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
        if (invalidEmailType) isValidFormat = false;

        isValidFormat = Regex.IsMatch(EmailStr,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase);
        return isValidFormat;
    }

    private string DomainMapper(Match match)
    {
        IdnMapping idn = new IdnMapping();

        string domainName = match.Groups[2].Value;
        try
        {
            domainName = idn.GetAscii(domainName);
        }
        catch (ArgumentException)
        {
            invalidEmailType = true;
        }
        return match.Groups[1].Value + domainName;
    }

    void ShowMessage(string msg)
    {
        Debug.Log("ShowMessage called with message: " + msg); // 디버그 로그 추가
        MessageText.gameObject.SetActive(true);
        MessageText.text = msg;
        ShowMsTimer = 2.0f;
    }

    void MessageUpdate()
    {
        if (ShowMsTimer > 0)
        {
            ShowMsTimer -= Time.deltaTime;
            if (ShowMsTimer <= 0)
            {
                MessageText.gameObject.SetActive(false);
            }
        }
    }
}

