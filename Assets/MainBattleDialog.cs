using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainBattleDialogContext : DialogContext
{
    
}
public class MainBattleDialog : Dialog<MainBattleDialogContext>
{
    private GameManager gameManager;
    private FightingManager fightingManager;
    private BattleResMgr battleResMgr;
    
    [Header("资源面板")]
    public Text populationText;
    public Text populationIncreaseRateText;
    public Text coinText;
    public Text coinIncreaseRateText;
    public Text mineralText;
    public Text mineralIncreaseRateText;
    public Text foodText;
    public Text foodIncreaseRateText;

    [Header("GlobalItemManager")] public GlobalItemManager globalItemManager;
    
    public static Dialog ShowDialog()
    {
        var dialog = GetShowingDialog(nameof(MainBattleDialog)) as MainBattleDialog;
        if (dialog != null)
        {
            return null;
        }

        return DialogUtil.ShowDialogWithContext(nameof(MainBattleDialog), new MainBattleDialogContext());
    }

    public override void Show()
    {
        base.Show();
        gameManager=GameManager.Instance;
        fightingManager = gameManager.GetFightingManager();
        battleResMgr = fightingManager.battleResMgr;
        globalItemManager.Init();
    }

    private void Update()
    {
        UpdateResPanel();
        globalItemManager.UpdateManager();
    }

    private void UpdateResPanel()
    {
        populationText.text = battleResMgr.GetRemainingResByResType(BattleResType.population).ToString();
        int populationIncrease = battleResMgr.battleResIncreaseRate[BattleResType.population];
        populationIncreaseRateText.text = $"{(populationIncrease > 0 ? "+" : "-")}{populationIncrease}/s";
        coinText.text = battleResMgr.GetRemainingResByResType(BattleResType.coin).ToString();
        int coinIncrease = battleResMgr.battleResIncreaseRate[BattleResType.coin];
        coinIncreaseRateText.text = $"{(coinIncrease > 0 ? "+" : "-")}{coinIncrease}/s";
        mineralText.text = battleResMgr.GetRemainingResByResType(BattleResType.mineral).ToString();
        int mineralIncrease = battleResMgr.battleResIncreaseRate[BattleResType.mineral];
        mineralIncreaseRateText.text = $"{(mineralIncrease > 0 ? "+" : "-")}{mineralIncrease}/s";
        foodText.text = battleResMgr.GetRemainingResByResType(BattleResType.food).ToString();
        int foodIncrease = battleResMgr.battleResIncreaseRate[BattleResType.food];
        foodIncreaseRateText.text = $"{(foodIncrease > 0 ? "+" : "-")}{foodIncrease}/s";
    }

    #region 全局道具

    

    #endregion
}
