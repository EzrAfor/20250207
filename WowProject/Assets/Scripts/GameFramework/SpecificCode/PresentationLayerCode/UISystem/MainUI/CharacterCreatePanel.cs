using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreatePanel : BasePanel
{

    private Button rotateLeftBtn;
    private Button rotateRightBtn;
    private Button exitBtn;
    private Button agreeBtn;
    private InputField nameIF;

    public override void OnInit()
    {
        base.OnInit();
        rotateLeftBtn = DeepFindTransform.DeepFindChild(transform, "Btn_RotateLeft").GetComponent<Button>();
        rotateRightBtn = DeepFindTransform.DeepFindChild(transform, "Btn_RotateRight").GetComponent<Button>();
        exitBtn = DeepFindTransform.DeepFindChild(transform, "Btn_Agree").GetComponent<Button>();
        agreeBtn = DeepFindTransform.DeepFindChild(transform, "Btn_Exit").GetComponent<Button>();
        nameIF = DeepFindTransform.DeepFindChild(transform, "IF_Name").GetComponent<InputField>();
        rotateLeftBtn.onClick.AddListener(OnLeftRotateCharacterClick);
        rotateRightBtn.onClick.AddListener(OnRightRotateCharacterClick);
        exitBtn.onClick.AddListener(OnAgreeCreateClick);
        agreeBtn.onClick.AddListener(OnReturnToChoicePanelClick);
    }

    private void OnLeftRotateCharacterClick()
    {
        uiSystem.RotateModel(-45);
    }

    private void OnRightRotateCharacterClick()
    {
        uiSystem.RotateModel(45);
    }

    private void OnReturnToChoicePanelClick()
    {
        OnClose();
        uiSystem.OpenPanel<ChoicePanel>();
    }

    private void OnAgreeCreateClick()
    {
        if (nameIF.text != null||!nameIF.text.Contains(" ")) {
            uiSystem.OpenPanel<TipPanel>("名字不能为空且不能包含空格");
        }
        OnClose();
        uiSystem.OpenPanel<TipPanel>("创建成功");
        uiSystem.OpenPanel<ChoicePanel>();
    }



}
