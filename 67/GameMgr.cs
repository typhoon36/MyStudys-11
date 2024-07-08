using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    GameIng,
    GameEnd
}

public class GameMgr : MonoBehaviour
{
    public static GameState s_GameState = GameState.GameIng;

    //Text UI 항목 연결을 위한 변수
    public Text txtScore;
    //누적 점수를 기록하기 위한 변수
    private int totScore = 0;

    public Button BackBtn;

    //몬스터가 출현할 위치를 담을 배열
    public Transform[] points;
    //몬스터 프리팹을 할당할 변수
    public GameObject monsterPrefab;
    //몬스터를 미리 생성해 저장할 리스트 자료형
    public List<GameObject> monsterPool = new List<GameObject>();

    //몬스터를 발생시킬 주기
    public float createTime = 2.0f;
    //몬스터의 최대 발생 개수
    public int maxMonster = 10;
    //게임 종료 여부 변수
    public bool isGameOver = false;

    [HideInInspector] public GameObject m_CoinItem = null;
    [Header("--- Gold UI ---")]
    public Text m_UserGoldText = null;  //이번 스테이지에서 얻은 골드값 표시 UI
    int m_CurGold = 0;

    //--- 머리 위에 힐텍스트 띄우기용 변수 선언
    [Header("--- HealText ---")]
    public Transform m_Heal_Canvas = null;
    public GameObject m_HTextPrefab = null;
    //--- 머리 위에 힐텍스트 띄우기용 변수 선언

    //## 쿨타임
    [Header("skill cool time")]
    public GameObject m_SkCool_Prefab = null;
    public Transform m_SkCool_RT = null;
    //### 스킬 인벤 버튼 연결
    public SkInven_Node[] m_SkInvenND;

    PlayerCtrl m_RefHero = null;

    //--- 싱글톤 패턴
    public static GameMgr Inst = null;

    void Awake()
    {
        Inst = this;   
    }
    //--- 싱글톤 패턴

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        s_GameState = GameState.GameIng;
        GlobalValue.LoadGameData();
        RefreshGameUI();

        DispScore(0);

