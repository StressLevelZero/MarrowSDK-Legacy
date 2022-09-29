using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SLZ.Marrow.Warehouse;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class MarrowPostHeaderGUI
{
    static MarrowPostHeaderGUI()
    {
        Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
    }


    private static Dictionary<GameObject, SpawnableCrate> swappableGameObjects = new Dictionary<GameObject, SpawnableCrate>();

    static void OnPostHeaderGUI(Editor editor)
    {
        using (new GUILayout.VerticalScope())
        {
            if (editor != null && editor.targets.Length > 0)
            {
                swappableGameObjects.Clear();
                GUILayout.Space(EditorGUIUtility.singleLineHeight / 4f);

                foreach (var target in editor.targets)
                {
                    if (target != null && AssetWarehouse.ready && target is GameObject gameObject)
                    {
                        if (PrefabUtility.IsOutermostPrefabInstanceRoot(gameObject))
                        {
                            var prefab = PrefabUtility.GetCorrespondingObjectFromSource<GameObject>(gameObject);
                            if (prefab != null)
                            {
                                if (AssetWarehouse.Instance.EditorObjectCrateLookup.TryGetValue(prefab, out Crate prefabCrate) && prefabCrate is SpawnableCrate spawnableCrate)
                                {
                                    swappableGameObjects[gameObject] = spawnableCrate;
                                }
                            }
                        }
                    }
                }

                if (swappableGameObjects.Count == 1)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Convert To Spawnable Placer", GUILayout.ExpandWidth(false)))
                        {
                            SpawnableCratePlacer.EditorSwapPrefabForSpawnablePlacer(swappableGameObjects.First().Key, true);
                        }
                        EditorGUILayout.ObjectField(GUIContent.none, swappableGameObjects.First().Value, swappableGameObjects.First().Value.GetType(), false, GUILayout.ExpandWidth(false));
                    }
                }
                else if (swappableGameObjects.Count > 1)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Convert To Spawnable Placer", GUILayout.ExpandWidth(false)))
                        {
                            foreach (var swappableGameObject in swappableGameObjects)
                            {
                                SpawnableCratePlacer.EditorSwapPrefabForSpawnablePlacer(swappableGameObject.Key, true);
                            }
                        }
                        EditorGUILayout.LabelField($"{swappableGameObjects.Count}/{editor.targets.Length}");
                    }
                }
                swappableGameObjects.Clear();
            }
        }
    }
}
