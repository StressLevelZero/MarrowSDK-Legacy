using SLZ.Marrow.Pool;
using SLZ.Marrow.Utilities;
using UnityEngine;

namespace SLZ.Marrow.SceneStreaming
{
	public class ChunkTracker : SpawnEvents
	{

		private int _freezeCount;

		[SerializeField]
		private Rigidbody _rigidbody;

		[SerializeField]
		private ChunkTrackerGroup _parentGroup;

		public bool IsFrozen
		{
			get
			{
				return default(bool);
			}
		}

		private void Awake()
		{
		}

		private void OnDestroy()
		{
		}

		protected override void OnPostSpawn(GameObject go)
		{
		}

		protected override void OnPostDespawn(GameObject go)
		{
		}

		public void Freeze()
		{
		}

		public void Unfreeze()
		{
		}

		public ChunkTracker()
			: base()
		{
		}
	}
}
