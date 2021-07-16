

using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    [TaskCategory("MyRTS")]
    public class SyncDestinationPosToCurPos : Action
    {
        public HasNewMoveEvent hasNewMoveEvent;
        public SharedVector3 DestinationPos;
        public SharedVector3 LastDestinationPos;

        public override TaskStatus OnUpdate()
        {
            hasNewMoveEvent?.DisableInput();
           
            DestinationPos.SetValue(transform.position);
            LastDestinationPos.SetValue(DestinationPos.Value);
            if (hasNewMoveEvent != null)
            {
                Debug.Log("Synced");
                Debug.Log("destionPos"+DestinationPos.Value);
                Debug.Log("lastPos"+LastDestinationPos.Value);
            }

            hasNewMoveEvent?.StartSyncTime();
            hasNewMoveEvent?.EnableInput();
            return TaskStatus.Success;
        }
    }
}