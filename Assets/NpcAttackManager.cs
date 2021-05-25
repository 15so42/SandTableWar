using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



/* NPCAttackManager script created by Oussama Bouanani, SoumiDelRio.
 * This script is part of the Unity RTS Engine */

namespace RTSEngine
{
    //Types of attack strategies for NPC factions
    public enum NPCAttackTargetType {random, mostAttackResources, leastAttackResources}

    /// <summary>
    /// Responsible for picking a target faction and launching attacks for a NPC faction
    /// </summary>
    public class NpcAttackManager : NpcComponent
    {
        #region Class Properties

        //Picking an attack target:
        [SerializeField, Tooltip("Can the NPC faction attack other factions?"), Header("Picking a Target")]
        private bool canAttack = true; //can this faction attack?

        [SerializeField, Tooltip("Resource types that determine the criteria of other factions strength.")]
        private ResourceTypeInfo[] attackResources = new ResourceTypeInfo[0];

        [SerializeField, Tooltip("What's the NPC faction's strategy when picking a target faction?")]
        private NPCAttackTargetType targetFactionType = NPCAttackTargetType.random;

        [SerializeField, Tooltip("Delay time before picking a target faction?")]
        private FloatRange setTargetFactionDelay = new FloatRange(10, 15); //target faction will only be set after this delay is done
        private float setTargetFactionTimer;

        private FactionManager targetFaction; //the faction manager component of the target faction.

        //Launching attacks:
        //timer at which the faction decides to attack target faction or not:
        [SerializeField, Tooltip("How often does the NPC faction thinks about whether to launch an attack or not?"), Header("Launching Attacks")]
        private FloatRange launchAttackReloadRange = new FloatRange(10.0f, 15.0f);
        private float launchAttackTimer;

        public bool IsAttacking { private set; get; } //is the NPC faction currently engaging in attack towards its target faction?

        //the launch attack power is required to have as the current attack power in order to launch an attack on another faction.
        [SerializeField, Tooltip("Minimum amount of resource types required to launch an attack.")]
        private BattleResMgr.ResourceInputRange[] launchAttackResources = new BattleResMgr.ResourceInputRange[0];

        //Attacking
        //whenever the below timer is through, this component will point army units to a target.
        [SerializeField, Tooltip("How often does the NPC faction pick the next target unit/building to attack while engaging an enemy faction?"), Header("Attacking")]
        private FloatRange attackOrderReloadRange = new FloatRange(3.0f, 7.0f);
        private float attackOrderTimer;

        [SerializeField, Tooltip("Only send attack units that are idle (and not performing another task at the time of the attack launch) to attack?")]
        private bool sendOnlyIdle = true;

        private Vector3 lastAttackPos; //the last position of the target building in the attack

        //list of units that will participate in the attack:
        private List<BattleUnitBase> currentAttackUnits = new List<BattleUnitBase>();

        //a list of the buildings/units codes that this faction will attempt to attack:
        [SerializeField, Tooltip("Define the buildings/units that the NPC faction will specifically target using their code and/or categories.")]
        private AttackTargetPicker targetPicker = new AttackTargetPicker();

        private BattleUnitBase currentTarget; //the current faction entity that this faction is attempting to destroy.

        //when the faction's army attack power goes below this value while the faction attacking another one, then a retreat will take place:
        [SerializeField, Tooltip("If the faction has one of the following resource types under the specified amount, the faction will stop its active attack")]
        private BattleResMgr.ResourceInputRange[] cancelAttackResources = new BattleResMgr.ResourceInputRange[0];
        #endregion

