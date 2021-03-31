using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class BattleUnitAnimCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    //protected StateController stateController;
    protected BattleUnitBase battleUnitBase;
    protected Animator anim;
    protected NavMeshAgent navMeshAgent;//通过寻路组件的速度来控制动画播放

    [Header("移动动画切换damp")]
    public float moveSpeedDampTime = 10;
    
    private float refAnimSpeed = 0;
    
    private readonly int speed = Animator.StringToHash("Speed");
    private readonly int isFighting = Animator.StringToHash("IsFighting");
    private readonly int attack = Animator.StringToHash("Attack");
    
    protected State lastState;

    private PhotonView photonView;
    protected virtual void Start()
    {
       
        //stateController = GetComponent<BattleUnitBase>().stateController;
        battleUnitBase = GetComponent<BattleUnitBase>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        photonView = PhotonView.Get(this);
    }

    // Update is called once per frame
    //不同人物写对应的脚本控制对应的变量
    protected virtual void Update()
    {
      
    }
    
    
    //攻击由weapon调用
    public virtual void AttackAnim()
    {
        anim.SetTrigger(attack);
    }

    public virtual void DieAnim()
    {
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }
        
    }
    
}
