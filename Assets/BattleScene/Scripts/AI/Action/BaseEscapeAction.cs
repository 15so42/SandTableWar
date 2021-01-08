
    using UnityEngine;
    using UnityEngine.AI;

    public class BaseEscapeAction : StateAction
    {
        public float lastTime;
        
        public override void Act(StateController controller)
        {
            NavMeshAgent navMeshAgent = controller.navMeshAgent;
            Vector3 ownerPos = controller.owner.transform.position;
            if (Time.time - lastTime > 1)//每1秒变更一次逃跑目标位置
            {
                Vector3 escapeVer=controller.owner.transform.forward;
                for (int i = 0; i < BattleUnitBase.enemyUnits.Count; i++)
                {
                    Vector3 enemyPos = BattleUnitBase.enemyUnits[i].transform.position;
                    if (Vector3.Distance(ownerPos, enemyPos) <=
                        controller.owner.prop.viewDistance)
                    {
                        escapeVer += ownerPos - enemyPos;
                    }
                }
                //设置逃跑位置
                NavMeshHit hit;
                if (NavMesh.SamplePosition(ownerPos + controller.owner.transform.forward * 2, out hit, 4f,NavMesh.AllAreas))
                {
                    navMeshAgent.SetDestination(hit.position);
                }
                //如果无法走，则不动
            }
        }
    }
