using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：UI系统
//***************************************** 
public class UISystem : IUISystem
{

    private Dictionary<string, BasePanel> panelDict = new Dictionary<string, BasePanel>();
    private Transform canvasTrans;
    private PlayerData choiePlayerData;
    public PlayerData ChoiePlayerData { get => choiePlayerData; set => choiePlayerData = value; }

    public ItemData[] itemsList;

    public void Init()
    {
        canvasTrans = GameObject.Find("Canvas").transform;
        AddPanelToDict<LoginPanel>();
        AddPanelToDict<RegisterPanel>();
        AddPanelToDict<MaskPanel>();
        AddPanelToDict<ChoicePanel>();
        AddPanelToDict<TipPanel>();
        AddPanelToDict<LoadPanel>();
        AddPanelToDict<CharacterCreatePanel>();
        OpenPanel<LoginPanel>();
        string jsonStr = File.ReadAllText(Application.streamingAssetsPath + "/ItemInfo.json");
        ItemDataList idl = JsonUtility.FromJson<ItemDataList>(jsonStr);
        itemsList=idl.itemDatas;
        //for (int i = 0; i < itemsList.Length; i++)
        //{
        //    Debug.Log(itemsList[i].description);
        //}
    }
    /// <summary>
    /// 添加面板到UI系统
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void AddPanelToDict<T>() where T:BasePanel
    {
        string panelType= typeof(T).ToString();
        if (panelDict.ContainsKey(panelType))
        {
            Debug.Log("当前字典已有："+panelType+"类型的面板");
            return;
        }
        GameObject panelGo= GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/UI/" + panelType), canvasTrans);
        panelDict.Add(panelType, panelGo.AddComponent<T>());
        panelDict[panelType].OnInit();
    }
    /// <summary>
    /// 从UI系统移除面板
    /// </summary>
    /// <param name="panelType"></param>
    private void RemovePanelInDict(string panelType)
    {
        if (!panelDict.ContainsKey(panelType))
        {
            Debug.Log("当前字典不包含：" + panelType + "类型的面板");
            return;
        }
        //Debug.Log("移除了"+ panelType);
        GameObject panelGo = panelDict[panelType].gameObject;
        panelDict.Remove(panelType);
        GameObject.Destroy(panelGo);
    }

    public void OpenPanel<T>(params object[] objs) where T : BasePanel
    {
        string panelType = typeof(T).ToString();
        if (!panelDict.ContainsKey(panelType))
        {
            Debug.Log("当前字典不包含：" + panelType + "类型的面板");
            return;
        }
        panelDict[panelType].OnShow(objs);
    }

    public void ClosePanel(string panelType)
    {
        if (!panelDict.ContainsKey(panelType))
        {
            Debug.Log("当前字典不包含：" + panelType + "类型的面板");
            return;
        }
        panelDict[panelType].OnClose();
    }

    public void UnLoadLoginPanelsAndLoadGamePanels()
    {
        RemovePanelInDict("LoginPanel");
        RemovePanelInDict("RegisterPanel");
        RemovePanelInDict("MaskPanel");
        RemovePanelInDict("ChoicePanel");
        RemovePanelInDict("TipPanel");
        RemovePanelInDict("LoadPanel");
        RemovePanelInDict("CharacterCreatePanel");
        AddPanelToDict<GamePanel>();
        AddPanelToDict<CharacterPanel>();
        AddPanelToDict<InventoryPanel>();
        OpenPanel<GamePanel>();
    }
    /// <summary>
    /// 通过物品ID获取物品信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ItemData GetItemDataByID(int id)
    {
        return itemsList[id];
    }
}
