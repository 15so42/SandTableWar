
    using System.Collections.Generic;
    using UnityEngine;

    public class TaskLauncher : MonoBehaviour
    {
        public BattleUnitBase battleUnitBase;
        private FightingManager fightingManager;
        private bool isActive = true; //is the task launcher component active?
        public bool IsActive () { return isActive; }
        
        public bool Initiated { private set; get; }
        
        private string code = "new_task_launcher"; //each task launcher must have a unique code
        public string GetCode() { return code; }
        
        [SerializeField]
        private int minHealth = 70; //minimum health required in order to launch/complete a task. 
        public int GetMinHealth () { return minHealth; }
        
        [SerializeField]
        private int maxTasks = 4; //The maximum amount of tasks that this component can handle at the same time.
        public int GetMaxTasksAmount() { return maxTasks; }
        
        [SerializeField, Tooltip("Tasks that this task launcher can start.")]
        private List<FactionEntityTask> tasksList = new List<FactionEntityTask>();
        //a dictionary of the task launcher's task where each task's code is the key and the actual task instance is the value.
        private Dictionary<string, FactionEntityTask> tasksDic = new Dictionary<string, FactionEntityTask>();
        
        private List<int> tasksQueue = new List<int>(); //this is the task's queue which holds all the pending tasks indexes
        private float taskQueueTimer = 0.0f;
        public int GetTaskQueueCount () { return tasksQueue.Count; }
        public int GetTasksCount () { return tasksDic.Count; }
        
        public IEnumerable<FactionEntityTask> GetAll () { return tasksDic.Values; }

        private float needTime;//当前任务所需时间

        public TaskLauncherUI taskLauncherUi;
        
        public void Init(FightingManager fightingManager, BattleUnitBase battleUnitBase)
        {
            ///assign the components
            this.fightingManager = fightingManager;
            this.battleUnitBase = battleUnitBase;

            for (int i = 0; i < tasksList.Count; i++) //init the tasks
            {
                if (tasksList[i].Init(fightingManager, battleUnitBase, i)) //if the task successfully initializes then it can be used by the task launcher:
                    tasksDic.Add(tasksList[i].GetCode(), tasksList[i]);
            }

            EventCenter.Broadcast(EnumEventType.OnTaskLauncherAdded,this); 

            Initiated = true;
            taskLauncherUi=UIManager.Instance.SetTaskLauncherUI(gameObject,GetComponent<BattleUnitBase>().hpUiOffset);
        }
        
        public FactionEntityTask GetTask (int index) {
            if (index < 0 || index >= tasksList.Count)
                return null;
            return tasksList[index]; }
        
        public FactionEntityTask GetTask (string code)
        {
            if (tasksDic.TryGetValue(code, out FactionEntityTask task))
                return task;

            return null;
        }
        
        public bool CanManageTask ()
        {
            if (battleUnitBase.battleUnitType == BattleUnitType.Building) //if the task holder is a building
                if (((BaseBattleBuilding)battleUnitBase).isInBuilding ) //and the building hasn't been constructed yet 
                    return false; //can't manage tasks.
            
            //for both units and buildings, check if they're not dead and that they have enough health to proceed.
            return battleUnitBase.IsAlive() && battleUnitBase.prop.hp >= minHealth;
        }
        
        
        public void Add (int taskID)
        {
            if (taskID < 0 || taskID >= tasksList.Count) //invalid task ID? 
                return;

            tasksQueue.Add(taskID); //add the new task to the queue

            //if the task queue of the task launcher was empty
            if (tasksQueue.Count == 1)
                StartNextTask(); //start the task instantly

            tasksList[taskID].Launch(); //launch actual task
            
            EventCenter.Broadcast(EnumEventType.OnTaskLaunched,this,taskID,tasksQueue.Count - 1);
        }
        
        public void StartNextTask ()
        {
            if (tasksQueue.Count == 0) //if the tasks queue is empty
                return;

            taskQueueTimer = tasksList[tasksQueue[0]].GetReloadTime(); //start the timer for the next one.
            needTime = taskQueueTimer;
            tasksList[tasksQueue[0]].Start();

            //CustomEvents.OnTaskStarted(this, tasksQueue[0], 0); //trigger custom event
        }

        public float GetProgress()
        {
            //Debug.Log( taskQueueTimer +"," +needTime);
            return taskQueueTimer / needTime;
            
        }

        
        private void Update()
        {
            //as long as this component is active, the faction entity can manage tasks and there are actual pending tasks in the queue
            if (isActive && tasksQueue.Count > 0 && CanManageTask())
                UpdatePendingTask();
        }
        
        void UpdatePendingTask()
        {
            //if the task timer is still going and we are not using the god mode
            if (taskQueueTimer > 0)
            {
                taskQueueTimer -= Time.deltaTime;
            }
            else //task timer is done
            {
                needTime = 0;
                OnTaskCompleted(); //complete the task
            }
        }
        
        public void OnTaskCompleted ()
        {
            if (tasksQueue.Count == 0) //if there are no tasks in the queue then do not proceed
                return;

            int completedTaskID = tasksQueue[0]; //get the first pending task and remove it from the queue
            tasksQueue.RemoveAt(0); //remove from task queue

            tasksList[completedTaskID].Complete(); //complete the task

            //CustomEvents.OnTaskCompleted(this, completedTaskID, 0); //custom delegate event
            EventCenter.Broadcast(EnumEventType.OnTaskCompleted,this,completedTaskID);
            StartNextTask(); //start the next task in queue
        }
        
        //a method that cancels an in progress task
        public void CancelInProgressTask (int queueIndex)
        {
            if (queueIndex < 0 || queueIndex >= tasksQueue.Count) //invalid task index
                return;

            int taskID = tasksQueue[queueIndex]; //get the actual task ID
            tasksQueue.RemoveAt(queueIndex); //remove from queue

            tasksList[taskID].Cancel(); //cancel the task

            //CustomEvents.OnTaskCanceled(this, taskID, queueIndex);
            EventCenter.Broadcast(EnumEventType.OnTaskCanceled,this,taskID,queueIndex);
            
            if (queueIndex == 0) //if the first task in the queue was the one that got cancelled and we still have more in queue
                StartNextTask();
        }

    }
