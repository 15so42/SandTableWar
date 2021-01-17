
    public class BaseChaseAction : StateAction
    {
        public override void Act(StateController controller)
        {
            controller.navMeshAgent.SetDestination(controller.chaseTarget.transform.position);
        }
    }
