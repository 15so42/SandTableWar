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
    public float maxSpeed=5;
    public float moveSpeedDampTime=10f;
    
    private float refAnimSpeed;

    public override void OnStart()
    {
        navMeshAgent = selfUnit.Value.transform.GetComponent<NavMeshAgent>();
        anim = navMeshAgent.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        float curAnimSeped = anim.GetFloat(moveAnimName);
        float targetAnimSpeed = navMeshAgent.desiredVelocity.magnitude > sppedThreshould ? maxSpeed : 0;
        anim.SetFloat(moveAnimName,
            Mathf.SmoothDamp(curAnimSeped,
                targetAnimSpeed,
                ref refAnimSpeed, moveSpeedDampTime * Time.deltaTime));
       
        if (Mathf.Abs(curAnimSeped - targetAnimSpeed) < 0.01f)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
