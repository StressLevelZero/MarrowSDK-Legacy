using UnityEngine;

namespace SLZ.Marrow.Pool
{
	public class SpawnEvents : MonoBehaviour
	{
		public GameObject parent;

		protected AssetPoolee _poolee;

		protected bool _hasPoolee;

		private bool _hasSpawn;

		private bool _hasPostSpawn;

		public ulong ID
		{
			get
			{
				return default(ulong);
			}
		}

		public bool IsDespawned
		{
			get
			{
				return default(bool);
			}
		}

		protected virtual void Start()
		{
		}

		public void Despawn()
		{
		}

		private void _OnSpawn(GameObject go)
		{
		}

		private void _OnPostSpawn(GameObject go)
		{
		}

		private void _OnDespawn(GameObject go)
		{
		}

		private void _OnPostDespawn(GameObject go)
		{
		}

		protected virtual void OnSpawn(GameObject go)
		{
		}

		protected virtual void OnPostSpawn(GameObject go)
		{
		}

		protected virtual void OnDespawn(GameObject go)
		{
		}

		protected virtual void OnPostDespawn(GameObject go)
		{
		}

		public SpawnEvents()
			: base()
		{
		}
	}
}
