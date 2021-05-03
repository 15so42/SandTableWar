using System.Collections;
using System.Collections.Generic;
using RTSEngine;
using UnityEngine;

[System.Serializable]
public struct SearchCellPosition
{
    public int x;
    public int y;
}
public class SearchCell
{

    public SearchCellPosition searchCellPosition;
    private GridSearchHandler gridSearchHandler;

    private List<BattleUnitBase> units=new List<BattleUnitBase>();
    
    //移动中的单位
    private List<BattleUnitBase> movingUnits = new List<BattleUnitBase>();
    
    private IEnumerator unitPositionCheckCoroutine;
    
    public void Init(SearchCellPosition searchCellPosition,GridSearchHandler gridSearchHandler)
    {
        this.searchCellPosition = searchCellPosition;
        this.gridSearchHandler = gridSearchHandler;
    }

    public void Add(BattleUnitBase unit)
    {
        if (!units.Contains(unit) && unit.unitMovement)
        {
            units.Add(unit);
            unit.unitMovement.unitMoveStart += OnUnitStartMoving;
            unit.unitMovement.unitMoveStop += OnUnitStopMoving;
            
        }
    }
    
    private void OnUnitStartMoving (BattleUnitBase unit)
    {
        if (!movingUnits.Contains(unit)) //正在移动的单位列表
            movingUnits.Add(unit); 

        if(unitPositionCheckCoroutine == null) //保证每个格子只有一个协程
        {
            unitPositionCheckCoroutine = UnitPositionCheck(0.1f);
            gridSearchHandler.StartCoroutine(unitPositionCheckCoroutine);
        }
    }
    
    private void OnUnitStopMoving (BattleUnitBase unit)
    {
        movingUnits.Remove(unit); //remove it from being the unit position track list
    }
    private IEnumerator UnitPositionCheck(float waitTime)//更新正在移动中的单位的所属格子
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            int i = 0;
            while (i < movingUnits.Count)
            {
                if (!IsPosIn(movingUnits[i].transform.position)) //如果单位已经离开了所属格子
                {
                    BattleUnitBase unit = movingUnits[i];

                    if(gridSearchHandler.TryGetSearchCell(unit.transform.position, out SearchCell newCell) == ErrorMessage.none) //找到单位的新的格子并将单位交给新的格子管理
                        newCell.Add(unit);

                    Remove(unit); //将单位从当前格子移除

                    continue;
                }

                i++;
            }
        }
    }

    private bool IsPosIn(Vector3 testPosition)
    {
        return testPosition.x >= searchCellPosition.x && testPosition.x < searchCellPosition.x + gridSearchHandler.CellSize
                                                      && testPosition.z >= searchCellPosition.y && testPosition.z < searchCellPosition.y + gridSearchHandler.CellSize;
    }

    public void Remove(BattleUnitBase unit)
    {
        if (units.Contains(unit))
        {
            units.Remove(unit);
            movingUnits.Remove(unit);
            unit.unitMovement.unitMoveStart -= OnUnitStartMoving;
            unit.unitMovement.unitMoveStop -= OnUnitStopMoving;
        }
        if (unitPositionCheckCoroutine != null && movingUnits.Count == 0) //if there are no more moving units and the check coroutine is runing
        {
            //stop coroutine as there are no longer units moving inside this cell.
            gridSearchHandler.StopCoroutine(unitPositionCheckCoroutine);
            unitPositionCheckCoroutine = null;
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
