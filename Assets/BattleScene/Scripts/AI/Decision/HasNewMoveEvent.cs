using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class HasNewMoveEvent : Conditional
{
    public SharedVector3 lastDestinationPos;
    public SharedVector3 destinationPos;

    public override void OnAwake()
    {
        Owner.RegisterEvent<Vector3>("SetDestinationPos", SetNewDestination);
    }

    
    public override TaskStatus OnUpdate()
    {
        if (destinationPos != null && lastDestinationPos.Value != destinationPos.Value)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;

    }
    

    public void SetNewDestination(Vector3 pos)
    {
        destinationPos.SetValue(pos);
    }
}
