
    using UnityEngine;

    public class BaseTankFightAction : StateAction
    {
        public override void Act(StateController controller)
        {
            controller.owner.weapon.WeaponUpdate();
        }
        
        
    }
