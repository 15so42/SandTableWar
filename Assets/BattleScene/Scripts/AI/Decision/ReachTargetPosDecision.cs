
    using UnityEngine;

    public class ReachTargetPosDecision:Decision
    {
        public override bool Decide(StateController controller)
        {
            
            float distance = GetDistanceIgnoreY(controller.owner.transform.position, controller.targetPos);
            //一定要忽略y轴对距离的计算
            
            bool result = distance < 0.5f;
            //Debug.Log($"[{nameof(ReachTargetPosDecision)}]是否到达目标地点：{result}");
            return result;//todo 更详细的判断
        }

        public float GetDistanceIgnoreY(Vector3 pointA,Vector3 pointB)
        {
            return Mathf.Sqrt(Mathf.Pow((pointB.x - pointA.x), 2) + Mathf.Pow((pointB.z - pointA.z), 2));
        }
    }
