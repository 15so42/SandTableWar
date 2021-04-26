using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts.AI;
using UnityEngine;

[CreateAssetMenu(menuName="Config/NpcUnitRegulatorDataConfig")]
public class NpcUnitRegulatorDataConfig : ScriptableObject
{
   public List<NpcUnitRegulatorDataFilter> pairs=new List<NpcUnitRegulatorDataFilter>();
   public NpcUnitRegulatorDataFilter commonPair;

   public NpcUnitRegulatorData Filter(BattleUnitId battleUnitId,NpcDifficulty difficulty)
   {
       NpcUnitRegulatorDataFilter filterPair = pairs.Find(x => x.battleUnitId == battleUnitId);
       if (filterPair == null)
       {
           filterPair= commonPair;
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
public class NpcUnitRegulatorDataFilter
{
    public BattleUnitId battleUnitId;
    public NpcUnitRegulatorData easyData;
    public NpcUnitRegulatorData normalData;
    public NpcUnitRegulatorData hardData;
}
