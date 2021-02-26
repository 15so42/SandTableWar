using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("CTF")]
    [TaskDescription("The flag has been captured. Celebrate by rotating in a circle.")]
    public class Celebrate : Action
    {
        [Tooltip("The speed to rotate")]
        public float rotationSpeed;

        // Will always return running. The behavior is going to be disabled soon so it never needs to return success/failure
        public override TaskStatus OnUpdate()
        {
            transform.Rotate(transform.up, rotationSpeed);
            return TaskStatus.Running;
        }
    }
}