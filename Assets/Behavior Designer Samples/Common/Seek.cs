using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace BehaviorDesigner.Samples
{
    // Move towards the target specified using Unity's nav mesh
    [TaskCategory("Common")]
    public class Seek : Action
    {
        [Tooltip("The speed of the nav agent")]
        public SharedFloat moveSpeed;
        [Tooltip("The rotation of the nav agent")]
        public SharedFloat rotationSpeed;
        [Tooltip("true if the nav agent's rotation should end with the same rotation as the target")]
        public bool rotateToTarget;
        [Tooltip("avoid running into objects who are on defense. Note that this probabily should be made into a tag instead but to prevent having to update project files " +
                 "with these demos we are doing it this way")]
        public bool avoidDefenseUnits;
        [Tooltip("The position that the agent is seeking")]
        public SharedVector3 targetPosition;
        [Tooltip("The target that the agent is seeking")]
        public SharedTransform target;

        // remember the magnitude within the previous frame so we know if the target respawns and we no longer need to seek the target
        private float prevMagnitude = Mathf.Infinity;
        // true if the target was obtained from the targets position
        private bool staticPosition = false;
        // true if the nav agent is currently on an alternate path to avoid the defensive object
        private bool alternatePath = false;

        private NavMeshAgent navMeshAgent;

        public override void OnAwake()
        {
            // cache for quick lookup
            navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

            // set the speed and angular speed
            navMeshAgent.speed = moveSpeed.Value;
            navMeshAgent.angularSpeed = rotationSpeed.Value;
        }

        public override void OnStart()
        {
            navMeshAgent.enabled = true;

            // use the position if it is not zero
            if (targetPosition.Value != Vector3.zero) {
                staticPosition = true;
                navMeshAgent.destination = targetPosition.Value;
            }

            // set the destination if it hasn't already been set with a static position
            if (staticPosition == false) {
                navMeshAgent.destination = target.Value.position;
            }
        }

        // Move towards the destination. Return success once we have reached the destination. Return failure if the destination has respawned and we no longer should be seeking it.
        // Will return running if we are currently seeking
        public override TaskStatus OnUpdate()
        {
            // use the nav agent's destination position if we are on an alternate path or the target is null. We are using an alternate path if the previous path would have collided with
            // an object on defense. target will be null when we are seeking a position specified by the position variable
            var targetPosition = (alternatePath || target.Value == null ? navMeshAgent.destination : target.Value.position);
            targetPosition.y = navMeshAgent.destination.y; // ignore y

            // we can only arrive if the path isn't pending
            if (!navMeshAgent.pathPending) {
                var thisPosition = transform.position;
                thisPosition.y = targetPosition.y;
                // If the magnitude is less than the arrive magnitude then we have arrived at the destination
                if (Vector3.SqrMagnitude(thisPosition - navMeshAgent.destination) < SampleConstants.ArriveMagnitude) {
                    // If we arrived from an alternate path then switch back to the regular path
                    if (alternatePath) {
                        alternatePath = false;
                        targetPosition = target.Value.position;
                    } else {
                        // return success if we don't need to rotate to the target or we are already at the target's rotation
                        if (!rotateToTarget || transform.rotation == target.Value.rotation) {
                            return TaskStatus.Success;
                        }
                        // not done yet. still need to rotate
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, target.Value.rotation, rotationSpeed.Value * Time.deltaTime);
                    }
                }

                // fail if the target moved too quickly in one frame. This happens after the target has been caught and respawns
                float distance;
                if (prevMagnitude * 2 < (distance = Vector3.SqrMagnitude(thisPosition - targetPosition))) {
                    return TaskStatus.Failure;
                }
                prevMagnitude = distance;
            }

            // try not to head directly for a defensive object
            RaycastHit hit;
            Vector3 hitPoint;
            if (avoidDefenseUnits && (rayCollision(transform.position - transform.right, targetPosition, out hit) ||
                                                rayCollision(transform.position + transform.right, targetPosition, out hit))) {
                // looks like an object is within the path. Avoid the object by setting a new destination towards the right of the object that we would have hit
                hitPoint = hit.point + transform.right * 5;
                hitPoint.y = transform.position.y;
                navMeshAgent.destination = hitPoint;
                // The avoid object may still be in the way even though we moved to the right of the object. If this is the case then move to the left and hope that works
                if (rayCollision(transform.position, navMeshAgent.destination, out hit)) {
                    hitPoint = hit.point - transform.right * 5;
                    hitPoint.y = transform.position.y;
                    navMeshAgent.destination = hitPoint;
                }

                // remember that we are taking an alternate path to prevent the agent from jittering back and forth
                alternatePath = true;
                var thisPosition = transform.position;
                thisPosition.y = hitPoint.y;
                prevMagnitude = Vector3.SqrMagnitude(thisPosition - hitPoint);
            } else if (navMeshAgent.destination != targetPosition) {
                // the target position has changed since we last set the destination. Update the destination
                navMeshAgent.destination = targetPosition;
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            // reset the variables and disable the nav mesh agent when the task ends
            alternatePath = false;
            prevMagnitude = Mathf.Infinity;

            navMeshAgent.enabled = false;
        }

        // cast a ray between startPosition and targetPosition. Return true if a defensive object was hit
        private bool rayCollision(Vector3 startPosition, Vector3 targetPosition, out RaycastHit hit)
        {
            if (avoidDefenseUnits && Physics.Raycast(startPosition, targetPosition - startPosition, out hit, Mathf.Infinity)) {
                NPC npc = null;
                if ((npc = hit.collider.GetComponent<NPC>()) != null) {
                    return !npc.IsOffense;
                }
            }
            hit = new RaycastHit();
            return false;
        }
    }
}