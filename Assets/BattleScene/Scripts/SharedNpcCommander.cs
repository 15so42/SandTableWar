
    using BehaviorDesigner.Runtime;
    using UnityEngine;

    public class SharedNpcCommander : SharedVariable
    {
        [SerializeField] public NpcCommander npcCommander;

        public NpcCommander Value
        {
            get { return npcCommander; }
            set { npcCommander = value; }
        }

        public override object GetValue()
        {
            return npcCommander;
        }

        public override void SetValue(object value)
        {
            npcCommander = (NpcCommander) value;
        }
    }
