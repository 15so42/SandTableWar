using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Buff/生命回复")]
public class RegenerationBuff : BuffBase
{
    private BattleUnitBase battleUnitBase;
    public override void Init<T>(T t)
    {
        battleUnitBase = (t as BattleUnitBase);
    }

    public override void Apply()
    {
       
    }

    protected override void PeriodTick()
    {
        battleUnitBase.CureHp(10);
    }
    
}
