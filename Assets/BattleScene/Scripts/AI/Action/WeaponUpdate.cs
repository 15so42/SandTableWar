
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using UnityEngine;

    public class WeaponUpdate : Action
    {
        public SharedBattleUnit selfUnit;
        public SharedBattleUnit enemyUnit;
        private Vector3 rotateVel = Vector3.zero;
        public bool needRotate=true;
        public override TaskStatus OnUpdate()
        {
            if (enemyUnit.Value == null)
            {
                return TaskStatus.Failure;
            }

            if (needRotate)
            {
                Transform ownerTransform = selfUnit.Value.transform;
                Vector3 dir = enemyUnit.Value.transform.position - ownerTransform.position;
                ownerTransform.forward = Vector3.SmoothDamp(ownerTransform.forward, dir, ref rotateVel, 0.3f);
            }
            
            selfUnit.Value.weapon.WeaponUpdate();
            return TaskStatus.Running;
        }

    }
