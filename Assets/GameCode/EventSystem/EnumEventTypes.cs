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
    BuildingStartPlacement,//NPC阵营开始寻找放置建筑位置
    BuildingStopPlacement,//NPC阵营取消放置建筑
    BuildingPlaced,//建筑防止完成（开始实例化）
    BuildingDestroyed,//建筑倍摧毁(Die）
    BuildingBuilt,//建筑建造即使完成
    
}

