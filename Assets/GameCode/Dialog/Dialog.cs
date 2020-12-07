
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Timer = UnityTimer.Timer;

public class DialogContext
{
    public string From { get; protected set; }

    public DialogContext(string from = null)
    {
        From = from;
    }
}

public class Dialog : MonoBehaviour
{
    protected static List<Dialog> dialogs = new List<Dialog>();
    private static UnityEvent dialogCloseCallBack = new UnityEvent();

    [SerializeField] public Image background;
    [SerializeField] public Transform frameImage;
    protected Sequence mainTween;
    protected bool isAnimationTranslate;

    protected bool isShowing = true;
    protected bool hasAnim = true;


    Action closeStateAction;
    /// <summary>
    /// Try to close the top dialog
    /// </summary>
    /// <returns>true if there're some dialogs showing, even the top dialog is not closable</returns>
    public static void AppendCloseCallBack(UnityAction callBack)
    {
        if (dialogs.Count > 0)
        {
            dialogCloseCallBack.AddListener(callBack);
        }
        else
        {
            callBack?.Invoke();
        }
    }


    public static bool CheckDialogShowing(string dialogName)
    {
        return dialogs.Any(x => x != null && x.gameObject.name.Contains(dialogName));
    }

    public static Dialog GetShowingDialog(string dialogName)
    {
        return dialogs.ToList().Find(m => m != null && m.gameObject.name.Contains(dialogName));
    }

    public static bool HasDialogShowing(params string[] ig)
    {
        if (dialogs == null || dialogs.Count == 0)
        {
            return false;
        }

        foreach (var dialog in dialogs)
        {
            var value = dialog.GetType().Name;
            if(ig.Contains(value))
                continue;
            if(value == nameof(TipsDialog))
                continue;
            if (dialog.isShowing)
                return true;
        }

        return false;
    }

    public static void CloseDialog(string dialogName)
    {
        List<Dialog> dialogList = new List<Dialog>();
        foreach (var item in dialogs)
        {
            if (item != null && item.name.Contains(dialogName))
            {
                dialogList.Add(item);
            }
        }

        for (var i = 0; i < dialogList.Count; i++)
        {
            dialogList[i].Close();
        }
    }

    public static void ClearAllDialogs()
    {
        dialogs.Clear();
    }

    public static void CloseAllDialogs(bool isAnim = true)
    {
        var arr = dialogs.ToArray();
        foreach (var dialog in arr)
        {
            if (dialog != null)
            {
                dialog.Close();
            }
        }

        ClearAllDialogs();
    }

    protected virtual void Awake()
    {
    }

    public virtual void Show()
    {
        isAnimationTranslate = true;
        Show(null);
        
    }

    public virtual void Show(Action onComplete)
    {
        Show(onComplete, 0);
      
    }
    public virtual void Show(Action onComplete=null,Action onClose=null)
    {
       
        if (onClose != null)
        {
            closeStateAction = onClose;
        }
        if (onComplete != null)
        {
            Show(onComplete, 0);
        }
        else
        {
            Show();
        }
    }

    public virtual void Show(Action onComplete, float delay)
    {
        //SceneMag.Instance.PreLoadScene();
        isShowing = true;
        Canvas canvas = GetComponent<Canvas>();
        //canvas.worldCamera = GameCamera.uiCam;

        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = GetTopSortingOrder();
        dialogs.Add(this);

        //OpenSound();

        mainTween = DOTween.Sequence();
        background.gameObject.SetActive(true);

        if(hasAnim)
            ShowAppearance();

        if (delay > 0)
        {
            Timer.Register(delay, () => { onComplete?.Invoke(); });
        }
        else
        {
            
            onComplete?.Invoke();
        }
      
    }
    public int GetTopSortingOrder()
    {
        int max = 10;
        for (int i = 0; i < dialogs.Count; i++)
        {
            if(dialogs[i]==null||dialogs[i].gameObject==null)
                continue;
            int curOrder = dialogs[i].GetComponent<Canvas>().sortingOrder;
            if (curOrder > max)
                max = curOrder ;
        }

        //大于20时是引导界面，加2是因为引导面板为20，高亮的是21，手指是20+2=22
        if (max >= 20)
            return max + 2;
        else
            return max+1;
    }

