using UnityEngine;
using System.Collections.Generic;

// WARNING: This code is crap. Do not use it in your game. Do not read it. It will make your brain bleed funny colors.
namespace FoW
{
    class FOWUnit
    {
        public Vector3 destination;
        public FogOfWarUnit unit;
        public Transform transform { get; private set; }
        public Vector3 position { get { return transform.position; } set { transform.position = value; } }

        public FOWUnit(FogOfWarUnit u)
        {
            unit = u;
            transform = unit.transform;
            destination = unit.transform.position;
        }
    }

    [AddComponentMenu("FogOfWar/Test/FogOfWarTestGUI")]
    public class FogOfWarTestGUI : MonoBehaviour
    {
        public int team = 0;
        public float unitMoveSpeed = 3.0f;
        public float cameraSpeed = 20.0f;
        public float unfogSize = 2;
        public Transform highlight;
        public FogOfWarUnit[] originalUnits;
        public Transform[] enemyUnits;
        public LayerMask unitLayer;
        public bool showGui = true;

        FogOfWar _fog;
        Texture2D _texture;
        GUIStyle _panelStyle;
        int _visibleEnemies = 0;

        Camera _camera;
        Transform _cameraTransform;

        List<FOWUnit> _units = new List<FOWUnit>();

        void Start()
        {
            _fog = GetComponent<FogOfWar>();
            _camera = GetComponent<Camera>();
            _cameraTransform = transform;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
                return;
            }

