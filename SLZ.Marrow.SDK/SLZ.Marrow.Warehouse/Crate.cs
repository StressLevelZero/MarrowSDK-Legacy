using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;
using SLZ.Serialize;
using SLZ.Marrow.Utilities;
using Object = UnityEngine.Object;

namespace SLZ.Marrow.Warehouse
{
    public interface ICrate
    {
        MarrowAsset MainAsset { get; set; }

        Type AssetType { get; }
    }

    public abstract class Crate : Scannable, ICrate, IJSONPackable
    {
        public virtual MarrowAsset MainAsset { get; set; }
        public virtual Type AssetType
        {
            get { return typeof(UnityEngine.Object); }
        }



        [SerializeField]
        private Pallet _pallet = null;
        public Pallet Pallet
        {
            get { return _pallet; }
            set { _pallet = value; }
        }

        [SerializeField]
        private List<string> _tags = new List<string>();
        public List<string> Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }





        private static Dictionary<Type, string> _crateNames = new Dictionary<Type, string>();
        public static Dictionary<Type, string> CrateNames { get { return _crateNames; } }

        public static string GetCrateName(Type crateType)
        {
            if (!CrateNames.TryGetValue(crateType, out var crateName))
            {

                crateName = crateType.Name.Replace("Crate", "");
                CrateNames[crateType] = crateName;
            }
            return crateName;
        }

        public override void GenerateBarcodeInternal(bool forceGeneration = false)
        {
            Barcode.GenerateID(forceGeneration, Pallet.Barcode.ID, GetCrateName(this.GetType()), Title.Replace(".", ""));
        }

        public virtual async UniTask<Object> LoadAssetAsync()
        {
            return await MainAsset.LoadAssetAsync<Object>();
        }

        public virtual void LoadAsset(Action<Object> loadCallback)
        {
            MainAsset.LoadAsset(loadCallback);
        }


        public virtual void Pack(ObjectStore store, JObject json)
        {
            json.Add("barcode", Barcode.ID);
            if (Barcode.IsValid(BarcodeOld))
                json.Add("barcodeOld", BarcodeOld.ID);


            if (SlimCode.IsValid())
                json.Add("slimCode", SlimCode.ToHexString());
            json.Add("title", Title);
            json.Add("description", Description);
            json.Add("unlockable", Unlockable);
            json.Add("redacted", Redacted);
            json.Add("mainAsset", MainAsset.AssetGUID);
            json.Add(new JProperty("tags", new JArray(Tags)));


            var packedAssets = CollectPackedAssets();
            var packedAssetsArray = new JArray();
            foreach (var packedAsset in packedAssets)
            {
                var packedAssetJObject = new JObject
                {
                    {"title", packedAsset.title},
                };

                if (packedAsset.marrowAsset != null)
                    packedAssetJObject.Add(new JProperty("guid", packedAsset.marrowAsset.AssetGUID));
                if (packedAsset.subAssets.Count > 0)
                {
                    packedAssetJObject.Add(new JProperty("subAssets",
                        new JArray(packedAsset.subAssets.Select(p => p.subAsset.AssetGUID)
                    )));
                }

                if (packedAsset.marrowAsset != null || packedAsset.subAssets.Count > 0)
                    packedAssetsArray.Add(packedAssetJObject);
            }
            json.Add(new JProperty("packedAssets", packedAssetsArray));










        }

        public virtual void Unpack(ObjectStore store, ObjectId objectId)
        {
            if (store.TryGetJSON("barcode", forObject: objectId, out JToken barcodeValue))
            {
                Barcode = new Barcode(barcodeValue.ToObject<string>());
            }
            if (store.TryGetJSON("barcodeOld", forObject: objectId, out JToken barcodeOldValue) && Barcode.IsValid(barcodeOldValue.ToObject<string>()))
            {
                BarcodeOld = new Barcode(barcodeOldValue.ToObject<string>());
            }
            if (store.TryGetJSON("slimCode", forObject: objectId, out JToken slimCodeValue))
            {
                SlimCode = new MarrowGuid(slimCodeValue.ToString());
            }
            if (store.TryGetJSON("title", forObject: objectId, out JToken titleValue))
            {
                name = titleValue.ToObject<string>();
                Title = titleValue.ToObject<string>();
            }
            if (store.TryGetJSON("description", forObject: objectId, out JToken descValue))
            {
                Description = descValue.ToObject<string>();
            }
            if (store.TryGetJSON("unlockable", forObject: objectId, out JToken unlockValue))
            {
                Unlockable = unlockValue.ToObject<bool>();
            }
            if (store.TryGetJSON("redacted", forObject: objectId, out JToken redaValue))
            {
                Redacted = redaValue.ToObject<bool>();
            }
            if (store.TryGetJSON("mainAsset", forObject: objectId, out JToken marrowAssetValue))
            {
                MainAsset = new MarrowAsset(marrowAssetValue.ToObject<string>());
            }
            if (store.TryGetJSON("tags", forObject: objectId, out JToken tagsValue))
            {


                JArray arr = (JArray)tagsValue;
                Tags = new List<string>();
                foreach (var tagValue in arr)
                {
                    Tags.Add(tagValue.ToObject<string>());
                }
            }





        }



        public static Crate CreateCrate(System.Type type, Pallet pallet, string title, MarrowAsset marrowAsset, bool generateBarcode = true)
        {
            Crate crate = null;
            if (type == typeof(SpawnableCrate) || type == typeof(LevelCrate) || type == typeof(AvatarCrate) || type == typeof(VFXCrate))
            {
                crate = (Crate)ScriptableObject.CreateInstance(type);
                crate.Title = title;
                crate.Pallet = pallet;
                crate.MainAsset = marrowAsset;
                if (generateBarcode)
                {
                    crate.GenerateBarcode();
                    crate.SlimCode.GenerateGuid(!crate.Pallet.Internal);
                }
            }

            return crate;
        }

        public static T CreateCrateT<T>(Pallet pallet, string title, MarrowAsset marrowAsset, bool generateBarcode = true) where T : Crate
        {
            return (T)CreateCrate(typeof(T), pallet, title, marrowAsset, generateBarcode);
        }

    }

    public abstract class CrateT<T> : Crate where T : UnityEngine.Object
    {
        public override Type AssetType
        {
            get
            {
                return typeof(T);
            }
        }


        public new async UniTask<T> LoadAssetAsync()
        {
            return await MainAsset.LoadAssetAsync<T>();
        }

        public void LoadAsset(Action<T> loadCallback)
        {
            MainAsset.LoadAsset(loadCallback);
        }


    }

}