using System;
using UnityEngine;

public static class DialogUtil
{
    public static Dialog ShowDialog(string dialogName)
    {
        Dialog dialog = CreateDialog(dialogName);
        if (null != dialog)
        {
            dialog.Show();
        }
        
        return dialog;
    }

    public static Dialog ShowDialogWithContext<T>(string dialogName, T context, bool isHideBanner = true) where T : DialogContext
    {
        var dialog = CreateDialog(dialogName) as Dialog<T>;
        if (null != dialog)
        {
            dialog.ShowWithContext(context);
        }
        
        return dialog;
    }

    public static Dialog ShowDialogWithContext<T>(string dialogName, T context, Action onComplete,Action closeStateAction, bool isHideBanner = true) where T : DialogContext
    {
        var dialog = CreateDialog(dialogName) as Dialog<T>;
        if (null != dialog)
        {
            dialog.ShowWithContext(context,onComplete,closeStateAction);
        }

        return dialog;
    }

    public static Dialog CreateDialog(string dialogName)
    {
        if (string.IsNullOrEmpty(dialogName))
        {
            return null;
        }

        var dialogPrefab = Resources.Load<Dialog>("Prefab/Dialogs/" + dialogName);
        //Debug.Log("生成Dialog"+dialogName);
        if (dialogPrefab != null)
        {
            var dialog = GameObject.Instantiate<Dialog>(dialogPrefab);
            return dialog;
        }

        return null;
    }

    public static void PreLoad(string dialogName)
    {
        Resources.LoadAsync<Dialog>("Dialogs/" + dialogName);
    }
}