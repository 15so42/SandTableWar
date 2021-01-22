using UnityEngine;

namespace FoW
{
    public class FogOfWarTest2DPlayer : MonoBehaviour
    {
        public Transform cameraTransform;
        public float movementSpeed = 4;
        Vector2 _targetPosition;

        Transform _transform;

        void Start()
        {
            _transform = transform;
            _targetPosition = _transform.position;
        }

        void Update()
        {
            _transform.position = Vector2.MoveTowards(_transform.position, _targetPosition, Time.deltaTime * movementSpeed);
            if (cameraTransform != null)
                cameraTransform.position = _transform.position + Vector3.back * 10;

            if (Input.GetKeyDown(KeyCode.Space))
                FogOfWar.GetFogOfWarTeam(0).SetAll(255);

            if (Vector2.Distance(_transform.position, _targetPosition) < 0.01f)
            {
                Vector2 dir = Vector2.zero;
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    ++dir.y;
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                    ++dir.x;
                else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    --dir.y;
                else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                    --dir.x;
            
                if (dir.sqrMagnitude > 0.1f)
                {
                    RaycastHit2D hit = Physics2D.Raycast(_targetPosition, dir, 1);
                    if (hit.collider == null)
                        _targetPosition += dir;
                }
            }
        }
    }
}