        #region Initializing/Terminating
        /// <summary>
        /// Initializes the NPCAttackManager instance, called from the NPCManager instance responsible for this component.
        /// </summary>
        /// <param name="fightingMgr">GameManager instance of the current game.</param>
        /// <param name="npcCommander">NPCManager instance that manages this NPCComponent instance.</param>
        /// <param name="factionMgr">FactionManager instance of the faction that this component manages.</param>
        public override void Init(FightingManager fightingMgr, NpcCommander npcCommander, FactionManager factionMgr)
        {
            base.Init(fightingMgr, npcCommander, factionMgr);

            //if we can't attack then disable this component:
            if (canAttack == false)
                Deactivate();
            else
                Activate();

            IsAttacking = false;
            targetFaction = null; //no target initially

            //start timers:
            setTargetFactionTimer = setTargetFactionDelay.getRandomValue(); //start the set attack target timer.
            launchAttackTimer = launchAttackReloadRange.getRandomValue();

            //start listening to events:
            // CustomEvents.UnitDead += OnUnitDead;
            // CustomEvents.UnitConversionComplete += OnUnitConverted;
            // CustomEvents.FactionEliminated += OnFactionEliminated;
            
            EventCenter.AddListener<BattleUnitBase>(EnumEventType.UnitDied,OnUnitDead);
            //还差阵营被击败事件
        }

        /// <summary>
        /// Called when the object holding this component is disabled/destroyed.
        /// </summary>
        private void OnDestroy()
        {
            //stop listening to events:
            // CustomEvents.UnitDead -= OnUnitDead;
            // CustomEvents.UnitConversionComplete -= OnUnitConverted;
            // CustomEvents.FactionEliminated -= OnFactionEliminated;
            EventCenter.RemoveListener<BattleUnitBase>(EnumEventType.UnitDied,OnUnitDead);
        }
        #endregion

        #region Event Callbacks
        /// <summary>
        /// Called when a unit is dead.
        /// </summary>
        /// <param name="unit">The Unit instance that died.</param>
        private void OnUnitDead(BattleUnitBase unit)
        {
            //if the unit is in the current attack units list, it will be removed:
            if (unit.factionId == factionMgr.FactionId)
                currentAttackUnits.Remove(unit);
        }

        /// <summary>
        /// Called when a unit is converted.
        /// </summary>
        /// <param name="converter">The Unit instance that launched the conversion.</param>
        /// <param name="target">The Unit instance that has been convereted.</param>
        private void OnUnitConverted(BattleUnitBase converter, BattleUnitBase target)
        {
            //if the unit is in the current attack units list, it will be removed:
            currentAttackUnits.Remove(target);
        }

        /// <summary>
        /// Called whenever a faction is eliminated.
        /// </summary>
        /// <param name="factionInfo">The FactionSlot instance of the eliminated faction.</param>
        private void OnFactionEliminated(FactionManager factionManager)
        {
            //if this is the current target faction?
            if (factionManager == targetFaction)
                CancelAttack(); //cancel the attack
        }
        #endregion

        #region Updating Timers
        /// <summary>
        /// Runs the timers when the NPC faction is actively attacking/searching for a target.
        /// </summary>
        protected override void OnActiveUpdate()
        {
            base.OnActiveUpdate();

            OnSetTargetUpdate();

            OnLaunchAttackUpdate();

            OnActiveAttackUpdate();
        }
        #endregion

        #region Picking Target Faction
        /// <summary>
        /// Runs the timer to set the NPC faction's next target faction.
        /// </summary>
        //a method that runs the set target faction timer in order to assign a new target faction
        private void OnSetTargetUpdate()
        {
            //if there's a target already assigned or we're still in peace time, do not continue
            if (targetFaction != null || fightingManager.InPeaceTime())
                return;

            //updating the attack target timer:
            if (setTargetFactionTimer > 0)
                setTargetFactionTimer -= Time.deltaTime;
            else
            {
                setTargetFactionTimer = setTargetFactionDelay.getRandomValue(); //reload the set attack target timer.
                SetTargetFaction(); //find a target faction.
            }
        }

