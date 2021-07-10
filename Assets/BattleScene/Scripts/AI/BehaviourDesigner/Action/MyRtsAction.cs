using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    public abstract class MyRtsAction : Action
    {
        protected NavMeshTacticalGroup.NavMeshTacticalAgent tacticalAgent;

        public override void OnAwake()
        {
            base.OnAwake();
            tacticalAgent = new NavMeshTacticalGroup.NavMeshTacticalAgent(transform);
        }

        public override void OnStart()
        {
            base.OnStart();
            if (tacticalAgent == null)
            {
                tacticalAgent = new NavMeshTacticalGroup.NavMeshTacticalAgent(transform);
            }
        }
    }
}