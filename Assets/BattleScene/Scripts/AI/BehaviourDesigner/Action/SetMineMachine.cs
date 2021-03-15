﻿using BehaviorDesigner.Runtime.Tasks;
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
            //播放人物动画，在开始播放人物动画时，矿机通过DoTween进行设置，矿机一定时间后完成。并播放设置完成动画，矿机设置完成后矿物的持有工人和持有矿机状态更新
            //在设置状态中移动则会毁坏矿机
            Animator anim = GetComponent<Animator>();
            anim.SetBool("SetMineMachine",true);
            WorkerUnit workerUnit=selfUnit.Value as WorkerUnit;
            MineralUnit curMineral = workerUnit.mineTarget as MineralUnit;
            curMineral.HasWorkerWorking = true;

            workerUnit.SetMineMachine();
        }
    }
}