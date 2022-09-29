using UnityEngine;
using UnityEditor;

namespace SLZ.MarrowEditor
{
    public class MarrowSDKPreferences
    {
        private class MarrowSDKSettingsProvider : SettingsProvider
        {
            public MarrowSDKSettingsProvider(string path, SettingsScope scopes = SettingsScope.User)
            : base(path, scopes)
            { }

            public override void OnGUI(string searchContext)
            {
                PreferencesGUI();
            }
        }

        [SettingsProvider]
        static SettingsProvider NewPreferenceProvider()
        {
            return new MarrowSDKSettingsProvider("Preferences/Marrow SDK");
        }



        private static bool prefsLoaded = false;


        public static bool startWarehouseLoadAssetDatabase = true;
        public static bool startWarehouseLoadModsFolder = true;
        public static bool startWarehouseLoadInternalBuiltPallets = true;
        public static bool verboseWarehouseLogging = false;
        public static bool proxyEditorDebug = false;
        public static bool unlockEditingScannables = false;
        public static bool loadAssetsFromAssetDatabase = true;



        public static void PreferencesGUI()
        {

            if (!prefsLoaded)
            {
                startWarehouseLoadAssetDatabase = EditorPrefs.GetBool("StartWarehouseLoadAssetDatabase", true);
                startWarehouseLoadModsFolder = EditorPrefs.GetBool("StartWarehouseLoadModsFolder", true);
                startWarehouseLoadInternalBuiltPallets = EditorPrefs.GetBool("StartWarehouseLoadInternalBuiltPallets", true);
                verboseWarehouseLogging = EditorPrefs.GetBool("VerboseWarehouseLogging", false);
                unlockEditingScannables = EditorPrefs.GetBool("UnlockEditingScannables", false);
                loadAssetsFromAssetDatabase = EditorPrefs.GetBool("LoadAssetsFromAssetDatabase", true);


                proxyEditorDebug = EditorPrefs.GetBool("ProxyEditorDebug", false);
                prefsLoaded = true;
            }


            EditorGUILayout.LabelField("Asset Warehouse", EditorStyles.boldLabel);
            startWarehouseLoadAssetDatabase = EditorGUILayout.Toggle("Load Pallets From AssetDatabase on Start", startWarehouseLoadAssetDatabase);
            startWarehouseLoadModsFolder = EditorGUILayout.Toggle("Load Pallets From Mods Folder on Start", startWarehouseLoadModsFolder);
            startWarehouseLoadInternalBuiltPallets = EditorGUILayout.Toggle("Load Internal Built Pallet on Start", startWarehouseLoadInternalBuiltPallets);
            verboseWarehouseLogging = EditorGUILayout.Toggle("Verbose AssetWarehouse Logging", verboseWarehouseLogging);
            unlockEditingScannables = EditorGUILayout.Toggle("Unlock Editing Pallets and Crates", unlockEditingScannables);
            loadAssetsFromAssetDatabase = EditorGUILayout.Toggle("Load Assets From AssetDatabase", loadAssetsFromAssetDatabase);

            EditorGUILayout.LabelField("Proxies", EditorStyles.boldLabel);
            proxyEditorDebug = EditorGUILayout.Toggle("Proxy Editor Debug", proxyEditorDebug);


            if (GUI.changed)
            {
                EditorPrefs.SetBool("StartWarehouseLoadAssetDatabase", startWarehouseLoadAssetDatabase);
                EditorPrefs.SetBool("StartWarehouseLoadModsFolder", startWarehouseLoadModsFolder);
                EditorPrefs.SetBool("StartWarehouseLoadInternalBuiltPallets", startWarehouseLoadInternalBuiltPallets);
                EditorPrefs.SetBool("VerboseWarehouseLogging", verboseWarehouseLogging);
                EditorPrefs.SetBool("UnlockEditingScannables", unlockEditingScannables);
                EditorPrefs.SetBool("ProxyEditorDebug", proxyEditorDebug);
                EditorPrefs.SetBool("LoadAssetsFromAssetDatabase", loadAssetsFromAssetDatabase);
            }
        }
    }
}