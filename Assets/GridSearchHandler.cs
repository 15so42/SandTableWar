using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RTSEngine;
using UnityEngine;


public class GridSearchHandler : MonoBehaviour
{
    [SerializeField, Tooltip("网格平面左上角")]
    private SearchCellPosition lowerLeftCorner = new SearchCellPosition { x = 0, y = 0 };
    [SerializeField, Tooltip("网格平面右上角")]
    private SearchCellPosition upperRightCorner = new SearchCellPosition { x = 100, y = 100 };
    
    [SerializeField, Tooltip("每个网格的大小"), Min(1)]
    private int cellSize = 10;
    public int CellSize { get { return cellSize; } }
    
    //通过坐标维护SearchCell字典
    private Dictionary<SearchCellPosition, SearchCell> gridDict = new Dictionary<SearchCellPosition, SearchCell>();
    
    //根据项目决定
    private FightingManager fightingManager;

    public static GridSearchHandler Instance;
    public void Init()
    {
        Instance = this;
        GenerateCells(); //生成寻敌网格
        
        //事件绑定
        EventCenter.AddListener<BattleUnitBase>(EnumEventType.UnitCreated,OnEntityCreated);
        EventCenter.AddListener<BattleUnitBase>(EnumEventType.UnitDied,OnEntityRemoved);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<BattleUnitBase>(EnumEventType.UnitCreated,OnEntityCreated);
        EventCenter.RemoveListener<BattleUnitBase>(EnumEventType.UnitDied,OnEntityRemoved);
    }

    private void GenerateCells()
    {
        fightingManager=FightingManager.Instance;
        for(int x = lowerLeftCorner.x; x < upperRightCorner.x; x += cellSize)
        for (int y = lowerLeftCorner.y; y < upperRightCorner.y; y += cellSize)
        {
            //each search cell instance is added to the dictionary after it is created for easier direct access using coordinates in the future.
            SearchCellPosition nextPosition = new SearchCellPosition
            {
                x = x,
                y = y
            };

            gridDict.Add(nextPosition, new SearchCell());
        }

        foreach (SearchCellPosition position in gridDict.Keys) //初始化每个寻敌格子，格子中存放在各自内的单位。
            gridDict[position].Init(position, this);

        // StartCoroutine(UnitPositionCheck(0.1f));
    }
    
    private void OnEntityCreated (BattleUnitBase entity)
    {
        if (TryGetSearchCell(entity.transform.position, out SearchCell cell) == ErrorMessage.none) //if there's a cell that can accept this entity
            cell.Add(entity); //add it
    }
    
    private void OnEntityRemoved (BattleUnitBase entity)
    {
        if (TryGetSearchCell(entity.transform.position, out SearchCell cell) == ErrorMessage.none) //if there's a cell that can accept this entity
            cell.Remove(entity); //remove it
    }
   
    public ErrorMessage TryGetSearchCell(Vector3 position, out SearchCell cell)
    {
        cell = null;

        SearchCellPosition nextPosition =
            new SearchCellPosition //(当前位置的x-起始位置x)/cellSize表示球的当前在第几格，注意强制转型int以避免整个计算被隐形转换为浮点数计算
            {
                x = ((int)( position.x - lowerLeftCorner.x) / cellSize) * cellSize + lowerLeftCorner.x,
                y = ((int)( position.z - lowerLeftCorner.y) / cellSize) * cellSize + lowerLeftCorner.y
            };

        if (gridDict.TryGetValue(nextPosition, out cell))//如果在格子类
            return ErrorMessage.none;

        Debug.LogError($"{position}不在寻敌网格范围内!");
        return ErrorMessage.searchCellNotFound;
    }
    
