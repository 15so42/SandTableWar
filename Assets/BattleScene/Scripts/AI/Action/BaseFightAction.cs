
    using UnityEngine;

    public class BaseFightAction : StateAction
    {
        //用于只执行一次
        public bool isStopped = false;
        private Vector3 rotateVel = Vector3.zero;
        public override void Act(StateController controller)
        {
            Transform ownerTransform = controller.owner.transform;
            Vector3 dir = controller.enemy.transform.position - ownerTransform.position;
            ownerTransform.forward = Vector3.SmoothDamp(ownerTransform.forward, dir, ref rotateVel, 0.3f);
            controller.owner.weapon.WeaponUpdate();
        }
    }
