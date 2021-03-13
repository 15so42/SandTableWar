using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSpawnMenuItem : BuildingMenuItem
{
    [HideInInspector]public BaseBattleBuilding targetBuilding;
    [HideInInspector]public BattleUnitId spawnId;
    private SpawnBattleUnitConfigInfo curUnitInfo;
    
    public Image spawnUnitImage;
    public Image fill;
    public Text amountText;

    public void SetParams(BattleUnitId spawnId,BaseBattleBuilding targetBuilding)
    {
        this.spawnId = spawnId;
        this.targetBuilding = targetBuilding;
    }
    public override void Init()
    {
        curUnitInfo = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(spawnId);
        spawnUnitImage.sprite = UnitIconLoader.GetSpriteByUnitId(curUnitInfo.battleUnitId);
        fill.fillAmount = 0;
        amountText.text = "0";
    }

    public override void Update()
    {
        amountText.text = GetAmountByIdInSpawnStack(spawnId)+"";
        if (targetBuilding.toSpawn.Count>0 && spawnId == targetBuilding.toSpawn.Peek())
        {
            fill.fillAmount = targetBuilding.GetSpawnRatio();
        }
    }

    private int GetAmountByIdInSpawnStack(BattleUnitId id)
    {
        int count = 0;
        for (int i = 0; i < targetBuilding.toSpawn.Count; i++)
        {
            if (targetBuilding.toSpawn.ElementAt(i) == id)
            {
                count++;
            }
        }

        return count;
    }
}
