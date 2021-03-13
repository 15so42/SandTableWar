using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityTimer;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    [TaskCategory("MyRTS/Solider/Worker")]
    public class SetMineMachine : Action
    {
        public SharedFloat SetAnimTime;//播放设置矿机的时间
        public override void OnStart()
        {
            Animator anim = GetComponent<Animator>();
            anim.SetBool("SetMineMachine",true);
            Timer.Register(SetAnimTime.Value, () =>
            {
                anim.SetBool("SetMineMachine",false);
            });
        }
    }
}