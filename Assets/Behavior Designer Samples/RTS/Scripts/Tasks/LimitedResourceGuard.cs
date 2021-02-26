using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("RTS")]
    [TaskDescription("A decorator task which prevents more than one object occupying the resource")]
    public class LimitedResourceGuard : Decorator
    {
        [Tooltip("A reference to the transform of the limited resource")]
        public SharedTransform limitedResourceTransform;
        private bool executing = false;

        private LimitedResource limitedResource;
        private Transform thisTransform;

        public override void OnAwake()
        {
            // cache for quick lookup
            thisTransform = transform;
        }

        public override void OnStart()
        {
            // Find the limited resource object from the target. If the resource is null then check the parent.
            if ((limitedResource = limitedResourceTransform.Value.GetComponent<LimitedResource>()) == null) {
                limitedResource = limitedResourceTransform.Value.parent.GetComponent<LimitedResource>();
            }
        }

        // Child tasks can be executed if the resource is available. This means that the resource is not currently occupied and is not currently executing
        public override bool CanExecute()
        {
            return limitedResource.OccupiedBy == null && !executing;
        }

        // The child task has started to run therefore the resource is no longer available
        public override void OnChildStarted()
        {
            executing = true;
            limitedResource.OccupiedBy = thisTransform;
        }

        // The child task has completed so the resource is avilable again
        public override void OnEnd()
        {
            executing = false;
        }

        // If the child task is currently executing then this task is currently running
        public override TaskStatus OverrideStatus(TaskStatus status)
        {
            return !executing ? TaskStatus.Running : status;
        }
    }
}