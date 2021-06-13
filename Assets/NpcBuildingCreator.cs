using System.Collections.Generic;
using UnityEngine;

/* NpcBuildingCreator script created by Oussama Bouanani, SoumiDelRio.
 * This script is part of the Unity RTS Engine */

namespace RTSEngine
{
    /// <summary>
    // Holds all the information needed regarding an active NpcBuildingRegulator instance.
    /// </summary>
    public class ActiveBuildingRegulator
    {
        public NpcBuildingRegulator instance; //the active instance of the building regulator.
        public float spawnTimer; //spawn timer for the active building regulators.
    }

    /// <summary>
    // Holds all the information needed regarding active NpcBuildingRegulator instances that belong to one building center.
    /// </summary>
    public class BuildingCenterRegulator
    {
        public BaseBattleBuilding buildingCenter; //the building center where the whole building regulation will happen at.

        public Dictionary<BattleUnitId, ActiveBuildingRegulator> activeBuildingRegulators = new Dictionary<BattleUnitId, ActiveBuildingRegulator>(); //holds the active building regulators
    }

    /// <summary>
    /// Responsible for managing the creation of buildings for a Npc faction.
    /// </summary>
    public class NpcBuildingCreator : NpcComponent
    {
        #region Component Properties
        //The independent building regulators list are not managed by any other Npc component.
        [SerializeField, Tooltip("Buildings that this component is able to create other than the center and population buildings.")]
        private List<BattleUnitId> independentBuildings = new List<BattleUnitId>(); //buildings that can be created by this Npc component that do not include the center and population buildings.
        //when this Npc component is initialized, it goes through this list and removes buildings that do not have a NpcBuildingRegulatorData asset that matches...
        //...the Npc faction's type and Npc Manager type.

        private List<BattleUnitId> currBuildings = new List<BattleUnitId>(); //a list of buildings that can be used by the Npc faction

        private List<BuildingCenterRegulator> buildingCenterRegulators = new List<BuildingCenterRegulator>(); //a list that holds building centers and their corresponding active building regulators

        //each building count is assocoiated with its total count for the whole Npc faction (for all building centers).
        private Dictionary<BattleUnitId, int> totalBuildingsCount = new Dictionary<BattleUnitId, int>();

        //has the first building center (a building with a Border component) been initialized?
        private bool firstBuildingCenterInitialized = false;
        #endregion

        #region Initiliazing/Terminating
        /// <summary>
        /// Initializes the NpcBuildingCreator instance, called from the NpcManager instance responsible for this component.
        /// </summary>
        /// <param name="fightingManager">GameManager instance of the current game.</param>
        /// <param name="NpcCommander">NpcManager instance that manages this NpcComponent instance.</param>
        /// <param name="factionMgr">FactionManager instance of the faction that this component manages.</param>
        public override void Init(FightingManager fightingManager, NpcCommander NpcCommander, FactionManager factionMgr)
        {
            base.Init(fightingManager, NpcCommander, factionMgr);

            //start listening to the required delegate events:
            EventCenter.AddListener(EnumEventType.AllFactionsInit,OnNpcFactionInit);
            EventCenter.AddListener<BuildingBorder>(EnumEventType.OnBorderActivated,OnBorderActivated);
            EventCenter.AddListener<BuildingBorder>(EnumEventType.OnBorderDeActivated,OnBorderDeactivated);
            // CustomEvents.NpcFactionInit += OnNpcFactionInit;
            //
            // CustomEvents.BorderDeactivated += OnBorderDeactivated;
            // CustomEvents.BorderActivated += OnBorderActivated;
            //
            // CustomEvents.BuildingUpgraded += OnBuildingUpgraded;
        }

        /// <summary>
        /// Called when the object holding this component is disabled/destroyed.
        /// </summary>
        void OnDisable()
        {
            //stop listening to the delegate events:
            EventCenter.RemoveListener(EnumEventType.AllFactionsInit,OnNpcFactionInit);
            EventCenter.RemoveListener<BuildingBorder>(EnumEventType.OnBorderActivated,OnBorderActivated);
            EventCenter.RemoveListener<BuildingBorder>(EnumEventType.OnBorderDeActivated,OnBorderDeactivated);
            // CustomEvents.NpcFactionInit -= OnNpcFactionInit;
            //
            // CustomEvents.BorderDeactivated -= OnBorderDeactivated;
            // CustomEvents.BorderActivated -= OnBorderActivated;
            //
            // CustomEvents.BuildingUpgraded -= OnBuildingUpgraded;

            DestroyAllActiveRegulators();
        }

