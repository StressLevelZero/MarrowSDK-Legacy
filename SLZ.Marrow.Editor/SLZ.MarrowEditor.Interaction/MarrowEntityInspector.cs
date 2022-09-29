using UnityEngine;
using UnityEditor;
using SLZ.Marrow.Interaction;

namespace SLZ.MarrowEditor.Interaction
{
    [CustomEditor(typeof(MarrowEntity))]
    [CanEditMultipleObjects]
    public class MarrowEntityInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(15);

            var mo = (MarrowEntity)target;
            if (GUILayout.Button("Bake"))
            {
                mo.Editor_Bake();
            }
        }
    }
}