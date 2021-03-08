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
        public override void OnAwake()
        {
            base.OnAwake();
            navMeshAgent = GetComponent<NavMeshAgent>();
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
                navMeshAgent.isStopped = true;
                (selfUnit.Value.animCtrl as MedicalAnimCtrl).CureAnim();
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
            
        }

        public override void OnEnd()
        {
            base.OnEnd();
            navMeshAgent.isStopped = false;
        }

        bool IsInCureRange()
        {
            return UnityTool.GetDistanceIgnoreY(selfUnit.Value.transform.position,
                       (selfUnit.Value as MedicalSolider).cureTarget.transform.position) <
                   selfUnit.Value.prop.attackDistance;
        }
        
    }
}