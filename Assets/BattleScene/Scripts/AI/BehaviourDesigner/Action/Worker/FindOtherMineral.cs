using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tactical.Tasks.Worker
{
    [TaskCategory("MyRTS/Solider/Worker")]
    public class FindOtherMineral : Action
    {
        public SharedBattleUnit selfUnit;
        public int findRadius=30;
        public override TaskStatus OnUpdate()
        {
            BattleUnitBase otherMineralUnit = FindOtherClosetMineral();
            if (otherMineralUnit == null)
            {
                return TaskStatus.Failure;
            }
            ResourceInfo otherMineral = otherMineralUnit.GetComponent<ResourceInfo>();
            if (otherMineral != null)
            {
                (selfUnit.Value as WorkerUnit).SetMineTarget(otherMineral);
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        private BattleUnitBase FindOtherClosetMineral()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, findRadius);
            for (int i = 0; i < colliders.Length; i++)
            {
                BattleUnitBase battleUnitBase = colliders[i].GetComponent<BattleUnitBase>();
                if (battleUnitBase && battleUnitBase.configId == BattleUnitId.Mineral && battleUnitBase.IsInFog()==false &&
                    (battleUnitBase as MineralUnit).HasWorkerWorking==false && (battleUnitBase as MineralUnit).HasMineMachine == false)
                {
                    return battleUnitBase;
                }
            }

            return null;
        }
    }
    
}