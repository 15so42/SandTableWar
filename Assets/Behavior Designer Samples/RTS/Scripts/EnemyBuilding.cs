using UnityEngine;
using BehaviorDesigner.Runtime;

namespace BehaviorDesigner.Samples
{
    // the enemy building manages each building. It is responsible for determining the attack position of the units
    // as well as resetting variables when the object is destroyed
    public class EnemyBuilding : MonoBehaviour
    {
        // how far from the center of the building should the objects be when attacking
        public float radius;
        // how spread out should the units be from each other when attacking
        public float attackSpread;
        // the initial offset of the spread
        public float spreadOffset;
        // when the building is destroyed should the building be reset
        public bool resetOnDestruction;

        private int unit = 0;
        private Health health;
        private Behavior behavior;

        public void Start()
        {
            // cache for quick lookup
            health = GetComponent<Health>();
            behavior = GetComponent<Behavior>();

            // continue initialization by resetting the variables
            reset();
        }

        // returns the next position around the enemy building. These positions will be aligned in a circle around the current building
        public Vector3 nextAttackPosition()
        {
            // use the parametric equation of a circle to determine the next attack position.
            var position = transform.position;
            position.x += radius * Mathf.Sin((attackSpread * unit + spreadOffset) * Mathf.Deg2Rad);
            position.z += radius * Mathf.Cos((attackSpread * unit + spreadOffset) * Mathf.Deg2Rad);
            unit++;
            return position;
        }

        // take some damage when a projectile collides with the building
        public void OnCollisionEnter(Collision collision)
        {
            Projectile projectile;
            if ((projectile = collision.gameObject.GetComponent<Projectile>()) != null) {
                // take damage. heath will fire an event if the damage is equal to 0
                health.takeDamage(projectile.damageAmount);
                // destroy the projectile
                projectile.destroySelf();
            }
        }

        // the building's heath is 0 so distroy it
        public void destroySelf()
        {
            // disable the renderer to indicate a destroyed building
            GetComponent<Renderer>().enabled = false;

            // disable the behavior if this building has a behavior attached to it
            if (behavior) {
                behavior.DisableBehavior();
            }

            // reset the game if specified
            if (resetOnDestruction) {
                RTSGameManager.instance.reset();
            }
        }

        public void reset()
        {
            // get notified when the health reaches 0
            health.onDeath += destroySelf;
            GetComponent<Renderer>().enabled = true;
            unit = 0;
        }
    }
}