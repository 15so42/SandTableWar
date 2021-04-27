using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumEventType
{
    OnBattleStart,
    OnGamePaused,
    
    UnitCreated,
    UnitDied,
    
    ResourceCreated,
    
    OnTaskLauncherAdded,
    OnTaskLauncherRemoved,
    
    OnTaskLaunched,
    OnTaskStarted,
    OnTaskCanceled,
    OnTaskCompleted,
    
    AllFactionsInit,
    
    OnCurrentPopulationUpdated,
}

