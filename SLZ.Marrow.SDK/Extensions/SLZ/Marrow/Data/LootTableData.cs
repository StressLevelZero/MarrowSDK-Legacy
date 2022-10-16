using System;
using UnityEngine;

namespace SLZ.Marrow.Data
{
	[Serializable]
	[CreateAssetMenu(fileName = "LootTable", menuName = "Variables/LootTable", order = 10)]
	public class LootTableData : ScriptableObject
	{
		[SerializeField]
		public LootItem[] items;

		public Spawnable GetLootItem()
		{
			return default(Spawnable);
		}

		public LootTableData()
			: base()
		{
		}
	}
}
