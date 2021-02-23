
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using UnityEngine;

    public class InAttackRange : Conditional
    {
        public SharedBattleUnit selfUnit;
        public SharedBattleUnit enemyUnit;
        public override TaskStatus OnUpdate()
        {
            if (enemyUnit.Value == null)
            {
                return TaskStatus.Failure;
            }
            if(Vector3.Distance(enemyUnit.Value.transform.position,
                selfUnit.Value.transform.position) < selfUnit.Value.prop.attackDistance)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
