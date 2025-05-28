using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：地图AI信息编辑的格子点
//***************************************** 
public class GridPoint : MonoBehaviour
{
    public PlayerData pd;
    public Material m;
    private Color originalColor;//初始颜色
    private Color defaultColor;//默认颜色
    public List<GridPoint> pathPoints=new List<GridPoint>();//巡逻路点
    public GridIndex gridIndex;

    private void Awake()
    {
        m = GetComponent<MeshRenderer>().material;
        originalColor = m.color;
        defaultColor = originalColor;
    }

    private void OnMouseDown()
    {
        //当前没有选择任何格子作为人物
        if (MapMaker.Instance.selectedGrid==null)
        {
            MapMaker.Instance.selectedGrid = this;
            m.color = Color.green;
            pathPoints.Add(this);
        }
        //已选格子
        else
        {
            //选中的人物格子是自己（二次点击）
            if (MapMaker.Instance.selectedGrid==this)
            {
                SetDefaultColorToMColor();
                SetPathPointsColor();
                MapMaker.Instance.selectedGrid = null;
            }
            //其他格子
            else
            {
                //如果当前选择格子已在我们编辑人物这个格子的路径里时（二次点击）
                if (MapMaker.Instance.selectedGrid.pathPoints.Contains(this))
                {
                    //把当前路径点移除
                    MapMaker.Instance.selectedGrid.pathPoints.Remove(this);
                    SetDefaultColorToMColor();
                }
                //第一次点击新格子
                else
                {
                    //作为当前NPC的新路点
                    MapMaker.Instance.selectedGrid.pathPoints.Add(this);
                    m.color = Color.red;
                }
            }
        }
    }
    /// <summary>
    /// 设置其他路径点的格子颜色
    /// </summary>
    /// <param name="setPathColor">是否把路径点设置为灰色</param>
    /// <param name="setOriginalColor">是否把路径点设置为原始颜色</param>
    public void SetPathPointsColor(bool setPathColor=false,bool setOriginalColor=false)
    {
        for (int i = 0; i < pathPoints.Count; i++)
        {
            if (setOriginalColor)
            {
                pathPoints[i].SetOriginalColor();
            }
            if (setPathColor)
            {
                if (i==0)
                {
                    continue;
                }
                pathPoints[i].SetDefaultColor(Color.gray);
            }
            //设置为默认颜色
            pathPoints[i].SetDefaultColorToMColor();
        }
    }

    /// <summary>
    /// 把对应的格子单位的材质球颜色设置为本身的默认颜色
    /// </summary>
    public void SetDefaultColorToMColor()
    {
        m.color = defaultColor;
    }
    /// <summary>
    /// 改变默认颜色
    /// </summary>
    /// <param name="color"></param>
    public void SetDefaultColor(Color color)
    {
        defaultColor = color;
    }
    /// <summary>
    /// 把原始颜色设置给默认颜色
    /// </summary>
    public void SetOriginalColor()
    {
        defaultColor = originalColor;
    }
    /// <summary>
    /// 把格子所有信息设置为默认值
    /// </summary>
    /// <param name="setPathColor">是否把路径点设置为灰色</param>
    /// <param name="setOriginalColor">是否把路径点设置为原始颜色</param>
    public void SetDefaultValue(bool setPathColor = false, bool setOriginalColor = false)
    {
        SetDefaultColorToMColor();
        SetPathPointsColor(setPathColor, setOriginalColor);
    }
    /// <summary>
    /// 清除当前AI人物的PlayerData属性
    /// </summary>
    public void ClearPDValue()
    {
        pd = null;
    }
    /// <summary>
    /// 清除当前AI所有的巡逻路点
    /// </summary>
    public void ClearPathPoints()
    {
        pathPoints.Clear();
    }
}
/// <summary>
/// 地图块索引
/// </summary>
[Serializable]
public struct GridIndex
{
    public int x;
    public int y;
    public int z;
}
/// <summary>
/// 单个人物AI的地图块信息
/// </summary>
[Serializable]
public class CharacterMapData
{
    public GridIndex gi;
    public PlayerData pd;
    public List<GridIndex> pathPoints;//巡逻路点
}
/// <summary>
/// 地图上所有的信息块（用于保存到json文件里）
/// </summary>
[Serializable]
public class CharacterMapDatasInfo
{
    public List<CharacterMapData> mapGridPointDatas;
}
