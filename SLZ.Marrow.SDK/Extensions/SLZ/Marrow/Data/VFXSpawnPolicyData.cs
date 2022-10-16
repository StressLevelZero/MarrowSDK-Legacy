using UnityEngine;

namespace SLZ.Marrow.Data
{
	[CreateAssetMenu(fileName = "VFX Spawn Policy Data", menuName = "StressLevelZero/VFX Spawn Policy Data")]
	public class VFXSpawnPolicyData : SpawnPolicyData
	{
		[Tooltip("Time in seconds between each spawn setting 0 will disable this feature")]
		public float spawnFrequency;

		[Tooltip("Allowable distance between each active spawn setting 0 will disable this feature")]
		public float spawnDistance;

		[Tooltip("Proximity to consider as stack")]
		public float spawnStackDistance;

		[Tooltip("Allowable spawns in a stack setting 0 will disable this feature")]
		public int spawnStackCount;

		public VFXSpawnPolicyData()
			: base()
		{
		}
	}
}
