using UnityEngine;

namespace BehaviorDesigner.Samples
{
    public class GauntletGUI : MonoBehaviour
    {
        public GUISkin guiSkin;
        private int collisionCount = 0;
        public int CollisionCount { get { return collisionCount; } set { collisionCount = value; } }

        // Show the number of times a sphere has collided with the agent
        public void OnGUI()
        {
            GUILayout.Label("Collisions: " + collisionCount, guiSkin.label, GUILayout.Width(400));
        }
    }
}