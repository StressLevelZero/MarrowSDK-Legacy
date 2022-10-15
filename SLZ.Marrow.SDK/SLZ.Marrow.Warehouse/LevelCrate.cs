using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using SLZ.Serialize;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace SLZ.Marrow.Warehouse
{
    public class LevelCrate : Crate
    {
        [FormerlySerializedAs("_assetReference")]
        [SerializeField]
        private MarrowScene _mainAsset;

        public override MarrowAsset MainAsset
        {
            get => _mainAsset;
            set
            {
                if (value != null && value.GetType() == typeof(MarrowAsset))
                {
                    _mainAsset = new MarrowScene(value.AssetGUID);
                }
                else
                {
                    _mainAsset = (MarrowScene)value;
                }
            }
        }

        public MarrowScene MainScene
        {
            get => _mainAsset;
            set => _mainAsset = value;
        }


        [SerializeField]
        private List<MarrowScene> _additionalAssetReferences = new List<MarrowScene>();

        [Tooltip("Level has multiple scenes")]
        [SerializeField] private bool _multiScene = false;
        public bool MultiScene
        {
            get => _multiScene;
            set => _multiScene = value;
        }

        [Tooltip("Scenes that also load in when the root scene loads, and will stay always loaded in until level change")]
        [SerializeField]
        private List<MarrowScene> _persistentScenes = new List<MarrowScene>();
        public List<MarrowScene> PersistentScenes => _persistentScenes;

        [Tooltip("Scenes that will be loaded dynamically in the level, for example: Chunk scenes loaded through a Chunk Trigger. All chunked scenes must be included here or they will not be included in the build")]
        [SerializeField]
        private List<MarrowScene> _chunkScenes = new List<MarrowScene>();
        public List<MarrowScene> ChunkScenes => _chunkScenes;

        [Tooltip("Scenes that will ONLY load in the editor outside of playmode. They will load in during builds, but will not be included in builds")]
        [SerializeField]
        private List<MarrowScene> _editorScenes = new List<MarrowScene>();
        public List<MarrowScene> EditorScenes => _editorScenes;

#if UNITY_EDITOR
        private string CURSED_SCENE_GUID = "99c9720ab356a0642a771bea13969a05";

        [ContextMenu("Validate Scene Guid")]
        public void ValidateSceneGUID()
        {
            if (!string.IsNullOrEmpty(MainScene.AssetGUID))
            {
                if (MainScene.AssetGUID == CURSED_SCENE_GUID)
                {
                    var scenePath = AssetDatabase.GUIDToAssetPath(CURSED_SCENE_GUID);
                    var metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(scenePath);

                    var metaText = System.IO.File.ReadAllText(metaPath);

                    if (metaText.Contains(CURSED_SCENE_GUID) && metaText.Contains($"guid: {CURSED_SCENE_GUID}"))
                    {
                        string newGuid = System.Guid.NewGuid().ToString("N");
                        metaText = metaText.Replace($"guid: {CURSED_SCENE_GUID}", $"guid: {newGuid}");

                        System.IO.File.WriteAllText(metaPath, metaText);
                        MainScene = new MarrowScene(newGuid);

                        EditorUtility.SetDirty(this);
                        AssetDatabase.ImportAsset(scenePath);
                        AssetDatabase.SaveAssetIfDirty(this);
                        AssetDatabase.Refresh();
                    }
                }
            }
        }
#endif

#if UNITY_EDITOR
        public override System.Type AssetType
        {
            get => typeof(SceneAsset);
        }
#else
#endif

        public override List<PackedAsset> CollectPackedAssets()
        {
            base.CollectPackedAssets();

            Type sceneAssetType = typeof(UnityEngine.SceneManagement.Scene);


            List<PackedSubAsset> subAssets = new List<PackedSubAsset>();
            for (var i = 0; i < _persistentScenes.Count; i++)
            {
                var persistentScene = _persistentScenes[i];
                string subTitle = i.ToString();
#if UNITY_EDITOR
                subTitle = $"{(persistentScene.EditorAsset != null ? MarrowSDK.SanitizeName(persistentScene.EditorAsset.name) : "")}-{subTitle}";
#endif
                subAssets.Add(new PackedSubAsset(subTitle, persistentScene));
            }
            PackedAssets.Add(new PackedAsset("PersistentScenes", subAssets, sceneAssetType, "_persistentScenes"));

            subAssets = new List<PackedSubAsset>();
            for (var i = 0; i < _chunkScenes.Count; i++)
            {
                var chunkScene = _chunkScenes[i];
                string subTitle = i.ToString();
#if UNITY_EDITOR
                subTitle = $"{(chunkScene.EditorAsset != null ? MarrowSDK.SanitizeName(chunkScene.EditorAsset.name) : "")}-{subTitle}";
#endif
                subAssets.Add(new PackedSubAsset(subTitle, chunkScene));
            }
            PackedAssets.Add(new PackedAsset("ChunkScenes", subAssets, sceneAssetType, "_chunkScenes"));

            return PackedAssets;
        }

#if UNITY_EDITOR
        public SceneSetup[] ToEditorSceneSetups()
        {
            var allScenesForEditor = new List<MarrowScene>();
            allScenesForEditor.Add(MainScene);
            allScenesForEditor.AddRange(PersistentScenes.Select(assetRef => assetRef).Where(scene => scene.EditorAsset != null || scene.AssetGUID != string.Empty));
            allScenesForEditor.AddRange(ChunkScenes.Select(assetRef => assetRef).Where(scene => scene.EditorAsset != null || scene.AssetGUID != string.Empty));

            var ret = new SceneSetup[allScenesForEditor.Count];
            bool first = true;
            for (var i = 0; i < allScenesForEditor.Count; i++)
            {
                var sceneAsset = allScenesForEditor[i];
                var sceneSetup = new SceneSetup
                {
                    path = AssetDatabase.GUIDToAssetPath(sceneAsset.AssetGUID),
                    isActive = first,
                    isLoaded = true
                };
                if (first)
                    first = false;
                ret[i] = sceneSetup;
            }

            return ret;
        }
#endif

        public override void Pack(ObjectStore store, JObject json)
        {
            base.Pack(store, json);

            json.Add("multiscene", MultiScene);
        }

        public override void Unpack(ObjectStore store, ObjectId objectId)
        {
            base.Unpack(store, objectId);

            if (store.TryGetJSON("multiscene", forObject: objectId, out JToken barcodeValue))
            {
                _multiScene = barcodeValue.ToObject<bool>();
            }

            if (store.TryGetJSON("packedAssets", forObject: objectId, out JToken packedAssetsValue))
            {
                foreach (var arrayToken in (JArray)packedAssetsValue)
                {
                    if (((JObject)arrayToken).TryGetValue("title", out var titleToken))
                    {
                        if (((JObject)arrayToken).TryGetValue("subAssets", out var subAssetsToken))
                        {
                            List<MarrowScene> marrowScenes = new List<MarrowScene>();
                            foreach (var subAssetToken in (JArray)subAssetsToken)
                            {
                                marrowScenes.Add(new MarrowScene(subAssetToken.ToObject<string>()));
                            }

                            switch (titleToken.ToObject<string>())
                            {
                                case "PersistentScenes":
                                    _persistentScenes = marrowScenes;
                                    break;
                                case "ChunkScenes":
                                    _chunkScenes = marrowScenes;
                                    break;
                                case "EditorScenes":
                                    _editorScenes = marrowScenes;
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

}