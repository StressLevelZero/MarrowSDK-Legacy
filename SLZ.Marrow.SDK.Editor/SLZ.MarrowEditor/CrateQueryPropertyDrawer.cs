using UnityEditor;
using UnityEngine;
using SLZ.Marrow.Warehouse;

namespace SLZ.MarrowEditor
{
    [CustomPropertyDrawer(typeof(CrateQuery))]
    public class CrateQueryPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);


            var labelPosition = position;
            labelPosition.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(labelPosition, property.isExpanded, label);
            position.y += EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    var linePosition = labelPosition;
                    linePosition.y += EditorGUIUtility.singleLineHeight;

                    EditorGUI.LabelField(linePosition, "WARNING: Experimental feature, likely to break later");
                    linePosition.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(linePosition, property.FindPropertyRelative("tagFilter"));
                    linePosition.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(linePosition, property.FindPropertyRelative("titleFilter"));
                    linePosition.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(linePosition, property.FindPropertyRelative("_barcode"));
                    linePosition.y += EditorGUIUtility.singleLineHeight;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? EditorGUIUtility.singleLineHeight * 5 : EditorGUIUtility.singleLineHeight;
        }
    }
}