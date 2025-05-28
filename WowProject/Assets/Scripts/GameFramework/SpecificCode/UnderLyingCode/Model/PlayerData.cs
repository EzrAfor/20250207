using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEditor.PlayerSettings;
//*****************************************
//创建人： Trigger 
//功能说明：玩家数据信息
//***************************************** 
/// <summary>
/// 玩家数据对象
/// </summary>
[Serializable]
public class PlayerSaveData
{
    public string id;//角色名
    public float x;
    public float y;
    public float z;
    public float ex;
    public float ey;
    public float ez;
    public float fx;
    public float fy;
    public float fz;
    public int coin;
    public int hp;
    public int mana;
    public int level;
    public ROLE role;
    public GENDER gender;
    public RACE race;
    public CHARACTERSTATE characterState;
    public bool isAI;
    public RoleAttributeValueData rd;//职业属性信息
    public List<SlotData> slotsList;//背包信息列表
    /// <summary>
    /// 玩家信息存贮对象转信息对象
    /// </summary>
    public PlayerData PSDtoPD()
    {
        PlayerData pd = new PlayerData();
        pd.id = id;
        pd.x = x;
        pd.y = y;
        pd.z = z;
        pd.ex = ex;
        pd.ey = ey;
        pd.ez = ez;
        pd.fx = fx;
        pd.fy = fy;
        pd.fz = fz;
        pd.hp = hp;
        pd.mana = mana;
        pd.level = level;
        pd.role = role;
        pd.race = race;
        pd.gender = gender;
        pd.characterState = characterState;
        pd.rd = rd;
        pd.isAI = isAI;
        return pd;
    }
}
/// <summary>
/// 玩家数据列表类，作为容器方便JsonUtility解析(当前账号下的角色)
/// </summary>
[Serializable]
public class PlayerSaveDatasList
{
    public int choiceID;//当前选择人物的ID
    public List<PlayerSaveData> playerSaveDatas;
}


/// <summary>
/// 玩家数据对象
/// </summary>
[Serializable]
public class PlayerData
{
    public string id;//角色名
    public float x;
    public float y;
    public float z;
    public float ex;
    public float ey;
    public float ez;
    public float fx;
    public float fy;
    public float fz;
    public int coin;
    public int hp;
    public int mana;
    public int level;
    public ROLE role;
    public GENDER gender;
    public RACE race;
    public CHARACTERSTATE characterState;
    public bool isAI;
    public RoleAttributeValueData rd;//职业属性信息

