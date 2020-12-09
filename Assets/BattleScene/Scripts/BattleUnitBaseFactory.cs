using Photon.Pun;
using UnityEngine;

public class BattleUnitBaseFactory : Singleton<BattleUnitBaseFactory>
{
    public const string SoliderPath = "BattleUnit/Solider/";
    public const string BuildingPath = "BattleUnit/Building/";
    //建筑和士兵都是战斗单位，id表中士兵从1往后数，建筑从299往前数
    public void SpawnBattleUnitAtPos(SpawnBattleUnitConfigInfo soliderInfo,Vector3 pos,int campId)
    {
        string path = (soliderInfo.id < 151) ? SoliderPath : BuildingPath;
        GameObject spawnedBase=PhotonNetwork.Instantiate($"{path}{soliderInfo.resourceName}", pos, Quaternion.identity);
        spawnedBase.GetComponent<BattleUnitBase>().SetCampId(campId);
    }

    public void SpawnBuildingAtPos()
    {
        
    }
    
}