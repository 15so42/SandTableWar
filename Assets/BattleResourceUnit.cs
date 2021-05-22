using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResourceUnit : BattleUnitBase
{
    public ResourceInfo resourceInfo;
    protected override void Awake()
    {
        base.Awake();
        resourceInfo = GetComponent<ResourceInfo>();
    }

    protected override void InitFactionEntityType()
    {
        battleUnitType = BattleUnitType.Resource;
    }

    public override void Die()
    {
        base.Die();
        EventCenter.Broadcast(EnumEventType.ResourceEmpty);
    }
}
