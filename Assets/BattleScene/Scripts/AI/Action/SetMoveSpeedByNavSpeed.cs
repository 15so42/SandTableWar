using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tactical.Tasks;
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
    
    public float SpeedAnimDampTime=10f;
    
    private float refForwardSpeed;
    private float refRightSpeed;

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
        float animSpeedRate = (float) navMeshAgent.velocity.magnitude / navMeshAgent.speed;//导航组件当前速度与最大速度的比值
        float targetRightSpeed = localDir.x*animSpeedRate;
        float targetForwardSpeed = localDir.y*animSpeedRate;
        Debug.DrawRay(transform.position,(transform.forward*targetForwardSpeed+transform.right*targetRightSpeed)*10,Color.red);
       
        anim.SetFloat(right,
            Mathf.SmoothDamp(curRightSpeed,
                targetRightSpeed,
                ref refRightSpeed, SpeedAnimDampTime * Time.deltaTime));
        
        anim.SetFloat(forward,
            Mathf.SmoothDamp(curForwardSpeed,
                targetForwardSpeed,
                ref refForwardSpeed, SpeedAnimDampTime * Time.deltaTime));

        if (Vector3.Distance(navMeshAgent.destination,transform.position)<=navMeshAgent.stoppingDistance)
        {
            if (curRightSpeed < 0.05f && curForwardSpeed < 0.05f)
            {
                anim.SetFloat(right,0f);
                anim.SetFloat(forward,0f);
                return TaskStatus.Success;
            }
        }
        return TaskStatus.Running;
    }
    
    Vector2 VectorFormWorldToLocal(Vector3 worldDirection, Transform localCoord)
    {
        float x = Vector3.Dot(worldDirection, localCoord.right);
        
        float y = Vector3.Dot(worldDirection, localCoord.forward);
        return new Vector2(x, y);
    }
}
