using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： Trigger 
//功能说明：人物创建面板
//***************************************** 
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
        rotateLeftBtn = DeepFindTransform.DeepFindChild(transform, "Btn_RotateLeft").GetComponent<Button>();
        rotateLeftBtn.onClick.AddListener(OnLeftRotateCharacterClick);
        rotateRightBtn = DeepFindTransform.DeepFindChild(transform, "Btn_RotateRight").GetComponent<Button>();
        rotateRightBtn.onClick.AddListener(OnRightRotateCharacterClick);
        exitBtn = DeepFindTransform.DeepFindChild(transform, "Btn_Exit").GetComponent<Button>();
        exitBtn.onClick.AddListener(OnReturnToChoicePanelClick);
        agreeBtn = DeepFindTransform.DeepFindChild(transform, "Btn_Agree").GetComponent<Button>();
        agreeBtn.onClick.AddListener(OnAgreeCreateClick);
        nameIF = DeepFindTransform.DeepFindChild(transform, "IF_Name").GetComponent<InputField>();
        //性别
        maleTGGo = DeepFindTransform.DeepFindChild(transform, "TG_RaceMale").gameObject;
        femaleTGGo = DeepFindTransform.DeepFindChild(transform, "TG_RaceFemale").gameObject;
        Transform sexListTrans = DeepFindTransform.DeepFindChild(transform, "Emp_SexList");
        //选择性别
        sexListTrans.GetChild(0).GetComponent<Toggle>().onValueChanged.AddListener((bool selected) =>
        {
            if (selected)
            {
                maleTGGo.SetActive(true);
                femaleTGGo.SetActive(false);
                playerData.gender = GENDER.MALE;
                this.SendEvent<SetMaterialEvent>(playerData);
            }
        });
        sexListTrans.GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener((bool selected) =>
        {
            if (selected)
            {
                maleTGGo.SetActive(false);
                femaleTGGo.SetActive(true);
                playerData.gender = GENDER.FEMALE;
                this.SendEvent<SetMaterialEvent>(playerData);
            }
        });
        //选择种族
        //男
        for (int i = 0; i < 10; i++)
        {
            Transform t = maleTGGo.transform.GetChild(i);
            maleRaceTrans.Add(t);
            t.GetComponent<Toggle>().onValueChanged.AddListener((bool selected) =>
            {
                if (selected)
                {
                    playerData.race = (RACE)maleRaceTrans.IndexOf(t);
                    this.SendEvent<SetMaterialEvent>(playerData);
                }
            });
        }
        //女
        for (int i = 0; i < 10; i++)
        {
            Transform t = femaleTGGo.transform.GetChild(i);
            femaleRaceTrans.Add(t);
            t.GetComponent<Toggle>().onValueChanged.AddListener((bool selected) =>
            {
                if (selected)
                {
                    playerData.race = (RACE)maleRaceTrans.IndexOf(t);
                    this.SendEvent<SetMaterialEvent>(playerData);
                }
            });
        }
        //职业
        Transform roleListTrans = DeepFindTransform.DeepFindChild(transform, "Emp_RoleList");
        for (int i = 0; i < 10; i++)
        {
            Transform t = roleListTrans.GetChild(i);
            roleTrans.Add(t);
            t.GetComponent<Toggle>().onValueChanged.AddListener((bool selected) =>
            {
                if (selected)
                {
                    playerData.role = (ROLE)roleTrans.IndexOf(t);
                    this.SendEvent<SetMaterialEvent>(playerData);
                }
            });
        }
        this.SendCommand<RegistPTListenerCommand>
            (new PTSrc()
            { ptName = "PTCreateNewCharacter", 
                listener = OnPTCreateNewCharacter
            });
        base.OnInit();
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
            this.SendEvent<RotateModelEvent>(Input.GetAxisRaw("Mouse X")*2);
        }
    }

    /// <summary>
    /// 向左旋转选择人物模型
    /// </summary>
    private void OnLeftRotateCharacterClick()
    {
        this.SendEvent<RotateModelEvent>(-45f);
    }
    /// <summary>
    /// 向右旋转选择人物模型
    /// </summary>
    private void OnRightRotateCharacterClick()
    {
        this.SendEvent<RotateModelEvent>(45f);
    }
    /// <summary>
    /// 返回角色选择面板
    /// </summary>
    private void OnReturnToChoicePanelClick()
    {
        OnClose();
        uiSystem.OpenPanel<ChoicePanel>();
    }
    /// <summary>
    /// 同意角色创建
    /// </summary>
    private void OnAgreeCreateClick()
    {
        if (nameIF.text==null||nameIF.text.Contains(" "))
        {
            uiSystem.OpenPanel<TipPanel>("名字不能为空且不能包含空格");
            return;
        }
        playerData.id = nameIF.text;
        playerData.level = 1;
        playerData.x = 0;
        playerData.y = 0;
        playerData.z = 0;
        playerData.ex = playerData.ey = playerData.ez = 0;
        PTCreateNewCharacter pnc= new PTCreateNewCharacter();
        pnc.playerDataJson = JsonUtility.ToJson(playerData);
        this.SendCommand<SendPTCommand>(pnc);
    }
    /// <summary>
    /// 收到创建角色成功协议
    /// </summary>
    /// <param name="pt"></param>
    private void OnPTCreateNewCharacter(PTBase pt)
    {
        PTCreateNewCharacter pnc = (PTCreateNewCharacter)pt;
        if (pnc.result==0)
        {
            uiSystem.OpenPanel<TipPanel>("创建成功");
            this.SendEvent<SetChoiceIDEvent>();
            uiSystem.OpenPanel<ChoicePanel>();
            PlayerData pd = JsonUtility.FromJson<PlayerData>(pnc.playerDataJson);
            this.SendCommand<SetPDValueCommand>(pd);
            OnClose();
        }
        else
        {
            uiSystem.OpenPanel<TipPanel>("创建失败，请重试",true);
        }
    }

    public override void OnShow(params object[] objs)
    {
        this.SendEvent<LoadOrDestoryPlayerModelGameObjectsEvent>(true);
        playerData = new PlayerData() { gender=GENDER.MALE,role=ROLE.PALADIN,race=RACE.BLOODELF};
        this.SendEvent<SetMaterialEvent>(playerData);
        this.SendEvent<ResetModelAngleEvent>();
        base.OnShow(objs);
    }
}
