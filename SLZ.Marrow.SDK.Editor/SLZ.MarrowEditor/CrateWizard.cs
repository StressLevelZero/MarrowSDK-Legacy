using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SLZ.Marrow.Warehouse;

namespace SLZ.MarrowEditor
{
    public class CrateWizard : ScriptableWizard
    {
        public enum CrateType
        {
            SPAWNABLE_CRATE,
            LEVEL_CRATE,
            AVATAR_CRATE,
            VFX_CRATE
        }

        public Barcode barcode = Barcode.EmptyBarcode();
        public Pallet pallet;
        public CrateType crateType;
        public string crateTitle = "";
        public Object assetReference;

        private Dictionary<System.Type, System.Type> assetTypeCache;
        private Crate tempCrate;

        private Pallet lastPallet;
        private string lastCrateTitle;
        private CrateType lastCrateType;
        private Object lastAssetRef;


        public static void CreateWizard()
        {
            CreateWizard(null);
        }

        public static void CreateWizard(Pallet pallet)
        {
            var wizard = ScriptableWizard.DisplayWizard<CrateWizard>("Create Crate", "Create");
            wizard.errorString = "";
            wizard.helpString = "Use this wizard to add a Crate to the Asset Warehouse.  Take care naming the crate as its title will be used as part of its unique barcode, which cannot be changed once set. \n"
            + "\n"
            + "Deleting crates and pallets from the Asset Warehouse is not recommended. \n";

            foreach (var selected in Selection.objects)
            {
                if (selected != null && selected.GetType() == typeof(Pallet))
                {
                    pallet = (Pallet)selected;
                }
                else if (selected != null && selected.GetType() != typeof(Pallet))
                {
                    if (selected.GetType() == typeof(SceneAsset))
                    {
                        wizard.crateType = CrateType.LEVEL_CRATE;
                    }
                    else if (selected.GetType() == typeof(GameObject))
                    {
                        if ((selected as GameObject).GetComponent("SLZ.VRMK.Avatar"))
                        {
                            wizard.crateType = CrateType.AVATAR_CRATE;
                        }
                        else
                        {
                            wizard.crateType = CrateType.SPAWNABLE_CRATE;
                        }
                    }

                    if (AssetDatabase.Contains(selected))
                    {
                        wizard.assetReference = selected;
                        wizard.crateTitle = ObjectNames.NicifyVariableName(selected.name);
                    }
                }
            }

            if (pallet == null)
            {
                var palletGuids = AssetDatabase.FindAssets("t:Pallet");
                if (palletGuids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(palletGuids[0]);
                    pallet = AssetDatabase.LoadAssetAtPath<Pallet>(path);
                }
            }

            if (pallet != null)
            {
                wizard.pallet = pallet;
            }

            wizard.OnWizardUpdate();
        }

        public System.Type GetCrateType(CrateType type)
        {
            switch (type)
            {
                case CrateType.SPAWNABLE_CRATE:
                    return typeof(SpawnableCrate);
                case CrateType.LEVEL_CRATE:
                    return typeof(LevelCrate);
                case CrateType.AVATAR_CRATE:
                    return typeof(AvatarCrate);
                case CrateType.VFX_CRATE:
                    return typeof(VFXCrate);
                default:
                    return null;
            }
        }

        public CrateType GetCrateType(System.Type type)
        {
            if (type == typeof(SpawnableCrate))
            {
                return CrateType.SPAWNABLE_CRATE;
            }
            else if (type == typeof(LevelCrate))
            {
                return CrateType.LEVEL_CRATE;
            }
            else if (type == typeof(AvatarCrate))
            {
                return CrateType.AVATAR_CRATE;
            }
            else if (type == typeof(VFXCrate))
            {
                return CrateType.VFX_CRATE;
            }
            return CrateType.SPAWNABLE_CRATE;
        }

