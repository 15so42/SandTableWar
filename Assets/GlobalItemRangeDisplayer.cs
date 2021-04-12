using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;


public class GlobalItemRangeDisplayer : MonoBehaviour
{
    public static GlobalItemRangeDisplayer Instance;
    
    public List<DisplayRangeStatus> displayRangeStatuses=new List<DisplayRangeStatus>();
    public List<RangeDisplayer> rangeDisplayers=new List<RangeDisplayer>();
    public GameObject circlePfb;
    public GameObject buildingAreaCirclePfb;
    private void Awake()
    {
        Instance = this;
        displayRangeStatuses.Add(new DisplayRangeStatus()
        {
          displayRangeType  = DisplayRangeType.SelfUnit,
          active = true
        });
        displayRangeStatuses.Add(new DisplayRangeStatus()
        {
            displayRangeType  = DisplayRangeType.BuildingArea,
        });
    }

    public void Start()
    {
        for (int i = 0; i < displayRangeStatuses.Count; i++)
        {
            GameObject gameObject=new GameObject();
            gameObject.transform.SetParent(transform);
            RangeDisplayer rangeDisplayer;
            if (displayRangeStatuses[i].displayRangeType == DisplayRangeType.BuildingArea)
            {
                rangeDisplayer = new RangeDisplayer(gameObject.transform, buildingAreaCirclePfb);
            }
            else
            {
                rangeDisplayer = new RangeDisplayer(gameObject.transform, circlePfb);
            }
            rangeDisplayers.Add(rangeDisplayer);
            displayRangeStatuses[i].rangeDisplayer = rangeDisplayer;
        }
        SetDefaultList();
        
    }
    

    public void OnSelfUnitCreate()
    {
        
    }
    private void Update()
    {
        for (int i = 0; i < displayRangeStatuses.Count; i++)
        {
            RangeDisplayer rangeDisplayer = displayRangeStatuses[i].rangeDisplayer;
            rangeDisplayer.Display(displayRangeStatuses[i].active);
        }
       
        
    }

    public void SetList(DisplayRangeType displayRangeType,List<BattleUnitBase> value)
    {
        displayRangeStatuses.Find(x=>x.displayRangeType==displayRangeType).rangeDisplayer.SetList(value);
    }

    public void SetColor(DisplayRangeType displayRangeType,Color targetColor)
    {
        displayRangeStatuses.Find(x=>x.displayRangeType==displayRangeType).rangeDisplayer.SetColor(targetColor);
    }

    public void EnableDisplayRangeType(DisplayRangeType displayRangeType,bool staus)
    {
        displayRangeStatuses.Find(x => x.displayRangeType == displayRangeType).active = staus;
    }

   

    public void SetDefaultList()
    {
        SetList(DisplayRangeType.SelfUnit,BattleUnitBase.selfUnits);
    }

    public void Hide()
    {
        for (int i = 0; i < displayRangeStatuses.Count; i++)
        {
            displayRangeStatuses[i].active = false;
        }
    }
}

public enum DisplayRangeType
{
    SelfUnit,
    BuildingArea
}

public class DisplayRangeStatus
{
    public DisplayRangeType displayRangeType;
    public RangeDisplayer rangeDisplayer;
    public bool active;
}

public class RangeDisplayer
{
    public List<BattleUnitBase> unitList=new List<BattleUnitBase>();
    public List<GameObject> circles=new List<GameObject>();
    private List<Outlinable> circleOutline=new List<Outlinable>();
    private Transform transform;
    
    private GameObject circlePfb;
    private Color circleColor;
    
    public RangeDisplayer(Transform transform,GameObject circlePfb)
    {
        this.transform = transform;
        this.circlePfb = circlePfb;
    }
    
    public void Display(bool status)
    {
        if (status == false)
        {
            UnityTool.SetActiveVirtual(transform.gameObject,false);
            return;
        }
        
        UnityTool.SetActiveVirtual(transform.gameObject,true);
        
           
        BattleUnitBase unitBase;
        for (int i = 0; i < unitList.Count; i++)
        {
            unitBase = unitList[i];

            if (unitBase == null)
            {
                continue;
            }
                
            
            if (circles.Count <= i)
            {
                GameObject tmpCircle = GameObject.Instantiate(circlePfb, transform);
                circles.Add(tmpCircle);
                Outlinable outlinable= tmpCircle.GetComponent<Outlinable>();
                outlinable.BackParameters.Color = circleColor;
                circleOutline.Add(outlinable);
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

        while (circles.Count > unitList.Count)//删除多余视野
        {
            GameObject.Destroy(circles[circles.Count-1]);
            circles.RemoveAt(circles.Count - 1);
            circleOutline.RemoveAt(circles.Count - 1);
        }
        // if (lastFrameUnitCount != targetUnits.Count)
        // {
        //     Debug.Log("重构范围显示");
        //     outlinable.AddAllChildRenderersToRenderingList();
        //     lastFrameUnitCount = BattleUnitBase.selfUnits.Count;
        // }
    }
    
    public void SetList(List<BattleUnitBase> value)
    {
        unitList = value;
    }

    public void SetColor(Color value)
    {
        for (int i = 0; i < circleOutline.Count; i++)
        {
            circleOutline[i].BackParameters.Color = value;
        }
    }
}
