using UnityEngine;
using BehaviorDesigner.Runtime;

namespace BehaviorDesigner.Samples
{
    public class NPC : MonoBehaviour
    {
        // true if the NPC is on offense
        [SerializeField]
        private bool isOffense;
        public bool IsOffense { get { return isOffense; } }

        // true if the NPC currently has the flag
        private bool hasFlag = false;
        public bool HasFlag { get { return hasFlag; } set { hasFlag = value; } }

        // remember the start position/rotation for reset
        private Vector3 startPosition;
        private Quaternion startRotation;

        private CTFGameManager gameManager;
        private Behavior[] behaviors;

        public void Awake()
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
        }

        public void Start()
        {
            // cache for quick lookup
            gameManager = CTFGameManager.instance;
            behaviors = GetComponents<Behavior>();
        }

        // reset the NPC. reset can either come from running into an enemy or from resetting the game because of a flag capture
        public void reset(bool fromCollision)
        {
            // drop the flag if it currently has the flag
            if (hasFlag) {
                // the flag will be the first and only child
                if (transform.childCount > 0) {
                    transform.GetChild(0).parent = null;
                    gameManager.flagDropped();
                }
            }

            // reset the variables
            hasFlag = false;
            transform.position = startPosition;
            transform.rotation = startRotation;

            // restart the behaviors if resetting from a behavior. Don't reset if not coming from a behavior because the behaviors will be
            // reset by the game manager
            if (fromCollision) {
                for (int i = 0; i < behaviors.Length; ++i) {
                    var enemy = behaviors[i].GetVariable("Enemy") as SharedTransform;
                    if (enemy != null)
                        enemy.Value = null;
                    if (behaviors[i].Group == gameManager.ActiveGroup) {
                        BehaviorManager.instance.RestartBehavior(behaviors[i]);
                        break;
                    }
                }
            }
        }

        // Reset the NPC if it is on offense and it collides with a defensive object.
        public void OnCollisionEnter(Collision collision)
        {
            if (isOffense && gameManager.GameActive && (hasFlag || !gameManager.IsFlagTaken)) {
                NPC npc = null;
                if ((npc = collision.gameObject.GetComponent<NPC>()) != null) {
                    if (!npc.IsOffense) {
                        reset(true);
                    }
                }
            }
        }
    }
}