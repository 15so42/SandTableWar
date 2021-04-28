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
   [Header("中文名称")]
   public string battleUnitName;
   public bool hasExtraIcon;
   public float spawnTime;
   public int needPopulation=1;
   public int needCoin = 1;
   public int needMineral = 1;
   public int needFood = 1;
}

public enum BattleUnitType
{
   Unit,
   Building,
   Resource
}

public enum BattleUnitId//不要改变顺序，否则会全部爆炸
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
   Farmland,
   AutomaticBattery_I,//I型自动炮台
   MineMachine,
   EngineeringBay,//工程站
   AutomaticBattery_T,//T型自动炮台
   MilitaryDepot,
   None,
   ResourceTree,

}