using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SLZ.MarrowEditor
{

    public class TextureStreamSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        public static readonly string settingsAssetPath = "Assets/_MarrowAssets/Settings/TextureStreamSettings.asset";
        private static TextureStreamSettings _instance;

        public static TextureStreamSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = AssetDatabase.LoadAssetAtPath<TextureStreamSettings>(settingsAssetPath);

                    if (_instance == null)
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(settingsAssetPath)))
                            Directory.CreateDirectory(Path.GetDirectoryName(settingsAssetPath));
                        _instance = CreateInstance<TextureStreamSettings>();
                        AssetDatabase.CreateAsset(_instance, settingsAssetPath);
                    }
                }

                return _instance;
            }
        }


        public HashSet<string> textureStreamingPreventionGuids = new HashSet<string>();
        [SerializeField] private List<string> textureStreamingPreventionGuidsList = new List<string>();

        [MenuItem("Stress Level Zero/Void Tools/Texture Stream Settings", priority = 6969420)]
        private static void EditorSettingMenuItem()
        {
            if (Instance != null)
                Selection.activeObject = Instance;
        }

        public bool IsTexturePreventedFromStreaming(string assetPath)
        {
            string guid = AssetDatabase.GUIDFromAssetPath(assetPath).ToString();
            return textureStreamingPreventionGuids.Contains(guid);
        }

        public bool SetTexturePreventedFromStreaming(string assetPath, bool preventStreaming)
        {
            string guid = AssetDatabase.GUIDFromAssetPath(assetPath).ToString();
            return SetTexturePreventedFromStreamingByGuid(guid, preventStreaming);
        }

        public bool SetTexturePreventedFromStreamingByGuid(string guid, bool preventStreaming)
        {
            bool changed = false;

            if (preventStreaming && !textureStreamingPreventionGuids.Contains(guid))
            {
                textureStreamingPreventionGuids.Add(guid);
                changed = true;
            }
            else if (!preventStreaming && textureStreamingPreventionGuids.Contains(guid))
            {
                textureStreamingPreventionGuids.Remove(guid);
                changed = true;
            }

            return changed;
        }

        public bool SetTextureStreamingForTexture(string textureGuid, bool enableStreaming)
        {
            bool changed = false;

            var importer = TextureStreamTool.GetTextureImporterForTextureGuid(textureGuid);
            if (importer != null)
            {
                if (enableStreaming && TextureStreamTool.IsTextureValidForStreaming(textureGuid))
                {
                    if (!importer.streamingMipmaps)
                    {
                        importer.streamingMipmaps = true;
                        changed = true;
                    }
                }
                else if (!enableStreaming)
                {
                    if (importer.streamingMipmaps)
                    {
                        importer.streamingMipmaps = false;
                        changed = true;
                    }
                }
            }

            return changed;
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }

        public void OnBeforeSerialize()
        {
            textureStreamingPreventionGuidsList.Clear();

            foreach (var guid in textureStreamingPreventionGuids)
            {
                textureStreamingPreventionGuidsList.Add(guid);
            }
        }

        public void OnAfterDeserialize()
        {
            textureStreamingPreventionGuids = new();

            foreach (var guid in textureStreamingPreventionGuidsList)
            {
                textureStreamingPreventionGuids.Add(guid);
            }
        }

    }
}