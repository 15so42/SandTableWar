using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPosStateAction : StateAction
{
    private bool hasSetDes;
    private Vector3 lastSetDesPos;
    public override void Act(StateController controller)
    {
        controller.navMeshAgent.isStopped = false;
        if (hasSetDes == false)//避免多次执行setDest函数
        {
            controller.navMeshAgent.SetDestination(controller.targetPos);
            hasSetDes = true;
            lastSetDesPos = controller.targetPos;
        }
        else
        {
            if (controller.targetPos != lastSetDesPos)
            {
                controller.navMeshAgent.SetDestination(controller.targetPos);
                lastSetDesPos = controller.targetPos;
            }
        }
        
    }
}
