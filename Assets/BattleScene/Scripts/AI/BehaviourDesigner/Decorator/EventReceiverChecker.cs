using BehaviorDesigner.Runtime.Tasks;

namespace BattleScene.Scripts.AI.BehaviourDesigner.Decorator
{
    public class EventReceiverChecker : BehaviorDesigner.Runtime.Tasks.Decorator
    {
        // The status of the child after it has finished running.
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override bool CanExecute()
        {
            // Keep running until the child task returns success.
            return executionStatus == TaskStatus.Failure || executionStatus == TaskStatus.Inactive;
        }
        
        public override TaskStatus Decorate(TaskStatus status)
        {
            // Invert the task status.
            if (status == TaskStatus.Success) {
                return TaskStatus.Success;
            } else if (status == TaskStatus.Failure) {
                return TaskStatus.Failure;
            }
            return status;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // Update the execution status after a child has finished running.
            executionStatus = childStatus;
        }
        
        public override void OnEnd()
        {
            // Reset the execution status back to its starting values.
            executionStatus = TaskStatus.Inactive;
        }
    }
}