using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Samples.UtilityTheory
{
    [TaskCategory("UtilityTheory")]
    public class Seek : Action
    {
        public SharedTransform target;

        private UnityEngine.AI.NavMeshAgent navMeshAgent;

        public override void OnAwake()
        {
            navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        public override void OnStart()
        {
            navMeshAgent.SetDestination(target.Value.position);
        }

        public override TaskStatus OnUpdate()
        {
            if (Vector3.Distance(transform.position, navMeshAgent.destination) < navMeshAgent.stoppingDistance) {
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }
}