        if(BackBtn != null)
            BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Lobby");
            });

        // Hierachy 뷰의 SpawnPoint를 찾아 하위에 있는 모든 Transform 컴포넌트를 찾아옴
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        //몬스터를 생성해 오브젝트 풀에 저장
        for(int i = 0; i < maxMonster; i++)
        {
            //몬스터 프리팹을 생성
            GameObject monster = (GameObject)Instantiate(monsterPrefab);
            //생성한 몬스터의 이름 설정
            monster.name = "Monster_" + i.ToString();
            //생성한 몬스터를 비활성화
            monster.SetActive(false);
            //생성한 몬스터를 오브젝트 풀에 추가
            monsterPool.Add(monster);
        }

        if(points.Length > 0)
        {
            //몬스터 생성 코루틴 함수 호출
            StartCoroutine(this.CreateMonster());
        }

        m_CoinItem = Resources.Load("CoinItem/CoinPrefab") as GameObject;

        m_RefHero = GameObject.FindObjectOfType<PlayerCtrl>();

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        //마우스 중앙버튼(휠 클릭)
        if(Input.GetMouseButtonDown(2) == true)
        {
            UseSkill_Key(SkillType.Skill_1);
        }

        //--- 단축키 이용으로 스킬 사용하기...
        if(Input.GetKeyDown(KeyCode.Alpha1) ||
            Input.GetKeyDown(KeyCode.Keypad1))
        {
            UseSkill_Key(SkillType.Skill_0);    //30% 힐링 아이템 스킬
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2) ||
            Input.GetKeyDown(KeyCode.Keypad2))
        {
            UseSkill_Key(SkillType.Skill_1);    //수류탄 사용
        }

        else if (Input.GetKeyDown(KeyCode.Alpha3) ||
               Input.GetKeyDown(KeyCode.Keypad3))
        {
            UseSkill_Key(SkillType.Skill_2);    //보호막 사용
        }

            //--- 단축키 이용으로 스킬 사용하기...

        //    if (Input.GetMouseButton(1)== true)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}
        //else if(Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //}


    }//void Update()

    //점수 누적 및 화면 표시
    public void DispScore(int score)
    {
        totScore += score;
        txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";
    }

    //몬스터 생성 코루틴 함수
    IEnumerator CreateMonster()
    {
        //게임 종료 시까지 무한 루프
        while( !isGameOver )
        {
            //몬스터 생성 주기 시간만큼 메인 루프에 양보
            yield return new WaitForSeconds(createTime);

            //플레이어가 사망했을 때 코루틴을 종료해 다음 루틴을 진행하지 않음
            if (GameMgr.s_GameState == GameState.GameEnd) 
                yield break; //코루틴 함수에서 함수를 빠져나가는 명령

            //오브젝트 풀의 처음부터 끝까지 순회
            foreach(GameObject monster in monsterPool)
            {
                //비활성화 여부로 사용 가능한 몬스터를 판단
                if(monster.activeSelf == false)
                {
                    //몬스터를 출현시킬 위치의 인덱스값을 추출
                    int idx = Random.Range(1, points.Length);
                    //몬스터의 출현위치를 설정
                    monster.transform.position = points[idx].position;
                    //몬스터를 활성화함
                    monster.SetActive(true);

                    //오브젝트 풀에서 몬스터 프리팹 하나를 활성화한 후 for 루프를 빠져나감
                    break;
                }//if(monster.activeSelf == false)
            }//foreach(GameObject monster in monsterPool)
        }// while( !isGameOver )

    }//IEnumerator CreateMonster()

    public void AddGold(int value = 10)
    {
        //이번 스테이지에서 얻은 골드값
        if(value < 0)
        {
            m_CurGold += value;
            if (m_CurGold < 0)
                m_CurGold = 0;
        }
        else if (m_CurGold <= int.MaxValue - value)
            m_CurGold += value;
        else
            m_CurGold = int.MaxValue;

        //로컬에 저장되어 있는 유저 보유 골드값
        if (value < 0)
        {
            GlobalValue.g_UserGold += value;
            if (GlobalValue.g_UserGold < 0)
                GlobalValue.g_UserGold = 0;
        }
        else if (GlobalValue.g_UserGold <= int.MaxValue - value)
            GlobalValue.g_UserGold += value;
        else
            GlobalValue.g_UserGold = int.MaxValue;

        if (m_UserGoldText != null)
            m_UserGoldText.text = "Gold <color=#ffff00>" + GlobalValue.g_UserGold + "</color>";

        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);

    }//public void AddGold(int value = 10)

    public void SpawnCoin(Vector3 a_Pos)
    {
        GameObject a_CoinObj = Instantiate(m_CoinItem);
        a_CoinObj.transform.position = a_Pos;
        Destroy(a_CoinObj, 10.0f);      //10초 내에 먹어야 한다.
    }

    void RefreshGameUI()
    {
        if (m_UserGoldText != null)
            m_UserGoldText.text = "Gold <color=#ffff00>" + GlobalValue.g_UserGold + "</color>";

        for(int i= 0; i < GlobalValue.g_SkillCount.Length; i++)
        {
            if (m_SkInvenND.Length <= i)
                continue;
            m_SkInvenND[i].InitState((SkillType)i);
        }
       
    }

    public static bool IsPointerOverUIObject() //UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
    {
        PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)

			List<RaycastResult> results = new List<RaycastResult>();
			for (int i = 0; i < Input.touchCount; ++i)
			{
				a_EDCurPos.position = Input.GetTouch(i).position;  
				results.Clear();
				EventSystem.current.RaycastAll(a_EDCurPos, results);
                if (0 < results.Count)
                    return true;
			}

			return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }//public bool IsPointerOverUIObject()

    public void UseSkill_Key(SkillType a_SkType)
    {
        if (GlobalValue.g_SkillCount[(int)a_SkType] <= 0)
            return;

        if (m_RefHero != null)
            m_RefHero.UseSkill_Item(a_SkType);

        if(m_SkInvenND.Length >= (int)a_SkType)
            m_SkInvenND[(int)a_SkType].m_SkCount_Txt.text =
                GlobalValue.g_SkillCount[(int)a_SkType].ToString();
    }

    public void SpawnHealText(int cont, Vector3 a_WSpawnPos, Color a_Color)
    {
        if(m_Heal_Canvas == null && m_HTextPrefab == null)
            return;

        GameObject a_HealObj = Instantiate(m_HTextPrefab) as GameObject;
        HealTextCtrl a_HealText = a_HealObj.GetComponent<HealTextCtrl>();
        if(a_HealText != null)
           a_HealText.InitState(cont, a_WSpawnPos, m_Heal_Canvas, a_Color);        
    }

    //## 스킬쿨타임 
    public void SkillTimeMethod(float a_Time , float a_Dur)
    {
        GameObject a_Obj = Instantiate(m_SkCool_Prefab) as GameObject;
        a_Obj.transform.SetParent(m_SkCool_RT, false);
        SkCool_NdCtrl a_SkCool = a_Obj.GetComponent<SkCool_NdCtrl>();
        a_SkCool.InitState(a_Time, a_Dur);

    }


}//public class GameMgr : MonoBehaviour
