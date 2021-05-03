using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SearchCellPosition
{
    public int x;
    public int y;
}
public class SearchCell
{

    private SearchCellPosition searchCellPosition;
    private GridSearchHandler gridSearchHandler;

    private List<BattleUnitBase> units=new List<BattleUnitBase>();
    public void Init(SearchCellPosition searchCellPosition,GridSearchHandler gridSearchHandler)
    {
        this.searchCellPosition = searchCellPosition;
        this.gridSearchHandler = this.gridSearchHandler;
    }

    public void Add(BattleUnitBase unit)
    {
        if (!units.Contains(unit))
        {
            units.Add(unit);
        }
    }

    public void Remove(BattleUnitBase unit)
    {
        if (units.Contains(unit))
        {
            units.Remove(unit);
        }
    }

    
    public List<BattleUnitBase> GetUnits()
    {
        //存储的单位可能已经因为死亡而销毁，因此在判断单位已经变为null或者IsAlive()==false时清除掉对应单位
        int i = 0;
        while (i < units.Count)//因为涉及到销毁，所以使用while循环，每次销毁units.Count都会跟随而发生变化以避免发生list越界问题
        {
            if (units[i] == null || units[i].IsAlive() == false)
            {
                Remove(units[i]);
            }

            i++;
        }

        return units;
    }
}
