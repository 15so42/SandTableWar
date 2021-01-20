using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerUnit : BattleUnitBase
{
    public int mineOutputRate = 1;
   protected void Awake()
   {
       base.Awake();
       weapon = null;
       stateController=new WorkerStateController(this);
   }

   protected override void Start()
   {
       base.Start();
       State mineState = (stateController as WorkerStateController).mineState;
       mineState.OnStateEnterEvent.AddListener(() =>
       {
           fightingManager.battleResMgr.AddIncreaseRate(BattleResType.mineral,mineOutputRate);
       });
       mineState.OnStateExitEvent.AddListener(() =>
       {
           fightingManager.battleResMgr.ReduceIncreaseRate(BattleResType.mineral,mineOutputRate);
       });
   }
}
