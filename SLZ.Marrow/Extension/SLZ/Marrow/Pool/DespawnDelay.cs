using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine;

namespace SLZ.Marrow.Pool
{
	public class DespawnDelay : SpawnEvents
	{
		public float secondsUntilDisable;

		protected override void OnSpawn(GameObject go)
		{
		}

		private UniTaskVoid AsyncDespawn(int timeMs)
		{
			return default(UniTaskVoid);
		}

		public DespawnDelay()
			: base()
		{
		}
	}
}
