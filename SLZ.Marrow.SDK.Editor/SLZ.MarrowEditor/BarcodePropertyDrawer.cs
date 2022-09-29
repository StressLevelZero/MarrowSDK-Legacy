using UnityEngine;
using UnityEditor;
using SLZ.Marrow.Warehouse;


namespace SLZ.MarrowEditor
{
    [CustomPropertyDrawer(typeof(Barcode))]
    public class BarcodePropertyDrawer : PropertyDrawer
    {
        GUIContent errorIcon = null;
        string errorText = "Barcode too long";
        private float indentSize = 0f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            bool prevWordWrap = EditorStyles.textField.wordWrap;
            EditorStyles.textField.wordWrap = true;
            Rect barcodePost = position;
            barcodePost.height = GetBarcodeHeight(property);
            EditorGUI.PropertyField(barcodePost, property.FindPropertyRelative("_id"), label);
            EditorGUI.LabelField(barcodePost, new GUIContent("", property.FindPropertyRelative("_id").stringValue));
            EditorStyles.textField.wordWrap = prevWordWrap;

            Rect warningPos = position;
            warningPos.x += indentSize;
            warningPos.width -= indentSize;
            warningPos.y += barcodePost.height;
            warningPos.height = EditorGUIUtility.singleLineHeight;
            if (!ValidateBarcodeSize(property.FindPropertyRelative("_id").stringValue))
            {
                SetupErrorIcon();
                errorIcon.text = errorText + " " + property.FindPropertyRelative("_id").stringValue.Length + "/" + Barcode.MAX_SIZE;
                EditorGUI.LabelField(warningPos, errorIcon);
            }

            EditorGUI.EndProperty();
        }

        public float GetBarcodeHeight(SerializedProperty property)
        {
            string barcode = property.FindPropertyRelative("_id").stringValue;
            if (barcode.Length > (Barcode.MAX_SIZE * 3f / 4f))
            {
                return EditorGUIUtility.singleLineHeight * 2f;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ValidateBarcodeSize(property.FindPropertyRelative("_id").stringValue))
            {
                return GetBarcodeHeight(property);
            }
            else
            {
                SetupErrorIcon();
                return GetBarcodeHeight(property) + EditorGUIUtility.singleLineHeight;
            }
        }

        public bool ValidateBarcodeSize(string barcode)
        {
            return Barcode.IsValidSize(barcode);
        }

        private void SetupErrorIcon()
        {
            if (errorIcon == null)
            {
                var errorIconUnity = EditorGUIUtility.IconContent("console.erroricon");
                errorIcon = new GUIContent();
                errorIcon.image = errorIconUnity.image;
                errorIcon.text = errorText;
                errorIcon.tooltip = errorIconUnity.tooltip;
            }
        }
    }
}