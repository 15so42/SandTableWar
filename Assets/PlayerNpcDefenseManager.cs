using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerNpcDefenseManager : PlayerNpcComp
{
    //support
    [SerializeField, Tooltip("Enable to allow a NPC unit to ask for support from units in its range when it is attacked.")]
    private bool unitSupportEnabled = true; //if enabled, then when a unit is attacked, it can ask support from in range units.
    [SerializeField, Tooltip("If Unit Support is enabled, then this is the range in which units can be called for support.")]
    private FloatRange unitSupportRange = new FloatRange(50, 100); //the actual support range.
    private void OnEnable()
    {
        EventCenter.AddListener<BattleUnitBase,int,BattleUnitBase>(EnumEventType.OnFactionUnitDamaged,OnFactionEntityHealthUpdated);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener<BattleUnitBase,int,BattleUnitBase>(EnumEventType.OnFactionUnitDamaged,OnFactionEntityHealthUpdated);
    }


    /// <summary>
    /// Called when a faction entity (unit or building) health is updated.
    /// </summary>
    /// <param name="factionEntity">The FactionEntity instance whose health is updated.</param>
    /// <param name="value">The amount by which the health has been updated.</param>
    /// <param name="source">The FactionEntity instance that caused this health update</param>
    private void OnFactionEntityHealthUpdated (BattleUnitBase factionEntity, int value, BattleUnitBase source)
    {
        if (factionEntity.factionId != playerNpcCommander.factionManager.FactionId //the faction entity must belong to the NPC faction
            || value <= 0.0f //the health update must be a decrease one (we're looking to see if NPC entities got damaged)
            || source == null //there's a valid source that caused this health drop
            || source.factionId == playerNpcCommander.factionManager.FactionId) //the source that caused this health update must not belong to this NPC faction
            return;

        OnUnitSupportRequest(factionEntity.transform.position, source); //launch unit support request, allows other NPC units to help defend the damaged unit

        //check if the building is not actually part of an active attack at another faction
        // if (!npcCommander.GetNpcComp<NpcAttackManager>().IsUnitDeployed(factionEntity as BattleUnitBase))
        //     //NPC faction is under attack, launch defense.
        //     LaunchDefense(factionEntity.transform.position, false);
    }
    
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
        List<BattleUnitBase> inRangeAttackUnits = playerNpcCommander.factionManager.GetAttackUnits()
            .Where(unit =>
            {
                return (unit != null 
                        && Vector3.Distance(unit.transform.position, position) <= nextSupportRange 
                        && unit.IsIdle());
            }).ToList();

        //request support by targeting the attacker.
        if (inRangeAttackUnits.Count > 0)
            GameManager.Instance.GetFightingManager().attackManager.LaunchAttack(inRangeAttackUnits, target);

        return true;
    }
}
