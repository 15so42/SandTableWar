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
            //如果正常完成资源收集则GetTarget为空
            if (resourceCollector.GetTarget() == null)
                return TaskStatus.Success;
            ResourceTypeInfo resourceTypeInfo = resourceCollector.GetTarget().resourceTypeInfo;
            switch (resourceTypeInfo.resourceType)
            {
                case BattleResType.Mineral:
                    (selfUnit.Value as WorkerUnit).InterruptMineMachineSetUp();
                    break;
                case BattleResType.Wood:
                    (selfUnit.Value as WorkerUnit).InterruptCollectWood();
                    break;
            }
            
            return base.OnUpdate();
        }
    }
}