using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts.AI;
using UnityEngine;

[CreateAssetMenu(menuName="Config/NpcUnitRegulatorDataConfig")]
public class NpcRegulatorDataConfig : ScriptableObject
{
   public List<NpcRegulatorDataFilter> unitPairs=new List<NpcRegulatorDataFilter>();
   public NpcRegulatorDataFilter unitCommonPair;
   
   public List<NpcRegulatorDataFilter> buildingPairs=new List<NpcRegulatorDataFilter>();
   public NpcRegulatorDataFilter buildingCommonPair;

   public NpcRegulatorData Filter(BattleUnitId battleUnitId,NpcDifficulty difficulty,bool building=false)
   {
       NpcRegulatorDataFilter filterPair = (building? buildingPairs : unitPairs).Find(x => x.battleUnitId == battleUnitId);
       if (filterPair == null)
       {
           var tmpPair = new NpcRegulatorDataFilter();
           var toCopyPair=building ?  buildingCommonPair : unitCommonPair;
           tmpPair.battleUnitId = toCopyPair.battleUnitId;
           tmpPair.easyData = toCopyPair.easyData;
           tmpPair.normalData = toCopyPair.normalData;
           tmpPair.hardData = toCopyPair.hardData;
           filterPair= tmpPair;
       }
       switch (difficulty)
       {
           case NpcDifficulty.Easy:
               return filterPair.easyData;
           case NpcDifficulty.Normal:
               return filterPair.normalData;
           case NpcDifficulty.Hard:
               return filterPair.hardData;
       }

       return filterPair.normalData;
   }
}

[System.Serializable]
public class NpcRegulatorDataFilter
{
    public BattleUnitId battleUnitId;
    public NpcRegulatorData easyData;
    public NpcRegulatorData normalData;
    public NpcRegulatorData hardData;
}
