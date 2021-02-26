
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using UnityEngine;
    
    public class IsBoolTrue :Conditional
    {
        public SharedBool boolVariable;
        
        public override TaskStatus OnUpdate()
        {
            if (boolVariable.Value)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    
    }
