
    public class IsInBuildingDecision :Decision
    {
        public override bool Decide(StateController controller)
        {
            return controller.owner.isInBuilding;
        }
    }
