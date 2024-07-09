using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Skill_0 = 0,        //30% 힐링
    Skill_1,            //수류탄
    Skill_2,            //보호막
    SkCount
}

public class GlobalValue
{
    public static string g_Unique_ID = "";  //유저의 고유번호

    public static string g_NickName = "";   //유저의 별명
    public static int g_BestScore = 0;      //게임점수
    public static int g_UserGold = 0;       //게임머니
    public static int g_Exp = 0;            //경험치 Experience
    public static int g_Level = 0;          //레벨

    public static int[] g_SkillCount = new int[3]; //스킬 보유수
    public static int g_BagItemCount = 0; // 가방에 있는 아이템 개수
    public static int g_BagItemLimit = 10; // 가방에 담을 수 있는 최대 아이템 개수

    public static void LoadGameData()
    {
        g_NickName  = PlayerPrefs.GetString("NickName", "SBS영웅");
        g_BestScore = PlayerPrefs.GetInt("BestScore", 0);
        g_UserGold  = PlayerPrefs.GetInt("UserGold", 0);
        g_BagItemCount = PlayerPrefs.GetInt("BagItemCount", 0); // BagItemCount 로드

        PlayerPrefs.SetInt("UserGold", 9999);
       

        //## 스킬 보유 수 로드
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
        PlayerPrefs.SetInt("BagItemCount", g_BagItemCount); // BagItemCount 저장

        //## 스킬 보유 수 저장
        string a_MkKey = "";
        for (int i = 0; i < g_SkillCount.Length; i++)
        {
            a_MkKey = "SkillCount" + i.ToString();
            PlayerPrefs.SetInt(a_MkKey, g_SkillCount[i]);
        }
    }
}
