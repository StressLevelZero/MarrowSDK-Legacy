using System.Collections;
using SLZ.Marrow;
using UnityEngine;
using UnityEditor;
using SLZ.Marrow.Warehouse;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace SLZ.MarrowEditor
{
    [CustomEditor(typeof(CrateT<>), true)]
    public class CrateTEditor : CrateEditor { }

    [CustomEditor(typeof(Crate))]
    [CanEditMultipleObjects]
    public class CrateEditor : ScannableEditor
    {
        SerializedProperty palletProperty;
        SerializedProperty tagsProperty;
        SerializedProperty assetReferenceProperty;
        protected SerializedProperty additionalAssetReferencesProperty;

        protected Crate crate = null;

        protected virtual string AssetReferenceDisplayName { get { return "Asset Reference"; } }
        protected virtual string AdditionalAssetReferencesDisplayName { get { return "Additional Asset References"; } }

        public override void OnEnable()
        {
            base.OnEnable();

            palletProperty = serializedObject.FindProperty("_pallet");
            tagsProperty = serializedObject.FindProperty("_tags");
            assetReferenceProperty = serializedObject.FindProperty("_mainAsset");
            additionalAssetReferencesProperty = serializedObject.FindProperty("_additionalAssetReferences");

            crate = serializedObject.targetObject as Crate;
        }

        public override void OnDisable()
        {

        }

        [OnOpenAsset]
        public static bool OpenAssetHandler(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);

            if (obj is Crate crate && crate.MainAsset.EditorAsset != null)
            {
                switch (crate)
                {
                    case LevelCrate levelCrate:
                        if (AssetDatabase.CanOpenAssetInEditor(levelCrate.MainAsset.EditorAsset.GetInstanceID()))
                        {
                            if (levelCrate.MultiScene)
                            {
                                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                                {
                                    EditorSceneManager.RestoreSceneManagerSetup(levelCrate.ToEditorSceneSetups());
                                    Selection.activeObject = crate;
                                }

                                return true;
                            }
                            else
                            {
                                AssetDatabase.OpenAsset(levelCrate.MainAsset.EditorAsset);
                                Selection.activeObject = crate;
                                return true;
                            }
                        }
                        break;
                    case SpawnableCrate spawnableCrate:
                    default:
                        if (AssetDatabase.CanOpenAssetInEditor(crate.MainAsset.EditorAsset.GetInstanceID()))
                        {
                            AssetDatabase.OpenAsset(crate.MainAsset.EditorAsset);
                            Selection.activeObject = crate;
                            return true;
                        }
                        break;
                }


            }

            return false;
        }


        public virtual void OnInspectorGUIBody() { }

        public virtual void OnInspectorGUIPackedAssets() { }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            if (!string.IsNullOrEmpty(AssetReferenceDisplayName))
                EditorGUILayout.PropertyField(assetReferenceProperty, new GUIContent(AssetReferenceDisplayName));

            else
                EditorGUILayout.PropertyField(assetReferenceProperty);




            EditorGUI.BeginChangeCheck();










            OnInspectorGUIBody();

            LockedPropertyField(palletProperty, null, true);
            LockedPropertyField(tagsProperty, false);

            if (EditorGUI.EndChangeCheck())
            {
                AssetWarehouse.Instance.LoadPalletsFromAssetDatabase(true);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Packed Assets", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                if (GUILayout.Button("Generate Packed Assets", GUILayout.ExpandWidth(false)))
                {
                    foreach (var tar in targets)
                    {
                        if (tar is Crate tarCrate)
                        {
                            tarCrate.GeneratePackedAssets();
                        }
                    }
                }

                OnInspectorGUIPackedAssets();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomPreview(typeof(CrateT<>))]
    public class CrateTPreview : ObjectPreview { }


    [CustomPreview(typeof(Crate))]
    public class CratePreview : ObjectPreview
    {
        protected Editor assetEditor = null;
        protected bool hasAssetPreview = false;
        protected PreviewRenderUtility previewRender = null;
        protected string previewTarget = null;
        protected Texture previewTexture = null;
        protected GameObject previewGameObject = null;
        protected Bounds previewBounds = default;
        protected int previewMeshTriangles = 0;
        protected int fullTris = 0;
        protected int previewMeshVerts = 0;
        protected int fullVerts = 0;
        protected Vector2 m_PreviewDir = new Vector2(120, -20);
        protected Rect prevRect = default;

        protected bool showPreviewMesh = true;
        protected bool showColliderBounds = true;

        public override void Initialize(Object[] targets)
        {
            base.Initialize(targets);

            ClearCachedAssets();
        }

        public override bool HasPreviewGUI()
        {
            if (m_Targets.Length > 1)
                return false;
            var crate = (Crate)target;



            return crate is GameObjectCrate objectCrate && (Application.isPlaying || (objectCrate.PreviewMesh != null && objectCrate.PreviewMesh.EditorAsset != null));
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            Crate crate = (Crate)target;

            if (crate != null)
            {
                var direction = Drag2D(m_PreviewDir, r);
                if (direction != m_PreviewDir)
                {
                    m_PreviewDir = direction;
                    ClearCachedTexture();
                }

                var barStyle = new GUIStyle();
                barStyle.alignment = TextAnchor.MiddleLeft;
                using (new GUILayout.HorizontalScope(barStyle))
                {
                    EditorGUI.BeginChangeCheck();
                    GUILayout.Button(EditorGUIUtility.IconContent("refresh"), GUILayout.ExpandWidth(false));
                    showPreviewMesh = GUILayout.Toggle(showPreviewMesh, "Preview Mesh", GUILayout.ExpandWidth(false));
                    showColliderBounds = GUILayout.Toggle(showColliderBounds, "Collider Bounds", GUILayout.ExpandWidth(false));

                    GUILayout.FlexibleSpace();

                    EditorGUILayout.LabelField("Preview Mesh Triangles", GUI.skin.box, GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField($"{previewMeshTriangles.ToString()}/{fullTris.ToString()}", GUI.skin.box, GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Vertices", GUI.skin.box, GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField($"{previewMeshVerts.ToString()}/{fullVerts.ToString()}", GUI.skin.box, GUILayout.ExpandWidth(false));
                    if (EditorGUI.EndChangeCheck())
                    {
                        ClearCachedAssets();
                    }
                }

                if ((r.width < 5f && r.height < 5f))
                {
                    return;
                }

                if (prevRect != r)
                {
                    prevRect = r;
                    ClearCachedTexture();
                }

                if (crate.Barcode != previewTarget)
                {
                    ClearCachedAssets();
                }

                if (previewTexture == null)
                {
                    CaptureRenderPreview(crate, r, background);
                }

                if (previewTexture != null)
                {
                    GUI.DrawTexture(r, previewTexture, ScaleMode.StretchToFill, false);
                }

            }
        }
        static int sliderHash = "Slider".GetHashCode();
        public static Vector2 Drag2D(Vector2 scrollPosition, Rect position)
        {
            int id = GUIUtility.GetControlID(sliderHash, FocusType.Passive);
            Event evt = Event.current;
            switch (evt.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (position.Contains(evt.mousePosition) && position.width > 50)
                    {
                        GUIUtility.hotControl = id;
                        evt.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        scrollPosition -= evt.delta * (evt.shift ? 3 : 1) / Mathf.Min(position.width, position.height) * 140.0f;
                        evt.Use();
                        GUI.changed = true;
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                        GUIUtility.hotControl = 0;
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    break;
            }
            return scrollPosition;
        }

        private void CaptureRenderPreview(Crate crate, Rect r, GUIStyle background)
        {
            if (previewRender == null)
            {
                SetupPreviewRender(crate, r, background);
            }

            previewTexture = CreatePreviewTexture(r);
        }

        private RenderTexture CreatePreviewTexture(Rect r)
        {
            float halfSize = Mathf.Max(previewBounds.extents.magnitude, 0.0001f);
            float distance = halfSize * 3.8f;
            Quaternion rot = Quaternion.Euler(-m_PreviewDir.y, -m_PreviewDir.x, 0);
            Vector3 pos = previewBounds.center - rot * (Vector3.forward * distance);
            previewRender.camera.transform.position = pos;
            previewRender.camera.transform.rotation = rot;

            previewRender.BeginPreview(new Rect(r), GUIStyle.none);

            previewRender.Render(true);

            var renderTexture = (RenderTexture)previewRender.EndPreview();
            renderTexture.name = "CrateEditorPreview RenderTexture";

            return renderTexture;
        }

        private void SetupPreviewRender(Crate crate, Rect r, GUIStyle background)
        {
            ClearCachedAssets();
            GameObjectCrate gameObjectCrate = (GameObjectCrate)crate;

            previewRender = new PreviewRenderUtility(true);

            System.GC.SuppressFinalize(previewRender);

            previewBounds = gameObjectCrate.ColliderBounds;
            if (previewBounds.extents == Vector3.zero)
                previewBounds = new Bounds(Vector3.zero, Vector3.one);

            fullTris = 0;
            fullVerts = 0;
            if (gameObjectCrate.MainGameObject != null && gameObjectCrate.MainGameObject.EditorAsset != null)
            {
                foreach (var meshFilter in gameObjectCrate.MainGameObject.EditorAsset.GetComponentsInChildren<MeshFilter>())
                {
                    if (meshFilter != null && meshFilter.sharedMesh != null && meshFilter.gameObject.activeSelf)
                    {
                        var sharedMesh = meshFilter.sharedMesh;
                        fullTris += sharedMesh.triangles.Length;
                        fullVerts += sharedMesh.vertexCount;
                    }
                }
            }
            else
            {
                fullTris = -1;
                fullVerts = -1;
            }

            previewTarget = crate.Barcode;

            previewRender.lights[0].transform.localEulerAngles = new Vector3(30, 30, 0);
            previewRender.lights[0].intensity = 2;

            previewRender.ambientColor = new Color(.1f, .1f, .1f, 0);

            SetupPreviewRenderGameObjects();

            float halfSize = Mathf.Max(previewBounds.extents.magnitude, 0.0001f);
            float distance = halfSize * 3.8f;

            var camera = previewRender.camera;
            camera.nearClipPlane = 0.001f;
            camera.farClipPlane = 1000;
            camera.fieldOfView = 30f;



            SetupPreviewRenderBoundBox(gameObjectCrate.ColliderBounds);

            Mesh previewMesh = null;
            if (gameObjectCrate.PreviewMesh != null && gameObjectCrate.PreviewMesh.EditorAsset != null)
            {
                previewMesh = gameObjectCrate.PreviewMesh.EditorAsset;
                previewBounds = previewMesh.bounds;

                if (previewMesh != null)
                {
                    previewMeshTriangles = previewMesh.triangles.Length;
                    previewMeshVerts = previewMesh.vertexCount;

                    SetupPreviewRenderMainMesh(previewMesh);
                }
            }
            else if (Application.isPlaying && gameObjectCrate.PreviewMesh != null)
            {
                gameObjectCrate.PreviewMesh.LoadAsset(loadedMesh =>
                {
                    previewMesh = loadedMesh;
                    previewBounds = loadedMesh.bounds;

                    previewMeshTriangles = previewMesh.triangles.Length;
                    previewMeshVerts = previewMesh.vertexCount;

                    float halfSize = Mathf.Max(previewBounds.extents.magnitude, 0.0001f);
                    float distance = halfSize * 3.8f;

                    var camera = previewRender.camera;
                    camera.nearClipPlane = 0.001f;
                    camera.farClipPlane = 1000;
                    camera.fieldOfView = 30f;



                    SetupPreviewRenderMainMesh(previewMesh);
                    ClearCachedTexture();

                });
            }

        }

        private void SetupPreviewRenderGameObjects()
        {
            previewGameObject = new GameObject("Preview Render GameObject");
            previewRender.AddSingleGO(previewGameObject);
        }

        private void SetupPreviewRenderMainMesh(Mesh mesh)
        {
            var previewObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            previewObject.transform.parent = previewGameObject.transform;
            previewObject.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterial;
            previewObject.GetComponent<MeshFilter>().mesh = mesh;
            previewObject.GetComponent<MeshRenderer>().enabled = showPreviewMesh;
        }

        private void SetupPreviewRenderBoundBox(Bounds bounds)
        {
            if (bounds.extents != Vector3.zero)
            {
                var boundGO = new GameObject("bound box");
                boundGO.transform.parent = previewGameObject.transform;
                GameObject boundGOBox;

                float width = bounds.extents.magnitude / 100f;

                boundGOBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                boundGOBox.transform.position = bounds.center;
                boundGOBox.transform.localScale = new Vector3(width * 2f, width * 2f, width * 2f);
                boundGOBox.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterialAlt;
                boundGOBox.GetComponent<MeshRenderer>().enabled = showColliderBounds;
                boundGOBox.transform.SetParent(boundGO.transform);


                boundGOBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                boundGOBox.transform.position = bounds.center + Vector3.forward * bounds.extents.z + Vector3.up * bounds.extents.y;
                boundGOBox.transform.localScale = new Vector3(bounds.size.x, width, width);
                boundGOBox.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterialAlt;
                boundGOBox.GetComponent<MeshRenderer>().enabled = showColliderBounds;
                boundGOBox.transform.SetParent(boundGO.transform);

                boundGOBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                boundGOBox.transform.position = bounds.center + Vector3.back * bounds.extents.z + Vector3.up * bounds.extents.y;
                boundGOBox.transform.localScale = new Vector3(bounds.size.x, width, width);
                boundGOBox.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterialAlt;
                boundGOBox.GetComponent<MeshRenderer>().enabled = showColliderBounds;
                boundGOBox.transform.SetParent(boundGO.transform);

                boundGOBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                boundGOBox.transform.position = bounds.center + Vector3.forward * bounds.extents.z + Vector3.down * bounds.extents.y;
                boundGOBox.transform.localScale = new Vector3(bounds.size.x, width, width);
                boundGOBox.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterialAlt;
                boundGOBox.GetComponent<MeshRenderer>().enabled = showColliderBounds;
                boundGOBox.transform.SetParent(boundGO.transform);

                boundGOBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                boundGOBox.transform.position = bounds.center + Vector3.back * bounds.extents.z + Vector3.down * bounds.extents.y;
                boundGOBox.transform.localScale = new Vector3(bounds.size.x, width, width);
                boundGOBox.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterialAlt;
                boundGOBox.GetComponent<MeshRenderer>().enabled = showColliderBounds;
                boundGOBox.transform.SetParent(boundGO.transform);


                boundGOBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                boundGOBox.name = "Crate Preview Bound Box";
                boundGOBox.transform.position = bounds.center + Vector3.forward * bounds.extents.z + Vector3.right * bounds.extents.x;
                boundGOBox.transform.localScale = new Vector3(width, bounds.size.y, width);
                boundGOBox.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterialAlt;
                boundGOBox.GetComponent<MeshRenderer>().enabled = showColliderBounds;
                boundGOBox.transform.SetParent(boundGO.transform);

                boundGOBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                boundGOBox.transform.position = bounds.center + Vector3.back * bounds.extents.z + Vector3.right * bounds.extents.x;
                boundGOBox.transform.localScale = new Vector3(width, bounds.size.y, width);
                boundGOBox.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterialAlt;
                boundGOBox.GetComponent<MeshRenderer>().enabled = showColliderBounds;
                boundGOBox.transform.SetParent(boundGO.transform);

                boundGOBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                boundGOBox.transform.position = bounds.center + Vector3.forward * bounds.extents.z + Vector3.left * bounds.extents.x;
                boundGOBox.transform.localScale = new Vector3(width, bounds.size.y, width);
                boundGOBox.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterialAlt;
                boundGOBox.GetComponent<MeshRenderer>().enabled = showColliderBounds;
                boundGOBox.transform.SetParent(boundGO.transform);

                boundGOBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                boundGOBox.transform.position = bounds.center + Vector3.back * bounds.extents.z + Vector3.left * bounds.extents.x;
                boundGOBox.transform.localScale = new Vector3(width, bounds.size.y, width);
                boundGOBox.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterialAlt;
                boundGOBox.GetComponent<MeshRenderer>().enabled = showColliderBounds;
                boundGOBox.transform.SetParent(boundGO.transform);


                boundGOBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                boundGOBox.transform.position = bounds.center + Vector3.up * bounds.extents.y + Vector3.right * bounds.extents.x;
                boundGOBox.transform.localScale = new Vector3(width, width, bounds.size.z);
                boundGOBox.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterialAlt;
                boundGOBox.GetComponent<MeshRenderer>().enabled = showColliderBounds;
                boundGOBox.transform.SetParent(boundGO.transform);

                boundGOBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                boundGOBox.transform.position = bounds.center + Vector3.down * bounds.extents.y + Vector3.right * bounds.extents.x;
                boundGOBox.transform.localScale = new Vector3(width, width, bounds.size.z);
                boundGOBox.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterialAlt;
                boundGOBox.GetComponent<MeshRenderer>().enabled = showColliderBounds;
                boundGOBox.transform.SetParent(boundGO.transform);

                boundGOBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                boundGOBox.transform.position = bounds.center + Vector3.up * bounds.extents.y + Vector3.left * bounds.extents.x;
                boundGOBox.transform.localScale = new Vector3(width, width, bounds.size.z);
                boundGOBox.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterialAlt;
                boundGOBox.GetComponent<MeshRenderer>().enabled = showColliderBounds;
                boundGOBox.transform.SetParent(boundGO.transform);

                boundGOBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                boundGOBox.transform.position = bounds.center + Vector3.down * bounds.extents.y + Vector3.left * bounds.extents.x;
                boundGOBox.transform.localScale = new Vector3(width, width, bounds.size.z);
                boundGOBox.GetComponent<MeshRenderer>().material = MarrowSDK.VoidMaterialAlt;
                boundGOBox.GetComponent<MeshRenderer>().enabled = showColliderBounds;
                boundGOBox.transform.SetParent(boundGO.transform);
            }
        }

        public void ClearCachedTexture()
        {
            if (previewTexture != null)
            {
                UnityEngine.Object.DestroyImmediate(previewTexture);
                previewTexture = null;
            }
        }

        public void ClearCachedAssets()
        {
            if (previewRender != null)
            {
                previewRender.Cleanup();
                previewRender = null;
            }

            ClearCachedTexture();

            if (previewGameObject != null)
            {
                UnityEngine.Object.DestroyImmediate(previewGameObject);
                previewGameObject = null;
            }

            previewMeshTriangles = 0;
            fullTris = 0;
            previewMeshVerts = 0;
            fullVerts = 0;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            ClearCachedAssets();
        }


    }

    [InitializeOnLoad]
    static class CrateEditorGUI
    {
        static CrateEditorGUI()
        {
            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }

        static void OnPostHeaderGUI(Editor editor)
        {
            using (new GUILayout.VerticalScope())
            {
                if (editor != null && editor.targets.Length > 0)
                {
                    Object target = editor.target;
                    if (target != null && AssetWarehouse.ready)
                    {
                        if (AssetWarehouse.Instance.EditorObjectCrateLookup.TryGetValue(target, out Crate crate))
                        {
                            EditorGUILayout.ObjectField("Crate", crate, crate.GetType(), false);
                        }
                    }
                }
            }
        }
    }
}