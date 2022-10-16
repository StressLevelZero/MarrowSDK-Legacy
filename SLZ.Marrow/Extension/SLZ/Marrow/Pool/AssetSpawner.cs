using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using SLZ.Marrow.Data;
using SLZ.Marrow.Warehouse;
using UnityEngine;

namespace SLZ.Marrow.Pool
{
	public class AssetSpawner : MonoBehaviour
	{
		private static AssetSpawner _instance;

		private Dictionary<Barcode, AssetPool> _barcodeToPool;

		private Dictionary<AssetPoolee, SpawnPolicy> _pooleeToPolicy;

		private Dictionary<ValueTuple<int, SpawnPolicyData, Barcode>, SpawnPolicy> _policySpawns;

		private List<AssetPool> _poolList;

		private void Awake()
		{
		}

		private void OnDestroy()
		{
		}

		private void LateUpdate()
		{
		}

		private static void Instantiate()
		{
		}

		public static void Register(Spawnable spawnable)
		{
		}

		public static UniTask RegisterAsync(Barcode barcode)
		{
			return default(UniTask);
		}

		public static void Spawn(Spawnable spawnable, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Vector3? scale = default(Vector3?), bool ignorePolicy = false, int? groupID = default(int?), Action<GameObject> spawnCallback = default(Action<GameObject>), Action<GameObject> despawnCallback = default(Action<GameObject>))
		{
		}

		public static UniTask<AssetPoolee> SpawnAsync(Spawnable spawnable, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Vector3? scale = default(Vector3?), bool ignorePolicy = false, int? groupID = default(int?), Action<GameObject> spawnCallback = default(Action<GameObject>), Action<GameObject> despawnCallback = default(Action<GameObject>))
		{
			return default(UniTask<AssetPoolee>);
		}

		private static UniTaskVoid SpawnCallbackFrameDelay(AssetPoolee poolee)
		{
			return default(UniTaskVoid);
		}

		public static void Clear(AssetPoolee poolee)
		{
		}

		public static void StageForDespawn(AssetPoolee poolee)
		{
		}

		public static void Despawn(AssetPoolee poolee)
		{
		}

		public AssetSpawner()
			: base()
		{
		}
	}
}
