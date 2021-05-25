using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveManager
{
    private FightingManager fightingManager;

    public void Init(FightingManager fightingManager)
    {
        this.fightingManager = fightingManager;
    }

    public void Move(BattleUnitBase unit,Vector3 targetPos,float radius)
    {
        NavMeshHit hit;
        Vector3 endPos = targetPos + Vector3.one + Random.insideUnitSphere*radius;
        endPos.y = targetPos.y;
        if(NavMesh.SamplePosition(endPos, out hit, 10,-1))
        {
            unit.SetTargetPos(hit.position);
        }
       
    }
}
