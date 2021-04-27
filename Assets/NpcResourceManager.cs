﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RTSEngine;

/// <summary>
    /// Responsible for managing the NPC faction's resources.
    /// </summary>
    public class NpcResourceManager : NpcComponent
    {
        public override void Init(FightingManager fightingManager, NpcCommander npcCommander, FactionManager factionMgr)
        {
            base.Init(fightingManager, npcCommander, factionMgr);
            EventCenter.AddListener<ResourceInfo>(EnumEventType.ResourceCreated,OnResourceCreated);
        }

        public List<ResourceInfo> allResource;

        public void OnResourceCreated(ResourceInfo resource)
        {
            allResource.Add(resource);
            npcCommander.GetNpcComp<NpcResourceCollector>().AddResourceToCollect(resource);
        }

        private void OnDisable()
        {
            EventCenter.RemoveListener<ResourceInfo>(EnumEventType.ResourceCreated,OnResourceCreated);
        }
    }