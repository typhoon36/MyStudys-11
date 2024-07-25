using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class Game_Mgr : MonoBehaviour
{
    public Text PrintText = null;
    public Button JsonLoad_Btn = null;

    // Start is called before the first frame update
    void Start()
    {
        if (JsonLoad_Btn != null)
        {
            JsonLoad_Btn.onClick.AddListener(JsonLoad_Click);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void JsonLoad_Click()
    {
        string a_StrOutPut = "";

        // Json 파일 로드
        TextAsset a_FilePath = Resources.Load("user_info.txt") as TextAsset;

        //string 텍스트 추출
        string a_JsonStr = a_FilePath.text;

        //파싱
        var N = JSON.Parse(a_JsonStr);

        string User_Job = N["직업"];

        //job 출력
        if (N["직업"] !=  null)
        {
            string user_job = N["직업"];
            a_StrOutPut += "N[\"직업\"] : " + user_job + "\n";


        }

        //name 출력
        if (N["이름"] != null)
        {
            string user_name = N["이름"];
            //다른 방식으로 파싱
            //string user_name2 = N["이름"].ToString(); 
            // "\"이름\"" : "홍길동" 이런식으로 출력됨

            a_StrOutPut += "N[\"이름\"] : " + user_name + "\n";
        }

        //성별 출력
        if (N["성별"] != null)
        {
            string User_kind = N["성별"];
            a_StrOutPut += "N[\"성별\"] : " + User_kind + "\n";
        }

        a_StrOutPut += "\n";

        //## 능력치 객체 중 레벨키값 출력
        if (N["능력치"] != null && N["능력치"]["레벨"] != null)
        {
            int Level = N["능력치"]["레벨"].AsInt;
            a_StrOutPut += "N[\"능력치\"][\"레벨\"] : " + Level + "\n";
        }

        //## 능력치 객체 중 활력 키값 출력
        if (N["능력치"] != null && N["능력치"]["활력"] != null)
        {
            int Energy = N["능력치"]["활력"].AsInt;
            a_StrOutPut += "N[\"능력치\"][\"레벨\"] : " + Energy + "\n";
        }


        //## 능력치 객체 중 생명력 키값 출력
        if (N["능력치"] != null && N["능력치"]["생명력"] != null)
        {
            int MHP = N["능력치"]["생명력"].AsInt;
            a_StrOutPut += "N[\"능력치\"][\"생명력\"] : " + MHP + "\n";
        }

        //## 능력치 객체 중 마나 키값 출력
        if (N["능력치"] != null && N["능력치"]["마나"] != null)
        {
            int Mana = N["능력치"]["마나"].AsInt;
            a_StrOutPut += "N[\"능력치\"][\"마나\"] : " + Mana + "\n";
        }

        //## 보유 스킬 배열값 로드
        if (N["보유스킬"] != null)
        for (int i = 0; i < N["보유스킬"].Count; i++)
        {
            a_StrOutPut += "N[\"보유스킬\"][" + i + "] : " +N["보유스킬"][i] + "\n";
        }

        //## 스코어 로드
        if (N["스코어"] != null)
        {
            int Score = N["스코어"].AsInt;
            a_StrOutPut += "N[\"스코어\"] : " + Score + "\n";
        }


        //## 텍스트에 출력
        PrintText.text = a_StrOutPut;

    }
}
