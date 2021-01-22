using UnityEngine;

namespace FoW
{
    [AddComponentMenu("FogOfWar/Test/FogOfWarTestEnemy")]
    public class FogOfWarTestEnemy : MonoBehaviour
    {
        public float speed = 4;
        public Vector3[] waypoints;
        int _currentWaypoint = 0;

        Rigidbody _rigidbody;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            Vector3 pos = _rigidbody.position;

            Vector3 target = waypoints[_currentWaypoint];
            target.y = pos.y;

            pos = Vector3.MoveTowards(pos, target, speed * Time.deltaTime);
            if ((pos - target).sqrMagnitude < 1)
                _currentWaypoint = (_currentWaypoint + 1) % waypoints.Length;
            else
                _rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, Quaternion.LookRotation(target - pos, Vector3.up), speed * Time.deltaTime);

            _rigidbody.position = pos;
        }

        void OnDrawGizmosSelected()
        {
            if (waypoints == null)
                return;

            Gizmos.color = Color.red;
            for (int i = 0; i < waypoints.Length; ++i)
            {
                Gizmos.DrawSphere(waypoints[i], 0.5f);
                Gizmos.DrawLine(waypoints[i], waypoints[(i + 1) % waypoints.Length]);
            }
        }
    }
}