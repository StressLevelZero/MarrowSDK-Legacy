using UnityEngine;
using UnityEditor;
using SLZ.Marrow.Warehouse;

namespace SLZ.MarrowEditor
{
    [CustomEditor(typeof(GameObjectCrate))]
    [CanEditMultipleObjects]
    public class GameObjectCrateEditor : CrateEditor
    {
        SerializedProperty previewMeshProperty;
        SerializedProperty previewMeshCustomQualityProperty;
        SerializedProperty previewMeshQualityProperty;
        SerializedProperty previewMeshMaxLodProperty;
        SerializedProperty colliderBoundsProperty;

        private bool previewMeshOptions = false;

        protected override string AssetReferenceDisplayName { get { return "Prefab"; } }

        public override void OnEnable()
        {
            base.OnEnable();

            previewMeshProperty = serializedObject.FindProperty("_previewMesh");
            previewMeshCustomQualityProperty = serializedObject.FindProperty("_customQuality");
            previewMeshQualityProperty = serializedObject.FindProperty("_previewMeshQuality");
            previewMeshMaxLodProperty = serializedObject.FindProperty("_maxLODLevel");
            colliderBoundsProperty = serializedObject.FindProperty("_colliderBounds");
        }

        public override void OnInspectorGUIPackedAssets()
        {
            base.OnInspectorGUIPackedAssets();

            LockedPropertyField(previewMeshProperty, false);
            previewMeshOptions = EditorGUILayout.Foldout(previewMeshOptions, "Preview Mesh Options");
            if (previewMeshOptions)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    LockedPropertyField(previewMeshCustomQualityProperty, false);
                    if (previewMeshCustomQualityProperty.boolValue)
                    {
                        LockedPropertyField(previewMeshQualityProperty, false);
                    }
                    LockedPropertyField(previewMeshMaxLodProperty, false);
                }
            }
            LockedPropertyField(colliderBoundsProperty, false);
        }
    }

    [CustomPreview(typeof(SpawnableCrate))]
    public class GameObjectCratePreview : CratePreview { }
}