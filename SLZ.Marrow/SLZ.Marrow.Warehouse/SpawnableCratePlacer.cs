using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using SLZ.Marrow.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SLZ.Marrow.Warehouse
{

    [SelectionBase]
    [AddComponentMenu("MarrowSDK/Spawnable Crate Placer")]
    public partial class SpawnableCratePlacer : MonoBehaviour
    {
        [SerializeField] public SpawnableCrateReference spawnableCrateReference = new SpawnableCrateReference(Barcode.EmptyBarcode());
        [SerializeField] public CrateQuery crateQuery = new CrateQuery();
        public bool useQuery = false;

        [SerializeField] public bool manualMode = false;

        [SerializeField]
        public OnPlaceEvent OnPlaceEvent;

#if UNITY_EDITOR

        public static bool showPreviewMesh = true;
        public static bool showLitMaterialPreview = false;
        private static Material defaultLitMat = null;
#endif

        private SpawnableCrateReference GetCrateReference()
        {
            if (AssetWarehouse.ready)
            {
                if (useQuery)
                {
                    return new SpawnableCrateReference(crateQuery.Barcode);
                }
                else
                {
                    return spawnableCrateReference;
                }
            }

            return null;
        }

        [ContextMenu("Place Spawnable")]
        public void PlaceSpawnable()
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Warehouse.SpawnableCratePlacer.PlaceSpawnable()");

            throw new System.NotImplementedException();
        }

#if UNITY_EDITOR

        [DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.NonSelected)]
        private static void DrawPreviewGizmo(SpawnableCratePlacer placer, GizmoType gizmoType)
        {
            if (!Application.isPlaying && placer.gameObject.scene != default)
            {
                if (showLitMaterialPreview && defaultLitMat == null)
                {
                    defaultLitMat = AssetDatabase.LoadAssetAtPath<Material>("Packages/com.unity.render-pipelines.universal/Runtime/Materials/Lit.mat");
                }
                EditorPreviewMeshGizmo.Draw("PreviewMesh", placer.gameObject, placer.GetCrateReference(), showLitMaterialPreview ? defaultLitMat : MarrowSDK.VoidMaterial, !showPreviewMesh, true);
                placer.EditorUpdateName();
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying && gameObject.scene != default)
            {
                EditorUpdateName();
            }
        }

        private void Reset()
        {
            gameObject.name = "Spawnable Placer";
        }

        [ContextMenu("Reset Name")]
        public void ResetName()
        {
            gameObject.name = "Spawnable Placer";
        }

        public void EditorUpdateName()
        {
            if (gameObject.name == "Spawnable Placer" && AssetWarehouse.ready && !Application.isPlaying && AssetWarehouse.Instance.TryGetCrate(GetCrateReference().Barcode, out var crate))
            {
                gameObject.name = useQuery ? "Spawnable Placer (query)" : $"Spawnable Placer ({crate.Title})";
                GameObjectUtility.EnsureUniqueNameForSibling(gameObject);
            }
        }




        [MenuItem("GameObject/MarrowSDK/Spawnable Placer", priority = 1)]
        private static void MenuCreatePlacer(MenuCommand menuCommand)
        {
            GameObject go = EditorCreateSpawnablePlacer();

            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Selection.activeObject = go;
        }

        public static void EditorCreateSpawnablePlacer(Barcode barcode, Transform targetTransform, Transform parentTransform = null)
        {
            if (AssetWarehouse.ready && AssetWarehouse.Instance.TryGetCrate<SpawnableCrate>(barcode, out var crate))
            {
                EditorCreateSpawnablePlacer(crate, targetTransform, parentTransform);
            }
        }

        public static void EditorSwapPrefabForSpawnablePlacer(GameObject prefabInstance, bool deleteOriginalGameObject = true)
        {
            if (PrefabUtility.IsOutermostPrefabInstanceRoot(prefabInstance))
            {
                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(prefabInstance);
                if (prefab != null)
                {
                    if (AssetWarehouse.Instance.EditorObjectCrateLookup.TryGetValue(prefab, out Crate prefabCrate) && prefabCrate is SpawnableCrate spawnableCrate)
                    {
                        EditorCreateSpawnablePlacer(spawnableCrate, prefabInstance.transform, prefabInstance.transform.parent);

                        if (deleteOriginalGameObject)
                        {
                            DeleteOriginalGameObject(prefabInstance).Forget();
                        }
                    }
                }
            }
        }

        private static async UniTaskVoid DeleteOriginalGameObject(GameObject go)
        {
            await UniTask.NextFrame();
            Undo.RecordObject(go, $"Delete Spawnable Placer Original GameObject {go.name}");
            UnityEngine.Object.DestroyImmediate(go);
        }

        public static GameObject EditorCreateSpawnablePlacer(SpawnableCrate crate = null, Transform targetTransform = null, Transform parentTransform = null)
        {
            GameObject go = new GameObject("Auto Spawnable Placer", typeof(SpawnableCratePlacer));
            go.transform.localScale = Vector3.one;
            if (parentTransform != null)
                go.transform.parent = parentTransform;
            if (targetTransform != null)
            {
                go.transform.localPosition = targetTransform.localPosition;
                go.transform.localRotation = targetTransform.localRotation;
            }

            var placer = go.GetComponent<SpawnableCratePlacer>();
            if (crate == null)
                placer.spawnableCrateReference = new SpawnableCrateReference();
            else
                placer.spawnableCrateReference = new SpawnableCrateReference(crate.Barcode);
            placer.EditorUpdateName();
            Undo.RegisterCreatedObjectUndo(go, $"Create Spawnable Placer {(crate != null ? crate.Title : "")}");

            return go;
        }


#endif
    }

    [Serializable]
    public class OnPlaceEvent : UnityEngine.Events.UnityEvent<SpawnableCratePlacer, GameObject> { }
}
