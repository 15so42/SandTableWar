using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BattleScene.Scripts.AI.BehaviourDesigner
{
    [TaskCategory("MyRTS/Solider/Worker")]
    public class HasMineTarget : Conditional
    {
        public SharedBattleUnit selfUnit;
        public SharedGameObject mineralGameObject;
        public override TaskStatus OnUpdate()
        {
            WorkerUnit workerUnit=selfUnit.Value as WorkerUnit;
            if (workerUnit.resourceTarget != null)
            {
                mineralGameObject.Value = workerUnit.resourceTarget.gameObject;
                return TaskStatus.Success;
            }
                
                
            return TaskStatus.Failure;
        }
    }
}