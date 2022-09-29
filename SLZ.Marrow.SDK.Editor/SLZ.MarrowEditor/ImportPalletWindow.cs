using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SLZ.Marrow;
using SLZ.Marrow.Warehouse;


namespace SLZ.MarrowEditor
{
    public class ImportPalletWindow : EditorWindow
    {
        private static readonly string PALLET_DEPENDENCIES_DIRNAME = "Pallet Dependencies";

        [MenuItem("Stress Level Zero/Void Tools/Import Pallet", false, 51)]
        static void Apply()
        {








            string path = EditorUtility.OpenFilePanel("Select " + Pallet.PALLET_JSON_FILENAME, MarrowSDK.RuntimeModsPath, "json");

            if (!string.IsNullOrEmpty(path))
            {
                Pallet pallet = PalletPacker.UnpackJsonFromFile(path);
                pallet.name = "_" + pallet.name;
                if (pallet == null)
                {
                    EditorUtility.DisplayDialog("Import Pallet", "Pallet could not be read from the file: " + path, "Ok");
                }
                else
                {
                    if (!AssetDatabase.IsValidFolder(MarrowSDK.GetMarrowAssetsPath()))
                    {
                        AssetDatabase.CreateFolder("Assets", MarrowSDK.GetMarrowAssetsPath());
                    }
                    if (!AssetDatabase.IsValidFolder(MarrowSDK.GetMarrowAssetsPath(PALLET_DEPENDENCIES_DIRNAME)))
                    {
                        AssetDatabase.CreateFolder(MarrowSDK.GetMarrowAssetsPath(), PALLET_DEPENDENCIES_DIRNAME);
                    }
                    if (!AssetDatabase.IsValidFolder(MarrowSDK.GetMarrowAssetsPath(PALLET_DEPENDENCIES_DIRNAME, pallet.Barcode)))
                    {
                        AssetDatabase.CreateFolder(MarrowSDK.GetMarrowAssetsPath(PALLET_DEPENDENCIES_DIRNAME), pallet.Barcode);
                    }
                    string destPath = MarrowSDK.GetMarrowAssetsPath(PALLET_DEPENDENCIES_DIRNAME, pallet.Barcode, (!pallet.Title.StartsWith("_") ? "_" : "") + pallet.Title + ".asset");

                    List<Crate> crates = new List<Crate>();
                    crates.AddRange(pallet.Crates);
                    pallet.Crates.Clear();

                    foreach (var crate in crates)
                    {
                        AssetDatabase.CreateAsset(crate, MarrowSDK.GetMarrowAssetsPath(PALLET_DEPENDENCIES_DIRNAME, pallet.Barcode, crate.Title + ".asset"));
                        string cratePath = AssetDatabase.GetAssetPath(crate);
                        pallet.Crates.Add(AssetDatabase.LoadAssetAtPath<Crate>(cratePath));
                    }

                    AssetDatabase.CreateAsset(pallet, destPath);
                    Selection.activeObject = pallet;
                    AssetWarehouse.Instance.LoadPalletsFromAssetDatabase(true);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Import Pallet", "Invalid " + Pallet.PALLET_JSON_FILENAME + " path", "Ok");
            }

        }
    }
}