using UnityEngine;

namespace BehaviorDesigner.Samples
{
    public class FireSphere : MonoBehaviour
    {
        public Transform sphereGroup;
        public Vector3 force;

        // When the agent enters the trigger shoot the group of spheres at the agent
        public void OnTriggerEnter(Collider other)
        {
            if (sphereGroup.GetComponent<Rigidbody>() != null) {
                Fire(sphereGroup.GetComponent<Rigidbody>());
            }

            for (int i = 0; i < sphereGroup.childCount; ++i) {
                if (sphereGroup.GetChild(i).GetComponent<Rigidbody>() != null) {
                    Fire(sphereGroup.GetChild(i).GetComponent<Rigidbody>());
                }
            }
        }

        // Fire with the specified force
        public void Fire(Rigidbody sphereRigidbody)
        {
            sphereRigidbody.isKinematic = false;
            sphereRigidbody.AddForce(force, ForceMode.VelocityChange);
        }
    }
}