    public virtual void ShowAppearance()
    {
        ShowAppearance(Ease.OutQuart);
    }

    protected virtual void ShowAppearance(Ease ease, float overshoot = 0)
    {
        var viewCanvasGroup = GetComponent<CanvasGroup>();
        if (null == viewCanvasGroup)
        {
            viewCanvasGroup = frameImage.GetComponent<CanvasGroup>();
            if (null != viewCanvasGroup)
            {
                viewCanvasGroup.alpha = 0;
            }
        }

        frameImage.localScale = Vector3.forward;
        mainTween = DOTween.Sequence();
        mainTween.AppendInterval(SetShowDelay());
        mainTween.Append(frameImage.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
        if (null != viewCanvasGroup)
        {
            mainTween.Join(viewCanvasGroup.DOFade(1f, 0.15f));
        }

        mainTween.SetUpdate(true);
        mainTween.timeScale = 1f;
        mainTween.OnComplete(() => { isAnimationTranslate = false; });
    }

    protected virtual void FadeAppearance()
    {
        CanvasGroup cg = frameImage.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = frameImage.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0;
        }

        mainTween = DOTween.Sequence();
        mainTween.Append(cg.DOFade(1, 0.5f).SetEase(Ease.OutQuart));
        mainTween.OnComplete(() => { isAnimationTranslate = false; });
    }


    protected virtual float SetShowDelay()
    {
        return 0f;
    }

    protected virtual void OpenSound()
    {
        if (isOpenSoundOn)
        {
            //SoundHelper.DialogOpen();
        }
    }

    protected virtual void CloseSound()
    {
        if (isCloseSoundOn)
        {
            //SoundManager.PopupClose();
        }
    }

    
    public virtual void Close()
    {    
        //Cmd_CharactorStop.Instance.Call(true);
        Close(closeStateAction, true);
    }
    
    

    public virtual void Close(bool isCloseAnim = true)
    {
        Close(null, isCloseAnim);
    }

    public virtual void Close(Action onComplete, bool isCloseAnim = true)
    {
        if (isCloseAnim)
        {
            while (dialogs.Count > 0 && dialogs.Contains(this))
            {
                dialogs.Remove(this);
            }

            PlayCloseAnim(() =>
            {
                onComplete?.Invoke();
                InvokeCloseCallBack();

                if (gameObject)
                {
                    Destroy(gameObject);
                }
            });
        }
        else
        {
            while (dialogs.Count > 0 && dialogs.Contains(this))
            {
                dialogs.Remove(this);
            }

            onComplete?.Invoke();
            InvokeCloseCallBack();
            Destroy(gameObject);
        }

        CloseSound();
    }

    protected void InvokeCloseCallBack()
    {
        dialogs.RemoveAll(a => a == null);

        if (dialogs.Count == 0)
        {
            dialogCloseCallBack?.Invoke();
            dialogCloseCallBack.RemoveAllListeners();
        }
    }

    protected virtual bool HandleBackEvent()
    {
        return false;
    }

    protected virtual void PlayCloseAnim(Action onComplete)
    {
        PlayCloseAnim(onComplete, Ease.OutQuart);
    }

    protected virtual void PlayCloseAnim(Action onComplete, Ease ease, float overshoot = 0)
    {
        onComplete?.Invoke();
    }


    protected virtual bool isOpenSoundOn => true;
    protected virtual bool isCloseSoundOn => true;
    protected virtual float BgAlpha => 0.4f;
    protected virtual float AppearTime => 0.25f;

    protected void FadeIn()
    {
        background.DOFade(0.75f, 0.75f);
    }
}

public class Dialog<T> : Dialog where T : DialogContext
{
    public void ShowWithContext(T context,Action onComplete=null,Action closeStateAction=null)
    {
        SetContext(context);
        if (onComplete == null)
        {
            if(closeStateAction==null)
                Show();
            else
            {
                Show(null, closeStateAction);
            }
        }
        else
        {
            Show(onComplete, closeStateAction);
        }
       
    }

    protected void SetContext(T context)
    {
        dialogContext = context;
    }

    protected T dialogContext;
}