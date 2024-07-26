using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class Game_Mgr : MonoBehaviour
{
    public Text PrintText = null;
    public Button JsonLoad_Btn = null;

    //## Jsons 만들기 / 로드
    public Button MJsonMake_Btn = null;
    public Button MJsonLoad_Btn = null;
    string a_JsonStr = "";



    // Start is called before the first frame update
    void Start()
    {
        if (JsonLoad_Btn != null)
        {
            JsonLoad_Btn.onClick.AddListener(JsonLoad_Click);
        }

        if (MJsonMake_Btn != null)
        {
            MJsonMake_Btn.onClick.AddListener(MJsonMake_Click);
        }

        if (MJsonLoad_Btn != null)
        {
            MJsonLoad_Btn.onClick.AddListener(MJsonLoad_Click);
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

    //# Json 만들기
    void MJsonMake_Click()
    {
        JSONObject a_MkJson = new JSONObject();
        a_MkJson["StrData"] = "Hello World!";
        //같은 키값을 넣으면 덮어씌워짐(덮어씌우지 않으려면 Add로 추가)
        a_MkJson["Level"] = 777;
        a_MkJson["BoolTest"] = true;
        a_MkJson["X_Pos"] = 12121212.46342;
        a_MkJson["FValue"] = 3.14;

        //배열 만들기(랭킹)
        a_MkJson["RkList"][0] = "A";
        a_MkJson["RkList"][1] = "Racoon";
        a_MkJson["RkList"][2] = 111;
        a_MkJson["RkList"][3] = "ccc";
        a_MkJson["RkList"][4] = "Cat";
        a_MkJson["RkList"][5] = 222;
        a_MkJson["RkList"][6] = "ddd";
        a_MkJson["RkList"][7] = "PalDog";
        a_MkJson["RkList"][8] = 333;

        a_JsonStr = a_MkJson.ToString();
        Debug.Log(a_JsonStr);
    }

    void MJsonLoad_Click()
    {
        string a_StrOutPut = "";

        //파싱
        if (string.IsNullOrEmpty(a_JsonStr) == true)
            return;

        JSONNode a_ParseJs = JSON.Parse(a_JsonStr);

        //## 키값 출력
        if (a_ParseJs["StrData"] != null)
        {
            string a_StrData = a_ParseJs["StrData"];
            a_StrOutPut += "문자열 : " + a_StrData + "\n";
        }

        if (a_ParseJs["BoolTest"] != null)
        {
            bool a_Val = a_ParseJs["BoolTest"].AsBool;
            a_StrOutPut += "BoolTest : " + a_Val + "\n";
        }

        if (a_ParseJs["X_Pos"] != null)
        {
            double a_Xpos = a_ParseJs["X_Pos"].AsDouble;
            a_StrOutPut += "X_Pos : " + a_Xpos + "\n";
        }

        if (a_ParseJs["FValue"] != null)
        {
            float FValue = a_ParseJs["FValue"].AsFloat;
            a_StrOutPut += "FValue : " + FValue + "\n";
        }

        a_StrOutPut += "\n";

        //## 랭킹 출력
        int Rank = 0;
        if (a_ParseJs["RkList"] != null)
            for (int i = 0; i < a_ParseJs["RkList"].Count; i++)
            {
                if ((i % 3) == 0)
                {
                    Rank = (i / 3) + 1;

                    a_StrOutPut += Rank + "등 : ";
                    int a_Add = i;
                    string userId = a_ParseJs["RkList"][a_Add];
                    a_StrOutPut += userId + "(" + userId + "), ";
                    a_Add++;
                    string user_name = a_ParseJs["RkList"][a_Add];
                    a_StrOutPut += "Nick (" + user_name + "), ";
                    a_Add++;
                    int BestScore = a_ParseJs["RkList"][a_Add].AsInt;
                    a_StrOutPut += "BestScore (" + BestScore + ")\n";
                }
            }

        //## 텍스트에 출력
        PrintText.text = a_StrOutPut;

    }

}
