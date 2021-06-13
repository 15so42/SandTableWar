using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using BattleScene.Scripts.AI;
using RTSEngine;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Responsible for managing the creation of units for a NPC faction.
/// </summary>
public class NpcUnitCreator : NpcComponent
{
    #region Class Properties

    /// <summary>
    // Holds all the information needed regarding an active unit regulator.
    /// </summary>
    private class ActiveUnitRegulator
    {
        public NpcUnitRegulator instance; //the active instance of the unit regulator.
        public float spawnTimer; //spawn timer for the active unit regulators.
    }

    [Header("特殊单位")]
    [SerializeField,
     Tooltip("Units that this component is able to create other than attack units, builders and collectors.")]
    private List<BattleUnitId>
        independentUnits =
            new List<BattleUnitId>(); //units that can be created by this NPC component that do not include attack units, builders and resource collectors
    //when this NPC component is initialized, it goes through this list and removes units that do not have a NPCUnitRegulatorData asset that matches...
    //...the NPC faction's type and NPC Manager type.

    private Dictionary<BattleUnitId, ActiveUnitRegulator> activeUnitRegulators =
        new Dictionary<BattleUnitId, ActiveUnitRegulator>(); //holds the active unit regulators

    #endregion

    #region Initiliazing/Terminating

    /// <summary>
    /// Initializes the NPCUnitCreator instance, called from the NPCManager instance responsible for this component.
    /// </summary>
    /// <param name="fightingManager">GameManager instance of the current game.</param>
    /// <param name="npcCommander">NPCManager instance that manages this NPCComponent instance.</param>
    /// <param name="factionMgr">FactionManager instance of the faction that this component manages.</param>
    public override void Init(FightingManager fightingManager, NpcCommander npcCommander, FactionManager factionMgr)
    {
        base.Init(fightingManager, npcCommander, factionMgr);

        //clear the active unit regulator list per default:
        activeUnitRegulators.Clear();

        //go through the independent units that this component is able to create:
        foreach (BattleUnitId unitId in independentUnits)
            //for each unit, get the NPCUnitRegulatorData instance that suits the current NPC type and its faction type and attempt to activate it
            ActivateUnitRegulator(unitId);

        //subscribe to custom events:
        //CustomEvents.UnitUpgraded += OnUnitUpgraded;
    }

    /// <summary>
    /// Called when the object holding this component is disabled/destroyed.
    /// </summary>
    private void OnDisable()
    {
        DestroyAllActiveRegulators();

        //subscribe to custom events:
        //CustomEvents.UnitUpgraded -= OnUnitUpgraded;
    }

    #endregion

    #region Event Callbacks

    /// <summary>
    /// Called when a unit type is upgraded.
    /// </summary>
    /// <param name="upgrade">Upgrade instance that handles the unit upgrade.</param>
    /// <param name="targetID">Index of the upgrade target.</param>
    // private void OnUnitUpgraded(Upgrade upgrade, int targetID)
    // {
    //     if (upgrade.Source.FactionID != factionMgr.FactionID) //only for units that belong to this NPC faction
    //         return;
    //
    //     DestroyActiveRegulator(upgrade.Source.GetCode()); //remove NPCUnitRegulator instance for upgrade source unit.
    //     ActivateUnitRegulator(upgrade.GetTarget(targetID) as Unit); //add NPCUnitRegulator instance for the upgrade target unit.
    // }

    #endregion

    #region Active Regulator Manipulation

    /// <summary>
    /// Creates and activates a NPCUnitRegulator instance for a unit prefab.
    /// </summary>
    /// <param name="unit">The <c>Unit</c> prefab for which NPCUnitRegulator instance will be created.</param>
    /// <returns>The created and active NPCUnitRegulator instance.</returns>
    public NpcUnitRegulator ActivateUnitRegulator(BattleUnitId battleUnitId)
    {
        NpcUnitRegulatorData data =
            ConfigHelper.Instance.GetNpcRegulatorData(battleUnitId,
                factionMgr.FactionSlot.npcDifficulty) as NpcUnitRegulatorData; //get the regulator data

        if (data == null) //invalid regulator data?
            return null; //do not proceed

        //see if the unit regulator is already active or not
        NpcUnitRegulator activeInstance = GetActiveUnitRegulator(battleUnitId);
        if (activeInstance != null) //if it is
            return activeInstance; //return the already active instance.

        ActiveUnitRegulator newUnitRegulator = new ActiveUnitRegulator()
        {
            //create new instance
            instance = new NpcUnitRegulator(data, battleUnitId, fightingManager, npcCommander,this),
            //initial spawning timer: regular spawn reload + start creating after value
            spawnTimer = data.GetCreationDelayTime()
        };

        //add it to the active unit regulators list:
        activeUnitRegulators.Add(battleUnitId, newUnitRegulator);

        //whenever a new regulator is added to the active regulators list, then move the unit creator into the active state
        Activate();
        return newUnitRegulator.instance;
    }

