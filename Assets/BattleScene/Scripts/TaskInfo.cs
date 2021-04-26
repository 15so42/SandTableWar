using System.Collections.Generic;

namespace BattleScene.Scripts
{
    public class TaskInfo
    {
        public int ID;

        public TaskManager.TaskTypes type;

        public TaskLauncher taskLauncher;

        public Entity source;
        public List<Entity> sourceList;

    }
}