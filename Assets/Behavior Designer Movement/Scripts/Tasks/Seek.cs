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
        private NavMeshVehicleMovement navMeshVehicleMovement;

        public override void OnAwake()
        {
            base.OnAwake();
            selfUnit = GetComponent<BattleUnitBase>();
            navMeshVehicleMovement = GetComponent<NavMeshVehicleMovement>();
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
            if (navMeshVehicleMovement && navMeshVehicleMovement.isTurnRound)
            {
                navMeshVehicleMovement.SetRealDest(Target());
                return TaskStatus.Running;
            }
            SetDestination(Target());
           
            if (HasArrived()) {
                return TaskStatus.Success;
            }
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

        protected override void Stop()
        {
            base.Stop();
            selfUnit.unitMovement.StopMove();
        }

        public override void OnEnd()
        {
            target.Value = null;
            base.OnEnd();
        }
    }
}