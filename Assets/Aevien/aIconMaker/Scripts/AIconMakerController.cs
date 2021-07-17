using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Aevien.IconMaker
{
    public class AIconMakerController : MonoBehaviour
    {
        private float[] _resolutions = new float[] { 64, 128, 256, 512, 640, 768, 896, 1024 };
        private float _resolutionX = 768;
        private float _resolutionY = 768;

        private Dictionary<string, string> _destinationFolders;
        private string _destinationDirectoryPath;
        private int _destinationFolderId = 0;
        private string _subdirectoryPath;

        private float _smoothTime = 0.3f;

        private float _distance = 0.0f;
        private float _distanceVelocity = 0.0f;

        private float _rotY = 0.0f;
        private float _rotYVelocity = 0.0f;

        private float _rotX = 0.0f;
        private float _rotXVelocity = 0.0f;

        /// <summary>
        /// Canvas scaler to setup
        /// </summary>
        private CanvasScaler _canvasScaler;

        /// <summary>
        /// Current active item to be shot
        /// </summary>
        private GameObject _activeItem;

        /// <summary>
        /// Selected item index
        /// </summary>
        private int _selectedItemIndex = 0;

        /// <summary>
        /// List of containers of items to be shot
        /// </summary>
        private List<GameObject> _objectsContainers;

        /// <summary>
        /// Root of items to be shot
        /// </summary>
        private GameObject _itemsHolder;

        [Header("COMPONENTS")]
        public Canvas RootCanvas;
        public Camera ViewCamera;
        public Transform CameraHolder;
        public RectTransform RenderCanvas;
        public GameObject Lights;

        [Header("INPUT SETTINGS")]
        [Range(1, 10)]
        public float ScrollSpeed = 10.0f;
        [Range(100, 200)]
        public float RotationSpeed = 100.0f;
        [Range(0.1f, 100.0f)]
        public float MaxDistance = 10.0f;
        public string MouseX = "Mouse X";
        public string MouseY = "Mouse Y";

        [Header("UI COMPONENTS")]
        public Dropdown ResolutionsWidthDropdown;
        public Dropdown ResolutionsHeightDropdown;
        public Button PrevItemButton;
        public Button NextItemButton;
        public Dropdown DestinationFolderDropdown;
        public InputField CurrentItemNameInput;
        public InputField SubdirectoryInput;
        public Slider RepositionItemXSlider;
        public Slider RepositionItemYSlider;
        public Slider RepositionItemZSlider;
        public Slider LightRotationSlider;

        [Header("ITEMS LIST")]
        public Transform[] ObjectsToBeShot;

        void Start()
        {
            //Set time scale to one
            Time.timeScale = 1.0f;

            LoadSettings();

            _itemsHolder = new GameObject("--ITEMS_HOLDER");

            //Set camera clear flag to Solid
            ViewCamera.clearFlags = CameraClearFlags.SolidColor;
            ViewCamera.backgroundColor = new Color32(34, 44, 54, 0);

            //Init containers list
            _objectsContainers = new List<GameObject>();

            //Draw objects
            DrawAllObjects();

            //Draw all UI info and add all listeners
            InitUIComponents();
        }

        void Update()
        {
            if (!ViewCamera || !CameraHolder) return;

            //Update camera distance
            UpdateCameraDistance();

            //Update camera rotation
            UpdateCameraRotation();
        }

        private void OnApplicationQuit()
        {
            SaveSettings();
        }

        /// <summary>
        /// Init all ui components
        /// </summary>
        private void InitUIComponents()
        {
            // Root canvas setup
            if (RootCanvas)
            {
                RootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                _canvasScaler = RootCanvas.GetComponent<CanvasScaler>();
                _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                _canvasScaler.referenceResolution = new Vector2(1920, 1080);
            }

            // Height and width dropdowns
            if (ResolutionsHeightDropdown && ResolutionsWidthDropdown)
            {
                var t_list = _resolutions.Select(i => i.ToString()).ToList();

                ResolutionsWidthDropdown.AddOptions(t_list);
                ResolutionsHeightDropdown.AddOptions(t_list);

                ResolutionsWidthDropdown.value = ResolutionsHeightDropdown.value = 5;

                ResolutionsWidthDropdown.onValueChanged.AddListener(p_val => {
                    _resolutionX = _resolutions[p_val];
                    ResizeRenderCanvas();
                });

                ResolutionsHeightDropdown.onValueChanged.AddListener(p_val => {
                    _resolutionY = _resolutions[p_val];
                    ResizeRenderCanvas();
                });
            }

            // Change items buttons
            if (PrevItemButton && NextItemButton)
            {
                PrevItemButton.onClick.AddListener(PrevItem);
                NextItemButton.onClick.AddListener(NextItem);
            }

            // Render canvas setup
            if (RenderCanvas)
            {
                ResizeRenderCanvas();
            }

            // Destination folder setup
            if (DestinationFolderDropdown)
            {
                _destinationFolders = new Dictionary<string, string>()
                {
                    { "Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) },
                    { "Assets", Application.dataPath }
                };

                List<string> t_list = _destinationFolders.Select(i => i.Key).ToList();

                // Add data to list
                DestinationFolderDropdown.AddOptions(t_list);

                DestinationFolderDropdown.onValueChanged.AddListener(p_val => {
                    _destinationDirectoryPath = _destinationFolders.Select(i => i.Value).ToList()[p_val];
                    _destinationFolderId = p_val;
                });

                // Select folder id
                DestinationFolderDropdown.value = _destinationFolderId;
            }

            // Subdirectory setup
            if (SubdirectoryInput)
            {
                SubdirectoryInput.text = _subdirectoryPath;
                SubdirectoryInput.onValueChanged.AddListener(p_val => {
                    _subdirectoryPath = p_val;
                });
            }

            // Reposition sliders setup
            if (RepositionItemXSlider && RepositionItemYSlider && RepositionItemZSlider)
            {
                RepositionItemXSlider.onValueChanged.AddListener(p_val => {
                    var t_container = _activeItem.GetComponent<AIconMakerItemContainer>();
                    t_container.OffsetItemByX(p_val);
                });

                RepositionItemYSlider.onValueChanged.AddListener(p_val => {
                    var t_container = _activeItem.GetComponent<AIconMakerItemContainer>();
                    t_container.OffsetItemByY(p_val);
                });

                RepositionItemZSlider.onValueChanged.AddListener(p_val => {
                    var t_container = _activeItem.GetComponent<AIconMakerItemContainer>();
                    t_container.OffsetItemByZ(p_val);
                });
            }

            // Current item name input fieald setup
            if (CurrentItemNameInput)
            {
                CurrentItemNameInput.text = _activeItem.GetComponent<AIconMakerItemContainer>().GetName();
                CurrentItemNameInput.onValueChanged.AddListener(p_val => {
                    _activeItem.GetComponent<AIconMakerItemContainer>().SetName(p_val);
                });
            }

            //Rotati light source
            if (LightRotationSlider)
            {
                Lights.transform.eulerAngles = Vector3.zero;

                LightRotationSlider.onValueChanged.AddListener(p_val => {
                    Lights.transform.eulerAngles = new Vector3(0, p_val, 0);
                });
            }
        }

        /// <summary>
        /// Show prev item
        /// </summary>
        private void PrevItem()
        {
            _selectedItemIndex--;

            if (_selectedItemIndex < 0)
            {
                _selectedItemIndex = _objectsContainers.Count - 1;
            }

            RenderItem();
        }

        /// <summary>
        /// Show next item
        /// </summary>
        private void NextItem()
        {
            _selectedItemIndex++;

            if (_selectedItemIndex > _objectsContainers.Count - 1)
            {
                _selectedItemIndex = 0;
            }

            RenderItem();
        }

        /// <summary>
        /// Render current selected item
        /// </summary>
        private void RenderItem()
        {
            if (_activeItem != null)
            {
                _activeItem.SetActive(false);
            }

            _activeItem = _objectsContainers[_selectedItemIndex];
            _activeItem.SetActive(true);

            var t_container = _activeItem.GetComponent<AIconMakerItemContainer>();
            RepositionItemXSlider.value = t_container.GetOffset().x;
            RepositionItemYSlider.value = t_container.GetOffset().y;
            RepositionItemZSlider.value = t_container.GetOffset().z;

            CurrentItemNameInput.text = t_container.GetName();
        }

        /// <summary>
        /// Resize render canvas
        /// </summary>
        private void ResizeRenderCanvas()
        {
            RenderCanvas.sizeDelta = new Vector2(_resolutionX, _resolutionY);
            RenderCanvas.GetComponentInChildren<Text>().text = string.Format("Width: {0}px / Height: {1}px", _resolutionX, _resolutionY);
        }

        /// <summary>
        /// Update camera rotation
        /// </summary>
        private void UpdateCameraRotation()
        {
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                _rotY += (Input.GetAxis(MouseX) * RotationSpeed) * Time.deltaTime;
                _rotX = Mathf.Clamp(_rotX - (Input.GetAxis(MouseY) * RotationSpeed) * Time.deltaTime, -45f, 45f);
            }

            float t_newRotY = Mathf.SmoothDampAngle(transform.eulerAngles.y, _rotY, ref _rotYVelocity, _smoothTime);
            transform.eulerAngles = new Vector3(0, t_newRotY, 0);

            float t_newRotX = Mathf.SmoothDampAngle(CameraHolder.localEulerAngles.x, _rotX, ref _rotXVelocity, _smoothTime);
            CameraHolder.localEulerAngles = new Vector3(t_newRotX, 0, 0);
        }

        /// <summary>
        /// Update camera position
        /// </summary>
        void UpdateCameraDistance()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                _distance = Mathf.Clamp(_distance + (Input.mouseScrollDelta.y * 0.01f) * ScrollSpeed, -MaxDistance, -0.1f);
            }

            float t_newDistance = Mathf.SmoothDamp(ViewCamera.transform.localPosition.z, _distance, ref _distanceVelocity, _smoothTime);
            ViewCamera.transform.localPosition = new Vector3(ViewCamera.transform.localPosition.x, ViewCamera.transform.localPosition.y, t_newDistance);
        }

        /// <summary>
        /// Draw all objects to be shot
        /// </summary>
        private void DrawAllObjects()
        {
            foreach (Transform t_itemPrefab in ObjectsToBeShot)
            {
                var t_container = new GameObject(t_itemPrefab.name);
                t_container.transform.SetParent(_itemsHolder.transform);

                //Add component of Container
                t_container.AddComponent<AIconMakerItemContainer>();

                var t_itemInstance = Instantiate(t_itemPrefab, t_container.transform, false);
                t_itemInstance.localPosition = Vector3.zero;
                t_itemInstance.eulerAngles = Vector3.zero;

                var t_bounds = GetBounds(t_container);

                t_itemInstance.localPosition = t_itemInstance.localPosition - t_bounds.center;

                _objectsContainers.Add(t_container);

                t_container.SetActive(false);
            }

            RenderItem();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        private Bounds GetBounds(GameObject p_item)
        {
            var t_bounds = new Bounds();

            foreach (var i in p_item.GetComponentsInChildren<Renderer>())
            {
                t_bounds.Encapsulate(i.bounds);
            }

            return t_bounds;
        }

        private Texture2D Screenshot()
        {
            //Create new render texture
            RenderTexture t_renderTexture = new RenderTexture((int)_canvasScaler.referenceResolution.x, (int)_canvasScaler.referenceResolution.y, 32);

            //Set target texture of our camera
            ViewCamera.targetTexture = t_renderTexture;


            //Get render area
            Rect t_renderRect = GetRenderArea();

            //Init screenshot
            Texture2D t_screenShot = new Texture2D((int)t_renderRect.width, (int)t_renderRect.height, TextureFormat.ARGB32, false);

            //Render camera manually
            ViewCamera.Render();

            //Set active render texture
            RenderTexture.active = t_renderTexture;

            //Read pixels from screen at defined coordinates
            t_screenShot.ReadPixels(t_renderRect, 0, 0);
            t_screenShot.Apply();

            //Reset and destroy all data
            ViewCamera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(t_renderTexture);

            return t_screenShot;
        }

        /// <summary>
        /// Calculate render area
        /// </summary>
        /// <returns></returns>
        private Rect GetRenderArea()
        {
            Rect t_rect = new Rect
            {
                x = (_canvasScaler.referenceResolution.x / 2) - (RenderCanvas.sizeDelta.x / 2),
                y = (_canvasScaler.referenceResolution.y / 2) - (RenderCanvas.sizeDelta.y / 2),
                width = RenderCanvas.sizeDelta.x,
                height = RenderCanvas.sizeDelta.y
            };

            return t_rect;
        }

        /// <summary>
        /// Save our icon
        /// </summary>
        public void SaveScreenshot()
        {
            if (!SubdirectoryInput) throw new Exception("No subdirectory input field set up!");

            string t_subdir = string.IsNullOrEmpty(_subdirectoryPath) ? "BakedIcons" : _subdirectoryPath;
            string t_fullDirectoryPath = Path.Combine(_destinationDirectoryPath, t_subdir);

            if (!Directory.Exists(t_fullDirectoryPath))
            {
                Directory.CreateDirectory(t_fullDirectoryPath);
            }

            var filename = _activeItem.GetComponent<AIconMakerItemContainer>().GetName() + ".png";
            File.WriteAllBytes(Path.Combine(t_fullDirectoryPath, filename), Screenshot().EncodeToPNG());

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator RefreshAssets()
        {
            yield return new WaitForSeconds(1.0f);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Save tools data
        /// </summary>
        private void SaveSettings()
        {
            PlayerPrefs.SetString("_sub_directory_path", _subdirectoryPath);
            PlayerPrefs.SetInt("_directory_path_id", _destinationFolderId);
            PlayerPrefs.Save();
        }

        private void LoadSettings()
        {
            if (PlayerPrefs.HasKey("_sub_directory_path"))
            {
                _subdirectoryPath = PlayerPrefs.GetString("_sub_directory_path");
            }

            if (PlayerPrefs.HasKey("_directory_path_id"))
            {
                _destinationFolderId = PlayerPrefs.GetInt("_directory_path_id");
            }
        }
    }
}