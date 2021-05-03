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
            ResourceInfo otherResource = FindOtherNearestResource();
            if (otherResource == null)
            {
                return TaskStatus.Failure;
            }
            
            (selfUnit.Value as WorkerUnit).GetComponent<ResourceCollector>().SetTarget(otherResource);
            return TaskStatus.Success;
        }

        
        private ResourceInfo FindOtherNearestResource()
        {
            FactionManager factionManager = FightingManager.Instance.GetFaction(selfUnit.Value.factionId);
            return FightingManager.Instance.GetFaction(selfUnit.Value.factionId).FindOtherNearestResource(
                GetRandomResourceType()
                , selfUnit.Value.transform.position);
            
            
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

        private BattleResType GetRandomResourceType()
        {
            ResourceCollector resourceCollector = selfUnit.Value.resCollectorComp;
            return resourceCollector.collectionObjects[Random.Range(0, resourceCollector.collectionObjects.Length)]
                .resourceType.resourceType;
        }
    }
    
}