            // select unit
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                RaycastHit hit;
                if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit))//, unitLayer))
                {
                    FogOfWarUnit unit = hit.collider.GetComponent<FogOfWarUnit>();
                    if (unit != null && unit.team == _fog.team)
                    {
                        int index = _units.FindIndex(((u) => u.unit == unit));
                        if (index != -1)
                        {
                            _units.Add(_units[index]);
                            _units.RemoveAt(index);
                        }
                        else
                            _units.Add(new FOWUnit(unit));
                    }
                }
            }

            // move unit
            if (_units.Count > 0 && Input.GetKeyDown(KeyCode.Mouse1))
            {
                RaycastHit[] hits = Physics.RaycastAll(_camera.ScreenPointToRay(Input.mousePosition));
                if (hits.Length > 0)
                {
                    Vector3 p = hits[hits.Length - 1].point;
                    p.y = 1.0f;
                    _units[_units.Count - 1].destination = p;
                }
            }

            // clear fogged area
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                RaycastHit hit;
                if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit))
                    _fog.SetFog(new Bounds(hit.point, Vector3.one * unfogSize), 0);
            }

            // update units
            float moveamount = unitMoveSpeed * Time.deltaTime;
            for (int i = 0; i < _units.Count; ++i)
            {
                FOWUnit u = _units[i];
                Vector3 direction = u.destination - u.position;
                direction.y = 0.0f;
                if (direction.sqrMagnitude < moveamount * moveamount)
                    u.position = new Vector3(u.destination.x, u.position.y, u.destination.z);
                else
                {
                    u.position += direction.normalized * moveamount;
                    u.transform.rotation = Quaternion.Slerp(u.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), moveamount);
                }
            }

            // update highlight
            if (_units.Count > 0)
            {
                highlight.position = new Vector3(_units[_units.Count - 1].position.x, 0.1f, _units[_units.Count - 1].position.z);
                highlight.gameObject.SetActive(true);
            }

            // update camera
            _cameraTransform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * (Time.deltaTime * cameraSpeed);
            
            if (Input.touchCount == 1)
            {
                Vector2 delta = Input.GetTouch(0).deltaPosition;
                _cameraTransform.position += new Vector3(-delta.x, 0, -delta.y);
            }

            // update camera zooming
            float zoomchange = Input.GetAxis("Mouse ScrollWheel");
            _cameraTransform.position = new Vector3(_cameraTransform.position.x, Mathf.Clamp(_cameraTransform.position.y - zoomchange * 10, 25, 50), _cameraTransform.position.z);
        }

        void DrawOnMap(string text, Vector3 position, int panelwidth)
        {
            Vector2 normalizedfogpos = (FogOfWarConversion.WorldToFogPlane(position, _fog.plane) - _fog.mapOffset) / _fog.mapSize + new Vector2(0.5f, 0.5f);
            Vector2i mappos = new Vector2i(normalizedfogpos * (panelwidth - 20));
            GUI.Label(new Rect(10 + mappos.x - 10, Screen.height - mappos.y - 10, 20, 20), text);
        }

        void OnGUI()
        {
            if (!showGui || _fog.fogTexture == null)
                return;

            bool minimal = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;

            if (_texture == null)
            {
                _texture = new Texture2D(_fog.fogTexture.width, _fog.fogTexture.height);
                _texture.wrapMode = TextureWrapMode.Clamp;
            }

            if (_panelStyle == null)
            {
                Texture2D panelTex = new Texture2D(1, 1);
                panelTex.SetPixels32(new Color32[] { new Color32(255, 255, 255, 64) });
                panelTex.Apply();
                _panelStyle = new GUIStyle();
                _panelStyle.normal.background = panelTex;
            }

            byte[] original = _fog.fogTexture.GetRawTextureData();
            Color32[] pixels = new Color32[original.Length];
            for (int i = 0; i < pixels.Length; ++i)
                pixels[i] = original[i] < 255 ? new Color32(0, 255, 50, 255) : new Color32(0, 0, 0, 255);
            _texture.SetPixels32(pixels);
            _texture.Apply();

            int panelwidth = (minimal ? 128 : 256) + 20;

            // draw panel
            GUI.Box(new Rect(0, 0, panelwidth, Screen.height), "", _panelStyle);
            GUILayout.BeginArea(new Rect(10, 10, panelwidth - 20, Screen.height - panelwidth - 20));

            if (!minimal)
            {
                GUILayout.Label("Partial Fog Amount:");
                _fog.partialFogAmount = GUILayout.HorizontalSlider(_fog.partialFogAmount, 0.0f, 1.0f);

                _fog.pointFiltering = GUILayout.Toggle(_fog.pointFiltering, "Point Filtering");

                GUILayout.Label("Fog Color:");
                _fog.fogColor.r = GUILayout.HorizontalSlider(_fog.fogColor.r, 0.0f, 1.0f);
                _fog.fogColor.g = GUILayout.HorizontalSlider(_fog.fogColor.g, 0.0f, 1.0f);
                _fog.fogColor.b = GUILayout.HorizontalSlider(_fog.fogColor.b, 0.0f, 1.0f);
            }

            if (GUILayout.Button("Reset Fog"))
                _fog.SetAll(255);

            if (_units.Count > 0)
            {
                GUILayout.Label("Selected Unit Vision Radius:");
                _units[_units.Count - 1].unit.circleRadius = GUILayout.HorizontalSlider(_units[_units.Count - 1].unit.circleRadius, 3.0f, 20.0f);
            }

            if (!minimal)
            {
                GUILayout.Label("\n-- Controls --");
                GUILayout.Label("Move Camera:\tWASD/Arrow Keys");
                GUILayout.Label("Select Unit:\tLeft Mouse Button");
                GUILayout.Label("Move Unit:\t\tMiddle/Right Mouse Button");
            }

            GUILayout.Label("\n-- Stats --");
            GUILayout.Label("Explored :\t" + Mathf.RoundToInt(_fog.ExploredArea() * 100) + "%");
            GUILayout.Label("Visible Enemies:\t" + _visibleEnemies);

            GUILayout.EndArea();

            // draw map
            GUI.DrawTexture(new Rect(10, Screen.height - panelwidth + 10, panelwidth - 20, panelwidth - 20), _texture);

            DrawOnMap("C", _cameraTransform.position, panelwidth);
            for (int i = 0; i < originalUnits.Length; ++i)
                DrawOnMap("U", originalUnits[i].transform.position, panelwidth);

            _visibleEnemies = 0;
            for (int i = 0; i < enemyUnits.Length; ++i)
            {
                if (_fog.GetFogValue(enemyUnits[i].position) < 128)
                {
                    ++_visibleEnemies;
                    DrawOnMap("E", enemyUnits[i].position, panelwidth);
                }
            }
        }
    }
}