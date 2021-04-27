using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BattleScene.Scripts.AI;
using UnityEngine;
using UnityEngine.Assertions;

/* NPCResourceCollector script created by Oussama Bouanani, SoumiDelRio.
 * This script is part of the Unity RTS Engine */

namespace RTSEngine
{
    /// <summary>
    /// Holds information regarding regulating how one resource type is collected, these are entered by the in the inspector.
    /// </summary>
    [System.Serializable]
    public struct ResourceTypeCollectionSettings
    {
        [Tooltip("The resource type whose collection will be regulated by the next settings.")]
        public ResourceTypeInfo type; //the resource type whose collection will be affected by the settings below

        //the collectors ratio determines how many collectors will be assigned to collect the above resource type.
        [Tooltip("How many collector will be assigned to collect this resource type? (Per resource instance!)")]
        public FloatRange instanceCollectorsRatio; //per resource instance!!

        [Tooltip("The maximum amount of collectors that can collect this resource at the same time.")]
        public FloatRange maxCollectorsRatioRange; //the maximum amount of collectors that can collect this resource at the same time.

        [Tooltip("To ensure that the NPC faction collects this resource type, you can set a minimum collector amount which the NPC faction will prioritize"), Min(0)]
        public int minCollectorsAmount;
    }

    /// <summary>
    /// Holds information regarding regulating how one resource type is collected (not entered through the inspector)
    /// </summary>
    public class ResourceTypeCollection
    {
        public FloatRange instanceCollectorsRatio; //per resource instance!!

        public FloatRange maxCollectorsRatioRange; //the maximum amount of collectors that can collect this resource at the same time.

        public int minCollectorsAmount = 0; //the minimum amount required to have of resource collectors to collect the resource type.需要多少工人才能手机资源
        public int collectorsAmount = 0; //current amount of resource collectors.

        //the instances of the same resource type that are being actively collected
        public List<ResourceInfo> instances = new List<ResourceInfo>();

        //actively monitors the instances of NPCUnitRegulator for collector units that are able to collect the resource regulated by an instance of this class
        public NpcActiveRegulatorMonitor collectorMonitor = new NpcActiveRegulatorMonitor(); 

        /// <summary>
        /// Calculates whether a new collector can be assigned to a resource type regulated by this instance
        /// </summary>
        /// <param name="totalCollectorsCount">The current total amount of resource collector unit instances.</param>
        /// <returns>True if a new collector can be added, otherwise false.</returns>
        public bool CanAddCollector (int availableCollectorsAmount)
        {
            return minCollectorsAmount > collectorsAmount //either the minimum collectors amount has not been reached...
                || (availableCollectorsAmount * maxCollectorsRatioRange.getRandomValue()) > collectorsAmount; //or we can still fit new collectors according to the max collectors ratio range.
        }

        /// <summary>
        /// Calculates the amount of resource collectors to send to collect a resource.
        /// </summary>
        /// <param name="resourceInfo">The Resource instance which will be collected.</param>
        /// <returns>Amount of collectors to send to collect the resource.</returns>
        public int GetTargetCollectorsAmount (ResourceInfo resourceInfo)
        {
            //how many builders do we need to assign for this building?
            int targetBuildersAmount = (int)(resourceInfo.workerManager.GetAvailableSlots() * instanceCollectorsRatio.getRandomValue());

            return Mathf.Max(targetBuildersAmount, 1); //can't be lower than 1
        }
    }

    /// <summary>
    /// Responsible for managing resource collector units and collecting resources for a NPC faction.
    /// </summary>
    public class NpcResourceCollector : NpcComponent
    {
        #region Class Properties
        [SerializeField, Tooltip("Potential list of units with the Resource Collector component that the NPC faction can use to collect resources.")]
        private List<BattleUnitId> collectors = new List<BattleUnitId>(); //potential units with a Resource Collector component.

        [SerializeField, Tooltip("A list of resource types and settings regarding regulating their collection.")]
        //this list is manipulated by you through the inspector
        private List<ResourceTypeCollectionSettings> collectionSettings = new List<ResourceTypeCollectionSettings>();
        //on initializiation, the above list is transformed in this dictionary which allows access time to be constant and also holds other settings for resource collection
        private Dictionary<ResourceTypeInfo, ResourceTypeCollection> collectionInfo = new Dictionary<ResourceTypeInfo, ResourceTypeCollection>();

