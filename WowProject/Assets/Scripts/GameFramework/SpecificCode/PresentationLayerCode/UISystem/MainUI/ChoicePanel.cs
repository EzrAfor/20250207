using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： Trigger 
//功能说明：人物选择面板
//***************************************** 
public class ChoicePanel : BasePanel
{
    private Button enterGameBtn;
    private Button rotateLeftBtn;
    private Button rotateRightBtn;
    private Button createCharacterBtn;
    private Button deleteCharacterBtn;
    private Button exitBtn;
    private ToggleGroup toggleGroup;
    //private Text playerIDText;
    //下雪背景
    private GameObject leafBGGo;
    //冰霜巨龙
    private GameObject characterGo;
    private List<GameObject> togglesGo=new List<GameObject>();
    private bool startRotating;

    private int choiceID;
    private int totalCharacterNum;

    public override void OnInit()
    {
        enterGameBtn = DeepFindTransform.DeepFindChild(transform, "Btn_StartGame").GetComponent<Button>();
        rotateLeftBtn = DeepFindTransform.DeepFindChild(transform, "Btn_RotateLeft").GetComponent<Button>();
        rotateRightBtn = DeepFindTransform.DeepFindChild(transform, "Btn_RotateRight").GetComponent<Button>();
        createCharacterBtn = DeepFindTransform.DeepFindChild(transform, "Btn_CreateCharacter").GetComponent<Button>();
        deleteCharacterBtn = DeepFindTransform.DeepFindChild(transform, "Btn_DeleteCharacter").GetComponent<Button>();
        exitBtn = DeepFindTransform.DeepFindChild(transform, "Btn_Exit").GetComponent<Button>();
        toggleGroup = DeepFindTransform.DeepFindChild(transform, "CharacterInfoContent").GetComponent<ToggleGroup>();
        //playerIDText = DeepFindTransform.DeepFindChild(transform, "Text_PlayerID").GetComponent<Text>();
        enterGameBtn.onClick.AddListener(OnEnterGameClick);
        rotateLeftBtn.onClick.AddListener(OnLeftRotateCharacterClick);
        rotateRightBtn.onClick.AddListener(OnRightRotateCharacterClick);
        createCharacterBtn.onClick.AddListener(OnOpenCreateCharacterPanelClick);
        deleteCharacterBtn.onClick.AddListener(OnDeleteCharacterClick);
        exitBtn.onClick.AddListener(OnReturnToLoginPanelClick);
        base.OnInit();
        this.SendCommand<RegistPTListenerCommand>(new PTSrc() {ptName= "PTGetPlayerDatas", listener= OnPTGetPlayerDatas });
        this.RegistEvent<LoadOrDestoryPlayerModelGameObjectsEvent>(LoadOrDestoryPlayerModelGameObjects);
        this.RegistEvent<SetChoiceIDEvent>(SetPlayerChoiceID);
    }
    /// <summary>
    /// 进入游戏
    /// </summary>
    private void OnEnterGameClick()
    {
        OnClose();
        uiSystem.OpenPanel<LoadPanel>();
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
    /// 打开角色创建面板
    /// </summary>
    private void OnOpenCreateCharacterPanelClick()
    {
        OnClose();
        uiSystem.OpenPanel<CharacterCreatePanel>();
    }
    /// <summary>
    /// 删除角色创建面板
    /// </summary>
    private void OnDeleteCharacterClick()
    {
        uiSystem.OpenPanel<TipPanel>("你是否要删除该角色？");
    }
    /// <summary>
    /// 返回登录面板
    /// </summary>
    private void OnReturnToLoginPanelClick()
    {
        OnClose();
        uiSystem.OpenPanel<LoginPanel>();
    }

    public override void OnShow(params object[] objs)
    {
        base.OnShow(objs);
        this.SendCommand<SendPTCommand>(new PTGetPlayerDatas());
    }

    public override void OnClose()
    {
        base.OnClose();
        LoadOrDestoryPlayerModelGameObjects(false);
    }

    /// <summary>
	/// 加载或销毁下雪特效与冰霜巨龙
	/// </summary>
	/// <param name="ifLoad"></param>
	public void LoadOrDestoryPlayerModelGameObjects(object obj)
    {   
        bool ifLoad = (bool)obj;
        if (leafBGGo== null)
        {
            leafBGGo = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Effect/LeavesPS"));
            characterGo = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Character/ChoiceModel/BloodDelf"));
        }
        leafBGGo.SetActive(ifLoad);
        characterGo.SetActive(ifLoad);
    }
    /// <summary>
    /// 收到获取玩家信息的反馈内容
    /// </summary>
    /// <param name="pt"></param>
    private void OnPTGetPlayerDatas(PTBase pt)
    {
        PTGetPlayerDatas ptpd = (PTGetPlayerDatas)pt;
        PlayerDatasList pdl = (PlayerDatasList)JsonUtility.FromJson(ptpd.playerDatasJson,typeof(PlayerDatasList));
        if (pdl.playerDatas.Count>0)
        {
            //有角色
            LoadOrDestoryPlayerModelGameObjects(true);
            UpdateCharactersInfo(pdl.playerDatas);
            choiceID = pdl.choiceID;
            PlayerData pd = pdl.playerDatas[pdl.choiceID];
            ChooseCharacter(pd);
        }
    }
    /// <summary>
    /// 更新所有角色信息
    /// </summary>
    private void UpdateCharactersInfo(List<PlayerData> list)
    {
        //清空上一次进入页面的UI游戏物体
        for (int i = 0; i < togglesGo.Count; i++)
        {
            Destroy(togglesGo[i]);
        }
        togglesGo.Clear();
        totalCharacterNum = list.Count;
        //更新当次进入的所有UI
        for (int i = 0; i < list.Count; i++)
        {
            PlayerData pd = list[i];
            GameObject go = Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/UI/ChoicePanelItem/Toggle_CharacterInfo"), toggleGroup.transform);
            togglesGo.Add(go);
            go.transform.GetChild(1).GetComponent<Text>().text = pd.id;
            go.transform.GetChild(2).GetComponent<Text>().text = "等级"+pd.level+ " " + pd.race + " "+pd.role;
            Toggle t = go.GetComponent<Toggle>();
            t.group = toggleGroup;
            t.onValueChanged.AddListener(
                (bool selected)=>
                {
                    if (selected)
                    {
                        int index = togglesGo.IndexOf(go);
                        choiceID = index;
                        ChooseCharacter(list[index]);
                    }
                }                
            );
        }
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
    /// <summary>
    /// 选择要玩的角色
    /// </summary>
    private void ChooseCharacter(PlayerData pd)
    {
        this.GetSystem<INetSystem>().SetChoiceID(choiceID);
        this.SendEvent<SetMaterialEvent>(pd);
        this.SendCommand<SetPDValueCommand>(pd);
        this.SendEvent<ResetModelAngleEvent>();
    }
    /// <summary>
    /// 设置当前要玩的角色的索引值
    /// </summary>
    /// <param name="obj"></param>
    private void SetPlayerChoiceID(object obj)
    {
        choiceID = totalCharacterNum;
    }
}