    //在指定位置通过半径在寻敌网格中寻敌，其中IsTargetValid用于判断那些单位可以被选中
    //Filter形如：
    // public override ErrorMessage IsTargetValid (BattleUnitBase target)
    // {
    //     if (target == null)
    //         return ErrorMessage.invalid;
    //     else if (target.HealthComp.CurrHealth >= target.HealthComp.MaxHealth)
    //         return ErrorMessage.targetMaxHealth;
    //
    //     return ErrorMessage.none;
    // }
    //在不同单位要使用Filter时只需重写对应的IsTargetValid函数即可，比如医疗兵判断是否满血，反坦克步兵判断是否时坦克，这样就不用写多次寻敌函数
    public ErrorMessage Search<T>(Vector3 sourcePosition, float radius,bool resource, System.Func<T, ErrorMessage> isTargetValid, out T resultTarget) where T : BattleUnitBase
        {
            resultTarget = null;
            ErrorMessage errorMessage;

            //如果在寻敌网格外
            if ((errorMessage = TryGetSearchCell(sourcePosition, out SearchCell sourceCell)) != ErrorMessage.none)
                return errorMessage;

            float distance = radius * radius;//使用sqrMangitude来节约性能
            List<T> resultsInRadius = GetUnitsByRadius(sourcePosition, radius, resource,isTargetValid);
            resultsInRadius = resultsInRadius.OrderByDescending(x => Vector3.SqrMagnitude(x.transform.position - sourcePosition)).ToList();
            //获取到的单位是正方形网格中的所有单位，此函数的radius是寻敌半径，所以要判断半径小于寻敌半径的才是要获取的单位
            for (int i = 0; i < resultsInRadius.Count; i++)
            {
                if (Vector3.SqrMagnitude(sourcePosition - resultsInRadius[i].transform.position) <= distance)
                {
                    resultTarget = resultsInRadius[i];
                }
            }
            
            if (resultTarget != null) 
                return ErrorMessage.none;
            return ErrorMessage.searchTargetNotFound;
        }
    
   
    /// <summary>
    /// 获取圆圈所占据的所有网格中所有的单位，
    /// </summary>
    /// <param name="sourcePosition"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    private List<T> GetUnitsByRadius<T>(Vector3 sourcePosition, float radius,bool resource,System.Func<T, ErrorMessage> filter) where T : BattleUnitBase
    {
        Vector2 lowerLeftCorner=new Vector2(sourcePosition.x-radius,sourcePosition.z-radius);
        Vector2 upperRightCorner=new Vector2(sourcePosition.x+radius,sourcePosition.z+radius);
        if(SearchRect(lowerLeftCorner, upperRightCorner,resource, filter, out var resultList)==ErrorMessage.none);
            return resultList;
    }
    
    //在矩形区域根据寻敌网格寻找单位，其中Filter是过滤器，这个函数也可用于鼠标框选单位功能。
    public ErrorMessage SearchRect<T>(Vector2 lowerLeftCorner, Vector2 upperRightCorner, bool resource,System.Func<T, ErrorMessage> filter, out List<T> resultList) where T : BattleUnitBase
    {
        resultList = new List<T>();
        ErrorMessage errorMessage;

        for(float x = lowerLeftCorner.x; x < upperRightCorner.x; x += cellSize)
        for(float y = lowerLeftCorner.y; y < upperRightCorner.y; y += cellSize)
        {
            if((errorMessage = TryGetSearchCell(new Vector3(x, 0, y), out SearchCell cell)) != ErrorMessage.none)
                return errorMessage;

            //for each cell, search the stored entities (get either resources or faction entities)
            foreach (BattleUnitBase unit in  cell.GetUnits(resource))
            {
                if (unit == null)
                    continue;

                if (unit.transform.position.x >= lowerLeftCorner.x && unit.transform.position.z >= lowerLeftCorner.y
                                                                   && unit.transform.position.x <= upperRightCorner.x &&
                                                                   unit.transform.position.z <= upperRightCorner.y
                                                                   && filter(unit as T)==ErrorMessage.none)
                    resultList.Add(unit as T);
            }
        }

        return ErrorMessage.none;
    }


    
#if UNITY_EDITOR
    [Header("Gizmos")]
    public Color gizmoColor = Color.yellow;
    [Min(1.0f)]
    public float gizmoHeight = 1.0f;

    private void OnDrawGizmosSelected()
    {
        if (cellSize <= 0)
            return;

        Gizmos.color = gizmoColor;
        Vector3 size = new Vector3(cellSize, gizmoHeight, cellSize);

        for(int x = lowerLeftCorner.x; x < upperRightCorner.x; x += cellSize)
        for (int y = lowerLeftCorner.y; y < upperRightCorner.y; y += cellSize)
        {
            Gizmos.DrawWireCube(new Vector3(x + cellSize/2.0f, 0.0f, y + cellSize/2.0f), size);
        }

        for (int i = 0; i < gridDict.Values.Count; i++)
        {
            SearchCell searchCell = gridDict.Values.ElementAt(i);
            List<BattleUnitBase> cellUnits = searchCell.GetAllUnits();
            Gizmos.color = Color.blue;
            for (int j = 0; j < cellUnits.Count; j++)
            {
                Gizmos.DrawLine(new Vector3(searchCell.searchCellPosition.x,0,searchCell.searchCellPosition.y),cellUnits[j].transform.position);
            }
            
        }
    }
#endif
}
