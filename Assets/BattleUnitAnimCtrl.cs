using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class BattleUnitAnimCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    private StateController stateController;
    private Animator anim;
    private NavMeshAgent navMeshAgent;//通过寻路组件的速度来控制动画播放

    [Header("移动动画切换damp")]
    public float moveSpeedDampTime = 10;
    
    private float refAnimSpeed = 0;
    
    private readonly int speed = Animator.StringToHash("Speed");
    private readonly int isFighting = Animator.StringToHash("IsFighting");
    private readonly int attack = Animator.StringToHash("Attack");
    
    private State lastState;

    private PhotonView photonView;
    void Start()
    {
        stateController = GetComponent<BattleUnitBase>().stateController;
        navMeshAgent = stateController.navMeshAgent;
        anim = GetComponent<Animator>();
        photonView = PhotonView.Get(this);
    }

    // Update is called once per frame
    //不同人物写对应的脚本控制对应的变量
    protected  virtual void Update()
    {
        if (!photonView || photonView.IsMine == false)
        {
            return;
        }
        State curState = stateController.currentState;
        if (curState.stateName == "闲置" || curState.stateName == "移动" ||
            curState.stateName == "强行移动")
        {
            anim.SetBool(isFighting,false);
            anim.SetFloat(speed,
                Mathf.SmoothDamp(anim.GetFloat(speed), navMeshAgent.desiredVelocity.magnitude > 1 ? 2 : 0,
                    ref refAnimSpeed, moveSpeedDampTime*Time.deltaTime));
        }

        if (lastState != curState && curState.stateName == "战斗")//进入战斗
        {
            anim.SetBool(isFighting,true);
        }

        lastState = curState;
    }
    
    //攻击由weapon调用
    public void AttackAnim()
    {
        anim.SetTrigger(attack);
    }
    
}
