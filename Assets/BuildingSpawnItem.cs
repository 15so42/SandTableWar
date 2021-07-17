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
    [SerializeField]private Text buildingText; 
    
    private FightingManager fightingManager;

    private GlobalItemRangeDisplayer globalItemRangeDisplayer;
    public void Init(SpawnBattleUnitConfigInfo buildingInfo)
    {
        this.buildingInfo = buildingInfo;
        buildingIcon.sprite = BuildingIconLoader.GetSpriteByUnitId(buildingInfo.battleUnitId);
        buildingText.text = buildingInfo.battleUnitName;
        fightingManager=GameManager.Instance.GetFightingManager(); 
        globalItemRangeDisplayer=GlobalItemRangeDisplayer.Instance;
    }
    public void UpdateSpawnBuildingItem()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        fightingManager.isBuildingPreview = true;
        

        PreviewBuilding previewBuilding =
            fightingManager.buildingManager.CreatePreviewingBuilding(buildingInfo.battleUnitId).GetComponent<PreviewBuilding>();
        previewBuilding.Init(true);
        fightingManager.previewBuilding = previewBuilding;
        
        
        globalItemRangeDisplayer.SetColor(DisplayRangeType.BuildingArea,Color.red);
        globalItemRangeDisplayer.EnableDisplayRangeType(DisplayRangeType.BuildingArea,true);
        List<BattleUnitBase> buildRange = globalItemRangeDisplayer.GetBuildRange();
        globalItemRangeDisplayer.SetList(DisplayRangeType.BuildingArea,buildRange);
    }
    
    

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        globalItemRangeDisplayer.EnableDisplayRangeType(DisplayRangeType.BuildingArea,false);
    }
}
