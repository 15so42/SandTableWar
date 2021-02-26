using UnityEngine;

namespace BehaviorDesigner.Samples
{
    // A projectile will move towards target
    public class Projectile : MonoBehaviour
    {
        // the amount of damage that is inflicted on the collided object
        public float damageAmount;
        // the speed of the projectile
        public float speed;

        public Transform Target { get { return target; } set { target = value; } }
        private Transform target;
        private Transform thisTransform;

        public void Start()
        {
            thisTransform = transform;
            thisTransform.LookAt(target);
        }

        public void Update()
        {
            // keep moving towards the target
            if (thisTransform == null) {
                return;
            }
            thisTransform.position = Vector3.MoveTowards(thisTransform.position, target.position, speed * Time.deltaTime);
        }

        // destroySelf is called by the colliding object
        public void destroySelf()
        {
            // completely destroy the projectile
            enabled = false;
            Destroy(gameObject);
        }

        public void OnDisable()
        {
            // the projectile is disabled so we no longer want to receive notifications when the target has been destoryed
            if (target != null)
                target.GetComponent<Health>().onDeath -= destroySelf;
        }
    }
}