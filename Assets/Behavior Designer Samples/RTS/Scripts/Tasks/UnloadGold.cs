using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("RTS")]
    [TaskDescription("Unload all of the gold that the harvester currently is carrying")]
    public class UnloadGold : Action
    {
        private Harvester harvester;
        private RTSGameManager gameManager;

        public override void OnStart()
        {
            // cache for quick lookup
            harvester = gameObject.GetComponent<Harvester>();
            gameManager = RTSGameManager.instance;
        }

        // will return success after the gold has been harvested
        public override TaskStatus OnUpdate()
        {
            // let the game manager know about the gold harvested
            gameManager.harvest(harvester.GoldHarvested);
            // the harvester no longer has any gold
            harvester.GoldHarvested = 0;
            return TaskStatus.Success;
        }
    }
}