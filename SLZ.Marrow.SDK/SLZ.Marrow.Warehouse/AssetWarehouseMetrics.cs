using Unity.Profiling;
using UnityEngine;

#if UNITY_EDITOR
using Unity.Profiling.Editor;
#endif

namespace SLZ.Marrow.Warehouse
{
    public static class AssetWarehouseMetrics
    {
        public static readonly ProfilerCategory AssetWarehouseCategory = ProfilerCategory.Scripts;

        public const string LoadedCrateAssetsCountName = "Loaded Crate Assets";

        public static readonly ProfilerCounterValue<int> LoadedCrateAssetsCount =
            new ProfilerCounterValue<int>(AssetWarehouseCategory, LoadedCrateAssetsCountName, ProfilerMarkerDataUnit.Count);


        public static void Reset()
        {
            LoadedCrateAssetsCount.Value = 0;
        }


#if UNITY_EDITOR
        [System.Serializable]
        [ProfilerModuleMetadata("Asset Warehouse")]
        public class AssetWarehouseProfileModule : ProfilerModule
        {
            static readonly ProfilerCounterDescriptor[] counters = new ProfilerCounterDescriptor[]
            {
                new ProfilerCounterDescriptor(AssetWarehouseMetrics.LoadedCrateAssetsCountName, AssetWarehouseMetrics.AssetWarehouseCategory),
            };

            static readonly string[] autoEnabledCategoryNames = new string[]
            {
                ProfilerCategory.Scripts.Name
            };

            public AssetWarehouseProfileModule() : base(counters, autoEnabledCategoryNames: autoEnabledCategoryNames) { }
        }
#endif
    }
}