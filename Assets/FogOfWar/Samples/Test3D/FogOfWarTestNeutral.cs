using UnityEngine;

namespace FoW
{
    [AddComponentMenu("FogOfWar/Test/FogOfWarTestEnemy")]
    public class FogOfWarTestNeutral : MonoBehaviour
    {
        public float speed = 4;
        public float retargetTime = 5;
        float _timeSinceRetarget = 0;
        Vector3 _target;

        Rigidbody _rigidbody;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            Retarget();
        }

        void Retarget()
        {
            _target = _rigidbody.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
            _timeSinceRetarget = 0;
        }

        void FixedUpdate()
        {
            Vector3 pos = _rigidbody.position;

            _target.y = pos.y;

            pos = Vector3.MoveTowards(pos, _target, speed * Time.deltaTime);
            _timeSinceRetarget += Time.deltaTime;
            if ((pos - _target).sqrMagnitude < 1 || _timeSinceRetarget >= retargetTime)
                Retarget();
            else
                _rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, Quaternion.LookRotation(_target - pos, Vector3.up), speed * Time.deltaTime);

            _rigidbody.position = pos;
        }
    }
}