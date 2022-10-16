using System;
using UnityEngine;

namespace SLZ.Marrow.Data
{
	[Serializable]
	public struct LootItem
	{
		[Range(0f, 100f)]
		public float percentage;

		public Spawnable spawnable;
	}
}
