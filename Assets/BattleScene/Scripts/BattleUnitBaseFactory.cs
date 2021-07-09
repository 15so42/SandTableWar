using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BattleUnitBaseFactory : Singleton<BattleUnitBaseFactory>
{
    public const string SoliderPath = "BattleUnit/Solider/";
    public const string BuildingPath = "BattleUnit/Building/";
    
    public List<BattleUnitBase> cachedBattleUnits=new List<BattleUnitBase>();
    
    public BattleUnitBase SpawnBattleUnitAtPos(SpawnBattleUnitConfigInfo soliderInfo,Vector3 instaniatePos,Vector3 gatherPos,int factionId)
    {
        string path = (soliderInfo.battleUnitType == BattleUnitType.Unit) ? SoliderPath : BuildingPath;
        GameObject spawnedBase;
        if (GameManager.Instance.gameMode == GameMode.Campaign)
        {
            spawnedBase = GameObject.Instantiate(GetBattleUnitLocally(soliderInfo).gameObject, instaniatePos,
                Quaternion.identity);
        }
        else
        {
            spawnedBase=PhotonNetwork.Instantiate($"{path}{soliderInfo.resourceName}", instaniatePos, Quaternion.identity);
        }
        
        BattleUnitBase spawnedUnit = spawnedBase.GetComponent<BattleUnitBase>();
       
        spawnedUnit.configId = soliderInfo.battleUnitId;
        spawnedUnit.SetCampInPhoton(factionId);
        spawnedUnit.spawnTargetPos = gatherPos;
        spawnedUnit.Init();
        return spawnedUnit;
    }

    public BattleUnitBase SpawnBattleUnitAtPosById(BattleUnitId battleUnitId,Vector3 instaniatePos,Vector3 gatherPos,int factionId)
    {
        SpawnBattleUnitConfigInfo soliderInfo = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(battleUnitId);
        return SpawnBattleUnitAtPos(soliderInfo, instaniatePos, gatherPos, factionId);
    }

    private BattleUnitBase GetBattleUnitLocally(SpawnBattleUnitConfigInfo soliderInfo)
    {
        //先从缓存列表中
        BattleUnitBase battleUnitBase = cachedBattleUnits.Find(x => x.name == soliderInfo.resourceName);
        if (battleUnitBase != null)
        {
            return battleUnitBase;
        }
        
        string path = (soliderInfo.battleUnitType == BattleUnitType.Unit) ? SoliderPath : BuildingPath;
        
        BattleUnitBase newBattleUnitBase= Resources.Load<BattleUnitBase>($"{path}{soliderInfo.resourceName}");
        cachedBattleUnits.Add(newBattleUnitBase);
        return newBattleUnitBase;
    }

    public BattleUnitBase GetBattleUnitLocally(BattleUnitId battleUnitId)
    {
        SpawnBattleUnitConfigInfo soliderInfo = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(battleUnitId);
        return GetBattleUnitLocally(soliderInfo);
    }
    
    
}