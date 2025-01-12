using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour,IController
{
    private CharacterController characterController;

    public float rotateSpeed=120;
    public float gravity = 9.8f;
    public float verticalVelocity = 0;
    public float MapJumpHeight = 1.7f;
    public float movespeed = 3;
    public float checkSphereRadius = 0.1f;
    public bool isGround;
    private Transform groundCheckPointTrans;
    public LayerMask groundLayer;
    private Vector3 motionVector;
    private bool isJumping;
    protected InputController ic;
    private characterFSM characterFSM;
    protected ChoiceCharacterDress ccd;
    private bool isSyncCharacter;
    private float syncInterval = 0.05f;
    private float lastSendSyncTime;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        groundCheckPointTrans = transform.Find("groundCheckPointTrans");
        ic = gameObject.AddComponent<InputController>();
        ccd = gameObject.AddComponent<ChoiceCharacterDress>();
        characterFSM = gameObject.AddComponent<characterFSM>();
        lastSendSyncTime = Time.time;
    }

   

    
    void Update()
    {
        PlayerRotateViewControl();
        PlayerMoveAndJumpControl();
        SyncUpdate();
    }

    private void PlayerRotateViewControl()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime);

    }

    private void PlayerMoveAndJumpControl() {
        motionVector = Vector3.zero;
        float h = ic.GetFloatInputValue(InputCode.HorizontalMoveValue);
        float v = ic.GetFloatInputValue(InputCode.VerticalMoveValue);

        if (characterFSM.GetCurrentState() != CHARACTERSTATE.JUMP)
        {
            JudgeAndChangeStateIdleOrMove(v, h);
        }
        this.SendEvent<RotateCharacterModelEvent>(new RotateModelSrc() { h=h,v=v});
        motionVector += transform.forward * movespeed * v * Time.deltaTime;
        motionVector += transform.right * movespeed * h * Time.deltaTime;
        isGround = Physics.CheckSphere(groundCheckPointTrans.position, checkSphereRadius, groundLayer);

        if (!isGround || isJumping) { 
            verticalVelocity -= gravity * Time.deltaTime;
            if (verticalVelocity <= 0 && isJumping && isGround) {
                isJumping = false;
                JudgeAndChangeStateIdleOrMove(v, h);
            }
        }
        motionVector += Vector3.up * verticalVelocity * Time.deltaTime;
        if (ic.GetBoolInputValue(InputCode.JumpState))
        {
            if (isGround) {
                isJumping = true;
            verticalVelocity = Mathf.Sqrt(2 * gravity * MapJumpHeight);
                characterFSM.ChangeState(CHARACTERSTATE.JUMP);
            }
        }
        characterController.Move(motionVector);
        if (isGround && !isJumping)
        {
            verticalVelocity = 0;
            JudgeAndChangeStateIdleOrMove(v, h);
            transform.position = new Vector3(transform.position.x, 0.825f, transform.position.z);
        }
    }

    private void JudgeAndChangeStateIdleOrMove(float v,float h) {
        
        if (v != 0 || h != 0)
        {
            characterFSM.ChangeState(CHARACTERSTATE.MOVE);
        }
        else
        {
            characterFSM.ChangeState(CHARACTERSTATE.IDLE);
        }
    }

    public void InitDressState(PlayerData pd)
    {
        ccd.SetMaterial(pd);
        characterFSM.InitFSM(ccd.currentCharacterGo.GetComponent<Animator>(), ic);
        gameObject.AddComponent<PlayerHeadController>().InitPlayerHeadCtrl(ccd.currentCharacterGo.transform, (int)pd.gender);
    }

    private void SyncUpdate() 
    {
        if (!isSyncCharacter) return;
        if (Time.time - lastSendSyncTime < syncInterval) return;
        lastSendSyncTime = Time.time;
        PTSyncCharacter pts = new PTSyncCharacter();
        pts.x = transform.position.x;
        pts.y = transform.position.y;
        pts.z = transform.position.z;
        pts.ex = transform.eulerAngles.x;
        pts.ey = transform.eulerAngles.y;
        pts.ez = transform.eulerAngles.z;
        pts.characterState = characterFSM.GetCurrentState();
        this.SendCommand<SendPTCommand>(pts);

    }




}
