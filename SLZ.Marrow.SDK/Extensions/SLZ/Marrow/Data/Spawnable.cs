using System;
using SLZ.Marrow.Warehouse;
using UnityEngine;

namespace SLZ.Marrow.Data
{
	[Serializable]
	public struct Spawnable
	{
		[SerializeField]
		public SpawnableCrateReference crateRef;

		[SerializeField]
		public SpawnPolicyData policyData;

		public bool IsValid()
		{
			return default(bool);
		}
	}
}
