using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Config/SpawnSoliderConfig")]
public class SpawnBattleUnitConfig : ScriptableObject
{
   public List<SpawnBattleUnitConfigInfo> config;
}

[System.Serializable]
public class SpawnBattleUnitConfigInfo
{
   public BattleUnitType battleUnitType;
   public BattleUnitId battleUnitId;
   public string resourceName;
   public float spawnTime;
   public int needPopulation=1;
   public int needCoin = 1;
   public int needMineral = 1;
   public int needFood = 1;
}

public enum BattleUnitType
{
   Solider,
   Building
}

public enum BattleUnitId
{
   Ranger,//突击兵
   Tank_A,
   Scout,
   MedicalSolider,
   Worker,
   Mineral,
   Base,
   Bunker_M,
   Tank_Tiger,//虎式坦克
}