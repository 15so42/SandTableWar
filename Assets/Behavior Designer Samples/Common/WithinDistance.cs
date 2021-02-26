using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace BehaviorDesigner.Samples
{
    // Check to see if the any object within the targets array is within the distance specified
    [TaskCategory("Common")]
    public class WithinDistance : Conditional
    {
        [Tooltip("The distance that the target object needs to be within")]
        public float magnitude;
        [Tooltip("true if the target must be within sight to be within distance. If this is true then an object behind a wall will not be within distance even though it may " +
                 "be physically close to the other object")]
        public bool lineOfSight;
        [Tooltip("An array of targets to check")]
        public Transform[] targets;
        [Tooltip("If targets is null then fill the variable dynamically with the targetTag")]
        public string targetTag;
        [Tooltip("the shared target variable that will be set so other tasks know what the target is")]
        public SharedTransform target;

        // true if we obtained the targets through the targetTag
        private bool dynamicTargets;
        // distnace * distance, optimization so we don't have to take the square root
        private float sqrMagnitude;

        public override void OnAwake()
        {
            // initialize the variables
            sqrMagnitude = magnitude * magnitude;
            dynamicTargets = targets != null && targets.Length == 0;
        }

        public override void OnStart()
        {
            // if targets is null then find all of the targets using the targetTag
            if (targets == null || targets.Length == 0) {
                var gameObjects = GameObject.FindGameObjectsWithTag(targetTag);
                targets = new Transform[gameObjects.Length];
                for (int i = 0; i < gameObjects.Length; ++i) {
                    targets[i] = gameObjects[i].transform;
                }
            }
        }

        // returns success if any object is within distance of the current object. Otherwise it will return failure
        public override TaskStatus OnUpdate()
        {
            Vector3 direction;
            // check each target. All it takes is one target to be able to return success
            for (int i = 0; i < targets.Length; ++i) {
                direction = targets[i].position - transform.position;
                // check to see if the square magnitude is less than what is specified
                if (Vector3.SqrMagnitude(direction) < sqrMagnitude) {
                    // the magnitude is less. If lineOfSight is true do one more check
                    if (lineOfSight) {
                        if (NPCViewUtilities.LineOfSight(transform, targets[i], direction)) {
                            // the target has a magnitude less than the specified magnitude and is within sight. Set the target and return success
                            target.Value = targets[i];
                            return TaskStatus.Success;
                        }
                    } else {
                        // the target has a magnitude less than the specified magnitude. Set the target and return success
                        target.Value = targets[i];
                        return TaskStatus.Success;
                    }
                }
            }
            // no targets are within distance. Return failure
            return TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            // set the targets array to null if dynamic targets is true so the targets will be found again the next time the task starts
            if (dynamicTargets) {
                targets = null;
            }
        }

        // Draw the distance
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.up, magnitude);
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}