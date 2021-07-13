

using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    [TaskCategory("MyRTS")]
    public class SyncDestinationPosToCurPos : Action
    {
        public SharedVector3 DestinationPos;
        public SharedVector3 LastDestinationPos;

        public override TaskStatus OnUpdate()
        {
            DestinationPos.SetValue(transform.position);
            LastDestinationPos.SetValue(DestinationPos.Value);
            return TaskStatus.Success;
        }
    }
}