using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    public class SharedBattleUnit : SharedVariable
    {
        [SerializeField]
        public BattleUnitBase battleUnitBase;

        public BattleUnitBase Value
        {
            get { return battleUnitBase; }
            set { battleUnitBase = value; }
        }
        public override object GetValue()
        {
            return battleUnitBase;
        }

        public override void SetValue(object value)
        {
            battleUnitBase = (BattleUnitBase) value;
        }
        
    }
}