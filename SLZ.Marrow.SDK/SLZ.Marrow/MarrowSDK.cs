using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SLZ.Marrow
{
    public static class MarrowSDK
    {
        public const string SDK_NAME = "Marrow SDK";
        public const string SDK_VERSION = "0.3.7-1748";

        public const string DEV_NAME = "Stress Level Zero";
        public const string DEV_NAME_ALT = "StressLevelZero";
        public const string PACKAGE_NAME = "com.stresslevelzero.marrow.sdk";

        public static readonly string PROJ4_NAME = "BONELAB";
        public static readonly string[] GAME_NAMES = { PROJ4_NAME };

        public const string RUNTIME_MODS_DIRECTORY_NAME = "Mods";
        public const string INSTALL_STAGING_DIRECTORY_NAME = "ModsStaging";
        public static readonly int MAX_PATH = 260;

#if UNITY_EDITOR
        public const string BUILT_PALLETS_NAME = "BuiltPallets";
        public const string EDITOR_ASSETS_FOLDER = "_MarrowAssets";
#endif

        private static string _runtimeModsPath;
        public static string RuntimeModsPath
        {
            get
            {
                if (string.IsNullOrEmpty(_runtimeModsPath))
                {
                    _runtimeModsPath = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, RUNTIME_MODS_DIRECTORY_NAME);
                }

                if (!Directory.Exists(_runtimeModsPath))
                {
                    Directory.CreateDirectory(_runtimeModsPath);
                }
                return _runtimeModsPath;
            }
        }

        private static string _installStagingPath;
        public static string InstallStagingPath
        {
            get
            {
                if (string.IsNullOrEmpty(_installStagingPath))
                {
                    _installStagingPath = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, INSTALL_STAGING_DIRECTORY_NAME);
                }

                if (!Directory.Exists(_installStagingPath))
                {
                    Directory.CreateDirectory(_installStagingPath);
                }
                return _installStagingPath;
            }
        }

        private static string _modsCachePath;
        public static string ModsCachePath
        {
            get
            {
                if (string.IsNullOrEmpty(_modsCachePath))
                {
                    _modsCachePath = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "com.unity.addressables");
                }
                return _modsCachePath;
            }
        }


        private static string _modSettingsPath;
        public static string ModSettingsPath
        {
            get
            {
                if (string.IsNullOrEmpty(_modSettingsPath))
                {
                    _modSettingsPath = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "settings_mods.json");
                }
                return _modSettingsPath;
            }
        }


#if UNITY_EDITOR







        private static string _packagePath;
        public static string PackagePath
        {
            get
            {
                if (string.IsNullOrEmpty(_packagePath))
                {





#if false
#endif

#if true
                    _packagePath = $"Packages/{PACKAGE_NAME}/sdk";
#endif































































                }

                return _packagePath;
            }
        }

        private static Material _voidMaterial;
        public static Material VoidMaterial
        {
            get
            {
                if (_voidMaterial == null)
                {


                    _voidMaterial = AssetDatabase.LoadAssetAtPath<Material>(MarrowSDK.GetPackagePath("Editor\\Assets\\Materials\\Void Glow.mat"));
                }
                return _voidMaterial;
            }
        }

        private static Material _voidMaterialAlt;
        public static Material VoidMaterialAlt
        {
            get
            {
                if (_voidMaterialAlt == null)
                {
                    _voidMaterialAlt = AssetDatabase.LoadAssetAtPath<Material>(MarrowSDK.GetPackagePath("Editor\\Assets\\Materials\\Void Glow Alt.mat"));
                }
                return _voidMaterialAlt;
            }
        }

        private static Mesh _voidMesh;
        public static Mesh VoidMesh
        {
            get
            {
                if (_voidMesh == null)
                {
                    _voidMesh = AssetDatabase.LoadAssetAtPath<Mesh>(MarrowSDK.GetPackagePath("Editor\\Assets\\Mesh\\VOID.fbx"));
                }
                return _voidMesh;
            }
        }

        private static Mesh _genericHumanMesh;
        public static Mesh GenericHumanMesh
        {
            get
            {
                if (_genericHumanMesh == null)
                {
                    _genericHumanMesh = AssetDatabase.LoadAssetAtPath<Mesh>(MarrowSDK.GetPackagePath("Editor\\Assets\\Mesh\\GenericHumanoid.mesh"));
                }
                return _genericHumanMesh;
            }
        }


        public static string GetPackagePath(string path)
        {
            if (!string.IsNullOrEmpty(MarrowSDK.PackagePath) && !string.IsNullOrEmpty(path))
                return Path.Combine(MarrowSDK.PackagePath, path);
            else
                return path;
        }

        public static string GetMarrowAssetsPath(string path = "")
        {
            if (!string.IsNullOrEmpty(path))
                return Path.Combine("Assets", MarrowSDK.EDITOR_ASSETS_FOLDER, path);
            else
                return Path.Combine("Assets", MarrowSDK.EDITOR_ASSETS_FOLDER);
        }

        public static string GetMarrowAssetsPath(params string[] paths)
        {
            var pathArr = new List<string>();
            pathArr.Add("Assets");
            pathArr.Add(MarrowSDK.EDITOR_ASSETS_FOLDER);
            pathArr.AddRange(paths);

            if (paths.Length > 0)
                return Path.Combine(pathArr.ToArray());
            else
                return Path.Combine("Assets", MarrowSDK.EDITOR_ASSETS_FOLDER);
        }
#endif


        private static char[] invalidFilenameCharacters;
        public static string SanitizeName(string name)
        {
            if (invalidFilenameCharacters == null)
            {
                var invalids = System.IO.Path.GetInvalidFileNameChars().ToList();
                invalids.Add('[');
                invalids.Add(']');
                invalids.Add('-');
                invalidFilenameCharacters = invalids.ToArray();
            }

            return String.Join("", name.Split(invalidFilenameCharacters, StringSplitOptions.RemoveEmptyEntries));
        }


        public static string SanitizeID(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return id;
            }

            Regex regex = new Regex("[^a-zA-Z0-9.]");
            return regex.Replace(id, "");
        }

    }
}
