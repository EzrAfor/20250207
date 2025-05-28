using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

public class GameManager
{
    public static GameManager Instance;
    //private JavaScriptSerializer jss = new JavaScriptSerializer();
    private int[,,] mapDatas;
    private int xLength;
    private int yLength;
    private int zLength;
    private float gridStep;
    private float boundX;
    private float boundY;
    private float boundZ;
    //AOI
    private float AOIGridStep;
    private List<Player>[,] AOIPlayerArray;
    private int AOIMapXLength;
    private int AOIMapYLength;
    //private List<Grid> leaveGridsList = new List<Grid>();
    //private List<Grid> enterGridsList = new List<Grid>();
    //private List<Grid> leaveTemp = new List<Grid>();
   // private List<Grid> enterTemp = new List<Grid>();

    public void ReadMapDatas()
    {
        string jsonPath = "Map.json";
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,jsonPath);
        string jsonStr = File.ReadAllText(filePath);
        MapData mapData = JsonConvert.DeserializeObject<MapData>(jsonStr);
        //MapData mapData= jss.Deserialize<MapData>(jsonStr);
        xLength = mapData.xLength;
        yLength = mapData.yLength;
        zLength = mapData.zLength;
        gridStep = mapData.gridStep;
        boundX = mapData.boundX;
        boundY = mapData.boundY;
        boundZ = mapData.boundZ;
        mapDatas = Convert1DTo3D(mapData.datas,xLength,yLength,zLength);
        //JudgePointInMap(-2.403f, 0.3868f, -12.43f);//101,0,108 -2.403 0.3868 -12.43

