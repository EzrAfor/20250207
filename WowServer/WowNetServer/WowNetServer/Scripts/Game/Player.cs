using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
/// <summary>
/// 玩家对象，每连接到服务器时一个客户端对应的一个玩家对象，会自动创建
/// </summary>
public class Player
{
    public string id = "";//用户名
    public ClientObject co;//属于哪一个客户端
    public PlayerSaveData psd;//当前选择人物的相关信息
    public PlayerSaveDatasList psdl;//玩家所有的角色
	public Player target;//当前玩家选择的目标玩家
	private bool readyToAttack;
	public Timer attackTimer=new Timer();
	protected bool hpHasChanged;
	public int AOIGridX;
	public int AOIGridY;
	private bool startJudgeHitEvent;//是否开始启动受击判定
	private Projectile currentPJ;
	public const int sendPTTimeval = 90;

	public Player(ClientObject clientObject)
    {
        co = clientObject;
		attackTimer.Elapsed += Attack;
	}
    public void Send(PTBase pt)
    {
		if (co==null)
		{
			return;
		}
        NetManager.Instance.Send(co,pt);
    }
	/// <summary>
	/// 开始自动攻击
	/// </summary>
	public void StartAttack()
	{
        if (psd.characterState!=CHARACTERSTATE.ATTACK)
        {
            psd.characterState = CHARACTERSTATE.BATTLE;
			attackTimer.Interval =(double)100000/ psd.rd.attackRate;
			attackTimer.Start();
		}
	}
	/// <summary>
	/// 结束自动攻击
	/// </summary>
	public void EndAttack()
	{
		attackTimer.Stop();
        //Console.WriteLine("结束攻击");
		PTSyncAttack p = new PTSyncAttack();
		p.pID = psd.id;
        if (target!=null)
        {
			p.canBeBattle = target.psd.characterState == 
				CHARACTERSTATE.DEAD ? false : true;
		}
        else
        {
			p.canBeBattle = false;
        }
		readyToAttack = false;
        //Console.WriteLine(p.canBeBattle);
		PlayerManager.Instance.BroadcastPTMessage(p);
	}
	/// <summary>
	/// 攻击
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Attack(object sender,ElapsedEventArgs e)
	{
		readyToAttack = true;
	}
	/// <summary>
	/// 被攻击
	/// </summary>
	/// <param name="attackPower">攻击者攻击力</param>
	/// <param name="hitChange">攻击者命中率</param>
	/// <param name="criticalStike">攻击者暴击率</param>
	/// <param name="level">攻击者等级</param>
	public void Hit(int attackPower,int hitChange,int criticalStike,int level)
	{
        psd.characterState = CHARACTERSTATE.HIT;
		int levelDis = psd.level-level;
		levelDis = levelDis > 0 ? levelDis : 0;
		int hitRate =85+hitChange - psd.rd.evasionRate- levelDis*5;
		//命中判定
		Random hitRandom = new Random();
		int hit= hitRandom.Next(0,100);
		bool hitTarget = hit > hitRate ? false : true;
		//暴击判定
		Random criticalRandom = new Random();
		int critical = criticalRandom.Next(0, 100);
		int criticalValue = critical > criticalStike ? 1 : 2;
		int result =hitTarget? attackPower* criticalValue - psd.rd.armor:0;
		psd.hp -= result;
		hpHasChanged = true;
	}
	public void StartHit(int attackPower, int hitChange, int criticalStike, int level, float speed, Vector3 pos)
	{
		currentPJ.SetValue(attackPower, hitChange, criticalStike,level,speed,pos);
		startJudgeHitEvent = true;
	}

