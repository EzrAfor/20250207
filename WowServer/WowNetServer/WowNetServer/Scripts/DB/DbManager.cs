using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Web.Script.Serialization;
/// <summary>
/// 数据库管理
/// </summary>
public class DbManager
{
    public static DbManager Instance;
    private MySqlConnection mySQL;
    //json解码编码器
    private JavaScriptSerializer jss = new JavaScriptSerializer();
    /// <summary>
    /// 连接数据库
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="userName"></param>
    /// <param name="pw"></param>
    /// <returns></returns>
    public bool Connect(string dbName,string ip,int port,string userName,string pw)
    {
        mySQL = new MySqlConnection();
        string str = string.Format("Database={0};Data Source={1};port={2};User Id={3};Password={4}",
            dbName, ip, port, userName, pw);
        mySQL.ConnectionString = str;
        try
        {
            mySQL.Open();
            Console.WriteLine("Wow数据库启动成功");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Wow数据库启动失败："+e.Message);
            return false;
        }
    }

    /// <summary>
	/// 判定安全字符串(防止SQL注入)
	/// </summary>
	/// <param name="str"></param>
	/// <returns></returns>
	private bool IsSafeString(string str)
    {
        return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
    }
    /// <summary>
    /// 判断用户是否存在
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsAccountExist(string id)
    {
        if (!IsSafeString(id))
        {
            return false;
        }
        string str = string.Format("select * from account where id='{0}';",id);
        try
        {
            MySqlCommand cmd = new MySqlCommand(str,mySQL);
            MySqlDataReader mySqlDataReader= cmd.ExecuteReader();
            bool hasRows= mySqlDataReader.HasRows;
            mySqlDataReader.Close();
            return hasRows;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库]当前用户查找异常"+e.Message);
            return false;
        }
    }
    /// <summary>
    /// 注册新账户
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pw"></param>
    /// <returns></returns>
    public bool RegistNewAccount(string id,string pw)
    {
        if (!IsSafeString(id))
        {
            Console.WriteLine("[数据库] 注册新用户失败，用户名不合法");
            return false;
        }
        if (!IsSafeString(pw))
        {
            Console.WriteLine("[数据库] 注册新用户失败，密码不合法");
            return false;
        }
        if (IsAccountExist(pw))
        {
            Console.WriteLine("[数据库] 注册新用户失败，账户已存在");
            return false;
        }
        string str= string.Format("insert into account set id='{0}',pw='{1}';",id,pw);
        try
        {
            MySqlCommand cmd = new MySqlCommand(str, mySQL);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] 注册新用户失败" + e.Message);
            return false;
        }
    }
    /// <summary>
    /// 检测校验用户名密码
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pw"></param>
    /// <returns></returns>
    public bool CheckPassword(string id,string pw)
    {
        if (!IsSafeString(id))
        {
            Console.WriteLine("[数据库] 注册新用户失败，用户名不合法");
            return false;
        }
        if (!IsSafeString(pw))
        {
            Console.WriteLine("[数据库] 注册新用户失败，密码不合法");
            return false;
        }
        string str = string.Format("select * from account where id='{0}'and pw='{1}';", id, pw);
        try
        {
            MySqlCommand cmd = new MySqlCommand(str, mySQL);
            MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
            bool hasRows = mySqlDataReader.HasRows;
            mySqlDataReader.Close();
            return hasRows;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] 当前检测校验用户名密码异常" + e.Message);
            return false;
        }
    }
    /// <summary>
    /// 获取玩家角色列表信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public PlayerSaveDatasList GetPlayerDatasList(string id)
    {
        if (!IsSafeString(id))
        {
            Console.WriteLine("[数据库] 获取玩家数据列表失败，用户名不合法");
            return null;
        }
        string str = string.Format("select * from player where id='{0}';", id);
        try
        {
            MySqlCommand cmd = new MySqlCommand(str, mySQL);
            MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
            bool hasRows = mySqlDataReader.HasRows;
            if (!hasRows)
            {
                mySqlDataReader.Close();
                return null;
            }
            mySqlDataReader.Read();
            string data= mySqlDataReader.GetString("data");
            PlayerSaveDatasList playerDatasList = jss.Deserialize<PlayerSaveDatasList>(data);
            mySqlDataReader.Close();
            return playerDatasList;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] 获取玩家角色列表信息异常" + e.Message);
            return null;
        }
    }
    /// <summary>
    /// 保存角色信息列表
    /// </summary>
    /// <param name="id"></param>
    /// <param name="playerDatasList"></param>
    /// <returns></returns>
    public bool UpdatePlayerDatasList(string id,PlayerSaveDatasList playerSaveDatasList)
    {
        string data= jss.Serialize(playerSaveDatasList);
        string str = string.Format("update player set data='{0}'where id='{1}';",data,id);
        try
        {
            MySqlCommand cmd = new MySqlCommand(str, mySQL);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] 保存角色信息列表失败" + e.Message);
            return false;
        }
    }
    /// <summary>
    /// 创建角色列表
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool CreatePlayerList(string id)
    {
        if (!IsSafeString(id))
        {
            Console.WriteLine("[数据库] 注册新用户失败，用户名不合法");
            return false;
        }
        PlayerSaveDatasList pdl = new PlayerSaveDatasList();
        pdl.playerSaveDatas = new List<PlayerSaveData>();
        string data = jss.Serialize(pdl);
        string str = string.Format("insert into player set id='{0}',data='{1}';", id, data);
        try
        {
            MySqlCommand cmd = new MySqlCommand(str, mySQL);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] 创建角色列表失败" + e.Message);
            return false;
        }
    }
}

