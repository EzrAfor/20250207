using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterPanel : BasePanel, IDragHandler
{
    private Button closeButton;
    private Text playerIDText;
    private Text basicValueText;
    private Text addValueText;

    public override void OnInit()
    {
        base.OnInit();

        playerIDText = DeepFindTransform.DeepFindChild(transform, "Text_Name").GetComponent<Text>();
        closeButton = transform.Find("Btn_Close").GetComponent<Button>();
        basicValueText = transform.Find("Text_BasicValue").GetComponent<Text>();
        addValueText= transform.Find("Text_AddValue").GetComponent<Text>();

        playerIDText.text = this.GetSystem<INetSystem>().GetPSDValue().id;
        closeButton.onClick.AddListener(() => { OnClose();this.SendEvent<SetCharacterPanelStateEvent>(false); });
    }

    public override void OnShow(params object[] objs)
    {
        base.OnShow(objs);
        RoleAttributeValueData rd= this.GetSystem<INetSystem>().GetPSDValue().rd;
        basicValueText.text = 
            "耐力：" + StrMgr.SetStrColor(StrMgr.staminaStrColor, rd.stamina) + "\n" +
            "力量：" + StrMgr.SetStrColor(StrMgr.strengthStrColor, rd.strength) + "\n" +
            "敏捷：" + StrMgr.SetStrColor(StrMgr.agilityStrColor, rd.agility) + "\n" +
            "智力:" + StrMgr.SetStrColor(StrMgr.intellectStrColor, rd.intellect) + "\n" +
            "精神：" + StrMgr.SetStrColor(StrMgr.spiritStrColor, rd.spirit) + "\n" +
            "护甲：" + StrMgr.SetStrColor(StrMgr.armorStrColor, rd.armor) + "\n" +
            "抗性：" + StrMgr.SetStrColor(StrMgr.resistanceStrColor, rd.resistance) + "\n" +
            "移动速度：" + StrMgr.SetStrColor(StrMgr.moveSpeedStrColor, rd.moveSpeed) + "\n" +
            "攻击范围：" + StrMgr.SetStrColor(StrMgr.attackRangeStrColor, rd.attackRange);
        addValueText.text = 
           "攻击力：" + "<color=#FF3030>" + rd.attackPower + "</color>" + "\n" +
           "法术强度：" + "<color=#63B8FF>" + rd.spellPower + " </color>" + "\n" +
           "攻击速度：" + "<color=#888800>" + rd.attackRate + " </color>" + "\n" +
           "命中几率：" + "<color=#888800>" + rd.hitChange + " </color>" + "\n" +
           "闪避率：" + "<color=#888800>" + rd.evasionRate + " </color>" + "\n" +
           "暴击率：" + "<color=#EE0000>" + rd.criticalStike + " </color>";
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }
}