	/// <summary>
	/// 攻击判定
	/// </summary>
	public void JudgeAttackEvent(bool needForward=true)
	{
        //没有自动攻击或攻击CD未冷却
        if (!readyToAttack|| psd.characterState != CHARACTERSTATE.BATTLE)
        {
            return;
        }
        //玩家选定目标为空，即没有目标   或目标已死亡
        if (target==null||target.psd.characterState==CHARACTERSTATE.DEAD)
        {
			psd.characterState = CHARACTERSTATE.IDLE;
			EndAttack();
			return;
        }
		//检测是否达到攻击范围内，到达则进行攻击
		Vector3 attacker = new Vector3(psd.x,psd.y,psd.z);
		Vector3 hitter = new Vector3(target.psd.x,target.psd.y,target.psd.z);
		Vector3 attackerForward = new Vector3(psd.fx,psd.fy,psd.fz);
        if (Vector3.Distance(attacker,hitter)<= psd.rd.attackRange)
        {
            float z = Vector3.Dot(attackerForward, hitter - attacker);
            bool isForward = z >= 0 ? true : false;
			if (!needForward||isForward)
			{
                psd.characterState = CHARACTERSTATE.ATTACK;
                switch (psd.role)
                {
					//近战
                    case ROLE.WARRIOP:
                    case ROLE.PALADIN:
					case ROLE.ROGUE:
					case ROLE.DEATHKNIGHT:
					case ROLE.DRVID:
						target.Hit(psd.rd.attackPower, psd.rd.hitChange, psd.rd.criticalStike, psd.level);
						break;
					case ROLE.HUNTER:
						target.StartHit(psd.rd.attackPower, psd.rd.hitChange, psd.rd.criticalStike, psd.level,20,new Vector3(psd.x,psd.y,psd.z));
						break;
					case ROLE.PRISST:
                    case ROLE.SHAMAM:
                    case ROLE.MAGE:
                    case ROLE.WARLOCK:
						target.StartHit(psd.rd.attackPower, psd.rd.hitChange, psd.rd.criticalStike, psd.level, 10, new Vector3(psd.x, psd.y, psd.z));
						break;
                    default:
                        break;
                }
              
                readyToAttack = false;
            }
        }
	}
	/// <summary>
	/// 受击判定
	/// </summary>
	public void JudgeHitEvent()
	{
		//如果有飞行物，先进行飞行物位置判定
        if (startJudgeHitEvent)
        {
			//受击者当前位置
			Vector3 playerPos = new Vector3(psd.x,psd.y,psd.z);
            //判断当前攻击我们的子弹离我们（即受击者）之间的距离
            if (Vector3.Distance(currentPJ.pos,playerPos)<=0.5f)
            {
				//到达攻击距离
				Hit(currentPJ.attackPower,currentPJ.hitChange,currentPJ.criticalStike,currentPJ.level);
				hpHasChanged = true;
				startJudgeHitEvent = false;
            }
            else
            {
				//子弹移动
				currentPJ.pos += (playerPos - currentPJ.pos).normalized * currentPJ.speed * sendPTTimeval / 1000;
            }

        }
		//到达目标位置时，则进行受击判定，如果是近战直接进行受击判定
        if (hpHasChanged)
        {
			hpHasChanged = false;
            if (psd.hp!=psd.rd.HP)
            {
				psd.characterState = CHARACTERSTATE.HIT;
			}
            if (psd.hp<=0)
            {
				psd.characterState = CHARACTERSTATE.DEAD;
            }
        }
    }

	//   //1.距离法
	//   private List<Player> AOIPList = new List<Player>();//AOI内的玩家
	//public void EnterAOI(Player p)
	//{
	//       if (!AOIPList.Contains(p))
	//       {
	//		//互相感兴趣
	//		AOIPList.Add(p);
	//		p.EnterAOI(this);
	//       }
	//}
	//public void LeaveAOI(Player p)
	//{
	//	if (AOIPList.Contains(p))
	//	{
	//		//互相不感兴趣
	//		AOIPList.Remove(p);
	//		p.LeaveAOI(this);
	//	}
	//}
	///// <summary>
	///// AOI消息广播
	///// </summary>
	///// <param name="pt"></param>
	//public void AOIBroadcastPTMessage(PTBase pt)
	//{
	//       for (int i = 0; i < AOIPList.Count; i++)
	//       {
	//		AOIPList[i].Send(pt);
	//       }
	//}

	////2.格子法
	//public int AOIGridX;
	//public int AOIGridY;
	///// <summary>
	///// 判断当前在哪一个AOI格子里
	///// </summary>
	///// <param name="x">新格子索引X</param>
	///// <param name="y">新格子索引Y</param>
	//public void JudgeAOIGrid(int x,int y)
	//{
 //       if (AOIGridX!=x||AOIGridY!=y)
 //       {
	//		//离开老格子
	//		LeaveAOI(AOIGridX,AOIGridY);
	//		//进入新格子
	//		EnterAOI(x,y);
	//		AOIGridX = x;
	//		AOIGridY = y;
 //       }
	//}
	//public void LeaveAOI(int x,int y)
	//{
	//	PlayerManager.Instance.AOIPlayerArray[x,y].Remove(this);
	//}
	//public void EnterAOI(int x,int y)
	//{
	//	PlayerManager.Instance.AOIPlayerArray[x, y].Add(this);
	//}
}


