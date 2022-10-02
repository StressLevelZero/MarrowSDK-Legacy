using UnityEngine;
using SLZ.Marrow.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SLZ.Marrow.SceneStreaming
{
    [AddComponentMenu("MarrowSDK/Player Marker")]
    public class PlayerMarker : MonoBehaviour
    {

#if UNITY_EDITOR
        [DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.NonSelected)]
        private static void DrawPreviewGizmo(PlayerMarker playerMarker, GizmoType gizmoType)
        {
            if (!Application.isPlaying && playerMarker.gameObject.scene != default)
            {
                EditorMeshGizmo.Draw("PlayerMarker Preview", playerMarker.gameObject, MarrowSDK.GenericHumanMesh, MarrowSDK.VoidMaterialAlt, MarrowSDK.GenericHumanMesh.bounds);
            }
        }

        [MenuItem("GameObject/MarrowSDK/Player Marker", priority = 1)]
        private static void MenuCreatePlacer(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Player Marker", typeof(PlayerMarker));
            go.transform.localScale = Vector3.one;

            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Selection.activeObject = go;
        }
#endif
    }
}