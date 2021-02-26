
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;

    public class SetBoolStatus : Action
    {
        public SharedBool boolVariable;
        public bool targetStatus;
        public override TaskStatus OnUpdate()
        {
            if (boolVariable == null)
            {
                return TaskStatus.Failure;
            }
            boolVariable.Value = targetStatus;
            return TaskStatus.Success;
        }
    }
