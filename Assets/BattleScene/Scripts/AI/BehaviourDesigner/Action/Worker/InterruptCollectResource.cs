using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tactical.Tasks.Worker
{
    [TaskCategory("MyRTS/Solider/Worker/InterruptCollectResource")]
    public class InterruptCollectResource : Action
    {
        public SharedBattleUnit selfUnit;
        public override TaskStatus OnUpdate()
        {
            ResourceCollector resourceCollector = selfUnit.Value.GetComponent<ResourceCollector>();
            ResourceTypeInfo resourceTypeInfo = resourceCollector.GetTarget().resourceTypeInfo;
            switch (resourceTypeInfo.resourceType)
            {
                case ResourceType.Mineral:
                    (selfUnit.Value as WorkerUnit).InterruptMineMachineSetUp();
                    break;
                case ResourceType.Wood:
                    (selfUnit.Value as WorkerUnit).InterruptCollectWood();
                    break;
            }
            
            return base.OnUpdate();
        }
    }
}