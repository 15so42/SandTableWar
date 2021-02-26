using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("CTF")]
    [TaskDescription("Check to see if the any object within the targets array is within sight")]
    public class WithinSight : Conditional
    {
        [Tooltip("The field of view angle of the nav agent (in degrees)")]
        public float fieldOfViewAngle;
        [Tooltip("How far out can the agent see")]
        public float viewMagnitude;
        [Tooltip("Returns success if an object within this array becomes within sight")]
        public Transform[] targets;
        [Tooltip("Returns success if this object becomes within sight")]
        public SharedTransform target;

        // magnitude * magnitude, taking the square root is expensive when it really doesn't need to be done
        private float sqrViewMagnitude;

        public override void OnAwake()
        {
            sqrViewMagnitude = viewMagnitude * viewMagnitude;
        }

        public override TaskStatus OnUpdate()
        {
            // Return success if a target is within sight
            for (int i = 0; i < targets.Length; ++i) {
                if (NPCViewUtilities.WithinSight(transform, targets[i], fieldOfViewAngle, sqrViewMagnitude)) {
                    // set the target so other tasks will know which transform is within sight
                    target.Value = targets[i];
                    return TaskStatus.Success;
                }
            }
            // a target is not within sight so return failure
            return TaskStatus.Failure;
        }

        // Draw the line of sight representation within the scene window
        public override void OnDrawGizmos()
        {
            NPCViewUtilities.DrawLineOfSight(Owner.transform, fieldOfViewAngle, sqrViewMagnitude);
        }
    }
}