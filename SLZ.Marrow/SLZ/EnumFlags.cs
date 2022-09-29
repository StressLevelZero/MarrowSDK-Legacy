





using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SLZ
{

    public class EnumFlags : PropertyAttribute
    {
        public EnumFlags() { }
    }


#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(EnumFlags))]
    public class EnumFlagsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        }
    }
#endif
}
