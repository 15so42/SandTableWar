

    using System;
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.Events;
    using Random = UnityEngine.Random;

    [Serializable]
    public class FactionEntityTask
    {
        public int ID { private set; get; }

        [SerializeField]
        private string code = "new_task_code"; //a unique code must be assigned to each task
        public string GetCode() { return code; }

        public BattleUnitId battleUnitId;

        private BattleUnitBase battleUnitBase; //the faction ID that this task belongs to.
        private FightingManager fightingManager;

        private bool _isAvailable = true;
        public bool IsAvailable
        {
            get { return _isAvailable; }
            set { _isAvailable = value; }
        }
        
        //生成单位配置
        [SerializeField]
        private TaskManager.UnitCreationTaskAttributes unitCreationAttributes = new TaskManager.UnitCreationTaskAttributes(); //will be shown only in case the task type is a unit creation.
        //if this is a unit creation task, the unit's code and category are copied into the following properties
        public string UnitCode { private set; get; }
        public string UnitCategory { private set; get; }
        public int UnitPopulationSlots { private set; get; }
        
        [System.Serializable]
        public struct TaskLockStatus
        {
            [Tooltip("Code of the task to unlock or lock.")]
            public string code;
            [Tooltip("Enable to unlock the task, disable to lock it.")]
            public bool unlock;
            [Tooltip("Enable to lock/unlock the task for all task launchers of the faction in game or disable to update only tasks in this task launcher instance.")]
            public bool localOnly;
        }
        [SerializeField, Tooltip("Tasks to lock/unlock once this task is completed.")]
        private TaskLockStatus[] updateTasksOnComplete = new TaskLockStatus[0];
        
        [SerializeField]
        private FactionTypeInfo[] factionTypes = new FactionTypeInfo[0]; //when assigned, this faction entity task will only be available for faction types inside the array

        [SerializeField]
        private string description = "describe your task here"; //description shown in the task panel when hovering over the task button.
        public string GetDescription() { return description; }

        [SerializeField]
        public TaskManager.TaskTypes type = TaskManager.TaskTypes.createUnit; //the actual task type which the task manager will be referring to.
        public TaskManager.TaskTypes GetTaskType() { return type; }

#if UNITY_EDITOR
        public enum AllowedTaskTypes { createUnit, destroy, custom, upgrade, lockAttack };

        [SerializeField]
        private AllowedTaskTypes allowedType = AllowedTaskTypes.createUnit; //so that only allowed task types are entered in the inspector
#endif
        
        [SerializeField]
        private float reloadTime = 3.0f; //how long does the task last?
        public float GetReloadTime() { return reloadTime; }

        [SerializeField]
        private BattleResMgr.ResourceInput[] requiredResources = new BattleResMgr.ResourceInput[0]; //Resources required to complete this task.
        public bool HasRequiredResources() { return FightingManager.Instance.GetFaction(battleUnitBase.factionId).BattleResMgr.HasRequiredResources(requiredResources); }
        public BattleResMgr.ResourceInput[] GetRequiredResources() { return requiredResources.Clone() as BattleResMgr.ResourceInput[]; }
        
        [SerializeField]
        private BattleResMgr.ResourceInput[] completeResources = new BattleResMgr.ResourceInput[0];
        public BattleResMgr.ResourceInput[] GetCompleteResources() { return completeResources.Clone() as BattleResMgr.ResourceInput[]; }
        
        public enum UseMode { multiple, onceThisInstance, onceAllInstances } //use mode regarding this task
        //multiple -> task can be launched multiple times
        //onceThisInstance -> task can be only launched and completed once on the task launcher attached that controls this task
        //onceAllInstances -> task can be only launched and completed once on all instances that have the same task type
        [SerializeField]
        private UseMode useMode = UseMode.multiple;

        [SerializeField]
        private UnityEvent launchEvent = null;
        [SerializeField]
        private UnityEvent startEvent = null;
        [SerializeField]
        private UnityEvent completeEvent = null;
        [SerializeField]
        private UnityEvent cancelEvent = null;

        
        public bool Init(FightingManager fightingManager, BattleUnitBase battleUnitBase, int ID)
        {
            this.fightingManager = fightingManager;
            this.ID = ID;

            this.battleUnitBase = battleUnitBase; //assign the faction entity

            bool factionTypeMatch = false;

            foreach (FactionTypeInfo factionType in factionTypes)
            {
                Debug.Log(fightingManager);
                int id = battleUnitBase.factionId;
                FactionManager factionManager = fightingManager.GetFaction(id);
                FactionSlot factionSlot = fightingManager.GetFaction(battleUnitBase.factionId).FactionSlot;
                
                //go through the assigned faction types (if any are assigned)
                if (factionType == fightingManager.GetFaction(battleUnitBase.factionId).FactionSlot.GetTFactionTypeInfo()) //if the faction code matches
                {
                    factionTypeMatch = true; //found the faction type here.
                    break;
                }
            } 

            if (factionTypes.Length > 0 && factionTypeMatch == false) //if there are faction types assigned and the faction type inited does not match
                return false; //false -> asks the task launcher to remove the task from the list so it can't be used

            //continue since this task can be used with the given faction type, but now check if it's available or not
            IsAvailable = TaskManager.Instance.Status.IsTaskEnabled(code, battleUnitBase.factionId, IsAvailable);

            reloadTime /= fightingManager.GetSpeedModifier(); //apply the speed modifier on the reload time

            // if(type == TaskManager.TaskTypes.createUnit)
            // {
            //     //set the population slots, unit code and category properties
            //     UnitPopulationSlots = unitCreationAttributes.prefabs[0].GetPopulationSlots();
            //     UnitCode = unitCreationAttributes.prefabs[0].GetCode();
            //     UnitCategory = unitCreationAttributes.prefabs[0].GetCode();
            // }

            return true;
        }
        
        public bool Launch()
        {
            // if (factionEntity.FactionID == GameManager.PlayerFactionID && launchAudio != null) //if this is the local player faction ID and there a launch audio
            //     gameMgr.AudioMgr.PlaySFX(launchAudio.Fetch(), false); //Play the audio clip
            
            // if (type == TaskManager.TaskTypes.createUnit)
            // {
            //     gameMgr.GetFaction(factionEntity.FactionID).UpdateCurrentPopulation(UnitPopulationSlots); //update population slots
            //     factionEntity.FactionMgr.UpdateLimitsList(UnitCode, UnitCategory, true);
            // }
            
            GetTaskFactionResMgr().UpdateRequiredResources(requiredResources,false);
            
            if (type == TaskManager.TaskTypes.createUnit)
            {
                FactionManager factionManager = FightingManager.Instance.GetFaction(battleUnitBase.factionId);
                factionManager.UpdateCurrentPopulation(UnitPopulationSlots);
                //factionManager.UpdateLimitsList(UnitCode, UnitCategory, true);
            }

            launchEvent.Invoke(); //invoke the task launch methods

            //task can only be used once
             if (useMode != UseMode.multiple)
             {
                 IsAvailable = false; //make it unavailable.
                 if (useMode == UseMode.onceAllInstances) //if this was marked as usable once for all instances.
                     TaskManager.Instance.Status.ToggleTask(code, battleUnitBase.factionId, false);
             }

           
            return false;
        }
        
        public void Start()
        {
            startEvent.Invoke();
        }
        
        public void Complete()
        {
            // if (battleUnitBase.factionId == GameManager.PlayerfactionId && completeAudio != null) //if this is the local player faction ID and there's task completed audio
            //     gameMgr.AudioMgr.PlaySFX(completeAudio.Fetch(), false); //Play the audio clip

            completeEvent.Invoke(); //invoke the complete unity event.

            switch (type) //type of the task that has been completed.
            {
                case TaskManager.TaskTypes.createUnit:
                    //Randomly pick a prefab to produce from the list
                    BattleUnitId unitId  = unitCreationAttributes.units[Random.Range(0, unitCreationAttributes.units.Count)];

                    Vector3 spawnPosition = battleUnitBase.transform.position;
                    BaseBattleBuilding createdBy = null;

                    //get the unit spawn position:
                    if (battleUnitBase.GetEntityType() == BattleUnitType.Building) //if this is a building, then see if it has a dedicated spawn position
                    {
                        createdBy = (BaseBattleBuilding)battleUnitBase;
                        spawnPosition = createdBy.GetSpawnPos();
                    }

                    //gameMgr.UnitMgr.CreateUnit(unitPrefab, spawnPosition, unitPrefab.transform.rotation, spawnPosition, battleUnitBase.factionId, createdBy, false, false); //finally create the unit
                    BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPosById(unitId, spawnPosition,
                        battleUnitBase.factionId);
                    break;
                
            }

            foreach(TaskLockStatus tls in updateTasksOnComplete) //go through the tasks that need to be updated when this one is completed
            {
                if (tls.localOnly) //update task in this task launcher only
                {
                    if (battleUnitBase.taskLauncherComp.GetTask(tls.code) != null) //only if the task exists on the same task launcher.
                        battleUnitBase.taskLauncherComp.GetTask(tls.code).IsAvailable = tls.unlock;
                }
                else //update all tasks of same code in all faction's task launchers
                    TaskManager.Instance.Status.ToggleTask(tls.code, battleUnitBase.factionId, tls.unlock);
            }

            //add the complete resourcs to th task launcher's faction
            GetTaskFactionResMgr().UpdateRequiredResources(completeResources,true);
           
        }

        //cancel an in progress task of this type
        public void Cancel()
        {
            switch (type)
            {
                case TaskManager.TaskTypes.createUnit:

                    //update the population slots
                    //fig.GetFaction(battleUnitBase.factionId).UpdateCurrentPopulation(-UnitPopulationSlots);

                    //update the limits list:
                    //battleUnitBase.FactionMgr.UpdateLimitsList(UnitCode, UnitCategory, false);
                    break;
            }

            //gameMgr.ResourceMgr.UpdateRequiredResources(requiredResources, true, battleUnitBase.factionId); //Give back the task resources.

            //cancelEvent.Invoke(); //trigger unity event.

            //if the task was supposed to be used once but is cancelled:
            // if (useMode != UseMode.multiple)
            // {
            //     IsAvailable = true; //make it available again.
            //     if (useMode == UseMode.onceAllInstances) //if this was marked as usable once for all instances.
            //         gameMgr.TaskMgr.Status.ToggleTask(code, battleUnitBase.factionId, true);
            // }

            // if (battleUnitBase.factionId == GameManager.PlayerfactionId && factionEntity.GetSelection().IsSelected) //if this is the local player and the faction entity is selected
            //     gameMgr.AudioMgr.PlaySFX(cancelAudio.Fetch(), false);
        }

        private BattleResMgr GetTaskFactionResMgr()
        {
            return FightingManager.Instance.GetFaction(battleUnitBase.factionId).BattleResMgr;
        }
    }
