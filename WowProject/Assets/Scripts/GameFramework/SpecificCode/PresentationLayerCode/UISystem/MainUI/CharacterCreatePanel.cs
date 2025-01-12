using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterCreatePanel : BasePanel
{

    private Button rotateLeftBtn;
    private Button rotateRightBtn;
    private Button exitBtn;
    private Button agreeBtn;
    private InputField nameIF;
    private PlayerData playerData;
    private GameObject maleTGGo;
    private GameObject femaleTGGo;
    private List<Transform> maleRaceTrans = new List<Transform>();
    private List<Transform> femaleRaceTrans = new List<Transform>();
    private List<Transform> roleTrans = new List<Transform>();
    private bool startRotating;



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
        maleTGGo = DeepFindTransform.DeepFindChild(transform, "TG_RaceMale").gameObject;
        femaleTGGo = DeepFindTransform.DeepFindChild(transform, "TG_RaceFemale").gameObject;
        Transform sexListTrans = DeepFindTransform.DeepFindChild(transform, "Emp_SexList");
        //选择性别
        sexListTrans.GetChild(0).GetComponent<Toggle>().onValueChanged.AddListener(
            (bool seleceted) =>
            {
                if (seleceted)
                {
                    maleTGGo.SetActive(true);
                    femaleTGGo.SetActive(false);
                    playerData.gender = GENDER.MALE;
                    this.SendEvent<SetMaterialEvent>(playerData);
                }
            }
            );
        sexListTrans.GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener(
           (bool seleceted) =>
           {
               if (seleceted)
               {
                   maleTGGo.SetActive(false);
                   femaleTGGo.SetActive(true);
                   playerData.gender = GENDER.FEMALE;
                   this.SendEvent<SetMaterialEvent>(playerData);
               }
           }
           );
        //选择种族
        //男
        for (int i = 0; i < 10; i++)
        {
            Transform t = maleTGGo.transform.GetChild(i);
            maleRaceTrans.Add(t);
            t.GetComponent<Toggle>().onValueChanged.AddListener(
           (bool seleceted) =>
           {
               if (seleceted)
               {
                   playerData.race = (RACE)maleRaceTrans.IndexOf(t);
                   this.SendEvent<SetMaterialEvent>(playerData);
               }
           }
           );
        }
        //女
        for (int i = 0; i < 10; i++)
        {
            Transform t = femaleTGGo.transform.GetChild(i);
            femaleRaceTrans.Add(t);
            t.GetComponent<Toggle>().onValueChanged.AddListener(
           (bool seleceted) =>
           {
               if (seleceted)
               {
                   playerData.race = (RACE)femaleRaceTrans.IndexOf(t);
                   this.SendEvent<SetMaterialEvent>(playerData);
               }
           }
           );
        }
        //职业
        Transform roleListTrans = DeepFindTransform.DeepFindChild(transform, "Emp_RoleList");
        for (int i = 0; i < 10; i++)
        {
            Transform t = roleListTrans.GetChild(i);
            roleTrans.Add(t);
            t.GetComponent<Toggle>().onValueChanged.AddListener(
           (bool seleceted) =>
           {
               if (seleceted)
               {
                   playerData.role = (ROLE)roleTrans.IndexOf(t);
                   this.SendEvent<SetMaterialEvent>(playerData);
               }
           }
           );
        }
        this.SendCommand<RegistPTListenerCommand>(new PTSrc() { ptName = "PTCreateNewCharacter", listener = OnPTCreateNewCharacter });

    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            startRotating = true;
        }
        else
        {
            startRotating = false;
        }
        if (startRotating)
        {
            this.SendEvent<RotateModelEvent>(Input.GetAxisRaw("Mouse X") * 2);
        }
    }
    private void OnLeftRotateCharacterClick()
    {
        this.SendEvent<RotateModelEvent>(-45f);
    }

    private void OnRightRotateCharacterClick()
    {
        this.SendEvent<RotateModelEvent>(45f);
    }

    private void OnReturnToChoicePanelClick()
    {
        OnClose();
        uiSystem.OpenPanel<ChoicePanel>();
    }

    private void OnAgreeCreateClick()
    {
        if (nameIF.text == null || nameIF.text.Contains(" "))
        {
            uiSystem.OpenPanel<TipPanel>("名字不能为空且不能包含空格");
            return;
        }
        //填充playerdata
        playerData.id = nameIF.text;
        playerData.level = 1;
        playerData.x = -46.38f;
        playerData.y = 0.825f;
        playerData.z = -11.83f;
        playerData.ex = playerData.ey = playerData.ez = 0;
        playerData.hp = 100;
        playerData.mp = 100;
        //转成json发送给服务器
        PTCreateNewCharacter pnc = new PTCreateNewCharacter();
        pnc.playDataJson = JsonUtility.ToJson(playerData);
        this.SendCommand<SendPTCommand>(pnc);


    }

    public void OnPTCreateNewCharacter(PTBase pt)
    {
        PTCreateNewCharacter pnc = (PTCreateNewCharacter)pt;
        if (pnc.result == 0)
        {
            uiSystem.OpenPanel<TipPanel>("创建成功");
            uiSystem.OpenPanel<ChoicePanel>();
            OnClose();
        }
        else
        {
            uiSystem.OpenPanel<TipPanel>("创建失败,请重试", true);
        }
    }


    public override void OnShow(params object[] objs)
    {
        this.SendEvent<LoadOrDestoryPlayerModelGameObjectsEvent>(true);
        playerData = new PlayerData() { gender = GENDER.MALE, role = ROLE.PALADIN, race = RACE.BLOODELF };//默认值
        this.SendEvent<SetMaterialEvent>(playerData);
        this.SendEvent<RotateModelAngleEvent>();
        base.OnShow(objs);
    }


}
