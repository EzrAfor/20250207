using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：地图AI信息编辑工具管理
//***************************************** 
public class MapMaker : MonoBehaviour
{
    public static MapMaker Instance;
    //当前正在作为AI编辑信息的格子引用
    public GridPoint selectedGrid;
    //存放当前所有已经保存信息的格子对象（作为人物的格子）
    public Dictionary<GridIndex, GridPoint> gridDict = new Dictionary<GridIndex, GridPoint>();
    //存放当前所有已经保存格子的信息对象
    public Dictionary<GridIndex, CharacterMapData> dataDic = new Dictionary<GridIndex, CharacterMapData>();

    private void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// 保存当前人物AI信息
    /// </summary>
    public void SaveCurrentAIInfo()
    {
        if (dataDic.ContainsKey(selectedGrid.gridIndex))
        {
            //已有当前格子信息时，需要把新设置的信息更新一下
            dataDic[selectedGrid.gridIndex].gi = selectedGrid.gridIndex;
            dataDic[selectedGrid.gridIndex].pd = selectedGrid.pd;
            dataDic[selectedGrid.gridIndex].pathPoints.Clear();
            for (int i = 0; i < selectedGrid.pathPoints.Count; i++)
            {
                dataDic[selectedGrid.gridIndex].pathPoints.
                    Add(selectedGrid.pathPoints[i].gridIndex);
            }
        }
        else
        {
            CharacterMapData cmd = new CharacterMapData()
            {
                gi = selectedGrid.gridIndex,
                pd = selectedGrid.pd,
                pathPoints = new List<GridIndex>()
            };
            for (int i = 0; i < selectedGrid.pathPoints.Count; i++)
            {
                cmd.pathPoints.Add(selectedGrid.pathPoints[i].gridIndex);
            }
            dataDic.Add(selectedGrid.gridIndex,cmd);
            gridDict.Add(selectedGrid.gridIndex,selectedGrid);
        }
        selectedGrid.SetDefaultColor(Color.magenta);
        selectedGrid.SetDefaultValue(true);
        selectedGrid = null;
    }
    /// <summary>
    /// 清除当前人物AI信息
    /// </summary>
    public void ClearCurrentAIInfo()
    {
        if (selectedGrid=null)
        {
            print("当前没有选择地图块");
            return;
        }
        selectedGrid.SetPathPointsColor();
        selectedGrid.ClearPathPoints();
    }
    /// <summary>
    /// 保存当前地图人物信息数据文件
    /// </summary>
    public void SaveMapDataFileByJson()
    {
        CharacterMapDatasInfo cmdi = new CharacterMapDatasInfo();
        List<CharacterMapData> mapGridPointDatas = new List<CharacterMapData>();
        foreach (var item in dataDic)
        {
            mapGridPointDatas.Add(item.Value);
        }
        cmdi.mapGridPointDatas = mapGridPointDatas;
        string json= JsonUtility.ToJson(cmdi);
        string filePath = Application.streamingAssetsPath + "/MapAIInfo.json";
        File.WriteAllText(filePath,json);
        ClearMapDataInfo();
    }
    /// <summary>
    /// 清除当前地图所有信息
    /// </summary>
    public void ClearMapDataInfo()
    {
        foreach (var item in gridDict)
        {
            item.Value.SetOriginalColor();
            item.Value.SetDefaultValue(false,true);
            item.Value.ClearPathPoints();
            item.Value.ClearPDValue();
        }
        dataDic.Clear();
        gridDict.Clear();
    }
}
