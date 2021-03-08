using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
   
    
    public class CureWounded : Action
    {
        public SharedBattleUnit selfUnit;
        public SharedBattleUnit wounded;
        
        private NavMeshAgent navMeshAgent;

        private bool startRotation;
        public override void OnAwake()
        {
            base.OnAwake();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public override void OnStart()
        {
            navMeshAgent.isStopped = false;
            base.OnStart();
            startRotation = selfUnit.Value.overrideRotationCtrl;
            selfUnit.Value.UpdateRotation(true);
            
        }

        public  override TaskStatus  OnUpdate()
        {
            if (wounded.Value == null)
            {
                return TaskStatus.Failure;
            }

            MedicalSolider medicalSolider = selfUnit.Value as MedicalSolider;
            medicalSolider.SetCureTarget(wounded.Value);
            navMeshAgent.SetDestination(wounded.Value.transform.position);
            
            if (IsInCureRange()) 
            {
                if (Vector3.Angle(selfUnit.Value.transform.forward, navMeshAgent.desiredVelocity) < 5)
                {
                    navMeshAgent.isStopped = true;
                    (selfUnit.Value.animCtrl as MedicalAnimCtrl).CureAnim();
                    return TaskStatus.Success;
                }
            }
            return TaskStatus.Running;
            
        }

        public override void OnEnd()
        {
            base.OnEnd();
            selfUnit.battleUnitBase.UpdateRotation(startRotation);
        }

        bool IsInCureRange()
        {
            return UnityTool.GetDistanceIgnoreY(selfUnit.Value.transform.position,
                       (selfUnit.Value as MedicalSolider).cureTarget.transform.position) <
                   selfUnit.Value.prop.attackDistance;
        }
        
    }
}