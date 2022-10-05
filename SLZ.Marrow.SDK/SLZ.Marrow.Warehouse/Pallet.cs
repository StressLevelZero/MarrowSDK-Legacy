using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;
using SLZ.Serialize;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SLZ.Marrow.Warehouse
{
    public partial class Pallet : Scannable, IJSONPackable, ISerializationCallbackReceiver
    {

        [SerializeField]
        private string _author;
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        [SerializeField]
        private string _version = "0.0.0";
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        [SerializeField]
        private string _sdkVersion = MarrowSDK.SDK_VERSION;
        public string SDKVersion
        {
            get { return _sdkVersion; }
            set { _sdkVersion = value; }
        }

        [SerializeField]
        private bool _internal = false;
        public bool Internal
        {
            get { return _internal; }
            set { _internal = value; }
        }

        [SerializeField]
        private List<Crate> _crates = new List<Crate>();
        public List<Crate> Crates
        {
            get { return _crates; }
            set { _crates = value; }
        }

        [SerializeField]
        private List<string> _tags = new List<string>();
        public List<string> Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }

        [System.Serializable]
        public struct ChangeLog
        {
            public string version;
            public string title;
            [TextArea(4, 20)]
            public string text;

            [Newtonsoft.Json.JsonConstructor]
            public ChangeLog(string version, string title, string text)
            {
                this.version = version;
                this.title = title;
                this.text = text;
            }
        }

        [SerializeField]
        private List<ChangeLog> _changeLogs = new List<ChangeLog>();
        public List<ChangeLog> ChangeLogs
        {
            get
            {
                return _changeLogs;
            }
        }

#if UNITY_EDITOR
        [SerializeField]

        private List<string> _crateTitles = new List<string>();
#endif




        public static readonly string PALLET_JSON_FILENAME = "pallet.json";

        public override void GenerateBarcodeInternal(bool forceGeneration = false)
        {
            Barcode.GenerateID(forceGeneration, Author, Title.Replace(".", ""));

            foreach (var crate in Crates)
            {
                crate.GenerateBarcode(true);
            }
        }













        public void Pack(ObjectStore store, JObject json)
        {
            json.Add("barcode", Barcode.ID);
            if (Barcode.IsValid(BarcodeOld))
                json.Add("barcodeOld", BarcodeOld.ID);
            json.Add("title", Title);
            json.Add("description", Description);
            json.Add("unlockable", Unlockable);
            json.Add("redacted", Redacted);
            json.Add("author", Author);
            json.Add("version", Version);
            json.Add("sdkVersion", SDKVersion);
            json.Add("internal", Internal);

            var jsonCrateArray = new JArray();
            foreach (var crate in Crates)
            {
                if (crate != null)
                {
                    jsonCrateArray.Add(store.PackReference(crate));
                }
            }
            json.Add(new JProperty("crates", jsonCrateArray));
            json.Add(new JProperty("tags", new JArray(Tags)));

            var changelogArray = new JArray();
            foreach (var changelog in ChangeLogs)
            {
                JObject logObject = new JObject();
                logObject.Add("version", changelog.version);
                logObject.Add("title", changelog.title);
                logObject.Add("text", changelog.text);
                changelogArray.Add(logObject);
            }
            json.Add(new JProperty("changelogs", changelogArray));

        }

        public void Unpack(ObjectStore store, ObjectId objectId)
        {
            if (store.TryGetJSON("barcode", forObject: objectId, out JToken barcodeValue))
            {
                Barcode = new Barcode(barcodeValue.ToObject<string>());
            }
            if (store.TryGetJSON("barcodeOld", forObject: objectId, out JToken barcodeOldValue) && Barcode.IsValid(barcodeOldValue.ToObject<string>()))
            {
                BarcodeOld = new Barcode(barcodeOldValue.ToObject<string>());
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
            if (store.TryGetJSON("author", forObject: objectId, out JToken authorValue))
            {
                Author = authorValue.ToObject<string>();
            }
            if (store.TryGetJSON("version", forObject: objectId, out JToken versionValue))
            {
                Version = versionValue.ToObject<string>();
            }
            if (store.TryGetJSON("sdkVersion", forObject: objectId, out JToken sdkVersionValue))
            {
                SDKVersion = sdkVersionValue.ToObject<string>();
            }
            if (store.TryGetJSON("internal", forObject: objectId, out JToken internalValue))
            {
                Internal = internalValue.ToObject<bool>();
            }
            if (store.TryGetJSON("crates", forObject: objectId, out JToken cratesValue))
            {
                JArray arr = (JArray)cratesValue;
                Crates = new List<Crate>();
                foreach (var crateValue in arr)
                {
                    store.TryCreateFromReference(crateValue, out Crate crate, t => Crate.CreateCrate(t, null, "", new MarrowAsset(), false));
                    crate.Pallet = this;
                    Crates.Add(crate);
                }
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
            if (store.TryGetJSON("changelogs", forObject: objectId, out JToken changeLogsValue))
            {
                JArray arr = (JArray)changeLogsValue;
                ChangeLogs.Clear();
                foreach (var changeLogValue in arr)
                {
                    string version = changeLogValue["version"].ToObject<string>();
                    string title = changeLogValue["title"].ToObject<string>();
                    string text = changeLogValue["text"].ToObject<string>();

                    ChangeLogs.Add(new ChangeLog(version, title, text));

                }
            }
        }


        public static Pallet CreatePallet(string title, string author, bool generateBarcode = true)
        {
            Pallet pallet = ScriptableObject.CreateInstance<Pallet>();
            pallet.Title = title;
            pallet.Author = author;
            if (generateBarcode)
            {
                pallet.GenerateBarcode();
            }
            return pallet;
        }


        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            _crateTitles.Clear();
            foreach (var crate in Crates)
            {
                if (crate != null)
                    _crateTitles.Add(crate.Title);
            }
#endif
        }

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR

#endif
        }

#if UNITY_EDITOR
        [ContextMenu("Save Json to File")]
        private void SaveJsonToAssetDatabase()
        {
            string palletPath = AssetDatabase.GetAssetPath(this);
            palletPath = System.IO.Path.GetDirectoryName(palletPath);
            string palletJsonPath = System.IO.Path.Combine(palletPath, "_Pallet_" + MarrowSDK.SanitizeName(this.Barcode) + ".json");

            PalletPacker.PackAndSaveToJson(this, palletJsonPath);
            AssetDatabase.Refresh();
        }

        [ContextMenu("Sort Crates")]
        public void SortCrates()
        {
            Crates.RemoveAll(item => item == null);

            foreach (var crate in Crates)
            {
                if (crate.Pallet == null)
                {
                    crate.Pallet = this;
                    EditorUtility.SetDirty(crate);
                }
            }

            Crates = Crates.OrderBy(crate => crate.GetType().Name)
            .ThenBy(crate =>
            {
                var cratePath = AssetDatabase.GetAssetPath(crate);
                var crateFilename = System.IO.Path.GetFileName(cratePath);
                return crateFilename;
            }).ToList();

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif

    }
}