
    public class InAttackRangeDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            return UnityTool.GetDistanceIgnoreY(controller.chaseTarget.transform.position,
                controller.owner.transform.position) < controller.owner.prop.attackDistance;
        }
    }
