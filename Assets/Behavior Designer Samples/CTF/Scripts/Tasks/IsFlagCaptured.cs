using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("CTF")]
    [TaskDescription("Conditional task that returns if the flag has been captured")]
    public class IsFlagCaptured : Conditional
    {
        private CTFGameManager gameManager;

        public override void OnAwake()
        {
            // cache for quick lookup
            gameManager = CTFGameManager.instance;
        }

        // will return success if the flag has been captured or failure if it has not been captured
        public override TaskStatus OnUpdate()
        {
            // the game is active when the flag has not been captured
            if (gameManager.GameActive)
                return TaskStatus.Failure;
            return TaskStatus.Success;
        }
    }
}