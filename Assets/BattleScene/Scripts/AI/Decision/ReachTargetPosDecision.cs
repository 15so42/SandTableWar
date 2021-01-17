
    using UnityEngine;

    public class ReachTargetPosDecision:Decision
    {
        public override bool Decide(StateController controller)
        {
            
            float distance = UnityTool.GetDistanceIgnoreY(controller.owner.transform.position, controller.targetPos);
            //一定要忽略y轴对距离的计算
            
            bool result = distance <= controller.navMeshAgent.stoppingDistance;
            //Debug.Log($"[{nameof(ReachTargetPosDecision)}]是否到达目标地点：{result}");
            return result;
        }

      
    }
