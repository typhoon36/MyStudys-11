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
}
