using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using HelpURL = BehaviorDesigner.Runtime.Tasks.HelpURLAttribute;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    
    [TaskCategory("MyRTS")]
    [TaskDescription("撤退到目标位置")]
    [TaskIcon("Assets/Behavior Designer Tactical/Editor/Icons/{SkinColor}RetreatIcon.png")]
    public class MyRetreat : NavMeshTacticalGroup
    {
        [Header("撤退目标位置")]
        public SharedVector3 destinationPos;

        public bool canRotate=false;
        
        protected override void AddAgentToGroup(Behavior agent, int index)
        {
            base.AddAgentToGroup(agent, index);

            if (tacticalAgent != null) {
                // Prevent the agent from updating its rotation so the agent can attack while retreating.
                if(canRotate==false)
                    tacticalAgent.UpdateRotation(false);
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            tacticalAgent.SetDestination(destinationPos.Value);
        }


        public override TaskStatus OnUpdate()
        {
            var baseStatus = base.OnUpdate();
            if (baseStatus != TaskStatus.Running || !started) {
                return baseStatus;
            }
            
            var safe = true;
            // Try to attack the enemy while retreating.
            FindAttackTarget();
            if (tacticalAgent.CanSeeTargetByDistance()) {

                if (canRotate == false)
                {
                    if (tacticalAgent.RotateTowardsPosition(tacticalAgent.TargetTransform.position)) {
                        tacticalAgent.TryAttack();
                    }
                }
                else
                {
                    tacticalAgent.TryAttack();
                }
                
            } else {
                // The agent can update its rotation when the agent is far enough away that it can't attack.
                tacticalAgent.UpdateRotation(true);
            }

            // The agents are only save once they have moved more than the safe distance away from the attack point.
            if (!tacticalAgent.HasArrived()) {
                safe = false;
                var targetPosition = destinationPos.Value;
                tacticalAgent.SetDestination(targetPosition);
            } else {
                tacticalAgent.Stop();
            }

            return safe ? TaskStatus.Success : TaskStatus.Running;
        }
        
        
    }
    
    
}