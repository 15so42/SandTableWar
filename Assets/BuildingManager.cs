using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BuildingManager
{
   private FightingManager fightingManager;
   public void Init(FightingManager fightingManager)
   {
      this.fightingManager = fightingManager;
   }

   public GameObject CreatePreviewingBuilding(BattleUnitId battleUnitId)
   {
        GameObject previewBuildingGo=new GameObject();
       
        MeshRenderer previewMeshRenderer = previewBuildingGo.AddComponent<MeshRenderer>();
        previewMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        previewBuildingGo.AddComponent<CollisionDetection>();
        previewBuildingGo.AddComponent<IsInBuildingArea>();
        MeshFilter[] buildingMeshFilters = BattleUnitBaseFactory.Instance.GetBattleUnitLocally(battleUnitId).GetComponent<BaseBattleBuilding>().meshFilters;
        
        CombineInstance[] combineInstances = new CombineInstance[buildingMeshFilters.Length]; //新建一个合并组，长度与 meshfilters一致
        for (int i = 0; i < buildingMeshFilters.Length; i++)                                  //遍历
        {
            combineInstances[i].mesh      = buildingMeshFilters[i].sharedMesh;                   //将共享mesh，赋值
            combineInstances[i].transform = buildingMeshFilters[i].transform.localToWorldMatrix; //本地坐标转矩阵，赋值
        }
        Mesh newMesh = new Mesh();                                  //声明一个新网格对象
        newMesh.CombineMeshes(combineInstances);

        MeshFilter buildingMeshFilter = previewBuildingGo.AddComponent<MeshFilter>();
        buildingMeshFilter.mesh = newMesh;
        
        // for (int i = 0; i < buildingMeshFilters.Length; i++)
        // {
        //     MeshFilter tempMeshFilter = previewBuildingGo
        //     tempMeshFilter.mesh = buildingMeshFilters[i].sharedMesh;
        //    
        // }
        
        //碰撞体
        MeshCollider collision = previewBuildingGo.AddComponent<MeshCollider>();
        collision.convex = true;
        collision.isTrigger = true;
        collision.sharedMesh = buildingMeshFilter.sharedMesh;

        //加上刚体才能碰撞
        previewBuildingGo.AddComponent<Rigidbody>().isKinematic = true;
        //材质
        Material previewBuildingMat = Resources.Load<Material>("Material/PreviewBuilding");
        previewMeshRenderer.material = previewBuildingMat;
        PreviewBuilding previewBuilding= previewBuildingGo.AddComponent<PreviewBuilding>();
        //设置实际的建筑以便生成实际建筑
        SpawnBattleUnitConfigInfo buildingInfo =
            ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(battleUnitId);
        previewBuilding.buildingInfo = buildingInfo;

        return previewBuildingGo;
   }
}
