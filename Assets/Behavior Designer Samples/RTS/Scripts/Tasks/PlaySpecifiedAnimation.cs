using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("RTS")]
    [TaskDescription("Plays the specified animation")]
    public class PlaySpecifiedAnimation : Action
    {
        [Tooltip("The name of the animation that should start playing")]
        public string animationName = "";

        private Animation thisAnimation;

        public override void OnAwake()
        {
            thisAnimation = GetComponent<Animation>();
        }

        public override TaskStatus OnUpdate()
        {
            // Stop the currently playing animation and play the specified animation. Return success.
            thisAnimation.Stop();
            thisAnimation.Play(animationName);
            return TaskStatus.Success;
        }
    }
}