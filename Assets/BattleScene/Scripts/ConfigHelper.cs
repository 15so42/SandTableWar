
    using BattleScene.Scripts.AI;
    using UnityEngine;

    public class ConfigHelper:Singleton<ConfigHelper>
    {
        private bool Inited = false;
        private string spawnSoliderConfigPath = "Config/ScriptableObject/SpawnBattleUnitConfig";
        private string battleFxConfigPath = "Config/ScriptableObject/BattleFxConfig";
        private string lineConfigPath = "Config/ScriptableObject/LineConfig";
        private string levelConfigPath = "Config/ScriptableObject/";
        private string npcUnitRegulatorDataConfigPath = "Config/ScriptableObject/NpcUnitRegulatorData/NpcRegulatorDataConfig";
       
        
        
        private SpawnBattleUnitConfig spawnBattleUnitUnitConfig;
        private BattleFxConfig battleFxConfig;
        private LineConfig lineConfig;
        private NpcRegulatorDataConfig npcRegulatorDataConfig;
        
        public ConfigHelper()
        {
            //单例模板通过无参构造函数生成单例，初始化则需要在这里初始化
            Init();
        }

        private void Init()
        {
            spawnBattleUnitUnitConfig = Resources.Load<SpawnBattleUnitConfig>(spawnSoliderConfigPath);
            battleFxConfig=Resources.Load<BattleFxConfig>(battleFxConfigPath);
            lineConfig = Resources.Load<LineConfig>(lineConfigPath);
            npcRegulatorDataConfig = Resources.Load<NpcRegulatorDataConfig>(npcUnitRegulatorDataConfigPath);

        }

        public SpawnBattleUnitConfigInfo GetSpawnBattleUnitConfigInfoByUnitId(BattleUnitId battleUnitId)
        {
            if (spawnBattleUnitUnitConfig == null)
                return null;
            return spawnBattleUnitUnitConfig.config.Find(x => x.battleUnitId == battleUnitId);
        }

        public GameObject GetFxPfbByBattleFxType(BattleFxType battleFxType)
        {
            if (battleFxConfig == null)
                return null;
            return battleFxConfig.GetFxPfbByBattleFxType(battleFxType);
        }
        
        public GameObject GetLinePfbByLineMode(LineMode lineMode)
        {
            if (lineConfig == null)
                return null;
            return lineConfig.GetLinePfbByLineMode(lineMode);
        }
        
        public LevelConfig GetLevelConfig(int level)
        {
            LevelConfig levelConfig = Resources.Load<LevelConfig>($"{levelConfigPath}Level_{level}");
            return levelConfig;
        }

        public NpcRegulatorData GetNpcRegulatorData(BattleUnitId battleUnitId, NpcDifficulty npcDifficulty,bool building=false)
        {
            return npcRegulatorDataConfig.Filter(battleUnitId, npcDifficulty,building).Clone() as NpcRegulatorData;
        }
       
    }
