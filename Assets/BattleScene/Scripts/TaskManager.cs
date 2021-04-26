using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using BattleScene.Scripts;
using RTSEngine;
using UnityEngine;


public class TaskManager : MonoBehaviour
{
    [System.Serializable]
    public struct UnitCreationTaskAttributes
    {
        //one prefab of this list will be randomly chosen to be created. Simply have one element here if you wish to use one prefab.
        public List<BattleUnitId> units;
    }
    public static TaskManager Instance;

    public FightingManager fightingManager;
    public TaskStatusManager Status { private set; get; }
    private void Awake()
    {
        Instance = this;
        this.fightingManager = FightingManager.Instance;

        Status = new TaskStatusManager();
    }
    
    
    
     public ErrorMessage CanAddTask (BattleUnitBase BattleUnit)
        {
            if (BattleUnit == null) //no source entity specified, return success
                return ErrorMessage.none;

            if (BattleUnit.IsAlive() == false) //if the task holder is dead -> can't launch task
                return ErrorMessage.sourceDead;

            if(BattleUnit.GetEntityType() == BattleUnitType.Building) //if the task holder is a building and it's not built
            {
                if (((BaseBattleBuilding)BattleUnit).isInBuilding)
                    return ErrorMessage.buildingNotBuilt;
            }

            return ErrorMessage.none;
        }

        //checks whether a task can be added or not and if not, provide the reason in the return value
        public ErrorMessage CanAddTaskToLauncher(TaskLauncher taskLauncher, FactionEntityTask task)
        {
            if (Status.IsTaskEnabled(task.GetCode(), taskLauncher.battleUnitBase.factionId, task.IsAvailable) == false) //if the task is disabled
                return ErrorMessage.componentDisabled;

            if (taskLauncher.battleUnitBase.prop.hp < taskLauncher.GetMinHealth()) //if the task holder does not have enough health to launch task
                return ErrorMessage.sourceLowHealth;

            if (taskLauncher.GetMaxTasksAmount() <= taskLauncher.GetTaskQueueCount()) //if the maximum amount of pending tasks has been reached
                return ErrorMessage.sourceMaxCapacityReached;

            if (task.HasRequiredResources() == false)
                return ErrorMessage.lowResources;
            
            if(task.GetTaskType() == TaskTypes.createUnit) //if this is a unit creation task..
            {
                int nextPopulation = task.UnitPopulationSlots + FightingManager.Instance.GetFaction(taskLauncher.battleUnitBase.factionId).GetCurrentPopulation();
                if (nextPopulation > FightingManager.Instance.GetFaction(taskLauncher.battleUnitBase.factionId).GetMaxPopulation()) //check the population slots
                    return ErrorMessage.maxPopulationReached;

                if (taskLauncher.battleUnitBase.GetFactionManager().HasReachedLimit(taskLauncher.battleUnitBase.configId)) //did the unit type to create reach its limit,
                    return ErrorMessage.factionLimitReached;
            }

            return CanAddTask(taskLauncher.battleUnitBase);
        }

        //called to add a task to a task launcher
        public ErrorMessage AddTask(TaskInfo taskInfo, bool playerCommand = false)
        {
            ErrorMessage addTaskMsg = ErrorMessage.none;
            BattleUnitBase sourceUnit = taskInfo.source as BattleUnitBase;

            if ( taskInfo.taskLauncher != null) //adding a task to task launcher
            {
                //if this task is simply cancelling a pending task, then execute it directly and don't proceed:
                if (taskInfo.type == TaskTypes.cancelPendingTask)
                {
                    taskInfo.taskLauncher.CancelInProgressTask(taskInfo.ID);
                    return ErrorMessage.none; //instant success
                }

                //-> dealing with a task that gets added to the task queue
                addTaskMsg = CanAddTaskToLauncher(taskInfo.taskLauncher, taskInfo.taskLauncher.GetTask(taskInfo.ID)); //check if the task can be added
                if (addTaskMsg != ErrorMessage.none) //if it's not successful
                {
                    if (playerCommand == true) //if this is a player command
                    {
                        //gameMgr.AudioMgr.PlaySFX(taskInfo.taskLauncher.GetLaunchDeclinedAudio(), false);
                        ErrorMessageHandler.OnErrorMessage(addTaskMsg, taskInfo.source); //display error
                    }
                    return addTaskMsg; //then return failure reason and stop
                }
                taskInfo.taskLauncher.Add(taskInfo.ID);

                return ErrorMessage.none;
            }
            else if ((addTaskMsg = CanAddTask(sourceUnit)) != ErrorMessage.none) //not a task launcher but we still check whether the source is valid enough to launch task
            {
                if (playerCommand == true) //if this is a player command
                        ErrorMessageHandler.OnErrorMessage(addTaskMsg, taskInfo.source); //display error

                return addTaskMsg; //then return failure reason and stop
            }


            switch (taskInfo.type)
            {
                // case TaskTypes.deselectIndiv: //deselecting individual units
                //
                //     if (gameMgr.SelectionMgr.MultipleSelectionKeyDown == true) //if the player is holding the multiple selection key then...
                //         gameMgr.SelectionMgr.Selected.Remove(attributes.source); //deselect the clicked entity
                //     else
                //         gameMgr.SelectionMgr.Selected.Add(attributes.source, SelectionTypes.single); //not holding the multiple key then select this unit only
                //     break;
                //
                // case TaskTypes.deselectMul: //deselecting multiple units
                //
                //     if (gameMgr.SelectionMgr.MultipleSelectionKeyDown == true) //if the player is holding the multiple selection key then...
                //         gameMgr.SelectionMgr.Selected.Remove(attributes.sourceList); //deselect the clicked entity
                //     else
                //         gameMgr.SelectionMgr.Selected.Add(attributes.sourceList.ToArray()); //not holding the multiple key then select this unit only
                //     break;
                // case TaskTypes.APCEject: //APC release task.
                //
                //     sourceFactionEntity.APCComp.Eject(sourceFactionEntity.APCComp.GetStoredUnit(attributes.ID), false);
                //     break;
                // case TaskTypes.APCEjectAll: //APC release task.
                //
                //     sourceFactionEntity.APCComp.EjectAll(false);
                //     break;
                // case TaskTypes.APCCall: //apc calling units
                //
                //     sourceFactionEntity.APCComp.CallUnits();
                //     break;
                // case TaskTypes.placeBuilding:
                //
                //     gameMgr.PlacementMgr.StartPlacingBuilding(taskInfo.ID);
                //
                //     break;

                // case TaskTypes.toggleWander:
                //
                //     ((Unit)sourceFactionEntity).WanderComp.Toggle(); //toggle the wandering behavior
                //     break;
                case TaskTypes.cancelPendingTask:

                    taskInfo.taskLauncher.CancelInProgressTask(taskInfo.ID);
                    break;
                // default:
                //     if (attributes.unitComponentTask == true)
                //         UnitComponent.SetAwaitingTaskType(attributes.type, attributes.icon);
                //     break;
            }

            return ErrorMessage.none;
        }

    public enum TaskTypes
    {
        none,
        movement,
        placeBuilding,
        build,
        generateResource,
        collectResource,
        convert,
        heal,
        APCEject,
        APCEjectAll,
        APCCall,
        createUnit,
        destroyBuilding,
        cancelPendingTask,
        attackTypeSelection,
        attack,
        toggleWander,
        upgrade,
        destroy,
        deselectIndiv,
        deselectMul,
        customTask,
        lockAttack,
    };
}