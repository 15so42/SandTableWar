using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("MiniGauntlet")]
    [TaskDescription("The agent collided with the sphere. Increase the collision count.")]
    public class SphereCollision : Action
    {
        private GauntletGUI gauntletGUI;

        public override void OnAwake()
        {
            gauntletGUI = Camera.main.GetComponent<GauntletGUI>();
        }

        public override TaskStatus OnUpdate()
        {
            gauntletGUI.CollisionCount++;
            return TaskStatus.Success;
        }
    }
}