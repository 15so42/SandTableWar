using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class ClearEnemy : Action
{
    public SharedBattleUnit enemyUnit;
    public override void OnStart()
    {
        base.OnStart();
        enemyUnit.Value = null;
    }
}
