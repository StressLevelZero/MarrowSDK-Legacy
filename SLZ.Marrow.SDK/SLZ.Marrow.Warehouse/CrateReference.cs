using System;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace SLZ.Marrow.Warehouse
{

    public interface ICrateReference
    {
        Barcode Barcode { get; set; }
        Crate Crate { get; }
        bool TryGetCrate(out Crate crate);

#if UNITY_EDITOR
        Type CrateType { get; }
#endif
    }


    [System.Serializable]
    public abstract class CrateReference : ICrateReference
    {
        [SerializeField]
        protected Barcode _barcode = Barcode.EmptyBarcode();

        public Barcode Barcode
        {
            get => _barcode;
            set => _barcode = value;
        }

        [SerializeField]
        private string _version;

        public string Version
        {
            get => _version;
            set => _version = value;
        }


        [SerializeField]
        private Barcode _palletBarcode = Barcode.EmptyBarcode();

        public Barcode PalletBarcode
        {
            get => _palletBarcode;
            set => _palletBarcode = value;
        }

        public CrateReference()
        {
        }

        public CrateReference(string barcode)
        {
            this.Barcode = new Barcode(barcode);
        }

        public virtual Crate Crate
        {
            get
            {
                Crate retCrate = null;
                if (AssetWarehouse.ready)
                {
                    retCrate = AssetWarehouse.Instance.GetCrate(Barcode);
                }

                return retCrate;
            }
        }

        public bool TryGetCrate(out Crate crate)
        {
            crate = null;
            bool success = false;

            if (AssetWarehouse.ready)
            {
                success = AssetWarehouse.Instance.TryGetCrate(Barcode, out crate);
            }

            return success;
        }

        public bool IsValid()
        {
            return Barcode.IsValid();
        }

        public static bool IsValid(CrateReference crateReference)
        {
            return crateReference != null && Barcode.IsValid(crateReference.Barcode);
        }

#if UNITY_EDITOR
        protected Crate _cachedCrate;
        protected Barcode _cachedBarcode = Barcode.EmptyBarcode();

#pragma warning disable CS0414
        [SerializeField] private bool _editorCrateChanged;
#pragma warning restore CS0414

        protected Crate CachedCrate
        {
            get
            {
                if (_cachedBarcode != _barcode || (_cachedCrate == null && Barcode.IsValid(_cachedBarcode)))
                {
                    _cachedCrate = null;
                    _cachedBarcode = Barcode.EmptyBarcode();
                }

                return _cachedCrate;
            }
            set
            {
                _cachedCrate = value;
                _cachedBarcode = _barcode;
            }
        }

        public Crate EditorCrate
        {
            get
            {
                if ((CachedCrate != null || string.IsNullOrEmpty(Barcode)) && CachedCrate.Barcode == Barcode)
                {
                    return CachedCrate;
                }

                return CachedCrate = FetchEditorCrate();
            }
        }

        public bool SetEditorCrate(Crate crate)
        {
            if (crate == null)
            {
                _barcode = Barcode.EmptyBarcode();
                CachedCrate = null;
                PalletBarcode = Barcode.EmptyBarcode();
                Version = Barcode.EmptyBarcode();
                _editorCrateChanged = true;
                return true;
            }

            if (CachedCrate != crate)
            {
                _barcode = new Barcode(crate.Barcode);
                CachedCrate = crate;
                if (CachedCrate.Pallet != null)
                {
                    PalletBarcode = CachedCrate.Pallet.Barcode;
                    Version = CachedCrate.Pallet.Version;
                }
                else
                {
                    PalletBarcode = Barcode.EmptyBarcode();
                    Version = Barcode.EmptyBarcode();
                }

            }

            _editorCrateChanged = true;
            return true;
        }

        public abstract Type CrateType { get; }

        protected virtual Crate FetchEditorCrate()
        {
            if (AssetWarehouse.ready && Barcode.IsValid(Barcode))
            {
                if (AssetWarehouse.Instance.TryGetCrate(Barcode, out var crate))
                {
                    return crate;
                }
            }
            return null;
        }

#endif

    }


    [Serializable]
    public class CrateReferenceT<T> : CrateReference where T : Crate
    {
        public CrateReferenceT() : base() { }
        public CrateReferenceT(string barcode) : base(barcode) { }


        public new T Crate
        {
            get
            {
                T retCrate = null;
                if (AssetWarehouse.ready)
                {
                    retCrate = AssetWarehouse.Instance.GetCrate<T>(Barcode);
                }
                return retCrate;
            }
        }

        public new bool TryGetCrate(out T crate)
        {
            crate = null;
            bool success = false;

            if (AssetWarehouse.ready)
            {
                success = AssetWarehouse.Instance.TryGetCrate(Barcode, out crate);
            }

            return success;
        }

#if UNITY_EDITOR
        public override Type CrateType
        {
            get { return typeof(T); }
        }

        public new T EditorCrate
        {
            get
            {
                if (CachedCrate as T != null || Barcode.IsValid(_cachedBarcode))
                {
                    return CachedCrate as T;
                }

                return (T)(CachedCrate = FetchEditorCrate());
            }
        }

        private new T FetchEditorCrate()
        {
            if (AssetWarehouse.ready && Barcode.IsValid(Barcode))
            {
                if (AssetWarehouse.Instance.TryGetCrate(Barcode, out var crate) && crate is T crateT)
                {
                    return crateT;
                }
            }
            return null;
        }
#endif
    }


    [Serializable]
    public class GenericCrateReference : CrateReferenceT<Crate>
    {
        public GenericCrateReference(string barcode) : base(barcode) { }
        public GenericCrateReference() : base(Barcode.EmptyBarcode()) { }
    }

    [Serializable]
    public class GameObjectCrateReference : CrateReferenceT<GameObjectCrate>
    {
        public GameObjectCrateReference(string barcode) : base(barcode) { }
        public GameObjectCrateReference() : base(Barcode.EmptyBarcode()) { }
    }

    [Serializable]
    public class SpawnableCrateReference : CrateReferenceT<SpawnableCrate>
    {
        public SpawnableCrateReference(string barcode) : base(barcode) { }
        public SpawnableCrateReference() : base(Barcode.EmptyBarcode()) { }
    }

    [Serializable]
    public class AvatarCrateReference : CrateReferenceT<AvatarCrate>
    {
        public AvatarCrateReference(string barcode) : base(barcode) { }
        public AvatarCrateReference() : base(Barcode.EmptyBarcode()) { }
    }

    [Serializable]
    public class LevelCrateReference : CrateReferenceT<LevelCrate>
    {
        public LevelCrateReference(string barcode) : base(barcode) { }
        public LevelCrateReference() : base(Barcode.EmptyBarcode()) { }
    }

    [Serializable]
    public class VFXCrateReference : CrateReferenceT<VFXCrate>
    {
        public VFXCrateReference(string barcode) : base(barcode) { }
        public VFXCrateReference() : base(Barcode.EmptyBarcode()) { }
    }

}