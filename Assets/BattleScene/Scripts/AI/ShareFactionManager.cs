
    using BehaviorDesigner.Runtime;
    using UnityEngine;

    public class SharedFactionManager : SharedVariable
    {
        [SerializeField]
        public FactionManager factionManager;

        public FactionManager Value
        {
            get { return factionManager; }
            set { factionManager = value; }
        }
        public override object GetValue()
        {
            return factionManager;
        }

        public override void SetValue(object value)
        {
            factionManager = (FactionManager) value;
        }
        
    }
