
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using UnityEngine;

    [TaskCategory("MyRTS/Worker")]
    public class ReturnBase : Action
    {
        public SharedBattleUnit selfUnit;
        private Vector3 basePos;
        public override void OnStart()
        {
            basePos= FightingManager.Instance.GetFaction(selfUnit.Value.factionId)
                .FindNearest(BattleUnitId.Base, selfUnit.Value.transform.position).transform.position;
            selfUnit.Value.SetTargetPos(basePos);
        }

        public override TaskStatus OnUpdate()
        {
            if (Vector3.Distance(basePos, transform.position) < 2)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }
