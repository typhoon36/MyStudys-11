using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Skill_0 = 0,        //30% ����
    Skill_1,            //����ź
    Skill_2,            //��ȣ��
    SkCount
}

public class GlobalValue
{
    public static string g_Unique_ID = "";  //������ ������ȣ

    public static string g_NickName = "";   //������ ����
    public static int g_BestScore = 0;      //��������
    public static int g_UserGold = 0;       //���ӸӴ�
    public static int g_Exp = 0;            //����ġ Experience
    public static int g_Level = 0;          //����

    public static int[] g_SkillCount = new int[3]; //��ų ������
    public static int g_BagItemCount = 0; // ���濡 �ִ� ������ ����
    public static int g_BagItemLimit = 10; // ���濡 ���� �� �ִ� �ִ� ������ ����

    public static void LoadGameData()
    {
        g_NickName  = PlayerPrefs.GetString("NickName", "SBS����");
        g_BestScore = PlayerPrefs.GetInt("BestScore", 0);
        g_UserGold  = PlayerPrefs.GetInt("UserGold", 0);
        g_BagItemCount = PlayerPrefs.GetInt("BagItemCount", 0); // BagItemCount �ε�

        PlayerPrefs.SetInt("UserGold", 9999);
       

        //## ��ų ���� �� �ε�
        string a_MkKey = "";
        for (int i = 0; i < g_SkillCount.Length; i++)
        {
            a_MkKey = "SkillCount" + i.ToString();
            //PlayerPrefs.SetInt(a_MkKey, 9);
            g_SkillCount[i] = PlayerPrefs.GetInt(a_MkKey, 1);
        }
    }

    public static void SaveGameData()
    {
        PlayerPrefs.SetString("NickName", g_NickName);
        PlayerPrefs.SetInt("BestScore", g_BestScore);
        PlayerPrefs.SetInt("UserGold", g_UserGold);
        PlayerPrefs.SetInt("BagItemCount", g_BagItemCount); // BagItemCount ����

        //## ��ų ���� �� ����
        string a_MkKey = "";
        for (int i = 0; i < g_SkillCount.Length; i++)
        {
            a_MkKey = "SkillCount" + i.ToString();
            PlayerPrefs.SetInt(a_MkKey, g_SkillCount[i]);
        }
    }
}
