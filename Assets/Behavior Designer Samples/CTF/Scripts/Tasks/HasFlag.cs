using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("CTF")]
    [TaskDescription("Conditional task that returns if the NPC component has the flag")]
    public class HasFlag : Conditional
    {
        private NPC npc;

        public override void OnAwake()
        {
            // cache for quick lookup
            npc = gameObject.GetComponent<NPC>();
        }

        // Return success if the NPC has the flag, failure if it doesn't.
        public override TaskStatus OnUpdate()
        {
            if (npc.HasFlag)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}