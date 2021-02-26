using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace BehaviorDesigner.Samples
{
    [TaskCategory("RTS")]
    [TaskDescription("Fires a projectile at the target")]
    public class FireProjectile : Action
    {
        [Tooltip("he prefab of the projectile to be fired")]
        public GameObject projectilePrefab;
        [Tooltip("The target of the object")]
        public SharedTransform target;

        private Collider thisCollider;

        public override void OnAwake()
        {
            thisCollider = GetComponent<Collider>();
        }

        // OnUpdate will return success in one frame after it has created the projectile
        public override TaskStatus OnUpdate()
        {
            // create a new project whose parent is the current game object
            var spawnedProjectile = GameObject.Instantiate(projectilePrefab) as GameObject;
            spawnedProjectile.transform.position = transform.position;
            spawnedProjectile.transform.LookAt(target.Value);
            spawnedProjectile.transform.parent = transform;

            var projectile = spawnedProjectile.GetComponent<Projectile>();
            projectile.Target = target.Value;
            // Add an event so the projectile is destroyed when the target is destroyed
            target.Value.GetComponent<Health>().onDeath += projectile.destroySelf;
            // ignore the collisions between the projectile and the object firing the projectile to 
            // prevent the projectile from inflicting damage on the object shooting it
            Physics.IgnoreCollision(thisCollider, projectile.GetComponent<Collider>());
            return TaskStatus.Success;
        }
    }
}