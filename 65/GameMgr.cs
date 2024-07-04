using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
    [Header("##### Gold UI ####")]
    public Text m_UserGoldTxt = null;
    int m_CurGold = 0;

    //## �÷��̾� ����
    public PlayerCtrl m_RefHero = null;




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

        //## �۷ι����� �ҷ����� & UI�ʱ�ȭ
        GlobalVal.LoadGameData();
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



        m_CoinItem = Resources.Load("CoinItem/CoinPrefab")as GameObject;

        m_RefHero = GameObject.FindObjectOfType<PlayerCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        //## ���콺 �߾ӹ�ư Ŭ��
        if (Input.GetMouseButtonDown(2) == true)
        {

            UserSkill_Key(SkillType.Skill_1);

        }

        //## ����Ű �ߵ�
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            UserSkill_Key(SkillType.Skill_0); //## 30% ����
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            UserSkill_Key(SkillType.Skill_1); //## �̰Ž� ����ź
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            UserSkill_Key(SkillType.Skill_2); //## ��ȣ��
        }


    }

    //���� ���� �� ȭ�� ǥ��
    public void DispScore(int score)
    {
        totScore += score;
        txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";
    }


    void RefreshGameUI()
    {
        if (m_UserGoldTxt != null)
            m_UserGoldTxt.text = "Gold <color=#ffff00>"+ GlobalVal.g_UserGold +"</color>";
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

    public void SpawnCoin(Vector3 a_pos)
    {
        GameObject a_CoinObj = Instantiate(m_CoinItem);
        a_CoinObj.transform.position = a_pos;
        Destroy(a_CoinObj, 10.0f);
    }


    //## ��� �߰� �Լ�
    public void AddGold(int Val = 10)
    {
        //���� ��尪
        if (Val < 0)
        {
            m_CurGold += Val;
            if (m_CurGold < 0)
                m_CurGold = 0;
        }

        else if (m_CurGold <=  int.MaxValue -Val)
            m_CurGold += Val;
        else
            m_CurGold = int.MaxValue;

        //## ���� ���� ���
        if (Val < 0)
        {
            GlobalVal.g_UserGold += Val;
            if (GlobalVal.g_UserGold < 0)
                GlobalVal.g_UserGold = 0;
        }

        else if (GlobalVal.g_UserGold <= int.MaxValue -Val)
            GlobalVal.g_UserGold += Val;
        else
            GlobalVal.g_UserGold = int.MaxValue;


        if (m_UserGoldTxt != null)
            m_UserGoldTxt.text = "Gold <color=#ffff00>"+
                GlobalVal.g_UserGold + "</color>";

        //## ���� ����
        PlayerPrefs.SetInt("UserGold", GlobalVal.g_UserGold);


    }

    //# ��ų ���

    public void UserSkill_Key(SkillType a_SkTy)
    {
        if (m_RefHero != null)
            m_RefHero.UseSkill(a_SkTy);

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

}//public class GameMgr : MonoBehaviour
