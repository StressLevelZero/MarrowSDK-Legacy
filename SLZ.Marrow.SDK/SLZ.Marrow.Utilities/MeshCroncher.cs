using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SLZ.Marrow.Utilities
{
    public class MeshCroncher
    {
        public static Mesh CronchMesh(GameObject gameObject, int maxLODLevel = 0, float quality = 0.5f, bool forceQuality = false)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool includeInactive = false;
            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>(includeInactive);
            SkinnedMeshRenderer[] skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive);
            var lodGroups = gameObject.GetComponentsInChildren<LODGroup>(includeInactive);
            Matrix4x4 prefabMatrix = Matrix4x4.TRS(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.localScale);

            List<CombineInstance> combine = new List<CombineInstance>();

#if UNITY_EDITOR
#if PLACER_FIXED
#endif
#endif





















            int targetLODValue = maxLODLevel;
            bool stripOtherLODs = true;
            var allLODRenderers = new List<Renderer>();
            var validLODRenderers = new List<Renderer>();

            if (stripOtherLODs)
            {
                foreach (var lodGroup in lodGroups)
                {
                    int maxLOD = Mathf.Min(targetLODValue, lodGroup.lodCount - 1);
                    var lods = lodGroup.GetLODs();
                    for (var lodLevel = 0; lodLevel < lods.Length; lodLevel++)
                    {
                        var lod = lods[lodLevel];
                        foreach (var lodRenderer in lod.renderers)
                        {
                            if (!allLODRenderers.Contains(lodRenderer))
                            {
                                allLODRenderers.Add(lodRenderer);
                            }

                            if (lodLevel == maxLOD)
                            {
                                if (!validLODRenderers.Contains(lodRenderer))
                                {
                                    validLODRenderers.Add(lodRenderer);
                                }
                            }
                        }
                    }
                }
            }



            foreach (var meshFilter in meshFilters)
            {
                var meshRenderer = meshFilter.gameObject.GetComponent<MeshRenderer>();
                if (meshRenderer != null && meshRenderer.enabled
                    && meshFilter.gameObject.activeSelf && meshFilter.sharedMesh != null && (!stripOtherLODs || (!allLODRenderers.Contains(meshRenderer) || validLODRenderers.Contains(meshRenderer))))
                {
                    for (int j = 0; j < meshFilter.sharedMesh.subMeshCount; j++)
                    {
                        CombineInstance instance = new CombineInstance();
                        instance.mesh = (Mesh)UnityEngine.Object.Instantiate(meshFilter.sharedMesh);
                        instance.mesh.name += " Cronchin";
                        instance.subMeshIndex = j;
                        instance.transform = prefabMatrix.inverse * meshFilter.transform.localToWorldMatrix;
                        combine.Add(instance);
                    }
                }
            }


            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                if (skinnedMeshRenderer.gameObject.activeSelf && skinnedMeshRenderer.enabled && (!stripOtherLODs || (!allLODRenderers.Contains(skinnedMeshRenderer) || validLODRenderers.Contains(skinnedMeshRenderer))))
                {
                    Mesh tempMesh = new Mesh();
                    tempMesh.name = "MarrowSDK-TempMesh";
                    skinnedMeshRenderer.BakeMesh(tempMesh);

                    tempMesh.RecalculateNormals();
                    tempMesh.RecalculateTangents();
                    tempMesh.RecalculateBounds();

                    for (int j = 0; j < tempMesh.subMeshCount; j++)
                    {
                        CombineInstance instance = new CombineInstance();
                        instance.mesh = tempMesh;
                        instance.subMeshIndex = j;
                        instance.transform = prefabMatrix.inverse * skinnedMeshRenderer.transform.localToWorldMatrix;
                        combine.Add(instance);
                    }
                }
            }



            Dictionary<Mesh, Mesh> meshesToCronch = new Dictionary<Mesh, Mesh>();
            foreach (var combineInstance in combine)
            {
                if (!meshesToCronch.ContainsKey(combineInstance.mesh))
                {
                    meshesToCronch.Add(combineInstance.mesh, null);
                }
            }

            var keyList = meshesToCronch.Keys.ToList();
            foreach (var meshKey in keyList)
            {
                AutoSimplifyMesh(meshKey, out var newMesh, quality, forceQuality);
                meshesToCronch[meshKey] = newMesh;
            }

            for (int i = combine.Count - 1; i >= 0; i--)
            {
                var meshPart = combine[i];
                if (meshesToCronch.TryGetValue(meshPart.mesh, out var crunchedMesh))
                {
                    meshPart.mesh = crunchedMesh;
                    combine[i] = meshPart;
                }
            }

            var combinedMesh = new Mesh();
            combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            try
            {
                combinedMesh.CombineMeshes(combine.ToArray(), true, true, true);
            }
            catch (Exception e)
            {
                Debug.Log("mesh too big");
                Debug.LogException(e);
            }

            foreach (var meshCronch in meshesToCronch)
            {
                if (meshCronch.Key != null)
                {
                    UnityEngine.Object.DestroyImmediate(meshCronch.Key);
                }
                if (meshCronch.Value != null)
                {
                    UnityEngine.Object.DestroyImmediate(meshCronch.Value);
                }
            }

            foreach (var combineMesh in combine)
            {
                if (combineMesh.mesh != null)
                {
                    UnityEngine.Object.DestroyImmediate(combineMesh.mesh);
                }
            }

            AutoSimplifyMesh(combinedMesh, out var bakedMesh, quality, forceQuality, gameObject.name, true);

            UnityEngine.Object.DestroyImmediate(combinedMesh);

            stopwatch.Stop();


            return bakedMesh;
        }

        public static async UniTask<Mesh> CronchMeshAsync(GameObject gameObject)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>(true);
            SkinnedMeshRenderer[] skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            Matrix4x4 prefabMatrix = Matrix4x4.TRS(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.localScale);

            List<CombineInstance> combine = new List<CombineInstance>();

