using System;
using System.Collections;
using System.Collections.Generic;
using FoW;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(WorkerManager))]
public class ResourceInfo : MonoBehaviour
{
    public ResourceTypeInfo resourceTypeInfo;
    public  WorkerManager workerManager;
    private int factionId;

    private BattleUnitBase battleUnitBase;
    [Header("Npc围绕资源放置建筑时的半径")]
    private float aroundRadius = 2;

    public int FactionId
    {
        get => factionId;
        set => factionId = value;
    }

    private bool isEmpty;
    
    public bool IsEmpty
    {
        get => isEmpty;
        set => isEmpty = value;
    }

    private UnityAction enterFogAction;
    
    // Start is called before the first frame update
    void Start()
    {
        workerManager = GetComponent<WorkerManager>();
        EventCenter.Broadcast(EnumEventType.ResourceCreated,this);
        battleUnitBase = GetComponent<BattleUnitBase>();
        enterFogAction = () =>
        {
            FightingManager.Instance.GetMyFaction().AddResource(this);
        };
        battleUnitBase.AddEnterFogListener(enterFogAction);
        
    }

    public BattleUnitId GetBattleUnitId()
    {
        return battleUnitBase.configId;
    }
    

    public ResourceTypeInfo GetResourceType()
    {
        return resourceTypeInfo;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanAddWorker()
    {
        return workerManager.CanAddWorker();
    }

    private void OnDestroy()
    {
        battleUnitBase.RemoveEnterFogListener(enterFogAction);
    }

    public float GetRadius()
    {
        return aroundRadius;
    }
}
