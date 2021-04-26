using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/* NPCActiveRegulatorMonitor script created by Oussama Bouanani, SoumiDelRio.
 * This script is part of the Unity RTS Engine */

namespace RTSEngine
{
    /// <summary>
    /// Responsible for tracking active instances of NPCUnitRegulator or NPCBuildingRegulaotr.
    /// </summary>
    public class NpcActiveRegulatorMonitor
    {
        private FactionManager factionMgr; //the faction manager instance to whome this instance belongs.

        private List<BattleUnitId> codes = new List<BattleUnitId>(); //faction entity codes whose NPCRegulator instances are monitored.

        /// <summary>
        /// Returns the amount of faction entity codes monitored by this component.
        /// </summary>
        /// <returns>Amount of faction entity codes monitored by this component.</returns>
        public int GetCount () { return codes.Count; } 

        /// <summary>
        /// Gets a random faction entity code that is monitored by this component.
        /// </summary>
        /// <returns>FactionEntity code.</returns>
        public BattleUnitId GetRandomCode ()
        {
            Assert.IsTrue(codes.Count > 0,
                "[NPCActiveRegulatorMonitor] No prefab codes are stored to return a random code from.");

            return codes.Count > 0 ? codes[Random.Range(0, codes.Count)] : BattleUnitId.None;
        }

        /// <summary>
        /// Gets an IEnumerable instance of all faction entity codes monitored by this component.
        /// </summary>
        /// <returns>IEnumerable instance of string values.</returns>
        public IEnumerable<BattleUnitId> GetAll ()
        {
            return codes;
        }

        /// <summary>
        /// Initializes the NPCActiveRegulatorMonitor instance.
        /// </summary>
        /// <param name="factionMgr">FactionManager instance that controls this instance.</param>
        public void Init(FactionManager factionMgr)
        {
            Assert.IsNotNull(factionMgr,
                "[NPCActiveRegulatorMonitor] Unable to initialize due to invalid FactionManager instance.");

            this.factionMgr = factionMgr;

            codes.Clear();

            //subscribe to custom events:
            // CustomEvents.UnitUpgraded += OnFactionEntityUpgraded;
            // CustomEvents.BuildingUpgraded += OnFactionEntityUpgraded;
        }

        /// <summary>
        /// Disables this component.
        /// </summary>
        public void Disable ()
        {
            //unsubscribe to custom events:
            // CustomEvents.UnitUpgraded -= OnFactionEntityUpgraded;
            // CustomEvents.BuildingUpgraded -= OnFactionEntityUpgraded;
        }

        /// <summary>
        /// Called when a unit/building is upgraded.
        /// </summary>
        /// <param name="upgrade">Upgrade instance for the upgraded faction entity.</param>
        /// <param name="targetID">Index of the upgrade faction entity target.</param>
        // private void OnFactionEntityUpgraded (Upgrade upgrade, int targetID)
        // {
        //     if(upgrade.Source.FactionID == factionMgr.FactionID) //only if the source belongs to the same NPC faction
        //         Replace(upgrade.Source.GetCode(), upgrade.GetTarget(targetID).GetCode());
        // }

        /// <summary>
        /// Removes code of a faction entity monitored by this component and replace it with a code of a faction entity whose active NPCRegulator instance(s) will be monitored by this component.
        /// </summary>
        /// <param name="oldCode">The faction entity prefab code to remove.</param>
        /// <param name="newCode">The newly added FactionEntity prefab code</param>
        public void Replace(BattleUnitId old, BattleUnitId newId)
        {
            //if the oldCode is null or empty, this means we're just adding a new code
          
            codes.Remove(old);
            codes.Add(newId);
            
        }

        public void Add(BattleUnitId value)
        {
            codes.Add(value);
        }
    }
}