        /// <summary>
        /// A method that picks the NPC's faction target faction from the available uneliminated factions.
        /// </summary>
        public void SetTargetFaction()
        {
            //first get the factions that are not yet defeated and order them by their attack reource amount
            FactionManager[] activeFactions = fightingManager.GetFactions()
                .Where(faction => !faction.isLost && faction != factionMgr)
                .OrderByDescending(faction => attackResources.Sum(resource => fightingManager.GetFaction(faction.FactionId).BattleResMgr.GetRemainingResByResType(resource.resourceType))).ToArray();

            //depending on the strategy of the NPC faction's towards picking a target faction, pick one
            switch (targetFactionType)
            {
                case NPCAttackTargetType.random:
                    targetFaction = activeFactions[Random.Range(0, activeFactions.Length)];
                    break;

                case NPCAttackTargetType.mostAttackResources:
                    targetFaction = activeFactions[0];
                    break;

                case NPCAttackTargetType.leastAttackResources:
                    targetFaction = activeFactions[activeFactions.Length - 1];
                    break;
            }
        }

        /// <summary>
        /// Set the NPC faction's target faction to the one given in the 'newTarget' parameter
        /// </summary>
        /// <param name="newTarget">The FactionManager instance that manages the new target faction.</param>
        /// <param name="launchAttack">Should the NPC faction instantly launch its attack towards the new faction?</param>
        /// <returns>True if the new target has been successfully set, otherwise false.</returns>
        public bool SetTargetFaction(FactionManager newTarget, bool launchAttack)
        {
            //if the new target faction is invalid, it's already eliminated, it's this faction or already set as the target faction, do nada.
            if (newTarget == null
                || newTarget == factionMgr
                || fightingManager.GetFaction(newTarget.FactionId).isLost
                || newTarget == targetFaction)
                return false;

            CancelAttack(); //cancel current attack
            targetFaction = newTarget; //set the new target faction.
            if (launchAttack) //if we're supposed to directly launch an attack, do it
                LaunchAttack();

            return true;
        }
        #endregion

        #region Launching Attacks
        /// <summary>
        /// Updates the NPC faction's launch attack timer, delay before the NPC faction engages its target faction.
        /// </summary>
        private void OnLaunchAttackUpdate()
        {
            if (IsAttacking //if the NPC faction is already attacking
                || targetFaction == null //or it has no target faction 
                || npcCommander.GetNpcComp<NpcDefenseManager>().IsActive) //or it's currently defending its territory
                return; //halt attack update

            //launch attack timer:
            if (launchAttackTimer > 0)
                launchAttackTimer -= Time.deltaTime;
            else
            {
                //reload timer:
                launchAttackTimer = launchAttackReloadRange.getRandomValue();

                //does the NPC faction has enough attacking power to launch attack?
                foreach (BattleResMgr.ResourceInputRange resource in launchAttackResources) //go through the attack launch required resources
                    if (!fightingManager.GetFaction(factionMgr.FactionId).BattleResMgr.HasEnoughRes(resource.resType,resource.Amount.getRandomValue())) //if the NPC faction has less than the required amount for one of the resource types, do not launch attack
                        return;

                //launch attack.
                LaunchAttack();
            }
        }

        /// <summary>
        /// Launches an attack on the NPC faction's current target faction.
        /// </summary>
        /// <returns>True if the attack has been successfully launched, otherwise false.</returns>
        public bool LaunchAttack()
        {
            //making sure there's a valid target faction or we're in the peace time
            if (targetFaction == null || fightingManager.InPeaceTime())
                return false;

            //mark as attacking:
            IsAttacking = true;

            RefreshCurrentAttackUnits();

            //we'll be searching for the next building to attack starting from the last attack pos, initially set it as the capital building
            lastAttackPos = fightingManager.GetFaction(factionMgr.FactionId).basePos;

            currentTarget = null;
            //pick a target building:
            SetTargetEntity(targetFaction.GetBuildings(), true);

            //start the attack order timer:
            attackOrderTimer = attackOrderReloadRange.getRandomValue();

            return true;
        }

