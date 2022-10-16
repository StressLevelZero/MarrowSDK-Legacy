using UnityEngine;

namespace SLZ.Marrow.Data
{
	[CreateAssetMenu(fileName = "Spawn Policy Data", menuName = "StressLevelZero/Spawn Policy Data")]
	public class SpawnPolicyData : ScriptableObject
	{
		public enum PolicyRule
		{
			GROW = 0,
			REUSEOLDEST = 1,
			REUSENEWEST = 2,
			REUSENONE = 3
		}

		public int maxSize;

		public PolicyRule mode;

		public SpawnPolicyData()
			: base()
		{
		}
	}
}
