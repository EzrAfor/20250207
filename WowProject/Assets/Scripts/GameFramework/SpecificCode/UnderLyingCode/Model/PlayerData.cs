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
//玩家数据列表类，作为容器方便jsonutility解析
public class PlayerDatasList
{
    public int choiceID;//当前选择的人物id
    public List<PlayerData> playerDatas;
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
