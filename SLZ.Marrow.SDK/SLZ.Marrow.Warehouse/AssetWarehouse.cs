using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using Cysharp.Threading.Tasks;
using SLZ.Marrow.Utilities;
using UnityEngine.LowLevel;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SLZ.Marrow.Warehouse
{

    public class AssetWarehouse
    {
        [Serializable]
        public struct PalletManifest
        {
            [SerializeField]
            public Pallet pallet;
            [SerializeField]
            public IResourceLocator catalog;
            [SerializeField]
            public string catalogPath;

            public bool active;
        }

        public static AssetWarehouse Instance;
        public static bool ready = false;
        private static Action _onReady;

        private readonly Dictionary<Barcode, PalletManifest> warehouseManifest = new Dictionary<Barcode, PalletManifest>();

        private Dictionary<Barcode, Scannable> internalPallets = new Dictionary<Barcode, Scannable>();

        private readonly Dictionary<Barcode, Scannable> _inventoryRegistry = new Dictionary<Barcode, Scannable>();
        private readonly Dictionary<Barcode, Scannable> _oldBarcodeInventoryRegistry = new Dictionary<Barcode, Scannable>();
        private readonly Dictionary<MarrowGuid, Scannable> _slimCodeInventoryRegistry = new Dictionary<MarrowGuid, Scannable>();

        public Dictionary<Barcode, Scannable> InventoryRegistry
        {
            get { return _inventoryRegistry; }
        }

        [ReadOnly]
        [SerializeField]
        private List<string> _allTags = new List<string>();

        public List<string> AllTags
        {
            get { return _allTags; }
            private set { _allTags = value; }
        }


        [SerializeField]
        [ReadOnly]
        private bool _initialLoaded = false;
        public bool InitialLoaded
        {
            get { return _initialLoaded; }
            private set { _initialLoaded = value; }
        }

        public static readonly string INTERNAL_PALLET_GROUP_NAME = "Internal Pallets";
        public static readonly string INTERNAL_PALLET_LABEL = "Internal Pallet";

#if UNITY_EDITOR
        private Dictionary<Object, Crate> _editorObjectCrateLookup = new Dictionary<Object, Crate>();
        public Dictionary<Object, Crate> EditorObjectCrateLookup
        {
            get => _editorObjectCrateLookup;
            private set => _editorObjectCrateLookup = value;
        }

        private Dictionary<string, Crate> _editorObjectGuidCrateLookup = new Dictionary<string, Crate>();
        public Dictionary<string, Crate> EditorObjectGuidCrateLookup
        {
            get => _editorObjectGuidCrateLookup;
            private set => _editorObjectGuidCrateLookup = value;
        }
#endif














        public static void OnReady(Action callbackWhenReady)
        {
            if (ready)
            {
                callbackWhenReady?.Invoke();
                return;
            }

            _onReady += callbackWhenReady;
        }


        public async UniTask InitAsync()
        {
            LogVerbose($"Init {UnityEngine.Random.Range(1, 100)}");
            Instance = this;
            AssetWarehouseMetrics.Reset();

#if UNITY_EDITOR
            if (EditorObjectCrateLookup == null)
                EditorObjectCrateLookup = new Dictionary<Object, Crate>();
            else
                EditorObjectCrateLookup.Clear();

            if (EditorObjectGuidCrateLookup == null)
                EditorObjectGuidCrateLookup = new Dictionary<string, Crate>();
            else
                EditorObjectGuidCrateLookup.Clear();
#endif

            await LoadInitialPalletsAsync();

            ready = true;
            _onReady?.Invoke();
            _onReady = null;
        }

        private void Init()
        {
            InitAsync().Forget();
        }

        public AssetWarehouse(bool autoInit = true)
        {
            if (autoInit)
                Init();
        }

        ~AssetWarehouse()
        {

            Clear();
        }

        public void Clear()
        {
            UnloadAllPallets();

            _inventoryRegistry.Clear();
            _oldBarcodeInventoryRegistry.Clear();
            _slimCodeInventoryRegistry.Clear();
            warehouseManifest.Clear();

#if UNITY_EDITOR
            EditorObjectCrateLookup.Clear();
            EditorObjectGuidCrateLookup.Clear();
#endif

            InitialLoaded = false;
            ready = false;
            _onReady = null;
            OnChanged = null;
            OnPalletAdded = null;
            OnCrateAdded = null;
            AssetWarehouseMetrics.Reset();
        }







































#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void EditorInitialize()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            Cysharp.Threading.Tasks.PlayerLoopHelper.Initialize(ref loop);

            LogVerbose("AssetWarehouse: InitializeOnLoadMethod");
            LogVerbose("EditorInitialize" + (EditorApplication.isPlayingOrWillChangePlaymode ? " isPlayingOrWillChangePlaymode" : "") + (Application.isPlaying ? " isPlaying" : ""));
            if (Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                LogVerbose("AW: EditorInitialize: playing");
            }
            else
            {
                LogVerbose("AW: EditorInitialize: not playing");
                if (AssetWarehouse.Instance == null && !AssetWarehouse.ready)
                {
                    var ass = new AssetWarehouse();
                }
            }
        }
#endif






























        private async UniTask LoadInitialPalletsAsync()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                await Addressables.InitializeAsync();
                if (EditorPrefs.GetBool("StartWarehouseLoadAssetDatabase", true))
                {
                    LoadPalletsFromAssetDatabase();
                }
                LoadPlaymodePallets();
                if (EditorPrefs.GetBool("StartWarehouseLoadModsFolder", true))
                {
                    await LoadPalletsFromModsFolderAsync();
                }
                if (EditorPrefs.GetBool("StartWarehouseLoadInternalBuiltPallets", true))
                {
                    await LoadInternalBuiltPalletsAsync();
                }
            }
            else
            {
                LoadPalletsFromAssetDatabase();
            }