        /// <summary>
        /// Called when a Npc faction is done initializing its components.
        /// </summary>
        /// <param name="factionSlot">FactionSlot of the Npc faction.</param>
        private void OnNpcFactionInit ()
        {
            // if (factionManager != factionMgr) //different Npc faction?
            //     return;

            currBuildings.Clear();

            //clear the active unit regulator list per default:
            buildingCenterRegulators.Clear();

            firstBuildingCenterInitialized = false;

            //go through the spawned building centers and init them:
            foreach(BaseBattleBuilding buildingCenter in factionMgr.GetBuildingCenters())
                AddBuildingCenterRegulator(buildingCenter);
        }
        #endregion

        #region Active Regulator Manipulation
        /// <summary>
        /// Creates and activates a BuildingCenterRegulator instance for a newly added building center.
        /// </summary>
        /// <param name="buildingCenter">The Building center instance that to create the BuildingCenterRegulator instance for.</param>
        public void AddBuildingCenterRegulator (BaseBattleBuilding buildingCenter)
        {
            //create new entry for new building center regulator
            BuildingCenterRegulator newCenterRegulator = new BuildingCenterRegulator
            {
                buildingCenter = buildingCenter,
                activeBuildingRegulators = new Dictionary<BattleUnitId, ActiveBuildingRegulator>()
            };
            //add it to the list:
            buildingCenterRegulators.Add(newCenterRegulator);

            //activate the independent building regulators for this new center regulator if it's the first building center
            //else add all of the buildings that have been used by the Npc faction from the 'currBuildings' list.
            foreach (BattleUnitId buildingId in !firstBuildingCenterInitialized ? independentBuildings : currBuildings)
                ActivateBuildingRegulator(buildingId, newCenterRegulator);

            //if this is the first border component that has been activated => capital building:
            if (!firstBuildingCenterInitialized)
            {
                //NpcMgr.GetNpcComp<NpcTerritoryManager>().ActivateCenterRegulator(); //activate the center regulator in the territory manager.
                //NpcMgr.GetNpcComp<NpcPopulationManager>().ActivatePopulationBuildingRegulators(); //activate the population building

                firstBuildingCenterInitialized = true; //component initiated for the first time
            }
        }

        /// <summary>
        /// Disables and destroys a BuildingCenterRegulator instance that manages a building center.
        /// </summary>
        /// <param name="buildingCenter">The Building center instance whose BuildingCenterRegulator will be removed.</param>
        public void DestroyBuildingCenterRegulator (BaseBattleBuilding buildingCenter)
        {
            //remove building center regulator from list since it has been destroyed:
            int i = 0;
            while(i < buildingCenterRegulators.Count)
            {
                //if this is the center we're looking for:
                if (buildingCenterRegulators[i].buildingCenter == buildingCenter)
                {
                    //go through all active building regulators in this building center and disable them:
                    foreach (ActiveBuildingRegulator abr in buildingCenterRegulators[i].activeBuildingRegulators.Values)
                        abr.instance.Disable();

                    //remove it
                    buildingCenterRegulators.RemoveAt(i);
                    //done:
                    return;
                }
                else
                    i++; //continue looking
            }
        }

        /// <summary>
        /// Gets the BuildingCenterRegulator instance that manages the Npc faction's capital building
        /// </summary>
        /// <returns></returns>
        public BuildingCenterRegulator GetCapitalBuildingRegualtor()
        {
            //first building center regulator in the list refers to the capital:
            return buildingCenterRegulators[0];
        }

        /// <summary>
        /// Creates and activates one NpcBuildingRegulator instance for a building prefab for each building center.
        /// </summary>
        /// <param name="Building">The Building prefab for which a NpcBuildingRegulator instances will be created.</param>
        public void ActivateBuildingRegulator(BattleUnitId buildingId)
        {
            //go through the building center regulators:
            foreach (BuildingCenterRegulator bcr in buildingCenterRegulators)
                ActivateBuildingRegulator(buildingId, bcr);
        }

