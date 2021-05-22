using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleFlyGraphicHandler : MonoBehaviour
{
    public static BattleFlyGraphicHandler Instance;
    private Camera mainCamera;

    [SerializeField]
    private GameObject textPfb;

    private Transform container;
    public void Init()
    {
        mainCamera = Camera.main;
        Transform canvasTrans = GameObject.Find("Canvas").transform;
        container =canvasTrans.Find("BattleFlyGraphicContainer");
        if (container == null)
        {
            GameObject tmpContainer=new GameObject("BattleFlyGraphicContainer");
            tmpContainer.transform.SetParent(canvasTrans);
            tmpContainer.transform.localPosition=Vector3.zero;
            container = tmpContainer.transform;
        }

        Instance = this;
    }

    public void FlyText(Vector3 pos,string str,float startHeight,float endHeight=1,float duration=1f)
    {
        RecycleAbleObject iTextGo = UnityObjectPoolManager.Allocate(textPfb.name);
        if (iTextGo==null)
        {
            iTextGo = GameObject.Instantiate(textPfb).GetComponent<RecycleAbleObject>();
            
        }
        //iTextGo.transform.SetParent(container);
        iTextGo.transform.position = Vector3.zero;
        TextMeshPro iTextTextComp=iTextGo.GetComponent<TextMeshPro>();
        iTextTextComp.text = str;
        iTextTextComp.DOFade(1, 0);
        
        
        Sequence sequence = DOTween.Sequence();
        
        //Vector3 startPos =  GetUiPos(pos) + Vector3.up * startHeight;
        Vector3 startPos =  pos + Vector3.up * startHeight;
        iTextGo.transform.position = startPos;
        
        sequence.Append(iTextGo.transform.DOJump(startPos + Vector3.up * endHeight, 1f,1,duration).OnUpdate(()=>
        {
            iTextGo.transform.forward=iTextGo.transform.position-mainCamera.transform.position;
        }));
        sequence.Join(iTextTextComp.DOFade(0, duration));
        sequence.OnComplete(() =>
        {
            iTextGo.Recycle();
        });

    }

    private Vector3 GetUiPos(Vector3 worldPos)
    {
        Vector3 resultPos = mainCamera.WorldToScreenPoint(worldPos);
        resultPos.z = 0;
        return resultPos;
    }
}
