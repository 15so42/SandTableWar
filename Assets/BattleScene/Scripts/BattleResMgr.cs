
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BattleResType
{
    Coin, //金币
    Mineral, //矿物
    Food, //食物
    Wood,//木材
}

    /// <summary>
    /// 控制游戏局内资源，如人口，金币，矿物，食物等
    /// </summary>
    public class BattleResMgr
    {
        [System.Serializable]
        public struct ResourceInput
        {
            public BattleResType resType;
            public int Amount;
        }

        [System.Serializable]
        public struct ResourceInputRange
        {
            public BattleResType resType;
            public IntRange Amount;
        }
        
        private int lastTime;
        public Dictionary<BattleResType,float> battleResHolder=new Dictionary<BattleResType, float>()
        {
            {BattleResType.Coin,10},
            {BattleResType.Mineral,10},
            {BattleResType.Food,10},
            {BattleResType.Wood,10},
        };
        /// <summary>
        /// 增长速率
        /// </summary>
        public Dictionary<BattleResType,float> battleResIncreaseRate=new Dictionary<BattleResType, float>()
        {
            {BattleResType.Coin,0},
            {BattleResType.Mineral,0},
            {BattleResType.Food,0},
            {BattleResType.Wood,0},
        };
        /// <summary>
        /// 消耗资源，返回false表示资源不足，否则表示消耗成功
        /// </summary>
        /// <param name="resType"></param>
        /// <returns></returns>
        public bool ConsumeRes(BattleResType resType,int value)
        {
            if (HasEnoughRes(resType, value) == false)
            {
                return false;
            }

            battleResHolder[resType] -= value;
            return true;
        }

        /// <summary>
        /// 是否有足够资源
        /// </summary>
        /// <param name="resType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool HasEnoughRes(BattleResType resType, int needRes)
        {
            return GetRemainingResByResType(resType) >= needRes;
        }

        public bool HasRequiredResources(ResourceInput[] resourceInputs)
        {
            for (int i = 0; i < resourceInputs.Length; i++)
            {
                if (!HasEnoughRes(resourceInputs[i].resType, resourceInputs[i].Amount))
                {
                    return false;
                }
            }

            return true;
        }

        public void UpdateRequiredResources(ResourceInput[] resourceInputs,bool add)
        {
            for (int i = 0; i < resourceInputs.Length; i++)
            {
                UpdateResource(resourceInputs[i].resType,resourceInputs[i].Amount,add);
            }
        }

        public float GetRemainingResByResType(BattleResType resType)
        {
            if (!battleResHolder.ContainsKey(resType))
            {
                return 0;
            }

            if (battleResHolder.TryGetValue(resType, out var value))
            {
                return value;
            }

            return 0;
        }

        public bool ConsumeResByUnitInfo(SpawnBattleUnitConfigInfo spawnInfo)
        {
           ResourceInput[] resourceInputs = spawnInfo.requiredResource;

            if (!HasRequiredResources(resourceInputs))
            {
                return false;
            }

            foreach (var resourceInput in resourceInputs)
            {
                ConsumeRes(resourceInput.resType, resourceInput.Amount);
            }

            return true;
        }

       
        /// <summary>
        ///     增加增长率
        /// </summary>
        public void UpdateIncreaseRate(BattleResType type, float num,bool add)
        {
            battleResIncreaseRate[type] += ( add ? 1 :-1 ) * num;
        }
        
        public void UpdateResource(BattleResType type, float num,bool add)
        {
            battleResHolder[type] += ( add ? 1 :-1 )* num;
        }
        
      
        
       
        
        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            if (Time.time >= lastTime + 1)
            {
                //自动增加
                // battleResHolder[BattleResType.Coin]+=battleResIncreaseRate[BattleResType.Coin];
                // battleResHolder[BattleResType.Mineral]+=battleResIncreaseRate[BattleResType.Mineral];
                // battleResHolder[BattleResType.Food]+=battleResIncreaseRate[BattleResType.Food];
                foreach (var increaseRate in battleResIncreaseRate)
                {
                    UpdateResource(increaseRate.Key,increaseRate.Value,true);
                }
                lastTime = (int)Time.time;
            }
        }
            
    }
