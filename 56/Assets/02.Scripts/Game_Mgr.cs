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




public class Game_Mgr : MonoBehaviour
{
    //## 게임 상태 -- static으로 관리 
    public static GameState s_Gamestate = GameState.GameIng;

    //## text 
    public Text txtScore;

    //## 누적 점수
    int totscore = 0;

    //## button 
    public Button Back_Btn;


    //## 싱글톤 패턴
    public static Game_Mgr Inst = null;

    //## 오브젝트 풀링
    private List<GameObject> monsterPool = new List<GameObject>();
    public int poolSize = 14;

    //## 몬스터 관련

    public GameObject Mon_Prefab; // 단일 몬스터 프리팹 참조
    public Transform[] Mon_Gen_Pos;

    //## 주기
    public float Gen_Time = 5.0f;

    //## 최대 발생 수
    public int Max_Mon = 10;

    //## 종료 여부
    public bool IsOver = false;



    void Awake()
    {

        Inst = this;
       
    }




    // Start is called before the first frame update
    void Start()
    {

        Mon_Gen_Pos = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        for(int i = 0; i < Max_Mon; i++)
        {
           GameObject obj = Instantiate(Mon_Prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            monsterPool.Add(obj);
        }


        if(Mon_Gen_Pos.Length > 0)
        {
            StartCoroutine(MonGen());
        }

        s_Gamestate = GameState.GameIng;

        DispScore(0);


        Back_Btn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Lobby");
        });

    }

    // Update is called once per frame
    void Update()
    {

    }


   
    //## 몬스터 생성
   IEnumerator MonGen()
    {
     
        while(!IsOver)
        {
            yield return new WaitForSeconds(Gen_Time);

            if(IsOver)yield break;

            //for(int i = 0; i < monsterPool.Count; i++)
            //{
            //    if (monsterPool[i].activeSelf == false)
            //    {
            //        int idx = Random.Range(1, Mon_Gen_Pos.Length);
            //        monsterPool[i].transform.position = Mon_Gen_Pos[idx].position;
            //        monsterPool[i].transform.rotation = Mon_Gen_Pos[idx].rotation;
            //        monsterPool[i].SetActive(true);
            //        break;
            //    }
            //}

            for (int i = 0; i < monsterPool.Count; i++)
            {
                if (!monsterPool[i].activeSelf)
                {
                    int idx = Random.Range(1, Mon_Gen_Pos.Length); // idx 정의
                    Vector3 spawnPos = Mon_Gen_Pos[idx].position; // spawnPos 정의

                    // 이미 활성화된 몬스터와의 거리 체크
                    bool isTooClose = false;
                    foreach (var monster in monsterPool)
                    {
                        if (monster.activeSelf && Vector3.Distance(monster.transform.position, spawnPos) < 3.0f) 
                        {
                            isTooClose = true;
                            break;
                        }
                    }

                    if (!isTooClose)
                    {
                        monsterPool[i].transform.position = spawnPos;
                        monsterPool[i].transform.rotation = Mon_Gen_Pos[idx].rotation;
                        monsterPool[i].SetActive(true);
                        break; // 위치가 적절하면 몬스터를 활성화하고 for 루프를 빠져나옴
                    }
                }
            }



        }






    }
    





    //## 점수 표시
    public void DispScore(int score)
    {
        totscore += score;
        txtScore.text = "score <color=#ff0000>" + totscore.ToString() + "</color>";
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
}
