using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 用于区分阵营
/// </summary>
public class FactionManager
{

    private FactionSlot factionSlot;

    public FactionSlot FactionSlot
    {
        get => factionSlot;
        set => factionSlot = value;
    }

    /// <summary>
    /// 基地位置
    /// </summary>
    public Vector3 basePos;
    
    //阵营id
    private  int factionId;

    public int FactionId
    {
        get => factionId;
        set => factionId = value;
    }

    public Color factionColor;
    
    //Task Launchers:
    private List<TaskLauncher> taskLaunchers = new List<TaskLauncher>();
    public IEnumerable<TaskLauncher> GetTaskLaunchers () { return taskLaunchers; }
    
    private List<FactionLimit> limits=new List<FactionLimit>();
    public FactionManager(int factionId,Vector3 basePos,FactionSlot factionSlot,Color factionColor)
    {
        FactionId = factionId;
        this.basePos = basePos;
        FactionSlot=factionSlot;
        this.factionColor = factionColor;
    }
    
    //盟友
    public List<FactionManager> ally=new List<FactionManager>();

    //阵营内得所有单位
    public List<BattleUnitBase> myUnits=new List<BattleUnitBase>();
    public List<BattleUnitBase> enemyUnits=new List<BattleUnitBase>();
    public List<BattleUnitBase> buildings=new List<BattleUnitBase>();
    
    //资源,Npc的资源直接获取全图资源，玩家的资源必须由自己探图将进入视野的资源加入
    public List<ResourceInfo> allResources=new List<ResourceInfo>();
    private Dictionary<BattleResType,List<ResourceInfo>> resourceDic=new Dictionary<BattleResType, List<ResourceInfo>>();

    private BattleResMgr battleResMgr;

    public BattleResMgr BattleResMgr
    {
        get => battleResMgr;
        set => battleResMgr = value;
    }

    private int currentPopulation;

    private int maxPopulation;

   

    public int GetCurrentPopulation()
    {
        return currentPopulation;
    }

    public void UpdateMaxPopulation(int value)
    {
        maxPopulation = value;
    }

    public int GetMaxPopulation()
    {
        return maxPopulation;
    }

    public void Init()
    {
        
           
        
        if (factionSlot.isPlayer == false)
        {
            GameObject npcCommanderGo = GameObject.Instantiate(Resources.Load<GameObject>("NpcCommander"));
            npcCommanderGo.GetComponent<NpcCommander>().Init(this);
        }
        battleResMgr=new BattleResMgr();

        foreach (var limit in factionSlot.factionLimits)
        {
            limits.Add(new FactionLimit()
            {
                battleUnitId = limit.battleUnitId,
                maxAmount = limit.maxAmount
            });
        }
        if (factionId == -1)
        {
            return;
        }
        
        
        //EVENT
        EventCenter.AddListener(EnumEventType.AllFactionsInit,OnAllFactionsInited);
        
        EventCenter.AddListener<BattleUnitBase>(EnumEventType.UnitCreated,OnUnitCreated);
        EventCenter.AddListener<BattleUnitBase>(EnumEventType.UnitDied,OnUnitDied);
        EventCenter.AddListener<TaskLauncher>(EnumEventType.OnTaskLauncherAdded,OnTaskLauncherAdded);
        EventCenter.AddListener<TaskLauncher>(EnumEventType.OnTaskLauncherRemoved,OnTaskLauncherRemoved);
        
        
    }

    public void OnAllFactionsInited()
    {
        BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPosById(BattleUnitId.Base,basePos,factionId);
    }

    private void OnUnitCreated(BattleUnitBase battleUnitBase)
    {
        if (battleUnitBase.factionId != factionId)
        {
            enemyUnits.Add(battleUnitBase);
        }
        else
        {
            myUnits.Add(battleUnitBase);
        }
    }

    private void OnUnitDied(BattleUnitBase battleUnitBase)
    {
       
            //if this is a free unit or does not belong to this faction
            if(battleUnitBase.factionId != factionId)
            {
                enemyUnits.Remove(battleUnitBase);
                return;
            }
            //todo 添加矿物死亡事件
            myUnits.Remove (battleUnitBase);
            CheckFactionDefeat(); //check if the faction doesn't have any buildings/units anymore and trigger the faction defeat in that case
        
    }

