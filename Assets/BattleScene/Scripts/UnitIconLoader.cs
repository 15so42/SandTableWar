
    using UnityEngine;

    public class UnitIconLoader
    {
        public const string spritePathPrefix = "Sprite/";
        public static Sprite GetSpriteByUnitId(BattleUnitId id)
        {
            SpawnBattleUnitConfigInfo curUnitInfo = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(id);
            if (curUnitInfo == null)
            {
                return null;
            }
            return Resources.Load<Sprite>(spritePathPrefix+curUnitInfo.resourceName);
        }
    }
    
    public class BuildingIconLoader
    {
        public const string spritePathPrefix = "Sprite/Building/";
        public static Sprite GetSpriteByUnitId(BattleUnitId id)
        {
            SpawnBattleUnitConfigInfo curUnitInfo = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(id);
            if (curUnitInfo == null)
            {
                return null;
            }
            return Resources.Load<Sprite>(spritePathPrefix+curUnitInfo.resourceName);
        }
    }
