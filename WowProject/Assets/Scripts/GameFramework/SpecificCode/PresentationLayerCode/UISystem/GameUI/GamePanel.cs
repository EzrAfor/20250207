using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： Trigger 
//功能说明：游戏面板
//***************************************** 
public class GamePanel : BasePanel
{
    private Transform playerInfoTrans;
    private Transform targetInfoTrans;

    private Text playerNameText;
    private Text playerLevelText;
    private Text playerHPText;
    private Text playerMPText;
    private Slider playerHPSlider;
    private Slider playerMPSlider;

    private Text targetNameText;
    private Text targetLevelText;
    private Text targetHPText;
    private Text targetMPText;
    private Slider targetHPSlider;
    private Slider targetMPSlider;

    private Button openCharacterPanelButton;
    private Button openHeadCharacterPanelButton;
    private bool characterPanelisOpen;

    private Button openInventoryPanelButton;
    private bool inventoryPanelisOpen;

    public override void OnInit()
    {
        base.OnInit();
        playerInfoTrans = transform.Find("Emp_PlayerInfo");
        targetInfoTrans = transform.Find("Emp_TargetInfo");
        //寻找组件
        playerNameText=DeepFindTransform.DeepFindChild(playerInfoTrans, "Text_Name").GetComponent<Text>();
        playerLevelText = DeepFindTransform.DeepFindChild(playerInfoTrans, "Text_Level").GetComponent<Text>();
        playerHPText = DeepFindTransform.DeepFindChild(playerInfoTrans, "Text_HP").GetComponent<Text>();
        playerMPText = DeepFindTransform.DeepFindChild(playerInfoTrans, "Text_MP").GetComponent<Text>();
        playerHPSlider = DeepFindTransform.DeepFindChild(playerInfoTrans, "Sli_HP").GetComponent<Slider>();
        playerMPSlider = DeepFindTransform.DeepFindChild(playerInfoTrans, "Sli_MP").GetComponent<Slider>();

        targetNameText = DeepFindTransform.DeepFindChild(targetInfoTrans, "Text_Name").GetComponent<Text>();
        targetLevelText = DeepFindTransform.DeepFindChild(targetInfoTrans, "Text_Level").GetComponent<Text>();
        targetHPText = DeepFindTransform.DeepFindChild(targetInfoTrans, "Text_HP").GetComponent<Text>();
        targetMPText = DeepFindTransform.DeepFindChild(targetInfoTrans, "Text_MP").GetComponent<Text>();
        targetHPSlider = DeepFindTransform.DeepFindChild(targetInfoTrans, "Sli_HP").GetComponent<Slider>();
        targetMPSlider = DeepFindTransform.DeepFindChild(targetInfoTrans, "Sli_MP").GetComponent<Slider>();

        openHeadCharacterPanelButton= transform.Find("Btn_HeadCharacter").GetComponent<Button>();
        openHeadCharacterPanelButton.onClick.AddListener(OpenOrCloseCharacterPanel);

        openCharacterPanelButton = DeepFindTransform.DeepFindChild(transform,"Btn_Character").GetComponent<Button>();
        openCharacterPanelButton.onClick.AddListener(OpenOrCloseCharacterPanel);

        openInventoryPanelButton = DeepFindTransform.DeepFindChild(transform,"Btn_Inventory").GetComponent<Button>();
        openInventoryPanelButton.onClick.AddListener(OpenOrCloseInventoryPanel);


        this.RegistEvent<UpdatePlayerInfoEvent>(UpdatePlayerInfo);
        this.RegistEvent<UpdateTargetInfoEvent>(UpdateTargetInfo);
        this.RegistEvent<SetCharacterPanelStateEvent>(SetCharacterPanelState);
        this.RegistEvent<SetInventoryPanelStateEvent>(SetInventoryPanelState);
    }

    private void UpdatePlayerInfo(object obj)
    {
        PlayerSaveData psd = (PlayerSaveData)obj;
        playerNameText.text = psd.id;
        playerLevelText.text = psd.level.ToString();
        playerHPSlider.value = (float)psd.hp / psd.rd.HP;
        playerMPSlider.value = (float)psd.mana / psd.rd.mana;
        playerHPText.text = psd.hp.ToString() + "/" + psd.rd.HP.ToString();
        playerMPText.text = psd.mana.ToString() + "/" + psd.rd.mana.ToString();
    }

    private void UpdateTargetInfo(object obj)
    {
        if (obj==null)
        {
            if (targetInfoTrans.gameObject.activeSelf)
            {
                targetInfoTrans.gameObject.SetActive(false);
            }
            return;
        }
        if (!targetInfoTrans.gameObject.activeSelf)
        {
            targetInfoTrans.gameObject.SetActive(true);
        }
        PlayerData pd = (PlayerData)obj;
        targetNameText.text = pd.id;
        targetLevelText.text = pd.level.ToString();
        targetHPSlider.value = (float)pd.hp / pd.rd.HP;
        targetMPSlider.value = (float)pd.mana / pd.rd.mana;
        targetHPText.text = pd.hp.ToString() + "/" + pd.rd.HP.ToString();
        targetMPText.text = pd.mana.ToString() + "/" + pd.rd.mana.ToString();
    }

    private void OpenOrCloseCharacterPanel()
    {
        characterPanelisOpen = !characterPanelisOpen;
        if (characterPanelisOpen)
        {
            this.GetSystem<IUISystem>().OpenPanel<CharacterPanel>();
        }
        else
        {
            this.GetSystem<IUISystem>().ClosePanel("CharacterPanel");
        }
    }

    private void SetCharacterPanelState(object obj)
    {
        characterPanelisOpen = (bool)obj;
    }

    private void OpenOrCloseInventoryPanel()
    {
        inventoryPanelisOpen = !inventoryPanelisOpen;
        if (inventoryPanelisOpen)
        {
            this.GetSystem<IUISystem>().OpenPanel<InventoryPanel>();
        }
        else
        {
            this.GetSystem<IUISystem>().ClosePanel("InventoryPanel");
        }
    }

    private void SetInventoryPanelState(object obj)
    {
        inventoryPanelisOpen = (bool)obj;
    }
}
