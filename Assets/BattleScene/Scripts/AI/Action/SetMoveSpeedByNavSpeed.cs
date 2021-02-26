using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class SetMoveSpeedByNavSpeed : Action
{
    public SharedBattleUnit selfUnit;
    private NavMeshAgent navMeshAgent;
    private Animator anim;
    public string moveAnimName = "Speed";

    [BehaviorDesigner.Runtime.Tasks.Tooltip("nav组件速度大于这个值人物开始以最大速度移动，小于这个值不懂")]
    public float sppedThreshould = 1;
    public float maxSpeed=2;
    public float moveSpeedDampTime=10f;
    
    public float refAnimSpeed;

    public override void OnAwake()
    {
        navMeshAgent = selfUnit.Value.transform.GetComponent<NavMeshAgent>();
        anim = navMeshAgent.transform.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        float curAnimSeped = anim.GetFloat(moveAnimName);
        float targetAnimSpeed = navMeshAgent.desiredVelocity.magnitude > sppedThreshould ? maxSpeed : 0;
        anim.SetFloat(moveAnimName,
            Mathf.SmoothDamp(curAnimSeped,
                targetAnimSpeed,
                ref refAnimSpeed, moveSpeedDampTime * Time.deltaTime));
        
        return TaskStatus.Running;
    }
}
