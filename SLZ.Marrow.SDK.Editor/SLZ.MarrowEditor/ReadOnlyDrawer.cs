using UnityEngine;
using UnityEditor;
using SLZ.Marrow.Utilities;

namespace SLZ.MarrowEditor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReadOnlyAttribute readOnlyAttribute = attribute as ReadOnlyAttribute;
            ReadOnlyProperty.DrawReadonlyProperty(position, property, label, readOnlyAttribute.includeChildren);
        }
    }

    public static class ReadOnlyProperty
    {
        public static void DrawReadonlyProperty(Rect position, SerializedProperty property, GUIContent label = null, bool includeChildren = false)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, includeChildren);
            EditorGUI.EndDisabledGroup();
        }

        public static void DrawReadonlyPropertyEditorGUILayout(SerializedProperty property, GUIContent label = null, bool includeChildren = false)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(property, label, includeChildren);
            EditorGUI.EndDisabledGroup();
        }
    }
}