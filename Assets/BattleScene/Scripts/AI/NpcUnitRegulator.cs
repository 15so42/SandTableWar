using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleScene.Scripts.AI
{
    public class NpcUnitRegulator : NpcRegulator<BattleUnitBase>
    {
        public NpcUnitRegulatorData Data { private set; get; }
        public float ratio = 0;
        
        
        
        private Dictionary<TaskLauncher, List<int>> unitCreators = new Dictionary<TaskLauncher, List<int>>();//获取所有的TaskLauncher，并得到每个TaskLauncher
        //中包含指定id的Task
        public int GetTaskLauncherCount () { return unitCreators.Count; }
        public IEnumerable<TaskLauncher> GetTaskLaunchers () { return unitCreators.Keys; }
        
        public IEnumerable<int> GetUnitCreationTasks (TaskLauncher taskLauncher)
        {
            if (unitCreators.TryGetValue(taskLauncher, out List<int> taskIDList))
                return taskIDList;

            return null;
        }

        private NpcUnitCreator npcUnitCreator; 
        
        public NpcUnitRegulator(NpcUnitRegulatorData data, BattleUnitId battleUnitId, FightingManager fightingManager, NpcCommander npcCommander,NpcUnitCreator npcUnitCreator) : base(data, battleUnitId, fightingManager, npcCommander)
        {
            this.Data = data;
            ratio = Data.GetRatio();
            UpdateTargetCount();
            this.npcUnitCreator = npcUnitCreator;
            
            foreach (var unit in factionManager.myUnits)
            {
                AddExisting(unit);   
            }
            
            foreach (TaskLauncher tl in this.factionManager.GetTaskLaunchers())
                OnTaskLauncherAdded(tl);
            
            EventCenter.AddListener<BattleUnitBase>(EnumEventType.UnitCreated,Add);
            EventCenter.AddListener<BattleUnitBase>(EnumEventType.UnitDied,Remove);
            EventCenter.AddListener<TaskLauncher, int , int>(EnumEventType.OnTaskLaunched,OnTaskLaunched);
            EventCenter.AddListener<TaskLauncher, int , int>(EnumEventType.OnTaskCanceled,OnTaskCanceled);
            EventCenter.AddListener<TaskLauncher>(EnumEventType.OnTaskLauncherAdded,OnTaskLauncherAdded);
            EventCenter.AddListener<TaskLauncher>(EnumEventType.OnTaskLauncherRemoved,OnTaskLauncherRemoved);
        }

        
        public void UpdateTargetCount()
        {
            //calculate new target amount for the regulated unit type instances and limit by max and min allowed amount
            int maxPopulation = factionManager.GetMaxPopulation();
            TargetCount = Mathf.Clamp( 
                (int)(maxPopulation * ratio),
                MinAmount,
                MaxAmount);
            
        }
        
        public List<BattleUnitBase> GetIdleUnitsFirst ()
        {
            var result = instances.OrderByDescending(unit => unit.IsIdle()).ToList();
            return result;
        }

        private void AddExisting(BattleUnitBase BattleUnit)
        {
            if (!CanBeRegulated(BattleUnit)) //only proceed if the faction entity can be regulated by this component
                return;

            //add it to list:
            instances.Add(BattleUnit);
            Count++;
        }


        #region Task Launcher Event Callbacks

        /// <summary>
        /// Called whenever a FactionEntityTask instance is launched from a TaskLauncher instance.
        /// </summary>
        /// <param name="taskLauncher">TaskLauncher instance whose task is launched.</param>
        /// <param name="taskID">ID of the launched task.</param>
        /// <param name="taskQueueID">ID of the launched task in the waiting queue.</param>
        private void OnTaskLaunched(TaskLauncher taskLauncher, int taskID, int taskQueueID)
        {
            //if the launched task is being tracked by this regulator.
            if (unitCreators.TryGetValue(taskLauncher, out List<int> taskIDList) && taskIDList.Contains(taskID))
                AddPending();
        }

        /// <summary>
        /// Called whenever a FactionEntityTask instance is canceled while in progress in a TaskLauncher instance.
        /// </summary>
        /// <param name="taskLauncher">TaskLauncher instance whose task is cancelled.</param>
        /// <param name="taskID">ID of the cancelled task.</param>
        /// <param name="taskQueueID">None.</param>
        private void OnTaskCanceled(TaskLauncher taskLauncher, int taskID, int taskQueueID)
        {
            //if the canceled task is being tracked by this regulator.
            if (unitCreators.TryGetValue(taskLauncher, out List<int> taskIDList) && taskIDList.Contains(taskID))
                Remove();
        }

        #endregion
        
        
        public void OnTaskLauncherAdded (TaskLauncher taskLauncher)
        {
            if (unitCreators.ContainsKey(taskLauncher) //if the task launcher is already registered
                || taskLauncher.battleUnitBase.factionId != factionManager.FactionId) //or the task launcher doesn't belong to the same NPC faction.
                return;

            for(int taskID = 0; taskID < taskLauncher.GetTasksCount(); taskID++)
            {
                FactionEntityTask nextTask = taskLauncher.GetTask(taskID);
                //if the task's type is unit creation and it the unit to create code matches the code of unit type that is regulated by this component
                if (nextTask.GetTaskType() == TaskManager.TaskTypes.createUnit && nextTask.battleUnitId == battleUnitId)
                {
                    if (unitCreators.TryGetValue(taskLauncher, out List<int> taskIDList)) //task launcher already is registered as key, just append value list
                        taskIDList.Add(taskID);
                    else //register task launcher as new key and task ID as the only element in the value list.
                        unitCreators.Add(taskLauncher, new List<int>(new int[] { taskID }));
                }
            }
        }


        public void OnTaskLauncherRemoved (TaskLauncher taskLauncher)
        {
            unitCreators.Remove(taskLauncher); //if the task launcher instance was tracked by this regulator then it will be removed.
        }

        public void OnMaxPopulationUpdated(FactionManager factionManager, int value)
        {
            //if this update belongs to the faction managed by this component:
            if (factionManager.FactionId == this.factionManager.FactionId)
                UpdateTargetCount();
        }

        public void Disable()
        {
            EventCenter.RemoveListener<BattleUnitBase>(EnumEventType.UnitCreated,Add);
            EventCenter.RemoveListener<BattleUnitBase>(EnumEventType.UnitDied,Remove);
            EventCenter.RemoveListener<TaskLauncher>(EnumEventType.OnTaskLauncherAdded,OnTaskLauncherAdded);
            EventCenter.RemoveListener<TaskLauncher>(EnumEventType.OnTaskLauncherRemoved,OnTaskLauncherRemoved);
        }
        
       


        protected override void OnSuccessfulRemove(BattleUnitBase factionEntity)
        {
            if(!HasReachedMaxAmount()) //and maximum allowed amount hasn't been reached yet
                npcUnitCreator.Activate(); //activate unit creator to create more instances of this unit type.
        }
    }
}