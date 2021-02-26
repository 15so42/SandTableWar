using UnityEngine;
using BehaviorDesigner.Runtime;

namespace BehaviorDesigner.Samples
{
    // The unit main responsibilities of the unit is to start the attack behavior and take damage when a projectile collides with it
    public class Unit : MonoBehaviour
    {
        private bool isAttacking;

        private Behavior behavior;
        private Health health;

        public void Start()
        {
            // cache for quick lookup
            behavior = GetComponent<Behavior>();
            health = GetComponent<Health>();
            health.onDeath += destroySelf;
        }

        public void attack()
        {
            // don't attack if we are already attacking
            if (isAttacking)
                return;

            // start the attack behavior
            behavior.EnableBehavior();
            isAttacking = true;
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

        // the unit has been destroyed by an enemy projectile
        public void destroySelf()
        {
            // notify the barracks
            Barracks.instance.unitDestoryed(this);

            // and destroy ourself
            Destroy(gameObject);
        }
    }
}