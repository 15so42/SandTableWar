
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using UnityEngine;

    public class IsEnemyAlive : Conditional
    {
        public SharedBattleUnit selfUnit;
        public SharedBattleUnit enemyUnit;
        public override TaskStatus OnUpdate()
        {
            if (enemyUnit.Value == null)
            {
                return TaskStatus.Failure;
            }

            if (enemyUnit.Value.IsAlive())
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
