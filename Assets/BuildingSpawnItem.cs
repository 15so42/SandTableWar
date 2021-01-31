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
        buildingIcon.sprite = BuildingIconLoader.GetSpriteByUnitId(buildingInfo.id);
        fightingManager=GameManager.Instance.GetFightingManager();
    }
    public void UpdateSpawnBuildingItem()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        fightingManager.isBuildingPreview = true;
        GameObject previewBuildingGo=new GameObject();
        MeshFilter previewMeshFilter=previewBuildingGo.AddComponent<MeshFilter>();
        MeshRenderer previewMeshRenderer = previewBuildingGo.AddComponent<MeshRenderer>();
        previewMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        previewBuildingGo.AddComponent<CollisionDetection>();
        MeshFilter building = BattleUnitBaseFactory.Instance.GetBattleUnitLocally(buildingInfo).GetComponent<MeshFilter>();
        previewMeshFilter.mesh = building.sharedMesh;
        //碰撞体
        MeshCollider collision = previewBuildingGo.AddComponent<MeshCollider>();
        collision.convex = true;
        collision.isTrigger = true;
        collision.sharedMesh = building.sharedMesh;
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
