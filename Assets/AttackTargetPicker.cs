using RTSEngine;
using UnityEngine;


    [System.Serializable]
    public class AttackTargetPicker : TargetPicker<BattleUnitBase, BattleUnitBase>
    {
        [SerializeField, Tooltip("Target and attack units?")]
        private bool engageUnits = true; //can attack units?
        [SerializeField, Tooltip("Target and attack flying units?")]
        private bool engageFlyingUnits = true; //can attack flying units?
        [SerializeField, Tooltip("Target and attack buildings?")]
        private bool engageBuildings = true; //can attack buildings?

        /// <summary>
        /// Determines whether a FactionEntity instance can be picked as a valid attack target.
        /// </summary>
        /// <param name="factionEntity">FactionEntity instance to test.</param>
        /// <returns>ErrorMessage.none if the faction entity can be picked, otherwise ErrorMessage.invalidTarget.</returns>
        public override ErrorMessage IsValidTarget(BattleUnitBase factionEntity)
        {
            return (factionEntity.battleUnitType == BattleUnitType.Building && !engageBuildings)
                || (factionEntity.battleUnitType == BattleUnitType.Unit
                    && (!engageUnits || ((factionEntity as BattleUnitBase).unitMovement.airUnit && !engageFlyingUnits)))
                ? ErrorMessage.invalidTarget : base.IsValidTarget(factionEntity);
        }
    }

