using UnityEngine;

namespace BehaviorDesigner.Samples
{
    // Health is attached to each object that has health. It will notify the interested objects when the health reaches 0
    public class Health : MonoBehaviour
    {
        public delegate void Death();
        public event Death onDeath;

        // how much health the object should start with
        public float startHealth = 100;

        public float Amount { get { return health; } }
        private float health;

        public void Start()
        {
            health = startHealth;
        }

        // the attached object has been damaged
        public void takeDamage(float amount)
        {
            health -= amount;

            // don't let the health go below zero
            if (health <= 0) {
                health = 0;
                // fire an event when the attached object is dead
                if (onDeath != null) {
                    onDeath();
                    onDeath = null;
                }
            }
        }

        // reset the variables back to their starting variables
        public void reset()
        {
            health = startHealth;
            onDeath = null;
        }
    }
}