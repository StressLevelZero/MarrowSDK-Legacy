using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SLZ.Marrow.Warehouse
{












    public class RuntimePallet : Pallet
    {



        public IResourceLocator catalog;

        public AsyncOperationHandle catalogOperationHandle;

        public string catalogPath;
    }
}