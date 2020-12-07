
    using UnityEngine;

    public class ReachTargetPosDecision:Decision
    {
        public override bool Decide(StateController controller)
        {
            float distance = Vector3.Distance(controller.owner.transform.position, controller.targetPos);
            return distance < 0.5f;//todo 更详细的判断
        }
    }
