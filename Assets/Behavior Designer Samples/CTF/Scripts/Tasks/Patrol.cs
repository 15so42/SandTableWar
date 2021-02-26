using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("CTF")]
    [TaskDescription("Patrols the nav agent around the waypoints. It will return success if a target becomes in sight.")]
    public class Patrol : Action
    {
        [Tooltip("The speed of the nav agent")]
        public SharedFloat moveSpeed;
        [Tooltip("The rotation of the nav agent")] 
        public SharedFloat rotationSpeed;
        [Tooltip("The field of view angle of the nav agent (in degrees)")] 
        public float fieldOfViewAngle;
        [Tooltip("How far out can the agent see")] 
        public float viewMagnitude;
        [Tooltip("The patrol waypoints")] 
        public Transform[] waypoints;
        [Tooltip("Return success if one of these targets become within sight")] 
        public Transform[] targets;
        [Tooltip("The transform of the object that we found while seeking")]
        public SharedTransform target;

        // the current index that we are heading towards within the waypoints array
        private int waypointIndex;
        // magnitude * magnitude, taking the square root is expensive when it really doesn't need to be done
        private float sqrViewMagnitude;

        private NavMeshAgent navMeshAgent;

        public override void OnAwake()
        {
            // cache for quick lookup
            navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

            // set the speed and angular speed
            navMeshAgent.speed = moveSpeed.Value;
            navMeshAgent.angularSpeed = rotationSpeed.Value;

            // initially move towards the closest waypoint
            float distance = Mathf.Infinity;
            float localDistance;
            for (int i = 0; i < waypoints.Length; ++i) {
                if ((localDistance = Vector3.Magnitude(transform.position - waypoints[i].position)) < distance) {
                    distance = localDistance;
                    waypointIndex = i;
                }
            }

            sqrViewMagnitude = viewMagnitude * viewMagnitude;
        }

        public override void OnStart()
        {
            navMeshAgent.enabled = true;
            navMeshAgent.destination = waypoints[waypointIndex].position;
        }

        public override TaskStatus OnUpdate()
        {
            // succceed if a target is within sight
            for (int i = 0; i < targets.Length; ++i) {
                if (NPCViewUtilities.WithinSight(transform, targets[i], fieldOfViewAngle, sqrViewMagnitude)) {
                    // set the target so the next task will know which transform it should target
                    target.Value = targets[i];
                    return TaskStatus.Success;
                }
            }

            // we can only arrive at the next waypoint if the path isn't pending
            if (!navMeshAgent.pathPending) {
                var thisPosition = transform.position;
                thisPosition.y = navMeshAgent.destination.y; // ignore y
                if (Vector3.SqrMagnitude(thisPosition - navMeshAgent.destination) < SampleConstants.ArriveMagnitude) {
                    // cycle through the waypoints
                    waypointIndex = (waypointIndex + 1) % waypoints.Length;
                    navMeshAgent.destination = waypoints[waypointIndex].position;
                }
            }

            // if no target is within sight then keep patroling
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            // disable the nav agent
            navMeshAgent.enabled = false;
        }

        public override void OnReset()
        {
            moveSpeed.Value = 0;
            rotationSpeed.Value = 0;
            fieldOfViewAngle = 0;
            viewMagnitude = 0;
            waypoints = null;
            targets = null;
        }

        // Draw the line of sight representation within the scene window
        public override void OnDrawGizmos()
        {
            NPCViewUtilities.DrawLineOfSight(Owner.transform, fieldOfViewAngle, viewMagnitude);
        }
    }
}