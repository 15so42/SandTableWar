
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyRTS/Worker")]
    public class DeliverResource : Action
    {
        public SharedBattleUnit selfUnit;
        public override TaskStatus OnUpdate()
        {
            selfUnit.Value.resCollectorComp.DeliveryResource();
            return base.OnUpdate();
        }
    }
