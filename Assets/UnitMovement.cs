using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于分区寻找单位
/// </summary>
public class UnitMovement : MonoBehaviour
{
    private BattleUnitBase battleUnitBase;

    public bool airUnit;
    // Start is called before the first frame update
    void Start()
    {
        battleUnitBase = GetComponent<BattleUnitBase>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Action<BattleUnitBase> unitMoveStart;
    public Action<BattleUnitBase> unitMoveStop;

    public void StartMove()
    {
        unitMoveStart?.Invoke(battleUnitBase);
    }

    public void StopMove()
    {
        unitMoveStop?.Invoke(battleUnitBase);
    }
}
