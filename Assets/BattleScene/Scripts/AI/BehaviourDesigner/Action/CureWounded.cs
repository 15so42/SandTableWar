using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
   
    [TaskCategory("MyRTS/Solider/Medical")]
    public class CureWounded : MyRtsAction
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
            if (navMeshAgent.enabled)
            {
                navMeshAgent.isStopped = false;
            }
            base.OnStart();
            //startRotation = selfUnit.Value.overrideRotationCtrl;
            //selfUnit.Value.UpdateRotation(true);
            
        }

        public  override TaskStatus  OnUpdate()
        {
            if (wounded.Value == null)
            {
                return TaskStatus.Failure;
            }

            MedicalSolider medicalSolider = selfUnit.Value as MedicalSolider;
            if (wounded.Value == selfUnit.Value)
            {
                //是自己的话直接加血
                medicalSolider.SetCureTarget(wounded.Value);
                medicalSolider.CureTargetUnit();
                return TaskStatus.Success;
            }
            medicalSolider.SetCureTarget(wounded.Value);
            tacticalAgent.SetDestination(wounded.Value.transform.position);
            
            
            if (IsInCureRange()) 
            {
                navMeshAgent.isStopped = true;
                if (tacticalAgent.RotateTowardsPosition(wounded.Value.transform.position))
                {
                    (selfUnit.Value.animCtrl as MedicalAnimCtrl).CureAnim();
                    return TaskStatus.Success;
                }
            }
            return TaskStatus.Running;
            
        }

        public override void OnEnd()
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(transform.position);
            base.OnEnd();
        }

        bool IsInCureRange()
        {
            return UnityTool.GetDistanceIgnoreY(selfUnit.Value.transform.position,
                       (selfUnit.Value as MedicalSolider).cureTarget.transform.position) <
                   selfUnit.Value.prop.attackDistance;
        }
        
    }
}