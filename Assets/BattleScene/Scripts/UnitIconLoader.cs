
    using UnityEngine;

    public class UnitIconLoader
    {
        public const string spritePathPrefix = "Sprite/";
        public static Sprite GetSpriteByUnitId(int id)
        {
            SpawnBattleUnitConfigInfo curUnitInfo = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoById(id);
            if (curUnitInfo == null)
            {
                return null;
            }
            return Resources.Load<Sprite>(spritePathPrefix+curUnitInfo.resourceName);
        }
    }
