using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

namespace BehaviorDesigner.Samples
{
    public class CTFGameManager : MonoBehaviour
    {
        public static CTFGameManager instance;

        // the time that the offense NPCs will celebrate after the flag is captured before the game is reset
        public float celebrationTime = 2.0f;
        // the parent transform of all of the NPCs
        public Transform NPCGroup;
        // the flag transform
        public Transform flag;

        // true if the game is currently active
        private bool gameActive = false;
        public bool GameActive { get { return gameActive; } }
        // Behaviors are grouped into two sections. The first set of behaviors contains all of the behaviors that should be running
        // the flag is not taken. When the flag is taken the second set of behaviors should run. For example, when the flag is not
        // taken the offense NPCs should seek the flag and avoid any enemies. When the flag is taken they should take the flag
        // to the capture point. This could have also been done using conditions within the actual behavior but this just shows
        // one more way to solve the problem.
        private int activeGroup = -1;
        public int ActiveGroup { get { return activeGroup; } }

        // true if the flag is taken by an offensive unit
        private bool isFlagTaken = false;
        public bool IsFlagTaken { get { return isFlagTaken; } }
        // the start flag position and rotation, used when the game resets
        private Vector3 startFlagPosition;
        private Quaternion startFlagRotation;
        // a list of all NPC units
        private List<NPC> NPCs = new List<NPC>();

        // keep a list of all of the flag not taken and flag taken behaviors so they can be enabled/disabled
        private List<Behavior> flagNotTakenBehaviors = new List<Behavior>();
        private List<Behavior> flagTakenBehaviors = new List<Behavior>();

        private BehaviorManager behaviorManager;

        public void Awake()
        {
            instance = this;
        }

        public void Start()
        {
            // cache for quick lookup
            behaviorManager = BehaviorManager.instance;

            // speed the game up a bit
            Time.timeScale = 2;

            // remember the starting position/rotation of the flag
            startFlagPosition = flag.position;
            startFlagRotation = flag.rotation;

            // find all of the NPCS
            for (int i = 0; i < NPCGroup.childCount; ++i) {
                NPCs.Add(NPCGroup.GetChild(i).GetComponent<NPC>());
            }

            // group the behaviors
            var allBehaviors = FindObjectsOfType(typeof(Behavior)) as Behavior[];
            for (int i = 0; i < allBehaviors.Length; ++i) {
                if (allBehaviors[i].Group == 0) { // 0 indicates flag not taken behaviors
                    flagNotTakenBehaviors.Add(allBehaviors[i]);
                } else { // 1 indicates flag taken behaviors
                    flagTakenBehaviors.Add(allBehaviors[i]);
                }
            }

            // currently the flag is not taken and the game is active
            activeGroup = 0;
            gameActive = true;
        }

        // the flag has been taken. Deactivate the flag not taken behaviors and activate the flag taken behaviors
        public bool flagTaken()
        {
            // return if the flag is already taken
            if (isFlagTaken) {
                return false;
            }

            // deactivate the flag not taken behaviors
            for (int i = 0; i < flagNotTakenBehaviors.Count; ++i) {
                if (behaviorManager.IsBehaviorEnabled(flagNotTakenBehaviors[i])) {
                    flagNotTakenBehaviors[i].DisableBehavior();
                }
            }

            // acctivate the flag taken behaviors
            for (int i = 0; i < flagTakenBehaviors.Count; ++i) {
                flagTakenBehaviors[i].EnableBehavior();
            }

            // the flag is taken
            isFlagTaken = true;
            return true;
        }

        // the flag has been dropped. Deactivate the flag taken behaviors and activate the flag not taken behaviors
        public void flagDropped()
        {
            // return if the flag has already been dropped
            if (!isFlagTaken) {
                return;
            }

            // deactivate the flag taken behaviors
            for (int i = 0; i < flagTakenBehaviors.Count; ++i) {
                if (behaviorManager.IsBehaviorEnabled(flagTakenBehaviors[i])) {
                    flagTakenBehaviors[i].DisableBehavior();
                }
            }

            // activate the flag not taken behaviors
            for (int i = 0; i < flagNotTakenBehaviors.Count; ++i) {
                flagNotTakenBehaviors[i].EnableBehavior();
            }

            // the flag is not taken
            isFlagTaken = false;
        }

        // the flag has been captured. Reset the game
        public void resetGame()
        {
            if (gameActive) {
                StartCoroutine(doReset());
                gameActive = false;
            }
        }

        public IEnumerator doReset()
        {
            yield return new WaitForSeconds(celebrationTime);

            // stop the behaviors
            for (int i = 0; i < flagTakenBehaviors.Count; ++i) {
                if (behaviorManager.IsBehaviorEnabled(flagTakenBehaviors[i])) {
                    flagTakenBehaviors[i].DisableBehavior();
                }
            }
            for (int i = 0; i < flagNotTakenBehaviors.Count; ++i) {
                if (behaviorManager.IsBehaviorEnabled(flagNotTakenBehaviors[i])) {
                    flagNotTakenBehaviors[i].DisableBehavior();
                }
            }

            // reset the flag position/rotation
            flag.parent = null;
            flag.position = startFlagPosition;
            flag.rotation = startFlagRotation;

            // reset the NPC position/rotation
            for (int i = 0; i < NPCs.Count; ++i) {
                NPCs[i].reset(false);
            }

            // start the behaviors
            for (int i = 0; i < flagNotTakenBehaviors.Count; ++i) {
                flagNotTakenBehaviors[i].EnableBehavior();
            }

            // reset the variables
            isFlagTaken = false;
            gameActive = true;
        }
    }
}