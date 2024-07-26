using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class Game_Mgr : MonoBehaviour
{
    public Text PrintText = null;
    public Button JsonLoad_Btn = null;

    //## Jsons ����� / �ε�
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

        // Json ���� �ε�
        TextAsset a_FilePath = Resources.Load("user_info.txt") as TextAsset;

        //string �ؽ�Ʈ ����
        string a_JsonStr = a_FilePath.text;

        //�Ľ�
        var N = JSON.Parse(a_JsonStr);

        string User_Job = N["����"];

        //job ���
        if (N["����"] !=  null)
        {
            string user_job = N["����"];
            a_StrOutPut += "N[\"����\"] : " + user_job + "\n";


        }

        //name ���
        if (N["�̸�"] != null)
        {
            string user_name = N["�̸�"];
            //�ٸ� ������� �Ľ�
            //string user_name2 = N["�̸�"].ToString(); 
            // "\"�̸�\"" : "ȫ�浿" �̷������� ��µ�

            a_StrOutPut += "N[\"�̸�\"] : " + user_name + "\n";
        }

        //���� ���
        if (N["����"] != null)
        {
            string User_kind = N["����"];
            a_StrOutPut += "N[\"����\"] : " + User_kind + "\n";
        }

        a_StrOutPut += "\n";

        //## �ɷ�ġ ��ü �� ����Ű�� ���
        if (N["�ɷ�ġ"] != null && N["�ɷ�ġ"]["����"] != null)
        {
            int Level = N["�ɷ�ġ"]["����"].AsInt;
            a_StrOutPut += "N[\"�ɷ�ġ\"][\"����\"] : " + Level + "\n";
        }

        //## �ɷ�ġ ��ü �� Ȱ�� Ű�� ���
        if (N["�ɷ�ġ"] != null && N["�ɷ�ġ"]["Ȱ��"] != null)
        {
            int Energy = N["�ɷ�ġ"]["Ȱ��"].AsInt;
            a_StrOutPut += "N[\"�ɷ�ġ\"][\"����\"] : " + Energy + "\n";
        }


        //## �ɷ�ġ ��ü �� ����� Ű�� ���
        if (N["�ɷ�ġ"] != null && N["�ɷ�ġ"]["�����"] != null)
        {
            int MHP = N["�ɷ�ġ"]["�����"].AsInt;
            a_StrOutPut += "N[\"�ɷ�ġ\"][\"�����\"] : " + MHP + "\n";
        }

        //## �ɷ�ġ ��ü �� ���� Ű�� ���
        if (N["�ɷ�ġ"] != null && N["�ɷ�ġ"]["����"] != null)
        {
            int Mana = N["�ɷ�ġ"]["����"].AsInt;
            a_StrOutPut += "N[\"�ɷ�ġ\"][\"����\"] : " + Mana + "\n";
        }

        //## ���� ��ų �迭�� �ε�
        if (N["������ų"] != null)
            for (int i = 0; i < N["������ų"].Count; i++)
            {
                a_StrOutPut += "N[\"������ų\"][" + i + "] : " +N["������ų"][i] + "\n";
            }

        //## ���ھ� �ε�
        if (N["���ھ�"] != null)
        {
            int Score = N["���ھ�"].AsInt;
            a_StrOutPut += "N[\"���ھ�\"] : " + Score + "\n";
        }


        //## �ؽ�Ʈ�� ���
        PrintText.text = a_StrOutPut;

    }

    //# Json �����
    void MJsonMake_Click()
    {
        JSONObject a_MkJson = new JSONObject();
        a_MkJson["StrData"] = "Hello World!";
        //���� Ű���� ������ �������(������� �������� Add�� �߰�)
        a_MkJson["Level"] = 777;
        a_MkJson["BoolTest"] = true;
        a_MkJson["X_Pos"] = 12121212.46342;
        a_MkJson["FValue"] = 3.14;

        //�迭 �����(��ŷ)
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

        //�Ľ�
        if (string.IsNullOrEmpty(a_JsonStr) == true)
            return;

        JSONNode a_ParseJs = JSON.Parse(a_JsonStr);

        //## Ű�� ���
        if (a_ParseJs["StrData"] != null)
        {
            string a_StrData = a_ParseJs["StrData"];
            a_StrOutPut += "���ڿ� : " + a_StrData + "\n";
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

        //## ��ŷ ���
        int Rank = 0;
        if (a_ParseJs["RkList"] != null)
            for (int i = 0; i < a_ParseJs["RkList"].Count; i++)
            {
                if ((i % 3) == 0)
                {
                    Rank = (i / 3) + 1;

                    a_StrOutPut += Rank + "�� : ";
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

        //## �ؽ�Ʈ�� ���
        PrintText.text = a_StrOutPut;

    }

}
