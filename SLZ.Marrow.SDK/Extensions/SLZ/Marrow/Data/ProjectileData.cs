using UnityEngine;

namespace SLZ.Marrow.Data
{
	[CreateAssetMenu(fileName = "New ProjectileData", menuName = "Variables/Projectile Data", order = 3)]
	public class ProjectileData : ScriptableObject
	{
		[Header("Dependencies")]
		public Spawnable spawnable;

		[Header("Physics Settings")]
		[Tooltip("Kilograms")]
		public float mass;

		[Tooltip("Meters per second")]
		public float startVelocity;

		[Header("Damage Settings")]
		public AttackType AttackType;

		[Tooltip("Multiplier to hit damage math")]
		public float damageMultiplier;

		[Header("Emission Settings")]
		[Tooltip("Number of copies spawned")]
		[Range(1f, 10f)]
		public int count;

		public EmissionType emissionType;

		[Range(0f, 180f)]
		[Tooltip("Spread angle in cone emission")]
		public float angle;

		[Tooltip("Start distance from center")]
		[Range(0f, 1f)]
		public float radius;

		public ProjectileData()
			: base()
		{
		}
	}
}
