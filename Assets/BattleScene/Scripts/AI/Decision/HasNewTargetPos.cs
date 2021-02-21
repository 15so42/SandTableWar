using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class HasNewTargetPos : Conditional
{
    public SharedVector3 destinationPos=Vector3.zero;
    public Vector3 lastDestinationPos;

    public override void OnStart()
    {
        lastDestinationPos = destinationPos.Value = transform.position;
        base.OnStart();
    }

    public override TaskStatus OnUpdate()
    {
        if (destinationPos != null && lastDestinationPos != destinationPos.Value)
        {
            lastDestinationPos = destinationPos.Value;
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;

    }
}