        //AOI
        AOIGridStep = gridStep * 15;
        AOIMapXLength = mapData.AOIMapXLength;
        AOIMapYLength = mapData.AOIMapYLength;
        AOIPlayerArray = new List<Player>[AOIMapXLength,AOIMapYLength];
        for (int x = 0; x < AOIMapXLength; x++)
        {
            for (int y = 0; y < AOIMapYLength; y++)
            {
                AOIPlayerArray[x, y] = new List<Player>();
            }
        }

    }

    /// <summary>
    /// 验证当前坐标点是否在地图上
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="posZ"></param>
    public bool JudgePointInMap(float posX,float posY,float posZ)
    {
        int x = (int)Math.Round((posX-boundX)/gridStep);
        int y = (int)Math.Round((posY - boundY) / gridStep);
        int z = (int)Math.Round((posZ - boundZ) / gridStep);
        //Console.WriteLine(x+","+y+","+z);
        //判断位置是否合法
        if (x>=0&&x<xLength&&y>=0&&y<yLength&&z>=0&&z<zLength)
        {
            int value = mapDatas[x, y, z];
            if (value>=1)
            {
                //Console.WriteLine("当前点在三维数组中的位置值为1");
                return true;
            }
            else
            {
                //Console.WriteLine("当前点在三维数组中的位置值为0");
                return false;
            }
        }
        else
        {
            Console.WriteLine("当前点超出了三维数组的范围");
        }
        return false;
    }
    /// <summary>
    /// 通过索引获取地图上的确定位置
    /// </summary>
    /// <param name="x">索引</param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Vector3 GetPointPosInMap(int x,int y,int z)
    {
        return new Vector3(x*gridStep+boundX,y*gridStep+boundY,z*gridStep+boundZ);
    }

    private int[,,] Convert1DTo3D(int[] array1D, int xleng,int yleng,int zleng)
    {
        int[,,] array3D = new int[xleng,yleng,zleng];
        int index = 0;
        for (int i = 0; i < xleng; i++)
        {
            for (int j = 0; j < yleng; j++)
            {
                for (int k = 0; k < zleng; k++)
                {
                    array3D[i, j, k] = array1D[index];
                    index++;
                }
            }
        }
        return array3D;
    }
    /// <summary>
    /// 设置初始位置所在格子
    /// </summary>
    /// <param name="player"></param>
    public void SetDefaultGrid(Player player)
    {
        player.AOIGridX = (int)Math.Round((player.psd.x - boundX) / AOIGridStep);
        player.AOIGridY = (int)Math.Round((player.psd.z - boundZ) / AOIGridStep);
        AOIPlayerArray[player.AOIGridX, player.AOIGridY].Add(player);
    }
    /// <summary>
    /// 判断当前玩家在哪个格子里
    /// </summary>
    /// <param name="player"></param>
    public void AOIDetect(Player player,PTSyncCharacter pt)
    {
        //当前玩家所在的新格子索引
        int x =(int)Math.Round((player.psd.x - boundX) / AOIGridStep);
        int y= (int)Math.Round((player.psd.z - boundZ) / AOIGridStep);
        //当前玩家所在的老格子索引
        int pX = player.AOIGridX;
        int pY = player.AOIGridY;
        if (pX!=x||pY!=y)
        {
            List<Grid> leaveGridsList = new List<Grid>();
            List<Grid> enterGridsList = new List<Grid>();
            //把当前离开的九宫格（九个格子）添加进离开列表里
            AddGridsToList(pX,pY,leaveGridsList);
            //把当前进入的九宫格（九个格子）添加进进入列表里
            AddGridsToList(x, y, enterGridsList);
            //把两个九宫格之间重复的部分剔除
            RemoveBothGridsInLists(ref leaveGridsList,ref enterGridsList);
            //在当前格子区域广播离开的玩家信息，让他销毁或隐藏
            PTSyncEnterOrLeaveAOI p = new PTSyncEnterOrLeaveAOI();
            p.enterAOI = false;
            p.pd = player.psd.PSDtoPD();
            ///Console.WriteLine("当前检测到的玩家是："+player.pd.id);
            //AOIBoardcastPTMessage(p,pX,pY);
            //中心通知四周
            AOIBoardcastPTMessage(p,leaveGridsList);
            //四周通知中心
            AOIBoardcastPTMessageToCenter(leaveGridsList,false,pX,pY);
            //离开当前格子
            AOIPlayerArray[pX, pY].Remove(player);
            //进入新格子
            AOIPlayerArray[x, y].Add(player);
            player.AOIGridX = x;
            player.AOIGridY = y;
            //在当前格子区域广播进入的玩家信息，让他生成或显示
            PTSyncEnterOrLeaveAOI pp = new PTSyncEnterOrLeaveAOI();
            pp.enterAOI = true;
            pp.pd = player.psd.PSDtoPD();
            //AOIBoardcastPTMessage(pp, x,y);
            //中心通知四周
            AOIBoardcastPTMessage(pp,enterGridsList);
            //四周通知中心
            AOIBoardcastPTMessageToCenter(enterGridsList, true, x, y);
        }
        else
        {
            AOIBoardcastPTMessage(pt, x, y);
        }
    }

    /// <summary>
    /// 把对应的九宫格添加进对应列表里
    /// </summary>
    /// <param name="gridX">中心格子索引</param>
    /// <param name="gridY"></param>
    /// <param name="list"></param>
    private void AddGridsToList(int gridX, int gridY, List<Grid> list)
    {
        for (int x = gridX - 1; x < gridX + 2; x++)
        {
            if (gridX + 2 >= AOIPlayerArray.GetLength(0))
            {
                continue;
            }
            for (int y = gridY - 1; y < gridY + 2; y++)
            {
                if (gridY + 2 >= AOIPlayerArray.GetLength(1))
                {
                    continue;
                }
                list.Add(new Grid() { x = x, y = y });
            }
        }
    }
    /// <summary>
    /// 剔除两个九宫格相交的格子部分
    /// </summary>
    private void RemoveBothGridsInLists(ref List<Grid> leaveGridsList,ref List<Grid> enterGridsList)
    {
        //leaveTemp = leaveGridsList.Where(item=>!enterGridsList.Contains(item)).ToList();
        //enterTemp = enterGridsList.Where(item=>!leaveGridsList.Contains(item)).ToList();
        List<Grid> leaveTemp = leaveGridsList.ToList().Except(enterGridsList).ToList();
        //enterTemp = enterGridsList.ToList().Except(leaveGridsList).ToList();
        leaveGridsList.Clear();
        //enterGridsList.Clear();
        leaveGridsList.AddRange(leaveTemp);
        //enterGridsList.AddRange(enterTemp);
        //leaveTemp.Clear();
        //enterTemp.Clear();
    }

    /// <summary>
    /// 九宫格法AOI消息广播(中心格子向其他格子广播)
    /// </summary>
    /// <param name="pt"></param>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    public void AOIBoardcastPTMessage(PTBase pt,int gridX,int gridY)
    {
        for (int x = gridX-1; x < gridX+2; x++)
        {
            if (gridX + 2 >= AOIPlayerArray.GetLength(0))
            {
                continue;
            }
            for (int y = gridY-1; y < gridY+2; y++)
            {
                if (gridY + 2 >= AOIPlayerArray.GetLength(1))
                {
                    continue;
                }
                for (int i = 0; i < AOIPlayerArray[x, y].Count; i++)
                {
                    AOIPlayerArray[x, y][i].Send(pt);
                }
            }
        }
    }
    /// <summary>
    /// 九宫格法AOI消息广播(中心格子向其他格子广播)
    /// </summary>
    /// <param name="pt"></param>
    /// <param name="list">其他格子索引列表</param>
    public void AOIBoardcastPTMessage(PTBase pt, List<Grid> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int x = list[i].x;
            int y = list[i].y;
            for (int j = 0; j < AOIPlayerArray[x,y].Count; j++)
            {
                AOIPlayerArray[x, y][j].Send(pt);
            }
        }
    }

    /// <summary>
    /// 九宫格法AOI消息广播(其他格子向中心格子广播)
    /// </summary>
    /// <param name="list">其他格子</param>
    /// <param name="centerX">中心格子索引</param>
    /// <param name="centerY"></param>
    public void AOIBoardcastPTMessageToCenter(List<Grid> list,bool enterAOI,int centerX,int centerY)
    {
        PTSyncEnterOrLeaveAOI p = new PTSyncEnterOrLeaveAOI();
        p.enterAOI = enterAOI;
        p.otherPlayerCDList = new List<PlayerData>();
        p.pd = new PlayerData();
        for (int i = 0; i < list.Count; i++)
        {
            //获取格子
            int x = list[i].x;
            int y = list[i].y;
            //遍历格子中当前的所有玩家
            for (int j = 0; j < AOIPlayerArray[x, y].Count; j++)
            {
                p.otherPlayerCDList.Add(AOIPlayerArray[x, y][j].psd.PSDtoPD());
            }
        }
        //遍历中心格子当前的所有玩家
        for (int j = 0; j < AOIPlayerArray[centerX, centerY].Count; j++)
        {
            AOIPlayerArray[centerX, centerY][j].Send(p);
        }
    }
    /// <summary>
    /// 获取当前九宫格内的所有玩家
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns></returns>
    public List<Player> PlayersInAOIGrid(int gridX,int gridY)
    {
        List<Player> list = new List<Player>();
        for (int x = gridX - 1; x < gridX + 2; x++)
        {
            if (gridX + 2 >= AOIPlayerArray.GetLength(0))
            {
                continue;
            }
            for (int y = gridY - 1; y < gridY + 2; y++)
            {
                if (gridY + 2 >= AOIPlayerArray.GetLength(1))
                {
                    continue;
                }
                list.AddRange(AOIPlayerArray[x, y]);
            }
        }
        return list;
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

public struct Grid
{
    public int x;
    public int y;
}
