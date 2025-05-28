using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

public class AIPlayer : Player
{
    public Timer sendPtTimer = new Timer(sendPTTimeval);
    public float chaseRange = 5;
    public float safeRange=10;
    private Vector3 initPos;//开始追击的初始位置
    private List<GridIndex> patrolPath = new List<GridIndex>();
    private int pathIndex = 0;
    private Vector3 targetPatrolPath;

    public AIPlayer(ClientObject clientObject, PlayerSaveData playerData,List<GridIndex> pplist) : base(clientObject)
    {
        psd = new PlayerSaveData();
        id = psd.id = playerData.id;
        psd.level = playerData.level;
        psd.gender = playerData.gender;
        psd.isAI = true;
        psd.role = playerData.role;
        patrolPath.AddRange(pplist);
        GridIndex fgi = patrolPath[0];
        targetPatrolPath = initPos = GameManager.Instance.GetPointPosInMap(fgi.x,fgi.y,fgi.z);
        psd.rd = PlayerManager.Instance.GetBasicRoleAttributeValueData(psd.role)+ 
            PlayerManager.Instance.GetGrowthRoleAttributeValueData(psd.role)*2;
        //GameManager.Instance.SetDefaultGrid(this);
        psd.x = targetPatrolPath.x;
        psd.y = targetPatrolPath.y;
        psd.z = targetPatrolPath.z;
        psd.hp = psd.rd.HP;
        psd.mana = psd.rd.mana;
        //Console.WriteLine(psd.rd.strength);
        sendPtTimer.Elapsed += SendPTToPlayer;
        sendPtTimer.Start();
    }
    /// <summary>
    /// 定时向当前AOI区域内的玩家发送自身状态信息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void SendPTToPlayer(object sender, ElapsedEventArgs e)
    {
        if (psd.characterState==CHARACTERSTATE.DEAD)
        {
            //NPC已死
            sendPtTimer.Stop();
            return;
        }
        PTSyncCharacter ptsc = new PTSyncCharacter();
        //NPC位置
        Vector3 npcPos = new Vector3(psd.x,psd.y,psd.z);
        if (target != null)
        {
            //有攻击目标
            Vector3 tPos = new Vector3(target.psd.x, target.psd.y, target.psd.z);
            if (Vector3.Distance(npcPos, tPos) >=psd.rd.attackRange)
            {
                //没有到达攻击范围
                psd.characterState = CHARACTERSTATE.MOVE;
                //朝目标移动
                npcPos = npcPos + (tPos - npcPos).normalized * sendPTTimeval / 1000 * psd.rd.moveSpeed;
            }
            else
            {
                //到达攻击范围
                if (target.psd.hp > 0)
                {
                    //目标未死
                    psd.characterState = CHARACTERSTATE.BATTLE;
                }
                else
                {
                    //目标已阵亡
                    target = null;
                    //重置当前目标并通知当前AOI区域的其他玩家
                    PTSyncSetChoiceTarget p = new PTSyncSetChoiceTarget();
                    p.pID = psd.id;
                    p.tID = null;
                    GameManager.Instance.AOIBoardcastPTMessage(p, AOIGridX, AOIGridY);
                }
            }
        }
        else
        {
            //无攻击目标
            psd.characterState = CHARACTERSTATE.MOVE;
            //我们离开始追击的位置是不是大于1米（代表我们不是平时的巡逻状态，
            //而是去做了其他事情需要返回）
            if (Vector3.Distance(npcPos, initPos) > 1)
            {
                //返回初始位置
                npcPos += (initPos - npcPos).normalized * sendPTTimeval / 1000 * psd.rd.moveSpeed;
            }
            else
            {
                //正常巡逻状态
                npcPos= Patrol(npcPos);
                initPos = npcPos;
                //回到正常巡逻状态时，血量需要回满
                if (psd.hp != psd.rd.HP)
                {
                    psd.hp = psd.rd.HP;
                    hpHasChanged = true;
                }
            }
        }
        psd.x = npcPos.x;
        psd.y = npcPos.y;
        psd.z = npcPos.z;
        //psd.characterState = CHARACTERSTATE.MOVE;
        //攻击判定
        JudgeAttackEvent(false);
        //受击判定
        JudgeHitEvent();
        //目标搜索判定
        JudgeTargetEvent(npcPos);
        ptsc.cd = psd.PSDtoPD().PDToCD();
        GameManager.Instance.AOIDetect(this, ptsc);
    }
    /// <summary>
    /// 搜索当前AOI区域内的玩家以寻找攻击目标
    /// </summary>
    /// <param name="npcPos"></param>
    public void JudgeTargetEvent(Vector3 npcPos)
    {
        if (target!=null)
        {
            //有目标不搜索
            return;
        }
        List<Player> l= GameManager.Instance.PlayersInAOIGrid(AOIGridX,AOIGridY);
        float minDis = float.MaxValue;
        //遍历当前AOI区域内所有的玩家，找到离我们最近的作为攻击对象
        for (int i = 0; i < l.Count; i++)
        {
            Player p = l[i];
            if (p.psd.id==psd.id||p.psd.characterState==CHARACTERSTATE.DEAD)
            {
                continue;
            }
            Vector3 pPos = new Vector3(p.psd.x,p.psd.y,p.psd.z);
            float dis= Vector3.Distance(npcPos,pPos);
            if (dis<=chaseRange)
            {
                if (dis<minDis)
                {
                    minDis = dis;
                    target = p;
                }
            }
        }
        //如果搜索到目标，则发送设置目标的协议告诉所有AOI区域内的玩家，并开始攻击
        if (target!=null)
        {
            PTSyncSetChoiceTarget p = new PTSyncSetChoiceTarget();
            p.pID = psd.id;
            p.tID = target.psd.id;
            GameManager.Instance.AOIBoardcastPTMessage(p,AOIGridX,AOIGridY);
            StartAttack();
        }
    }

    public Vector3 Patrol(Vector3 npcPos)
    {
        if (Vector3.Distance(targetPatrolPath,npcPos)<=0.5f)
        {
            pathIndex++;
            if (pathIndex>=patrolPath.Count)
            {
                pathIndex = 0;
            }
            GridIndex fgi = patrolPath[pathIndex];
            targetPatrolPath = GameManager.Instance.GetPointPosInMap(fgi.x,fgi.y,fgi.z);
        }
        else
        {
           npcPos+= (targetPatrolPath - npcPos).normalized * psd.rd.moveSpeed * sendPTTimeval / 1000;
        }
        return npcPos;
    }
}
