using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： Trigger 
//功能说明：
//***************************************** 
public class SyncPMCtrl : PlayerMovementController
{
    //上一次的目标
    public Vector3 lastPos;
    private Vector3 lastRot;
    //当次的目标
    public Vector3 targetPos;
    private Vector3 targetRot;
    //最近一次收到的位置同步协议的时间
    private float receiveTime;
    //人物状态
    private CHARACTERSTATE characterState;

    public bool isAI;
    void Start()
    {
        ic.isSyncCharacter = true;
        lastPos= targetPos = transform.position;
        lastRot = targetRot= transform.eulerAngles;
    }

    //private void OnEnable()
    //{
    //    if (cFSM!=null)
    //    {
    //        cFSM.ChangeState(characterState);
    //    }
    //}

    void Update()
    {
        if (cFSM.GetCurrentState() == CHARACTERSTATE.DEAD)
        {
            return;
        }
        UpdatePosAndRot();
    }
    /// <summary>
    /// 立刻更新当前位置与旋转角度
    /// </summary>
    public void ImmediateUpdateSyncPosAndRot(Vector3 pos,Vector3 rot,CHARACTERSTATE c)
    {
        lastPos = targetPos = transform.position = pos;
        lastRot = targetRot = transform.eulerAngles = rot;
        characterState = c;
        cFSM.ChangeState(characterState);
    }
    public float x, y, z;
    /// <summary>
    /// 同步位置和旋转
    /// </summary>
    public void SyncPosAndRot(PTSyncCharacter pt)
    {
        Vector3 pos = new Vector3(pt.cd.x, pt.cd.y, pt.cd.z);
        Vector3 rot = new Vector3(pt.cd.ex, pt.cd.ey, pt.cd.ez);
        targetRot = rot;
        //Debug.Log(targetRot);
        targetPos = pos + (pos - lastPos) / 2;
        characterState = pt.cd.characterState;
        lastPos = targetPos;
        lastRot = targetRot;
        receiveTime = Time.time;
        x = 0; z = 0;
        if (Vector3.Distance(transform.position, targetPos) <= 0.05f)
        {
            x = z = 0;
        }
        else
        {
            x = Vector3.Cross(transform.forward, targetPos - transform.position).y;
            x = x > 0 ? 1 : -1;
            z = Vector3.Dot(transform.forward, targetPos - transform.position);
            z = z > 0 ? 1 : -1;

            if (ForwardBehindOrLeftRight(targetPos))
            {
                x = 0;
            }
            else//左右
            {
                z = 0;
            }
        }
        ic.SetInputValue(InputCode.VerticalMoveValue, z);
        ic.SetInputValue(InputCode.HorizontalMoveValue, x);
        cFSM.ChangeState(characterState);
        if (isAI)
        {
            SyncAI();
        }
        else
        {
            SyncPlayer();
        }
    }

    private void SyncPlayer()
    {
        y = targetRot.y - transform.eulerAngles.y;
        if (y > 0)
        {
            y = 1;
        }
        else if (y < 0)
        {
            y = -1;
        }
        else
        {
            y = 0;
        }
        ic.SetInputValue(InputCode.HorizontalRotateValue, y);
    }

    private void SyncAI()
    {
        if (targetTrans!=null)
        {
            Vector3 tPos = targetTrans.position;
            transform.LookAt(new Vector3(tPos.x, transform.position.y, tPos.z));
        }
        else
        {
            transform.LookAt(new Vector3(targetPos.x, transform.position.y, targetPos.z));
        }
        cFSM.ChangeState(characterState);
    }

    /// <summary>
    /// 判断前后左右影响度（true前后）
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private bool ForwardBehindOrLeftRight(Vector3 targetPos)
    {
        targetPos=transform.worldToLocalMatrix.MultiplyPoint3x4(targetPos);
        if (Mathf.Abs(targetPos.z)-Mathf.Abs(targetPos.x)>=-0.05f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 更新同步信息
    /// </summary>
    private void UpdatePosAndRot()
    {
        float t = (Time.time - receiveTime) / syncInterval;
        t = Mathf.Clamp(t,0,1);
        transform.position = Vector3.Lerp(transform.position,targetPos,t);
        if (!isAI)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRot), t);
        }
    }
}
