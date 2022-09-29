using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SLZ.MarrowEditor
{
    public static class SerializedPropertyExtensions
    {
        public static bool TryFindParentArrayProperty(this SerializedProperty serializedProperty, out SerializedProperty parentProperty)
        {






            var propertyPaths = serializedProperty.propertyPath.Split('.');
            parentProperty = default;


            if (propertyPaths.Length > 1 && propertyPaths[^2] == "Array")
            {
                var arrayPath = string.Join('.', propertyPaths.SkipLast(1));
                var arrayProp = serializedProperty.serializedObject.FindProperty(arrayPath);
                if (arrayProp != null)
                {
                    parentProperty = arrayProp;
                    return true;
                }
            }

            return false;
        }
    }
}