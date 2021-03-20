using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    [TaskCategory("MyRTS")]
    public class SetCurPostToDest : MyRtsAction
    {
        public override TaskStatus OnUpdate()
        {
            tacticalAgent.SetDestination(transform.position);
            return base.OnUpdate();
        }
    }
}