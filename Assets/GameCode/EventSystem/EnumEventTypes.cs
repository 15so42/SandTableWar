using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumEventType
{
    OnBattleStart,
    OnGamePaused,
    
    UnitCreated,
    UnitDied,
    
    OnFactionUnitDamaged,//用于单位被攻击时呼叫支援
    
    ResourceCreated,
    ResourceEmpty,
    
    OnTaskLauncherAdded,
    OnTaskLauncherRemoved,
    
    OnTaskLaunched,
    OnTaskStarted,
    OnTaskCanceled,
    OnTaskCompleted,
    
    AllFactionsInit,
    
    OnCurrentPopulationUpdated,
    
    OnUnitCollectionOrder,
    OnUnitStopCollecting,
    
    //边界
    OnBorderActivated,
    OnBorderDeActivated,
    
    //建筑
    BuildingStartPlacement,
    BuildingStopPlacement,
    BuildingPlaced,
    BuildingDestroyed,
    BuildingBuilt,
    
}

