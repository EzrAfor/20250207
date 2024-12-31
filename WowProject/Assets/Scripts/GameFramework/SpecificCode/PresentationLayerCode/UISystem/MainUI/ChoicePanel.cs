using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePanel : BasePanel
{

    private Button enterGameBtn;
    private Button rotateLeftBtn;
    private Button rotateRightBtn;
    private Button createCharacterBtn;
    private Button deleteCharacterBtn;
    private Button exitBtn;
    private ToggleGroup toggleGroup;
    private Text playerIDText;

    private GameObject leafBGGO;
    private GameObject characterGO;
    public override void OnInit()
    {
        base.OnInit();
        enterGameBtn = DeepFindTransform.DeepFindChild(transform, "Btn_StartGame").GetComponent<Button>();
        rotateLeftBtn = DeepFindTransform.DeepFindChild(transform, "Btn_RotateLeft").GetComponent<Button>();
        rotateRightBtn = DeepFindTransform.DeepFindChild(transform, "Btn_RotateRight").GetComponent<Button>();
        createCharacterBtn = DeepFindTransform.DeepFindChild(transform, "Btn_CreateCharacter").GetComponent<Button>();
        deleteCharacterBtn = DeepFindTransform.DeepFindChild(transform, "Btn_DeleteCharacter").GetComponent<Button>();
        exitBtn = DeepFindTransform.DeepFindChild(transform, "Btn_Exit").GetComponent<Button>();
        toggleGroup = DeepFindTransform.DeepFindChild(transform, "CharacterInfoContent").GetComponent<ToggleGroup>();
        playerIDText = DeepFindTransform.DeepFindChild(transform, "Text_PlayerID").GetComponent<Text>();
        enterGameBtn.onClick.AddListener(OnEnterGameClick);
        rotateLeftBtn.onClick.AddListener(OnLeftRotateCharacterClick);
        rotateRightBtn.onClick.AddListener(OnRightRotateCharacterClick);
        createCharacterBtn.onClick.AddListener(OnOpenCreateCharacterPanelClick);
        deleteCharacterBtn.onClick.AddListener(OnDeleteCharacterClick);
        exitBtn.onClick.AddListener(OnReturnToLoginPanelClick);


    }

    private void OnEnterGameClick() {
        base.OnClose();
        uiSystem.OpenPanel<LoadPanel>();
    }

    private void OnLeftRotateCharacterClick() {
        uiSystem.RotateModel(-45);
    }

    private void OnRightRotateCharacterClick()
    {
        uiSystem.RotateModel(45);
    }

    private void OnOpenCreateCharacterPanelClick() {
        base.OnClose();
        uiSystem.OpenPanel<CharacterCreatePanel>();
    }

    private void OnDeleteCharacterClick() {
        uiSystem.OpenPanel<TipPanel>("你是否要删除该角色");

    }

    private void OnReturnToLoginPanelClick() {
        base.OnClose();
        uiSystem.OpenPanel<LoginPanel>();
    }

    public override void OnClose()
    {
        base.OnClose();
        LoadOrDestoryChoicePanelGameObjects();
    }

    public override void OnShow(params object[] objs)
    {
        base.OnShow(objs); LoadOrDestoryChoicePanelGameObjects(true);
    }

    public void LoadOrDestoryChoicePanelGameObjects(bool ifLoad = false)
    {
        if (ifLoad == true)
        {
            leafBGGO = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Effect/LeavesPS"));
            characterGO = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Character/ChoiceModel/BloodDelf"));
        }
        else
        {
            GameObject.Destroy(leafBGGO);
            GameObject.Destroy(characterGO);
        }
    }


}
