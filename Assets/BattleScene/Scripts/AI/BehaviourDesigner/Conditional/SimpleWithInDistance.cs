using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BattleScene.Scripts.AI.BehaviourDesigner
{
    public class SimpleWithInDistance : Conditional
    {
        public float distance;
        public SharedGameObject target;
        public override TaskStatus OnUpdate()
        {
            return Vector3.Distance(transform.position, target.Value.transform.position) < distance
                ? TaskStatus.Success
                : TaskStatus.Failure;
        }
    }
}