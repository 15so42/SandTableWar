using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BattleScene.Scripts.AI.BehaviourDesigner
{
    [TaskCategory("MyRTS/Solider/Worker")]
    public class HasMineTarget : Conditional
    {
        public SharedBattleUnit selfUnit;
        public override TaskStatus OnUpdate()
        {
            WorkerUnit workerUnit=selfUnit.Value as WorkerUnit;
            if (workerUnit.mineTarget != null)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}