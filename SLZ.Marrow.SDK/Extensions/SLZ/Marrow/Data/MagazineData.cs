using UnityEngine;

namespace SLZ.Marrow.Data
{
	[CreateAssetMenu(fileName = "New Magazine Value", menuName = "Variables/Magazine Item", order = 2)]
	public class MagazineData : ScriptableObject
	{
		public Spawnable spawnable;

		public string platform;

		public int rounds;

		public MagazineData()
			: base()
		{
		}
	}
}
