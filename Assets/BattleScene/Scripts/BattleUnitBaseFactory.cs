using Photon.Pun;
using UnityEngine;

public class BattleUnitBaseFactory : Singleton<BattleUnitBaseFactory>
{
    public const string SoliderPath = "BattleUnit/Solider/";
    public const string BuildingPath = "BattleUnit/Building/";
    
    public BattleUnitBase SpawnBattleUnitAtPos(SpawnBattleUnitConfigInfo soliderInfo,Vector3 pos,int factionId)
    {
        string path = (soliderInfo.battleUnitType == BattleUnitType.Unit) ? SoliderPath : BuildingPath;
        GameObject spawnedBase;
        if (GameManager.Instance.gameMode == GameMode.Campaign)
        {
            spawnedBase = GameObject.Instantiate(Resources.Load<GameObject>($"{path}{soliderInfo.resourceName}"), pos,
                Quaternion.identity);
        }
        else
        {
            spawnedBase=PhotonNetwork.Instantiate($"{path}{soliderInfo.resourceName}", pos, Quaternion.identity);
        }
        
        BattleUnitBase spawnedUnit = spawnedBase.GetComponent<BattleUnitBase>();
       
        spawnedUnit.configId = soliderInfo.battleUnitId;
        spawnedUnit.SetCampInPhoton(factionId);
        spawnedUnit.Init();
        return spawnedUnit;
    }

    public BattleUnitBase SpawnBattleUnitAtPosById(BattleUnitId battleUnitId,Vector3 pos,int factionId)
    {
        SpawnBattleUnitConfigInfo soliderInfo = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(battleUnitId);
        return SpawnBattleUnitAtPos(soliderInfo, pos, factionId);
    }

    public BattleUnitBase GetBattleUnitLocally(SpawnBattleUnitConfigInfo soliderInfo)
    {
        string path = (soliderInfo.battleUnitType == BattleUnitType.Unit) ? SoliderPath : BuildingPath;
        
        return Resources.Load<BattleUnitBase>($"{path}{soliderInfo.resourceName}");
    }
    
    
}