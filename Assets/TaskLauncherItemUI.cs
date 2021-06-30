using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskLauncherItemUI : MonoBehaviour
{

    private TaskLauncher taskLauncher;
    public FactionEntityTask factionEntityTask;

    public Image spawnUnitImage;
    public Image extraImage;
    public Image fill;
    public Text amountText;
    public Text nameText;

    private int queueId;
    

    public void Init(TaskLauncher taskLauncher1,FactionEntityTask factionEntityTask1,int queueId)
    {
        this.taskLauncher = taskLauncher1;
        this.factionEntityTask = factionEntityTask1;
        this.queueId = queueId;
        
        
        if (factionEntityTask.type == TaskManager.TaskTypes.createUnit)
        {
            var unitId = factionEntityTask.battleUnitId;
            var curUnitInfo = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(unitId);
            spawnUnitImage.sprite = UnitIconLoader.GetSpriteByUnitId(unitId);
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
            amountText.text = "";
            nameText.text = curUnitInfo.battleUnitName;
        }
    }
    
   
    public virtual void UpdateItemUi()
    {
        if(factionEntityTask==null)
            return;
        
        if (transform.GetSiblingIndex()==0)
        {
            fill.fillAmount = taskLauncher.GetProgress();
        }
        
    }
  
}
