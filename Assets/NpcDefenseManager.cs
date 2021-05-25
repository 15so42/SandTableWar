using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RTSEngine;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
    /// Responsible for defending a NPC faction's territory.
    /// </summary>
    public class NpcDefenseManager : NpcComponent
    {
        #region Class Properties
        [SerializeField, Tooltip("Is the NPC faction allowed to defend its territory when it's under attack?")]
        private bool canDefend = true;

        //ratio of the army units that will stay during an attacking for defensing purposes.
        [SerializeField, Tooltip("Ratio of the attack units that will remain from the total amount of attack units to remain when attacking a target faction.")]
        private FloatRange defenseRatioRange = new FloatRange(0.1f,0.2f);
        /// <summary>
        /// Gets a value for the defense ratio from the defense ratio range.
        /// </summary>
        /// <returns>The defense ratio value.</returns>
        public float GetDefenseRatio () { return Mathf.Clamp(defenseRatioRange.getRandomValue(), 0.0f, 1.0f); }

        //if there's a in progress attack and this faction is attacked, do we cancel the attack?
        [SerializeField, Tooltip("Cancel an active attack with a target faction if this faction is under attack?")]
        private bool cancelAttackOnDefense = true;

        //timer that will assign defense orders for units in case the faction is in defensive mode:
        [SerializeField, Tooltip("How oft does the NPC faction decide whether it has to keep defending its territory or stop?")]
        private FloatRange cancelDefenseReloadRange = new FloatRange(3.0f, 7.0f);
        private float cancelDefenseTimer;

        private BattleUnitBase lastDefenseCenter; //the last defense center is saved here.

        //support:
        [SerializeField, Tooltip("Enable to allow a NPC unit to ask for support from units in its range when it is attacked.")]
        private bool unitSupportEnabled = true; //if enabled, then when a unit is attacked, it can ask support from in range units.
        [SerializeField, Tooltip("If Unit Support is enabled, then this is the range in which units can be called for support.")]
        private FloatRange unitSupportRange = new FloatRange(5, 10); //the actual support range.
        #endregion

        #region Initializing/Terminating
        /// <summary>
        /// Initializes the NPCDefenseManager instance, called from the NPCManager instance responsible for this component.
        /// </summary>
        /// <param name="fightingManager">GameManager instance of the current game.</param>
        /// <param name="npcCommander">NPCManager instance that manages this NPCComponent instance.</param>
        /// <param name="factionMgr">FactionManager instance of the faction that this component manages.</param>
        public override void Init(FightingManager fightingManager, NpcCommander npcCommander, FactionManager factionMgr)
        {
            base.Init(fightingManager, npcCommander, factionMgr);

            Deactivate(); //component is disabled by default

            //add event listeners:
            //CustomEvents.FactionEntityHealthUpdated += OnFactionEntityHealthUpdated;
            EventCenter.AddListener<BattleUnitBase,int,BattleUnitBase>(EnumEventType.OnFactionUnitDamaged,OnFactionEntityHealthUpdated);
        }

        /// <summary>
        /// Activates defending ability for the NPC faction.
        /// </summary>
        public override void Activate()
        {
            if(canDefend) //only allow to activate component if this is true
                base.Activate();
        }

        /// <summary>
        /// Called when the object holding this component is disabled/destroyed.
        /// </summary>
        private void OnDisable()
        {
            //stop listening to events:
            //CustomEvents.FactionEntityHealthUpdated -= OnFactionEntityHealthUpdated;
            EventCenter.RemoveListener<BattleUnitBase,int,BattleUnitBase>(EnumEventType.OnFactionUnitDamaged,OnFactionEntityHealthUpdated);
        }
        #endregion

        #region Event Callbacks
        /// <summary>
        /// Called when a faction entity (unit or building) health is updated.
        /// </summary>
        /// <param name="factionEntity">The FactionEntity instance whose health is updated.</param>
        /// <param name="value">The amount by which the health has been updated.</param>
        /// <param name="source">The FactionEntity instance that caused this health update</param>
        private void OnFactionEntityHealthUpdated (BattleUnitBase factionEntity, int value, BattleUnitBase source)
        {
            if (factionEntity.factionId != factionMgr.FactionId //the faction entity must belong to the NPC faction
                || value >= 0.0f //the health update must be a decrease one (we're looking to see if NPC entities got damaged)
                || source == null //there's a valid source that caused this health drop
                || source.factionId == factionMgr.FactionId) //the source that caused this health update must not belong to this NPC faction
                return;

            OnUnitSupportRequest(factionEntity.transform.position, source); //launch unit support request, allows other NPC units to help defend the damaged unit

            //check if the building is not actually part of an active attack at another faction
            if (!npcCommander.GetNpcComp<NpcAttackManager>().IsUnitDeployed(factionEntity as BattleUnitBase))
                //NPC faction is under attack, launch defense.
                LaunchDefense(factionEntity.transform.position, false);
        }
        #endregion

        #region Enabling/Disabling Defense Mode
        /// <summary>
        /// Updates the NPC faction defense timer
        /// </summary>
        protected override void OnActiveUpdate()
        {
            base.OnActiveUpdate();

            OnDefenseProgress();
        }

        //called when the faction is defending:
        void OnDefenseProgress ()
        {
            //defense timer:
            if (cancelDefenseTimer > 0)
                cancelDefenseTimer -= Time.deltaTime;
            else
                //if the timer is over -> defense mode is no longer required:
                StopDefense();
        }

        //a method that launches the NPC faction's
        /// <summary>
        /// Launches the NPC faction's defense mode to defend a certain position.
        /// </summary>
        /// <param name="defensePosition">The position to defend.</param>
        /// <param name="forceCapitalChange">True to allow units to defend territory that's not under the capital building even if capital building's territory is under attack, otherwise false.</param>
        public void LaunchDefense (Vector3 defensePosition, bool forceCapitalChange)
        {
            Activate(); //activate defense mode

            if (!IsActive) //failed to activate defense? do not proceed
                return;

            //reload the defense timer:
            cancelDefenseTimer = cancelDefenseReloadRange.getRandomValue();

            //Get the building that is closest to where the damage has been done.
            BattleUnitBase defenseCenter = factionMgr.GetNearestBuilding(defensePosition);

            ToggleCenterDefense(defenseCenter, true, forceCapitalChange); //enable defense for the closest building center to the defense position.

            //is the NPC faction is undergoing an active attack on another faction but it's not allowed in defense mode?
            if(cancelAttackOnDefense && npcCommander.GetNpcComp<NpcAttackManager>().IsAttacking)
                //cancel attack:
                npcCommander.GetNpcComp<NpcAttackManager>().CancelAttack();
        }

        /// <summary>
        /// Enables or disables the NPC faction to defend its territory around a building center.
        /// </summary>
        /// <param name="defensePosition">The position where the NPC faction has been attacked.</param>
        /// <param name="enable">True to enable building center defense mode, false to disable it.</param>
        /// <param name="forceCapitalChange">True to allow units to defend territory that's not under the capital building even if capital building's territory is under attack, otherwise false.</param>
        public void ToggleCenterDefense(BattleUnitBase nextDefenseCenter, bool enable, bool forceCapitalChange)
        {
            //if we're enabling center defense:
            if (enable)
            {
                //if it's the same current defense center
                if (lastDefenseCenter == nextDefenseCenter)
                    return;

                //if the last defense center wasn't the capital or it was but we're allowed to change it:
                if (lastDefenseCenter != fightingManager.GetFaction(factionMgr.FactionId).GetBaseBuilding() || forceCapitalChange)
                    //get closest capital building to attack position:
                    lastDefenseCenter = nextDefenseCenter;

                //if no valid center is assigned:
                if (lastDefenseCenter == null)
                    return; //do not continue
            }

            //go through the army units and set the defense center:
            foreach (BattleUnitBase unit in factionMgr.GetAttackUnits())
            {
                //make sure the unit is not deployed for attack:
                if (npcCommander.GetNpcComp<NpcAttackManager>().IsUnitDeployed(unit) == false)
                    //go through attack types of the unit.
                    //todo 暂时不用这么复杂的防守
                    // foreach (AttackEntity attackEntity in unit.AllAttackComp)
                    //     attackEntity.SearchRangeCenter = (enable) ? lastDefenseCenter.BorderComp : null;
                    
                    unit.SetTargetPos(lastDefenseCenter.transform.position);
                    
            }
        }

        /// <summary>
        /// Disables the NPC faction's defense mode.
        /// </summary>
        public void StopDefense()
        {
            Deactivate();

            ToggleCenterDefense(null, false, false); //stop center defense mode
            lastDefenseCenter = null;

            //send back defending units to their creator buildings
            SendBackUnits(factionMgr.GetAttackUnits());
        }
        #endregion

        #region Additional Defensive Behavior
        /// <summary>
        /// Called when a faction entity requests support from nearby NPC units
        /// </summary>
        /// <param name="position">The position where the support request has been initiated.</param>
        /// <param name="target">The target that has to be eliminated.</param>
        /// <returns>True if the support request has successfully processed, otherwise false.</returns>
        public bool OnUnitSupportRequest (Vector3 position, BattleUnitBase target)
        {
            //if the unit support feature is disabled or the target is invalid
            if (!unitSupportEnabled || target == null)
                return false; //do not proceed.

            //pick the next support range:
            float nextSupportRange = unitSupportRange.getRandomValue();

            //get attack units inside the chosen support range
            //and make sure that they do not have an active target.
            List<BattleUnitBase> inRangeAttackUnits = factionMgr.GetAttackUnits()
                .Where(unit =>
                {
                    return (unit != null 
                        && Vector3.Distance(unit.transform.position, position) <= nextSupportRange 
                        && unit.IsIdle());
                }).ToList();

            //request support by targeting the attacker.
            if (inRangeAttackUnits.Count > 0)
                fightingManager.attackManager.LaunchAttack(inRangeAttackUnits, target);

            return true;
        }

        /// <summary>
        /// Sends units back to their creator buildings if it exists, else send them back to their capital building or initial faction start position.
        /// </summary>
        /// <param name="units">IEnumerable instance of Unit instances to send back.</param>
        public void SendBackUnits (IEnumerable<BattleUnitBase> units)
        {
            //go through the units:
            foreach(BattleUnitBase u in units)
                //if the unit is valid:
                if(u != null)
                {
                    //make each unit go back to its creator building if it exists, else the faction capital
                    BattleUnitBase targetBuilding = u.creatorBuilding ? u.creatorBuilding : fightingManager.GetFaction(factionMgr.FactionId).GetBaseBuilding();
                    
                    //send unit back (it there's no capital building, send to initial camera look at position):
                    fightingManager.moveManager.Move(u, 
                        targetBuilding ? (targetBuilding as BaseBattleBuilding).spawnPos.transform.position : fightingManager.GetFaction(factionMgr.FactionId).GetBaseBuilding().transform.position,
                        targetBuilding ? targetBuilding.prop.viewDistance : 0.0f);
                }
        }
        #endregion
    }