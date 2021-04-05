using DefaultNamespace;
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

        private BattleUnitBase selfUnit;
        private NavMeshUnitMovement navMeshUnitMovement;

        public override void OnAwake()
        {
            base.OnAwake();
            selfUnit = GetComponent<BattleUnitBase>();
            navMeshUnitMovement = GetComponent<NavMeshUnitMovement>();
        }

        public override void OnStart()
        {
            base.OnStart();
            SetDestination(Target());
        }

       
        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            SetDestination(Target());
            
            if (navMeshUnitMovement && navMeshUnitMovement.isTurnRound)
            {
                return TaskStatus.Running;
            }
            if (HasArrived()) {
                return TaskStatus.Success;
            }

           // navMeshAgent.Move(transform.forward *6*Time.deltaTime);

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