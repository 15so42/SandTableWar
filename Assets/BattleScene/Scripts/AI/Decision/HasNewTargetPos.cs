using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class HasNewTargetPos : Conditional
{
    public SharedVector3 lastDestinationPos;
    public SharedVector3 destinationPos;
    
    public override TaskStatus OnUpdate()
    {
        if (destinationPos != null && lastDestinationPos.Value != destinationPos.Value)
        {
            lastDestinationPos.Value = destinationPos.Value;
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;

    }
}
