using UnityEngine;

namespace BehaviorDesigner.Samples
{
    // A limited resource is one in which can only be used by one object at a time. Examples include the gold field and the refinery's unloading dock
    public class LimitedResource : MonoBehaviour
    {
        public Transform OccupiedBy { get { return occupiedBy; } set { occupiedBy = value; } }
        private Transform occupiedBy = null;

        // if the occupiedBy transform exits the trigger then the resource is available again
        public void OnTriggerExit(Collider other)
        {
            if (other.transform.Equals(occupiedBy)) {
                occupiedBy = null;
            }
        }
    }
}