#else
#if !PLATFORM_PSVR2
#endif
#endif
            await UniTask.Yield();

            InitialLoaded = true;
            LogVerbose("Initial Pallets loaded", true);
        }




        public Action OnChanged;
        public Action<string> OnPalletAdded;
        public Action<string> OnCrateAdded;


        public Crate GetCrate(Barcode barcode)
        {
            if (barcode == null)
            {
                return null;
            }
            else
            {
                return GetCrate<Crate>(barcode);
            }
        }


        public T GetCrate<T>(string barcode) where T : Crate
        {
            if (TryGetCrate<T>(barcode, out var crate))
            {
                return crate;
            }
            else
            {
                return null;
            }
        }


        public async UniTask<T> GetCrateAsync<T>(string barcode, Action<T> callback = null) where T : Crate
        {
            if (!HasCrate<T>(barcode))
            {


                await UniTask.WaitUntil(() => InitialLoaded);
            }

            var completion = new UniTaskCompletionSource<T>();

            if (TryGetCrate<T>(barcode, out var crate))
            {
                completion.TrySetResult(crate);
                callback?.Invoke(crate);
            }
            else
            {
                completion.TrySetResult(null);
            }

            return await completion.Task;
        }


        public async UniTask<Crate> GetCrateAsync(string barcode, Action<Crate> callback = null, CancellationToken cancellationToken = default)
        {
            if (!HasCrate(barcode))
            {

                await UniTask.WaitUntil(() => InitialLoaded);
            }

            var completion = new UniTaskCompletionSource<Crate>();

            if (TryGetCrate(barcode, out var crate))
            {
                completion.TrySetResult(crate);
                callback?.Invoke(crate);
            }
            else
            {
                completion.TrySetResult(null);
            }

            return await completion.Task;
        }


        public bool TryGetPallet(string barcode, out Pallet pallet)
        {
            if (TryGetScannable<Pallet>(barcode, out Pallet foundpallet))
            {
                pallet = foundpallet;
                return true;
            }
            pallet = null;
            return false;
        }

        public bool TryGetPallet(MarrowGuid slimCode, out Pallet pallet)
        {
            if (TryGetScannable<Pallet>(slimCode, out Pallet foundpallet))
            {
                pallet = foundpallet;
                return true;
            }
            pallet = null;
            return false;
        }

        public bool TryGetCrate(string barcode, out Crate crate)
        {
            if (TryGetCrate<Crate>(barcode, out Crate foundCrate))
            {
                crate = foundCrate;
                return true;
            }
            crate = null;
            return false;
        }

        public bool TryGetCrate<T>(string barcode, out T crateT) where T : Crate
        {
            if (TryGetScannable<T>(barcode, out T foundCrate))
            {
                crateT = foundCrate;
                return true;
            }
            crateT = null;
            return false;
        }

        public bool TryGetCrate<T>(MarrowGuid slimCode, out T crateT) where T : Crate
        {
            if (TryGetScannable<T>(slimCode, out T foundCrate))
            {
                crateT = foundCrate;
                return true;
            }
            crateT = null;
            return false;
        }

        public bool TryGetScannable(string barcode, out Scannable scannable)
        {
            if (TryGetScannable<Scannable>(barcode, out Scannable item))
            {
                scannable = item;
                return true;
            }
            scannable = null;
            return false;
        }

        public bool TryGetScannable<T>(string barcode, out T scannableT) where T : Scannable
        {
            scannableT = null;
            bool found = false;
            if (Barcode.IsValid(barcode))
            {
                found = _inventoryRegistry.TryGetValue((Barcode)barcode, out Scannable scannable) && scannable != null && scannable is T;
                if (!found)
                {
                    found = _oldBarcodeInventoryRegistry.TryGetValue((Barcode)barcode, out scannable) && scannable != null && scannable is T;
                }

                if (found)
                {
                    scannableT = (T)scannable;
                }
            }
            return found;
        }

        public bool TryGetScannable<T>(MarrowGuid slimCode, out T scannableT) where T : Scannable
        {
            scannableT = null;
            bool found = false;
            if (MarrowGuid.IsValid(slimCode))
            {
                found = _slimCodeInventoryRegistry.TryGetValue(slimCode, out Scannable scannable) && scannable != null && scannable is T;
                if (found)
                {
                    scannableT = (T)scannable;
                }
            }
            return found;
        }

        public bool TryGetSlimCode(string barcode, out MarrowGuid slimCode)
        {
            slimCode = new MarrowGuid();
            if (TryGetScannable(barcode, out var scannable))
            {
                if (scannable.SlimCode.IsValid())
                {
                    slimCode = scannable.SlimCode;
                    return true;
                }
            }
            return false;
        }


        public bool HasScannable<T>(string barcode) where T : Scannable
        {
            return TryGetScannable<T>(barcode, out _);
        }

        public bool HasScannable(string barcode)
        {
            return HasScannable<Scannable>(barcode);
        }

        public bool HasCrate<T>(string barcode) where T : Crate
        {
            return HasScannable<T>(barcode);
        }

        public bool HasCrate(string barcode)
        {
            return HasCrate<Crate>(barcode);
        }

        public bool HasPallet(string barcode)
        {
            return HasScannable<Pallet>(barcode);
        }


        public bool UnloadCrateAsset(string barcode, bool forcedUnload = false, bool forceUnloadPackedAssets = false)
        {
            bool unloaded = false;
            if (TryGetCrate(barcode, out var crate))
            {
                unloaded = UnloadCrateAsset(crate);
            }

            return unloaded;
        }

        public bool UnloadCrateAsset(Crate crate, bool forcedUnload = false, bool forceUnloadPackedAssets = false)
        {

            foreach (var packedAsset in crate.PackedAssets)
            {
                if (packedAsset.marrowAsset != null)
                {
                    packedAsset.marrowAsset.UnloadAsset(true);
                }

                foreach (var packedSubAsset in packedAsset.subAssets)
                {
                    packedSubAsset.subAsset.UnloadAsset(true);
                }
            }
            return crate.MainAsset.UnloadAsset(forcedUnload);
        }

        public int UnloadAllCrateAssets(string excludeBarcode = "", bool forcedUnload = false, bool forceUnloadPackedAssets = false)
        {
            int unloadCount = 0;
            var crates = GetCrates();
            foreach (var crate in crates)
            {
                if (!Barcode.IsValid(excludeBarcode) || crate.Barcode != excludeBarcode)
                {
                    if (UnloadCrateAsset(crate, forcedUnload))
                        unloadCount++;
                }
            }

            return unloadCount;
        }

        public void UnloadCrate(string barcode)
        {
            if (TryGetCrate(barcode, out var crate))
            {
                UnloadCrate(crate);
            }
        }

        public void UnloadCrate(Crate crate)
        {
            UnloadCrateAsset(crate, true, true);
            InventoryRegistry.Remove(crate.Barcode);
            if (Barcode.IsValid(crate.BarcodeOld) && InventoryRegistry.ContainsKey(crate.BarcodeOld))
                InventoryRegistry.Remove(crate.BarcodeOld);

#if UNITY_EDITOR
            Object removeItem = null;
            foreach (var objectCrateKVP in EditorObjectCrateLookup)
            {
                if (objectCrateKVP.Value == crate)
                {
                    removeItem = objectCrateKVP.Key;
                }
            }
            if (removeItem != null)
                EditorObjectCrateLookup.Remove(removeItem);

            string removeItem2 = null;
            foreach (var objectCrateKVP in EditorObjectGuidCrateLookup)
            {
                if (objectCrateKVP.Value == crate)
                {
                    removeItem2 = objectCrateKVP.Key;
                }
            }
            if (removeItem2 != null)
                EditorObjectGuidCrateLookup.Remove(removeItem2);
#endif
        }


