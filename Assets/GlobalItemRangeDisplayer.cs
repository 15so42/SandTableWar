using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;

public class GlobalItemRangeDisplayer : MonoBehaviour
{
    public GameObject circlePfb;
    private Outlinable outlinable;
    private Color circleColor=Color.cyan;

    private List<GameObject> circles=new List<GameObject>();
    private List<BattleUnitBase> battleUnitBases=new List<BattleUnitBase>();

    public static GlobalItemRangeDisplayer Instance;
    
    private void Awake()
    {
        Instance = this;
        outlinable = GetComponent<Outlinable>();
    }

    public void Start()
    {
        SetList(BattleUnitBase.selfUnits);
    }

    private int lastFrameUnitCount=0;
    // public void Display()
    // {
    //     outlinable.OutlineParameters.Enabled = true;
    //     BattleUnitBase unitBase;
    //     for (int i = 0; i < BattleUnitBase.selfUnits.Count; i++)
    //     {
    //         unitBase = BattleUnitBase.selfUnits[i];
    //
    //         if (unitBase == null)
    //         {
    //             continue;
    //         }
    //             
    //         
    //         if (circles.Count <= i)
    //         {
    //            circles.Add(GameObject.Instantiate(circlePfb,transform));
    //         }
    //         
    //         GameObject circle = circles[i];
    //         circle.transform.position = unitBase.transform.position;
    //         if (unitBase.IsAlive())
    //         {
    //             circle.gameObject.SetActive(true);
    //         }
    //         else
    //         {
    //             circle.gameObject.SetActive(false);
    //         }
    //         float size = unitBase.prop.viewDistance*2;
    //         circle.transform.localScale=new Vector3(size,0.1f,size);
    //     }
    //
    //     while (circles.Count > BattleUnitBase.selfUnits.Count)//删除多余视野
    //     {
    //         Destroy(circles[circles.Count-1]);
    //         circles.RemoveAt(circles.Count - 1);
    //     }
    //     if (lastFrameUnitCount != BattleUnitBase.selfUnits.Count)
    //     {
    //         Debug.Log("重构范围显示");
    //         outlinable.AddAllChildRenderersToRenderingList();
    //         lastFrameUnitCount = BattleUnitBase.selfUnits.Count;
    //     }
    //     
    // }
    
    public void Display(List<BattleUnitBase> targetUnits)
    {
        outlinable.OutlineParameters.Enabled = true;
        outlinable.OutlineParameters.Color = circleColor;
        BattleUnitBase unitBase;
        for (int i = 0; i < targetUnits.Count; i++)
        {
            unitBase = targetUnits[i];

            if (unitBase == null)
            {
                continue;
            }
                
            
            if (circles.Count <= i)
            {
                circles.Add(GameObject.Instantiate(circlePfb,transform));
            }
            
            GameObject circle = circles[i];
            circle.transform.position = unitBase.transform.position;
            if (unitBase.IsAlive())
            {
                circle.gameObject.SetActive(true);
            }
            else
            {
                circle.gameObject.SetActive(false);
            }
            float size = unitBase.prop.viewDistance*2;
            circle.transform.localScale=new Vector3(size,0.1f,size);
        }

        while (circles.Count > targetUnits.Count)//删除多余视野
        {
            Destroy(circles[circles.Count-1]);
            circles.RemoveAt(circles.Count - 1);
        }
        if (lastFrameUnitCount != targetUnits.Count)
        {
            Debug.Log("重构范围显示");
            outlinable.AddAllChildRenderersToRenderingList();
            lastFrameUnitCount = BattleUnitBase.selfUnits.Count;
        }
        
    }
    

    public void OnSelfUnitCreate()
    {
        
    }
    private void Update()
    {
        Display(battleUnitBases);
    }

    public void SetList(List<BattleUnitBase> battleUnitBases)
    {
        this.battleUnitBases = battleUnitBases;
    }

    public void SetColor(Color targetColor)
    {
        circleColor = targetColor;
    }

    public void SetDefaultColor()
    {
        circleColor=Color.cyan;
    }

    public void SetDefaultList()
    {
        SetList(BattleUnitBase.selfUnits);
    }

    public void Hide()
    {
        outlinable.OutlineParameters.Enabled = false;
    }
}
