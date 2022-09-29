using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Management;

namespace SLZ.MarrowEditor
{
#if !MARROW_PROJECT
    [InitializeOnLoad]
    public static class MarrowSDKValidationRules
    {
        private const string XR_LOADER_OCULUS_SETTINGS_NAME = "Unity.XR.Oculus.OculusLoader";
        private static string XR_LOADER_MOCK_SETTINGS_NAME = "Unity.XR.MockHMD.MockHMDLoader";

        static MarrowSDKValidationRules()
        {
            MarrowProjectValidation.ValidationRules.AddRange(new List<MarrowProjectValidation.MarrowValidationRule>()
            {
#if true
                new MarrowProjectValidation.MarrowValidationRule
                {
                    message = "Addressable Settings need fixing",
                    Validate = () => AddressablesManager.ModSettings != null,
                    FixRule = () => AddressablesManager.SetupModSettings(),
                    fixMessage = "Switch to use correct Addressable Settings File"
                },

                new()
                {
                    message = "XR Settings Loaders incorrect",
                    Validate = () =>
                    {
                        bool xrLoaderCorrect = EditorUserBuildSettings.selectedBuildTargetGroup switch
                        {
                            BuildTargetGroup.Standalone => XRPackageMetadataStore.IsLoaderAssigned(XR_LOADER_MOCK_SETTINGS_NAME, BuildTargetGroup.Standalone),
                            BuildTargetGroup.Android => XRPackageMetadataStore.IsLoaderAssigned(XR_LOADER_OCULUS_SETTINGS_NAME, BuildTargetGroup.Android),
                            _ => false
                        };

                        return xrLoaderCorrect;
                    },
                    FixRule = () =>
                    {
                        XRGeneralSettingsPerBuildTarget generalSettingsForTarget = typeof(XRGeneralSettingsPerBuildTarget)
                            .GetMethod("GetOrCreate", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null) as XRGeneralSettingsPerBuildTarget;

                        if (!generalSettingsForTarget.HasManagerSettingsForBuildTarget(BuildTargetGroup.Standalone))
                        {
                            generalSettingsForTarget.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.Standalone);
                        }

                        if (!generalSettingsForTarget.HasManagerSettingsForBuildTarget(BuildTargetGroup.Android))
                        {
                            generalSettingsForTarget.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.Android);
                        }




                        XRGeneralSettings standaloneSettings = generalSettingsForTarget.SettingsForBuildTarget(BuildTargetGroup.Standalone);
                        XRManagerSettings xrManagerSettings = standaloneSettings.AssignedSettings;
                        var activeLoaders = xrManagerSettings.activeLoaders;
                        foreach (var activeLoader in activeLoaders)
                        {
                            xrManagerSettings.TryRemoveLoader(activeLoader);
                        }
                        if (!XRPackageMetadataStore.AssignLoader(xrManagerSettings, XR_LOADER_MOCK_SETTINGS_NAME, BuildTargetGroup.Standalone))
                        {
                            Debug.LogError("MarrowSDKValidationRules: CRITICAL ERROR adding XR Settings Loaders");
                        }

                        XRGeneralSettings androidSettings = generalSettingsForTarget.SettingsForBuildTarget(BuildTargetGroup.Android);
                        xrManagerSettings = androidSettings.AssignedSettings;
                        activeLoaders = xrManagerSettings.activeLoaders;
                        foreach (var activeLoader in activeLoaders)
                        {
                            xrManagerSettings.TryRemoveLoader(activeLoader);
                        }
                        if (!XRPackageMetadataStore.AssignLoader(xrManagerSettings, XR_LOADER_OCULUS_SETTINGS_NAME, BuildTargetGroup.Android))
                        {
                            Debug.LogError("MarrowSDKValidationRules: CRITICAL ERROR adding XR Settings Loaders");
                        }
                    },
                    fixMessage = "Switch to use required XR Loader"
                },
#endif

            });
        }
    }
#endif
}