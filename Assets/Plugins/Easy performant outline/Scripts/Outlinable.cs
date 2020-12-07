using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace EPOOutline
{
    public enum DilateRenderMode
    {
        PostProcessing,
        EdgeShift
    }

    public enum RenderStyle
    {
        Single = 1,
        FrontBack = 2
    }

    [Flags]
    public enum OutlinableDrawingMode
    {
        Normal = 1,
        ZOnly = 2
    }

    [ExecuteAlways]
    public class Outlinable : MonoBehaviour
    {
        private static HashSet<Outlinable> outlinables = new HashSet<Outlinable>();

        private static Plane[] frustrumPlanes = new Plane[6];

        [System.Serializable]
        public class OutlineProperties
        {
#pragma warning disable CS0649
            [SerializeField]
            private bool enabled = true;

            public bool Enabled
            {
                get
                {
                    return enabled;
                }

                set
                {
                    enabled = value;
                }
            }

            [SerializeField]
            private Color color = Color.yellow;

            public Color Color
            {
                get
                {
                    return color;
                }

                set
                {
                    color = value;
                }
            }

            [SerializeField]
            [Range(0.0f, 1.0f)]
            private float dilateShift = 1.0f;

            public float DilateShift
            {
                get
                {
                    return dilateShift;
                }

                set
                {
                    dilateShift = value;
                }
            }

            [SerializeField]
            [Range(0.0f, 1.0f)]
            private float blurShift = 1.0f;

            public float BlurShift
            {
                get
                {
                    return blurShift;
                }

                set
                {
                    blurShift = value;
                }
            }

            [SerializeField]
            private SerializedPass fillPass = new SerializedPass();

            public SerializedPass FillPass
            {
                get
                {
                    return fillPass;
                }
            }
#pragma warning restore CS0649
        }

        [SerializeField]
        private OutlinableDrawingMode drawingMode = OutlinableDrawingMode.Normal;

        [SerializeField]
        private int outlineLayer = 0;

        [SerializeField]
        private List<OutlineTarget> outlineTargets = new List<OutlineTarget>();

        [SerializeField]
        private RenderStyle renderStyle = RenderStyle.Single;

#pragma warning disable CS0649
        [SerializeField]
        private OutlineProperties outlineParameters = new OutlineProperties();

        [SerializeField]
        private OutlineProperties backParameters = new OutlineProperties();

        [SerializeField]
        private OutlineProperties frontParameters = new OutlineProperties();

#pragma warning restore CS0649

        public RenderStyle RenderStyle
        {
            get
            {
                return renderStyle;
            }

            set
            {
                renderStyle = value;
            }
        }

        public OutlinableDrawingMode DrawingMode
        {
            get
            {
                return drawingMode;
            }
        }

        public int OutlineLayer
        {
            get
            {
                return outlineLayer;
            }

            set
            {
                outlineLayer = value;
            }
        }


        public List<OutlineTarget> OutlineTargets
        {
            get
            {
                return outlineTargets;
            }
        }

        public OutlineProperties OutlineParameters
        {
            get
            {
                return outlineParameters;
            }
        }

        public OutlineProperties BackParameters
        {
            get
            {
                return backParameters;
            }
        }

        public OutlineProperties FrontParameters
        {
            get
            {
                return frontParameters;
            }
        }

        private bool IsVisible(Plane[] planes)
        {
            var visibleCount = 0;
            foreach (var target in outlineTargets)
            {
                if (target.Renderer != null && GeometryUtility.TestPlanesAABB(planes, target.Renderer.bounds))
                    visibleCount++;
            }

            return visibleCount > 0;
        }

        private void Reset()
        {
            AddAllChildRenderersToRenderingList();
        }

        private void OnValidate()
        {
            outlineLayer = Mathf.Clamp(outlineLayer, 0, 63);
        }

        private void OnEnable()
        {
            outlinables.Add(this);
        }

        private void OnDisable()
        {
            outlinables.Remove(this);
        }

        public static void GetAllActiveOutlinables(Camera camera, List<Outlinable> outlinablesList)
        {
            outlinablesList.Clear();
            GeometryUtility.CalculateFrustumPlanes(camera, frustrumPlanes);
            foreach (var outlinable in outlinables)
                if (outlinable.IsVisible(frustrumPlanes))
                    outlinablesList.Add(outlinable);
        }

        private int GetSubmeshCount(Renderer renderer)
        {
            if (renderer is MeshRenderer)
                return renderer.GetComponent<MeshFilter>().sharedMesh.subMeshCount;
            else if (renderer is SkinnedMeshRenderer)
                return (renderer as SkinnedMeshRenderer).sharedMesh.subMeshCount;
            else
                return 1;
        }

        public void AddAllChildRenderersToRenderingList()
        {
            outlineTargets.Clear();
            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                var submeshesCount = GetSubmeshCount(renderer);
                for (var index = 0; index < submeshesCount; index++)
                    outlineTargets.Add(new OutlineTarget(renderer, index));
            }
        }
    }
}