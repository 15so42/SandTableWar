using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceInfo))]
public class MineralUnit : BattleUnitBase
{
   public bool HasMineMachine { get; set; }
   public bool HasWorkerWorking { get; set; }

   public MineMachine MineMachine;
   protected override void Awake()
   {
      base.Awake();
      SetCampInPhoton(-1);//中立
      
   }

   protected override void InitFactionEntityType()
   {
      factionType = BattleUnitType.Resource;
   }

   public override void Die()
   {
      MineMachine.Die();
      base.Die();
   }
}
