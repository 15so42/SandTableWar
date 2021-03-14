using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityTimer;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    [TaskCategory("MyRTS/Solider/Worker")]
    public class SetMineMachine : Action
    {
        public SharedBattleUnit selfUnit;
        public SharedFloat SetAnimTime;//播放设置矿机的时间
        public override void OnStart()
        {
            Animator anim = GetComponent<Animator>();
            anim.SetBool("SetMineMachine",true);
            WorkerUnit workerUnit=selfUnit.Value as WorkerUnit;
            MineralUnit curMineral = workerUnit.mineTarget as MineralUnit;

            curMineral.HasWorkerWorking = true;
            Timer.Register(SetAnimTime.Value, () =>
            {
                curMineral.HasWorkerWorking = false;
                curMineral.HasMineMachine = true;
                anim.SetBool("SetMineMachine",false);
            });
        }
    }
}