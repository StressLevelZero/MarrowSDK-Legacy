using System;
using System.Collections.Generic;
using SLZ.Marrow.Utilities;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SLZ.Marrow.Warehouse
{
    public interface IReadOnlyScannable
    {
        Barcode Barcode { get; }
        string Title { get; }
        string Description { get; }
    }

    public interface IScannable : IReadOnlyScannable
    {
        bool Unlockable { get; }
        bool Redacted { get; }

        void GenerateBarcode(bool forceGeneration = false);
        void GenerateBarcodeInternal(bool forceGeneration = false);

        List<PackedAsset> CollectPackedAssets();

#if UNITY_EDITOR
        void GeneratePackedAssets(bool saveAsset = true);
#endif
    }

    public abstract class Scannable : ScriptableObject, IScannable
    {
        [SerializeField]
        private Barcode _barcode;
        public Barcode Barcode
        {
            get { return _barcode; }
            set { _barcode = value; }
        }

        [SerializeField]
        private Barcode _barcodeOld;
        public Barcode BarcodeOld
        {
            get { return _barcodeOld; }
            set { _barcodeOld = value; }
        }

        [SerializeField]
        private MarrowGuid _slimCode;
        public MarrowGuid SlimCode
        {
            get { return _slimCode; }
            set { _slimCode = value; }
        }


        [SerializeField]
        [Delayed]
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        [SerializeField]
        private string _description = "";
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [SerializeField]
        [Tooltip("Locks the crate from the user until it is unlocked")]
        private bool _unlockable = false;
        public bool Unlockable
        {
            get { return _unlockable; }
            set { _unlockable = value; }
        }

        [SerializeField]
        [Tooltip("Hides the crate from Menus")]
        private bool _redacted = false;
        public bool Redacted
        {
            get { return _redacted; }
            set { _redacted = value; }
        }

        public void GenerateBarcode(bool forceGeneration = false)
        {
            if (Barcode == null || !Barcode.IsValid())
            {
                Barcode = new Barcode();
            }

            if (forceGeneration)
            {
                BarcodeOld = new Barcode(Barcode.ID);
            }
            GenerateBarcodeInternal(forceGeneration);
        }

        public abstract void GenerateBarcodeInternal(bool forceGeneration = false);

        private List<PackedAsset> packedAssets = new List<PackedAsset>();
        public List<PackedAsset> PackedAssets
        {
            get => packedAssets;
        }

        public virtual List<PackedAsset> CollectPackedAssets()
        {
            PackedAssets.Clear();
            return PackedAssets;
        }

#if UNITY_EDITOR
        public virtual void GeneratePackedAssets(bool saveAsset = true) { }

        [ContextMenu("Generate Barcode")]
        private void GenerateBarcodeMenuButton()
        {
            GenerateBarcode(true);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }

        [ContextMenu("Archive Barcode")]
        private void ArchiveBarcodeMenuButton()
        {
            BarcodeOld = Barcode;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }

        [ContextMenu("Generate Slimcode")]
        private void GenerateSlimCode()
        {
            var generatedCode = SlimCode;
            generatedCode.GenerateGuid(true);
            SlimCode = generatedCode;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
#endif

    }

}