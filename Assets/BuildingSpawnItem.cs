using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingSpawnItem : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public SpawnBattleUnitConfigInfo buildingInfo;
    public Image buildingIcon;

    public void Init(SpawnBattleUnitConfigInfo buildingInfo)
    {
        this.buildingInfo = buildingInfo;
        buildingIcon.sprite = BuildingIconLoader.GetSpriteByUnitId(buildingInfo.id);
    }
    public void UpdateSpawnBuildingItem()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        FightingManager fightingManager = GameManager.Instance.GetFightingManager();
        fightingManager.isUsingItem = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
