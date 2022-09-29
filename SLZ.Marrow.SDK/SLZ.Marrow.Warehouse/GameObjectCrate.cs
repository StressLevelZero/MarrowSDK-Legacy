using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SLZ.Serialize;
using SLZ.Marrow.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SLZ.Marrow.Warehouse
{
    public class GameObjectCrate : CrateT<GameObject>
    {
        [FormerlySerializedAs("_assetReference")]
        [SerializeField]
        private MarrowGameObject _mainAsset;

        public override MarrowAsset MainAsset
        {
            get => _mainAsset;
            set
            {
                if (value != null && value.GetType() == typeof(MarrowAsset))
                {
                    _mainAsset = new MarrowGameObject(value.AssetGUID);
                }
                else
                {
                    _mainAsset = (MarrowGameObject)value;
                }
            }
        }

        public MarrowGameObject MainGameObject
        {
            get => _mainAsset;
            set => _mainAsset = value;
        }

        [SerializeField] private MarrowAssetT<Mesh> _previewMesh = new MarrowAssetT<Mesh>();
        public MarrowAssetT<Mesh> PreviewMesh
        {
            get => _previewMesh;
            set => _previewMesh = value;
        }


        [SerializeField] private bool _customQuality = false;
        public bool CustomQuality
        {
            get => _customQuality;
            set => _customQuality = value;
        }

        [SerializeField] [Range(0f, 1f)] private float _previewMeshQuality = 0.5f;
        public float PreviewMeshQuality
        {
            get => _previewMeshQuality;
            set => _previewMeshQuality = value;
        }

        [SerializeField] [Range(0f, 5f)] private int _maxLODLevel = 0;
        public int MaxLODLevel
        {
            get => _maxLODLevel;
            set => _maxLODLevel = value;
        }



        [SerializeField] public Bounds _colliderBounds;
        public Bounds ColliderBounds
        {
            get => _colliderBounds;
            set => _colliderBounds = value;
        }


        public override List<PackedAsset> CollectPackedAssets()
        {
            base.CollectPackedAssets();

            PackedAssets.Add(new PackedAsset("PreviewMesh", PreviewMesh, PreviewMesh.AssetType, "previewMesh"));

            return PackedAssets;
        }

#if UNITY_EDITOR
        public override void GeneratePackedAssets(bool saveAsset = true)
        {
            base.GeneratePackedAssets();


            GenerateColliderBounds();


            GeneratePreviewMesh();

            EditorUtility.SetDirty(this);
            if (saveAsset)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }


        protected virtual void GenerateColliderBounds()
        {
            if (MainGameObject.EditorAsset != null)
            {
                PreviewRenderUtility fakeScene = new PreviewRenderUtility();

                var parentGO = new GameObject("root");
                fakeScene.AddSingleGO(parentGO);

                var go = Object.Instantiate(MainGameObject.EditorAsset, Vector3.zero, Quaternion.identity, parentGO.transform);

                Collider[] cols = go.GetComponentsInChildren<Collider>();

                var bounds = new Bounds();
                bool isFirst = true;
                for (int j = 0; j < cols.Length; j++)
                {
                    if (cols[j].isTrigger)
                        continue;

                    if (isFirst)
                        bounds = cols[j].bounds;
                    else
                        bounds.Encapsulate(cols[j].bounds);

                    isFirst = false;
                }


                bounds.center = bounds.center - go.transform.position;

                this.ColliderBounds = bounds;

                DestroyImmediate(go);
                DestroyImmediate(parentGO);
                fakeScene.Cleanup();
            }
        }

        protected virtual void GeneratePreviewMesh()
        {
            if (MainGameObject.EditorAsset != null)
            {
                Mesh mesh;
                if (CustomQuality)
                    mesh = MeshCroncher.CronchMesh(MainGameObject.EditorAsset, MaxLODLevel, PreviewMeshQuality, CustomQuality);
                else
                    mesh = MeshCroncher.CronchMesh(MainGameObject.EditorAsset, MaxLODLevel);

                if (mesh != null)
                {

                    if (!AssetDatabase.IsValidFolder(MarrowSDK.GetMarrowAssetsPath()))
                    {
                        AssetDatabase.CreateFolder("Assets", MarrowSDK.EDITOR_ASSETS_FOLDER);
                    }

                    if (!AssetDatabase.IsValidFolder(MarrowSDK.GetMarrowAssetsPath("PackedAssets")))
                    {
                        AssetDatabase.CreateFolder(MarrowSDK.GetMarrowAssetsPath(), "PackedAssets");
                    }

                    if (!AssetDatabase.IsValidFolder(MarrowSDK.GetMarrowAssetsPath("PackedAssets", Pallet.Barcode)))
                    {
                        AssetDatabase.CreateFolder(MarrowSDK.GetMarrowAssetsPath("PackedAssets"), Pallet.Barcode);
                    }

                    if (!AssetDatabase.IsValidFolder(MarrowSDK.GetMarrowAssetsPath("PackedAssets", Pallet.Barcode, "PreviewMesh")))
                    {
                        AssetDatabase.CreateFolder(MarrowSDK.GetMarrowAssetsPath("PackedAssets", Pallet.Barcode), "PreviewMesh");
                    }

                    string path = MarrowSDK.GetMarrowAssetsPath("PackedAssets", Pallet.Barcode, "PreviewMesh", $"{MarrowSDK.SanitizeName(Title)} PreviewMesh.mesh");
                    AssetDatabase.CreateAsset(mesh, path);
                    var meshAsset = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                    PreviewMesh.SetEditorAsset(meshAsset);
                }
            }
        }
#endif


        public override void Pack(ObjectStore store, JObject json)
        {
            base.Pack(store, json);

            json.Add(new JProperty("colliderBounds", new JObject
                {
                    {
                        "center", new JObject
                        {
                            {"x", _colliderBounds.center.x},
                            {"y", _colliderBounds.center.y},
                            {"z", _colliderBounds.center.z}
                        }
                    },
                    {
                        "extents", new JObject
                        {
                            {"x", _colliderBounds.extents.x},
                            {"y", _colliderBounds.extents.y},
                            {"z", _colliderBounds.extents.z}
                        }
                    }
                }
            ));
        }


        public override void Unpack(ObjectStore store, ObjectId objectId)
        {
            base.Unpack(store, objectId);

            if (store.TryGetJSON("packedAssets", forObject: objectId, out JToken packedAssetsValue))
            {
                foreach (var arrayToken in (JArray)packedAssetsValue)
                {
                    if (((JObject)arrayToken).TryGetValue("title", out var titleToken))
                    {
                        if (((JObject)arrayToken).TryGetValue("guid", out var guidToken))
                        {
                            switch (titleToken.ToObject<string>())
                            {
                                case "PreviewMesh":
                                    _previewMesh = new MarrowMesh(guidToken.ToObject<string>());
                                    break;
                            }
                        }
                    }
                }
            }

            if (store.TryGetJSON("colliderBounds", objectId, out JToken colliderBoundsToken))
            {
                _colliderBounds = colliderBoundsToken.ToObject<Bounds>();
            }

            CollectPackedAssets();
        }
    }

}