#if UNITY_EDITOR
        private struct EditorCatalogEntry
        {
            public string guid;
            public string address;
            public Object obj;
            public Type assetType;
            public string assetPath;

            public EditorCatalogEntry(string guid, string address, Object obj, Type assetType)
            {
                this.guid = guid;
                this.address = address;
                this.obj = obj;
                this.assetType = assetType;

                assetPath = AssetDatabase.GUIDToAssetPath(this.guid);

                if (this.obj == null)
                {
                    this.obj = AssetDatabase.LoadAssetAtPath(assetPath, this.assetType);
                }

            }
        }

        private void LoadEditorPallet(Pallet pallet, bool loadPalletJson = true)
        {
            if (Application.isPlaying && loadPalletJson)
            {
                var runtimePalletJson = PalletPacker.PackIntoJson(pallet);
                var runtimePallet = PalletPacker.UnpackJson(runtimePalletJson);
                AddPallet(runtimePallet);
            }
            else
            {
                AddPallet(pallet);
            }

#if false
#endif
        }

        private void LoadSimulatedCatalog(Pallet pallet)
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Warehouse.AssetWarehouse.LoadSimulatedCatalog(SLZ.Marrow.Warehouse.Pallet)");

            throw new System.NotImplementedException();
        }
#endif


        private void AddPallet(Pallet pallet, string catalogPath = "")
        {
            if (pallet == null)
            {
                Debug.LogWarning("AssetWarehouse: Cannot add null pallet");
                return;
            }

            AddScannable(pallet);

            PalletManifest manifest = new PalletManifest();
            manifest.pallet = pallet;
            if (!string.IsNullOrEmpty(catalogPath))
            {
                manifest.catalogPath = catalogPath;
            }

            warehouseManifest.Remove(manifest.pallet.Barcode);
            warehouseManifest.Add(manifest.pallet.Barcode, manifest);

            foreach (var crate in pallet.Crates)
            {
                AddCrate(crate);
            }

            OnChanged?.Invoke();
            OnPalletAdded?.Invoke(pallet.Barcode);
        }

        private void AddCrate(Crate crate)
        {
            if (crate == null)
            {
                Debug.LogWarning("AssetWarehouse: Cannot add null crate");
                return;
            }

            AddScannable(crate);
            AddTags(crate.Tags);

#if UNITY_EDITOR
            if (crate.MainAsset.EditorAsset != null)
            {
                EditorObjectCrateLookup[crate.MainAsset.EditorAsset] = crate;
            }
            EditorObjectGuidCrateLookup[crate.MainAsset.AssetGUID] = crate;
#endif

            OnCrateAdded?.Invoke(crate.Barcode);
        }

        private bool AddScannable(Scannable item)
        {
            bool added = false;
            if (Barcode.IsValid(item.Barcode))
            {
                _inventoryRegistry[item.Barcode] = item;
                added = true;
                if (Barcode.IsValid(item.BarcodeOld) && item.Barcode != item.BarcodeOld)
                {
                    _oldBarcodeInventoryRegistry[new Barcode(item.BarcodeOld)] = item;
                }

                if (item.SlimCode.IsValid())
                {
                    _slimCodeInventoryRegistry[item.SlimCode] = item;
                }
            }

            if (!added)
            {
                Debug.LogError("AssetWarehouse: Cannot add item " + item.Title + ", invalid barcode");
            }
            return added;
        }

        public void UnloadPallet(string barcode)
        {
            if (TryGetPallet(barcode, out var pallet))
            {
                UnloadPallet(pallet);
            }
        }

        public void UnloadPallet(Pallet pallet)
        {
            foreach (var crate in pallet.Crates)
            {
                UnloadCrate(crate);
            }

            InventoryRegistry.Remove(pallet.Barcode);
            if (Barcode.IsValid(pallet.BarcodeOld) && InventoryRegistry.ContainsKey(pallet.BarcodeOld))
                InventoryRegistry.Remove(pallet.BarcodeOld);

            if (warehouseManifest.TryGetValue(pallet.Barcode, out var palletManifest))
            {














                warehouseManifest.Remove(pallet.Barcode);
            }
        }

        public void UnloadAllPallets()
        {
            foreach (var pallet in GetPallets())
            {
                if (pallet != null)
                {
                    UnloadPallet(pallet);
                }
            }
        }

        public void DeletePallet(string barcode)
        {
            if (TryGetPallet(barcode, out var pallet))
            {
                DeletePallet(pallet);
            }
        }

        public void DeletePallet(Pallet pallet)
        {
            PalletManifest palletManifest;
            warehouseManifest.TryGetValue(pallet.Barcode, out palletManifest);

            UnloadPallet(pallet);

            if (!palletManifest.pallet.Internal && !string.IsNullOrEmpty(palletManifest.catalogPath))
            {
                Debug.Log("Deleting entire mod directory at \"" + Path.GetDirectoryName(palletManifest.catalogPath) + "\"");
            }
        }


        private void AddTags(List<string> newTags)
        {
            bool added = false;
            foreach (var tag in newTags)
            {
                if (!AllTags.Contains(tag))
                {
                    AllTags.Add(tag);
                    added = true;
                }
            }
            if (added)
            {
                AllTags.Sort();
            }
        }


        public List<Pallet> GetPallets()
        {
            List<Pallet> pallets = new List<Pallet>();

            foreach (var palletManifest in warehouseManifest.Values)
            {
                pallets.Add(palletManifest.pallet);
            }

            return pallets;
        }

        public void GetPallets(ref List<Pallet> pallets)
        {
            if (pallets == null)
                pallets = new List<Pallet>();
            else
                pallets.Clear();

            foreach (var palletManifest in warehouseManifest.Values)
            {
                pallets.Add(palletManifest.pallet);
            }
        }

        public List<Crate> GetCrates()
        {
            List<Crate> crates = new List<Crate>();

            foreach (var scannable in _inventoryRegistry.Values)
            {
                if (scannable is Crate crate && !crates.Contains(scannable))
                {
                    crates.Add(crate);
                }
            }

            return crates;
        }

        public void GetCrates(in List<Crate> crates)
        {
            if (crates.Count > 0)
                crates.Clear();

            foreach (var scannable in _inventoryRegistry.Values)
            {
                if (scannable is Crate crate && !crates.Contains(scannable))
                {
                    crates.Add(crate);
                }
            }
        }

        public class HideLevelCrateFilter : ICrateFilter<LevelCrate>
        {
            public bool Filter(LevelCrate crate)
            {
                return !crate.Redacted;
            }
        }


        public List<T> FilterCrates<T>(ref List<T> crateList, ICrateFilter<T> crateFilter) where T : Crate
        {
            for (int i = crateList.Count - 1; i >= 0; i--)
            {
                if (crateFilter == null || crateFilter.Filter(crateList[i]))
                {
                    crateList.RemoveAt(i);
                }
            }

            return crateList;
        }

        public List<T> GetCrates<T>(ICrateFilter<T> crateFilter = null) where T : Crate
        {
            List<T> crates = new List<T>();

            foreach (var scannable in _inventoryRegistry.Values)
            {
                if (scannable is T crate && (crateFilter == null || crateFilter.Filter(crate)))
                {
                    crates.Add(crate);
                }
            }
            return crates;
        }






        public void TestQueries()
        {
            var pallets = GetPallets();

            var allCrates = GetCrates();

            var allSpawnableCrates = GetCrates<SpawnableCrate>();

            var allSceneCrates = GetCrates<LevelCrate>();

            var testFilterCrates = GetCrates(new HideLevelCrateFilter());

            var testFilterCrates2 = AssetWarehouse.Instance.GetCrates<LevelCrate>().Filter(new HideLevelCrateFilter());
        }






        public static void LogVerbose(string text, bool logInRuntime = false)
        {

            bool logIt = false;
#if UNITY_EDITOR
            if (EditorPrefs.GetBool("VerboseWarehouseLogging", false))
            {
                logIt = true;
            }
#else
#endif
            if (logIt)
            {
                Debug.Log("AssetWarehouse: " + text);
            }
        }







