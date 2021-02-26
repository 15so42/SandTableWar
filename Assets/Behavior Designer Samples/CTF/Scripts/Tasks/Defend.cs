using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("CTF")]
    [TaskDescription("Defend the object specified from the target.")]
    public class Defend : Action
    {
        [Tooltip("The speed of the nav agent")]
        public SharedFloat moveSpeed;
        [Tooltip("The rotation of the nav agent")]
        public SharedFloat rotationSpeed;

        // the transform of the object to defend
        public Transform defendObject;
        // defend within the specified radius of defeindObject
        public float defendRadius;

        // defend from the object specified
        public SharedTransform target;

        // remember the magnitude within the previous frame so we know if the target respawns and we no longer need to seek the target
        private float prevMagnitude = Mathf.Infinity;
        // defendRadius * defendRadius, taking the square root is expensive when it really doesn't need to be done
        private float sqrDefendRadius;

        private NavMeshAgent navMeshAgent;

        public override void OnAwake()
        {
            // cache for quick lookup
            navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

            // set the speed and angular speed
            navMeshAgent.speed = moveSpeed.Value;
            navMeshAgent.angularSpeed = rotationSpeed.Value;

            sqrDefendRadius = defendRadius * defendRadius;
        }

        public override void OnStart()
        {
            // set the destination to the target's position
            navMeshAgent.enabled = true;
            var targetPosition = target.Value.position;
            targetPosition.y = navMeshAgent.destination.y; // ignore y
            if (targetPosition != navMeshAgent.destination) {
                navMeshAgent.destination = targetPosition;
            }
        }

        public override TaskStatus OnUpdate()
        {
            // we can only reach the target if the path isn't pending
            if (!navMeshAgent.pathPending) {
                var thisPosition = transform.position;
                thisPosition.y = navMeshAgent.destination.y; // ignore y
                float sqrMgnitude = Vector3.SqrMagnitude(thisPosition - navMeshAgent.destination);
                // return failure if we are outside our area to defend
                if (sqrMgnitude > sqrDefendRadius) {
                    return TaskStatus.Failure;
                } else if (sqrMgnitude < SampleConstants.ArriveMagnitude) { // return success if we reached our target
                    return TaskStatus.Success;
                }

                // fail if the target moved too quickly in one frame. This happens after the target has been caught and respawns
                if (prevMagnitude * 2 < sqrMgnitude) {
                    return TaskStatus.Failure;
                }
                prevMagnitude = sqrMgnitude;
            }

            // set a new destination if the target has moved
            var targetPosition = target.Value.position;
            targetPosition.y = navMeshAgent.destination.y; // ignore y
            if (targetPosition != navMeshAgent.destination) {
                navMeshAgent.destination = targetPosition;
            }

            // keep running as long as we are within the radius to defend and haven't caught the target yet
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            // reset the variables
            prevMagnitude = Mathf.Infinity;
            navMeshAgent.enabled = false;
        }

        // Draw the area that we are defending within the editor scene window
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (defendObject != null) {
                var oldColor = UnityEditor.Handles.color;
                UnityEditor.Handles.color = new Color(1, 1, 0, 0.3f);
                UnityEditor.Handles.DrawWireDisc(defendObject.position, defendObject.up, defendRadius);
                UnityEditor.Handles.color = oldColor;
            }
#endif
        }
    }
}