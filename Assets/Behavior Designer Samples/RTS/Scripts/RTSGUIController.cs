using UnityEngine;
using System.Collections.Generic;

namespace BehaviorDesigner.Samples
{
    // The RTSGUIController manages the GUI, using OnGUI
    public class RTSGUIController : MonoBehaviour
    {
        public static RTSGUIController instance;

        // a list of all of the cameras
        public Camera[] cameras;
        private int activeCameraIndex = 0;

        private GUIStyle labelStyle;
        private float startEnemyHealth = 0;

        private List<Health> enemyHealth = new List<Health>();
        private RTSGameManager gameManager;

        public void Awake()
        {
            instance = this;
        }

        public void Start()
        {
            // cache for quick lookup
            gameManager = RTSGameManager.instance;

            // increase the OnGUI font size
            labelStyle = new GUIStyle();
            labelStyle.fontSize = 24;
            labelStyle.normal.textColor = Color.white;

            // keep a reference to all of the enemy building health components so the gui can display how much enemy health is left
            var enemyBuildings = FindObjectsOfType(typeof(EnemyBuilding)) as EnemyBuilding[];
            for (int i = 0; i < enemyBuildings.Length; ++i) {
                enemyHealth.Add(enemyBuildings[i].GetComponent<Health>());
                startEnemyHealth += enemyHealth[i].startHealth;
            }
        }

        public void OnGUI()
        {
            // list the important variables such as the amount of gold and the enemy's health
            GUILayout.Label(string.Format("Gold: {0}", gameManager.GoldAmount), labelStyle);
            float currentHealth = 0;
            for (int i = 0; i < enemyHealth.Count; ++i) {
                currentHealth += enemyHealth[i].Amount;
            }
            GUILayout.Label(string.Format("Enemy Health: {0}%", Mathf.RoundToInt((currentHealth / startEnemyHealth) * 100)), labelStyle);

            // allow the user to perform some actions
            if (GUILayout.Button("Create Unit")) {
                gameManager.createUnit();
            }
            if (GUILayout.Button("Attack")) {
                gameManager.attack();
            }
            if (GUILayout.Button("Switch Cameras")) {
                switchCameras();
            }
        }

        // switch to a better camera
        private void switchCameras()
        {
            // cycle through all of the cameras
            cameras[activeCameraIndex].enabled = false;
            activeCameraIndex = (activeCameraIndex + 1) % cameras.Length;
            cameras[activeCameraIndex].enabled = true;
        }
    }
}