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

    //Text UI �׸� ������ ���� ����
    public Text txtScore;
    //���� ������ ����ϱ� ���� ����
    private int totScore = 0;
    int m_CurScore = 0;     //�̹� ������������ ���� ��������

    public Button BackBtn;

    //���Ͱ� ������ ��ġ�� ���� �迭
    public Transform[] points;
    //���� �������� �Ҵ��� ����
    public GameObject monsterPrefab;
    //���͸� �̸� ������ ������ ����Ʈ �ڷ���
    public List<GameObject> monsterPool = new List<GameObject>();

    //���͸� �߻���ų �ֱ�
    public float createTime = 2.0f;
    //������ �ִ� �߻� ����
    public int maxMonster = 10;
    //���� ���� ���� ����
    public bool isGameOver = false;

    [HideInInspector] public GameObject m_CoinItem = null;
    [Header("--- Gold UI ---")]
    public Text m_UserGoldText = null;  //�̹� ������������ ���� ��尪 ǥ�� UI
    int m_CurGold = 0;

    //--- �Ӹ� ���� ���ؽ�Ʈ ����� ���� ����
    [Header("--- HealText ---")]
    public Transform m_Heal_Canvas = null;
    public GameObject m_HTextPrefab = null;
    //--- �Ӹ� ���� ���ؽ�Ʈ ����� ���� ����

    [Header("--- Skill Cool Timer ---")]
    public GameObject m_SkCoolPrefab = null;
    public Transform m_SkCoolRoot = null;
    public SkInvenNode[] m_SkInvenNode;   //Skill �κ��丮 ��ư ���� ����

    [Header("--- GameOver ---")]
    public GameObject GameOverPanel = null;
    public Text Title_Text = null;
    public Text Result_Text = null;
    public Button Replay_Btn = null;
    public Button RstLobby_Btn = null;

    [Header("--- Door ---")]
    public Text m_FL_Tm_Text = null;
    public Text m_LastFloor_Text = null;
    public Text m_DoorOpenTxt = null;
    float m_FTimeout = 0.0f;
    GameObject[] m_DoorObj = new GameObject[3];
    public static GameObject m_DiaItem = null;


    PlayerCtrl m_RefHero = null;

    //--- �̱��� ����
    public static GameMgr Inst = null;

    void Awake()
    {
        Inst = this;
    }
    //--- �̱��� ����

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        s_GameState = GameState.GameIng;
        GlobalValue.LoadGameData();
        RefreshGameUI();

        DispScore(0);

        if (BackBtn != null)
            BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Lobby");
            });

        // Hierachy ���� SpawnPoint�� ã�� ������ �ִ� ��� Transform ������Ʈ�� ã�ƿ�
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        //���͸� ������ ������Ʈ Ǯ�� ����
        for (int i = 0; i < maxMonster; i++)
        {
            //���� �������� ����
            GameObject monster = (GameObject)Instantiate(monsterPrefab);
            //������ ������ �̸� ����
            monster.name = "Monster_" + i.ToString();
            //������ ���͸� ��Ȱ��ȭ
            monster.SetActive(false);
            //������ ���͸� ������Ʈ Ǯ�� �߰�
            monsterPool.Add(monster);
        }

        if (points.Length > 0)
        {
            //���� ���� �ڷ�ƾ �Լ� ȣ��
            StartCoroutine(this.CreateMonster());
        }

        m_CoinItem = Resources.Load("CoinItem/CoinPrefab") as GameObject;

        //--- GameOver ��ư ó�� �ڵ�
        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("scLevel01");
                SceneManager.LoadScene("scPlay", LoadSceneMode.Additive);
            });

        if (RstLobby_Btn != null)
            RstLobby_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Lobby");
            });
        //--- GameOver ��ư ó�� �ڵ�

        m_RefHero = GameObject.FindObjectOfType<PlayerCtrl>();

        //## �� 
        m_FL_Tm_Text.text = GlobalValue.g_CurFloorNum + "��(����:"
            + GlobalValue.g_BestFloorNum + "F)";
        GameObject a_DoorObj = GameObject.Find("Gate_In_1");

        if (a_DoorObj != null)
            m_DoorObj[0] = a_DoorObj;

        a_DoorObj = GameObject.Find("Gate_Exit_1");
        if (a_DoorObj != null)
        {
            m_DoorObj[1] = a_DoorObj;
            m_DoorObj[1].SetActive(false);
        
        }

        a_DoorObj = GameObject.Find("Gate_Exit_2");
        if (a_DoorObj != null)
        {
            m_DoorObj[2] = a_DoorObj;
            m_DoorObj[2].SetActive(false);
        }

        if(GlobalValue.g_CurFloorNum <= 1)
        
                m_DoorObj[0].SetActive(false);
        
        if(GlobalValue.g_CurFloorNum < GlobalValue.g_BestFloor)
        {
            ShowDoor();
        }
    


    }//void Start()

    // Update is called once per frame
    void Update()
    {
        //���콺 �߾ӹ�ư(�� Ŭ��)
        if (Input.GetMouseButtonDown(2) == true)
        {
            UseSkill_Key(SkillType.Skill_1);
        }

        //--- ����Ű �̿����� ��ų ����ϱ�...
        if (Input.GetKeyDown(KeyCode.Alpha1) ||
            Input.GetKeyDown(KeyCode.Keypad1))
        {
            UseSkill_Key(SkillType.Skill_0);    //30% ���� ������ ��ų
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) ||
                Input.GetKeyDown(KeyCode.Keypad2))
        {
            UseSkill_Key(SkillType.Skill_1);    //����ź ���
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) ||
                Input.GetKeyDown(KeyCode.Keypad3))
        {
            UseSkill_Key(SkillType.Skill_2);    //��ȣ�� �ߵ�
        }
        //--- ����Ű �̿����� ��ų ����ϱ�...

    }//void Update()

    //## �� �����ֱ�
    public void ShowDoor()
    {
        int a_Idx = (GlobalValue.g_CurFloorNum % 2) + 1;
    
        if((1 <= a_Idx) && (a_Idx < 2) && m_DoorObj[a_Idx] !=null)
        
            m_DoorObj[a_Idx].SetActive(true);
        
        if(m_LastFloor_Text != null)
            m_LastFloor_Text.gameObject.SetActive(false);

        if(m_DoorOpenTxt != null)
            m_DoorOpenTxt.gameObject.SetActive(true);
    
    }




    //���� ���� �� ȭ�� ǥ��
    public void DispScore(int score)
    {
        //totScore += score;
        //txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";

        m_CurScore += score;
        if (m_CurScore < 0)
            m_CurScore = 0;

        if (score < 0)
        {
            GlobalValue.g_BestScore += score;
            if (GlobalValue.g_BestScore < 0)
                GlobalValue.g_BestScore = 0;
        }
        else if (GlobalValue.g_BestScore <= int.MaxValue - score)
        {
            GlobalValue.g_BestScore += score;
        }
        else
        {
            GlobalValue.g_BestScore = int.MaxValue;
        }

        txtScore.text = "SCORE <color=#ff0000>" + m_CurScore.ToString() +
                "</color> / BEST <color=#ff0000>" +
                GlobalValue.g_BestScore.ToString() + "</color>";

        //txtScore.text = "SCORE <color=#ff0000>" + m_CurScore.ToString("0000") +
        //                "</color> / BEST <color=#ff0000>" + 
        //                GlobalValue.g_BestScore.ToString("0000") + "</color>";

        PlayerPrefs.SetInt("BestScore", GlobalValue.g_BestScore);
    }

    //���� ���� �ڷ�ƾ �Լ�
    IEnumerator CreateMonster()
    {
        //���� ���� �ñ��� ���� ����
        while (!isGameOver)
        {
            //���� ���� �ֱ� �ð���ŭ ���� ������ �纸
            yield return new WaitForSeconds(createTime);

            //�÷��̾ ������� �� �ڷ�ƾ�� ������ ���� ��ƾ�� �������� ����
            if (GameMgr.s_GameState == GameState.GameEnd)
                yield break; //�ڷ�ƾ �Լ����� �Լ��� ���������� ���

            //������Ʈ Ǯ�� ó������ ������ ��ȸ
            foreach (GameObject monster in monsterPool)
            {
                //��Ȱ��ȭ ���η� ��� ������ ���͸� �Ǵ�
                if (monster.activeSelf == false)
                {
                    //���͸� ������ų ��ġ�� �ε������� ����
                    int idx = Random.Range(1, points.Length);
                    //������ ������ġ�� ����
                    monster.transform.position = points[idx].position;
                    //���͸� Ȱ��ȭ��
                    monster.SetActive(true);

                    //������Ʈ Ǯ���� ���� ������ �ϳ��� Ȱ��ȭ�� �� for ������ ��������
                    break;
                }//if(monster.activeSelf == false)
            }//foreach(GameObject monster in monsterPool)
        }// while( !isGameOver )

    }//IEnumerator CreateMonster()

    public void AddGold(int value = 10)
    {
        //�̹� ������������ ���� ��尪
        if (value < 0)
        {
            m_CurGold += value;
            if (m_CurGold < 0)
                m_CurGold = 0;
        }
        else if (m_CurGold <= int.MaxValue - value)
            m_CurGold += value;
        else
            m_CurGold = int.MaxValue;

        //���ÿ� ����Ǿ� �ִ� ���� ���� ��尪
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
        Destroy(a_CoinObj, 10.0f);      //10�� ���� �Ծ�� �Ѵ�.
    }

    void RefreshGameUI()
    {
        if (m_UserGoldText != null)
            m_UserGoldText.text = "Gold <color=#ffff00>" + GlobalValue.g_UserGold + "</color>";

        for (int i = 0; i < GlobalValue.g_SkillCount.Length; i++)
        {
            if (m_SkInvenNode.Length <= i)
                continue;

            m_SkInvenNode[i].InitState((SkillType)i);

        }//for (int i = 0; i < GlobalValue.g_SkillCount.Length; i++)

    }//void RefreshGameUI()

    public static bool IsPointerOverUIObject() //UGUI�� UI���� ���� ��ŷ�Ǵ��� Ȯ���ϴ� �Լ�
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

        if ((int)a_SkType < m_SkInvenNode.Length)
            m_SkInvenNode[(int)a_SkType].m_SkCountText.text =
                   GlobalValue.g_SkillCount[(int)a_SkType].ToString();
    }

    public void SpawnHealText(int cont, Vector3 a_WSpawnPos, Color a_Color)
    {
        if (m_Heal_Canvas == null && m_HTextPrefab == null)
            return;

        GameObject a_HealObj = Instantiate(m_HTextPrefab) as GameObject;
        HealTextCtrl a_HealText = a_HealObj.GetComponent<HealTextCtrl>();
        if (a_HealText != null)
            a_HealText.InitState(cont, a_WSpawnPos, m_Heal_Canvas, a_Color);
    }

    public void SkillTimeMethod(float a_Time, float a_Dur)
    {
        GameObject obj = Instantiate(m_SkCoolPrefab);
        obj.transform.SetParent(m_SkCoolRoot, false);
        SkCool_NodeCtrl skNode = obj.GetComponent<SkCool_NodeCtrl>();
        skNode.InitState(a_Time, a_Dur);
    }

    public void GameOverMethod()
    {
        GameOverPanel.SetActive(true);
        Result_Text.text = "NickName\n" + GlobalValue.g_NickName + "\n\n" +
                            "ȹ�� ����\n" + m_CurScore + "\n\n" +
                            "ȹ�� ���\n" + m_CurGold;
    }

}//public class GameMgr : MonoBehaviour
