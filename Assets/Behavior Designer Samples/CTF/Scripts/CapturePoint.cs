using UnityEngine;

namespace BehaviorDesigner.Samples
{
    // Notifies the game manager when the flag enters the trigger
    public class CapturePoint : MonoBehaviour
    {
        // the flag's game object
        public GameObject flag;

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.Equals(flag)) {
                // When the flag reaches the capture point the game is over
                CTFGameManager.instance.resetGame();
            }
        }
    }
}