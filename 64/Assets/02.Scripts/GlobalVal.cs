using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVal 
{
    public static string g_UniqueID = "";


    //# ���� ���� �� ���� ����,��尪
    public static string g_NickName = "";

    public static int g_BestScore = 0;
    
    
    public static int g_UserGold = 0;


    //# ���� & ����ġ
    public static int g_Exp = 0;
    public static int g_Lv = 0;



    //# �ε��Լ�
    public static void LoadGameData()
    {
        g_NickName = PlayerPrefs.GetString("NickName", "������");
        g_BestScore = PlayerPrefs.GetInt("BestScore", 0);
        g_UserGold = PlayerPrefs.GetInt("UserGold", 0);
    }


}
