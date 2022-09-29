using UnityEngine;
using UnityEditor;
using SLZ.Marrow.Interaction;

namespace SLZ.MarrowEditor.Interaction
{
    [CustomEditor(typeof(SplineJoint))]
    [CanEditMultipleObjects]
    public class SplineJointInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(15);

            SplineJoint sj = (SplineJoint)target;
            if (GUILayout.Button("Attach to Spline"))
            {
                sj.PlaceOnSpline();
            }
        }
    }
}