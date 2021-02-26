
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using UnityEngine;
    
    public class BaseInit : Action
    {
        public SharedBattleUnit selfBattleUnit;
        public SharedGameObject selfGameObject;
        public SharedVector3 destinationPos;
        public SharedVector3 lastDestinationPos;

        public override void OnAwake()
        {
            selfBattleUnit.Value = GetComponent<BattleUnitBase>();
            selfGameObject.Value = gameObject;
            destinationPos.Value = transform.position;
            lastDestinationPos.Value = destinationPos.Value;
        }
    }
