
    using System.Collections.Generic;
    using UnityEngine;

    public class ResourceBuilding : BaseBattleBuilding
    {
        [Header("资源设置")]
        public List<ResIncreaseRateConfig> resIncreaseRateConfigs=new List<ResIncreaseRateConfig>();

        public override void OnBuildSuccess()
        {
            base.OnBuildSuccess();
            FightingManager fightingManager = GameManager.Instance.GetFightingManager();
            for (int i = 0; i < resIncreaseRateConfigs.Count; i++)
            {
               fightingManager.myBattleResMgr.UpdateIncreaseRate(resIncreaseRateConfigs[i].battleResType,resIncreaseRateConfigs[i].increaseSpeed,true);
                
            }
        }
    }

    [System.Serializable]
    public class ResIncreaseRateConfig
    {
        public BattleResType battleResType;
        public float increaseSpeed;
    }