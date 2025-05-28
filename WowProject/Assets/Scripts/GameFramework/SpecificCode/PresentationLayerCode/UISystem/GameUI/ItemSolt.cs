using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//*****************************************
//创建人： Trigger 
//功能说明：物品糙
//***************************************** 
public class ItemSolt : MonoBehaviour,IController,IPointerEnterHandler,IPointerExitHandler
{
    private ItemData id;
    public Image itemIconImage;
    public GameObject[] qualityGos;
    public Text itemNumText;
    public string showStr;//物品所有信息

    public void InitSolt()
    {
        itemIconImage = transform.Find("Img_ItemIcon").GetComponent<Image>();
        qualityGos = new GameObject[4];
        Transform empTrans = transform.Find("Emp_Quility");
        for (int i = 0; i < qualityGos.Length; i++)
        {
            qualityGos[i] = empTrans.GetChild(i).gameObject;
            //Debug.Log(qualityGos[i]);
        }
        itemNumText = transform.Find("Text_ItemNum").GetComponent<Text>();
    }

    void Update()
    {

    }
    /// <summary>
    /// 设置物品信息
    /// </summary>
    /// <param name="itemData"></param>
    public void SetItemData(ItemData itemData,int itemNum)
    {
        id = itemData;
        ShowQualityUI(id.quality);
        SetItemInfoStr();
        itemNumText.text = itemNum.ToString();
        itemIconImage.sprite = GameResSystem.GetRes<Sprite>("New_UI/ItemsIcon/"+id.id);
    }
    /// <summary>
    /// 设置物品UI信息字符串
    /// </summary>
    private void SetItemInfoStr()
    {
        showStr += StrMgr.SetStrColor(StrMgr.qualityStrColor[(int)id.quality], id.name);
        showStr += "\n" + "价格：" + StrMgr.SetStrColor(StrMgr.priceStrColor, id.price);
        string equipTypeStr = "";
        switch (id.equipType)
        {
            case EQUIPTYPE.HELMET:
                equipTypeStr = "头盔";
                break;
            case EQUIPTYPE.CLOAK:
                equipTypeStr = "披风";
                break;
            case EQUIPTYPE.SHAWL:
                equipTypeStr = "披肩";
                break;
            case EQUIPTYPE.PANTS:
                equipTypeStr = "裤子";
                break;
            case EQUIPTYPE.SHOES:
                equipTypeStr = "鞋子";
                break;
            case EQUIPTYPE.CLOVES:
                equipTypeStr = "手套";
                break;
            case EQUIPTYPE.CLOTHS:
                equipTypeStr = "衣服";
                break;
            case EQUIPTYPE.ACCESSORY:
                equipTypeStr = "饰品";
                break;
            case EQUIPTYPE.ONE_HAND_SWORD:
                equipTypeStr = "单手剑";
                break;
            case EQUIPTYPE.TWO_HANDS_SWORD:
                equipTypeStr = "双手剑";
                break;
            case EQUIPTYPE.DAGGER:
                equipTypeStr = "匕首";
                break;
            case EQUIPTYPE.STAFF:
                equipTypeStr = "法杖";
                break;
            case EQUIPTYPE.BOW:
                equipTypeStr = "弓箭";
                break;
            case EQUIPTYPE.ASSISTANT:
                equipTypeStr = "副手武器";
                break;
            default:
                break;
        }
        showStr += "\n" + "装备类型：" + StrMgr.SetStrColor(StrMgr.equipStrColor,equipTypeStr);
        if ((int)id.armorType<8)
        {
            string armorTypeStr = "";
            switch (id.armorType)
            {
                case ARMORTYPE.PLATE:
                    armorTypeStr = "板甲";
                    break;
                case ARMORTYPE.MAIL:
                    armorTypeStr = "锁甲";
                    break;
                case ARMORTYPE.LEATHER:
                    armorTypeStr = "皮甲";
                    break;
                case ARMORTYPE.CLOTH:
                    armorTypeStr = "布甲";
                    break;
                default:
                    break;
            }
            showStr += "\n" + "护甲类型：" + StrMgr.SetStrColor(StrMgr.armorStrColor, armorTypeStr);
        }
        showStr += "\n" + id.description;
        if (id.stamina > 0)
        {
            showStr += "\n" + "耐力：" + StrMgr.SetStrColor(StrMgr.staminaStrColor, id.stamina);
        }
        if (id.strength > 0)
        {
            showStr += "\n" + "力量：" + StrMgr.SetStrColor(StrMgr.strengthStrColor, id.strength);
        }
        if (id.intellect > 0)
        {
            showStr += "\n" + "智力:" + StrMgr.SetStrColor(StrMgr.intellectStrColor, id.intellect);
        }
        if (id.agility > 0)
        {
            showStr += "\n" + "敏捷：" + StrMgr.SetStrColor(StrMgr.agilityStrColor, id.agility);
        }
        if (id.spirit > 0)
        {
            showStr += "\n" + "精神：" + StrMgr.SetStrColor(StrMgr.spiritStrColor, id.spirit);
        }
        if (id.armor > 0)
        {
            showStr += "\n" + "护甲：" + StrMgr.SetStrColor(StrMgr.armorStrColor, id.armor);
        }
        if (id.resistance > 0)
        {
            showStr += "\n" + "抗性：" + StrMgr.SetStrColor(StrMgr.resistanceStrColor, id.resistance);
        }
        if (id.moveSpeed > 0)
        {
            showStr += "\n" + "移动速度：" + StrMgr.SetStrColor(StrMgr.moveSpeedStrColor, id.moveSpeed);
        }
        if (id.attackRange > 0)
        {
            showStr += "\n" + "攻击范围：" + StrMgr.SetStrColor(StrMgr.attackRangeStrColor, id.attackRange);
        }
    }

    /// <summary>
    /// 根据品质显示物品不同的背景色
    /// </summary>
    private void ShowQualityUI(QUALITY q=QUALITY.NONE)
    {
        for (int i = 0; i < qualityGos.Length; i++)
        {
            qualityGos[i].SetActive(false);
        }
        if (q!=QUALITY.NONE)
        {
            //Debug.Log(q);
            //Debug.Log((int)q);
            qualityGos[(int)q].SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.SendEvent<ShowOrHideTipTextEvent>(new ShowOrHideTipTextParam() {show=true,itemInfo=showStr,pos=eventData.position });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.SendEvent<ShowOrHideTipTextEvent>(new ShowOrHideTipTextParam() { show = false });
    }
}
