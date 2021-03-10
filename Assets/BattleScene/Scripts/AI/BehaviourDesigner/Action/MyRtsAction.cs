using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    public abstract class MyRtsAction : Action
    {
        protected NavMeshTacticalGroup.NavMeshTacticalAgent tacticalAgent;

        public override void OnStart()
        {
            base.OnStart();
            tacticalAgent = new NavMeshTacticalGroup.NavMeshTacticalAgent(transform);
        }
    }
}