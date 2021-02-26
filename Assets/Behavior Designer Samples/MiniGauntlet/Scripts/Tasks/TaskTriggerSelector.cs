using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Samples
{
    public enum TaskTriggerType { Idle, Jump, SphereCollision, PlaySound }
    [TaskCategory("MiniGauntlet")]
    [TaskDescription("A composite task that will choose its child based on the latest collision/trigger event.")]
    public class TaskTriggerSelector : Composite
    {
        // Start with the idle task
        private int nextTaskIndex = 0;

        public override int CurrentChildIndex()
        {
            // The next task index will be 0 unless a collision or trigger event has occurred
            return nextTaskIndex;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // Set the next task index back to 0 immediately after the child has executed to prevent a non-idle task from running multiple tiles
            nextTaskIndex = 0;
        }

        // OnTriggerEnter is called when the agent runs into a trigger that specifies what task to run text
        public override void OnTriggerEnter(Collider other)
        {
            TriggerType triggerType = null;
            if ((triggerType = other.GetComponent<TriggerType>()) != null) {
                nextTaskIndex = (int)triggerType.triggerType;
            }
        }

        // OnCollisionEnter is called when a sphere collides with the player
        public override void OnCollisionEnter(Collision collision)
        {
            nextTaskIndex = (int)TaskTriggerType.SphereCollision;
        }
    }
}