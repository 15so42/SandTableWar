using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("RTS")]
    [TaskDescription("Add gold to the harvester after the harvester is on a gold field")]
    public class HarvestGold : Action
    {
        [Tooltip("The amount of gold to harvest")]
        public float amount;

        private Harvester harvester;

        public override void OnAwake()
        {
            // cache for quick lookup
            harvester = gameObject.GetComponent<Harvester>();
        }

        // OnUpdate will return success in one frame after it has harvested the gold
        public override TaskStatus OnUpdate()
        {
            // add the gold to the harvester
            harvester.GoldHarvested += amount;
            return TaskStatus.Success;
        }
    }
}