using System.Collections.Generic;

public class TaskStatusManager
    {
        //data structure for tasks that are disabled after a one-time use or enabled after they were unavailable by default for all task launcher types:
        public class TaskStatus
        {
            public string taskCode; //code of the task.
            public int factionID; //faction that the task launcher belongs to.
            public bool enabled; //is the task enabled or disabled?
        }
        private List<TaskStatus> toggledTasks = new List<TaskStatus>(); //holds a list of toggled tasks.

        //enable/disable a task:
        public void ToggleTask (string taskCode, int factionID, bool enable)
        {

            //go through the toggled tasks first.
            for (int i = 0; i < toggledTasks.Count; i++)
            {
                //if the task code and the faction ID match.
                if (toggledTasks[i].taskCode == taskCode && toggledTasks[i].factionID == factionID)
                {
                    //then the task is already registerd:
                    toggledTasks[i].enabled = enable;
                    return; //do not proceed as the task is found.
                }
            }

            //if the task wasn't found already then simply add it.
            toggledTasks.Add(new TaskStatus { taskCode = taskCode, factionID = factionID, enabled = enable });
        }

        //see if a task is enabled/disabled:
        public bool IsTaskEnabled (string taskCode, int factionID, bool defaultStatus)
        {
            //see if the task is already registerd in the toggled tasks list
            //go through the toggled tasks
            for (int i = 0; i < toggledTasks.Count; i++)
            {
                //if the task code and the faction ID match.
                if (toggledTasks[i].taskCode == taskCode && toggledTasks[i].factionID == factionID)
                {
                    //then the task is already registerd:
                    return toggledTasks[i].enabled; //return the status of the task.
                }
            }

            //if the task isn't registerd in the toggled tasks list then return its default status
            return defaultStatus;
        }
    }