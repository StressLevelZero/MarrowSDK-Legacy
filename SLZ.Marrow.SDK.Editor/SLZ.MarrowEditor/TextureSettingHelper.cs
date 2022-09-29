using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SLZ.MarrowEditor
{
    [InitializeOnLoad]
    public static class TextureSettingHelper
    {
        static TextureSettingHelper()
        {
            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }


        static void OnPostHeaderGUI(Editor editor)
        {
            if (TextureStreamSettings.Instance == null)
                return;

            using (new GUILayout.VerticalScope())
            {
                if (editor != null)
                {
                    bool preventStreaming = true;
                    List<string> assetPaths = new List<string>();
                    List<string> assetGuids = new List<string>();
                    foreach (var target in editor.targets)
                    {
                        if (target != null)
                        {
                            string assetPath = AssetDatabase.GetAssetPath(target);
                            Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                            if (!string.IsNullOrEmpty(assetPath) && assetType != null && assetType == typeof(Texture2D))
                            {
                                assetPaths.Add(assetPath);
                                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                                assetGuids.Add(guid);
                                preventStreaming &= TextureStreamSettings.Instance.IsTexturePreventedFromStreaming(assetPath);
                            }
                        }
                    }

                    if (assetPaths.Count == 1)
                    {
                        preventStreaming = TextureStreamSettings.Instance.textureStreamingPreventionGuids.Contains(assetGuids[0]);
                    }

                    if (assetPaths.Count == 0)
                        return;

                    GUILayout.Space(EditorGUIUtility.singleLineHeight / 4f);
                    using (var changeCheck = new EditorGUI.ChangeCheckScope())
                    {
                        preventStreaming = GUILayout.Toggle(preventStreaming, new GUIContent("Prevent Texture Streaming", "Fully prevents this Texture from Streaming in, will always be at Max Resolution. ONLY do this if it is having serious issues."));

                        if (changeCheck.changed)
                        {
                            bool needToSave = false;
                            List<TextureImporter> importersToReimport = new List<TextureImporter>();
                            foreach (var assetPath in assetPaths)
                            {
                                if (TextureStreamSettings.Instance.SetTexturePreventedFromStreaming(assetPath, preventStreaming))
                                {
                                    needToSave = true;
                                }

                                var guid = AssetDatabase.AssetPathToGUID(assetPath).ToString();
                                if (!preventStreaming && TextureStreamTool.IsTextureValidForStreaming(guid))
                                {
                                    TextureStreamSettings.Instance.SetTextureStreamingForTexture(guid, true);
                                    importersToReimport.Add(TextureStreamTool.GetTextureImporterForTextureGuid(guid));
                                }
                                else if (preventStreaming)
                                {
                                    TextureStreamSettings.Instance.SetTextureStreamingForTexture(guid, false);
                                    importersToReimport.Add(TextureStreamTool.GetTextureImporterForTextureGuid(guid));
                                }
                            }

                            if (needToSave)
                            {
                                TextureStreamSettings.Instance.Save();
                                AssetDatabase.StartAssetEditing();
                                foreach (var importer in importersToReimport)
                                {
                                    if (importer != null)
                                        importer.SaveAndReimport();
                                }
                                AssetDatabase.StopAssetEditing();
                            }
                        }
                    }

                }
            }
        }
    }
}