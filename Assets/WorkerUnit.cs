using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerUnit : BattleUnitBase
{
    public int mineOutputRate = 1;
    public BattleUnitBase mineTarget;
   protected void Awake()
   {
       base.Awake();
       weapon = null;
       
   }

   public void SetMineTarget(BattleUnitBase mineTarget)
   {
       this.mineTarget = mineTarget;
   }

  
}
