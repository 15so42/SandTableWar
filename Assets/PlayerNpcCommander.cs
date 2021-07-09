using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNpcCommander : PlayerNpcComp
{
   public FactionManager factionManager;
   public void Init(FactionManager factionManager)
   {
      this.factionManager = factionManager;
      foreach (var playerNpcComp in transform.GetComponentsInChildren<PlayerNpcComp>())
      {
         playerNpcComp.Init(this);
      }
   }
}
