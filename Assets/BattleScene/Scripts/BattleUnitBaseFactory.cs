using Photon.Pun;
using UnityEngine;

public class BattleUnitBaseFactory : Singleton<BattleUnitBaseFactory>
{
    public const string SoliderPath = "BattleUnit/Solider/";
    public const string BuildingPath = "BattleUnit/Building/";
    //建筑和士兵都是战斗单位，id表中士兵从1往后数，建筑从299往前数
    public BattleUnitBase SpawnBattleUnitAtPos(SpawnBattleUnitConfigInfo soliderInfo,Vector3 pos,int campId)
    {
        string path = (soliderInfo.battleUnitType == BattleUnitType.Solider) ? SoliderPath : BuildingPath;
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
        spawnedUnit.SetCampInPhoton(campId);
        return spawnedUnit;
    }

    public void SpawnBuildingAtPos()
    {
        
    }

    public BattleUnitBase GetBattleUnitLocally(SpawnBattleUnitConfigInfo soliderInfo)
    {
        string path = (soliderInfo.battleUnitType == BattleUnitType.Solider) ? SoliderPath : BuildingPath;
        
        return Resources.Load<BattleUnitBase>($"{path}{soliderInfo.resourceName}");
    }
    
    
}