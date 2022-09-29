using System;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace SLZ.Marrow.Warehouse
{
    [Serializable]
    public class MarrowAsset
    {
        [SerializeField]
        [FormerlySerializedAs("m_AssetGUID")]
        private string _assetGUID = "";

        public string AssetGUID
        {
            get => _assetGUID ?? string.Empty;
        }

        private AsyncOperationHandle _operationHandle;

        public AsyncOperationHandle OperationHandle
        {
            get => _operationHandle;
        }

        public virtual Object Asset
        {
            get
            {
                if (!_operationHandle.IsValid())
                    return null;

                return _operationHandle.Result as Object;
            }
        }

        protected bool IsLoading
        {
            get => OperationHandle.IsValid() && !OperationHandle.IsDone;
        }

        protected bool IsDone
        {
            get => OperationHandle.IsValid() && OperationHandle.IsDone;
        }

        protected Type _assetType;

        public virtual Type AssetType
        {
            get
            {
                if (_assetType == null)
                {
                    _assetType = typeof(Object);
                }

                return _assetType;
            }
        }





        public virtual UniTask<Object> LoadTask
        {
            get => default;
            protected set { }
        }

        public MarrowAsset()
        {
            _assetGUID = string.Empty;
        }

        public MarrowAsset(string guid)
        {
            _assetGUID = guid;
        }



        protected virtual AsyncOperationHandle<TObject> LoadAssetAsyncInternal<TObject>()
        {
            AsyncOperationHandle<TObject> result = default(AsyncOperationHandle<TObject>);
            if (!_operationHandle.IsValid())
            {
                result = Addressables.LoadAssetAsync<TObject>(AssetGUID);
                AssetWarehouseMetrics.LoadedCrateAssetsCount.Value++;
                _operationHandle = result;
            }

            return result;
        }

        protected virtual AsyncOperationHandle<SceneInstance> LoadSceneAsyncInternal(LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            AsyncOperationHandle<SceneInstance> result = default(AsyncOperationHandle<SceneInstance>);
            if (!_operationHandle.IsValid())
            {
                result = Addressables.LoadSceneAsync(AssetGUID, loadMode, activateOnLoad, priority);
                _operationHandle = result;
            }
            return result;
        }

        protected virtual bool ReleaseAsset()
        {
            if (!_operationHandle.IsValid())
            {
                return false;
            }

            Addressables.Release(_operationHandle);
            AssetWarehouseMetrics.LoadedCrateAssetsCount.Value--;
            _operationHandle = default;
            return true;
        }


        public async UniTask<TObject> LoadAssetAsync<TObject>() where TObject : Object
        {
            if (IsDone || IsLoading)
            {



                return await OperationHandle.Convert<TObject>().ToUniTask();
            }
            else
            {
                var task = LoadAssetAsyncInternal<TObject>().ToUniTask();
                var retObject = await task;




                return retObject;
            }
        }

        public void LoadAsset<TObject>(Action<TObject> loadCallback) where TObject : Object
        {
            if (IsDone)
            {



                loadCallback.Invoke(Asset as TObject);
            }
            else
            {
                LoadAssetAsync<TObject>().ContinueWith(loadCallback).Forget();
            }
        }

        public async UniTask<SceneInstance> LoadSceneAsync(LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            if (IsDone || IsLoading)
            {



                return await OperationHandle.Convert<SceneInstance>().ToUniTask();
            }
            else
            {
                var task = LoadSceneAsyncInternal(loadMode, activateOnLoad, priority).ToUniTask();
                var retObject = await task;




                return retObject;
            }
        }

        public bool UnloadAsset(bool forced = false)
        {
            bool unloaded = false;
            if (forced)
            {
                unloaded = ReleaseAsset();
            }
            else
            {







            }

            return unloaded;
        }


























        public bool Equals(MarrowAsset other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _assetGUID == other._assetGUID;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MarrowAsset)obj);
        }

        public override int GetHashCode()
        {
            return (_assetGUID != null ? _assetGUID.GetHashCode() : 0);
        }

        public static bool operator ==(MarrowAsset left, MarrowAsset right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MarrowAsset left, MarrowAsset right)
        {
            return !Equals(left, right);
        }