    public CharacterSyncData PDToCD()
    {
        CharacterSyncData cd = new CharacterSyncData();
        cd.id = id;
        cd.x = x;
        cd.y = y;
        cd.z = z;
        cd.ex = ex;
        cd.ey = ey;
        cd.ez = ez;
        cd.fx = fx;
        cd.fy = fy;
        cd.fz = fz;
        cd.hp = hp;
        cd.mana = mana;
        cd.level = level;
        cd.characterState = characterState;
        cd.maxHP = rd.HP;
        cd.maxMana = rd.mana;
        return cd;
    }
    /// <summary>
    /// 信息对象转玩家信息存贮对象
    /// </summary>
    /// <returns></returns>
    public PlayerSaveData PDToPSD()
    {
        PlayerSaveData psd = new PlayerSaveData();
        psd.id = id;
        psd.x = x;
        psd.y = y;
        psd.z = z;
        psd.ex = ex;
        psd.ey = ey;
        psd.ez = ez;
        psd.fx = fx;
        psd.fy = fy;
        psd.fz = fz;
        psd.hp = hp;
        psd.mana = mana;
        psd.level = level;
        psd.role = role;
        psd.race = race;
        psd.gender = gender;
        psd.characterState = characterState;
        psd.rd = rd;
        psd.isAI = isAI;
        return psd;
    }
}
/// <summary>
/// 玩家数据列表类，作为容器方便JsonUtility解析(当前账号下的角色)
/// </summary>
[Serializable]
public class PlayerDatasList
{
    public int choiceID;//当前选择人物的ID
    public List<PlayerData> playerDatas;
}
/// <summary>
/// 用于同步角色信息
/// </summary>
[Serializable]
public class CharacterSyncData
{
    public string id;//角色名
    public float x;
    public float y;
    public float z;
    public float ex;
    public float ey;
    public float ez;
    public float fx;
    public float fy;
    public float fz;
    public int hp;
    public int mana;
    public int level;
    public int maxHP;
    public int maxMana;
    public CHARACTERSTATE characterState;
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

/// <summary>
/// 角色属性数据
/// </summary>
[Serializable]
public class RoleAttributeValueData
{
    /// <summary>
    /// 能量
    /// </summary>
    public int mana;
    /// <summary>
    /// 耐力
    /// </summary>
    public int stamina;
    /// <summary>
    /// 力量
    /// </summary>
    public int strength;
    /// <summary>
    /// 敏捷
    /// </summary>
    public int agility;
    /// <summary>
    /// 智力
    /// </summary>
    public int intellect;
    /// <summary>
    /// 精神
    /// </summary>
    public int spirit;
    /// <summary>
    /// 护甲
    /// </summary>
    public int armor;
    /// <summary>
    /// 抗性
    /// </summary>
    public int resistance;
    /// <summary>
    /// 移动速度
    /// </summary>
    public float moveSpeed;
    /// <summary>
    /// 攻击范围
    /// </summary>
    public float attackRange;
    /// <summary>
    /// 生命值
    /// </summary>
    public int HP;
    /// <summary>
    /// 攻击力
    /// </summary>
    public int attackPower;
    /// <summary>
    /// 法术强度
    /// </summary>
    public int spellPower;
    /// <summary>
    /// 攻击速度
    /// </summary>
    public int attackRate;
    /// <summary>
    /// 命中几率
    /// </summary>
    public int hitChange;
    /// <summary>
    /// 闪避率
    /// </summary>
    public int evasionRate;
    /// <summary>
    /// 暴击率
    /// </summary>
    public int criticalStike;

    public static RoleAttributeValueData operator +(RoleAttributeValueData rd1, RoleAttributeValueData rd2)
    {
        RoleAttributeValueData result = new RoleAttributeValueData();
        //基础值
        result.mana = rd1.mana + rd2.mana;
        result.stamina = rd1.stamina + rd2.stamina;
        result.strength = rd1.strength + rd2.strength;
        result.agility = rd1.agility + rd2.agility;
        result.intellect = rd1.intellect + rd2.intellect;
        result.spirit = rd1.spirit + rd2.spirit;
        result.armor = rd1.armor + rd2.armor;
        result.resistance = rd1.resistance + rd2.resistance;
        result.moveSpeed = rd1.moveSpeed + rd2.moveSpeed;
        result.attackRange = rd1.attackRange + rd2.attackRange;
        //影响值
        result.HP = result.stamina * 10;
        result.attackPower = result.strength;
        result.spellPower = result.intellect;
        result.attackRate = result.agility * 3;
        result.hitChange = result.agility / 5;
        result.evasionRate = result.agility;
        result.criticalStike = result.attackPower / 2 + result.intellect / 5;
        return result;
    }

    public static RoleAttributeValueData operator *(RoleAttributeValueData rd1, int level)
    {
        RoleAttributeValueData result = new RoleAttributeValueData();
        level -= 1;
        //基础值
        result.mana = rd1.mana * level;
        result.stamina = rd1.stamina * level;
        result.strength = rd1.strength * level;
        result.agility = rd1.agility * level;
        result.intellect = rd1.intellect * level;
        result.spirit = rd1.spirit * level;
        result.armor = rd1.armor * level;
        result.resistance = rd1.resistance * level;
        result.moveSpeed = rd1.moveSpeed * level;
        result.attackRange = rd1.attackRange * level;
        //影响值
        result.HP = result.stamina * 10;
        result.attackPower = result.strength;
        result.spellPower = result.intellect;
        result.attackRate = result.agility * 3;
        result.hitChange = result.agility / 5;
        result.evasionRate = result.agility;
        result.criticalStike = result.attackPower / 2 + result.intellect / 5;
        return result;
    }
}