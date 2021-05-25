using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager
{
    private FightingManager fightingManager;

    public void Init(FightingManager fightingManager)
    {
        this.fightingManager = fightingManager;
    }
    public void LaunchAttack(List<BattleUnitBase> attackUnits,BattleUnitBase targetUnit)
    {
        fightingManager.MoveToSpecificPos(attackUnits,targetUnit.transform.position);
    }
}
