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

    public static int[] g_SkillCount = new int[3];  //������ ������

    //## ���� ���� �ǹ� ����
    public static int g_BestFloorNum = 1;
    //## ���� ���� �ǹ� ����
    public static int g_CurFloorNum = 1;


    public static void LoadGameData()
    {
        PlayerPrefs.SetInt("UserGold", 999999);

        g_NickName  = PlayerPrefs.GetString("NickName", "SBS����");
        g_BestScore = PlayerPrefs.GetInt("BestScore", 0);
        g_UserGold  = PlayerPrefs.GetInt("UserGold", 0);

        string a_MkKey = "";
        for(int i = 0; i < g_SkillCount.Length; i++)
        {
            a_MkKey = "SkItem_" + i.ToString();
            
            g_SkillCount[i] = PlayerPrefs.GetInt(a_MkKey, 1);
        }

        g_BestFloorNum = PlayerPrefs.GetInt("BestFloorNum", 1);
        g_CurFloorNum = PlayerPrefs.GetInt("CurFloorNum", 1);


    }//public static void LoadGameData()


    public static void FloorGameData()
    {

    }


}
