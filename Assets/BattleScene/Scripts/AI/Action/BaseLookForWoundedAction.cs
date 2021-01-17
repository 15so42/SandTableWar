public class BaseLookForWoundedAction : StateAction
{
    public override void Act(StateController controller)
    {
        MedicalStateController medicalStateController = (controller as MedicalStateController);
        BattleUnitBase cureTarget = medicalStateController.cureTarget;
        if (cureTarget == null)
        {
            float minPercentage = 1;
            for (int i = 0; i < BattleUnitBase.selfUnits.Count; i++)
            {
                BattleUnitBase unit = BattleUnitBase.selfUnits[i];
                if (unit == null)
                    continue;
                if (UnityTool.GetDistanceIgnoreY(controller.owner.transform.position, unit.transform.position) <=
                    controller.owner.prop.viewDistance)
                {
                    float percentage = unit.prop.GetPercentage();
                    if (percentage <= minPercentage)
                    {
                        minPercentage = percentage;
                        controller.navMeshAgent.SetDestination(unit.transform.position);
                        medicalStateController.SetCureTarget(unit);
                    }
                }
            }
        }

        if (cureTarget == null)
        {
            return;
        }
        if (IsInCureRange())
        {
            controller.navMeshAgent.isStopped = true;
            (controller.owner.animCtrl as MedicalAnimCtrl).CureAnim();
        }
        
        bool IsInCureRange()
        {
            return UnityTool.GetDistanceIgnoreY(controller.owner.transform.position,
                       cureTarget.transform.position) <
                   controller.owner.prop.attackDistance;
        }
    }
    
    
}