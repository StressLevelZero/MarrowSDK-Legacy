using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;

namespace SLZ.MarrowEditor
{
    [InitializeOnLoad]
    public static class MarrowGenericValidationRules
    {
        private static readonly string SLZ_URP_CORE_GIT_URL = "https://github.com/StressLevelZero/Custom-RenderPipelineCore.git";
        private static readonly string SLZ_URP_GIT_URL = "https://github.com/StressLevelZero/Custom-URP.git";

        static MarrowGenericValidationRules()
        {
            MarrowProjectValidation.ValidationRules.AddRange(new List<MarrowProjectValidation.MarrowValidationRule>()
            {
                new MarrowProjectValidation.MarrowValidationRule
                {
                    order = 0,
                    message = "Addressables must be initialized",
                    Validate = () =>
                    {
                        return AddressableAssetSettingsDefaultObject.Settings != null;
                    },
                    FixRule = () =>
                    {
                        AddressableAssetSettingsDefaultObject.Settings = AddressableAssetSettings.Create(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder, AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName, true, true);
                    },
                    fixMessage = "Initialize Addressables"
                },
                new MarrowProjectValidation.MarrowValidationRule
                {
                    message = "Color space must be Linear",
                    Validate = () => { return PlayerSettings.colorSpace == ColorSpace.Linear; },
                    FixRule = () => { PlayerSettings.colorSpace = ColorSpace.Linear; },
                    fixMessage = "Set Player Settings Color Space to Linear"
                },
                new MarrowProjectValidation.MarrowValidationRule
                {
                    message = "Scripting Backend must be IL2CPP",
                    Validate = () => { return PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) == ScriptingImplementation.IL2CPP; },
                    FixRule = () => { PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.IL2CPP); },
                    fixMessage = "Set Player Settings Scripting Backend to IL2CPP"
                },
                new MarrowProjectValidation.MarrowValidationRule
                {
                    message = "API Compatibility Level must be NET 4.6",
                    Validate = () => { return PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup) == ApiCompatibilityLevel.NET_4_6; },
                    FixRule = () => { PlayerSettings.SetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup, ApiCompatibilityLevel.NET_4_6); },
                    fixMessage = "Set Player Settings Scripting Backend to IL2CPP"
                },
                new MarrowProjectValidation.MarrowValidationRule
                {
                    message = "Android Texture Compression Format must be ASTC",
                    Validate = () =>
                    {


                        return 3 == ((int) typeof(PlayerSettings).GetMethod("GetDefaultTextureCompressionFormat", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, new object[] {BuildTargetGroup.Android})!);
                    },
                    FixRule = () => { typeof(PlayerSettings).GetMethod("SetDefaultTextureCompressionFormat", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, new object[] {BuildTargetGroup.Android, 3}); },
                    fixMessage = "Set Android Texture Compression Format to ASTC"
                },





















                new()
                {
                    message = "Graphics API must be DX11 for Windows, Vulkan for Android",
                    Validate = () =>
                    {
                        return (!PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android)
                                   && PlayerSettings.GetGraphicsAPIs(BuildTarget.Android).Length == 1
                                   && PlayerSettings.GetGraphicsAPIs(BuildTarget.Android)[0] == GraphicsDeviceType.Vulkan)
                               && (!PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64)
                                   && PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows64).Length == 1
                                   && PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows64)[0] == GraphicsDeviceType.Direct3D11);
                    },
                    FixRule = () =>
                    {
                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
                        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new GraphicsDeviceType[] {GraphicsDeviceType.Vulkan});
                        PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, new GraphicsDeviceType[] {GraphicsDeviceType.Direct3D11});
                    },
                    fixMessage = "Switch to the required Graphics APIs"
                },
            });
        }

        public static async UniTaskVoid InstallSLZURP()
        {
            var request = Client.Add(SLZ_URP_CORE_GIT_URL);
            Events.registeringPackages += (a) =>
            {
                EditorUtility.ClearProgressBar();
            };
            while (!request.IsCompleted)
            {
                EditorUtility.DisplayProgressBar("Package Manager", "Installing SLZ-URP...", (Mathf.Sin(Time.realtimeSinceStartup) + 1) / 2f);
                await UniTask.Delay(TimeSpan.FromMilliseconds(100));
            }

            if (request.Status >= StatusCode.Failure)
                Debug.LogError(request.Error.message);
            EditorUtility.ClearProgressBar();


            request = Client.Add(SLZ_URP_GIT_URL);
            Events.registeringPackages += (a) =>
            {
                EditorUtility.ClearProgressBar();
            };
            while (!request.IsCompleted)
            {
                EditorUtility.DisplayProgressBar("Package Manager", "Installing SLZ-URP-CORE...", (Mathf.Sin(Time.realtimeSinceStartup) + 1) / 2f);
                await UniTask.Delay(TimeSpan.FromMilliseconds(100));
            }

            if (request.Status >= StatusCode.Failure)
                Debug.LogError(request.Error.message);
            EditorUtility.ClearProgressBar();
        }

    }
}