        /// <summary>
        /// Refreshes the list of the units that this component can send to engage an enemy unit/building.
        /// </summary>
        public void RefreshCurrentAttackUnits ()
        {
            //clear the current attack units list:
            currentAttackUnits.Clear();
            currentAttackUnits.AddRange(factionMgr.GetAttackUnits(1 - npcCommander.GetNpcComp<NpcDefenseManager>().GetDefenseRatio())); //get the required units for this attack.
        }
        #endregion

        #region Picking Faction Entity Target
        /// <summary>
        /// Sets the current target of the NPC faction to the closest from an enumerable of FactionEntity instances.
        /// </summary>
        /// <param name="factionEntities">The enumerable of FactionEntity instances to pick a target from.</param>
        /// <param name="clearCurrentTarget">False if you want to keep the current target if no valid target is found.</param>
        /// <returns>True if the target has been successfully set, otherwise false.</returns>
        public bool SetTargetEntity(IEnumerable<BattleUnitBase> factionEntities, bool clearCurrentTarget)
        {
            if (clearCurrentTarget)
                ResetCurrentTarget();

            //search the target faction's entities and see if there's a match:
            float lastDistance = 0; //we wanna get the closest entity to the last attack position:

            foreach (BattleUnitBase entity in factionEntities)
            {
                if (!IsValidTarget(entity)) //can't be a valid target?
                    continue; //move to next one

                //get the closest possible entity
                if (currentTarget == null || Vector3.Distance(currentTarget.transform.position, lastAttackPos) < lastDistance)
                {
                    currentTarget = entity;
                    lastDistance = Vector3.Distance(entity.transform.position, lastAttackPos);
                }
            }

            return currentTarget != null;
        }

        /// <summary>
        /// Sets the current target of the NPC faction to the input target.
        /// </summary>
        /// <param name="nextTarget">The FactionEntity instance that will be the next target.</param>
        /// <param name="clearCurrentTarget">False if you want to keep the current target if no valid target is found.</param>
        /// <returns>True if the target has been successfully set, otherwise false.</returns>
        public bool SetTargetEntity(BattleUnitBase nextTarget, bool clearCurrentTarget)
        {
            if (clearCurrentTarget)
                ResetCurrentTarget();

            if (!IsValidTarget(nextTarget)) //can't be a valid target?
                return false;

            currentTarget = nextTarget;

            return true;
        }

        /// <summary>
        /// Tests whether a FactionEntity can be a potential target for the NPC faction.
        /// </summary>
        /// <param name="potentialTarget">The FactionEntity instance to test.</param>
        /// <returns>True if the potential target can be assigned as a target, otherwise false.</returns>
        public bool IsValidTarget(BattleUnitBase potentialTarget)
        {
            return potentialTarget != null //must be valid
                && potentialTarget.IsAlive() //must be still alive
                && targetFaction != null //there must be a target faction
                && potentialTarget.factionId == targetFaction.FactionId //factionID must match the target faction's ID
                                                                        //if the entity can be targeted by the NPC faction or this is an eliminate all game:
                && (fightingManager.GetDefeatCondition() == DefeatConditions.eliminateAll || targetPicker.IsValidTarget(potentialTarget) == ErrorMessage.none);
        }

        /// <summary>
        /// Resets the currentTarget FactionEntity to null.
        /// </summary>
        public void ResetCurrentTarget()
        {
            currentTarget = null;
        }
        #endregion

