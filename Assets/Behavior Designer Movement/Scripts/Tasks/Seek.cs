using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Seek the target specified using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class Seek : NavMeshMovement
    {
        [Tooltip("The GameObject that the agent is seeking")]
        public SharedGameObject target;
        [Tooltip("If target is null then use the target position")]
        public SharedVector3 targetPosition;

        private float maxSpeed;
        private float lastFrameSpeed;

        public override void OnStart()
        {
            base.OnStart();

            maxSpeed = navMeshAgent.speed;
            SetDestination(Target());
            //navMeshAgent.speed = 0.1f;
        }

        private float rotateMoveSpeed;//旋转时的速度,不是旋转速度
        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            if (HasArrived()) {
                return TaskStatus.Success;
            }

            SetDestination(Target());
            // if (Vector3.Angle(Target()-transform.position, transform.forward )>30f)
            // {
            //     //navMeshAgent.speed = 0.1f;
            //    
            //     //RotateTowardPos(Target());
            //     
            //     //navMeshAgent.Move(transform.forward *Time.deltaTime*rotateMoveSpeed);
            // }
            // else
            // {
            //     navMeshAgent.speed = maxSpeed;
            //     lastFrameSpeed = navMeshAgent.velocity.magnitude;
            // }
            //rotateMoveSpeed = Mathf.Lerp(rotateMoveSpeed, lastFrameSpeed, 0.1f*Time.deltaTime);
            navMeshAgent.Move(transform.forward *Time.deltaTime*2);

            return TaskStatus.Running;
        }
        
        // Return targetPosition if target is null
        private Vector3 Target()
        {
            if (target.Value != null) {
                return target.Value.transform.position;
            }
            return targetPosition.Value;
        }

        public override void OnReset()
        {
            base.OnReset();
            target = null;
            targetPosition = Vector3.zero;
        }
    }
}