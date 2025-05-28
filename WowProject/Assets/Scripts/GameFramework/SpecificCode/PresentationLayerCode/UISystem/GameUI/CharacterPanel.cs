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
            "������" + StrMgr.SetStrColor(StrMgr.staminaStrColor, rd.stamina) + "\n" +
            "������" + StrMgr.SetStrColor(StrMgr.strengthStrColor, rd.strength) + "\n" +
            "���ݣ�" + StrMgr.SetStrColor(StrMgr.agilityStrColor, rd.agility) + "\n" +
            "����:" + StrMgr.SetStrColor(StrMgr.intellectStrColor, rd.intellect) + "\n" +
            "����" + StrMgr.SetStrColor(StrMgr.spiritStrColor, rd.spirit) + "\n" +
            "���ף�" + StrMgr.SetStrColor(StrMgr.armorStrColor, rd.armor) + "\n" +
            "���ԣ�" + StrMgr.SetStrColor(StrMgr.resistanceStrColor, rd.resistance) + "\n" +
            "�ƶ��ٶȣ�" + StrMgr.SetStrColor(StrMgr.moveSpeedStrColor, rd.moveSpeed) + "\n" +
            "������Χ��" + StrMgr.SetStrColor(StrMgr.attackRangeStrColor, rd.attackRange);
        addValueText.text = 
           "��������" + "<color=#FF3030>" + rd.attackPower + "</color>" + "\n" +
           "����ǿ�ȣ�" + "<color=#63B8FF>" + rd.spellPower + " </color>" + "\n" +
           "�����ٶȣ�" + "<color=#888800>" + rd.attackRate + " </color>" + "\n" +
           "���м��ʣ�" + "<color=#888800>" + rd.hitChange + " </color>" + "\n" +
           "�����ʣ�" + "<color=#888800>" + rd.evasionRate + " </color>" + "\n" +
           "�����ʣ�" + "<color=#EE0000>" + rd.criticalStike + " </color>";
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }
}