#if UNITY_EDITOR


        public void LoadPalletsFromAssetDatabase(bool clear = false)
        {
            LogVerbose("LoadPalletsFromAssetDatabase");
            if (clear)
            {
                _inventoryRegistry.Clear();
                warehouseManifest.Clear();
            }

            var foundAssets = AssetDatabase.FindAssets("t:Pallet");
            foreach (var guid in foundAssets)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Pallet pallet = AssetDatabase.LoadAssetAtPath<Pallet>(path);
                if (!HasPallet(pallet.Barcode))
                {
                    LoadEditorPallet(pallet);
                }
            }
        }

        public void LoadPlaymodePallets()
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.Marrow.Warehouse.AssetWarehouse.LoadPlaymodePallets()");

            throw new System.NotImplementedException();
        }

        public void LoadFromBuildLocalProject()
        {
            LoadFromBuildLocalProjectAsync().Forget();
        }

        public async UniTask LoadFromBuildLocalProjectAsync()
        {
            LogVerbose("LoadFromBuildLocalProject: " + Path.GetFullPath(MarrowSDK.BUILT_PALLETS_NAME));

            await LoadPalletsFromFolderAsync(Path.GetFullPath(MarrowSDK.BUILT_PALLETS_NAME));
        }
#endif

        public void LoadPalletsFromModsFolder()
        {
            LoadPalletsFromModsFolderAsync().Forget();
        }

        public async UniTask LoadPalletsFromModsFolderAsync()
        {
            LogVerbose("LoadPalletsFromModsFolder: " + MarrowSDK.RuntimeModsPath, true);

            await LoadPalletsFromFolderAsync(MarrowSDK.RuntimeModsPath);
        }

        public void LoadInternalBuiltPallets()
        {
            LoadInternalBuiltPalletsAsync().Forget();
        }

        public async UniTask LoadInternalBuiltPalletsAsync()
        {
            LogVerbose("LoadInternalBuiltPallets", true);

            try
            {


                var internalPalletsCheck = await Addressables.LoadResourceLocationsAsync(INTERNAL_PALLET_LABEL);
                if (internalPalletsCheck == null || internalPalletsCheck.Count <= 0)
                {
                    LogVerbose("LoadInternalBuiltPallets: failed to load any internal pallets: ", true);
                    Addressables.Release(internalPalletsCheck);
                }
                else
                {
                    var pallets = await Addressables.LoadAssetsAsync<Pallet>(INTERNAL_PALLET_LABEL, null);
                    foreach (var pallet in pallets)
                    {
                        await LoadPalletAsync(pallet);
                    }
                    Addressables.Release(internalPalletsCheck);
                }
            }
            catch (Exception e)
            {
                LogVerbose("LoadInternalBuiltPallets: failed to load any internal pallets: " + e.ToString(), true);
            }

        }

        public void LoadPalletsFromSLZServer()
        {
            LoadPalletsFromSLZServerAsync().Forget();
        }

        public async UniTask LoadPalletsFromSLZServerAsync()
        {


            LogVerbose("LoadPalletsFromSLZServer", true);
            await UniTask.Yield();
        }

        public async UniTask LoadPalletsFromFolderAsync(string path)
        {
            if (Directory.Exists(path))
            {
                string[] palletJsons = Directory.GetFiles(path, Pallet.PALLET_JSON_FILENAME, SearchOption.AllDirectories);

                foreach (var palletJsonPath in palletJsons)
                {
                    bool success = await LoadPalletFromFolderAsync(palletJsonPath);
                }
            }
            else
            {
                Debug.LogWarning("AssetWarehouse: Cannot load pallets from folder, missing folder: " + (path == null ? "null" : path));
            }
        }

        public async UniTask<bool> LoadPalletFromFolderAsync(string palletPath, bool updateCatalog = false)
        {
            if (!palletPath.EndsWith(Pallet.PALLET_JSON_FILENAME))
                palletPath = Path.Combine(palletPath, Pallet.PALLET_JSON_FILENAME);
            Pallet loadedPallet = PalletPacker.UnpackJsonFromFile(palletPath);

            if (loadedPallet != null)
            {
                return await LoadPalletAsync(loadedPallet, palletPath, updateCatalog);
            }
            else
            {
                Debug.LogWarning("AssetWarehouse: Cannot read " + Pallet.PALLET_JSON_FILENAME + " at " + palletPath);
                return false;
            }
        }


        public async UniTask<bool> LoadPalletFromText(string palletJsonText)
        {
            Pallet loadedPallet = PalletPacker.UnpackJson(palletJsonText);

            if (loadedPallet != null)
            {
                return await LoadPalletAsync(loadedPallet);
            }
            else
            {
                Debug.LogWarning("AssetWarehouse: Cannot read Pallet json text");
                return false;
            }
        }

        public async UniTask<bool> LoadPalletAsync(Pallet pallet, string palletPath = "", bool updateCatalog = false)
        {
            bool success = false;

            if (warehouseManifest.ContainsKey(pallet.Barcode))
            {
                return await UniTask.FromResult(success);
            }


            if (pallet.Internal)
            {
                AddPallet(pallet);
                success = true;
            }


            if (!pallet.Internal && !string.IsNullOrEmpty(palletPath))
            {
                string modPath = Path.GetDirectoryName(palletPath);
                string catalogPath = Path.Combine(modPath, "catalog_" + pallet.Barcode + ".json");
                if (updateCatalog)
                {
                    var updatableCatalogs = await Addressables.CheckForCatalogUpdates(true);
                    string updateCatalogPath = null;
                    if (updatableCatalogs != null && updatableCatalogs.Count > 0)
                    {
                        foreach (var catalog in updatableCatalogs)
                        {
                            if (Path.GetFullPath(catalogPath) == Path.GetFullPath(catalog))
                            {
                                updateCatalogPath = catalog;
                            }
                        }

                        if (!string.IsNullOrEmpty(updateCatalogPath))
                        {
                            await Addressables.UpdateCatalogs(false, new[] { updateCatalogPath }, true);
                            AddPallet(pallet, catalogPath);
                        }
                    }
                }
                if (File.Exists(catalogPath))
                {
                    LogVerbose($"Found catalog for pallet {pallet.Barcode}: {catalogPath}", true);

                    var operationHandle = Addressables.LoadContentCatalogAsync(catalogPath, true);
                    var catalogLoc = await operationHandle;
                    if (catalogLoc == null)
                    {
                        Debug.LogError($"AssetWarehouse: Failed to open catalog for pallet {pallet.Barcode} at {catalogPath}");
                    }
                    else
                    {
                        LogVerbose($"Loaded catalog {catalogPath}: {catalogLoc.Keys.Count()} keys", true);
                        AddPallet(pallet, catalogPath);
                        success = true;
                    }
                }
            }
            else if (!pallet.Internal && string.IsNullOrEmpty(palletPath))
            {
                Debug.LogWarning($"AssetWarehouse: Cannot load pallet {pallet.Barcode} with null/empty pallet path");
            }

            return await UniTask.FromResult(success);
        }



#if UNITY_EDITOR
        public void ClearPallets()
        {
            _inventoryRegistry.Clear();
            warehouseManifest.Clear();

            OnChanged?.Invoke();
        }
#endif


    }

    public static class AssetWarehouseExtensions
    {
        public static List<T> Filter<T>(this List<T> crateList, ICrateFilter<T> crateFilter) where T : Crate
        {
            return AssetWarehouse.Instance.FilterCrates(ref crateList, crateFilter);
        }
    }

}