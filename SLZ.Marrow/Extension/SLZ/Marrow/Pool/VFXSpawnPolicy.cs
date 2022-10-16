using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using SLZ.Marrow.Data;
using UnityEngine;

namespace SLZ.Marrow.Pool
{
	public class VFXSpawnPolicy : SpawnPolicy
	{
		private double _timeOfLastSpawn;

		private AssetPoolee _lastSpawn;

		public override UniTask<AssetPoolee> Spawn(AssetPool pool, SpawnPolicyData policyData, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Vector3? scale = default(Vector3?), bool? autoEnable = default(bool?))
		{
			return default(UniTask<AssetPoolee>);
		}

		public VFXSpawnPolicy()
			: base()
		{
		}
	}
}
