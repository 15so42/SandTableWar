
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using UnityEngine;

    public class CanSeeEnemy :Conditional
    {
        public float timer;
        public float interval = 1;//間隔

        public SharedBattleUnit selfBattleUnit;
        public SharedBattleUnit enemyBattleUnit;
        public SharedGameObject returnedEnemy;
        public SharedGameObjectList targetGroup;

        private BattleUnitBaseProp prop;

        public override void OnStart()
        {
            base.OnStart();
            prop = selfBattleUnit.Value.prop;
        }

        public override TaskStatus OnUpdate()
        {
            if (enemyBattleUnit.Value!=null)
            {
                //判断是否还在视野范围内
                if (Vector3.Distance(enemyBattleUnit.Value.transform.position, transform.position) <=
                    prop.viewDistance)
                {
                    //Debug.Log($"[{nameof(FindEnemyDecision)}]本来就有敌人且在视野范围内");
                    return TaskStatus.Success;
                }
            }
            
            //每一秒投射一次球形碰撞體來尋找敵人
            timer += Time.deltaTime;
            if (timer >  interval)
            {
                timer = 0;
                BattleUnitBase enemy=FindEnemy();
                if (enemy)
                {
                    SetEnemy(enemy);
                    //Debug.Log($"[{nameof(FindEnemyDecision)}]碰撞找到敌人");
                    return TaskStatus.Success;
                }
            }
            //Debug.Log($"[{nameof(FindEnemyDecision)}]碰撞没有找到敌人");
            return TaskStatus.Failure;
        }

        public BattleUnitBase FindEnemy()
        {
            //todo 檢測範圍需要修改
            Collider[] colliders = Physics.OverlapSphere(transform.position, prop.viewDistance);
            foreach (var collider in colliders)
            {
                BattleUnitBase unitBase = collider.GetComponent<BattleUnitBase>();
                if (unitBase==null)
                {
                    continue;
                }
                //todo 區分敵我的方式在不同模式中還需完善,如2v2模式不能使用這種方式區分
                if (unitBase.campId != selfBattleUnit.Value.campId && unitBase.campId!=-1)
                {
                    return unitBase;
                }
            }
           
            return null;
        }

        private void SetEnemy(BattleUnitBase value)
        {
            enemyBattleUnit.Value = value;
            var o = value.gameObject;
            returnedEnemy.Value = o;
            targetGroup.Value.Add(o);
        }
    }
