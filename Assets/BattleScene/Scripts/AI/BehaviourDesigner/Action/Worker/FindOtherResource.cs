using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tactical.Tasks.Worker
{
    [TaskCategory("MyRTS/Solider/Worker")]
    public class FindOtherResource : Action
    {
        public SharedBattleUnit selfUnit;
        public override TaskStatus OnUpdate()
        {
            ResourceInfo otherResource = FindOtherNearestMineral();
            if (otherResource == null)
            {
                return TaskStatus.Failure;
            }
            
            (selfUnit.Value as WorkerUnit).SetTargetResource(otherResource);
            return TaskStatus.Success;
        }

        
        private ResourceInfo FindOtherNearestMineral()
        {
            return FightingManager.Instance.GetFaction(selfUnit.Value.factionId).FindOtherNearestMineral(
                (selfUnit.Value as WorkerUnit).resourceTarget.resourceTypeInfo.resourceType,
                selfUnit.Value.transform.position);
            
            
            // Collider[] colliders = Physics.OverlapSphere(transform.position, findRadius);
            // for (int i = 0; i < colliders.Length; i++)
            // {
            //     BattleUnitBase battleUnitBase = colliders[i].GetComponent<BattleUnitBase>();
            //     if (battleUnitBase && battleUnitBase.configId == BattleUnitId.Mineral && battleUnitBase.IsInFog()==false &&
            //         (battleUnitBase as MineralUnit).HasWorkerWorking==false && (battleUnitBase as MineralUnit).HasMineMachine == false)
            //     {
            //         return battleUnitBase;
            //     }
            // }
            //
            // return null;
        }
    }
    
}