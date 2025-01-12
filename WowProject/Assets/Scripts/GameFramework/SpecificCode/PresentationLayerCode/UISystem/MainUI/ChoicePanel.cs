using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private List<GameObject> toggoleGo = new List<GameObject>();
    private bool startRotating;
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
        this.SendCommand<RegistPTListenerCommand>(new PTSrc() { ptName= "PTGetPlayerDatas", listener=OnPTGetPlayerDatas});
        this.RegisterEvent<LoadOrDestoryPlayerModelGameObjectsEvent>(LoadOrDestoryPlayerModelGameObjects);

    }

    private void OnEnterGameClick() {
        OnClose();
        uiSystem.OpenPanel<LoadPanel>();
    }

    private void OnLeftRotateCharacterClick() {
        this.SendEvent<RotateModelEvent>(-45f);
    }

    private void OnRightRotateCharacterClick()
    {
        this.SendEvent<RotateModelEvent>(45f);
    }

    private void OnOpenCreateCharacterPanelClick() {
        OnClose();
        uiSystem.OpenPanel<CharacterCreatePanel>();
    }

    private void OnDeleteCharacterClick() {
        uiSystem.OpenPanel<TipPanel>("你是否要删除该角色");

    }

    private void OnReturnToLoginPanelClick() {
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

    public void LoadOrDestoryPlayerModelGameObjects(object obj)
    {
        bool ifLoad = (bool)obj;
        if (leafBGGO == null)
        {
            leafBGGO = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Effect/LeavesPS"));
            characterGO = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Character/ChoiceModel/BloodDelf"));
        }
        leafBGGO.SetActive(ifLoad);
        characterGO.SetActive(ifLoad);
    }

    private void OnPTGetPlayerDatas(PTBase pt)
    {
        PTGetPlayerDatas ptpd = (PTGetPlayerDatas)pt;
        PlayerDatasList pdl = (PlayerDatasList)JsonUtility.FromJson(ptpd.playerDatasJson,typeof(PlayerDatasList));
        if (pdl.playerDatas.Count>0)
        {
            //有角色
            LoadOrDestoryPlayerModelGameObjects(true);
            UpdateCharacterInfo(pdl.playerDatas);
            PlayerData pd = pdl.playerDatas[pdl.choiceID];
            ChooseCharacter(pd); 
        }
    }

    //更新所有角色信息
    private void UpdateCharacterInfo(List<PlayerData> list)
    {
        //清空上一次进入页面的ui物体
        for (int i = 0; i < toggoleGo.Count; i++)
        {
            Destroy(toggoleGo[i]);    
        }
        toggoleGo.Clear();
        //更新当此进入所有ui
        for (int i = 0; i < list.Count; i++)
        {
            PlayerData pd = list[i];
            GameObject go = Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/UI/ChoicePanelItem/Toggle_CharacterInfo"),toggleGroup.transform);
            toggoleGo.Add(go);
            go.transform.GetChild(1).GetComponent<Text>().text = pd.id;
            go.transform.GetChild(2).GetComponent<Text>().text = "等级"+pd.level+" "+pd.race+" "+pd.role;
            Toggle t = go.GetComponent<Toggle>();
            t.group = toggleGroup;
            t.onValueChanged.AddListener(//onvaluechanged方法会默认返回一个selected值用来表示是否被选中
                (bool selected) => {
                    if (selected) {                        
                        ChooseCharacter(list[toggoleGo.IndexOf(go)]);
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


    private void ChooseCharacter(PlayerData pd)
    {
        this.SendEvent<SetMaterialEvent>(pd);
        this.SendCommand<SetPDValueCommand>(pd);
        this.SendEvent<RotateModelAngleEvent>();
    }




}
