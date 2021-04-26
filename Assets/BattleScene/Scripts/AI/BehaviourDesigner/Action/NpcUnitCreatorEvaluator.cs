using BehaviorDesigner.Runtime.Tasks;


    [TaskCategory("MyTS/Commander")]
    public class NpcUnitCreatorEvaluator : Task
    {
        public SharedFactionManager sharedFactionManager;
        public SharedNpcCommander sharedNpcCommander;
        
        public override TaskStatus OnUpdate()
        {
            NpcCommander npcCommander = sharedNpcCommander.Value;
            if (npcCommander.isInLowResourcesTime())
            {
            }
            return base.OnUpdate();
        }
    }
