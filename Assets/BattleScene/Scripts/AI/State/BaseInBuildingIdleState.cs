
    public class BaseInBuildingIdleState : State
    {
        public BaseInBuildingIdleState(StateController controller, string stateName) : base(controller, stateName)
        {
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            ownerController.navMeshAgent.isStopped = true;
        }
    }