        /// <summary>
        /// Creates and activates a NpcBuildingRegulator instance for a building prefab for one building center.
        /// </summary>
        /// <param name="Building">The Building prefab for which a NpcBuildingRegulator instance will be created.</param>
        /// <param name="centerRegulator">The BuildingCenterRegulator instance for which the NpcBuildingRegulator instance will be created.</param>
        public NpcBuildingRegulator ActivateBuildingRegulator(BattleUnitId buildingId, BuildingCenterRegulator centerRegulator)
        {
            NpcBuildingRegulatorData data = ConfigHelper.Instance.GetNpcRegulatorData(buildingId,
                factionMgr.FactionSlot.npcDifficulty,true) as NpcBuildingRegulatorData; //get the regulator data

            if (data == null) //invalid regulator data?
                return null; //do not proceed

            //see if the building regulator is already active on the center or not
            NpcBuildingRegulator activeInstance = GetActiveBuildingRegulator(buildingId, centerRegulator);
            if(activeInstance != null)
                return activeInstance; //return the already active instance.

            //we will be activating the building regulator for the input center only
            ActiveBuildingRegulator newBuildingRegulator = new ActiveBuildingRegulator()
            {
                //create new instance
                instance = new NpcBuildingRegulator(data, buildingId, fightingManager, npcCommander, this, centerRegulator.buildingCenter),
                //initial spawning timer: regular spawn reload + start creating after value
                spawnTimer = data.GetCreationDelayTime()
            };

            //add it to the active building regulators list of the current building center.
            centerRegulator.activeBuildingRegulators.Add(buildingId, newBuildingRegulator);

            //if the building is not already in the current list of the buildings that can be used by the Npc faction, add it:
            if(!currBuildings.Contains(buildingId))
                currBuildings.Add(buildingId);

            //whenever a new regulator is added to the active regulators list, then move the building creator into the active state
            Activate();

            //return the new created instance:
            return newBuildingRegulator.instance;
        }

        /// <summary>
        /// Returns an the active NpcBuildingRegulator instance (if it exists) that manages a building type of a given code.
        /// </summary>
        /// <param name="code">The code that identifies the building type.</param>
        /// <param name="centerRegulator">The BuildingCenterRegulator instance which holds pointers to a list of NpcBuildingRegulator instances working inside the correspondant building center.</param>
        /// <returns>ActiveBuildingRegulator instance which includes a pointer to the actual NpcBuildingRegulator instance.</returns>
        public NpcBuildingRegulator GetActiveBuildingRegulator (BattleUnitId unitId, BuildingCenterRegulator centerRegulator)
        {
            //if the active building regulator instance for the sepififed building code exists, return it
            if (centerRegulator.activeBuildingRegulators.TryGetValue(unitId, out ActiveBuildingRegulator abr))
                return abr.instance;

            return null; //regulator hasn't been found.
        }

        /// <summary>
        /// Disables and removes all active NpcBuildingRegulator instances.
        /// </summary>
        public void DestroyAllActiveRegulators()
        {
            foreach (BuildingCenterRegulator bcr in buildingCenterRegulators) //go through the active regulators
                foreach (ActiveBuildingRegulator abr in bcr.activeBuildingRegulators.Values)
                    abr.instance.Disable();

            //clear the list, no references to the currently active building regulators -> garbage collector will handle deleting them
            buildingCenterRegulators.Clear();
        }

        /// <summary>
        /// Disables and removes the active NpcBuildingRegulator instance that manages a building whose code matches the given code.
        /// </summary>
        /// <param name="buildingCode">Code of the building that is being managed by the regulator to remove.</param>
        public void DestroyActiveRegulator (BattleUnitId buildingCode)
        {
            //remove the prefab associatedd with the building code from the list of buildings that can be used by the Npc faction.
            BattleUnitId prefabId = GetCapitalBuildingRegualtor().activeBuildingRegulators[buildingCode].instance.battleUnitId;
            currBuildings.Remove(prefabId);

            foreach (BuildingCenterRegulator bcr in buildingCenterRegulators) //go through the active regulators
            {
                //destroy the active instance:
                GetActiveBuildingRegulator(buildingCode, bcr)?.Disable();
                //remove from list and garbage collector will handle deleting it
                bcr.activeBuildingRegulators.Remove(buildingCode);
            }
        }
        #endregion

