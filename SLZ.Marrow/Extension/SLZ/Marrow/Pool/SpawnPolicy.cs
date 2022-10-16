using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using SLZ.Marrow.Data;
using UnityEngine;

namespace SLZ.Marrow.Pool
{
	public class SpawnPolicy
	{
		protected List<AssetPoolee> _poolees;

		public virtual UniTask<AssetPoolee> Spawn(AssetPool pool, SpawnPolicyData policyData, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Vector3? scale = default(Vector3?), bool? autoEnable = default(bool?))
		{
			return default(UniTask<AssetPoolee>);
		}

		protected virtual UniTask<AssetPoolee> SpawnFromPool(AssetPool pool, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Vector3? scale = default(Vector3?))
		{
			return default(UniTask<AssetPoolee>);
		}

		public virtual bool Despawn(AssetPool pool, AssetPoolee poolee)
		{
			return default(bool);
		}

		public SpawnPolicy()
			: base()
		{
		}
	}
}
