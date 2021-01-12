
    using UnityEngine;

    public class BaseTankFightAction : StateAction
    {
        //用于只执行一次
        public bool isStopped = false;
        private Vector3 rotateVel = Vector3.zero;
        public override void Act(StateController controller)
        {
            controller.owner.weapon.WeaponUpdate();
        }
        
        
    }
