using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace BattleScene.Scripts.AI.BehaviourDesigner
{
    public class HasWounded : Conditional
    {
        public SharedBattleUnit selfUnit;
        public SharedBattleUnit wounded;//伤员

        private NavMeshAgent navMeshAgent;
        public override void OnAwake()
        {
            base.OnAwake();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public override TaskStatus OnUpdate()
        {
            MedicalSolider medicalSolider = (selfUnit.Value as MedicalSolider);

            //每次寻找受伤最严重的敌人
                float minPercentage = 1;
                for (int i = 0; i < BattleUnitBase.selfUnits.Count; i++)
                {
                    BattleUnitBase unit = BattleUnitBase.selfUnits[i];
                    if (unit == null)
                        continue;
                    if (UnityTool.GetDistanceIgnoreY(medicalSolider.transform.position, unit.transform.position) <=
                        medicalSolider.prop.viewDistance)
                    {
                        float percentage = unit.prop.GetPercentage();
                        if (percentage <= minPercentage && percentage < 1)
                        {
                            minPercentage = percentage;
                            wounded.Value = unit;
                        }
                    }
                }
                
                return wounded.Value? TaskStatus.Success: TaskStatus.Failure;
        }

        
    }
}