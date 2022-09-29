using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SLZ.Marrow.Warehouse;

namespace SLZ.MarrowEditor
{

    public class ScannableEditor : Editor
    {
        protected SerializedProperty barcodeProperty;
        protected SerializedProperty titleProperty;
        protected SerializedProperty descriptionProperty;
        protected SerializedProperty unlockableProperty;
        protected SerializedProperty redactedProperty;

        protected List<SerializedProperty> lockableProperties;

        public virtual void OnEnable()
        {
            EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
            lockableProperties = new List<SerializedProperty>();

            barcodeProperty = serializedObject.FindProperty("_barcode");
            lockableProperties.Add(barcodeProperty);
            titleProperty = serializedObject.FindProperty("_title");
            lockableProperties.Add(titleProperty);
            descriptionProperty = serializedObject.FindProperty("_description");
            lockableProperties.Add(descriptionProperty);
            unlockableProperty = serializedObject.FindProperty("_unlockable");
            redactedProperty = serializedObject.FindProperty("_redacted");
        }

        public virtual void OnDisable()
        {

        }

        void OnDestroy()
        {
            EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            LockedPropertyField(barcodeProperty);
            LockedPropertyField(titleProperty);
            LockedPropertyField(descriptionProperty, false);
            LockedPropertyField(unlockableProperty, false);
            LockedPropertyField(redactedProperty, false);

            if (EditorGUI.EndChangeCheck())
            {
                AssetWarehouse.Instance.LoadPalletsFromAssetDatabase(true);
            }

            serializedObject.ApplyModifiedProperties();
        }


        public static void LockedPropertyField(SerializedProperty prop, bool? lockOverride = null, bool hideProperty = false, GUIContent label = null)
        {
            bool editingLocked = lockOverride.HasValue ? lockOverride.Value : !EditorPrefs.GetBool("UnlockEditingScannables", false);

            if (editingLocked)
            {
                if (!hideProperty)
                {
                    ReadOnlyProperty.DrawReadonlyPropertyEditorGUILayout(prop, label);
                }
            }
            else
            {
                if (label != null)
                    EditorGUILayout.PropertyField(prop, label);
                else
                    EditorGUILayout.PropertyField(prop);

            }
        }

        void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            if (!lockableProperties.Contains(property))
                return;

            if (!typeof(Scannable).IsAssignableFrom(property.serializedObject.targetObject.GetType()))
                return;

            bool unlocked = EditorPrefs.GetBool("UnlockEditingScannables", false);

            if (!unlocked)
            {
                menu.AddItem(new GUIContent("Unlock Editing"), false, () =>
                {
                    EditorPrefs.SetBool("UnlockEditingScannables", true);
                });
            }
            else
            {
                menu.AddItem(new GUIContent("Lock Editing"), false, () =>
                {
                    EditorPrefs.SetBool("UnlockEditingScannables", false);
                });
            }

        }
    }
}