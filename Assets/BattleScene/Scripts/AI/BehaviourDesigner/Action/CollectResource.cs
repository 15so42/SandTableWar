using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityTimer;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    [TaskCategory("MyRTS/Solider/Worker")]
    public class CollectResource : Action
    {
        public SharedBattleUnit selfUnit;
        public SharedFloat SetAnimTime;//播放设置矿机的时间
        public override void OnStart()
        {
            //播放人物动画，在开始播放人物动画时，矿机通过DoTween进行设置，矿机一定时间后完成。并播放设置完成动画，矿机设置完成后矿物的持有工人和持有矿机状态更新
            //在设置状态中移动则会毁坏矿机
            Animator anim = GetComponent<Animator>();
            
            WorkerUnit workerUnit=selfUnit.Value as WorkerUnit;
            ResourceInfo resourceInfo = workerUnit.GetComponent<ResourceCollector>().GetTarget();
            ResourceType resourceType = resourceInfo.resourceTypeInfo.resourceType;

            switch (resourceType)
            {
                case ResourceType.Mineral:
                    anim.SetBool("SetMineMachine",true);
                    workerUnit.SetMineMachine();
                    break;
                case ResourceType.Wood:
                    anim.SetBool("CollectWood",true);
                    //使用帧事件来看书
                    break;
            }
           
        }
    }
}