using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets.Initialization;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using SLZ.Marrow;
using SLZ.Marrow.Warehouse;

namespace SLZ.MarrowEditor
{
    public class PalletPackerEditor
    {
        private static Pallet _currentPallet;
        public static Pallet CurrentPallet
        {
            get { return _currentPallet; }
            set { _currentPallet = value; }
        }

        public static string CurrentPalletTitle
        {
            get { return CurrentPallet.Title; }
        }

        public static string CurrentPalletBarcode
        {
            get { return CurrentPallet.Barcode.ToString(); }
        }










        public static bool PackPallet(Pallet pallet)
        {
            return PackPallet(pallet, true, !pallet.Internal, false);
        }


        public static bool PackPallet(Pallet pallet, bool setupGroups, bool buildContent, bool ignoreValidSettings)
        {


            if (!ignoreValidSettings && !MarrowProjectValidation.ValidateProject())
            {
                MarrowProjectValidationWindow.ShowWindow();

                return false;
            }

            if (buildContent)
            {
                if (Directory.Exists(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder))
                {
                    Directory.Delete(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder, true);
                }

                AssetDatabase.Refresh();

                List<MarrowProjectValidation.MarrowValidationRule> issues = new List<MarrowProjectValidation.MarrowValidationRule>();
                MarrowProjectValidation.GetIssues(issues);





                MarrowProjectValidation.FixIssues(issues);
            }



            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            if (!pallet.Internal)
            {
                CurrentPallet = pallet;
            }

            if (!pallet.Internal)
            {

                foreach (var crate in pallet.Crates)
                {
                    if (crate != null && crate is LevelCrate levelCrate)
                    {
                        levelCrate.ValidateSceneGUID();
                    }
                }
            }



            TextureStreamTool.ApplyTextureStreamingToAllTextures();





            AddressablesRuntimeProperties.ClearCachedPropertyValues();

            string buildPath = Path.GetFullPath(ModBuilder.BuildPath);


            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }
            else
            {
                if (!pallet.Internal && buildContent)
                {

                    System.IO.DirectoryInfo di = new DirectoryInfo(buildPath);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                }
            }

            GeneratePackedAssets(pallet);

            Dictionary<Type, AddressableAssetGroup> crateTypeToGroup = null;
            Dictionary<string, bool> groupNameToIncludeInBuild = null;

            if (setupGroups)
            {
                crateTypeToGroup = SetupPalletGroups(settings, pallet);


                if (!pallet.Internal)
                {
                    groupNameToIncludeInBuild = new Dictionary<string, bool>();
                    foreach (var group in settings.groups)
                    {
                        if (!crateTypeToGroup.ContainsValue(group) && group.HasSchema<BundledAssetGroupSchema>())
                        {
                            groupNameToIncludeInBuild[group.Name] = group.GetSchema<BundledAssetGroupSchema>().IncludeInBuild;
                            group.GetSchema<BundledAssetGroupSchema>().IncludeInBuild = false;
                        }
                    }
                }
            }



            if (buildContent)
            {
                PackPalletContent(pallet);
            }


            if (setupGroups)
            {

                if (!pallet.Internal)
                {
                    foreach (var group in settings.groups)
                    {
                        if (group.HasSchema<BundledAssetGroupSchema>())
                        {
                            if (!crateTypeToGroup.ContainsValue(group))
                            {
                                if (groupNameToIncludeInBuild.TryGetValue(group.Name, out bool includeInBuild))
                                {
                                    group.GetSchema<BundledAssetGroupSchema>().IncludeInBuild = includeInBuild;
                                }
                            }
                        }
                    }
                }
            }

            string palletJsonPath = "";

            if (pallet.Internal)
            {
                string palletPath = AssetDatabase.GetAssetPath(pallet);
                palletPath = System.IO.Path.GetDirectoryName(palletPath);
                palletJsonPath = Path.Combine(palletPath, "_Pallet_" + pallet.Barcode + ".json");
            }
            else
            {
                palletJsonPath = Path.Combine(buildPath, Pallet.PALLET_JSON_FILENAME);
            }

            PalletPacker.PackAndSaveToJson(pallet, palletJsonPath);

            return true;
        }