        //how often will this component check resources to collect?
        [SerializeField, Tooltip("How often to check for resources to collect?")]
        private FloatRange collectionTimerRange = new FloatRange(3.0f, 5.0f);
        private float collectionTimer;

        [SerializeField, Tooltip("When enabled, other NPC components can request to collect resources.")]
        private bool collectOnDemand = true; //when another component requests to collect a resource, allow it or not?
        #endregion

        #region Initializing/Terminating
        /// <summary>
        /// Initializes the NPCResourceCollector instance, called from the NPCManager instance responsible for this component.
        /// </summary>
        /// <param name="gameMgr">GameManager instance of the current game.</param>
        /// <param name="npcMgr">NPCManager instance that manages this NPCComponent instance.</param>
        /// <param name="factionMgr">FactionManager instance of the faction that this component manages.</param>
        public override void Init(FightingManager fightingManager, NpcCommander npcCommander, FactionManager factionMgr)
        {
            base.Init(fightingManager, npcCommander, factionMgr);

            InitResourceCollectionInfo(); //initialize the dictionary data structure that basically controls how each resource type is collected

            //add event listeners:
            // CustomEvents.UnitStopCollecting += OnUnitStopCollecting;
            // CustomEvents.UnitCollectionOrder += OnUnitCollectionOrder;
            //CustomEvents.ResourceEmpty += OnResourceEmpty;
            //CustomEvents.UnitCreated += OnUnitCreated;
            
            EventCenter.AddListener<BattleUnitBase>(EnumEventType.UnitCreated,OnUnitCreated);
        }

        /// <summary>
        /// Called when the object holding this component is disabled/destroyed.
        /// </summary>
        private void OnDisable()
        {
            //remove event listeners:
            //CustomEvents.UnitStopCollecting -= OnUnitStopCollecting;
            //CustomEvents.UnitCollectionOrder -= OnUnitCollectionOrder;
            //CustomEvents.ResourceEmpty -= OnResourceEmpty;
            //CustomEvents.UnitCreated -= OnUnitCreated;
            EventCenter.RemoveListener<BattleUnitBase>(EnumEventType.UnitCreated,OnUnitCreated);
            
            foreach(ResourceTypeCollection rtc in collectionInfo.Values)
                rtc.collectorMonitor.Disable();
        }

        /// <summary>
        /// Initializes the 'resourceCollectionInfo' dictionary
        /// </summary>
        private void InitResourceCollectionInfo ()
        {
            collectionInfo.Clear();

            //using the inputs from the 'resourceCollectionSettings' in the inspector
            foreach (ResourceTypeCollectionSettings rtcs in collectionSettings)
            {
                if (rtcs.type == null)
                    continue;

                //add new entry for each resource type
                collectionInfo.Add(rtcs.type, new ResourceTypeCollection
                {
                    instanceCollectorsRatio = rtcs.instanceCollectorsRatio,
                    maxCollectorsRatioRange = rtcs.maxCollectorsRatioRange,
                    minCollectorsAmount = rtcs.minCollectorsAmount,
                    collectorsAmount = 0,
                    instances = new List<ResourceInfo>(),
                });

                //activate the unit regulator instnaces that can collect the current resource type
                ActivateCollectorRegulator(rtcs.type, collectionInfo[rtcs.type].collectorMonitor);
            }
        }

