
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using UnityEngine;

    public class HasEnemy :Conditional
    {
        public SharedBattleUnit enemyBattleUnit;

        private BattleUnitBaseProp prop;
        
        public override TaskStatus OnUpdate()
        {
            if (enemyBattleUnit.Value!=null)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    
    }
