using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerUnit : BattleUnitBase
{
    
   protected void Awake()
   {
       base.Awake();
       weapon = null;
       stateController=new WorkerStateController(this);
   }
}
