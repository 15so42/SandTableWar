
    public class EnemyAliveDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            return controller.enemy != null && controller.enemy.prop.hp > 0;
        }
    }
