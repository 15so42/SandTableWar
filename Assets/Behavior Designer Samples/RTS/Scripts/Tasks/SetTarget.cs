using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("RTS")]
    [TaskDescription("Set the SharedTransform value to target. If target is null then find the target with target name")]
    public class SetTarget : Action
    {
        [Tooltip("The target to set the SharedTransform's value to. If target is null then targetName will be used")]
        public Transform target;
        [Tooltip("targetNamed is used in cases when a behavior is saved out to a asset and we need to reference an object within the scene")]
        public string targetName = "";
        [Tooltip("The returned variable that we are setting the target to")]
        public SharedTransform targetVariable = null;

        public override void OnAwake()
        {
            // do a search if target is null
            if (targetName.Length > 0) {
                target = GameObject.Find(targetName).transform;
            }
        }

        // will return success after the target has been set
        public override TaskStatus OnUpdate()
        {
            // set the variable's target
            targetVariable.Value = target;
            return TaskStatus.Success;
        }
    }
}