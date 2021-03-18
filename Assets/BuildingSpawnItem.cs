using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BuildingSpawnItem : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public SpawnBattleUnitConfigInfo buildingInfo;
    public Image buildingIcon;
    
    private FightingManager fightingManager;
    public void Init(SpawnBattleUnitConfigInfo buildingInfo)
    {
        this.buildingInfo = buildingInfo;
        buildingIcon.sprite = BuildingIconLoader.GetSpriteByUnitId(buildingInfo.battleUnitId);
        fightingManager=GameManager.Instance.GetFightingManager();
    }
    public void UpdateSpawnBuildingItem()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        fightingManager.isBuildingPreview = true;
        GameObject previewBuildingGo=new GameObject();
        
       
        MeshRenderer previewMeshRenderer = previewBuildingGo.AddComponent<MeshRenderer>();
        previewMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        previewBuildingGo.AddComponent<CollisionDetection>();
        MeshFilter[] buildingMeshFilters = BattleUnitBaseFactory.Instance.GetBattleUnitLocally(buildingInfo).GetComponent<BaseBattleBuilding>().meshFilters;
        
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
        previewBuilding.buildingInfo = buildingInfo;
      

        fightingManager.previewBuilding = previewBuilding;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
