﻿using System;
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
    public Text woodText;
    public Text woodIncreaseRateText;

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
        battleResMgr = fightingManager.myBattleResMgr;
        globalItemManager.Init();
    }

    private void Update()
    {
        UpdateResPanel();
        globalItemManager.UpdateManager();
    }

    private void UpdateResPanel()
    {
        // populationText.text = battleResMgr.GetRemainingResByResType(BattleResType.population).ToString("#0");
        // float populationIncrease = battleResMgr.battleResIncreaseRate[BattleResType.population];
        // populationIncreaseRateText.text = $"{(populationIncrease > 0 ? "+" : "-")}{populationIncrease:#0.0}/s";
        coinText.text = battleResMgr.GetRemainingResByResType(BattleResType.Coin).ToString("#0");
        float coinIncrease = battleResMgr.battleResIncreaseRate[BattleResType.Coin];
        coinIncreaseRateText.text = $"{(coinIncrease > 0 ? "+" : "-")}{coinIncrease:#0.0}/s";
        mineralText.text = battleResMgr.GetRemainingResByResType(BattleResType.Mineral).ToString("#0");
        float mineralIncrease = battleResMgr.battleResIncreaseRate[BattleResType.Mineral];
        mineralIncreaseRateText.text = $"{(mineralIncrease > 0 ? "+" : "-")}{mineralIncrease:#0.0}/s";
        foodText.text = battleResMgr.GetRemainingResByResType(BattleResType.Food).ToString("#0");
        float foodIncrease = battleResMgr.battleResIncreaseRate[BattleResType.Food];
        foodIncreaseRateText.text = $"{(foodIncrease > 0 ? "+" : "-")}{foodIncrease:#0.0}/s";
        woodText.text = battleResMgr.GetRemainingResByResType(BattleResType.Wood).ToString("#0");
        float woodInCrease = battleResMgr.battleResIncreaseRate[BattleResType.Wood];
        woodIncreaseRateText.text = $"{(woodInCrease > 0 ? "+" : "-")}{woodInCrease:#0.0}/s";
    }

    #region 全局道具

    

    #endregion
}
