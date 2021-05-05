
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using UnityEngine;

    [TaskCategory("MyRTS/Worker")]
    public class EndCollectResource : Action
    {
        public SharedBattleUnit selfUnit;
        private WorkerUnit workerUnit;
        private ResourceInfo resourceInfo;
        private BattleResType resourceType;
        private Animator anim;

        public override void OnStart()
        {
            base.OnStart();
            WorkerUnit workerUnit=selfUnit.Value as WorkerUnit;
            resourceInfo = workerUnit.GetComponent<ResourceCollector>().GetTarget();
            resourceType = resourceInfo.resourceTypeInfo.resourceType;
            anim = selfUnit.Value.GetComponent<Animator>();
        }

        public override TaskStatus OnUpdate()
        {
            switch (resourceType)
            {
                case BattleResType.Mineral:
                    anim.SetBool("SetMineMachine",false);
                    break;
                case BattleResType.Wood:
                    anim.SetBool("CollectWood",false);
                    //使用帧事件来看书
                    break;
            }
            EventCenter.Broadcast(EnumEventType.OnUnitStopCollecting,selfUnit.Value,resourceInfo);
            TipsDialog.ShowDialog("采集完成",false);

            return TaskStatus.Success;
            
        }
    }