        /// <summary>
        /// Activates the main NPCUnitRegulator instance for the main resource collector unit.
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="collectorMonitor"></param>
        private void ActivateCollectorRegulator (ResourceTypeInfo resourceType, NpcActiveRegulatorMonitor collectorMonitor)
        {
            collectorMonitor.Init(factionMgr);

            //Go ahead and add the resource collector regulator (if there's one)..
            foreach (BattleUnitId collectorId in collectors)
            {
                // Assert.IsNotNull(collector, 
                //     $"[NPCResourceCollector] NPC Faction ID: {factionMgr.FactionId} 'Collectors' list has some unassigned elements.");

                NpcUnitRegulator nextRegulator = null;
                BattleUnitBase battleUnitBase=BattleUnitBaseFactory.Instance.GetBattleUnitLocally(collectorId);
                //as soon a collector prefab produces a valid unit regulator instance (matches the faction type and faction npc manager), add it to monitor component
                if (battleUnitBase.GetComponent<ResourceCollector>().CanCollectResourceType(resourceType, false) //also make sure the resource collector can collect this resource type.
                    && (nextRegulator = npcCommander.GetNpcComp<NpcUnitCreator>().ActivateUnitRegulator(collectorId)) != null)
                    collectorMonitor.Add(nextRegulator.battleUnitId);
            }

            Assert.IsTrue(collectorMonitor.GetCount() > 0, 
                $"[NPCBuildingConstructor] NPC Faction ID: {factionMgr.FactionId} doesn't have a resource collector regulator assigned for resource type: {resourceType.Key}!");
        }
        #endregion

        #region Resource Collection Manipulation
        /// <summary>
        /// Adds a new resource instance so that its collection is handled by the NPCResourceCollector instance.
        /// </summary>
        /// <param name="resourceInfo">The Resource instance whose collection will be regulated.</param>
        public void AddResourceToCollect (ResourceInfo resourceInfo)
        {
            //look for the resource type collection instance that regulates the resource's type
            if(collectionInfo.TryGetValue(resourceInfo.GetResourceType(), out ResourceTypeCollection rtc))
                if (!rtc.instances.Contains(resourceInfo)) //only if the resource hasn't been already added.
                    rtc.instances.Add(resourceInfo);
        }

        /// <summary>
        /// Removes a resource instance so that its collection is no longer handled by the NPCResourceCollector instance.
        /// </summary>
        /// <param name="resourceInfo">The Resource instance whose collection will no longer be regulated.</param>
        public void RemoveResourceToCollect (ResourceInfo resourceInfo)
        {
            //look for the resource type collection instance that regulates the resource's type
            if (collectionInfo.TryGetValue(resourceInfo.GetResourceType(), out ResourceTypeCollection rtc))
                rtc.instances.Remove(resourceInfo);
        }
        #endregion

        #region Unit/Resource Event Callbacks
        /// <summary>
        /// Called when a unit is created.
        /// </summary>
        /// <param name="unit">The Unit instance that has been created.</param>
        private void OnUnitCreated(BattleUnitBase unit)
        {
            //if this unit belongs to this faction & it has a resource collector comp:
            if (unit.factionId == factionMgr.FactionId && unit.resCollectorComp)
                Activate();
        }

        /// <summary>
        /// Called when a unit stops collecting a resource.
        /// </summary>
        /// <param name="unit">The Unit instance that stopped collecting.</param>
        /// <param name="resourceInfo">The Resource instance that was being collected by the unit.</param>
        private void OnUnitStopCollecting(BattleUnitBase unit, ResourceInfo resourceInfo)
        {
            if (unit.factionId == factionMgr.FactionId //if the unit belongs to this faction, check the resource collection info responsible for the resource type
                && collectionInfo.TryGetValue(resourceInfo.GetResourceType(), out ResourceTypeCollection rtc))
            {
                //activate the component so that it checks for resource collectors to send to this resource
                Activate();

                rtc.collectorsAmount--; //decrement collectors amount
                if (rtc.collectorsAmount < 0) //make sure that the collectors amount is always valid
                    rtc.collectorsAmount = 0;
            }
        }

        /// <summary>
        /// Called when a unit is ordered to collect a resource.
        /// </summary>
        /// <param name="unit">The Unit instance that has been ordered to collect.</param>
        /// <param name="resourceInfo">The Resource instance that the unit is ordered to collect.</param>
        private void OnUnitCollectionOrder(BattleUnitBase unit, ResourceInfo resourceInfo)
        {
            if (unit.factionId == factionMgr.FactionId //if the unit belongs to this faction, check the resource collection info responsible for the resource type
                && collectionInfo.TryGetValue(resourceInfo.GetResourceType(), out ResourceTypeCollection rtc))
            {
                rtc.collectorsAmount++; //increment collectors amount
            }
        }

        /// <summary>
        /// Called when a resource is empty.
        /// </summary>
        /// <param name="resourceInfo">The Resource instance that is now empty</param>
        private void OnResourceEmpty(ResourceInfo resourceInfo)
        {
            if(resourceInfo.FactionId == factionMgr.FactionId) //if this resource was inside this npc faction's territory
                RemoveResourceToCollect(resourceInfo);
        }
        #endregion

