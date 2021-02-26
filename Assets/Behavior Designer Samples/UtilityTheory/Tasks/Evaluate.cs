using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Samples.UtilityTheory
{
    [TaskCategory("UtilityTheory")]
    public class Evaluate : Action
    {
        [Tasks.Tooltip("The curve of the utility function")]
        public AnimationCurve curve;
        [Tasks.Tooltip("The current branch should be active for the specified amount of time")]
        public SharedFloat activeDuration = 4;
        [Tasks.Tooltip("The utility curve should return 1 after the specified amount of time")]
        public SharedFloat inactiveMaxDuration = 10;
        [Tasks.Tooltip("For debugging the utility value will be outputted every tick")]
        public SharedString branchName;

        private bool isActive;
        private float value;
        private float startTime;
        private float startInactiveTime;

        public override void OnAwake()
        {
            startInactiveTime = Time.time;
        }

        public override void OnStart()
        {
            // When the task starts set the active state, start time, and value. The value should be set to 0 because the utility value is now based on the duration of the
            // action rather than a curve.
            isActive = true;
            startTime = Time.time;
            value = 0;
        }

        public override TaskStatus OnUpdate()
        {
            // Keep returning a status of running - the task will be aborted when its utility value is no longer the highest
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            isActive = false;
            startInactiveTime = Time.time;
        }

        public override float GetUtility()
        {
            // When the task is inactive the utility value should increase indicating that the branch has a higher likelihood of being run.
            // When the task is active the utility value should return 1 indicating that the current branch should run. As soon as the task has been active for the 
            // active duration then a value of 0 will be returned indicating that the branch should no longer be run.
            if (isActive) {
                if (startTime + activeDuration.Value > Time.time) {
                    return 1;
                }
            } else {
                value = curve.Evaluate(Mathf.Clamp01((Time.time - startInactiveTime) / inactiveMaxDuration.Value));
            }
            Debug.Log(branchName.Value + " utility value: " + value);
            return value;
        }
    }
}