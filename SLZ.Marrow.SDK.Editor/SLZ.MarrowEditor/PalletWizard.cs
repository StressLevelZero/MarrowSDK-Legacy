using System.IO;
using SLZ.Marrow;
using UnityEditor;
using SLZ.Marrow.Warehouse;

namespace SLZ.MarrowEditor
{
    public class PalletWizard : ScriptableWizard
    {
        public string palletTitle = "My Pallet";
        public string palletAuthor = "Author";
        public static readonly string palletFolderName = "_Pallets";

        [MenuItem("Assets/New Pallet", false, 42160)]
        static void CreateWizard()
        {
            var wizard = ScriptableWizard.DisplayWizard<PalletWizard>("Create Pallet", "Create");

            wizard.helpString = "";
            wizard.errorString = "";

            wizard.OnWizardUpdate();
        }

        void OnWizardCreate()
        {
            Pallet pallet = Pallet.CreatePallet(palletTitle, palletAuthor);

            if (!Directory.Exists(MarrowSDK.GetMarrowAssetsPath(palletFolderName, pallet.Barcode)))
            {
                Directory.CreateDirectory(MarrowSDK.GetMarrowAssetsPath(palletFolderName, pallet.Barcode));
            }

            AssetDatabase.CreateAsset(pallet, MarrowSDK.GetMarrowAssetsPath(palletFolderName, pallet.Barcode, "_" + palletTitle + ".asset"));

            EditorUtility.SetDirty(pallet);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            AssetWarehouse.Instance.LoadPalletsFromAssetDatabase(true);
        }

        void OnWizardUpdate()
        {
            helpString = "A Marrow mod is structured into a single Pallet that can contain one or many Crates of various types.\n\nPallets can refer to the contents of other Pallets through dependencies.\n";

            errorString = "";
            if (string.IsNullOrEmpty(palletTitle))
            {
                errorString += "Missing Title!";
            }
            if (string.IsNullOrEmpty(palletAuthor))
            {
                if (!string.IsNullOrEmpty(errorString))
                    errorString += "\n";
                errorString += "Missing Author!";
            }

            isValid = string.IsNullOrEmpty(errorString);
        }

    }
}