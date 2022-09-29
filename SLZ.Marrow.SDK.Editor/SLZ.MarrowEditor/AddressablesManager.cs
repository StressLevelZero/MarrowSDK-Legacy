using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using SLZ.Marrow;

namespace SLZ.MarrowEditor
{
    public class AddressablesManager
    {
        public static AddressableAssetSettings Settings
        {
            get
            {
                return AddressableAssetSettingsDefaultObject.Settings;
            }
            set
            {
                AddressableAssetSettingsDefaultObject.Settings = value;
            }
        }

        private const string MODS_RUNTIME_PATH = "{SLZ.Marrow.MarrowSDK.RuntimeModsPath}";

        public struct ProfileVariables
        {
            public static readonly string BUILD_TARGET = "BuildTarget";
            public static readonly string MOD_TITLE = "ModTitle";
            public static readonly string MOD_ID = "ModID";
            public static readonly string MOD_BUILD_PATH = "ModBuildPath";
            public static readonly string LOCAL_BUILD_PATH = AddressableAssetSettings.kLocalBuildPath;
            public static readonly string LOCAL_LOAD_PATH = AddressableAssetSettings.kLocalLoadPath;
        }

        private struct ProfileDefaultValues
        {
            public static readonly string BUILD_TARGET = "[UnityEditor.EditorUserBuildSettings.activeBuildTarget]";
            public static readonly string MOD_TITLE = "[SLZ.MarrowEditor.PalletPackerEditor.CurrentPalletFullName]";
            public static readonly string MOD_ID = "[SLZ.MarrowEditor.PalletPackerEditor.CurrentPalletBarcode]";
            public static readonly string MOD_BUILD_PATH = "[SLZ.Marrow.MarrowSDK.BUILT_PALLETS_NAME]";
            public static readonly string LOCAL_BUILD_PATH = "[" + ProfileVariables.MOD_BUILD_PATH + "]/[" + ProfileVariables.BUILD_TARGET + "]/[" + ProfileVariables.MOD_ID + "]";
            public static readonly string LOCAL_LOAD_PATH = MODS_RUNTIME_PATH + "/[" + ProfileVariables.MOD_ID + "]";
        }

        private static string _packageModSettingsPath;
        private static string packageModSettingsPath
        {
            get
            {
                if (string.IsNullOrEmpty(_packageModSettingsPath))
                {
                    _packageModSettingsPath = MarrowSDK.GetPackagePath("Editor\\AA\\AddressableAssetSettings\\PalletAddressableAssetSettings.asset");
                }
                return _packageModSettingsPath;
            }
        }

        private static string projectModSettingsPath = AddressableAssetSettingsDefaultObject.kDefaultConfigFolder + "\\PalletAddressableAssetSettings.asset";

        private static AddressableAssetSettings _modSettings = null;
        public static AddressableAssetSettings ModSettings
        {
            get
            {
                if (_modSettings == null)
                {
                    AddressableAssetSettings loadedSettings = (AddressableAssetSettings)AssetDatabase.LoadAssetAtPath(projectModSettingsPath, typeof(AddressableAssetSettings));
                    if (loadedSettings != null)
                    {
                        _modSettings = loadedSettings;
                    }
                }

                return _modSettings;
            }
            private set
            {
                _modSettings = value;
            }
        }

        public static AddressableAssetSettings SetupModSettings()
        {
            AddressableAssetSettings packageModSettings = (AddressableAssetSettings)AssetDatabase.LoadAssetAtPath(packageModSettingsPath, typeof(AddressableAssetSettings));
            if (packageModSettings != null)
            {
                AssetDatabase.CopyAsset(packageModSettingsPath, projectModSettingsPath);

                AddressableAssetSettings loadedSettings = (AddressableAssetSettings)AssetDatabase.LoadAssetAtPath(projectModSettingsPath, typeof(AddressableAssetSettings));
                loadedSettings.profileSettings.SetValue(loadedSettings.profileSettings.GetProfileId("Pallet"), ProfileVariables.BUILD_TARGET, ProfileDefaultValues.BUILD_TARGET);
                loadedSettings.profileSettings.SetValue(loadedSettings.profileSettings.GetProfileId("Pallet"), ProfileVariables.MOD_TITLE, ProfileDefaultValues.MOD_TITLE);
                loadedSettings.profileSettings.SetValue(loadedSettings.profileSettings.GetProfileId("Pallet"), ProfileVariables.MOD_ID, ProfileDefaultValues.MOD_ID);
                loadedSettings.profileSettings.SetValue(loadedSettings.profileSettings.GetProfileId("Pallet"), ProfileVariables.MOD_BUILD_PATH, ProfileDefaultValues.MOD_BUILD_PATH);
                loadedSettings.profileSettings.SetValue(loadedSettings.profileSettings.GetProfileId("Pallet"), ProfileVariables.LOCAL_BUILD_PATH, ProfileDefaultValues.LOCAL_BUILD_PATH);
                loadedSettings.profileSettings.SetValue(loadedSettings.profileSettings.GetProfileId("Pallet"), ProfileVariables.LOCAL_LOAD_PATH, ProfileDefaultValues.LOCAL_LOAD_PATH);







                if (loadedSettings.DefaultGroup != null && !loadedSettings.groups.Contains(loadedSettings.DefaultGroup))
                {
                    loadedSettings.groups.Add(loadedSettings.DefaultGroup);
                }
                AddressableAssetSettingsDefaultObject.Settings = ModSettings;

                AssetDatabase.SaveAssets();
            }

            return ModSettings;
        }


        private static AddressableAssetGroupTemplate _packedPalletGroupTemplate;
        public static AddressableAssetGroupTemplate PackedPalletGroupTemplate
        {
            get
            {
                if (_packedPalletGroupTemplate == null)
                {
                    AddressableAssetGroupTemplate loadedTemplate = (AddressableAssetGroupTemplate)AssetDatabase.LoadAssetAtPath(System.IO.Path.Combine(MarrowSDK.PackagePath, "Editor/AA/AssetGroupTemplates/Packed Pallet Assets.asset"), typeof(AddressableAssetGroupTemplate));
                    if (loadedTemplate != null)
                    {
                        _packedPalletGroupTemplate = loadedTemplate;
                    }
                }

                return _packedPalletGroupTemplate;
            }
        }

        private static AddressableAssetGroupTemplate _splitPalletInternalTemplate;
        public static AddressableAssetGroupTemplate SplitPalletInternalTemplate
        {
            get
            {
                if (_splitPalletInternalTemplate == null)
                {
                    AddressableAssetGroupTemplate loadedTemplate = (AddressableAssetGroupTemplate)AssetDatabase.LoadAssetAtPath(System.IO.Path.Combine(MarrowSDK.PackagePath, "Editor/AA/AssetGroupTemplates/Split Pallet Internal.asset"), typeof(AddressableAssetGroupTemplate));
                    if (loadedTemplate != null)
                    {
                        _splitPalletInternalTemplate = loadedTemplate;
                    }
                }

                return _splitPalletInternalTemplate;
            }
        }


        public static string EvaluateProfileValue(string variableName)
        {
            string value;
            if (Settings == null)
            {
                value = "SETTINGS NULL";
            }
            else
            {
                value = Settings.profileSettings.GetValueByName(Settings.activeProfileId, variableName);
                value = Settings.profileSettings.EvaluateString(Settings.activeProfileId, value);
            }
            return value;
        }

    }
}