        #region Resource Collection
        /// <summary>
        /// Handles the resource collection timer
        /// </summary>
        protected override void OnActiveUpdate()
        {
            base.OnActiveUpdate();

            //resource collection timer:
            if (collectionTimer > 0)
                collectionTimer -= Time.deltaTime;
            else
            {
                //reload timer
                collectionTimer = collectionTimerRange.getRandomValue();

                Deactivate(); //assume that all resource instances have the required amount of collectors

                //go through the ResourceTypeCollection instances that regulate each resource type
                foreach (ResourceTypeCollection rtc in collectionInfo.Values)
                {
                    /*ResourceTypeCollection rtc = test.Value;
                    if (factionMgr.FactionID == 1 && test.Key.GetName() == "Metal")
                    {
                        Debug.Log(rtc.CanAddCollector(npcMgr.GetNPCComp<NPCUnitCreator>().GetActiveUnitRegulator(collectorMonitor.GetRandomCode()).Count));
                        Debug.Log(rtc.collectorsAmount);
                    }*/

                    if (!rtc.CanAddCollector(npcCommander.GetNpcComp<NpcUnitCreator>().GetActiveUnitRegulator(rtc.collectorMonitor.GetRandomCode()).Count))
                        continue; //next resource type

                    Activate(); //set state back to active so we can keep monitoring whether resource collectors have been correctly assigned or not.

                    rtc.instances.OrderByDescending(x => Vector3.Distance(x.transform.position, factionMgr.basePos));//按基地距离排序
                    //go through all resource instances of the current type
                    foreach(ResourceInfo resource in rtc.instances)
                    {
                        int targetCollectorsAmount = rtc.GetTargetCollectorsAmount(resource);

                        //does the resource still need collectors?
                        if (targetCollectorsAmount > resource.workerManager.currentWorker)
                        {
                            //send resource collectors for this one.
                            OnResourceCollectionRequest(resource, targetCollectorsAmount, true, false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Used to request to send an amount of resource collectors to collect a resource.
        /// </summary>
        /// <param name="resourceInfo">The Resource instance to collect from.</param>
        /// <param name="targetCollectorsAmount">How many collectors to send?</param>
        /// <param name="auto">True if this has been called from the NPCResourceCollector component instance, false otherwise.</param>
        /// <param name="force">Ignore other requirements and force sending collectors to resource?</param>
        //Method used by other components to request to collect a certain resource:
        public void OnResourceCollectionRequest(ResourceInfo resourceInfo, int targetCollectorsAmount, bool auto, bool force)
        {
            if (resourceInfo == null //if the resource is invalid
                || resourceInfo.IsEmpty //or it's empty
                || (auto == false && collectOnDemand == false) //or if this was requested by another component and we don't allow collection on demand
                || !collectionInfo.ContainsKey(resourceInfo.GetResourceType())) //or the resource type can be collected by the NPC faction
                return;

            //how much collectors is required?
            int requiredCollectors = targetCollectorsAmount - resourceInfo.workerManager.currentWorker;

            int i = 0; //counter.
            List<BattleUnitBase> currentCollectors = npcCommander.GetNpcComp<NpcUnitCreator>().GetActiveUnitRegulator(
                collectionInfo[resourceInfo.GetResourceType()].collectorMonitor.GetRandomCode()).GetIdleUnitsFirst(); //get the list of the current faction collectors.

            //while we still need collectors for the building and we haven't gone through all collectors.
            while (i < currentCollectors.Count && requiredCollectors > 0)
            {
                //is the collector currently in idle mode or do we force him to construct this building ?
                //& make sure it's not already collecting this resource.
                if (currentCollectors[i] != null 
                    && (currentCollectors[i].IsIdle() || force == true) 
                    && currentCollectors[i].resCollectorComp.GetTarget() != resourceInfo)
                {
                    //send to collect the resource:
                    currentCollectors[i].resCollectorComp.SetTarget(resourceInfo);
                    //decrement amount of required builders:
                    requiredCollectors--;
                }

                i++;
            }
        }
        #endregion
    }
}
