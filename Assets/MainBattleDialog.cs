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
    public Text coinText;
    public Text mineralText;
    public Text foodText;
    
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
    }

    private void Update()
    {
        UpdateResPanel();
    }

    private void UpdateResPanel()
    {
        populationText.text = battleResMgr.GetRemainingResByResType(BattleResType.population).ToString();
        coinText.text = battleResMgr.GetRemainingResByResType(BattleResType.coin).ToString();
        mineralText.text = battleResMgr.GetRemainingResByResType(BattleResType.mineral).ToString();
        foodText.text = battleResMgr.GetRemainingResByResType(BattleResType.food).ToString();
    }
}
