using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using JetBrains.Annotations;
using System.IO;
using Unity.VisualScripting.YamlDotNet.Core;

[CustomEditor(typeof(MapMaker))]
public class MapDataTool:Editor
{

    [MenuItem("MapDataGenerateTool/Generate NavmeshData")]
    public static void GenerateNavmeshData()
    {
        GameObject sceneMap = GameObject.Find("SilvermoonCity");
        //生成地图网格的密度 步频
        float step = 0.8f;
        MeshCollider mc = sceneMap.GetComponent<MeshCollider>();

        //用于观看效果
        GameObject mapCube = GameObject.Find("MapCube");
        GameObject map = new GameObject("Map");//方便管理
        ////通过三个维度遍历去生成可以行走的区域
        //for (float x = mc.bounds.min.x; x < mc.bounds.max.x; x += step)
        //{
        //    for (float z = mc.bounds.min.z; z < mc.bounds.max.z; z += step)
        //    {
        //        for (float y = mc.bounds.max.y; y < mc.bounds.max.y + 20; y += step)
        //        {
        //            Vector3 pos = new Vector3(x, y, z);
        //            NavMeshHit hit;
        //            //生成的网格与检测点在0.5范围内命中
        //            if (NavMesh.SamplePosition(pos, out hit, 0.5f, NavMesh.AllAreas))
        //            {
        //                GameObject g = GameObject.Instantiate(mapCube, map.transform);
        //                g.name = x + "," + y + "," + z;
        //                g.transform.position = pos;
        //                g.transform.localScale = Vector3.one * 0.9f;
        //            }
        //        }
        //    }
        //}

        //存贮数据
        int sizeX = Mathf.RoundToInt(mc.bounds.size.x / step);
        int sizeY = Mathf.RoundToInt(mc.bounds.size.y + 20 - mc.bounds.size.y / step);
        int sizeZ = Mathf.RoundToInt(mc.bounds.size.z / step);
        MapData mapData = new MapData();
        mapData.gridStep = step;
        mapData.boundX = mc.bounds.min.x;
        mapData.boundY = mc.bounds.max.y;
        mapData.boundZ = mc.bounds.min.z;
        mapData.AOIMapXLength = Mathf.RoundToInt(mc.bounds.size.x / step * 15);
        mapData.AOIMapYLength = Mathf.RoundToInt(mc.bounds.size.z/ step * 15);
        int[,,] mData = new int[sizeX, sizeY, sizeZ];
        //通过三个维度遍历去生成可以行走的区域
        for (float x = mc.bounds.min.x; x < mc.bounds.max.x; x += step)
        {
            int indexX = Mathf.RoundToInt((x - mc.bounds.min.x) / step);
            for (float z = mc.bounds.min.z; z < mc.bounds.max.z; z += step)
            {
                int indexZ = Mathf.RoundToInt((z - mc.bounds.min.z) / step);
                for (float y = mc.bounds.max.y; y < mc.bounds.max.y + 20; y += step)
                {
                    int indexY = Mathf.RoundToInt((y - mc.bounds.min.y) / step);
                    Vector3 pos = new Vector3(x, y, z);
                    NavMeshHit hit;
                    if (indexX >= sizeX)
                    {
                        indexX = sizeX - 1;
                    }
                    if (indexY >= sizeY)
                    {
                        indexY = sizeY - 1;
                    }
                    if (indexZ >= sizeZ)
                    {
                        indexZ = sizeZ - 1;
                    }
                    //生成的网格与检测点在0.5范围内命中
                    if (NavMesh.SamplePosition(pos, out hit, 0.5f, NavMesh.AllAreas))
                    {
                        GameObject g = GameObject.Instantiate(mapCube, map.transform);
                        GridPoint gp= g.GetComponent<GridPoint>();
                        gp.gridIndex = new GridIndex() {x=indexX,y=indexY,z=indexZ };
                        g.name = indexX + "," + indexY + "," + indexZ;
                        g.transform.position = pos;
                        g.transform.localScale = Vector3.one * 0.5f;
                        mData[indexX, indexY, indexZ] = 1;
                        for (int i = indexY; i < indexY + 6; i++)
                        {
                            if (i >= sizeY)
                            {
                                break;
                            }
                            mData[indexX, i, indexZ] = 1;
                        }
                    }
                }
            }
        }
        //Debug.Log(mData[54, 0, 88]);
        mapData.datas = Convert3DTo1D(mData, mapData);
        string json = JsonUtility.ToJson(mapData);
        string filePath = Application.streamingAssetsPath + "/Map.json";
        File.WriteAllText(filePath, json);
    }
    /// <summary>
    /// 将三维数组转化为一维数组
    /// </summary>
    /// <param name="array3D"></param>
    /// <param name="mapData"></param>
    /// <returns></returns>
    private static int[] Convert3DTo1D(int[,,] array3D, MapData mapData)
    {
        int xLength = array3D.GetLength(0);
        int yLength = array3D.GetLength(1);
        int zLength = array3D.GetLength(2);

        mapData.xLength = xLength;
        mapData.yLength = yLength;
        mapData.zLength = zLength;

        int size = xLength * yLength * zLength;
        int index = 0;
        int[] array1D = new int[size];
        for (int i = 0; i < xLength; i++)
        {
            for (int j = 0; j < yLength; j++)
            {
                for (int k = 0; k < zLength; k++)
                {
                    array1D[index] = array3D[i, j, k];
                    index++;
                }
            }
        }
        return array1D;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("保存当前人物AI信息"))
            {
                MapMaker.Instance.SaveCurrentAIInfo();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("清除当前人物AI信息"))
            {
                MapMaker.Instance.ClearCurrentAIInfo();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("保存当前地图人物信息数据文件"))
            {
                MapMaker.Instance.SaveMapDataFileByJson();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("清除当前地图所有信息"))
            {
                MapMaker.Instance.ClearMapDataInfo();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

}
[System.Serializable]
public class MapData
{
    public int[] datas;
    public int xLength;
    public int yLength;
    public int zLength;
    public float boundX;
    public float boundY;
    public float boundZ;
    public float gridStep;
    public int AOIMapXLength;
    public int AOIMapYLength;
}
