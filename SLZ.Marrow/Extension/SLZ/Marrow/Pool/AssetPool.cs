using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using SLZ.Marrow.Warehouse;
using UnityEngine;

namespace SLZ.Marrow.Pool
{
	public class AssetPool
	{
		public SpawnableCrate _crate;

		private Transform _parentTransform;

		private ulong _spawnCount;

		public List<AssetPoolee> objects { get; private set; }

		public List<AssetPoolee> staged { get; private set; }

		public List<AssetPoolee> despawned { get; private set; }

		public List<AssetPoolee> spawned { get; private set; }

		public List<AssetPoolee> cleaned { get; private set; }

		public AssetPool(SpawnableCrate sc, Transform rootTransform = default(Transform))
			: base()
		{
		}

		public UniTask<AssetPoolee> Spawn(Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Vector3? scale = default(Vector3?), bool? autoEnable = default(bool?))
		{
			return default(UniTask<AssetPoolee>);
		}

		public void Clear(AssetPoolee poolee)
		{
		}

		public void StageForDespawn(AssetPoolee poolee)
		{
		}

		public void Despawn(AssetPoolee poolee)
		{
		}

		public void CleanupDespawn()
		{
		}
	}
}
