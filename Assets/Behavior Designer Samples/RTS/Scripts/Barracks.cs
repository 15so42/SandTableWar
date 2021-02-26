using UnityEngine;
using System.Collections.Generic;

namespace BehaviorDesigner.Samples
{
    // The main function of the barracks is to create units. It will also keep track of all of the units created so it can respond to events such as attacking
    public class Barracks : MonoBehaviour
    {
        public static Barracks instance;

        // the prefab of the unit to create
        public GameObject unit;
        // the bottom left position of where the units should spawn
        public Transform spawnPoint;
        // the parent of the unit that should be specified after the unit spawns
        public Transform unitParent;
        // the maximum number of units that can appear at any one time
        public int maxUnits = 15;
        // when spawning units, this is the number of units that can appear in one row before it switches to a new row
        public int unitsPerRow = 5;
        // how far apart the units should be when spawned
        public int unitSpacing = 3;

        private int rowCount = 0;
        private int rowIndex = 0;

        private List<GameObject> units = new List<GameObject>();

        public void Awake()
        {
            instance = this;
        }

        // create a new unit
        public bool createUnit()
        {
            // don't create too many units
            if (units.Count == maxUnits)
                return false;

            // spawn the unit in a grid pattern with spawnPoint.position being the bottom left position
            if (rowCount == unitsPerRow) {
                rowIndex++;
                rowCount = 0;
            }

            // we can figure out the correct position now that we have the correct row and column index
            var spawnPosition = spawnPoint.position + spawnPoint.transform.right * rowCount * unitSpacing + spawnPoint.transform.forward * rowIndex * unitSpacing;
            var spawnedUnit = GameObject.Instantiate(unit, spawnPosition, spawnPoint.rotation) as GameObject;
            spawnedUnit.transform.parent = unitParent;

            // store the spawned unit in a list so we can reference it later
            units.Add(spawnedUnit);

            // increase the row count for the next unit
            rowCount++;

            return true;
        }

        // attack with all units and reset the row variables so new units spawn from the beginning
        public void attack()
        {
            for (int i = 0; i < units.Count; ++i) {
                units[i].GetComponent<Unit>().attack();
            }

            rowCount = rowIndex = 0;
        }

        // a unit has been destroyed, remove it from the list
        public void unitDestoryed(Unit unit)
        {
            units.Remove(unit.gameObject);
        }

        // reset the game back to the original state. Destroy all units and reinitialize the row variables
        public void reset()
        {
            for (int i = units.Count - 1; i > -1; --i) {
                Destroy(units[i]);
            }
            units.Clear();
            rowCount = rowIndex = 0;
        }
    }
}