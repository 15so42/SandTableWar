
    using UnityEngine;

    public class ConfigHelper:Singleton<ConfigHelper>
    {
        private bool Inited = false;
        private string spawnSoliderConfigPath = "Config/ScriptableObject/SpawnBattleUnitConfig";
        
        private SpawnBattleUnitConfig spawnBattleUnitUnitConfig;
        public ConfigHelper()
        {
            //单例模板通过无参构造函数生成单例，初始化则需要在这里初始化
            Init();
        }

        private void Init()
        {
            spawnBattleUnitUnitConfig = Resources.Load<SpawnBattleUnitConfig>(spawnSoliderConfigPath);
            
        }

        public SpawnBattleUnitConfigInfo GetSpawnBattleUnitConfigInfoById(int id)
        {
            if (spawnBattleUnitUnitConfig == null)
                return null;
            return spawnBattleUnitUnitConfig.config.Find(x => x.id == id);
        }
        
       
    }
