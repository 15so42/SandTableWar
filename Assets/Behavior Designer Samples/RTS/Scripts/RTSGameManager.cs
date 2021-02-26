using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Samples
{
    // RTSGameManager keeps track of the game state
    public class RTSGameManager : MonoBehaviour
    {
        public static RTSGameManager instance;

        // how much does each unit cost
        public int unitCost;
        // the barracks component
        public Barracks barracks;

        // how much gold is currently collected
        public float GoldAmount { get { return goldAmount; } }
        private float goldAmount = 0;

        public void Awake()
        {
            instance = this;
        }

        public void Start()
        {
            // speed the game up a bit
            Time.timeScale = 2;
        }

        // the harvester has reached the unloading dock and wants to harvest the gold
        public void harvest(float amount)
        {
            goldAmount += amount;
        }

        // the user wants to create a unit
        public void createUnit()
        {
            // don't create a unit if there isn't enough gold
            if (goldAmount < unitCost) {
                return;
            }

            // only decrement the gold amount if the unit creation was successful. It may be unsuccessful if there are already too many units
            if (barracks.createUnit()) {
                goldAmount -= unitCost;
            }
        }

        // attack!
        public void attack()
        {
            barracks.attack();
        }

        // the game is over so reset
        public void reset()
        {
            // wait a little bit before actually restarting
            StartCoroutine(doReset());
        }

        public IEnumerator doReset()
        {
            // wait two seconds before actually resetting
            yield return new WaitForSeconds(2);

            goldAmount = 0;
            barracks.reset();

            // reset the enemy buildings
            var enemyBuildings = FindObjectsOfType(typeof(EnemyBuilding)) as EnemyBuilding[];
            for (int i = 0; i < enemyBuildings.Length; ++i) {
                enemyBuildings[i].GetComponent<Health>().reset();
                enemyBuildings[i].reset();
            }

            // the limited resources are no longer occupied
            var limitedResource = FindObjectsOfType(typeof(LimitedResource)) as LimitedResource[];
            for (int i = 0; i < limitedResource.Length; ++i) {
                limitedResource[i].OccupiedBy = null;
            }

            // reset the harvesters
            var harvesters = FindObjectsOfType(typeof(Harvester)) as Harvester[];
            for (int i = 0; i < harvesters.Length; ++i) {
                harvesters[i].reset();
            }
        }
    }
}