        #region Event Callbacks
        /// <summary>
        /// Called whenever a Border instance is activated.
        /// </summary>
        /// <param name="border">The Border instance that has been activated.</param>
        private void OnBorderActivated (BuildingBorder border)
        {
            //if the border's building belongs to this faction.
            if (border.building.factionId == factionMgr.FactionId)
                AddBuildingCenterRegulator(border.building);
        }

        /// <summary>
        /// Called whenever a Border instance is deactivated.
        /// </summary>
        /// <param name="border">The Border instance that has been deactivated.</param>
        void OnBorderDeactivated(BuildingBorder border)
        {
            //if the border's building belongs to this faction
            if (border.building.factionId == factionMgr.FactionId)
                DestroyBuildingCenterRegulator(border.building);
        }

        /// <summary>
        /// Called when a building type is upgraded.
        /// </summary>
        /// <param name="upgrade">Upgrade instance that handles the building upgrade.</param>
        /// <param name="targetID">Index of the upgrade target.</param>
        /*private void OnBuildingUpgraded(Upgrade upgrade, int targetID)
        {
            if (upgrade.Source.FactionID != factionMgr.FactionId) //only for buildings that belong to this Npc faction
                return;

            DestroyActiveRegulator(upgrade.Source.GetCode()); //remove NpcBuildingRegulator instance for upgrade source building.
            ActivateBuildingRegulator(upgrade.GetTarget(targetID) as Building); //add NpcBuildingRegulator instance for the upgrade target building.
        }*/
        #endregion

        #region Building Creation
        /// <summary>
        /// Update the building creation timer to monitor creating buildings.
        /// </summary>
        protected override void OnActiveUpdate()
        {
            base.OnActiveUpdate();

            Deactivate(); //assume that the unit creator has finished its job with the current active unit regulators.

            //go through the active building regulators
            for(int i = 0; i < buildingCenterRegulators.Count; i++)
                foreach (ActiveBuildingRegulator abr in buildingCenterRegulators[i].activeBuildingRegulators.Values)
                {
                    if (abr.instance.Data.CanAutoCreate() //make sure the building can be auto created in the first place.
                    //if the building didn't reach its max amount yet and still didn't reach its min amount.
                    //buildings are only automatically created if they haven't reached their min amount
                        && !abr.instance.HasReachedMinAmount() && !abr.instance.HasReachedMaxAmount())
                    {
                        //we are active since the min amount of one of the buildings regulated hasn't been reached
                        Activate();

                        //spawn timer:
                        if (abr.spawnTimer > 0.0f)
                            abr.spawnTimer -= Time.deltaTime;
                        else
                        {
                            //reload timer:
                            abr.spawnTimer = abr.instance.Data.GetSpawnReload();
                            //attempt to create as much as it is possible from this building:
                            OnCreateBuildingRequest(abr.instance, true, buildingCenterRegulators[i].buildingCenter);
                        }
                    }
                }
        }

        /// <summary>
        /// Searches for a NpcBuildingRegulator active instance for which a new building can be placed and constructed.
        /// </summary>
        /// <param name="buildingCode">The code that identifies the building type.</param>
        /// <param name="auto">True if the goal is to create a new instance for the building from the NpcBuildingCreator component.</param>
        /// <returns>An active instance of the building's type NpcBuildingRegulator if found, otherwise null.</returns>
        public NpcBuildingRegulator GetValidBuildingRegulator (BattleUnitId buildingCode, bool auto)
        {
            //go through the building center regulators
            foreach (BuildingCenterRegulator bcr in buildingCenterRegulators)
                //see if this building center has an active regulator instance of the requested type
                if (bcr.activeBuildingRegulators.TryGetValue(buildingCode, out ActiveBuildingRegulator abr))
                    //make sure that we're either automatically going to create the new building or the building can be created when requested by other Npc components
                    if ((auto || abr.instance.Data.CanCreateOnDemand())
                        //and make sure that the regulator instance hasn't reached the max allowed amount
                        && !abr.instance.HasReachedMaxAmount())
                        return abr.instance;

            return null; //no active valid instance found..
        }

        /// <summary>
        /// Launches the next building creation task on the NpcBuildingPlacer component if all requirements are met.
        /// </summary>
        /// <param name="code">The code that identifies the building type to create.</param>
        /// <param name="auto">True if this has been called from the NpcBuildingCreator component, false if called from another Npc component.</param>
        /// <param name="buildingCenter">The Building center instance that will have the next building created inside its territory</param>
        public bool OnCreateBuildingRequest(BattleUnitId unitId, bool auto, BaseBattleBuilding buildingCenter = null)
        {
            return OnCreateBuildingRequest(GetValidBuildingRegulator(unitId, auto), auto, buildingCenter);
        }

