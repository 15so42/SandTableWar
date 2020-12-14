
    using UnityEngine;

    public class FindEnemyDecision :Decision
    {
        public float timer;
        public float interval = 1;//間隔
        public override bool Decide(StateController controller)
        {
            if (controller.enemy!=null)
            {
                //判断是否还在视野范围内
                if (Vector3.Distance(controller.enemy.transform.position, controller.owner.transform.position) <=
                    controller.owner.prop.viewDistance)
                {
                    Debug.Log($"[{nameof(FindEnemyDecision)}]本来就有敌人且在视野范围内");
                    return true;
                }
            }
            
            //每一秒投射一次球形碰撞體來尋找敵人
            timer += Time.deltaTime;
            if (timer >  interval)
            {
                timer = 0;
                BattleUnitBase enemy=FindEnemy(controller);
                if (enemy)
                {
                    controller.SetEnemy(enemy);
                    Debug.Log($"[{nameof(FindEnemyDecision)}]碰撞找到敌人");
                    return true;
                }
            }
            Debug.Log($"[{nameof(FindEnemyDecision)}]碰撞没有找到敌人");
            return false;
        }

        public BattleUnitBase FindEnemy(StateController controller)
        {
            //todo 檢測範圍需要修改
            Collider[] colliders = Physics.OverlapSphere(controller.owner.transform.position, controller.owner.prop.viewDistance);
            foreach (var collider in colliders)
            {
                BattleUnitBase unitBase = collider.GetComponent<BattleUnitBase>();
                if (unitBase==null)
                {
                    continue;
                }
                //todo 區分敵我的方式在不同模式中還需完善,如2v2模式不能使用這種方式區分
                if (unitBase.campId != controller.owner.campId)
                {
                    return unitBase;
                }
            }
           
            return null;
        }
    }
