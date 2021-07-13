using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class HasNewMoveEvent : Conditional
{
    public SharedVector3 lastDestinationPos;
    public SharedVector3 destinationPos;

    private BattleUnitBase selfUnit;
    private FightingManager fightingManager;
    public override void OnAwake()
    {
        Owner.RegisterEvent<Vector3>("SetDestinationPos", SetNewDestination);
        selfUnit = GetComponent<BattleUnitBase>();
        fightingManager = GameManager.Instance.GetFightingManager();
    }

    private bool setFalsed;
    private TaskStatus lastTaskStatus;

    public override void OnStart()
    {
        base.OnStart();
        setFalsed = false;
        
    }

    public override TaskStatus OnUpdate()
    {
        if (destinationPos != null && lastDestinationPos.Value != destinationPos.Value)
        {
            lastTaskStatus = TaskStatus.Success;
            return TaskStatus.Success;
        }

        if(lastTaskStatus==TaskStatus.Success)
            setFalsed = true;
        lastTaskStatus = TaskStatus.Failure;
        return TaskStatus.Failure;
    }
    

    public void SetNewDestination(Vector3 pos)
    {
        if (setFalsed && selfUnit.factionId==fightingManager.myFactionId && selfUnit.configId==BattleUnitId.Ranger)
        {
            Debug.Log("出现啦！！！！！！！！！！！！！！！！！！！！！");
        }
        destinationPos.SetValue(pos);
       
    }
}