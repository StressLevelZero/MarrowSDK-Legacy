using System;
using System.Collections.Generic;
using UnityEngine;

namespace SLZ.Marrow.Warehouse
{
    [Serializable]
    public struct PackedAsset
    {
        public string title;
        public MarrowAsset marrowAsset;
        public List<PackedSubAsset> subAssets;
        public Type assetType;
        private string assetFieldName;

        public PackedAsset(string title, MarrowAsset marrowAsset, Type assetType, string assetFieldName)
        {
            this.title = title;
            this.marrowAsset = marrowAsset;
            this.subAssets = new List<PackedSubAsset>();
            this.assetFieldName = assetFieldName;
            this.assetType = assetType;
        }

        public PackedAsset(string title, List<PackedSubAsset> subAssets, Type assetType, string assetFieldName)
        {
            this.title = title;
            this.marrowAsset = null;
            this.subAssets = subAssets;
            this.assetType = assetType;
            this.assetFieldName = assetFieldName;
        }
    }

    [Serializable]
    public struct PackedSubAsset
    {
        public string subTitle;
        public MarrowAsset subAsset;

        public PackedSubAsset(string subTitle, MarrowAsset subAsset)
        {
            this.subTitle = subTitle;
            this.subAsset = subAsset;
        }
    }
}