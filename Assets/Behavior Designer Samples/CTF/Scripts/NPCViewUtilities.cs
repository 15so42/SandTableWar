using UnityEngine;

namespace BehaviorDesigner.Samples
{
    // A static class that contains common functions used by multiple classes
    public static class NPCViewUtilities
    {
        // returns true if targetTransform is within sight of transform
        public static bool WithinSight(Transform transform, Transform targetTransform, float fieldOfViewAngle, float sqrViewMagnitude)
        {
            Vector3 direction = targetTransform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);
            // an object is within sight if it is within the field of view and has a magnitude less than what the object can see
            if (angle < fieldOfViewAngle && Vector3.SqrMagnitude(direction) < sqrViewMagnitude) {
                RaycastHit hit;
                // to be in sight no objects can be obtruding the view
                if (Physics.Raycast(transform.position, direction.normalized, out hit)) {
                    // if the hit object is the target object then no objects are obtruding the view
                    if (hit.transform.Equals(targetTransform)) {
                        return true;
                    }
                }
            }

            return false;
        }

        // returns true if targetTransform is in a line of sight of transform
        public static bool LineOfSight(Transform transform, Transform targetTransform, Vector3 direction)
        {
            RaycastHit hit;
            // cast a ray. If the ray hits the targetTransform then no objects are obtruding the view
            if (Physics.Raycast(transform.position, direction.normalized, out hit)) {
                if (hit.transform.Equals(targetTransform)) {
                    return true;
                }
            }
            return false;
        }

        // hekp visualize the line of sight within the editor
        public static void DrawLineOfSight(Transform transform, float fieldOfViewAngle, float viewMagnitude)
        {
#if UNITY_EDITOR
            float radius = viewMagnitude * Mathf.Sin(fieldOfViewAngle * Mathf.Deg2Rad);
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            // draw a disk at the end of the sight distance.
            UnityEditor.Handles.DrawWireDisc(transform.position + transform.forward * viewMagnitude, transform.forward, radius);
            // draw to lines to represent the left and right side of the line of sight
            UnityEditor.Handles.DrawLine(transform.position, transform.TransformPoint(new Vector3(radius, 0, viewMagnitude)));
            UnityEditor.Handles.DrawLine(transform.position, transform.TransformPoint(new Vector3(-radius, 0, viewMagnitude)));
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}