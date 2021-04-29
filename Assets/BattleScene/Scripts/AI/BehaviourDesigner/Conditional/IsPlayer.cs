using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


    public class IsPlayer : Conditional
    {
        public SharedBattleUnit selfUnit;
        public override TaskStatus OnUpdate()
        {
            if (FightingManager.Instance.GetFaction(selfUnit.Value.factionId).IsPlayer())
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
