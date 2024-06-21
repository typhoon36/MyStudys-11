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


    //## 스폰 위치
    public Transform[] SpawnPoints;

    //## 몬스터 프리팹
    public GameObject Monster_Prefab;


    //## 몬스터 스폰 주기

    public float CreateTime = 2.0f;

    //## 몬스터 스폰 최대갯수
    public int MaxMonster = 10;

    //## 게임 종료 여부
    public bool IsGameOver = false;

    //## 오브젝트 풀링
    public List<GameObject> MonsterPool = new List<GameObject>();


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
        s_GameState = GameState.GameIng;

        DispScore(0);

        if (BackBtn != null)
            BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Lobby");
            });


        //## 스폰포인트를 찾아 하위 모든 transform 컴포넌트를 찾아 배열에 저장

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
            //## 몬스터 생성 코루틴 함수 호출
            StartCoroutine(this.CreateMonster());
        }

    }

    // Update is called once per frame
    void Update()
    {

    }


    //점수 누적 및 화면 표시
    public void DispScore(int score)
    {
        totScore += score;
        txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";
    }

    //## 생성 코루틴 
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

}//public class GameMgr : MonoBehaviour
