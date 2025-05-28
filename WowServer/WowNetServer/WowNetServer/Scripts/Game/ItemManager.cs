using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ItemManager
{
    public static ItemManager Instance;
    public ItemData[] itemsList;

    public void ReadItemDatas()
    {
        string bjsonPath = "ItemInfo.json";
        string bfilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, bjsonPath);
        string bjsonStr = File.ReadAllText(bfilePath);
        itemsList = JsonConvert.DeserializeObject<ItemData[]>(bjsonStr);
        //ItemDataList idl = new ItemDataList();
        //idl.itemDatas = itemsList;
        //Console.WriteLine(JsonConvert.SerializeObject(idl));
    }
}
public class ItemData
{
    /// <summary>
    /// 物品ID
    /// </summary>
    public int id;
    /// <summary>
    /// 名字
    /// </summary>
    public string name;
    /// <summary>
    /// 价格
    /// </summary>
    public int price;
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
    /// 等级限定
    /// </summary>
    public int level;
    /// <summary>
    /// 物品描述
    /// </summary>
    public string description;
    /// <summary>
    /// 物品品质
    /// </summary>
    public QUALITY quality;
    /// <summary>
    /// 装备类型
    /// </summary>
    public EQUIPTYPE equipType;
    /// <summary>
    /// 护甲类型
    /// </summary>
    public ARMORTYPE armorType;
}
/// <summary>
/// 物品品质
/// </summary>
public enum QUALITY
{
    LEGENDARY,//传说
    EPIC,//精品
    RARE,//良好
    GENERAL,//一般
    NONE
}
/// <summary>
/// 装备类型
/// </summary>
public enum EQUIPTYPE
{
    HELMET,//头盔
    CLOAK,//披风
    SHAWL,//披肩
    PANTS,//裤子 3
    SHOES,//鞋子
    CLOVES,//手套
    CLOTHS,//衣服 6
    ACCESSORY,//饰品
    ONE_HAND_SWORD,//单手剑 8
    TWO_HANDS_SWORD,//双手剑
    DAGGER,//匕首 10
    STAFF,//法杖
    BOW,//弓箭
    ASSISTANT//副手武器 13
}
/// <summary>
/// 护甲类型
/// </summary>
public enum ARMORTYPE
{
    PLATE,//板甲
    MAIL,//锁甲
    LEATHER,//皮甲
    CLOTH//布甲
}
[System.Serializable]
public class ItemDataList
{
    public ItemData[] itemDatas;
}
/// <summary>
/// 背包格子信息
/// </summary>
[System.Serializable]
public class SlotData
{
    public int id;
    public int num;
}
/// <summary>
/// 背包物品信息列表
/// </summary>
[System.Serializable]
public class InventoryItemList
{
    public List<SlotData> slotsList;//背包信息列表
}