
    using UnityEngine;

    public class BaseTankFightAction : StateAction
    {
        //用于只执行一次
        public bool isStopped = false;
        private Vector3 rotateVel = Vector3.zero;
        public override void Act(StateController controller)
        {
            //停止移动,但是目标地点不变，当战斗结束后将isStopped设置为false继续像目标位置前进
            controller.navMeshAgent.isStopped = true;
            //Transform ownerTransform = controller.owner.transform;
            controller.owner.weapon.WeaponUpdate();
        }
    }
