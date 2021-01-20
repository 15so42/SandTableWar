using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerAnimCtrl : BattleUnitAnimCtrl
{
    protected override void OnBattleState()
    {
        //do nothing
    }

    protected override void StateCheck()
    {
        base.StateCheck();
        if (stateController.currentState.stateName == "采矿")
        {
            anim.SetBool("Mine",true);
        }
        else
        {
            anim.SetBool("Mine",false);
        }
    }
    
    
}
