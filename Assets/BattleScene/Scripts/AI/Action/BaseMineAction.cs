
    public class BaseMineAction : StateAction
    {
        public override void Act(StateController controller)
        {
            WorkerStateController workerController=controller as WorkerStateController;
            if (controller.chaseTarget != null)
            {
            }
        }
    }
