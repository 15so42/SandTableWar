
    public class BaseLookForWoundedAction : StateAction
    {
        public override void Act(StateController controller)
        {
            for (int i = 0; i < BattleUnitBase.selfUnits.Count; i++)
            {
                BattleUnitBase unit = BattleUnitBase.selfUnits[i];
                if(unit==null)
                    continue;
                if (UnityTool.GetDistanceIgnoreY(controller.owner.transform.position, unit.transform.position) <=
                    controller.owner.prop.viewDistance)
                {
                    //controller.SetFollowingTarget(unit);
                }
            }
        }
    }