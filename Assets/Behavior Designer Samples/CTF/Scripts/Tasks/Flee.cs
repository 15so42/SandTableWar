using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("CTF")]
    [TaskDescription("Flee in the opposite direction of the enemy")]
    public class Flee : Action
    {
        [Tooltip("The speed of the nav agent")]
        public SharedFloat moveSpeed;
        [Tooltip("The rotation of the nav agent")]
        public SharedFloat rotationSpeed;
        [Tooltip("The flee was successful when this distance from the enemy has been reached")]
        public float fleedDistance;
        [Tooltip("How far should we lookahead in the opposite direction")]
        public float lookAheadDistance;
        [Tooltip("Get the transform of the enemy that we are fleeing from")]
        public SharedTransform fleeFromTransform;

        // The position to flee to
        private Vector3 targetPosition;

        // fleedDistance * fleedDistance, taking the square root is expensive when it really doesn't need to be done
        private float sqrFleedDistance;

        private NavMeshAgent navMeshAgent;

        public override void OnAwake()
        {
            // cache for quick lookup
            navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

            // set the speed and angular speed
            navMeshAgent.speed = moveSpeed.Value;
            navMeshAgent.angularSpeed = rotationSpeed.Value;
            sqrFleedDistance = fleedDistance * fleedDistance;
        }

        public override void OnStart()
        {
            // If the transform is null then we have nobody to flee from
            if (fleeFromTransform.Value == null)
                return;

            // flee in the opposite direction
            targetPosition = OppositeDirection();
            navMeshAgent.enabled = true;
            navMeshAgent.destination = targetPosition;
        }

        public override TaskStatus OnUpdate()
        {
            // The flee failed if the transform no longer exists
            if (fleeFromTransform.Value == null) {
                return TaskStatus.Failure;
            }

            // The flee only has a chance of being successful if the path isn't pending
            if (!navMeshAgent.pathPending) {
                // get our position, ignoring y
                var thisPosition = transform.position;
                thisPosition.y = navMeshAgent.destination.y;

                // the flee was a success if we are far away from the enemy
                if (Vector3.SqrMagnitude(thisPosition - fleeFromTransform.Value.position) > sqrFleedDistance) {
                    return TaskStatus.Success;
                    // flee to a new position in the opposite direction if we have arrived at our flee destination
                } else if (Vector3.SqrMagnitude(thisPosition - navMeshAgent.destination) < SampleConstants.ArriveMagnitude) {
                    targetPosition = OppositeDirection();
                    navMeshAgent.destination = targetPosition;
                }
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            navMeshAgent.enabled = false;
        }

        private Vector3 OppositeDirection()
        {
            // compute a value in the opposite direction of the enemy
            return transform.position + (transform.position - fleeFromTransform.Value.position).normalized * lookAheadDistance;
        }
    }
}