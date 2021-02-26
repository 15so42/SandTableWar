using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Samples
{
    [TaskCategory("MiniGauntlet")]
    [TaskDescription("A do nothing task. Returns success immediately.")]
    public class Idle : Action
    {
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}