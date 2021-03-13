using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefenceBuilding : BaseBattleBuilding
{
    [Header("防守点")]
    public Transform[] defencePoses;

    [Header("可以进入的兵种id")] public BattleUnitId[] allowedId;
    [Header("入口")] public Transform entrance;
    
    public List<BattleUnitBase> soliderInBuilding=new List<BattleUnitBase>();

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void OnCollisionEnter(Collision other)
    {
        BattleUnitBase battleUnitBase = other.transform.root.GetComponent<BattleUnitBase>();
        if (battleUnitBase == null)
        {
            return;
        }
        
        //判断是否与允许进入
        DiplomaticRelation relation = EnemyIdentifier.Instance.GetDiplomaticRelation(campId);
        
        
        if (battleUnitBase.isGoingBuilding)
        {
            if (campId != -1 && relation != DiplomaticRelation.Ally)//自己不是中立，表示已经有人，而且要进入的人时敌人就不能进入
            {
                battleUnitBase.isGoingBuilding = false;
                return;
            }
            if (!soliderInBuilding.Contains(battleUnitBase))
            {
                ClearDiedSolider();
                LetSoliderIn(battleUnitBase);
                //首个进入时改变阵营
                if (soliderInBuilding.Count == 1)
                {
                    SetCampInPhoton(battleUnitBase.campId);
                }
            }
        }
    }

    public void LetSoliderIn(BattleUnitBase battleUnitBase)
    {
        if (soliderInBuilding.Count>= defencePoses.Length)
        {
            return;
        }
        battleUnitBase.BeforeInDefenceBuilding();
        soliderInBuilding.Add(battleUnitBase);
        battleUnitBase.transform.position = defencePoses[soliderInBuilding.Count - 1].transform.position;
        battleUnitBase.OnEnterBuilding(this);
    }

    protected override void OnRightMouseUp()
    {
        base.OnRightMouseUp();
        fightingManager.MoveToSpecificPos(GetEntrance());
        int count = 0;
        DiplomaticRelation relation = EnemyIdentifier.Instance.GetDiplomaticRelation(campId);
        if (relation == DiplomaticRelation.Enemy)
        {
            return;
        }
        
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

    public void OutBuilding()
    {
        foreach (var solider in soliderInBuilding)
        {
            Vector3 exitPos=spawnPos.transform.position;
            solider.transform.position = exitPos;
            solider.OutBuilding();
            solider.SetTargetPos(exitPos);
        }
        SetCampInPhoton(-1);
    }

    public Vector3 GetEntrance()
    {
        return entrance.transform.position;
    }

    //清除死亡士兵的占位
    private void ClearDiedSolider()
    {
        for (int i = 0; i < soliderInBuilding.Count; i++)
        {
            if (soliderInBuilding[i] == null)
            {
                soliderInBuilding.Remove(soliderInBuilding[i]);
            }
        }
    }
   
    
}
