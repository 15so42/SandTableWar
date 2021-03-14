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
      
       MineralUnit mineralUnit=mineTarget as MineralUnit;
       if (mineralUnit.HasWorkerWorking == false)
       {
           mineralUnit.HasWorkerWorking = true;
           this.mineTarget = mineTarget;
       }
       else
       {
           this.mineTarget=FindOtherClosetMineral(mineTarget.transform.position);
           (this.mineTarget as MineralUnit).HasWorkerWorking = true;
       }

       if (this.mineTarget != null)
       {
           SetTargetPos(this.mineTarget.transform.position);
       }
       
       
   }

   private BattleUnitBase FindOtherClosetMineral(Vector3 pos)
   {
       Collider[] colliders = Physics.OverlapSphere(pos, 10);
       for (int i = 0; i < colliders.Length; i++)
       {
           BattleUnitBase battleUnitBase = colliders[i].GetComponent<BattleUnitBase>();
           if (battleUnitBase && battleUnitBase.configId == BattleUnitId.Mineral && battleUnitBase.IsInFog()==false &&
               (battleUnitBase as MineralUnit).HasWorkerWorking==false && (battleUnitBase as MineralUnit).HasMineMachine == false)
           {
               return battleUnitBase;
           }
       }

       return null;
   }

   public override void SetChaseTarget(BattleUnitBase battleUnitBase)
   {
       if (battleUnitBase.configId == BattleUnitId.Mineral)
       {
           SetMineTarget(battleUnitBase);
       }
   }

  
}
