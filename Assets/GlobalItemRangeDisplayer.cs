using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;

public class GlobalItemRangeDisplayer : MonoBehaviour
{
    public GameObject circlePfb;
    private Outlinable outlinable;

    private List<GameObject> circles=new List<GameObject>();

    public static GlobalItemRangeDisplayer Instance;
    private void Awake()
    {
        Instance = this;
        outlinable = GetComponent<Outlinable>();
    }

    public void Display()
    {
        outlinable.OutlineParameters.Enabled = true;
        BattleUnitBase unitBase;
        for (int i = 0; i < BattleUnitBase.selfUnits.Count; i++)
        {
            unitBase = BattleUnitBase.selfUnits[i];
            
            if(unitBase==null)
                continue;
            
            if (circles.Count <= i)
            {
               circles.Add(GameObject.Instantiate(circlePfb,transform));
            }

            GameObject circle = circles[i];
            circle.transform.position = unitBase.transform.position;
            float size = unitBase.prop.viewDistance;
            circle.transform.localScale=new Vector3(size,0.1f,size);
        }
        outlinable.AddAllChildRenderersToRenderingList();
    }

    private void Update()
    {
        GlobalItemRangeDisplayer.Instance.Display();
    }

    public void Hide()
    {
        outlinable.OutlineParameters.Enabled = false;
    }
}
