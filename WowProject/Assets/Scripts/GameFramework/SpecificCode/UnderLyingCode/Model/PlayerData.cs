using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string id;
    public float x;
    public float y;
    public float z;
    public float ex;
    public float ey;
    public float ez;
    public int coin;
    public int mp;
    public int hp;
    public int level;
    public ROLE role;
    public GENDER gender;
    public RACE race;
    public CHARACTERSTATE characterState;


}
[Serializable]
//��������б��࣬��Ϊ��������jsonutility����
public class PlayerDatasList
{
    public int choiceID;//��ǰѡ�������id
    public List<PlayerData> playerDatas;
}

public enum ROLE
{
    WARRIOP,//սʿ	
    PALADIN,//ʥ��ʿ	1
    HUNTER,//����2
    ROGUE,//����3
    PRISST,//��ʦ	
    DEATHKNIGHT,//������ʿ
    SHAMAM,//����
    MAGE,//��ʦ7
    WARLOCK,//��ʿ
    DRVID//��³��
}

public enum RACE
{
    HUMAN,
    DWARF,//����
    GNOME,//٪��
    NIGHTELF,//��ҹ����
    DRAENEI,//������
    ORC,//����
    TROLL,//��ħ
    FORSAKEN,//��������
    TAUREN,//ţͷ��
    BLOODELF//Ѫ����
}

public enum GENDER
{
    MALE,
    FEMALE
}
