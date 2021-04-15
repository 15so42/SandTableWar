using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingSetSpawnPosMenuItem : BuildingMenuItem,IPointerDownHandler
{
    private BattleUnitBase ownerBattleUnitBase;
    private BattleBuildingMenuDialog ownerDialog;
    public void SetParams(BattleUnitBase battleUnitBase,BattleBuildingMenuDialog dialog)
    {
        ownerDialog = dialog;
        ownerBattleUnitBase = battleUnitBase;
    }
   
    public void OnPointerDown(PointerEventData eventData)
    {
        // (ownerBattleUnitBase as BaseBattleBuilding).StartSetSpawnPos();
        // // GameManager.Instance.GetFightingManager().isDragFromBuilding = true;
        // // GameManager.Instance.GetFightingManager().buildingWhichIsSetSpawnPos = ownerBattleUnitBase as BaseBattleBuilding;
        // GameManager.Instance.GetFightingManager().EnableSelectUnitByRect(false);
        //
        // ownerDialog.Close();
    }

    public override void Init()
    {
    }

    public override void Update()
    {
    }
}
