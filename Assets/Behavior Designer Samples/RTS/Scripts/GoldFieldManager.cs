using UnityEngine;

namespace BehaviorDesigner.Samples
{
    // the gold field manager has knowledge of all of the gold fields and will cycle through them when the harvester wants to find a new gold field
    public class GoldFieldManager : MonoBehaviour
    {
        public static GoldFieldManager instance;

        // all of the gold transforms that are available
        public Transform[] goldTransforms;

        private int nextGoldIndex = 0;

        public void Awake()
        {
            instance = this;
        }

        // returns the next gold field avaiable.
        public Transform nextGoldFieldTransform()
        {
            // linearly cycle through all of the gold transforms
            int index = nextGoldIndex;
            nextGoldIndex = (nextGoldIndex + 1) % goldTransforms.Length;
            return goldTransforms[index];
        }
    }
}