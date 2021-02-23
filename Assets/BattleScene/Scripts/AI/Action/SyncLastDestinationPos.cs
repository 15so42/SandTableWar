
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using UnityEngine;

    public class SyncLastDestinationPos : Action
    {
        public SharedVector3 lastDestinationPos;
        public SharedVector3 destinationPos;
        
        public override TaskStatus OnUpdate()
        {
            lastDestinationPos.Value = destinationPos.Value;
            return TaskStatus.Success;
        }

    }