        public System.Type GetCrateAssetType(System.Type type)
        {
            if (assetTypeCache == null)
            {
                assetTypeCache = new Dictionary<System.Type, System.Type>();

                Crate crate = ScriptableObject.CreateInstance<SpawnableCrate>();
                assetTypeCache[typeof(SpawnableCrate)] = crate.AssetType;

                crate = ScriptableObject.CreateInstance<LevelCrate>();
                assetTypeCache[typeof(LevelCrate)] = crate.AssetType;

                crate = ScriptableObject.CreateInstance<AvatarCrate>();
                assetTypeCache[typeof(AvatarCrate)] = crate.AssetType;

                crate = ScriptableObject.CreateInstance<VFXCrate>();
                assetTypeCache[typeof(VFXCrate)] = crate.AssetType;
            }

            if (assetTypeCache.TryGetValue(type, out var assetType))
            {
                return assetType;
            }
            else
            {
                return null;
            }
        }

        void OnWizardCreate()
        {
            System.Type type = GetCrateType(crateType);
            if (type != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(assetReference);
                MarrowAsset crateAssetReference = null;
                if (!string.IsNullOrEmpty(assetPath))
                {
                    string guid = AssetDatabase.AssetPathToGUID(assetPath);
                    if (!string.IsNullOrEmpty(guid))
                    {
                        crateAssetReference = new MarrowAsset(guid);
                    }
                }

                if (crateAssetReference != null)
                {

                    Crate crate = Crate.CreateCrate(type, pallet, crateTitle, crateAssetReference);

                    string palletPath = AssetDatabase.GetAssetPath(pallet);
                    palletPath = System.IO.Path.GetDirectoryName(palletPath);
                    AssetDatabase.CreateAsset(crate, palletPath + "/" + crateTitle + ".asset");

                    pallet.Crates.Add(crate);

                    EditorUtility.SetDirty(pallet);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    AssetWarehouse.Instance.LoadPalletsFromAssetDatabase(true);
                }
                else
                {
                    Debug.LogError("Invalid Asset");
                }
            }
        }

        void OnWizardUpdate()
        {
            errorString = "";

            bool generateBarcode = false;
            bool generateCrateTitle = false;

            if (pallet == null)
            {
                errorString += "Missing Pallet!";
            }
            else if (lastPallet != pallet)
            {
                generateBarcode = true;
            }

            if (assetReference == null)
            {
                if (!string.IsNullOrEmpty(errorString))
                    errorString += "\n";
                errorString += "Missing Asset Reference!";
            }
            else if (!AssetDatabase.Contains(assetReference))
            {
                if (!string.IsNullOrEmpty(errorString))
                    errorString += "\n";
                errorString += "Invalid Asset Reference!";
            }
            else
            {
                System.Type assetType = GetCrateAssetType(GetCrateType(crateType));
                if (assetType != assetReference.GetType())
                {
                    if (!string.IsNullOrEmpty(errorString))
                        errorString += "\n";
                    errorString += "Asset Reference must be a " + assetType.Name + "!";
                }
                else
                {
                    if (string.IsNullOrEmpty(crateTitle))
                    {
                        generateCrateTitle = true;
                    }
                    if (lastAssetRef != assetReference)
                    {
                        generateCrateTitle = true;
                        generateBarcode = true;
                    }
                }
            }

            if (generateCrateTitle)
            {
                crateTitle = ObjectNames.NicifyVariableName(assetReference.name);
            }

            if (string.IsNullOrEmpty(crateTitle))
            {
                if (!string.IsNullOrEmpty(errorString))
                    errorString += "\n";
                errorString += "Missing Title!";
            }
            else if (crateTitle != lastCrateTitle)
            {
                generateBarcode = true;
            }

            if (crateType != lastCrateType)
            {
                generateBarcode = true;
            }

            if (generateBarcode)
            {
                tempCrate = Crate.CreateCrate(GetCrateType(crateType), pallet, crateTitle, null);
                barcode = new Barcode(tempCrate.Barcode);
            }

            if (!Barcode.IsValidSize(barcode))
            {
                if (!string.IsNullOrEmpty(errorString))
                    errorString += "\n";
                errorString += "Barcode too long! Max length is " + Barcode.MAX_SIZE;
            }

            if (AssetWarehouse.Instance.HasCrate(barcode))
            {
                if (!string.IsNullOrEmpty(errorString))
                    errorString += "\n";
                errorString += "Crate already exists with that Barcode!";
            }

            isValid = string.IsNullOrEmpty(errorString);


            lastPallet = pallet;
            lastCrateTitle = crateTitle;
            lastCrateType = crateType;
            lastAssetRef = assetReference;
        }

    }
}