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
    WARRIOP,//战士	
    PALADIN,//圣骑士	1
    HUNTER,//猎人2
    ROGUE,//盗贼3
    PRISST,//牧师	
    DEATHKNIGHT,//死亡骑士
    SHAMAM,//萨满
    MAGE,//法师7
    WARLOCK,//术士
    DRVID//德鲁伊
}

public enum RACE
{
    HUMAN,
    DWARF,//矮人
    GNOME,//侏儒
    NIGHTELF,//暗夜精灵
    DRAENEI,//德莱尼
    ORC,//兽人
    TROLL,//巨魔
    FORSAKEN,//被遗忘者
    TAUREN,//牛头人
    BLOODELF//血精灵
}

public enum GENDER
{
    MALE,
    FEMALE
}
