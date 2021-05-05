
    using System;
    using System.Collections.Generic;
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using RTSEngine;
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
        private List<BattleUnitBase> selfUnits;
        private List<BattleUnitBase> enemyUnitsInMyView;

        private Func<BattleUnitBase, ErrorMessage> isTargetValid;
        //public bool ignoreHeight=true;
        //todo 添加防空检测
        public override void OnStart()
        {
            base.OnStart();
            prop = selfBattleUnit.Value.prop;
            enemyUnitsInMyView = BattleUnitBase.enemyUnitsInMyView;
            selfUnits = BattleUnitBase.selfUnits; 
            isTargetValid = selfBattleUnit.Value.IsTargetValid;
        }

        public override TaskStatus OnUpdate()
        {
            if (enemyBattleUnit.Value!=null)
            {
                //判断是否还在视野范围内
                if (Vector3.Distance(enemyBattleUnit.Value.transform.position, transform.position) <=
                    prop.viewDistance*1.1f && enemyBattleUnit.Value.IsAlive())//1.03f是为了解决正巧在极限距离外一点点导致判断为false的问题
                {
                    //Debug.Log($"[{nameof(FindEnemyDecision)}]本来就有敌人且在视野范围内");
                    return TaskStatus.Success;
                }
                targetGroup.Value.Remove(enemyBattleUnit.Value.gameObject);
                enemyBattleUnit.Value = null;
                
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
            DiplomaticRelation diplomaticRelation =
                EnemyIdentifier.Instance.GetMyDiplomaticRelation(selfBattleUnit.Value.factionId);
            if (diplomaticRelation == DiplomaticRelation.Self)//本机自己
            {
                // for (int i = 0; i < enemyUnitsInMyView.Count; i++)
                // {
                //     if (Vector3.Distance(enemyUnitsInMyView[i].transform.position, transform.position) <
                //         prop.viewDistance && enemyUnitsInMyView[i].IsAlive())
                //     {
                //         return enemyUnitsInMyView[i];
                //     }
                // }
                if (GridSearchHandler.Instance.Search(transform.position, prop.viewDistance,false, isTargetValid,
                    out var target)==ErrorMessage.none)
                {
                    return target;
                }
            }else if (diplomaticRelation == DiplomaticRelation.Enemy)//本机上的敌人
            {
                ErrorMessage errorMessage = GridSearchHandler.Instance.Search(transform.position, prop.viewDistance,
                    false,isTargetValid,
                    out var target);
                if (errorMessage==ErrorMessage.none)
                {
                    return target;
                }
            }

            return null;
        }

        private void SetEnemy(BattleUnitBase value)
        {
            enemyBattleUnit.Value = value;
            var o = value.gameObject;
            returnedEnemy.Value = o;
            if(!targetGroup.Value.Contains(o))
                targetGroup.Value.Add(o);
        }
    }
