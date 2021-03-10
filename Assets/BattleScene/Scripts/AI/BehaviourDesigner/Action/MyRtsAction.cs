using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    public class MyRtsAction : Action
    {
        protected TacticalAgent tacticalAgent;

        public override void OnStart()
        {
            base.OnStart();
            tacticalAgent = new NavMeshTacticalGroup.NavMeshTacticalAgent(transform);
        }
    }
}