/// <summary>
/// 玩家信息存贮对象
/// </summary>
[Serializable]
public class PlayerSaveData
{
    public string id;//角色名
    public float x;
    public float y;
    public float z;
    public float ex;
    public float ey;
    public float ez;
    public float fx;
    public float fy;
    public float fz;
    public int coin;
    public int hp;
    public int mana;
    public int level;
    public ROLE role;
    public GENDER gender;
    public RACE race;
    public CHARACTERSTATE characterState;
    public bool isAI;
    public RoleAttributeValueData rd;//职业属性信息
    public List<SlotData> slotsList;//背包信息列表
	/// <summary>
	/// 玩家信息存贮对象转信息对象
	/// </summary>
	public PlayerData PSDtoPD()
	{
        PlayerData pd = new PlayerData();
        pd.id = id;
        pd.x = x;
        pd.y = y;
        pd.z = z;
        pd.ex = ex;
        pd.ey = ey;
        pd.ez = ez;
        pd.fx = fx;
        pd.fy = fy;
        pd.fz = fz;
        pd.hp = hp;
        pd.mana = mana;
        pd.level = level;
		pd.role = role;
		pd.race = race;
		pd.gender = gender;
        pd.characterState = characterState;
        pd.rd = rd;
		pd.isAI = isAI;
        return pd;
    }
}
/// <summary>
/// 玩家数据列表类，作为容器方便JsonUtility解析(当前账号下的角色)
/// </summary>
[Serializable]
public class PlayerSaveDatasList
{
    public int choiceID;//当前选择人物的ID
    public List<PlayerSaveData> playerSaveDatas;
}


/// <summary>
/// 玩家数据对象
/// </summary>
[Serializable]
public class PlayerData
{
	public string id;//角色名
	public float x;
	public float y;
	public float z;
	public float ex;
	public float ey;
	public float ez;
	public float fx;
	public float fy;
	public float fz;
	public int coin;
	public int hp;
	public int mana;
	public int level;
	public ROLE role;
	public GENDER gender;
	public RACE race;
	public CHARACTERSTATE characterState;
	public bool isAI;
	public RoleAttributeValueData rd;//职业属性信息

	public CharacterSyncData PDToCD()
	{
		CharacterSyncData cd = new CharacterSyncData();
		cd.id = id;
		cd.x = x;
		cd.y = y;
		cd.z = z;
		cd.ex = ex;
		cd.ey = ey;
		cd.ez = ez;
		cd.fx = fx;
		cd.fy = fy;
		cd.fz = fz;
		cd.hp = hp;
		cd.mana = mana;
		cd.level = level;
		cd.characterState = characterState;
		cd.maxHP = rd.HP;
		cd.maxMana = rd.mana;
		return cd;
	}
    /// <summary>
    /// 信息对象转玩家信息存贮对象
    /// </summary>
    /// <returns></returns>
    public PlayerSaveData PDToPSD()
    {
        PlayerSaveData psd = new PlayerSaveData();
        psd.id = id;
        psd.x = x;
        psd.y = y;
        psd.z = z;
        psd.ex = ex;
        psd.ey = ey;
        psd.ez = ez;
        psd.fx = fx;
        psd.fy = fy;
        psd.fz = fz;
        psd.hp = hp;
        psd.mana = mana;
        psd.level = level;
        psd.role = role;
        psd.race = race;
        psd.gender = gender;
        psd.characterState = characterState;
        psd.rd = rd;
		psd.isAI = isAI;
        return psd;
    }
}
/// <summary>
/// 玩家数据列表类，作为容器方便JsonUtility解析(当前账号下的角色)
/// </summary>
[Serializable]
public class PlayerDatasList
{
    public int choiceID;//当前选择人物的ID
    public List<PlayerData> playerDatas;
}




/// <summary>
/// 用于同步角色信息
/// </summary>
[Serializable]
public class CharacterSyncData
{
	public string id;//角色名
	public float x;
	public float y;
	public float z;
	public float ex;
	public float ey;
	public float ez;
	public float fx;
	public float fy;
	public float fz;
	public int hp;
	public int mana;
	public int level;
	public int maxHP;
	public int maxMana;
	public CHARACTERSTATE characterState;
}



public enum ROLE
{
	WARRIOP,//战士	
	PALADIN,//圣骑士	1
	HUNTER,//猎人2
	ROGUE,//盗贼3
	PRISST,//牧师	
	DEATHKNIGHT,//死亡骑士
	SHAMAM,//萨满
	MAGE,//法师7
	WARLOCK,//术士
	DRVID//德鲁伊9
}

