using Photon.Pun;
using UnityEngine;

public class BattleUnitBaseFactory : Singleton<BattleUnitBaseFactory>
{
    public const string SoliderPath = "BattleUnit/Solider/";
    public const string BuildingPath = "BattleUnit/Building/";
    //建筑和士兵都是战斗单位，id表中士兵从1往后数，建筑从299往前数
    public BattleUnitBase SpawnBattleUnitAtPos(SpawnBattleUnitConfigInfo soliderInfo,Vector3 pos,int campId)
    {
        string path = (soliderInfo.id < 151) ? SoliderPath : BuildingPath;
        Debug.LogError("准备实例化");
        GameObject spawnedBase=PhotonNetwork.Instantiate($"{path}{soliderInfo.resourceName}", pos, Quaternion.identity);
        Debug.LogError("实例化完成");
        BattleUnitBase spawnedUnit = spawnedBase.GetComponent<BattleUnitBase>();
        Debug.LogError("获取堡垒");
        spawnedUnit.configId = soliderInfo.id;
        spawnedUnit.SetCampInPhoton(campId);
        return spawnedUnit;
    }

    public void SpawnBuildingAtPos()
    {
        
    }
    
}