        /// <summary>
        /// Launches the next building creation task on the NpcBuildingPlacer component if all requirements are met.
        /// </summary>
        /// <param name="instance">The NpcBuildingRegulator instance that will be creating the next building.</param>
        /// <param name="auto">True if this has been called from the NpcBuildingCreator component, false if called from another Npc component.</param>
        /// <param name="buildingCenter">The Building center instance that will have the next building created inside its territory</param>
        public bool OnCreateBuildingRequest(NpcBuildingRegulator instance, bool auto, BaseBattleBuilding buildingCenter = null)
        {
            //active instance is invalid
            if (instance == null 
                //can't create the building by requests from other Npc components
                || (!auto && !instance.Data.CanCreateOnDemand()) 
                //maximum amount has been already reached
                || instance.HasReachedMaxAmount() )
                return false; //do not proceed

            //check if faction doesn't have enough resources to place the chosen prefab above.
            if (!factionMgr.BattleResMgr.HasRequiredResources(ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(instance.battleUnitId).requiredResource ))
            {
                //FUTURE FEATURE -> NO RESOURCES FOUND -> ASK FOR SOME.
                return false;
            }
            /*else if(!RTSHelper.TestFactionEntityRequirements(instance.Prefab.FactionEntityRequirements, factionMgr))
            {
                //FUTURE FEATURE -> NOT ALL FACTION ENTITIES ARE SPAWNED -> ASK TO CREATE THEM
                return false;
            }*/

            //if the building center hasn't been chosen and no building center can have the next building built inside its territory
            if(buildingCenter == null && (buildingCenter = factionMgr.GetFreeBuildingCenter(instance.battleUnitId)) == null)
            {
                //FUTURE FEATURE -> no building center is found -> request to place a building center?
                return false;
            }

            
            //all requests have been met but the placement options:
            GameObject buildAround = null; //this is the object that the building will be built around.
            float buildAroundRadius = 0.0f; //this is the radius of the build around object if it exists

            //go through all the building placement option cases:
            switch(instance.Data.GetPlacementOption())
            {
                case NpcPlacementOption.aroundResource:
                    //building will be placed around a resource:
                    //get the list of the resources in the building center where the building will be placed with the requested resource name
                    List<ResourceInfo> availableResourceList = factionMgr.FindResourcesByUnitId(factionMgr.allResources,instance.Data.GetPlacementOptionInfo());
                    if (availableResourceList.Count > 0) //if there are resources found:
                    {
                        //pick one randomly:
                        int randomResourceIndex = Random.Range(0, availableResourceList.Count);
                        buildAround = availableResourceList[randomResourceIndex].gameObject;
                        buildAroundRadius = availableResourceList[randomResourceIndex].GetComponent<ResourceInfo>().GetRadius();
                    }
                    break;
                case NpcPlacementOption.aroundBuilding:
                    //building will be placed around another building
                    //get the list of the buildings that match the requested code around the building center
                    List<BattleUnitBase> availableBuildingList =  factionMgr.FindUnitsByUnitId(factionMgr.FindBuildingsInRange(buildingCenter, 10),
                        instance.Data.GetPlacementOptionInfo());

                    if (availableBuildingList.Count > 0) //if there are buildings found:
                    {
                        //pick one randomly:
                        int randomBuildingIndex = Random.Range(0, availableBuildingList.Count);
                        buildAround = availableBuildingList[Random.Range(0, availableBuildingList.Count)].gameObject;
                        buildAroundRadius = availableBuildingList[randomBuildingIndex].GetRadius();
                    }
                    break;
                default:
                    //no option?
                    buildAround = buildingCenter.gameObject; //build around building center.
                    buildAroundRadius = buildingCenter.GetRadius();
                    break;
            }

            //finally make request to place building:
            npcCommander.GetNpcComp<NpcBuildingPlacer>().OnBuildingPlacementRequest(
                instance.battleUnitId, buildAround, buildAroundRadius, buildingCenter,
                instance.Data.GetBuildAroundDistance(), instance.Data.CanRotate());

            return true;
        }
        #endregion
    }
}
