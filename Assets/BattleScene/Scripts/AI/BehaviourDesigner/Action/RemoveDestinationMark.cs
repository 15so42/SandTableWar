
    using BattleScene.Scripts;
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;

    [TaskCategory("MyRTS")]
    public class RemoveDestinationMark : Action
    {
        public SharedBattleUnit selfUnit;
      

        public override TaskStatus OnUpdate()
        {
            selfUnit.Value.RemoveDestinationMark();
            return base.OnUpdate();
        }
    }
