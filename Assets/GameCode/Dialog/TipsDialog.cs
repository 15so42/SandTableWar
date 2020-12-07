using System;
using UnityEngine;
using UnityEngine.UI;
using UnityTimer;

public class TipsDialogContext : DialogContext
{
    public string tipsContext;
    public bool withBg;
    public bool isWarning;
    public int offsetY;

    public TipsDialogContext(string tipsContext, bool withBg, bool isWarning, int offsetY = 0)
    {
        this.tipsContext = tipsContext;
        this.withBg = withBg;
        this.isWarning = isWarning;
        this.offsetY = offsetY;
    }
}

public class TipsDialog : Dialog<TipsDialogContext>
{
    [SerializeField] private Text tipsTxt;
    [SerializeField] private Text withoutBgTxt;

    public static void ShowDialog(string tips, bool withBg = true, bool isWarning = false, int offsetY = 0)
    {
        var dialog = GetShowingDialog(nameof(TipsDialog)) as TipsDialog;
        if (dialog != null && (dialog && dialog.tipsTxt.text == tips))
        {
            return;
        }

        DialogUtil.ShowDialogWithContext(nameof(TipsDialog), new TipsDialogContext(tips, withBg, isWarning, offsetY));
    }

    public override void Show()
    {
        base.Show();
        frameImage.gameObject.SetActive(dialogContext.withBg);
        withoutBgTxt.gameObject.SetActive(!dialogContext.withBg);
        withoutBgTxt.text = dialogContext.tipsContext;
        withoutBgTxt.color = dialogContext.isWarning ? Color.red : Color.white;
        tipsTxt.text = dialogContext.tipsContext;
        frameImage.transform.localPosition = new Vector2(0,dialogContext.offsetY);
        Timer.Register(2f, () => { Close(); });
    }

    protected override void OpenSound()
    {
    }
}