using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class UpdateIdleStatus : Action
{
    public SharedBattleUnit selfUnit;
    public bool targetStatus;
    public override void OnStart()
    {
        base.OnStart();
        selfUnit.Value.UpdateIdleStatus(targetStatus);
    }
}