#if UNITY_EDITOR
        private Object _cachedAsset;
        private string _cachedGUID;


#pragma warning disable CS0414
        [SerializeField] private bool _editorAssetChanged;
#pragma warning restore CS0414

        protected Object CachedAsset
        {
            get
            {
                if (_cachedGUID != _assetGUID)
                {
                    _cachedAsset = null;
                    _cachedGUID = "";
                }

                return _cachedAsset;
            }
            set
            {
                _cachedAsset = value;
                _cachedGUID = _assetGUID;
            }
        }

        public virtual Object EditorAsset
        {
            get
            {
                if (CachedAsset != null || string.IsNullOrEmpty(_assetGUID))
                    return CachedAsset;

                var asset = FetchEditorAsset();

                return CachedAsset = asset;
            }
        }

        protected Object FetchEditorAsset()
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(AssetGUID);
            return AssetDatabase.LoadAssetAtPath(assetPath, AssetType);
        }

        public virtual bool SetEditorAsset(Object obj)
        {
            if (obj == null)
            {
                CachedAsset = null;
                _assetGUID = string.Empty;
                _editorAssetChanged = true;
                return true;
            }

            if (CachedAsset != obj)
            {
                var path = AssetDatabase.GetAssetOrScenePath(obj);
                _assetGUID = AssetDatabase.AssetPathToGUID(path);
                Object mainAsset;
                if (AssetType != typeof(Object))
                {
                    mainAsset = AssetDatabase.LoadAssetAtPath(path, AssetType);
                }
                else
                {
                    mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
                }

                CachedAsset = mainAsset;
            }

            _editorAssetChanged = true;
            return true;
        }
#endif

    }

    [Serializable]
    public class MarrowAssetT<TObject> : MarrowAsset where TObject : Object
    {
        public MarrowAssetT() { }
        public MarrowAssetT(string guid) : base(guid)
        {
#if UNITY_EDITOR
            _assetType = typeof(TObject);
#endif
        }

        public new TObject Asset
        {
            get
            {
                if (!OperationHandle.IsValid())
                    return null;

                return OperationHandle.Result as TObject;
            }
        }

        public override Type AssetType
        {
            get
            {
                if (_assetType == null)
                {
                    _assetType = typeof(TObject);
                }

                return _assetType;
            }
        }

        public async UniTask<TObject> LoadAssetAsync()
        {
            return await LoadAssetAsync<TObject>();
        }

        public void LoadAsset(Action<TObject> loadCallback)
        {
            LoadAsset<TObject>(loadCallback);
        }




#if UNITY_EDITOR
        public new TObject EditorAsset
        {
            get
            {
                if (CachedAsset as TObject != null || string.IsNullOrEmpty(AssetGUID))
                    return CachedAsset as TObject;

                var asset = FetchAsset();

                return asset;
            }
        }

        protected TObject FetchAsset()
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(AssetGUID);
            return AssetDatabase.LoadAssetAtPath<TObject>(assetPath);
        }
#endif

    }



    [Serializable]
    public class MarrowGameObject : MarrowAssetT<GameObject>
    {
        public MarrowGameObject() { }
        public MarrowGameObject(string guid) : base(guid)
        {
        }
    }

    [Serializable]
    public class MarrowMesh : MarrowAssetT<Mesh>
    {
        public MarrowMesh(string guid) : base(guid)
        {
        }
    }

    [Serializable]
    public class MarrowTexture : MarrowAssetT<Texture>
    {
        public MarrowTexture(string guid) : base(guid)
        {
        }
    }

    [Serializable]
    public class MarrowTexture2D : MarrowAssetT<Texture2D>
    {
        public MarrowTexture2D(string guid) : base(guid)
        {
        }
    }

    [Serializable]
    public class MarrowScene : MarrowAsset
    {
        private Type _assetType1;

        public MarrowScene(string guid) : base(guid)
        {
        }

#if UNITY_EDITOR

        public new UnityEditor.SceneAsset EditorAsset => base.EditorAsset as UnityEditor.SceneAsset;

        public override Type AssetType => typeof(SceneAsset);
#endif
    }
}