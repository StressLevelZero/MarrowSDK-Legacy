using UnityEngine;

namespace SLZ.Marrow.Data
{
	[CreateAssetMenu(fileName = "New Bullet Value", menuName = "Variables/Bullet Item", order = 3)]
	public class CartridgeData : ScriptableObject
	{
		public ProjectileData projectile;

		[Header("Dependencies")]
		public Spawnable cartridgeSpawnable;

		public Spawnable cartridgeCaseSpawnable;

		public CartridgeData()
			: base()
		{
		}
	}
}
