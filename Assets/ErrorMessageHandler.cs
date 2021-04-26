using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSEngine
{
    public enum ErrorMessage
    {
        none,
        invalid,
        inactive,
        uninteractable,
        blocked,

        notLocalPlayer,
        requestRelayed,

        notMovable, targetPositionNotFound, positionReserved, positionOccupied, invalidMvtPath,

        invalidTarget,

        searchCellNotFound, searchTargetNotFound,

        underAttack,

        unitGroupSet, unitGroupEmpty, unitGroupSelected,

        targetDifferentFaction, targetSameFaction, noFactionAccess,

        targetNoConversion, targetNoAttack,

        sourceDead, targetDead, targetMaxHealth, sourceLowHealth,

        lowResources,

        targetMaxWorkers, targetMaxCapacityReached, sourceMaxCapacityReached, targetEmpty,

        targetOutsideTerritory,

        targetPortalMissing,

        entityNotAllowed,

        buildingNotBuilt, buildingNotPlaced,

        peaceTime,

        attackTypeLocked, attackTypeNotFound, attackInCooldown, attackTargetNoChange, attackTargetRequired, attackTargetOutOfRange, canNotAttack, attackPositionNotFound, attackPositionOutOfRange,
        moveToTargetNoAttack,

        dropoffBuildingMissing,

        componentDisabled,

        sourceUpgrading,

        maxPopulationReached,

        factionLimitReached,
        alreadyInAttackPosition,
    }

    public class ErrorMessageHandler : MonoBehaviour
    {
        //other components:
        static GameManager gameMgr;

        public void Init(GameManager gameMgrIns)
        {
            gameMgr = gameMgrIns;
        }

        public static void OnErrorMessage (ErrorMessage message, Entity source, Entity target = null)
        {
            switch (message)
            {
                case ErrorMessage.invalid:
                    TipsDialog.ShowDialog("Invalid target.");
                    break;
                case ErrorMessage.inactive:
                    TipsDialog.ShowDialog("Inactive target.");
                    break;
                case ErrorMessage.uninteractable:
                    TipsDialog.ShowDialog("Unteractable target.");
                    break;


                case ErrorMessage.blocked:
                    //TipsDialog.ShowDialog("Faction Entity blocked.", UIManager.MessageTypes.info);
                    break;

                case ErrorMessage.unitGroupSet:
                    TipsDialog.ShowDialog("Unit group assigned.");
                    break;
                case ErrorMessage.unitGroupSelected:
                    TipsDialog.ShowDialog("Unit group selected.");
                    break;
                case ErrorMessage.unitGroupEmpty:
                    TipsDialog.ShowDialog("Unit group empty.");
                    break;
               
                case ErrorMessage.targetDifferentFaction:
                    TipsDialog.ShowDialog("The target doesn't belong to your faction!");
                    break;
                case ErrorMessage.targetSameFaction:
                    TipsDialog.ShowDialog("The target belongs to your faction!");
                    break;
                case ErrorMessage.noFactionAccess:
                    TipsDialog.ShowDialog("Your faction doesn't have access!");
                    break;

                case ErrorMessage.targetNoConversion:
                    TipsDialog.ShowDialog("The target can't be converted!");
                    break;
                case ErrorMessage.targetNoAttack:
                    TipsDialog.ShowDialog("The target can't be attacked!");
                    break;
                    
                case ErrorMessage.sourceDead:
                    TipsDialog.ShowDialog("The source is dead!");
                    break;
                case ErrorMessage.targetDead:
                    TipsDialog.ShowDialog("The target is dead!");
                    break;
                case ErrorMessage.targetMaxHealth:
                    TipsDialog.ShowDialog("Your target has reached maximum health!");
                    break;
                case ErrorMessage.sourceLowHealth:
                    TipsDialog.ShowDialog("The source has low health!");
                    break;

                case ErrorMessage.lowResources:
                    TipsDialog.ShowDialog("Not enough resources");
                    break;

                case ErrorMessage.targetMaxWorkers:
                    TipsDialog.ShowDialog("The target has maximum workers!");
                    break;
                case ErrorMessage.targetMaxCapacityReached:
                    TipsDialog.ShowDialog("The target has reached maximum capacity!");
                    break;
                case ErrorMessage.sourceMaxCapacityReached:
                    TipsDialog.ShowDialog("The source has reached maximum capacity!");
                    break;
                case ErrorMessage.targetEmpty:
                    TipsDialog.ShowDialog("The target is empty!");
                    break;

                case ErrorMessage.targetOutsideTerritory:
                    TipsDialog.ShowDialog("The target is outside your faction's territory!");
                    break;

                case ErrorMessage.targetPortalMissing:
                    TipsDialog.ShowDialog("The target portal is missing!");
                    break;

                case ErrorMessage.entityNotAllowed:
                    TipsDialog.ShowDialog("This entity is not allowed!");
                    break;

                case ErrorMessage.buildingNotBuilt:
                    TipsDialog.ShowDialog("The building is not built yet!");
                    break;
                case ErrorMessage.buildingNotPlaced:
                    TipsDialog.ShowDialog("This building is not placed!");
                    break;

                case ErrorMessage.peaceTime:
                    TipsDialog.ShowDialog("Can't attack in peace time!");
                    break;

                case ErrorMessage.attackInCooldown:
                    TipsDialog.ShowDialog("Attack is currently in cooldown!");
                    break;
                case ErrorMessage.attackTargetNoChange:
                    TipsDialog.ShowDialog("Attack target can't be changed!");
                    break;
                case ErrorMessage.attackTargetRequired:
                    TipsDialog.ShowDialog("Attack target can't be changed!");
                    break;
                case ErrorMessage.attackTargetOutOfRange:
                    TipsDialog.ShowDialog("Attack target out of range!");
                    break;
                case ErrorMessage.canNotAttack:
                    TipsDialog.ShowDialog("Source can not launch an attack!");
                    break;
                case ErrorMessage.attackPositionNotFound:
                    TipsDialog.ShowDialog("No valid attack position can be found!");
                    break;
                case ErrorMessage.attackPositionOutOfRange:
                    TipsDialog.ShowDialog("Attack engagement position is out of range!");
                    break;

                case ErrorMessage.dropoffBuildingMissing:
                    TipsDialog.ShowDialog("The dropoff building is missing!");
                    break;

                case ErrorMessage.notMovable:
                    TipsDialog.ShowDialog("The unit is not movable!");
                    break;
                case ErrorMessage.targetPositionNotFound:
                    TipsDialog.ShowDialog("The target position is not found!");
                    break;

                case ErrorMessage.componentDisabled:
                    TipsDialog.ShowDialog("The component is disabled!");
                    break;

                case ErrorMessage.sourceUpgrading:
                    TipsDialog.ShowDialog("The source is upgrading!");
                    break;

                case ErrorMessage.maxPopulationReached:
                    TipsDialog.ShowDialog("Maximum population reached!");
                    break;

                case ErrorMessage.factionLimitReached:
                    TipsDialog.ShowDialog("Faction limit has been reached!");
                    break;

                case ErrorMessage.attackTypeLocked:
                    TipsDialog.ShowDialog("Attack type is locked!");
                    break;
                case ErrorMessage.attackTypeNotFound:
                    TipsDialog.ShowDialog("Attack type is not found!");
                    break;

            }
        }
    }
}
