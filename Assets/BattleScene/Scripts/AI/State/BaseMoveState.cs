﻿
    public class BaseMoveState:State
    {
        public BaseMoveState(StateController controller, string stateName) : base(controller, stateName)
        {
        }
        
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            ownerController.navMeshAgent.isStopped = false;
        }
    }