        #region Active Engagement Management
        /// <summary>
        /// Updates the component while it's actively engaging in an attack
        /// </summary>
        private void OnActiveAttackUpdate()
        {
            if (!IsAttacking //not really actively attacking?
                || targetFaction == null //no target faction assigned?
                || fightingManager.InPeaceTime()) //or still in peace time?
                return;

            //attack order timer:
            if (attackOrderTimer > 0)
                attackOrderTimer -= Time.deltaTime;
            else
            {
                //reload attack order timer:
                attackOrderTimer = attackOrderReloadRange.getRandomValue();

                if (currentAttackUnits.Count == 0) //if there are no more units in the attacking squad? stop attacking
                {
                    CancelAttack();
                    return;
                }

                //did the current attack power hit the surrender resource type amount?
                foreach (BattleResMgr.ResourceInputRange resource in cancelAttackResources)
                    if (!fightingManager.GetFaction(factionMgr.FactionId).BattleResMgr.HasEnoughRes(resource.resType,resource.Amount.getRandomValue()))
                    {
                        CancelAttack(); //Cancel the attack and do not proceed.
                        return; 
                    }

                //if it doesn't have a current target yet, start by search for a building to attack, if none is found then look for a target unit (if defeat condition is set to eliminate all)
                if (currentTarget == null && SetTargetEntity(targetFaction.GetBuildings(), false) == false
                                          && fightingManager.GetDefeatCondition() == DefeatConditions.eliminateAll)
                {
                    if (SetTargetEntity(targetFaction.GetUnits(), false)==false)
                    {
                        //如果找到中立阵营，在特殊情况下可能既没有建筑也没有敌人，单中立阵营并没有lost的概念，所以需要无视此次攻击寻找
                        CancelAttack();
                        return;
                    }
                }
                    

                EngageCurrentTarget();
            }
        }

        /// <summary>
        /// Check whether a unit is deployed for the attack of a target faction or not.
        /// </summary>
        /// <param name="unit">The Unit instance to test.</param>
        /// <returns>True if the unit is part of the units that are engaging in the attack, otherwise false.</returns>
        public bool IsUnitDeployed (BattleUnitBase unit)
        {
            return unit != null && currentAttackUnits.Contains(unit);
        }

        /// <summary>
        /// A method that stops the NPC faction from attacking and resets its targets.
        /// </summary>
        public void CancelAttack ()
        {
            //send back units:
            npcCommander.GetNpcComp<NpcDefenseManager>().SendBackUnits(currentAttackUnits);

            //clear the current attack units:
            currentAttackUnits.Clear();

            currentTarget = null; //reset the target.

            targetFaction = null;

            //stop attacking:
            IsAttacking = false;
        }

        /// <summary>
        /// Orders the NPC faction to engage its currently set target.
        /// </summary>
        public void EngageCurrentTarget ()
        {
            //what units is the NPC faction sending to attack?
            List<BattleUnitBase> nextAttackUnits = sendOnlyIdle ? currentAttackUnits.Where(unit => unit.IsIdle()).ToList() : currentAttackUnits;

            if (!IsAttacking //not actively attacking?
                || currentTarget == null //has a current target?
                || targetFaction == null //no target faction assigned?
                || fightingManager.InPeaceTime() //or still in peace time?
                || nextAttackUnits.Count == 0) //or no more attack units?
                return;

            //如果建筑正在建造则攻击正在建造这种建筑的工人
            // //if the current target is a building and it is being constructed:
            // if(currentTarget.battleUnitType == BattleUnitType.Building && (currentTarget as Building).WorkerMgr.currWorkers > 0)
            // {
            //     Unit[] workersList = (currentTarget as Building).WorkerMgr.GetAll(); //get all workers in the worker manager
            //     //attack the workers first: go through the workers positions
            //     for (int i = 0; i < workersList.Length; i++)
            //         //find worker:
            //         if (workersList[i] != null && workersList[i].BuilderComp.InProgress == true)
            //             //assign it as target.
            //             currentTarget = workersList[i];
            // }

            //launch the actual attack:
            fightingManager.attackManager.LaunchAttack(
                //if only idle units can be sent to attack, make sure this is the case.
                nextAttackUnits
                , currentTarget);
        }
        #endregion
    }
}
