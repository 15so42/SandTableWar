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
    public string right = "Right";
    public string forward = "Forward";

    [BehaviorDesigner.Runtime.Tasks.Tooltip("nav组件速度大于这个值人物开始以最大速度移动，小于这个值不懂")]
    public float sppedThreshould = 1;
    public float maxSpeed=2;
    public float moveSpeedDampTime=10f;
    
    public float refForwardSpeed;
    public float refRightSpeed;

    public override void OnAwake()
    {
        navMeshAgent = selfUnit.Value.transform.GetComponent<NavMeshAgent>();
        anim = navMeshAgent.transform.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        float curRightSpeed = anim.GetFloat(right);
        float curForwardSpeed = anim.GetFloat(forward);

        Vector2 localDir = VectorFormWorldToLocal(navMeshAgent.desiredVelocity, transform).normalized;
        float targetRightSpeed = localDir.x;
        float targetForwardSpeed = localDir.y;
        Debug.DrawRay(transform.position,(Vector3.forward*targetForwardSpeed+Vector3.right*targetRightSpeed)*10,Color.red);
       
        anim.SetFloat(right,
            Mathf.SmoothDamp(curRightSpeed,
                targetRightSpeed,
                ref refRightSpeed, moveSpeedDampTime * Time.deltaTime));
        
        anim.SetFloat(forward,
            Mathf.SmoothDamp(curForwardSpeed,
                targetForwardSpeed,
                ref refForwardSpeed, moveSpeedDampTime * Time.deltaTime));
        
        return TaskStatus.Running;
    }
    
    Vector2 VectorFormWorldToLocal(Vector3 worldDirection, Transform localCoord)
    {
        float x = Vector3.Dot(worldDirection, localCoord.right);
        
        float y = Vector3.Dot(worldDirection, localCoord.forward);
        return new Vector2(x, y);
    }
}
