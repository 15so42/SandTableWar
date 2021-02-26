using UnityEngine;
using BehaviorDesigner.Runtime;

namespace BehaviorDesigner.Samples
{
    // the harvester collects gold
    public class Harvester : MonoBehaviour
    {
        public float GoldHarvested { get { return goldHarvested; } set { goldHarvested = value; } }
        private float goldHarvested;

        private Vector3 startPosition;
        private Quaternion startRotation;

        public void Start()
        {
            // remember the start position/rotation for a restart
            startPosition = transform.position;
            startRotation = transform.rotation;
        }

        // reset all of the variables back to their starting value
        public void reset()
        {
            transform.position = startPosition;
            transform.rotation = startRotation;

            goldHarvested = 0;

            // restart the behavior
            var behavior = GetComponent<Behavior>();
            behavior.DisableBehavior();
            behavior.EnableBehavior();
        }
    }
}