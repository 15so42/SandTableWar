using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tactical.Tasks.Worker
{
    [TaskCategory("MyRTS/Solider/Worker")]
    public class CancelMinePlan : Action
    {
        public SharedBattleUnit selfUnit;
        public override TaskStatus OnUpdate()
        {
            WorkerUnit workerUnit = selfUnit.Value as WorkerUnit;
            (workerUnit.mineTarget as MineralUnit).HasWorkerWorking = false;
            workerUnit.mineTarget.OnUnSelect();
            workerUnit.SetMineTarget(null);
            return TaskStatus.Success;
        }
    }
}