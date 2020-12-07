    using System;
    using UnityEngine;
    using UnityEngine.Timeline;

    public class Weapon : MonoBehaviour
    {
        public float atkValue=1;//攻擊力
        public float atkRate=1;//射速,幾次/秒

        //記時
        private float lastAtkTime = 0;
        private float atkTimer = 0;
        
        //近战时直接使用此伤害计算
        protected int baseDamage=30;
        public BattleUnitBase owner;

        //一般由狀態機每幀執行此方法，對應類需要自己計時
        public virtual void WeaponUpdate()
        {
            atkTimer += Time.deltaTime;
            if (atkTimer > lastAtkTime + (float) 1 / atkRate)
            {
                atkTimer = 0;
                Attack();
            }
        }

        public virtual void Attack()
        {
            
        }

        public void SetOwner(BattleUnitBase unitBase)
        {
            this.owner = unitBase;
        }
    }
