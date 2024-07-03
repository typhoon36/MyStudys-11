using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVal 
{
    public static string g_UniqueID = "";


    //# 유저 별명 및 게임 점수,골드값
    public static string g_NickName = "";

    public static int g_BestScore = 0;
    
    
    public static int g_UserGold = 0;


    //# 레벨 & 경험치
    public static int g_Exp = 0;
    public static int g_Lv = 0;



    //# 로딩함수
    public static void LoadGameData()
    {
        g_NickName = PlayerPrefs.GetString("NickName", "워리어");
        g_BestScore = PlayerPrefs.GetInt("BestScore", 0);
        g_UserGold = PlayerPrefs.GetInt("UserGold", 0);
    }


}
