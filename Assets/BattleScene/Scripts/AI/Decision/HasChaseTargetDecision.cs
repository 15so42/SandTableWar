
    using UnityEngine;

    public class HasChaseTargetDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            if (controller.chaseTarget == false)
                return false;
            Vector3 chaseTargetPos = controller.chaseTarget.transform.position;
            Vector3 selfPos = controller.owner.transform.position;
            if (UnityTool.GetDistanceIgnoreY(chaseTargetPos, selfPos) <= controller.owner.prop.attackDistance )
            {
                return false;
            }

            return true;
        }
    }
