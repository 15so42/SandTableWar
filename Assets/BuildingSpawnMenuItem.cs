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
    private FactionEntityTask factionEntityTask;
    
    public Image spawnUnitImage;
    public Image extraImage;
    public Image fill;
    public Text amountText;
    public Text nameText;

    public void SetParams(FactionEntityTask factionEntityTask,BaseBattleBuilding targetBuilding)
    {
        this.factionEntityTask = factionEntityTask;
        this.spawnId = factionEntityTask.battleUnitId;
        
        this.targetBuilding = targetBuilding;
    }
    public override void Init()
    {
        
        curUnitInfo = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(spawnId);
        spawnUnitImage.sprite = UnitIconLoader.GetSpriteByUnitId(curUnitInfo.battleUnitId);
        if (curUnitInfo.hasExtraIcon)
        {
            Sprite sprite=UnitIconLoader.GetExtraIconSpriteByUnitId(curUnitInfo.battleUnitId);
            if (sprite != null)
            {
                extraImage.sprite=sprite;
            }
            else
            {
                extraImage.gameObject.SetActive(false);
            }
        }
        else
        {
            extraImage.gameObject.SetActive(false);
        }
        fill.fillAmount = 0;
        amountText.text = "0";
        nameText.text = curUnitInfo.battleUnitName;
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
