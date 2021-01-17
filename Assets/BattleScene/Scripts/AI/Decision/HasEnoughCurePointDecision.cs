
    public class HasEnoughCurePointDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            return (controller.owner as MedicalSolider).curePoint > 5;
        }
    }
