using System.IO;
using UnityEngine;
using SLZ.Serialize;
using Newtonsoft.Json.Linq;

namespace SLZ.Marrow.Warehouse
{
    public class PalletPacker
    {
        public static bool ValidatePallet(Pallet pallet)
        {
            bool valid = true;

            if (pallet == null)
            {
                valid = false;
                Debug.LogWarning("PalletPacker Validate: Failed, pallet is null");
            }
            if (pallet.Barcode == null)
            {
                valid = false;
                Debug.LogWarning("PalletPacker Validate: Failed, pallet barcode is null");
            }
            if (string.IsNullOrEmpty(pallet.Title))
            {
                valid = false;
                Debug.LogWarning("PalletPacker Validate: Failed, pallet title is empty");
            }
            if (string.IsNullOrEmpty(pallet.Author))
            {
                valid = false;
                Debug.LogWarning("PalletPacker Validate: Failed, pallet author is empty");
            }
            if (pallet.Barcode == Barcode.EMPTY)
            {
                valid = false;
                Debug.LogWarning("PalletPacker Validate: Failed, pallet barcode is empty, generate now");
            }

            return valid;
        }


        public static void PackAndSaveToJson(Pallet pallet, string savePath)
        {
            string json = PackIntoJson(pallet);



            string path = savePath;
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(path))
            {
                path = Path.Combine(Directory.GetParent(Application.dataPath).ToString(), MarrowSDK.BUILT_PALLETS_NAME, pallet.Title + ".json");
            }
#endif

            File.WriteAllText(path, json);
        }

        public static string PackIntoJson(Pallet pallet)
        {
            string json = "";
            if (ValidatePallet(pallet))
            {
                var store = new ObjectStore();
                store.TryPack(pallet, out JObject obj);
                json = obj.ToString();
            }

            return json;
        }


        public static Pallet UnpackJsonFromFile(string path)
        {
            Pallet pallet = null;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                pallet = UnpackJson(json);
            }
            else
            {
                Debug.LogError("PalletPacker UnpackJsonFromFile: Could not find file " + path);
            }

            return pallet;
        }

        public static Pallet UnpackJson(string palletJson)
        {
            Pallet pallet = null;

            JObject obj = JObject.Parse(palletJson);
            var decodeTest = new ObjectStore(obj);
            decodeTest.LoadTypes(obj["types"] as JObject);
            pallet = ScriptableObject.CreateInstance<Pallet>();
            pallet.Unpack(decodeTest, new ObjectId(obj["root"]["ref"].ToObject<string>(), stripPrefix: true));

            return pallet;
        }


    }
}