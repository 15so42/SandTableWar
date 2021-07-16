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
    private bool inputAble=true;
    private float syncFrame;
    private bool syncingFrame = false;
    private Coroutine setPosCoroutine;
    
    public override void OnAwake()
    {
        Owner.RegisterEvent<Vector3>("SetDestinationPos", SetNewDestination);
        selfUnit = GetComponent<BattleUnitBase>();
        fightingManager = GameManager.Instance.GetFightingManager();
    }

    
    private TaskStatus lastTaskStatus;

    public void DisableInput()
    {
        inputAble = false;
    }
    
    public void EnableInput()
    {
        inputAble = true;
    }
    public override void OnStart()
    {
        base.OnStart();
        
        
    }

    public override TaskStatus OnUpdate()
    {
        if (syncingFrame)
            syncFrame++;
        if (destinationPos != null && lastDestinationPos.Value != destinationPos.Value)
        {
            return TaskStatus.Success;
        }

        syncingFrame = false;
        return TaskStatus.Failure;
    }

    public void StartSyncTime()
    {
        syncFrame = 0;
        syncingFrame = true;
    }

    public void SetNewDestination(Vector3 pos)
    {
        if (inputAble == false)
        {
            return;
        }

        StartCoroutine(SetPos(pos));


    }

    IEnumerator SetPos(Vector3 pos)
    {
       
        if (selfUnit.factionId==fightingManager.myFactionId && selfUnit.configId==BattleUnitId.Ranger)
        {
            //Debug.Log("设置destionPos:"+pos+",lastDestPos:"+lastDestinationPos.Value);
            //Debug.Log("时间差为:"+(Time.time-syncFrame));
            if (syncFrame < 1)
            {
                
                Debug.Log("在sync得同一帧内设置了新地址");
                yield return null;//等待一帧
            }
        }
        
        destinationPos.SetValue(pos);
    }
}