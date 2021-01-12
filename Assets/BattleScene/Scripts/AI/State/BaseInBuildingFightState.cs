
    public class BaseInBuildingFightState : State
    {
        public BaseInBuildingFightState(StateController controller, string stateName) : base(controller, stateName)
        {
        }
        
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            ownerController.navMeshAgent.isStopped = true;
        }
        
        
    }
