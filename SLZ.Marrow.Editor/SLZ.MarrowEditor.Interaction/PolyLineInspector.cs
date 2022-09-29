using UnityEngine;
using UnityEditor;
using SLZ.Marrow.Interaction;
using Unity.Mathematics;
using UnityEngine.Splines;

namespace SLZ.MarrowEditor.Interaction
{
    [CustomEditor(typeof(PolyLine))]
    [CanEditMultipleObjects]
    public class PolyLineInspector : Editor
    {
        private PolyLineData.SegmentResolution _currentResolution = PolyLineData.SegmentResolution.Decimeter;
        private bool _hasBeenLoaded = false;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PolyLine pl = (PolyLine)target;

            GUILayout.Space(15);
            GUILayout.Label("Bake Settings");

            if (!_hasBeenLoaded && pl.lineData != null)
            {
                _currentResolution = pl.lineData.segmentResolution;
                _hasBeenLoaded = true;
            }

            _currentResolution = (PolyLineData.SegmentResolution)EditorGUILayout.EnumPopup("Segment Resolution", _currentResolution);

            if (GUILayout.Button("Bake PolyLine"))
            {
                BakePolyLine();
            }
        }

        private PolyLineData MakePolyLineAsset()
        {
            PolyLine pl = (PolyLine)target;

            string instanceID = pl.GetInstanceID().ToString();
            var polyLineDataAssets = AssetDatabase.FindAssets($"t:{typeof(PolyLineData)} {pl.name}_PolyLine_{instanceID}");

            if (polyLineDataAssets.Length <= 0)
            {
                PolyLineData asset = ScriptableObject.CreateInstance<PolyLineData>();
                AssetDatabase.CreateAsset(asset, $"Assets/{pl.name}_PolyLine_{instanceID}.asset");
                AssetDatabase.SaveAssets();
                return asset;
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(polyLineDataAssets[0]);
            return AssetDatabase.LoadAssetAtPath<PolyLineData>(assetPath) as PolyLineData;
        }

        private void BakePolyLine()
        {
            PolyLine pl = (PolyLine)target;

            if (pl.lineData == null)
            {
                pl.lineData = MakePolyLineAsset();
                EditorUtility.SetDirty(pl);
                AssetDatabase.SaveAssetIfDirty(pl);
            }

            pl.lineData.segmentResolution = _currentResolution;


            pl.lineData.totalDistance = pl.SplineContainer.Spline.GetLength();
            int vertCount = Mathf.FloorToInt(pl.lineData.totalDistance / pl.lineData.SegmentDistance());
            float remainder = pl.lineData.totalDistance - (vertCount * pl.lineData.SegmentDistance());
            float edgeLength = pl.lineData.SegmentDistance() + (remainder / vertCount);

            pl.lineData.polyVerts = new PolyLineVert[vertCount];

            for (int i = 0; i < vertCount; i++)
            {
                float distance = i * edgeLength;
                float percentage = distance / pl.lineData.totalDistance;

                var vert = new PolyLineVert();

                pl.SplineContainer.Evaluate(percentage, out vert.position, out vert.tangent, out vert.normal);


                vert.tangent = math.normalize(vert.tangent);
                var cross = math.cross(vert.tangent, vert.normal);
                vert.normal = math.normalize(math.cross(cross, vert.tangent));

                pl.lineData.polyVerts[i] = vert;
            }


            for (int i = 0; i < vertCount; i++)
            {
                var vert = pl.lineData.polyVerts[i];
                var nextVert = pl.lineData.polyVerts[PolyLine.Mod(i + 1, vertCount)];

                if (i == (vertCount - 1) && !pl.SplineContainer.Spline.Closed)
                {
                    pl.lineData.polyVerts[i].forward = pl.lineData.polyVerts[i - 1].forward;
                }
                else
                {
                    pl.lineData.polyVerts[i].forward = math.normalize(nextVert.position - vert.position);
                }
            }


            for (int i = vertCount - 1; i >= 0; i--)
            {
                var vert = pl.lineData.polyVerts[i];
                var nextVert = pl.lineData.polyVerts[PolyLine.Mod(i - 1, vertCount)];

                if (i == 0 && !pl.SplineContainer.Spline.Closed)
                {
                    pl.lineData.polyVerts[i].backward = pl.lineData.polyVerts[i + 1].backward;
                }
                else
                {
                    pl.lineData.polyVerts[i].backward = math.normalize(nextVert.position - vert.position);
                }
            }


            for (int i = 0; i < vertCount; i++)
            {
                var vert = pl.lineData.polyVerts[i];

                vert.position = pl.transform.InverseTransformPoint(vert.position);
                vert.tangent = pl.transform.InverseTransformDirection(vert.tangent);
                vert.normal = pl.transform.InverseTransformDirection(vert.normal);
                vert.forward = pl.transform.InverseTransformDirection(vert.forward);
                vert.backward = pl.transform.InverseTransformDirection(vert.backward);

                pl.lineData.polyVerts[i] = vert;
            }


            EditorUtility.SetDirty(pl.lineData);
            AssetDatabase.SaveAssetIfDirty(pl.lineData);
            AssetDatabase.Refresh();



        }

    }
}