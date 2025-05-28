using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：玩家的行为控制
//***************************************** 
public class PlayerMovementController : MonoBehaviour,IController
{
    //组件
    private CharacterController characterController;
    private Transform groundCheckPointTrans;
    //旋转
    public float rotateSpeed=120;
    //移动
    public float moveSpeed;
    //跳跃
    public float gravity = 9.8f;
    public float verticalVelocity = 0;
    public float maxJumpHeight = 1.7f;
    public float checkSphereRadius = 0.1f;
    public LayerMask groundLayer;
    public bool isGround;
    private bool isJumping;
    protected InputController ic;
    public CharacterFSM cFSM;
    protected ChoiceCharacterDress ccd;
    //protected PlayerHeadController phc;
    public bool isSyncCharacer;
    protected float syncInterval=0.05f;
    private float lastSendSyncTime;
    public Transform targetTrans;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        groundCheckPointTrans = transform.Find("GroundCheckPoint");
        ic = gameObject.AddComponent<InputController>();
        ccd= gameObject.AddComponent<ChoiceCharacterDress>();
        cFSM = gameObject.AddComponent<CharacterFSM>();
        lastSendSyncTime = Time.time;
    }

    void Update()
    {
        if (cFSM.GetCurrentState()==CHARACTERSTATE.DEAD)
        {
            return;
        }
        if (!isSyncCharacer)
        {
            float h = 0, v = 0,r=0;
            PlayerInput(ref h,ref v,ref r);
            PlayerRotateViewControl(r);
            PlayerMoveAndJumpControl(h,v);            
        }
        SyncUpdate();
    }

    private void PlayerInput(ref float h,ref float v,ref float r)
    {        
        h = ic.GetFloatInputValue(InputCode.HorizontalMoveValue);
        v = ic.GetFloatInputValue(InputCode.VerticalMoveValue);
        r = ic.GetFloatInputValue(InputCode.HorizontalRotateValue);
        //当前客户端玩家选择操作目标
        if (ic.GetBoolInputValue(InputCode.ChoiceTarget))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.transform.CompareTag("Character") && raycastHit.transform.name != transform.name)
                {
                    targetTrans = raycastHit.transform;
                    PTSyncSetChoiceTarget p = new PTSyncSetChoiceTarget();
                    p.pID = transform.name;
                    p.tID = raycastHit.transform.name;
                    this.SendCommand<SendPTCommand>(p);
                    //this.SendCommand<SetTargetPDValueCommand>(raycastHit.transform.name);
                }
                else
                {
                    targetTrans = null;
                    PTSyncSetChoiceTarget p = new PTSyncSetChoiceTarget();
                    p.pID = transform.name;
                    p.tID = null;
                    this.SendCommand<SendPTCommand>(p);
                    //this.SendCommand<SetTargetPDValueCommand>(null);
                }
            }
        }
        //是否开启自动攻击
        if (targetTrans != null && ic.GetBoolInputValue(InputCode.BattleState))
        {
            //进入战斗状态
            PTSyncAttack p = new PTSyncAttack();
            p.pID = transform.name;
            this.SendCommand<SendPTCommand>(p);
        }
    }

    private void PlayerRotateViewControl(float r)
    {
        transform.Rotate(Vector3.up*r*rotateSpeed*Time.deltaTime);
    }

    private void PlayerMoveAndJumpControl(float h,float v)
    {
        Vector3 motionVector = Vector3.zero;

        if (cFSM.GetCurrentState() != CHARACTERSTATE.JUMP)
        {
            JudgeAndChangeStateIdleOrMove(v, h);
        }
        //ccd.RotateCharacterModel(h,v);
        //phc.GetLookTargetIndex(ccd.offsetAngle);
        motionVector += transform.forward * moveSpeed * v * Time.deltaTime;
        motionVector += transform.right * moveSpeed * h * Time.deltaTime;
        isGround = Physics.CheckSphere(groundCheckPointTrans.position, checkSphereRadius, groundLayer);
        if (!isGround||isJumping)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            if (verticalVelocity <= 0 && isJumping && isGround)
            {
                isJumping = false;
                JudgeAndChangeStateIdleOrMove(v, h);
            }
        }
        motionVector += Vector3.up * verticalVelocity * Time.deltaTime;
        if (ic.GetBoolInputValue(InputCode.JumpState))
        {
            if (isGround)
            {
                isJumping = true;
                verticalVelocity = Mathf.Sqrt(2 * gravity * maxJumpHeight);
                cFSM.ChangeState(CHARACTERSTATE.JUMP);
            }
        }
        characterController.Move(motionVector);
        if (isGround && !isJumping)
        {
            verticalVelocity = 0;
            JudgeAndChangeStateIdleOrMove(v, h);
            //transform.position = new Vector3(transform.position.x, 0.825f, transform.position.z);
        }
    }
    /// <summary>
    /// 判断状态切为idle还是Move
    /// </summary>
    private void JudgeAndChangeStateIdleOrMove(float v,float h)
    {
        if (v != 0 || h != 0)
        {
            cFSM.ChangeState(CHARACTERSTATE.MOVE);
        }
        else
        {
            if (cFSM.beBattle)
            {
                cFSM.ChangeState(CHARACTERSTATE.BATTLE);
            }
            else
            {
                cFSM.ChangeState(CHARACTERSTATE.IDLE);
            }
        }
    }
    /// <summary>
    /// 设置当前角色外观
    /// </summary>
    /// <param name="pd"></param>
    public void InitDressState(PlayerData pd)
    {
        ccd.SetMaterial(pd);
        //phc = gameObject.AddComponent<PlayerHeadController>();
        //phc.InitPlayerHeadCtrl(ccd.currentCharacterGo.transform, (int)pd.gender);
        cFSM.InitFSM(ccd.currentCharacterGo.GetComponent<Animator>(), ic/*,phc*/,this);       
    }
    /// <summary>
    /// 发送同步信息
    /// </summary>
    public void SyncUpdate()
    {
        if (isSyncCharacer)
        {
            return;
        }
        if (Time.time-lastSendSyncTime<syncInterval)
        {
            return;
        }
        lastSendSyncTime = Time.time;
        PTSyncCharacter pts = new PTSyncCharacter();
        pts.cd = new CharacterSyncData();
        pts.cd.x = transform.position.x;
        pts.cd.y = transform.position.y;
        pts.cd.z = transform.position.z;
        pts.cd.ex = (int)transform.eulerAngles.x;
        pts.cd.ey = (int)transform.eulerAngles.y;
        pts.cd.ez = (int)transform.eulerAngles.z;
        pts.cd.fx = transform.forward.x;
        pts.cd.fy = transform.forward.y;
        pts.cd.fz = transform.forward.z;
        pts.cd.characterState = cFSM.GetCurrentState();
        this.SendCommand<SendPTCommand>(pts);
    }
    /// <summary>
    /// 把角色对应的必要表现属性值设置给对应变量
    /// </summary>
    /// <param name="rd"></param>
    public void SetPlayerDataValue(RoleAttributeValueData rd)
    {
        moveSpeed = rd.moveSpeed;
    }
}
