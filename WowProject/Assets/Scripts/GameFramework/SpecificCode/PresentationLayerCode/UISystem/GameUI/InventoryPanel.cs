using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//*****************************************
//创建人： Trigger 
//功能说明：背包面板
//***************************************** 
public class InventoryPanel : BasePanel,IDragHandler
{
    private Button closeButton;
    private Text copperText;
    private Text silverText;
    private Text goldText;
    private Transform bagTrans;
    private Text tipText;
    private Text tipViewText;

    public override void OnInit()
    {
        base.OnInit();
        closeButton = DeepFindTransform.DeepFindChild(transform, "Btn_Close").GetComponent<Button>();
        copperText= DeepFindTransform.DeepFindChild(transform, "Text_Copper").GetComponent<Text>();
        silverText = DeepFindTransform.DeepFindChild(transform, "Text_Silver").GetComponent<Text>();
        goldText = DeepFindTransform.DeepFindChild(transform, "Text_Gold").GetComponent<Text>();
        closeButton.onClick.AddListener(() => { OnClose();this.SendEvent<SetInventoryPanelStateEvent>(false); });
        
        //背包物品槽
        bagTrans = transform.Find("Img_BagBG");       
        this.RegistEvent<InitInventoryItemSlotsEvent>(InitInventoryItemSlots);

        //物品信息
        tipText = transform.Find("Text_Tip").GetComponent<Text>();
        tipViewText = DeepFindTransform.DeepFindChild(tipText.transform, "Text_TipView").GetComponent<Text>();
        tipText.gameObject.SetActive(false);
        this.RegistEvent<ShowOrHideTipTextEvent>(ShowOrHideTipText);
    }

    public override void OnShow(params object[] objs)
    {
        base.OnShow(objs);
        int coin = 20105;
        int copper = coin % 100;
        int silver = coin / 100;
        int gold = silver / 100;
        silver -= gold * 100;
        copperText.text = copper.ToString();
        silverText.text = silver.ToString();
        goldText.text = gold.ToString();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    private void ShowOrHideTipText(object obj)
    {
        ShowOrHideTipTextParam sot = (ShowOrHideTipTextParam)obj;
        if (sot.show)//显示
        {
            tipText.gameObject.SetActive(true);
            tipText.text = tipViewText.text =sot.itemInfo;
            tipText.transform.position = sot.pos;
        }
        else//隐藏
        {
            tipText.gameObject.SetActive(false);
        }
    }

    private void InitInventoryItemSlots(object obj)
    {
        List<SlotData> l = this.GetSystem<INetSystem>().GetPSDValue().slotsList;
        for (int i = 0; i < l.Count; i++)
        {
            ItemSolt itemSolt = Instantiate(GameResSystem.GetRes<GameObject>
                ("Prefabs/UI/InventoryPanelItem/ItemSlot"), bagTrans).GetComponent<ItemSolt>();
            itemSolt.InitSolt();
            itemSolt.SetItemData(this.GetSystem<IUISystem>().GetItemDataByID(l[i].id), l[i].num);
        }
    }
}
