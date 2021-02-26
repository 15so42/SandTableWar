using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("RTS")]
    [TaskDescription("Conditional task which determines if the limited resouce is occupied")]
    public class IsResourceAvailable : Conditional
    {
        [@Tooltip("The resource that we are interested in")]
        public LimitedResource resource;

        // return success if the resource is empty and failure if it is not
        public override TaskStatus OnUpdate()
        {
            if (resource.OccupiedBy == null) {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}