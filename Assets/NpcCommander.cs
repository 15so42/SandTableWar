using System;
using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts.AI;
using UnityEngine;
using UnityEngine.Assertions;

public class NpcCommander : MonoBehaviour
{
    //所属阵营
    public FactionManager factionManager;
    private FactionSlot factionSlot;//阵营基本配置

    private float elapsedTime;
    public int lowResourcesTime=30;

    //Holds the NPC components that extend NPCComponent that regulate the behavior of this instance of a NPC faction
    private Dictionary<Type, NpcComponent> npcCompDic = new Dictionary<Type, NpcComponent>();

   

    public void Init(FactionManager factionManager)
    {
        this.factionManager = factionManager;
        this.factionSlot = factionManager.FactionSlot;
        foreach (NpcComponent comp in GetComponentsInChildren<NpcComponent>()) //go through the NPC components and init them
        {
            npcCompDic.Add(comp.GetType(), comp);
            comp.Init(FightingManager.Instance, this, this.factionManager);
        }
      
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        
    }

   

    /// <summary>
    /// 开局30秒内发育时间
    /// </summary>
    /// <returns></returns>
    public bool isInLowResourcesTime()
    {
        return elapsedTime < lowResourcesTime;
    }

    public T GetNpcComp<T> () where T : NpcComponent
    {
        Assert.IsTrue(npcCompDic.ContainsKey(typeof(T)),
            $"[NPCManager] NPC Faction ID {factionManager.FactionId} does not have an active instance of NPCComponent type: {typeof(T)}!");

        if (npcCompDic.TryGetValue(typeof(T), out NpcComponent value))
            return value as T;

        return null;
    }
   
    
}
