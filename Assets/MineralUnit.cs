using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralUnit : BattleUnitBase
{
   protected override void Awake()
   {
      base.Awake();
      SetCampInPhoton(-1);//中立
      
   }

   protected override void Start()
   {
      base.Start();
      stateController = null;
   }

   protected override void OnRightMouseUp()
   {
      base.OnRightMouseUp();
      
   }
}
