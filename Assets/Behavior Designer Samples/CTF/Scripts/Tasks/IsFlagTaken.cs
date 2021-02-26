using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("CTF")]
    [TaskDescription("Conditional task that returns if the flag has been taken")]
    public class IsFlagTaken : Conditional
    {
        private CTFGameManager gameManager;

        public override void OnAwake()
        {
            // cache for quick lookup
            gameManager = CTFGameManager.instance;
        }

        // will return success if the flag has been taken or failure if it has not been taken
        public override TaskStatus OnUpdate()
        {
            if (gameManager.IsFlagTaken)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}