    /// <summary>
    /// Returns the active NPCUnitRegulator instance (if it exists) that manages a unit type of a given code
    /// </summary>
    /// <param name="code">The code that identifies the unit type.</param>
    /// <returns>Reference to the NPCUnitRegulator instance that regulates the unit type.</returns>
    public NpcUnitRegulator GetActiveUnitRegulator(BattleUnitId battleUnitId)
    {
        //if the active unit regulator instance for the sepififed building code exists, return it
        if (activeUnitRegulators.TryGetValue(battleUnitId, out ActiveUnitRegulator aur))
            return aur.instance;

        return null;
    }

    /// <summary>
    /// Disables and removes all active NPCUnitRegulator instances.
    /// </summary>
    public void DestroyAllActiveRegulators()
    {
        foreach (ActiveUnitRegulator aur in activeUnitRegulators.Values)
            aur.instance.Disable(); //disable each active instance of a unit regulator

        //clear the list, no references to the currently active unit regulators -> garbage collector will handle deleting them
        activeUnitRegulators.Clear();
    }

    /// <summary>
    /// Disables and removes the active NPCUnitRegulator instance that manages a unit whose code matches the given code.
    /// </summary>
    /// <param name="unitCode">Code of the unit that is being managed by the regulator to remove.</param>
    public void DestroyActiveRegulator(BattleUnitId battleUnitId)
    {
        //if there's a unit regulator that manages the unit with the input code, disable it
        GetActiveUnitRegulator(battleUnitId)?.Disable();
        //remove from list, lose refernce to the active NPCUnitRegulator instance -> garbage collector handles removing it
        activeUnitRegulators.Remove(battleUnitId);
    }

    #endregion

    #region Unit Creation

    /// <summary>
    /// Update the unit creation timer to monitor creating units.
    /// </summary>
    protected override void OnActiveUpdate()
    {
        base.OnActiveUpdate();

        Deactivate(); //assume that the unit creator has finished its job with the current active unit regulators.
        //go through the active unit regulators:
        foreach (ActiveUnitRegulator aur in activeUnitRegulators.Values)
        {
            //if we can auto create this:
            if (aur.instance.HasReachedMaxAmount() == false)
            {
                //we are active since the max amount of one of the units regulated hasn't been reached
                Activate();

                //spawn timer:
                if (aur.spawnTimer > 0.0f)
                    aur.spawnTimer -= Time.deltaTime;
                else
                {
                    //reload timer:
                    aur.spawnTimer = aur.instance.Data.GetSpawnReload();
                    //attempt to create as much as it is possible from this unit:
                    OnCreateUnitRequest(aur.instance, true, aur.instance.TargetCount);
                }
            }
        }
    }

    /// <summary>
    /// Launches the next unit creation task if all requirements are met.
    /// </summary>
    /// <param name="instance">The NPCUnitRegulator instance that will be creating the next unit.</param>
    /// <param name="auto">True if this has been called from the NPCUnitCreator component, false if called from another NPC component.</param>
    /// <param name="requestedAmount">The amount of unit instances requested to create.</param>
    public void OnCreateUnitRequest(NpcUnitRegulator instance, bool auto, int requestedAmount)
    {
        if (instance == null //if the instance is invalid
            //if this has been requested from another NPC component and the regulator doesn't allow it
            // || (!auto && !instance.Data.CanCreateOnDemand())
            // //or if this attempt is done automatically (from the NPC Unit Creator itself) and the regulator doesn't allow it
            // || (auto && !instance.Data.CanAutoCreate())
            //if we have raeched the maximum allowed amount or the requested amount is invalid
            || instance.HasReachedMaxAmount() || requestedAmount <= 0)
            return; //do not proceed.

        if (instance.GetTaskLauncherCount() == 0) //if there are no task launchers assigned to this regulator:
        {
            //FUTURE FEATURE: Communicate with the NPC Building Creator in order to ask to spawn one of the task launchers.
            return;
        }

        //enough task launchers that can create this unit? go through the task launchers that this unit regulator uses:
        foreach (TaskLauncher taskLauncher in instance.GetTaskLaunchers())
        {
            foreach (int taskID in instance.GetUnitCreationTasks(taskLauncher))
            {
                ErrorMessage addTaskMsg = ErrorMessage.none;

                //as long as launching the unit creation task is successful and we still have units to create
                while (addTaskMsg == ErrorMessage.none && requestedAmount > 0)
                {
                    //attempt to add new unit creation task
                    addTaskMsg = TaskManager.Instance.AddTask(new TaskInfo()
                    {
                        taskLauncher = taskLauncher,
                        ID = taskID,
                        type = TaskManager.TaskTypes.createUnit
                    });

                    //handle cases:
                    switch (addTaskMsg)
                    {
                        case ErrorMessage.none: //in case of success
                            requestedAmount--; //decrease amount required
                            break;

                        case ErrorMessage.maxPopulationReached: //in case of failure due to max population reach.
                            //ask the NPC population manager to add a new population building and stop the whole process:
                            //npcMgr.GetNPCComp<NPCPopulationManager>().OnAddPopulationRequest(false);
                            return;

                        default:
                            break; //FUTURE FEATURE: HANLDE OTHER FAILURE MESSAGES SUCH AS: sending builders to fix low health buildings, 
                        //... or asking resource manager to collect resource in case of missing resource.
                    }
                }

                //as soon we create all the required units then stop this whole thing:
                if (requestedAmount == 0)
                    return;
            }
        }
    }

    #endregion
}