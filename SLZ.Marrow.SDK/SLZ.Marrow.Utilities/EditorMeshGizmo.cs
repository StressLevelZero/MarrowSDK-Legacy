#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Vector3 = UnityEngine.Vector3;

namespace SLZ.Marrow.Utilities
{
    public class EditorMeshGizmo : MonoBehaviour
    {
        public readonly static string EditorMeshGizmoName = "Editor Mesh Gizmo";

        [SerializeField]
        private string id;

        public string ID
        {
            get => id;
            set => id = value;
        }

        [SerializeField] private Mesh _editorMesh;
        public Mesh EditorMesh
        {
            get
            {
                return _editorMesh;
            }
            set
            {
                if (_editorMesh != value)
                {
                    _editorMesh = value;
                    if (meshFilter != null)
                    {
                        meshFilter.sharedMesh = _editorMesh;
                    }
                }
            }
        }

        [SerializeField] private Material editorMaterial;
        public Material EditorMaterial
        {
            get
            {
                return editorMaterial;
            }
            set
            {
                if (editorMaterial != value)
                {
                    editorMaterial = value;

                    if (meshRenderer != null)
                    {
                        meshRenderer.sharedMaterial = editorMaterial;
                    }
                }
            }
        }

        [SerializeField]
        private Bounds? _editorBounds = null;

        public Bounds? EditorBounds
        {
            get => _editorBounds;
            set => _editorBounds = value;
        }

        [SerializeField]
        private bool _showInPlaymode = false;

        public bool ShowInPlaymode
        {
            get { return _showInPlaymode; }
            set { _showInPlaymode = value; }
        }

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;

        private static readonly Dictionary<(string, GameObject), EditorMeshGizmo> meshGizmoCache = new Dictionary<(string, GameObject), EditorMeshGizmo>();

        public void DrawBounds()
        {
            if (EditorBounds.HasValue || EditorBounds.Value != default)
            {
                Bounds bounds = EditorBounds.Value;

                var color = Gizmos.color;
                var transform1 = transform;
                var position = transform1.position;
                var rotation = transform1.rotation;
                Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

                Gizmos.color = new Color(0f, 0.86f, 0.91f);
                Gizmos.matrix *= Matrix4x4.TRS(position, rotation, Vector3.one);
                Gizmos.matrix *= Matrix4x4.Translate(bounds.center);
                Gizmos.DrawWireCube(Vector3.zero, bounds.size);

                Gizmos.color = color;
                Gizmos.matrix = oldGizmosMatrix;
            }
        }

        public static EditorMeshGizmo Draw(string id, GameObject targetGameObject, Mesh mesh, Material material, Bounds bounds = default, bool showInPlayMode = false)
        {
            return Draw<EditorMeshGizmo>(id, targetGameObject, mesh, material, bounds, showInPlayMode);
        }

        protected static EditorGizmoT Draw<EditorGizmoT>(string id, GameObject targetGameObject, Material material, Bounds bounds = default, bool showInPlayMode = false) where EditorGizmoT : EditorMeshGizmo
        {
            return Draw<EditorGizmoT>(id, targetGameObject, null, material, bounds, showInPlayMode);
        }

        protected static EditorGizmoT Draw<EditorGizmoT>(string id, GameObject targetGameObject, Mesh mesh, Material material, Bounds bounds = default, bool showInPlayMode = false) where EditorGizmoT : EditorMeshGizmo
        {
            EditorGizmoT editorMeshGizmo = null;
            if (!Application.isPlaying)
            {
                if (meshGizmoCache.TryGetValue((id, targetGameObject), out var foundEditorMeshGizmo))
                {
                    editorMeshGizmo = foundEditorMeshGizmo as EditorGizmoT;
                }
                else
                {
                    var existingGizmos = targetGameObject.GetComponents<EditorGizmoT>();
                    foreach (var existingGizmo in existingGizmos)
                    {
                        if (existingGizmo.ID == id)
                        {
                            editorMeshGizmo = existingGizmo;
                            meshGizmoCache[(id, targetGameObject)] = editorMeshGizmo;

                        }
                    }
                }

                if (editorMeshGizmo == null)
                {
                    editorMeshGizmo = SetupGizmo<EditorGizmoT>(id, targetGameObject, mesh, material, bounds, showInPlayMode);
                    meshGizmoCache[(id, targetGameObject)] = editorMeshGizmo;
                }


                if (editorMeshGizmo != null)
                {
                    editorMeshGizmo.EditorMesh = mesh;
                    editorMeshGizmo.EditorBounds = bounds;
                    editorMeshGizmo.EditorMaterial = material;

                    editorMeshGizmo.DrawBounds();
                }

            }

            return editorMeshGizmo;
        }

        private static EditorGizmoT SetupGizmo<EditorGizmoT>(string id, GameObject targetGameObject, Mesh mesh = null, Material material = null, Bounds bounds = default, bool showInPlayMode = false) where EditorGizmoT : EditorMeshGizmo
        {
            GameObjectUtility.SetStaticEditorFlags(targetGameObject, 0);
            EditorGizmoT editorMeshGizmo = targetGameObject.AddComponent<EditorGizmoT>();
            editorMeshGizmo.ID = id;
            editorMeshGizmo.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.HideInInspector;
            targetGameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            editorMeshGizmo.ShowInPlaymode = showInPlayMode;

            var meshFilter = targetGameObject.GetComponent<MeshFilter>();
            editorMeshGizmo.meshFilter = meshFilter != null ? meshFilter : targetGameObject.AddComponent<MeshFilter>();
            editorMeshGizmo.meshFilter.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.HideInInspector;

            var meshRenderer = targetGameObject.GetComponent<MeshRenderer>();
            editorMeshGizmo.meshRenderer = meshRenderer != null ? meshRenderer : targetGameObject.AddComponent<MeshRenderer>();
            editorMeshGizmo.meshRenderer.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.HideInInspector;
            editorMeshGizmo.meshRenderer.receiveGI = 0;
            editorMeshGizmo.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            editorMeshGizmo.meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            editorMeshGizmo.meshRenderer.allowOcclusionWhenDynamic = false;

            if (mesh != null)
            {
                editorMeshGizmo.EditorMesh = mesh;
            }
            if (material != null)
            {
                editorMeshGizmo.EditorMaterial = material;
            }
            if (bounds != default)
            {
                editorMeshGizmo.EditorBounds = bounds;
            }

            return editorMeshGizmo;
        }
    }
}
#endif