#if UNITY_EDITOR
#if PLACER_FIXED
#endif
#endif























            foreach (var meshFilter in meshFilters)
            {
                var meshRenderer = meshFilter.gameObject.GetComponent<MeshRenderer>();
                if (meshRenderer != null && meshRenderer.enabled
                    && meshFilter.gameObject.activeSelf && meshFilter.sharedMesh != null)
                {
                    for (int j = 0; j < meshFilter.sharedMesh.subMeshCount; j++)
                    {
                        CombineInstance instance = new CombineInstance();
                        instance.mesh = (Mesh)UnityEngine.Object.Instantiate(meshFilter.sharedMesh);
                        instance.mesh.name += " Cronchin";
                        instance.subMeshIndex = j;
                        instance.transform = prefabMatrix.inverse * meshFilter.transform.localToWorldMatrix;
                        combine.Add(instance);
                    }
                }
            }


            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                if (skinnedMeshRenderer.gameObject.activeSelf && skinnedMeshRenderer.enabled)
                {
                    Mesh tempMesh = new Mesh();
                    tempMesh.name = "MarrowSDK-TempMesh";
                    skinnedMeshRenderer.BakeMesh(tempMesh);

                    tempMesh.RecalculateNormals();
                    tempMesh.RecalculateTangents();
                    tempMesh.RecalculateBounds();

                    for (int j = 0; j < tempMesh.subMeshCount; j++)
                    {
                        CombineInstance instance = new CombineInstance();
                        instance.mesh = tempMesh;
                        instance.subMeshIndex = j;
                        instance.transform = prefabMatrix.inverse * skinnedMeshRenderer.transform.localToWorldMatrix;
                        combine.Add(instance);
                    }
                }
            }



            Dictionary<Mesh, Mesh> meshesToCronch = new Dictionary<Mesh, Mesh>();
            foreach (var combineInstance in combine)
            {
                if (!meshesToCronch.ContainsKey(combineInstance.mesh))
                {
                    meshesToCronch.Add(combineInstance.mesh, null);
                }
            }

            var keyList = meshesToCronch.Keys.ToList();
            foreach (var meshKey in keyList)
            {
                var newMesh = await AutoSimplifyMeshAsync(meshKey);
                meshesToCronch[meshKey] = newMesh;
            }

            for (int i = combine.Count - 1; i >= 0; i--)
            {
                var meshPart = combine[i];
                if (meshesToCronch.TryGetValue(meshPart.mesh, out var crunchedMesh))
                {
                    meshPart.mesh = crunchedMesh;
                    combine[i] = meshPart;
                }
            }

            var combinedMesh = new Mesh();
            combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            try
            {
                combinedMesh.CombineMeshes(combine.ToArray(), true, true, true);
            }
            catch (Exception e)
            {
                Debug.Log("mesh too big");
                Debug.LogException(e);
            }

            foreach (var meshCronch in meshesToCronch)
            {
                if (meshCronch.Key != null)
                {
                    Debug.Log($"CRONCHIN-key: destroy {meshCronch.Key.name}");
                    UnityEngine.Object.DestroyImmediate(meshCronch.Key);
                }
                if (meshCronch.Value != null)
                {
                    Debug.Log($"CRONCHIN-value: destroy {meshCronch.Value.name}");
                    UnityEngine.Object.DestroyImmediate(meshCronch.Value);
                }
            }

            foreach (var combineMesh in combine)
            {
                if (combineMesh.mesh != null)
                {
                    Debug.Log($"COMBININ: destroy {combineMesh.mesh.name}");
                    UnityEngine.Object.DestroyImmediate(combineMesh.mesh);
                }
            }

            var bakedMesh = await AutoSimplifyMeshAsync(combinedMesh, gameObject.name, true);
            UnityEngine.Object.DontDestroyOnLoad(bakedMesh);

            UnityEngine.Object.DestroyImmediate(combinedMesh);

            stopwatch.Stop();


            return await UniTask.FromResult(bakedMesh);
        }

        private static void AutoSimplifyMesh(Mesh inputMesh, out Mesh outputMesh, float quality = 0.5f, bool forceQuality = false, string name = null, bool forceLossless = false)
        {
            var meshSimpAggressive = new UnityMeshSimplifier.MeshSimplifier();
            meshSimpAggressive.Initialize(inputMesh);
            meshSimpAggressive.SimplifyMesh(quality);

            UnityMeshSimplifier.MeshSimplifier bestSimp = meshSimpAggressive;

            if (!forceQuality)
            {
                var meshSimpLossless = new UnityMeshSimplifier.MeshSimplifier();
                meshSimpLossless.Initialize(inputMesh);
                if (forceLossless || quality != 1f)
                    meshSimpLossless.SimplifyMeshLossless();
                bestSimp = meshSimpLossless;

                if (!forceLossless && quality != 1f && (meshSimpLossless.Vertices.Length > 100 && meshSimpAggressive.Vertices.Length > 0 && meshSimpAggressive.Vertices.Length < meshSimpLossless.Vertices.Length))
                {
                    bestSimp = meshSimpAggressive;
                }
            }

            outputMesh = bestSimp.ToMesh();
            outputMesh.RecalculateNormals();
            outputMesh.RecalculateTangents();
            outputMesh.RecalculateBounds();
            if (name != null)
            {
                outputMesh.name = "CRONCHED " + name;
            }
            else
            {
                outputMesh.name = "CRONCHED mesh";
            }
        }

        private static async UniTask<Mesh> AutoSimplifyMeshAsync(Mesh inputMesh, string name = null, bool forceLossless = false)
        {
            await UniTask.SwitchToMainThread();
            Mesh outputMesh;
            var meshSimp = new UnityMeshSimplifier.MeshSimplifier();
            meshSimp.Initialize(inputMesh);

            var meshSimpAggressive = new UnityMeshSimplifier.MeshSimplifier();
            meshSimpAggressive.Initialize(inputMesh);
            float quality = 0.5f;

            await UniTask.SwitchToThreadPool();
            meshSimp.SimplifyMeshLossless();
            await UniTask.SwitchToMainThread();

            await UniTask.SwitchToThreadPool();
            meshSimpAggressive.SimplifyMesh(quality);
            await UniTask.SwitchToMainThread();

            UnityMeshSimplifier.MeshSimplifier bestSimp = meshSimp;
            if (!forceLossless && (meshSimp.Vertices.Length > 100 && meshSimpAggressive.Vertices.Length > 0 && meshSimpAggressive.Vertices.Length < meshSimp.Vertices.Length))
            {
                bestSimp = meshSimpAggressive;
            }

            outputMesh = bestSimp.ToMesh();

            outputMesh.RecalculateNormals();
            outputMesh.RecalculateTangents();
            outputMesh.RecalculateBounds();
            if (name != null)
            {
                outputMesh.name = "CRONCHED " + name;
            }
            else
            {
                outputMesh.name = "CRONCHED mesh";
            }

            var completion = new UniTaskCompletionSource<Mesh>();
            completion.TrySetResult(outputMesh);

            return await completion.Task;
        }

    }
}