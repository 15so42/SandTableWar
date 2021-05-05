using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceInfo))]
public class MineralUnit : BattleResourceUnit
{
   
   public bool HasMineMachine { get; set; }
   
   public MineMachine MineMachine;
  

   public override void Die()
   {
      if (MineMachine)
      {
         MineMachine.Die();
      }
      base.Die();
   }
}
