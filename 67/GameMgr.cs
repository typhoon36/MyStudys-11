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

    //## ��Ÿ��
    [Header("skill cool time")]
    public GameObject m_SkCool_Prefab = null;
    public Transform m_SkCool_RT = null;
    //### ��ų �κ� ��ư ����
    public SkInven_Node[] m_SkInvenND;

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

        if(BackBtn != null)
            BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Lobby");
            });

        // Hierachy ���� SpawnPoint�� ã�� ������ �ִ� ��� Transform ������Ʈ�� ã�ƿ�
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        //���͸� ������ ������Ʈ Ǯ�� ����
        for(int i = 0; i < maxMonster; i++)
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

        if(points.Length > 0)
        {
            //���� ���� �ڷ�ƾ �Լ� ȣ��
            StartCoroutine(this.CreateMonster());
        }

        m_CoinItem = Resources.Load("CoinItem/CoinPrefab") as GameObject;

        m_RefHero = GameObject.FindObjectOfType<PlayerCtrl>();

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        //���콺 �߾ӹ�ư(�� Ŭ��)
        if(Input.GetMouseButtonDown(2) == true)
        {
            UseSkill_Key(SkillType.Skill_1);
        }

        //--- ����Ű �̿����� ��ų ����ϱ�...
        if(Input.GetKeyDown(KeyCode.Alpha1) ||
            Input.GetKeyDown(KeyCode.Keypad1))
        {
            UseSkill_Key(SkillType.Skill_0);    //30% ���� ������ ��ų
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2) ||
            Input.GetKeyDown(KeyCode.Keypad2))
        {
            UseSkill_Key(SkillType.Skill_1);    //����ź ���
        }

        else if (Input.GetKeyDown(KeyCode.Alpha3) ||
               Input.GetKeyDown(KeyCode.Keypad3))
        {
            UseSkill_Key(SkillType.Skill_2);    //��ȣ�� ���
        }

            //--- ����Ű �̿����� ��ų ����ϱ�...

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

    //���� ���� �� ȭ�� ǥ��
    public void DispScore(int score)
    {
        totScore += score;
        txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";
    }

    //���� ���� �ڷ�ƾ �Լ�
    IEnumerator CreateMonster()
    {
        //���� ���� �ñ��� ���� ����
        while( !isGameOver )
        {
            //���� ���� �ֱ� �ð���ŭ ���� ������ �纸
            yield return new WaitForSeconds(createTime);

            //�÷��̾ ������� �� �ڷ�ƾ�� ������ ���� ��ƾ�� �������� ����
            if (GameMgr.s_GameState == GameState.GameEnd) 
                yield break; //�ڷ�ƾ �Լ����� �Լ��� ���������� ���

            //������Ʈ Ǯ�� ó������ ������ ��ȸ
            foreach(GameObject monster in monsterPool)
            {
                //��Ȱ��ȭ ���η� ��� ������ ���͸� �Ǵ�
                if(monster.activeSelf == false)
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

        for(int i= 0; i < GlobalValue.g_SkillCount.Length; i++)
        {
            if (m_SkInvenND.Length <= i)
                continue;
            m_SkInvenND[i].InitState((SkillType)i);
        }
       
    }

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

    //## ��ų��Ÿ�� 
    public void SkillTimeMethod(float a_Time , float a_Dur)
    {
        GameObject a_Obj = Instantiate(m_SkCool_Prefab) as GameObject;
        a_Obj.transform.SetParent(m_SkCool_RT, false);
        SkCool_NdCtrl a_SkCool = a_Obj.GetComponent<SkCool_NdCtrl>();
        a_SkCool.InitState(a_Time, a_Dur);

    }


}//public class GameMgr : MonoBehaviour
