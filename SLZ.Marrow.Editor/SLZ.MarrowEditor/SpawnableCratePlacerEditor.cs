using SLZ.Marrow.Warehouse;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SLZ.MarrowEditor
{
    [CustomEditor(typeof(SpawnableCratePlacer))]
    [CanEditMultipleObjects]
    public class SpawnableCratePlacerEditor : Editor
    {
        SerializedProperty spawnableCrateReferenceProperty;
        SerializedProperty crateQueryProperty;
        SerializedProperty useQueryProperty;
        SerializedProperty manualModeProperty;
        SerializedProperty placedProperty;
        SerializedProperty placingProperty;
        SerializedProperty placedSpawnableProperty;
        SerializedProperty onPlaceEventProperty;

        private static GUIContent previewMeshGizmoIcon = null;
        private static GUIContent colliderBoundsGizmoIcon = null;
        private static GUIContent materialIconOn = null;
        private static GUIContent materialIconOff = null;

        private bool helpText = false;
        private bool defaultInspector = false;
        private int padding = 5;

        public virtual void OnEnable()
        {
            EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
            spawnableCrateReferenceProperty = serializedObject.FindProperty("spawnableCrateReference");
            crateQueryProperty = serializedObject.FindProperty("crateQuery");
            useQueryProperty = serializedObject.FindProperty("useQuery");
            manualModeProperty = serializedObject.FindProperty("manualMode");
            placedProperty = serializedObject.FindProperty("placed");
            placingProperty = serializedObject.FindProperty("placing");
            placedSpawnableProperty = serializedObject.FindProperty("placedSpawnable");
            onPlaceEventProperty = serializedObject.FindProperty("OnPlaceEvent");





            var castedTarget = (SpawnableCratePlacer)target;

            if (castedTarget.transform.gameObject.activeInHierarchy)
            {
                if (PrefabUtility.GetPrefabAssetType(castedTarget.transform.gameObject) == PrefabAssetType.Regular)
                {
                    PrefabUtility.UnpackPrefabInstance(castedTarget.transform.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }
            }


            if (previewMeshGizmoIcon == null)
            {
                previewMeshGizmoIcon = new GUIContent(EditorGUIUtility.IconContent("d_GizmosToggle On@2x"));
                previewMeshGizmoIcon.tooltip = "Toggle Preview Mesh Gizmo";
            }
            if (colliderBoundsGizmoIcon == null)
            {
                colliderBoundsGizmoIcon = new GUIContent(EditorGUIUtility.IconContent("d_BoxCollider2D Icon"));
                colliderBoundsGizmoIcon.tooltip = "Toggle Collider Bounds";
            }
            if (materialIconOn == null)
            {
                materialIconOn = new GUIContent(EditorGUIUtility.IconContent("d_Material On Icon"));
                materialIconOn.tooltip = "Swap Preview Mesh Material";
            }
            if (materialIconOff == null)
            {
                materialIconOff = new GUIContent(EditorGUIUtility.IconContent("d_Material On Icon"));
                materialIconOff.tooltip = "Swap Preview Mesh Material";
            }
        }

        void OnDestroy()
        {
            EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
        }


        public override void OnInspectorGUI()
        {
            helpText = EditorGUILayout.Foldout(helpText, "Usage Info");
            if (helpText)
            {
                EditorGUILayout.LabelField("The Spawnable Crate Placer acts as a spawn point for the entity contained in the Spawnable Crate", EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField("Click the selector button to the right of the 'Spawnable Crate Reference' field to choose a Spawnable Crate or drag a Spawnable Crate from the Asset Warehouse into the 'Spawnable Crate Reference' field.", EditorStyles.wordWrappedLabel);
                EditorGUILayout.Space(padding);
                EditorGUILayout.LabelField("If the 'Manual Mode' checkbox is enabled, the Spawnable will not be placed when the level loads.  Instead, a trigger or event that calls the 'Spawnable Crate Placer' GameObject's PlaceSpawnable() method will place it into the level once activated.", EditorStyles.wordWrappedLabel);
                EditorGUILayout.Space(padding);
                EditorGUILayout.LabelField("The 'On Place Event' list provides the ability to trigger additional events or actions once the Spawnable is placed into the level.", EditorStyles.wordWrappedLabel);
                EditorGUILayout.Space(padding);

                if (GUILayout.Button(new GUIContent("MarrowSDK Documentation - Spawnables", "Open the MarrowSDK Documentation in the default web browser."), MarrowGUIStyles.DefaultButton))
                {
                    Application.OpenURL("https://github.com/StressLevelZero/MarrowSDK/wiki/Spawnables");
                }

                EditorGUILayout.Space(padding);
                defaultInspector = EditorGUILayout.Toggle("(Advanced) Show the Default Inspector", defaultInspector);
            }
            EditorGUILayout.Space(padding);

            serializedObject.Update();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (EditorPrefs.GetBool("UnlockEditingScannables", false))
                {
                    if (useQueryProperty.boolValue)
                    {
                        EditorGUILayout.PropertyField(crateQueryProperty);
                    }
                    else
                    {

                        EditorGUILayout.PropertyField(spawnableCrateReferenceProperty);
                    }

                    EditorGUILayout.PropertyField(useQueryProperty);
                }
                else
                {
                    EditorGUILayout.PropertyField(spawnableCrateReferenceProperty);
                }

                EditorGUILayout.Space(padding);
                EditorGUILayout.PropertyField(manualModeProperty);

                string manualModeTooltip = "";
                manualModeTooltip = manualModeProperty.boolValue ? "Manual Mode enabled: The Placer must be activated manually using a trigger or event that calls the PlaceSpawnable() method" : "Manual Mode disabled: The Spawnable will be placed when the level loads";
                EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), new GUIContent("", manualModeTooltip));

                if (check.changed)
                {
                    foreach (var targetObject in serializedObject.targetObjects)
                    {
                        ((SpawnableCratePlacer)targetObject).ResetName();
                    }
                }
            }

            EditorGUILayout.Space(padding);
            EditorGUILayout.PropertyField(onPlaceEventProperty);

            EditorGUILayout.Space();

#if MARROW_PROJECT
#endif

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Gizmo Options", EditorStyles.boldLabel);
                using (new GUILayout.HorizontalScope())
                {







                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var togglebut = GUILayout.Toggle(SpawnableCratePlacer.showPreviewMesh, previewMeshGizmoIcon, MarrowGUIStyles.DefaultIconButton);
                        if (check.changed)
                        {
                            SpawnableCratePlacer.showPreviewMesh = togglebut;
                            InternalEditorUtility.RepaintAllViews();
                        }
                    }

                    var skinnyIcon = new GUIStyle(MarrowGUIStyles.DefaultIconButton);
                    skinnyIcon.padding = new RectOffset((int)-EditorGUIUtility.singleLineHeight, (int)-EditorGUIUtility.singleLineHeight, 0, 0);

                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var togglebut = GUILayout.Toggle(SpawnableCratePlacer.showColliderBounds, colliderBoundsGizmoIcon, skinnyIcon);
                        if (check.changed)
                        {
                            SpawnableCratePlacer.showColliderBounds = togglebut;
                            InternalEditorUtility.RepaintAllViews();
                        }
                    }

                    if (GUILayout.Button((SpawnableCratePlacer.showLitMaterialPreview ? materialIconOff : materialIconOn), skinnyIcon))
                    {
                        SpawnableCratePlacer.showLitMaterialPreview = !SpawnableCratePlacer.showLitMaterialPreview;
                        InternalEditorUtility.RepaintAllViews();
                    }







                }
            }



            serializedObject.ApplyModifiedProperties();

            if (defaultInspector)
            {
                DrawDefaultInspector();
            }

        }


        void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            if (property.type != "CrateQuery")
                return;

            menu.AddItem(new GUIContent("Run Query"), false, () =>
            {
                foreach (var targetObject in serializedObject.targetObjects)
                {
                    if (targetObject is SpawnableCratePlacer placer)
                    {
                        if (placer.useQuery)
                            placer.crateQuery.RunQuery();
                    }
                }
            });
        }

    }
}