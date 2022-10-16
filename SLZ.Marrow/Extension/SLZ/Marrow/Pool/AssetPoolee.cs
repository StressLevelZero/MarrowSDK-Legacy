using System;
using System.Runtime.CompilerServices;
using SLZ.Marrow.Utilities;
using SLZ.Marrow.Warehouse;
using UnityEngine;

namespace SLZ.Marrow.Pool
{
	public class AssetPoolee : MonoBehaviour
	{
		public SpawnableCrate spawnableCrate;

		public Action<GameObject> onSpawnDelegate;

		public Action<GameObject> onSpawnCallbackDelegate;

		public Action<GameObject> onPostSpawnDelegate;

		public Action<GameObject> onDespawnDelegate;

		public Action<GameObject> onDespawnCallbackDelegate;

		public Action<GameObject> onPostDespawnDelegate;

		public ulong ID { get; private set; }

		private void Awake()
		{
		}

		private void OnDestroy()
		{
		}

		public virtual void Despawn()
		{
		}

		public void StageForDespawn()
		{
		}

		public virtual void OnDespawn()
		{
		}

		public virtual void OnSpawn(ulong spawnId)
		{
		}

		public AssetPoolee()
			: base()
		{
		}
	}
}
