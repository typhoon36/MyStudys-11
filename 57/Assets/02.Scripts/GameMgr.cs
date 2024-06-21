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


    //## ���� ��ġ
    public Transform[] SpawnPoints;

    //## ���� ������
    public GameObject Monster_Prefab;


    //## ���� ���� �ֱ�

    public float CreateTime = 2.0f;

    //## ���� ���� �ִ밹��
    public int MaxMonster = 10;

    //## ���� ���� ����
    public bool IsGameOver = false;

    //## ������Ʈ Ǯ��
    public List<GameObject> MonsterPool = new List<GameObject>();


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
        s_GameState = GameState.GameIng;

        DispScore(0);

        if (BackBtn != null)
            BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Lobby");
            });


        //## ��������Ʈ�� ã�� ���� ��� transform ������Ʈ�� ã�� �迭�� ����

        SpawnPoints = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        for (int i = 0; i < MaxMonster; i++)
        {
            GameObject monster = (GameObject)Instantiate(Monster_Prefab);
            monster.name = "Monster_" + i.ToString();
            monster.SetActive(false);
            MonsterPool.Add(monster);
        }




        if (SpawnPoints.Length > 0)
        {
            //## ���� ���� �ڷ�ƾ �Լ� ȣ��
            StartCoroutine(this.CreateMonster());
        }

    }

    // Update is called once per frame
    void Update()
    {

    }


    //���� ���� �� ȭ�� ǥ��
    public void DispScore(int score)
    {
        totScore += score;
        txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";
    }

    //## ���� �ڷ�ƾ 
    IEnumerator CreateMonster()
    {
        while (!IsGameOver)
        {
            yield return new WaitForSeconds(CreateTime);

            if (GameMgr.s_GameState == GameState.GameEnd) yield break;

            foreach (GameObject monster in MonsterPool)
            {
                if (monster.activeSelf == false)
                {
                    int idx = Random.Range(1, SpawnPoints.Length);
                    monster.transform.position = SpawnPoints[idx].position;
                    monster.SetActive(true);
                    break;
                }
            }


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

}//public class GameMgr : MonoBehaviour
