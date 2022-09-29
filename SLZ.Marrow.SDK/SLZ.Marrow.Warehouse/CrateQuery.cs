using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SLZ.Marrow.Warehouse
{
    [Serializable]
    public class CrateQuery : CrateReference
    {
        [SerializeField] public string tagFilter;
        [SerializeField] public string titleFilter;

        private bool queryRan = false;

        Barcode tempBarcode = Barcode.EmptyBarcode();

#if UNITY_EDITOR
        public new Barcode Barcode
        {
            get
            {
                if (!queryRan)
                {
                    RunQuery();
                }

                return tempBarcode;
            }
            set { tempBarcode = value; }
        }

        private Type _crateType;
        public override Type CrateType => _crateType;
#endif


        public void RunQuery()
        {
            Barcode queryBarcode = Barcode.EmptyBarcode();
            if (AssetWarehouse.ready)
            {
                var crates = AssetWarehouse.Instance.GetCrates(new CrateQueryFilter(tagFilter, titleFilter));
                if (crates.Count > 0)
                {
                    if (crates.Count > 1)
                    {
                        queryBarcode = crates[Random.Range(0, crates.Count)].Barcode;
                    }
                    else
                    {
                        queryBarcode = crates[0].Barcode;
                    }
                }
            }

            if (Barcode.IsValid(queryBarcode))
            {
                tempBarcode = queryBarcode;
                queryRan = true;
            }
        }

        class CrateQueryFilter : ICrateFilter<Crate>
        {
            private readonly string tagFilter;
            private readonly string titleFilter;

            public CrateQueryFilter(string tagFilter, string titleFilter)
            {
                this.tagFilter = tagFilter;
                this.titleFilter = titleFilter;
            }

            public bool Filter(Crate crate)
            {
                return ((!string.IsNullOrEmpty(tagFilter) && crate.Tags.Contains(tagFilter, StringComparer.OrdinalIgnoreCase)))
                || ((!string.IsNullOrEmpty(titleFilter) && crate.Title.Contains(titleFilter, StringComparison.OrdinalIgnoreCase)));
            }
        }
    }
}