        public static Dictionary<Pallet, Dictionary<Type, AddressableAssetGroup>> SetupMultiplePalletGroups(AddressableAssetSettings settings, List<Pallet> pallets, bool clearAllPalletGroups = true)
        {
            if (clearAllPalletGroups)
            {
                for (int i = settings.groups.Count - 1; i >= 0; i--)
                {
                    var group = settings.groups[i];
                    if (group == null || (group.HasSchema<PalletGroupSchema>()))
                    {
                        settings.RemoveGroup(group);
                    }
                }
            }
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Dictionary<Pallet, Dictionary<Type, AddressableAssetGroup>> dict = new Dictionary<Pallet, Dictionary<Type, AddressableAssetGroup>>();

            foreach (var pallet in pallets)
            {
                dict.Add(pallet, SetupPalletGroups(settings, pallet, false));
            }

            return dict;
        }

        public static void GeneratePackedAssets(Pallet pallet, bool saveAssets = true)
        {
            pallet.CollectPackedAssets();
            pallet.GeneratePackedAssets(false);
            foreach (var crate in pallet.Crates)
            {
                crate.CollectPackedAssets();
                crate.GeneratePackedAssets(false);
            }

            if (saveAssets)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        public static void GeneratePackedAssets(List<Pallet> pallets)
        {
            foreach (var pallet in pallets)
            {
                GeneratePackedAssets(pallet, false);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        public static Dictionary<Type, AddressableAssetGroup> SetupPalletGroups(AddressableAssetSettings settings, Pallet pallet, bool clearPalletGroups = true)
        {
            if (pallet.SDKVersion != MarrowSDK.SDK_VERSION)
            {
                pallet.SDKVersion = MarrowSDK.SDK_VERSION;
                EditorUtility.SetDirty(pallet);
                AssetDatabase.SaveAssetIfDirty(pallet);
                AssetDatabase.Refresh();
            }


            if (clearPalletGroups)
            {
                for (int i = settings.groups.Count - 1; i >= 0; i--)
                {
                    var group = settings.groups[i];
                    if (group == null || (group.HasSchema<PalletGroupSchema>() && group.GetSchema<PalletGroupSchema>().Pallet.Barcode.Equals(pallet.Barcode)))
                    {
                        settings.RemoveGroup(group);
                    }
                }
            }


            var crateTypeToGroup = new Dictionary<Type, AddressableAssetGroup>();

            crateTypeToGroup[typeof(SpawnableCrate)] = CreatePalletCrateGroup(settings, pallet, typeof(SpawnableCrate));
            crateTypeToGroup[typeof(LevelCrate)] = CreatePalletCrateGroup(settings, pallet, typeof(LevelCrate));
            crateTypeToGroup[typeof(AvatarCrate)] = CreatePalletCrateGroup(settings, pallet, typeof(AvatarCrate));
            crateTypeToGroup[typeof(VFXCrate)] = CreatePalletCrateGroup(settings, pallet, typeof(VFXCrate));








            foreach (var crate in pallet.Crates)
            {
                if (crate != null && crate is Crate)
                {
                    if (crateTypeToGroup.TryGetValue(crate.GetType(), out var group))
                    {
                        string assetPath = AssetDatabase.GetAssetPath(crate.MainAsset.EditorAsset);
                        string cleanCrateTitle = MarrowSDK.SanitizeName(crate.Title);
                        if (!string.IsNullOrEmpty(assetPath))
                        {
                            string guid = AssetDatabase.AssetPathToGUID(assetPath);
                            if (!string.IsNullOrEmpty(guid))
                            {
                                AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
                                entry.address = $"{Crate.GetCrateName(crate.GetType())}/{cleanCrateTitle}";
                            }
                        }

                        foreach (var packedAsset in crate.CollectPackedAssets())
                        {
                            if (packedAsset.marrowAsset != null)
                            {
                                string packedAssetPath = AssetDatabase.GetAssetPath(packedAsset.marrowAsset.EditorAsset);
                                if (!string.IsNullOrEmpty(packedAssetPath))
                                {
                                    string guid = AssetDatabase.AssetPathToGUID(packedAssetPath);
                                    if (!string.IsNullOrEmpty(guid))
                                    {
                                        AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
                                        entry.address = $"{MarrowSDK.SanitizeName(packedAsset.title)}/{cleanCrateTitle}";
                                    }
                                }
                            }

                            foreach (var packedSubAsset in packedAsset.subAssets)
                            {
                                string packedSubAssetPath = AssetDatabase.GetAssetPath(packedSubAsset.subAsset.EditorAsset);
                                if (!string.IsNullOrEmpty(packedSubAssetPath))
                                {
                                    string guid = AssetDatabase.AssetPathToGUID(packedSubAssetPath);
                                    if (!string.IsNullOrEmpty(guid))
                                    {
                                        AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
                                        entry.address = $"{MarrowSDK.SanitizeName(packedAsset.title)}/{cleanCrateTitle}/{packedSubAsset.subTitle}";
                                    }
                                }
                            }
                        }
                    }
                }
            }



            for (int i = settings.groups.Count - 1; i >= 0; i--)
            {
                var group = settings.groups[i];
                if (group.HasSchema<PalletGroupSchema>() && group.Name.Contains(pallet.Barcode) && group.entries.Count == 0)
                {
                    settings.RemoveGroup(group);
                    foreach (var kvp in crateTypeToGroup)
                    {
                        if (kvp.Value == group)
                        {
                            crateTypeToGroup.Remove(kvp.Key);
                            break;
                        }
                    }
                }
            }

            foreach (var group in crateTypeToGroup.Values)
            {
                EditorUtility.SetDirty(group);
            }

            return crateTypeToGroup;
        }

        private static void PackPalletContent(Pallet pallet)
        {
            CurrentPallet = pallet;

            AddressablesRuntimeProperties.ClearCachedPropertyValues();

            AddressableAssetSettings.BuildPlayerContent();
        }

        private static AddressableAssetGroup CreatePalletPackedAssetGroup(AddressableAssetSettings settings, Pallet pallet, Type crateType, PackedAsset packedAsset)
        {
            var group = CreatePalletGroup(settings, AddressablesManager.PackedPalletGroupTemplate, pallet, Crate.GetCrateName(crateType), packedAsset.title);

            return group;
        }

        private static AddressableAssetGroup CreatePalletCrateGroup(AddressableAssetSettings settings, Pallet pallet, Type crateType)
        {
            AddressableAssetGroup group = null;

            if (pallet.Internal)
            {
                group = CreateInternalPalletCrateGroup(settings, pallet, crateType);
            }
            else
            {
                group = CreateExternalPalletCrateGroup(settings, pallet, crateType);
            }

            return group;
        }

        private static AddressableAssetGroup CreatePalletGroup(AddressableAssetSettings settings, AddressableAssetGroupTemplate groupTemplate, Pallet pallet, params string[] subTitles)
        {
            var groupSB = new StringBuilder(MarrowSDK.SanitizeID(pallet.Title));
            foreach (var subTitle in subTitles)
            {
                if (!string.IsNullOrEmpty(subTitle))
                {
                    groupSB.Append("_");
                    groupSB.Append(MarrowSDK.SanitizeName(subTitle));
                }
            }

            AddressableAssetGroup group = settings.CreateGroup(groupSB.ToString(), false, false, true, groupTemplate.SchemaObjects);

            if (group.HasSchema<PalletGroupSchema>())
            {
                group.GetSchema<PalletGroupSchema>().Pallet = pallet;
            }

            var schema = group.GetSchema<BundledAssetGroupSchema>();

            schema.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kLocalLoadPath);
            schema.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kLocalBuildPath);

            return group;
        }

        private static AddressableAssetGroup CreateInternalPalletCrateGroup(AddressableAssetSettings settings, Pallet pallet, Type crateType)
        {
            UnityEngine.Debug.Log("Hollowed Method: SLZ.MarrowEditor.PalletPackerEditor.CreateInternalPalletCrateGroup(UnityEditor.AddressableAssets.Settings.AddressableAssetSettings, SLZ.Marrow.Warehouse.Pallet, System.Type)");

            throw new System.NotImplementedException();
        }
        private static AddressableAssetGroup CreateExternalPalletCrateGroup(AddressableAssetSettings settings, Pallet pallet, Type crateType)
        {
            return CreatePalletGroup(settings, AddressablesManager.PackedPalletGroupTemplate, pallet, Crate.GetCrateName(crateType) + "s");
        }
    }
}