using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralUnit : BattleUnitBase
{
   public bool HasMineMachine { get; set; }
   public bool HasWorkerWorking { get; set; }
   protected override void Awake()
   {
      base.Awake();
      SetCampInPhoton(-1);//中立
      
   }
   

}
