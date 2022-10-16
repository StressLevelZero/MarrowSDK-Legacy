using System;
using SLZ.Marrow.Warehouse;
using UnityEngine;

namespace SLZ.Marrow.Data
{
	[Serializable]
	public struct VFXSpawnable
	{
		[SerializeField]
		public VFXCrateReference crateRef;

		[SerializeField]
		public VFXSpawnPolicyData policyData;

		public bool IsValid()
		{
			return default(bool);
		}
	}
}
