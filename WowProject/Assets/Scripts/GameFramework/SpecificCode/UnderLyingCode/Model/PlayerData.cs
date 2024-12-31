using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerData
{
   public string id;
    public ROLE role=ROLE.PALADIN;
    public GENDER gender=GENDER.MALE;
    public RACE race=RACE.BLOODELF;

   
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
