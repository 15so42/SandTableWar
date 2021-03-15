using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tactical.Tasks.Worker
{
    public class InterruptMineMachineSetUp : Action
    {
        public SharedBattleUnit selfUnit;
        public override TaskStatus OnUpdate()
        {
            (selfUnit.Value as WorkerUnit).InterruptMineMachineSetUp();
            return base.OnUpdate();
        }
    }
}