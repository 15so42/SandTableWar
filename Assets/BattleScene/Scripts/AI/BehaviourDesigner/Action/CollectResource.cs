
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using UnityEngine;

    public class CollectResource :Action
    {
        public SharedBattleUnit selfUnit;

        private ResourceCollector resourceCollector;
        public override void OnStart()
        {
            base.OnStart();
            resourceCollector = selfUnit.Value.resCollectorComp;
        }

        public override TaskStatus OnUpdate()
        {
            if (resourceCollector.OverWeight())
            {
                return TaskStatus.Success;
            }
            resourceCollector.CollectResource();
            return TaskStatus.Running;

        }
    }
