
using System.Collections.Generic;
using UnityEngine;

public enum BattleResType
{
    population, //人口
    coin, //金币
    mineral, //矿物
    food //食物
}

    /// <summary>
    /// 控制游戏局内资源，如人口，金币，矿物，食物等
    /// </summary>
    public class BattleResMgr
    {
        private int lastTime;
        public Dictionary<BattleResType,int> battleResHolder=new Dictionary<BattleResType, int>()
        {
            {BattleResType.population,10},
            {BattleResType.coin,10},
            {BattleResType.mineral,10},
            {BattleResType.food,10},
        };
        /// <summary>
        /// 增长速率
        /// </summary>
        public Dictionary<BattleResType,int> battleResIncreaseRate=new Dictionary<BattleResType, int>()
        {
            {BattleResType.population,1},
            {BattleResType.coin,1},
            {BattleResType.mineral,1},
            {BattleResType.food,1},
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

        public int GetRemainingResByResType(BattleResType resType)
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
            if (HasEnoughRes(BattleResType.population, spawnInfo.needPopulation) &&
                HasEnoughRes(BattleResType.coin, spawnInfo.needCoin) &&
                HasEnoughRes(BattleResType.mineral, spawnInfo.needMineral) &&
                HasEnoughRes(BattleResType.food, spawnInfo.needFood))
            {
                ConsumeRes(BattleResType.population, spawnInfo.needPopulation);
                ConsumeRes(BattleResType.coin, spawnInfo.needCoin);
                ConsumeRes(BattleResType.mineral, spawnInfo.needMineral);
                ConsumeRes(BattleResType.food, spawnInfo.needFood);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 单词得到资源
        /// </summary>
        /// <param name="type"></param>
        /// <param name="num"></param>
        public void EarnRes(BattleResType type, int num)
        {
            battleResHolder[type] += num;
        }

        /// <summary>
        ///     增加增长率
        /// </summary>
        public void AddIncreaseRate(BattleResType type, int num)
        {
            battleResIncreaseRate[type] += num;
        }
        
        public void ReduceIncreaseRate(BattleResType type, int num)
        {
            battleResIncreaseRate[type] -= num;
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            if (Time.time >= lastTime + 1)
            {
                //自动增加
                battleResHolder[BattleResType.population]+=battleResIncreaseRate[BattleResType.population];
                battleResHolder[BattleResType.coin]+=battleResIncreaseRate[BattleResType.coin];
                battleResHolder[BattleResType.mineral]+=battleResIncreaseRate[BattleResType.mineral];
                battleResHolder[BattleResType.food]+=battleResIncreaseRate[BattleResType.food];
                lastTime = (int)Time.time;
            }
        }
            
    }
