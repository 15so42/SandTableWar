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
   public int id;
   public string resourceName;
   public float spawnTime;
}