public enum RACE
{
	HUMAN,
	DWARF,//矮人
	GNOME,//侏儒
	NIGHTELF,//暗夜精灵
	DRAENEI,//德莱尼
	ORC,//兽人
	TROLL,//巨魔
	FORSAKEN,//被遗忘者
	TAUREN,//牛头人
	BLOODELF//血精灵
}

public enum GENDER
{
	MALE,
	FEMALE
}

public enum CHARACTERSTATE
{
	//NONE,
	IDLE,
	MOVE,
	JUMP,
	BATTLE,
	ATTACK,
	HIT,
	DEAD
}
/// <summary>
/// 角色属性数据
/// </summary>
[Serializable]
public class RoleAttributeValueData
{
	/// <summary>
	/// 能量
	/// </summary>
	public int mana;
	/// <summary>
	/// 耐力
	/// </summary>
	public int stamina;
	/// <summary>
	/// 力量
	/// </summary>
	public int strength;
	/// <summary>
	/// 敏捷
	/// </summary>
	public int agility;
	/// <summary>
	/// 智力
	/// </summary>
	public int intellect;
	/// <summary>
	/// 精神
	/// </summary>
	public int spirit;
	/// <summary>
	/// 护甲
	/// </summary>
	public int armor;
	/// <summary>
	/// 抗性
	/// </summary>
	public int resistance;
	/// <summary>
	/// 移动速度
	/// </summary>
	public float moveSpeed;
	/// <summary>
	/// 攻击范围
	/// </summary>
	public float attackRange;
	/// <summary>
	/// 生命值
	/// </summary>
	public int HP;
	/// <summary>
	/// 攻击力
	/// </summary>
	public int attackPower;
	/// <summary>
	/// 法术强度
	/// </summary>
	public int spellPower;
	/// <summary>
	/// 攻击速度
	/// </summary>
	public int attackRate;
	/// <summary>
	/// 命中几率
	/// </summary>
	public int hitChange;
	/// <summary>
	/// 闪避率
	/// </summary>
	public int evasionRate;
	/// <summary>
	/// 暴击率
	/// </summary>
	public int criticalStike;

	public static RoleAttributeValueData operator +(RoleAttributeValueData rd1, RoleAttributeValueData rd2)
	{
		RoleAttributeValueData result = new RoleAttributeValueData();
		//基础值
		result.mana = rd1.mana + rd2.mana;
		result.stamina = rd1.stamina + rd2.stamina;
		result.strength = rd1.strength + rd2.strength;
		result.agility = rd1.agility + rd2.agility;
		result.intellect = rd1.intellect + rd2.intellect;
		result.spirit = rd1.spirit + rd2.spirit;
		result.armor = rd1.armor + rd2.armor;
		result.resistance = rd1.resistance + rd2.resistance;
		result.moveSpeed = rd1.moveSpeed + rd2.moveSpeed;
		result.attackRange = rd1.attackRange + rd2.attackRange;
		//影响值
		result.HP = result.stamina * 10;
		result.attackPower = result.strength;
		result.spellPower = result.intellect;
		result.attackRate = result.agility * 3;
		result.hitChange= result.agility / 5;
		result.evasionRate = result.agility;
		result.criticalStike = result.attackPower/2+result.intellect/5;
		return result;
	}

	public static RoleAttributeValueData operator *(RoleAttributeValueData rd1, int level)
	{
		RoleAttributeValueData result = new RoleAttributeValueData();
		level -= 1;
		//基础值
		result.mana = rd1.mana *level;
		result.stamina = rd1.stamina * level;
		result.strength = rd1.strength * level;
		result.agility = rd1.agility * level;
		result.intellect = rd1.intellect * level;
		result.spirit = rd1.spirit * level;
		result.armor = rd1.armor * level;
		result.resistance = rd1.resistance * level;
		result.moveSpeed = rd1.moveSpeed * level;
		result.attackRange = rd1.attackRange * level;
		//影响值
		result.HP = result.stamina * 10;
		result.attackPower = result.strength;
		result.spellPower = result.intellect;
		result.attackRate = result.agility * 3;
		result.hitChange = result.agility / 5;
		result.evasionRate = result.agility;
		result.criticalStike = result.attackPower / 2 + result.intellect / 5;
		return result;
	}
}

public struct Projectile
{
	public int attackPower;
	public int hitChange;
	public int criticalStike;
	public int level;
	public float speed;
	public Vector3 pos;

	public void SetValue(int attackPower, int hitChange, int criticalStike, int level,float speed,Vector3 pos)
	{
		this.attackPower = attackPower;
		this.hitChange = hitChange;
		this.criticalStike = criticalStike;
		this.level = level;
		this.speed = speed;
		this.pos = pos;
	}
}
