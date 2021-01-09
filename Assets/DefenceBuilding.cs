using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefenceBuilding : BaseBattleBuilding
{
    [Header("防守点")]
    public Transform[] defencePoses;

    [Header("可以进入的兵种id")] public int[] allowedId;
    [Header("入口")] public Transform entrance;
    
    public List<BattleUnitBase> soliderInBuilding=new List<BattleUnitBase>();

    protected override void Awake()
    {
        base.Awake();
        Debug.LogError("堡垒Awake");
    }

    protected override void Start()
    {
        base.Start();
        Debug.LogError("堡垒Start");
    }

    private void OnCollisionEnter(Collision other)
    {
        BattleUnitBase battleUnitBase = other.transform.root.GetComponent<BattleUnitBase>();
        Debug.LogError("碰撞者isGoingBuilding为"+battleUnitBase.isGoingBuilding);
        if (battleUnitBase.isGoingBuilding)
        {
            if(!soliderInBuilding.Contains(battleUnitBase))
                LetSoliderIn(battleUnitBase);
        }
    }

    public void LetSoliderIn(BattleUnitBase battleUnitBase)
    {
        if (soliderInBuilding.Count > defencePoses.Length)
        {
            return;
        }
        battleUnitBase.BeforeInDefenceBuilding();
        soliderInBuilding.Add(battleUnitBase);
        battleUnitBase.transform.position = defencePoses[soliderInBuilding.Count - 1].transform.position;
        battleUnitBase.OnEnterBuilding();
    }

    protected override void OnRightMouseUp()
    {

        base.OnRightMouseUp();
        int count = 0;
        foreach (var unit in fightingManager.selectedUnits)
        {
            if (allowedId.Contains(unit.configId))
            {
                if (count < defencePoses.Length)//还有空位
                {
                    unit.GoInDefenceBuilding(this);
                    count++;
                }
            }
        }
    }

    public Vector3 GetEntrance()
    {
        return entrance.transform.position;
    }
}