    private void OnTaskLauncherAdded(TaskLauncher taskLauncher)
    {
        if(taskLauncher.battleUnitBase.factionId == factionId) //make sure the new task launcher belongs to the faction managed by this component
            taskLaunchers.Add(taskLauncher); //add task launcher to list.
    }
    
    private void OnTaskLauncherRemoved (TaskLauncher taskLauncher)
    {
        if(taskLauncher.battleUnitBase.factionId == factionId) //make sure the new task launcher belongs to the faction managed by this component
            taskLaunchers.Remove(taskLauncher); //remove task launcher from list
    }
    
    public bool HasReachedLimit(BattleUnitId unitId)
    {
        foreach(FactionLimit limit in limits)
        {
            if (limit.battleUnitId==unitId && limit.IsMaxReached())
                return limit.IsMaxReached();
        }

        //if the building/unit is not found in the list
        return false;
    }

    public void UpdateLimitList(BattleUnitId battleUnitId,int updateAmount)
    {
        foreach(FactionLimit limit in limits)
        {
            if (limit.battleUnitId==battleUnitId)
                limit.Update(updateAmount);
        }
    }

    public void Update()
    {
        battleResMgr.Update();
    }
    private void CheckFactionDefeat ()
    {
        //if the defeat condition is set to eliminate all units and buildings and there are no more units and buildings for this faction
        // if (FightingManager.Instance.GetDefeatCondition() == DefeatConditions.eliminateAll && units.Count == 0 && buildings.Count == 0)
        //     gameMgr.OnFactionDefeated(FactionID);
    }


    public void Disable()//销毁阵营
    {
        //Event
        EventCenter.RemoveListener(EnumEventType.AllFactionsInit,OnAllFactionsInited);
        
        EventCenter.RemoveListener<BattleUnitBase>(EnumEventType.UnitCreated,OnUnitCreated);
        EventCenter.RemoveListener<BattleUnitBase>(EnumEventType.UnitDied,OnUnitDied);
        EventCenter.RemoveListener<TaskLauncher>(EnumEventType.OnTaskLauncherAdded,OnTaskLauncherAdded);
        EventCenter.RemoveListener<TaskLauncher>(EnumEventType.OnTaskLauncherRemoved,OnTaskLauncherRemoved);
        
    }

    public void UpdateCurrentPopulation(int value)
    {
        currentPopulation += value;
        //custom event trigger:
        //CustomEvents.OnCurrentPopulationUpdated(this, value);
    }

    public void AddResource(ResourceInfo resourceInfo)
    {
        allResources.Add(resourceInfo);
        var resourceInfos = new List<ResourceInfo>();
        BattleResType resourceType = resourceInfo.resourceTypeInfo.resourceType;
        if (resourceDic.ContainsKey(resourceType))
        {
            resourceDic[resourceType].Add(resourceInfo);
        }
        else
        {
            resourceDic.Add(resourceInfo.resourceTypeInfo.resourceType,new List<ResourceInfo>(){resourceInfo});
        }
        
    }

    public void RemoveResource(ResourceInfo resourceInfo)
    {
        allResources.Remove(resourceInfo);
        resourceDic[resourceInfo.GetResourceType().resourceType].Remove(resourceInfo);
    }

    public ResourceInfo FindOtherNearestResource(BattleResType resourceType,Vector3 pos)
    {
        if (resourceDic.ContainsKey(resourceType) == false)
            return null;
        List<ResourceInfo> resourceInfos = resourceDic[resourceType];
        resourceInfos=resourceInfos.OrderBy(x => Vector3.Distance(x.transform.position, pos)).ToList();
        for (int i = 0; i < resourceInfos.Count; i++)
        {
            if (resourceInfos[i].CanAddWorker()&& resourceInfos[i].IsEmpty==false)
            {
                return resourceInfos[i];
            }
        }

        return null;
    }

    public BattleUnitBase FindNearest(List<BattleUnitBase> units,Vector3 pos)
    {
        var temp=units.OrderBy(x => Vector3.Distance(x.transform.position, pos));
        return temp.ElementAtOrDefault(0);
    }

    public List<BattleUnitBase> FindUnitsByUnitId(BattleUnitId value)
    { 
        var units=myUnits.FindAll(x => x.configId == value);
        return units;
    }
    
    public BattleUnitBase FindNearest(BattleUnitId value,Vector3 pos)
    {
        var units=myUnits.FindAll(x => x.configId == value);
        return FindNearest(units, pos);
    }
    
    public bool IsPlayer()
    {
        return factionSlot.isPlayer;
    }
}
